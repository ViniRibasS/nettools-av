using System.Text;

namespace NetToolsAV.Core;

public sealed class AnalysisSession
{
    private readonly List<string> protocolIds = [];
    public Guid Id { get; }
    public string Name { get; }
    public string Location { get; }
    public DateTimeOffset CreatedUtc { get; }
    public string? InterfaceId { get; private set; }
    public IReadOnlyList<string> ProtocolIds => protocolIds.AsReadOnly();
    public string CatalogVersion { get; }

    public AnalysisSession(string name, string location, Guid? id = null, DateTimeOffset? created = null, string? catalogVersion = null)
    {
        Name = Validate(name, 70, "Nome da sessão");
        Location = Validate(location, 100, "Ponto de medição");
        Id = id ?? Guid.NewGuid();
        if (Id == Guid.Empty) throw new ArgumentException("Identificador de sessão inválido.");
        CreatedUtc = (created ?? DateTimeOffset.UtcNow).ToUniversalTime();
        CatalogVersion = catalogVersion ?? ProtocolCatalog.Version;
    }

    public bool AddProtocol(string id)
    {
        _ = ProtocolCatalog.Get(id);
        if (protocolIds.Contains(id, StringComparer.Ordinal)) return false;
        protocolIds.Add(id);
        return true;
    }

    public void SelectInterface(string? id) => InterfaceId = string.IsNullOrWhiteSpace(id) ? null : id;

    private static string Validate(string value, int max, string field)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > max || value.Any(char.IsControl))
            throw new ArgumentException($"{field}: informe de 1 a {max} caracteres, sem caracteres de controle.");
        return value.Trim();
    }
}

public static class SessionReport
{
    public static string Create(AnalysisSession session, string interfaceDescription)
    {
        var text = new StringBuilder();
        text.AppendLine("NETTOOLS AV — SESSÃO POR PROTOCOLOS");
        text.AppendLine("Configuração da sessão · nenhuma análise de tráfego anexada");
        text.AppendLine($"Sessão: {session.Name}");
        text.AppendLine($"Ponto: {session.Location}");
        text.AppendLine($"Criação UTC: {session.CreatedUtc:O}");
        text.AppendLine($"Catálogo: {session.CatalogVersion}");
        text.AppendLine($"Interface selecionada: {interfaceDescription}");
        text.AppendLine($"Identificador da interface: {session.InterfaceId ?? "não selecionada"}");
        text.AppendLine("Janela de captura: não executada");
        foreach (var id in session.ProtocolIds)
        {
            var protocol = ProtocolCatalog.Get(id);
            text.AppendLine();
            text.AppendLine($"{protocol.Name} | {protocol.Version}");
            text.AppendLine("Estado: Não executado; presença e contagem indisponíveis.");
            text.AppendLine($"Limitação: {protocol.Limitation}");
        }
        if (session.ProtocolIds.Count == 0) text.AppendLine("Nenhum protocolo selecionado.");
        text.AppendLine();
        text.AppendLine("Este arquivo registra configuração da sessão, não prova de funcionamento da rede.");
        return text.ToString();
    }
}
