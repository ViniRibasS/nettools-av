using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NetToolsAV.Core;

namespace NetToolsAV.Infrastructure;

public sealed record CaptureDevice(string Name, string Description);
public sealed record CaptureRequest(Guid SessionId, string InterfaceId, string Directory, string Filter = "", bool Promiscuous = false, int DurationSeconds = 900, int LimitMiB = 256);

public static class NpcapCapture
{
    static NpcapCapture()
    {
        NativeLibrary.SetDllImportResolver(typeof(NpcapCapture).Assembly, (name, assembly, _) => name == "wpcap.dll"
            ? NativeLibrary.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Npcap", name), assembly,
                DllImportSearchPath.UseDllDirectoryForDependencies | DllImportSearchPath.System32) : IntPtr.Zero);
    }
    public static IReadOnlyList<CaptureDevice> Devices()
    {
        var error = new StringBuilder(256);
        if (Native.pcap_findalldevs(out var head, error) != 0) throw new IOException(error.ToString());
        try
        {
            var devices = new List<CaptureDevice>();
            for (var p = head; p != IntPtr.Zero;)
            {
                var d = Marshal.PtrToStructure<Device>(p);
                devices.Add(new(Marshal.PtrToStringAnsi(d.Name) ?? "", Marshal.PtrToStringAnsi(d.Description) ?? "")); p = d.Next;
            }
            return devices;
        }
        finally { Native.pcap_freealldevs(head); }
    }

    public static CaptureSnapshot Run(CaptureRequest request, Action<CaptureSnapshot> progress, CancellationToken cancellation)
    {
        if (!Guid.TryParse(request.InterfaceId, out var nic)) throw new ArgumentException("Interface Windows inválida.");
        if (request.DurationSeconds is < 1 or > 3600 || request.LimitMiB is < 1 or > 1024 || request.Filter.Length > 1024 || request.Filter.Any(char.IsControl))
            throw new ArgumentException("Limites ou filtro inválidos.");
        ValidateFilter(request.Filter);
        var device = Devices().SingleOrDefault(d => string.Equals(d.Name, @"\Device\NPF_" + nic.ToString("B"), StringComparison.OrdinalIgnoreCase))
            ?? throw new IOException("Interface selecionada não encontrada no Npcap. Nenhuma outra interface foi usada.");
        var error = new StringBuilder(256); var handle = Native.pcap_open_live(device.Name, 65535, request.Promiscuous ? 1 : 0, 100, error);
        if (handle == IntPtr.Zero) throw new IOException("Npcap não abriu a interface: " + error + ". Verifique driver e permissões; a aplicação não eleva privilégios automaticamente.");
        var analyzer = new TrafficAnalyzer(); var files = new List<string>(); PcapNgWriter? writer = null;
        var context = new CaptureSnapshot { SessionId = request.SessionId, Source = device.Name, Filter = request.Filter, Status = "Capturando" };
        var clock = Stopwatch.StartNew(); var nextUpdate = TimeSpan.Zero; long written = 0;
        try
        {
            if (Native.pcap_datalink(handle) != 1) throw new NotSupportedException("Captura ao vivo suporta somente Ethernet (DLT_EN10MB). Não foi gravado um link type incorreto.");
            if (Native.pcap_setnonblock(handle, 1, error) != 0) throw new IOException(error.ToString());
            if (request.Filter.Length > 0)
            {
                if (Native.pcap_compile(handle, out var filter, request.Filter, 1, uint.MaxValue) != 0) throw new ArgumentException("Filtro BPF inválido: " + Error(handle));
                try { if (Native.pcap_setfilter(handle, ref filter) != 0) throw new IOException(Error(handle)); }
                finally { Native.pcap_freecode(ref filter); }
            }
            Directory.CreateDirectory(request.Directory);
            NextFile();
            while (!cancellation.IsCancellationRequested && clock.Elapsed.TotalSeconds < request.DurationSeconds && written < request.LimitMiB * 1024L * 1024)
            {
                var result = Native.pcap_next_ex(handle, out var headerPointer, out var dataPointer);
                if (result == 1)
                {
                    var header = Marshal.PtrToStructure<Header>(headerPointer);
                    if (header.Captured > 65535 || header.Captured > header.Original || header.Microseconds is < 0 or >= 1_000_000) throw new InvalidDataException("Cabeçalho Npcap inválido.");
                    var data = new byte[header.Captured]; Marshal.Copy(dataPointer, data, 0, data.Length);
                    var time = DateTimeOffset.FromUnixTimeSeconds(header.Seconds).AddTicks(header.Microseconds * 10L);
                    if (written + data.Length + 84 > request.LimitMiB * 1024L * 1024) break;
                    if (writer!.Length + data.Length + 36 > 32 * 1024 * 1024) NextFile();
                    writer!.Write(data, header.Original, time); written += data.Length + 36;
                    analyzer.Add(data, header.Original, time);
                }
                else if (result == 0) cancellation.WaitHandle.WaitOne(10);
                else throw new IOException("Leitura Npcap interrompida: " + Error(handle));
                if (clock.Elapsed >= nextUpdate)
                {
                    writer!.Flush(); progress(Current("Capturando")); nextUpdate = clock.Elapsed + TimeSpan.FromSeconds(1);
                }
            }
            writer!.Flush();
            return Current(cancellation.IsCancellationRequested ? "Concluída" : "Limite atingido");
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or NotSupportedException or InvalidOperationException)
        {
            return Current("Falha") with { Error = ex.Message };
        }
        finally { try { writer?.Dispose(); } finally { Native.pcap_close(handle); } }

        void NextFile()
        {
            writer?.Dispose(); var file = $"capture-{files.Count + 1:000}.pcapng";
            writer = new(Path.Combine(request.Directory, file)); files.Add(file);
            written += writer.Length;
        }
        CaptureSnapshot Current(string status)
        {
            uint? received = null, dropped = null; var stats = Marshal.AllocHGlobal(128);
            try { if (Native.pcap_stats(handle, stats) == 0) { received = unchecked((uint)Marshal.ReadInt32(stats)); dropped = unchecked((uint)Marshal.ReadInt32(stats, 4)); } }
            finally { Marshal.FreeHGlobal(stats); }
            return analyzer.Snapshot(context with { Status = status, DriverReceived = received, DriverDropped = dropped, Files = files.ToArray() });
        }
    }
    private static string Error(IntPtr handle) => Marshal.PtrToStringAnsi(Native.pcap_geterr(handle)) ?? "Erro não informado.";
    public static void ValidateFilter(string text)
    {
        if (text.Length == 0) return;
        var handle = Native.pcap_open_dead(1, 65535);
        if (handle == IntPtr.Zero) throw new IOException("Npcap não criou o contexto de validação do filtro.");
        try
        {
            if (Native.pcap_compile(handle, out var filter, text, 1, uint.MaxValue) != 0) throw new ArgumentException("Filtro BPF inválido: " + Error(handle));
            Native.pcap_freecode(ref filter);
        }
        finally { Native.pcap_close(handle); }
    }
    [StructLayout(LayoutKind.Sequential)] private struct Device { public IntPtr Next, Name, Description, Addresses; public uint Flags; }
    // Windows ABI: timeval uses two 32-bit longs, including x64.
    [StructLayout(LayoutKind.Sequential)] private struct Header { public int Seconds, Microseconds; public uint Captured, Original; }
    [StructLayout(LayoutKind.Sequential)] private struct Filter { public uint Length; public IntPtr Instructions; }
    private static class Native
    {
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr pcap_open_dead(int link, int snaplen);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern int pcap_findalldevs(out IntPtr devices, StringBuilder error);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern void pcap_freealldevs(IntPtr devices);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)] internal static extern IntPtr pcap_open_live(string name, int snaplen, int promiscuous, int timeout, StringBuilder error);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern int pcap_datalink(IntPtr handle);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern int pcap_setnonblock(IntPtr handle, int enabled, StringBuilder error);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)] internal static extern int pcap_compile(IntPtr handle, out Filter filter, string text, int optimize, uint mask);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern int pcap_setfilter(IntPtr handle, ref Filter filter);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern void pcap_freecode(ref Filter filter);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern int pcap_next_ex(IntPtr handle, out IntPtr header, out IntPtr packet);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern IntPtr pcap_geterr(IntPtr handle);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern int pcap_stats(IntPtr handle, IntPtr stats);
        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)] internal static extern void pcap_close(IntPtr handle);
    }
}
