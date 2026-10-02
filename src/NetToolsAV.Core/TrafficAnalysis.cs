using System.Buffers.Binary;
using System.Net;

namespace NetToolsAV.Core;

public sealed record PacketEvidence(long Number, DateTimeOffset Time, string Protocol, string Source, string Destination, string Detail);
public sealed record ProtocolCount(string Id, long Packets);
public sealed record CaptureSnapshot
{
    public int Schema { get; init; } = 1;
    public string Analyzer { get; init; } = "headers-0.2";
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid SessionId { get; init; }
    public string Source { get; init; } = "";
    public string Filter { get; init; } = "";
    public string Status { get; init; } = "Preparando";
    public string? Error { get; init; }
    public DateTimeOffset StartedUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; init; } = DateTimeOffset.UtcNow;
    public long Packets { get; init; }
    public long Bytes { get; init; }
    public long Truncated { get; init; }
    public long Uninterpreted { get; init; }
    public long Malformed { get; init; }
    public uint? DriverReceived { get; init; }
    public uint? DriverDropped { get; init; }
    public IReadOnlyList<ProtocolCount> Protocols { get; init; } = [];
    public IReadOnlyList<PacketEvidence> Evidence { get; init; } = [];
    public IReadOnlyList<string> Files { get; init; } = [];
    public string Limitations { get; init; } = "Inspeção de cabeçalhos; checksums e entrega não validados. Contagens de camadas se sobrepõem. Amostra de até 80 evidências; não é inventário de equipamentos.";
}

public sealed class TrafficAnalyzer
{
    public static readonly IReadOnlySet<string> Supported = new HashSet<string>(["arp", "icmp", "tcp", "udp", "dns", "mdns", "igmp2", "igmp3", "ptp2", "ndp"], StringComparer.Ordinal);
    private readonly Dictionary<string, long> counts = [];
    private readonly Queue<PacketEvidence> evidence = [];
    public long Packets { get; private set; }
    public long Bytes { get; private set; }
    public long Truncated { get; private set; }
    public long Uninterpreted { get; private set; }
    public long Malformed { get; private set; }

    public CaptureSnapshot Snapshot(CaptureSnapshot context) => context with
    {
        UpdatedUtc = DateTimeOffset.UtcNow, Packets = Packets, Bytes = Bytes, Truncated = Truncated,
        Uninterpreted = Uninterpreted, Malformed = Malformed,
        Protocols = counts.OrderBy(c => c.Key).Select(c => new ProtocolCount(c.Key, c.Value)).ToArray(), Evidence = evidence.ToArray()
    };

