using System.Buffers.Binary;
using NetToolsAV.Core;

namespace NetToolsAV.Infrastructure;

public sealed class PcapNgWriter : IDisposable
{
    private readonly FileStream stream;
    private readonly BinaryWriter writer;
    public long Length => stream.Position;
    public PcapNgWriter(string path)
    {
        stream = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        writer = new(stream);
        Block(0x0a0d0d0a, b => { b.Write(0x1a2b3c4d); b.Write((ushort)1); b.Write((ushort)0); b.Write(-1L); });
        Block(1, b => { b.Write((ushort)1); b.Write((ushort)0); b.Write(65535u); });
        Flush();
    }
    public void Write(ReadOnlySpan<byte> data, uint original, DateTimeOffset timestamp)
    {
        if (data.Length > 65535 || original < data.Length) throw new InvalidDataException("Comprimento de pacote inválido.");
        var micro = checked((ulong)((timestamp.UtcTicks - DateTimeOffset.UnixEpoch.UtcTicks) / 10));
        var padded = (data.Length + 3) & ~3; var length = 32 + padded;
        writer.Write(6u); writer.Write((uint)length); writer.Write(0u); writer.Write((uint)(micro >> 32)); writer.Write((uint)micro);
        writer.Write((uint)data.Length); writer.Write(original); writer.Write(data);
        for (var i = data.Length; i < padded; i++) writer.Write((byte)0);
        writer.Write((uint)length);
    }
    private void Block(uint type, Action<BinaryWriter> body)
    {
        using var memory = new MemoryStream(); using var b = new BinaryWriter(memory); body(b);
        var length = checked((uint)memory.Length + 12); writer.Write(type); writer.Write(length); writer.Write(memory.ToArray()); writer.Write(length);
    }
    public void Flush() { writer.Flush(); stream.Flush(true); }
    public void Dispose() { try { writer.Dispose(); } finally { stream.Dispose(); } }
}

public static class PcapNgReader
{
    public const long MaxFileSize = 512L * 1024 * 1024;
    private sealed record Interface(uint Link, uint Snap, byte Resolution, long Offset);

