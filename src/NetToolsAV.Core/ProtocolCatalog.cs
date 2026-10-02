namespace NetToolsAV.Core;

public sealed record ProtocolDefinition(string Id, string Name, string Family, string Version, string Transport, string Limitation)
{
    public string State => "Não executado";
    public string Presence => "Indisponível";
    public string Packets => "—";
}

public static class ProtocolCatalog
{
    public const string Version = "0.2.0";
    public static IReadOnlyList<ProtocolDefinition> All { get; } = Array.AsReadOnly(new[]
    {
        new ProtocolDefinition("arp", "ARP", "Rede básica", "Ethernet / IPv4", "Ethernet", "Associações observadas são evidências de protocolo, não inventário de equipamentos."),
        new("icmp", "ICMPv4", "Rede básica", "IPv4", "IP · protocolo 1", "Respostas ICMP não comprovam saúde de serviços."),
        new("tcp", "TCP", "Transporte", "RFC 9293", "IP · protocolo 6", "Conexão TCP não comprova funcionamento da aplicação."),
        new("udp", "UDP", "Transporte", "RFC 768", "IP · protocolo 17", "Silêncio UDP é inconclusivo."),
        new("mdns", "mDNS / DNS-SD", "Descoberta", "RFC 6762 / 6763", "UDP · 5353", "Presença de anúncios não comprova transporte de mídia."),
        new("igmp2", "IGMPv2", "Multicast", "v2", "IP · protocolo 2", "Captura local não comprova todas as tabelas de snooping."),
        new("igmp3", "IGMPv3", "Multicast", "v3", "IP · protocolo 2", "Queries e reports observados não comprovam entrega fora do ponto de medição."),
        new("ptp1", "PTPv1", "Sincronismo", "v1", "Conforme perfil", "Presença de mensagens não comprova lock ou precisão."),
        new("ptp2", "PTPv2", "Sincronismo", "v2", "Conforme perfil", "Precisão exige base de tempo e método qualificados."),
        new("qdp", "QDP", "Descoberta", "Perfil pendente", "Conforme documentação", "Parser depende de fonte verificável de estrutura; portas não bastam."),
        new("rtp", "RTP / RTCP", "Mídia", "RTP v2", "UDP · portas por sessão", "Lacunas podem decorrer de descarte na captura. Jitter exige clock rate conhecido."),
        new("sap", "SAP / SDP", "Anúncios", "Perfil a validar", "Anúncio / descrição de sessão", "Descrições de sessão não comprovam entrega de mídia."),
        new("rtsp", "RTSP", "Controle", "Versão a selecionar", "Conforme perfil", "Resposta de controle não comprova reprodução."),
        new("dns", "DNS", "Resolução", "Perfil DNS", "UDP / TCP", "A análise depende do ponto de captura e da consulta."),
        new("dhcp", "DHCPv4", "Endereçamento", "IPv4", "UDP", "Ausência durante a janela não implica indisponibilidade do serviço."),
        new("ssdp", "SSDP", "Descoberta", "Perfil SSDP", "UDP", "Anúncios são limitados à janela e visibilidade da captura."),
        new("lldp", "LLDP", "Vizinhança", "Perfil LLDP", "Ethernet", "A aplicação interpreta mensagens, sem construir inventário remoto."),
        new("ndp", "NDP / ICMPv6", "Rede básica", "IPv6", "ICMPv6", "Endereços link-local exigem contexto de interface."),
        new("ndi", "NDI", "Tecnologia / integração", "SDK a avaliar", "Conforme documentação", "A presença no catálogo não declara SDK integrado ou suporte homologado."),
        new("artnet", "Art-Net", "Automação", "Revisão a selecionar", "UDP", "Mensagens não garantem efeitos físicos."),
        new("sacn", "sACN", "Automação", "E1.31 · revisão pendente", "UDP", "Interpretação depende de norma e corpus qualificados."),
        new("modbus", "Modbus TCP", "Automação", "Somente leitura", "TCP", "Não executar funções de escrita."),
        new("snmp", "SNMP", "Gerenciamento", "Versão a selecionar", "UDP", "Protocolo de gerenciamento; não inclui painel de diagnóstico de equipamentos."),
        new("dante", "Dante", "Perfil de correlação", "Perfil a validar", "Múltiplos protocolos", "Correlação de descoberta, relógio e mídia; não é um único protocolo de fio."),
        new("aes67", "AES67", "Perfil de correlação", "Perfil a validar", "PTP / RTP / sessão", "Observações não certificam conformidade AES67."),
        new("qlan", "Q-LAN", "Perfil de correlação", "Perfil a validar", "Conforme documentação", "Suporte depende de evidências documentadas; não validar apenas por portas.")
    });

    public static ProtocolDefinition Get(string id) => All.FirstOrDefault(p => p.Id == id)
        ?? throw new ArgumentException($"Protocolo desconhecido: {id}.", nameof(id));
}
