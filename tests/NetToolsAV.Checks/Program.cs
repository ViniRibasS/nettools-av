using Microsoft.Data.Sqlite;
using NetToolsAV.Core;
using NetToolsAV.Infrastructure;

if (args.Contains("--npcap-list"))
{
    foreach (var device in NpcapCapture.Devices()) Console.WriteLine($"{device.Name} | {device.Description}");
    return;
}
if (args.Contains("--npcap-check"))
{
    NpcapCapture.ValidateFilter("udp port 5353 or arp");
    try { NpcapCapture.ValidateFilter("udp and ( invalid"); throw new Exception("Filtro inválido foi aceito."); }
    catch (ArgumentException) { Console.WriteLine("PASS: filtros BPF válido/inválido verificados offline pela biblioteca Npcap; nenhuma interface aberta."); }
    return;
}
if (args.Length == 2 && args[0] == "--analyze")
{
    var result = PcapNgReader.Analyze(args[1], Guid.NewGuid());
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, CaptureArchive.Json)); return;
}
var root = Path.GetFullPath(args.FirstOrDefault() ?? Path.Combine("artifacts", "checks", Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(root);
var passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    passed++;
}
void Reject<T>(Action action, string name) where T : Exception
{
    try { action(); }
    catch (T) { Check(true, name); return; }
    throw new InvalidOperationException("FAIL (não rejeitado): " + name);
}

Check(ProtocolCatalog.All.Select(p => p.Id).Distinct().Count() == ProtocolCatalog.All.Count, "IDs de catálogo únicos");
var first = new AnalysisSession("Sessão 'áudio' ; SELECT 1", "Bancada A");
Check(first.AddProtocol("rtp") && !first.AddProtocol("rtp"), "Seleção sem duplicatas");
Reject<ArgumentException>(() => first.AddProtocol("inventado"), "Protocolo desconhecido recusado");
Reject<ArgumentException>(() => new AnalysisSession("\n", "Ponto"), "Nome vazio / controle recusado");
Check(ProtocolCatalog.All.All(p => p.State == "Não executado" && p.Packets == "—"), "Nenhum protocolo começa como observado ou validado");
first.SelectInterface("interface-inexistente-para-teste");
var store = new SessionStore(Path.Combine(root, "roundtrip"));
store.Save(first);
var second = new AnalysisSession("Outra sessão", "Bancada B");
second.AddProtocol("mdns");
store.Save(second);
var reloaded = new SessionStore(Path.Combine(root, "roundtrip")).Load();
var actual = reloaded.Single(s => s.Id == first.Id);
Check(actual.Name == first.Name && actual.Location == first.Location && actual.CreatedUtc == first.CreatedUtc && actual.CatalogVersion == first.CatalogVersion && actual.InterfaceId == first.InterfaceId && actual.ProtocolIds.SequenceEqual(first.ProtocolIds), "SQLite reabre sessão e metadados sem alteração");
Check(reloaded.Single(s => s.Id == second.Id).ProtocolIds.SequenceEqual(new[] { "mdns" }), "Isolamento entre sessões");
using (var db = new SqliteConnection($"Data Source={store.DatabasePath};Pooling=False"))
{
    db.Open();
    using var command = db.CreateCommand();
    command.CommandText = "CREATE TRIGGER reject_test BEFORE INSERT ON session_protocols WHEN NEW.protocol_id='qdp' BEGIN SELECT RAISE(ABORT,'falha injetada'); END;";
    command.ExecuteNonQuery();
}
first.AddProtocol("qdp");
Reject<SqliteException>(() => store.Save(first), "Falha de gravação propagada");
Check(store.Load().Single(s => s.Id == first.Id).ProtocolIds.SequenceEqual(new[] { "rtp" }), "Transação preserva seleção anterior após falha");
var report = SessionReport.Create(actual, "Indisponível");
Check(report.Contains("RTP / RTCP") && report.Contains("Não executado") && report.Contains("interface-inexistente-para-teste") && !report.Contains("mDNS"), "Relatório usa somente dados da sessão e explicita não execução");
var newerDirectory = Path.Combine(root, "newer");
var newer = new SessionStore(newerDirectory);
using (var db = new SqliteConnection($"Data Source={newer.DatabasePath};Pooling=False"))
{
    db.Open();
    using var command = db.CreateCommand();
    command.CommandText = "PRAGMA user_version=99";
    command.ExecuteNonQuery();
}
var before = File.ReadAllBytes(newer.DatabasePath);
Reject<InvalidDataException>(() => new SessionStore(newerDirectory), "Esquema futuro recusado");
Check(before.SequenceEqual(File.ReadAllBytes(newer.DatabasePath)), "Banco de versão futura preservado");
var arp = Convert.FromHexString("FFFFFFFFFFFF00112233445508060001080006040001001122334455C0000201000000000000C0000202");
var dns = Convert.FromHexString("00112233445566778899AABB0800450000390001000040110000C0000201C00002350035003500250000123401000001000000000000076578616D706C6503636F6D0000010001");
var timestamp = DateTimeOffset.Parse("2026-10-01T12:34:56.123456Z");
var analyzer = new TrafficAnalyzer(); analyzer.Add(arp, (uint)arp.Length, timestamp); analyzer.Add(dns, (uint)dns.Length, timestamp);
var snapshot = analyzer.Snapshot(new() { SessionId = first.Id, Status = "Importado" });
Check(snapshot.Packets == 2 && snapshot.Protocols.Single(p => p.Id == "arp").Packets == 1 && snapshot.Protocols.Single(p => p.Id == "dns").Packets == 1 && snapshot.Protocols.Single(p => p.Id == "udp").Packets == 1, "Ethernet ARP e DNS/UDP com contagens de camadas explícitas");
Check(snapshot.Evidence.Any(p => p.Source == "192.0.2.1" && p.Destination == "192.0.2.2" && p.Protocol == "arp"), "Endereços são evidências do protocolo");
byte[] Ipv4(byte protocol, byte[] payload)
{
    var header = Convert.FromHexString("00112233445566778899AABB0800450000000001000040000000C0000201C0000202");
    System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(16, 2), (ushort)(20 + payload.Length)); header[23] = protocol;
    return [..header, ..payload];
}
var mdns = dns.ToArray(); mdns[34] = mdns[36] = 0x14; mdns[35] = mdns[37] = 0xe9;
var ptpPayload = new byte[44]; ptpPayload[1] = 2; ptpPayload[3] = 44;
var ptp = Convert.FromHexString("00112233445566778899AABB88F7").Concat(ptpPayload).ToArray();
var ipv6 = Convert.FromHexString("00112233445566778899AABB86DD6000000000083A40FE800000000000000000000000000001FF0200000000000000000000000000018000000000000000");
var protocolCorpus = new[] { arp, dns, mdns, ptp, ipv6, Ipv4(1, Convert.FromHexString("0800000000010001")), Ipv4(6, Convert.FromHexString("04D2005000000000000000005002000000000000")), Ipv4(2, Convert.FromHexString("16000000E0010101")), Ipv4(2, Convert.FromHexString("220000000000000101000000E0010101")) };
var coverage = new TrafficAnalyzer();
foreach (var packet in protocolCorpus) coverage.Add(packet, (uint)packet.Length, timestamp);
Check(TrafficAnalyzer.Supported.SetEquals(coverage.Snapshot(new()).Protocols.Select(p => p.Id)), "Corpus cobre todos os dez analisadores de cabeçalho declarados");
using (var writer = new PcapNgWriter(Path.Combine(root, "protocol-corpus.pcapng"))) foreach (var packet in protocolCorpus) writer.Write(packet, (uint)packet.Length, timestamp);
foreach (var packet in protocolCorpus) for (var n = 0; n < packet.Length; n++) new TrafficAnalyzer().Add(packet.AsSpan(0, n), (uint)packet.Length, timestamp);
Check(true, "Truncamentos de todos os protocolos do corpus tratados sem exceção");
var brokenDns = dns.ToArray(); brokenDns[54] = 0xC0; brokenDns[55] = 0x0C;
var broken = new TrafficAnalyzer(); broken.Add(brokenDns, (uint)brokenDns.Length, timestamp);
Check(broken.Snapshot(new()).Malformed == 1 && broken.Snapshot(new()).Protocols.All(p => p.Id != "dns"), "DNS com ponteiro cíclico não identificado como mensagem íntegra");
var fragment = dns.ToArray(); fragment[20] = 0x20;
var fragmented = new TrafficAnalyzer(); fragmented.Add(fragment, (uint)fragment.Length, timestamp);
Check(fragmented.Snapshot(new()).Protocols.Count == 0 && fragmented.Snapshot(new()).Uninterpreted == 1, "Fragmentos IPv4 não viram datagramas UDP completos");
var unknown = new TrafficAnalyzer(); unknown.Add(arp, (uint)arp.Length, timestamp, 101);
Check(unknown.Snapshot(new()).Protocols.Count == 0 && unknown.Snapshot(new()).Uninterpreted == 1, "Link type desconhecido não interpretado como Ethernet");
foreach (var packet in new[] { arp, dns }) for (var n = 0; n < packet.Length; n++) new TrafficAnalyzer().Add(packet.AsSpan(0, n), (uint)packet.Length, timestamp);
Check(true, "Todos os prefixos truncados do corpus têm tratamento controlado");
var random = new Random(7331); var fuzz = new TrafficAnalyzer();
for (var i = 0; i < 20000; i++) { var data = new byte[random.Next(0, 300)]; random.NextBytes(data); fuzz.Add(data, (uint)data.Length, timestamp); }
Check(fuzz.Snapshot(new()).Evidence.Count <= 80, "20 mil entradas aleatórias sem exceção e memória de evidências limitada");
var captureFile = Path.Combine(root, "golden.pcapng");
using (var writer = new PcapNgWriter(captureFile)) { writer.Write(arp, (uint)arp.Length, timestamp); writer.Write(dns, (uint)dns.Length, timestamp.AddSeconds(1)); writer.Flush(); }
var imported = PcapNgReader.Analyze(captureFile, first.Id);
Check(imported.Packets == 2 && imported.Bytes == arp.Length + dns.Length && imported.Protocols.Single(p => p.Id == "dns").Packets == 1, "PCAPNG round-trip de pacotes e protocolo");
Check(Math.Abs((imported.StartedUtc - timestamp).TotalMicroseconds) <= 1, "Timestamp PCAPNG em microssegundos preservado");
var badCapture = Path.Combine(root, "broken.pcapng"); var captureBytes = File.ReadAllBytes(captureFile); captureBytes[^1] = 1; File.WriteAllBytes(badCapture, captureBytes);
Reject<InvalidDataException>(() => PcapNgReader.Analyze(badCapture, first.Id), "Trailer de bloco divergente recusado");
File.WriteAllBytes(badCapture, File.ReadAllBytes(captureFile)[..^1]);
Reject<EndOfStreamException>(() => PcapNgReader.Analyze(badCapture, first.Id), "PCAPNG truncado recusado sem resultado completo falso");
var multiple = Path.Combine(root, "multiple-sections.pcapng"); var valid = File.ReadAllBytes(captureFile); File.WriteAllBytes(multiple, [..valid, ..valid]);
Check(PcapNgReader.Analyze(multiple, first.Id).Packets == 4, "Múltiplas seções PCAPNG aceitas com reset de interfaces");
var bigEndian = Path.Combine(root, "big-endian.pcapng");
var beSection = Convert.FromHexString("0A0D0D0A0000001C1A2B3C4D00010000FFFFFFFFFFFFFFFF0000001C");
var beInterface = Convert.FromHexString("0000000100000014000100000000FFFF00000014");
var bePacket = Convert.FromHexString("000000060000004C0000000000000000000F42400000002A0000002A");
File.WriteAllBytes(bigEndian, [..beSection, ..beInterface, ..bePacket, ..arp, 0, 0, 0, 0, 0, 0x4c]);
var beResult = PcapNgReader.Analyze(bigEndian, first.Id);
Check(beResult.Packets == 1 && beResult.StartedUtc == DateTimeOffset.UnixEpoch.AddSeconds(1), "PCAPNG big endian independente com timestamp conhecido");
var beNanoInterface = Convert.FromHexString("0000000100000020000100000000FFFF00090001090000000000000000000020");
var beNanoPacket = Convert.FromHexString("000000060000004C00000001000000003B9ACA000000002A0000002A");
File.WriteAllBytes(bigEndian, [..beSection, ..beInterface, ..beNanoInterface, ..beNanoPacket, ..arp, 0, 0, 0, 0, 0, 0x4c]);
Check(PcapNgReader.Analyze(bigEndian, first.Id).StartedUtc == DateTimeOffset.UnixEpoch.AddSeconds(1), "Resolução temporal e identificador da segunda interface respeitados");
var badInterface = valid.ToArray(); badInterface[56] = 99; File.WriteAllBytes(badCapture, badInterface);
Reject<InvalidDataException>(() => PcapNgReader.Analyze(badCapture, first.Id), "Referência a interface não declarada recusada");
var otherSession = new AnalysisSession("<script>alert(1)</script>", "Laboratório");
var html = CaptureArchive.Report(otherSession, imported, "html");
Check(html.Contains("&lt;script&gt;") && !html.Contains("<script>"), "Relatório HTML escapa conteúdo da sessão");
Check(new ProtocolView(ProtocolCatalog.Get("qdp"), imported).State == "Sem analisador" && new ProtocolView(ProtocolCatalog.Get("mdns"), imported).Presence == "Não observado", "Sem analisador difere de protocolo não observado");
var archive = CaptureArchive.NewDirectory(root, first.Id); CaptureArchive.Save(archive, imported); CaptureArchive.Seal(root);
Check(CaptureArchive.Latest(root, first.Id)?.Packets == 2, "Resumo de captura persistido e reaberto por sessão");
CaptureArchive.Save(archive, imported with { Status = "Capturando" });
Check(CaptureArchive.Latest(root, first.Id)?.Status == "Interrompida", "Resumo sem encerramento confirmado sinaliza interrupção");
using (var cancelled = new CancellationTokenSource()) { cancelled.Cancel(); Reject<OperationCanceledException>(() => PcapNgReader.Analyze(captureFile, first.Id, cancelled.Token), "Importação respeita cancelamento"); }
Reject<ArgumentException>(() => NpcapCapture.Run(new(first.Id, "não-é-guid", root), _ => { }, default), "Interface inválida recusada antes de acessar o driver");
var importDirectory = CaptureArchive.NewDirectory(root, second.Id);
var integration = CaptureArchive.Import(captureFile, importDirectory, second.Id, default);
Check(File.ReadAllBytes(captureFile).SequenceEqual(File.ReadAllBytes(Path.Combine(importDirectory, "original.pcapng"))) && integration.Packets == 2 && File.Exists(Path.Combine(importDirectory, "sha256.json")), "Importação integrada preserva bytes originais, cria resumo e hashes");
var exportPath = Path.Combine(root, "evidence.zip"); CaptureArchive.Export(importDirectory, exportPath);
using (var zip = System.IO.Compression.ZipFile.OpenRead(exportPath))
using (var original = zip.GetEntry("original.pcapng")!.Open())
using (var memory = new MemoryStream())
{
    original.CopyTo(memory);
    Check(memory.ToArray().SequenceEqual(File.ReadAllBytes(captureFile)) && zip.GetEntry("summary.json") is not null && zip.GetEntry("sha256.json") is not null, "Exportação ZIP mantém PCAPNG, resumo e manifesto sem alteração");
}
Reject<ArgumentException>(() => CaptureArchive.Export(importDirectory, Path.Combine(importDirectory, "self.zip")), "Exportação dentro da própria captura recusada");
Reject<InvalidDataException>(() => CaptureArchive.Validate(imported with { Protocols = [new("inventado", 1)] }, first.Id), "Resumo adulterado com protocolo desconhecido recusado");
Console.WriteLine($"{passed} verificações aprovadas. Dados de teste: {root}");