    public void Add(ReadOnlySpan<byte> frame, uint originalLength, DateTimeOffset time, uint linkType = 1)
    {
        Packets++; Bytes += frame.Length;
        if (frame.Length < originalLength) Truncated++;
        var seen = new HashSet<string>();
        try
        {
            if (linkType != 1) { Uninterpreted++; return; }
            Need(frame, 14);
            var type = U16(frame, 12); var offset = 14;
            for (var tags = 0; type is 0x8100 or 0x88a8; tags++)
            {
                if (tags == 4) { Uninterpreted++; return; }
                Need(frame, offset + 4); type = U16(frame, offset + 2); offset += 4;
            }
            var data = frame[offset..];
            if (type == 0x0806)
            {
                Need(data, 8); Need(data, 8 + 2 * (data[4] + data[5]));
                if (U16(data, 0) == 1 && U16(data, 2) == 0x0800 && data[4] == 6 && data[5] == 4 && U16(data, 6) is 1 or 2)
                    Observe("arp", new IPAddress(data.Slice(14, 4)).ToString(), new IPAddress(data.Slice(24, 4)).ToString(), $"Opcode {U16(data, 6)}; Ethernet/IPv4");
            }
            else if (type == 0x88f7) Ptp(data, "Ethernet", "Ethernet");
            else if (type == 0x0800)
            {
                Need(data, 20); var ihl = (data[0] & 15) * 4; var total = U16(data, 2);
                if (data[0] >> 4 != 4 || ihl < 20 || total < ihl) throw new InvalidDataException();
                Need(data, total); var ip = data[..total];
                if ((U16(ip, 6) & 0x3fff) != 0) { Uninterpreted++; return; }
                Transport(ip[ihl..], ip[9], new IPAddress(ip.Slice(12, 4)).ToString(), new IPAddress(ip.Slice(16, 4)).ToString(), false);
            }
            else if (type == 0x86dd)
            {
                Need(data, 40); if (data[0] >> 4 != 6) throw new InvalidDataException();
                var length = U16(data, 4); Need(data, 40 + length);
                // Extension headers and fragments deliberately remain uninterpreted in this profile.
                Transport(data.Slice(40, length), data[6], new IPAddress(data.Slice(8, 16)).ToString(), new IPAddress(data.Slice(24, 16)).ToString(), true);
            }
            if (seen.Count == 0) Uninterpreted++;
        }
        catch (InvalidDataException) { Malformed++; }

        void Observe(string id, string source, string destination, string detail)
        {
            if (!seen.Add(id)) return;
            counts[id] = counts.GetValueOrDefault(id) + 1;
            if (evidence.Count == 80) evidence.Dequeue();
            evidence.Enqueue(new(Packets, time, id, source, destination, detail));
        }
        void Ptp(ReadOnlySpan<byte> p, string source, string destination)
        {
            Need(p, 34);
            if ((p[1] & 15) != 2) return;
            var length = U16(p, 2); var message = p[0] & 15;
            var minimum = message switch { 0 or 1 or 8 => 44, 2 or 3 or 9 or 10 => 54, 11 => 64, 12 => 44, 13 => 48, _ => int.MaxValue };
            if (length < minimum || length > p.Length) throw new InvalidDataException();
            Observe("ptp2", source, destination, $"Mensagem {message}; domínio {p[4]}; sequência {U16(p, 30)}. Sem medição de precisão.");
        }
        void Transport(ReadOnlySpan<byte> p, int protocol, string source, string destination, bool ipv6)
        {
            if (protocol == 1 && !ipv6) { Need(p, 8); Observe("icmp", source, destination, $"Tipo {p[0]}; código {p[1]}. Sem correlação RTT."); }
            else if (protocol == 58 && ipv6) { Need(p, 8); Observe("ndp", source, destination, $"ICMPv6 tipo {p[0]}; código {p[1]}. NDP sem validação semântica."); }
            else if (protocol == 2 && !ipv6)
            {
                Need(p, 8);
                if (p[0] == 0x22)
                {
                    var cursor = 8; var records = U16(p, 6);
                    for (var n = 0; n < records; n++) { Need(p, cursor + 8); var size = 8 + 4 * U16(p, cursor + 2) + 4 * p[cursor + 1]; Need(p, cursor + size); cursor += size; }
                    Observe("igmp3", source, destination, $"Report v3; {records} registros.");
                }
                else if (p[0] == 0x11 && p.Length >= 12) { Need(p, 12 + 4 * U16(p, 10)); Observe("igmp3", source, destination, "Query v3."); }
                else if (p[0] is 0x16 or 0x17 || p[0] == 0x11 && p.Length == 8 && p[1] != 0)
                    Observe("igmp2", source, destination, $"Tipo 0x{p[0]:X2}; grupo {new IPAddress(p.Slice(4, 4))}.");
            }
            else if (protocol == 6)
            {
                Need(p, 20); var header = (p[12] >> 4) * 4;
                if (header < 20) throw new InvalidDataException(); Need(p, header);
                Observe("tcp", $"{source}:{U16(p, 0)}", $"{destination}:{U16(p, 2)}", $"Flags 0x{p[13]:X2}; cabeçalho {header} bytes. Sem remontagem.");
            }
            else if (protocol == 17)
            {
                Need(p, 8); var length = U16(p, 4); if (length < 8) throw new InvalidDataException(); Need(p, length);
                var src = U16(p, 0); var dst = U16(p, 2); var body = p.Slice(8, length - 8);
                Observe("udp", $"{source}:{src}", $"{destination}:{dst}", $"Datagrama {length} bytes.");
                if (src is 319 or 320 || dst is 319 or 320) Ptp(body, source, destination);
                else if (src is 53 or 5353 || dst is 53 or 5353)
                {
                    ValidateDns(body);
                    var id = src == 5353 || dst == 5353 ? "mdns" : "dns";
                    Observe(id, source, destination, $"ID {U16(body, 0)}; {(body[2] >> 7 == 1 ? "resposta" : "consulta")}; {U16(body, 4)} perguntas; {U16(body, 6)} respostas. Conteúdo dos registros não interpretado.");
                }
            }
        }
    }

