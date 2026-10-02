using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net;
using System.IO.Compression;
using NetToolsAV.Core;

namespace NetToolsAV.Infrastructure;

public static class CaptureArchive
{
    public static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    public static void Save(string directory, CaptureSnapshot snapshot)
    {
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "summary.json");
        File.WriteAllText(path + ".tmp", JsonSerializer.Serialize(snapshot, Json), Encoding.UTF8);
        File.Move(path + ".tmp", path, true);
    }
    public static void Seal(string directory)
    {
        var hashes = Directory.EnumerateFiles(directory, "*.pcapng").Select(path =>
        {
            using var stream = File.OpenRead(path);
            return new { File = Path.GetFileName(path), Bytes = stream.Length, SHA256 = Convert.ToHexString(SHA256.HashData(stream)) };
        }).ToArray();
        File.WriteAllText(Path.Combine(directory, "sha256.json"), JsonSerializer.Serialize(hashes, Json));
    }
    public static void Export(string directory, string destination)
    {
        var source = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var target = Path.GetFullPath(destination);
        if (target.StartsWith(source, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Escolha um destino fora da pasta da captura.");
        if (File.Exists(target)) throw new IOException("Destino já existe; escolha outro nome.");
        var temporary = target + "." + Guid.NewGuid().ToString("N") + ".partial";
        using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write))
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            foreach (var file in Directory.EnumerateFiles(source).Where(p => Path.GetExtension(p) == ".pcapng" || Path.GetFileName(p) is "summary.json" or "sha256.json"))
                archive.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Fastest);
        File.Move(temporary, target, false);
    }
    public static void Validate(CaptureSnapshot result, Guid session)
    {
        if (result.Schema != 1 || result.SessionId != session || result.Packets < 0 || result.Bytes < 0 || result.Truncated < 0 || result.Malformed < 0 || result.Uninterpreted < 0 || result.Protocols is null || result.Evidence is null || result.Files is null || result.Source is null || result.Filter is null || result.Limitations is null || result.Analyzer is null || result.Status is null)
            throw new InvalidDataException("Resumo incompatível ou inválido.");
        if (result.Protocols.Count > ProtocolCatalog.All.Count || result.Evidence.Count > 80 || result.Files.Count > 64 || result.Source.Length > 2048 || result.Filter.Length > 1024 || result.Limitations.Length > 16384)
            throw new InvalidDataException("Resumo excede limites de leitura.");
        if (result.Protocols.Any(p => p is null || p.Packets < 0 || p.Packets > result.Packets || !TrafficAnalyzer.Supported.Contains(p.Id)) || result.Protocols.Select(p => p.Id).Distinct().Count() != result.Protocols.Count)
            throw new InvalidDataException("Contagem ou protocolo inválido no resumo.");
        if (result.Evidence.Any(p => p is null || p.Number < 1 || p.Number > result.Packets || !TrafficAnalyzer.Supported.Contains(p.Protocol) || p.Source is null || p.Destination is null || p.Detail is null || p.Source.Length > 2048 || p.Destination.Length > 2048 || p.Detail.Length > 4096))
            throw new InvalidDataException("Evidência inválida no resumo.");
        if (result.Files.Any(f => string.IsNullOrWhiteSpace(f) || Path.GetFileName(f) != f)) throw new InvalidDataException("Referência de arquivo inválida.");
    }
    public static CaptureSnapshot? Latest(string root, Guid session)
    {
        var directory = Path.Combine(root, "captures", session.ToString("N"));
        if (!Directory.Exists(directory)) return null;
        var path = Directory.EnumerateFiles(directory, "summary.json", SearchOption.AllDirectories).OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
        if (path is null) return null;
        if (new FileInfo(path).Length > 1024 * 1024) throw new InvalidDataException("Resumo de captura acima do limite de leitura.");
        var result = JsonSerializer.Deserialize<CaptureSnapshot>(File.ReadAllText(path)) ?? throw new InvalidDataException("Resumo vazio.");
        Validate(result, session);
        return result.Status is "Capturando" or "Preparando" ? result with { Status = "Interrompida", Error = "Encerramento não confirmado. Contagens refletem o último resumo gravado." } : result;
    }
    public static string NewDirectory(string root, Guid session) => Path.Combine(root, "captures", session.ToString("N"), DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N"));
    public static CaptureSnapshot Import(string source, string directory, Guid session, CancellationToken cancellation)
    {
        if (new FileInfo(source).Length > PcapNgReader.MaxFileSize) throw new InvalidDataException("Arquivo excede 512 MiB.");
        Directory.CreateDirectory(directory); var target = Path.Combine(directory, "original.pcapng");
        File.Copy(source, target, false);
        var snapshot = PcapNgReader.Analyze(target, session, cancellation) with { Source = Path.GetFileName(source) };
        Save(directory, snapshot); Seal(directory); return snapshot;
    }
    public static string Report(AnalysisSession session, CaptureSnapshot? snapshot, string format)
    {
        var text = new StringBuilder(SessionReport.Create(session, "Ver contexto de captura abaixo"));
        if (snapshot is not null)
        {
            text.Clear(); text.AppendLine("NETTOOLS AV — OBSERVAÇÕES POR PROTOCOLO");
            text.AppendLine($"Sessão: {session.Name}\nPonto: {session.Location}\nOrigem: {snapshot.Source}\nEstado: {snapshot.Status}\nFiltro BPF: {snapshot.Filter}\nJanela UTC: {snapshot.StartedUtc:O} a {snapshot.UpdatedUtc:O}\nAnalisador: {snapshot.Analyzer}");
            text.AppendLine($"Pacotes: {snapshot.Packets}; bytes capturados: {snapshot.Bytes}; truncados: {snapshot.Truncated}; estrutura inválida/incompleta: {snapshot.Malformed}; sem interpretação: {snapshot.Uninterpreted}");
            text.AppendLine($"Npcap recebidos: {snapshot.DriverReceived?.ToString() ?? "indisponível"}; descartes do driver: {snapshot.DriverDropped?.ToString() ?? "indisponível"}. Descartes da interface e anteriores ao ponto de captura: indisponíveis.");
            foreach (var p in ProtocolCatalog.All.Where(p => session.ProtocolIds.Contains(p.Id) || snapshot.Protocols.Any(c => c.Id == p.Id)))
            {
                var view = new ProtocolView(p, snapshot);
                text.AppendLine($"\n{p.Name}: {view.Presence}; {view.Packets} pacotes; {view.State}\n{p.Limitation}");
            }
            text.AppendLine($"\n{snapshot.Limitations}\n{snapshot.Error}");
            text.AppendLine("Arquivos: " + string.Join(", ", snapshot.Files));
        }
        return format switch
        {
            "json" => JsonSerializer.Serialize(new { Session = new { session.Id, session.Name, session.Location, session.ProtocolIds }, Capture = snapshot }, Json),
            "html" => "<!doctype html><html lang=\"pt-BR\"><meta charset=\"utf-8\"><title>NetTools AV</title><style>body{font:15px system-ui;background:#f8f8f6;color:#292a28;margin:40px;max-width:1000px}pre{white-space:pre-wrap;line-height:1.6}</style><h1>NetTools AV</h1><pre>" + WebUtility.HtmlEncode(text.ToString()) + "</pre></html>",
            _ => text.ToString()
        };
    }
}