    public static CaptureSnapshot Analyze(string path, Guid session, CancellationToken cancellation = default)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length > MaxFileSize) throw new InvalidDataException("Limite de importação: 512 MiB por arquivo.");
        var analyzer = new TrafficAnalyzer(); var interfaces = new List<Interface>();
        var little = true; var section = false; var skipped = 0; var unsupported = 0;
        var header = new byte[8]; var first = (DateTimeOffset?)null; var last = (DateTimeOffset?)null;
        while (stream.Position < stream.Length)
        {
            cancellation.ThrowIfCancellationRequested(); stream.ReadExactly(header);
            var shb = header.AsSpan(0, 4).SequenceEqual(new byte[] { 10, 13, 13, 10 });
            byte[]? magic = null;
            if (shb)
            {
                magic = new byte[4]; stream.ReadExactly(magic);
                little = magic.AsSpan().SequenceEqual(new byte[] { 0x4d, 0x3c, 0x2b, 0x1a });
                if (!little && !magic.AsSpan().SequenceEqual(new byte[] { 0x1a, 0x2b, 0x3c, 0x4d })) throw new InvalidDataException("Byte-order magic inválido.");
            }
            else if (!section) throw new InvalidDataException("Arquivo não é PCAPNG: seção inicial ausente.");
            uint U32(ReadOnlySpan<byte> p) => little ? BinaryPrimitives.ReadUInt32LittleEndian(p) : BinaryPrimitives.ReadUInt32BigEndian(p);
            ushort U16(ReadOnlySpan<byte> p) => little ? BinaryPrimitives.ReadUInt16LittleEndian(p) : BinaryPrimitives.ReadUInt16BigEndian(p);
            var type = U32(header); var length = U32(header.AsSpan(4));
            if (length < (shb ? 28 : 12) || length > 4 * 1024 * 1024 || length % 4 != 0) throw new InvalidDataException("Tamanho de bloco PCAPNG inválido ou acima de 4 MiB.");
            var block = new byte[checked((int)length - 8)]; var prefix = shb ? 4 : 0;
            magic?.CopyTo(block, 0); stream.ReadExactly(block.AsSpan(prefix));
            if (U32(block.AsSpan(block.Length - 4)) != length) throw new InvalidDataException("Comprimentos inicial/final do bloco divergem.");
            var body = block.AsSpan(0, block.Length - 4);
            if (shb)
            {
                if (U16(body[4..]) != 1 || U16(body[6..]) != 0) throw new InvalidDataException("Versão de seção PCAPNG não suportada.");
                interfaces.Clear(); section = true;
            }
            else if (type == 1)
            {
                if (body.Length < 8 || interfaces.Count >= 1024) throw new InvalidDataException("IDB inválido ou interfaces demais.");
                byte resolution = 6; long offset = 0;
                for (var at = 8; at < body.Length;)
                {
                    if (body.Length - at < 4) throw new InvalidDataException("Opção incompleta.");
                    var code = U16(body[at..]); var size = U16(body[(at + 2)..]); at += 4;
                    if (size > body.Length - at) throw new InvalidDataException("Opção fora do bloco.");
                    if (code == 0) { if (size != 0) throw new InvalidDataException("Fim de opções inválido."); break; }
                    if (code == 9) { if (size != 1) throw new InvalidDataException("if_tsresol inválido."); resolution = body[at]; }
                    if (code == 14) { if (size != 8) throw new InvalidDataException("if_tsoffset inválido."); offset = little ? BinaryPrimitives.ReadInt64LittleEndian(body[at..]) : BinaryPrimitives.ReadInt64BigEndian(body[at..]); }
                    at += (size + 3) & ~3;
                    if (at > body.Length) throw new InvalidDataException("Padding inválido.");
                }
                interfaces.Add(new(U16(body), U32(body[4..]), resolution, offset));
            }
            else if (type == 6)
            {
                if (body.Length < 20) throw new InvalidDataException("EPB incompleto.");
                var id = U32(body); var cap = U32(body[12..]); var original = U32(body[16..]);
                if (id >= interfaces.Count || cap > 65535 || cap > original || ((cap + 3) & ~3u) > body.Length - 20) throw new InvalidDataException("EPB com interface/comprimento inválido.");
                var nic = interfaces[(int)id]; if (nic.Snap > 0 && cap > nic.Snap) throw new InvalidDataException("Pacote excede snaplen.");
                var raw = ((ulong)U32(body[4..]) << 32) | U32(body[8..]);
                var exponent = nic.Resolution & 0x7f;
                if (exponent > 63) throw new InvalidDataException("Resolução temporal não suportada.");
                var divisor = Math.Pow((nic.Resolution & 128) != 0 ? 2 : 10, exponent);
                var seconds = raw / divisor + nic.Offset;
                if (!double.IsFinite(seconds) || seconds < -62135596800d || seconds > 253402300798d) throw new InvalidDataException("Timestamp fora do intervalo suportado.");
                var time = DateTimeOffset.UnixEpoch.AddSeconds(seconds); first ??= time; last = time;
                if (nic.Link != 1) unsupported++;
                analyzer.Add(body.Slice(20, (int)cap), original, time, nic.Link);
            }
            else if (type is 2 or 3) throw new InvalidDataException("Bloco de pacote PB/SPB não suportado neste perfil; original preservado.");
            else skipped++;
        }
        if (!section) throw new InvalidDataException("Arquivo vazio.");
        var result = analyzer.Snapshot(new() { SessionId = session, Source = Path.GetFileName(path), Status = "Importado", StartedUtc = first ?? DateTimeOffset.UtcNow, Files = [Path.GetFileName(path)] });
        return result with { UpdatedUtc = last ?? result.UpdatedUtc, Limitations = result.Limitations + $" Importação: {skipped} blocos auxiliares não interpretados; {unsupported} pacotes com link type não suportado. Metadados originais permanecem no arquivo; estatísticas de descarte não importadas." };
    }
}