    private static void ValidateDns(ReadOnlySpan<byte> p)
    {
        Need(p, 12); var q = U16(p, 4); var r = U16(p, 6) + U16(p, 8) + U16(p, 10);
        if (q + r > 4096 || (p[2] & 0x78) != 0) throw new InvalidDataException();
        var at = 12;
        for (var i = 0; i < q + r; i++)
        {
            SkipName(p, ref at); Need(p, at + (i < q ? 4 : 10));
            if (i < q) at += 4;
            else { var length = U16(p, at + 8); at += 10; Need(p, at + length); at += length; }
        }
        if (at != p.Length) throw new InvalidDataException();
    }
    private static void SkipName(ReadOnlySpan<byte> p, ref int cursor)
    {
        var pos = cursor; var end = -1; var hops = 0; var length = 0;
        while (true)
        {
            Need(p, pos + 1); if (++hops > 128) throw new InvalidDataException(); var n = p[pos++];
            if (n == 0) { cursor = end < 0 ? pos : end; return; }
            if ((n & 0xc0) == 0xc0)
            {
                Need(p, pos + 1); var target = ((n & 0x3f) << 8) | p[pos++];
                if (target >= pos - 2) throw new InvalidDataException();
                if (end < 0) end = pos; pos = target;
            }
            else { if (n > 63 || (length += n + 1) > 255) throw new InvalidDataException(); Need(p, pos + n); pos += n; }
        }
    }
    private static int U16(ReadOnlySpan<byte> data, int at) => BinaryPrimitives.ReadUInt16BigEndian(data.Slice(at, 2));
    private static void Need(ReadOnlySpan<byte> data, int length) { if (length > data.Length) throw new InvalidDataException("Mensagem truncada ou estrutura inválida."); }
}

public sealed record ProtocolView(ProtocolDefinition Definition, CaptureSnapshot? Capture)
{
    public string Id => Definition.Id;
    public string Name => Definition.Name;
    public string Family => Definition.Family;
    public string Version => Definition.Version;
    public string Transport => Definition.Transport;
    public string Limitation => Definition.Limitation;
    public long Count => Capture?.Protocols.FirstOrDefault(p => p.Id == Id)?.Packets ?? 0;
    public string Packets => Capture is null || !TrafficAnalyzer.Supported.Contains(Id) ? "—" : Count.ToString("N0");
    public string Presence => Capture is null || Capture.Status == "Preparando" || !TrafficAnalyzer.Supported.Contains(Id) ? "Indisponível" : Count > 0 ? "Observado" : Capture.Status is "Falha" or "Interrompida" ? "Inconclusivo" : "Não observado";
    public string State => Capture is null || Capture.Status == "Preparando" ? "Não executado" : !TrafficAnalyzer.Supported.Contains(Id) ? "Sem analisador" : Capture.Status is "Falha" or "Interrompida" ? "Inconclusivo" : "Inspeção parcial";
}
