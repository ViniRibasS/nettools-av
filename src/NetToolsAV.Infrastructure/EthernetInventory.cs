using System.Net.NetworkInformation;

namespace NetToolsAV.Infrastructure;

public sealed record EthernetSnapshot(string Id, string Name, string Description, string State, string Speed, string Addresses, string Dns, string Gateway, string Mac, string? Error)
{
    public string DisplayName => $"{Name} · {State}";
    public string Summary => $"{Description}\nEstado: {State} · Velocidade informada: {Speed}\nEndereços: {Addresses}\nGateway: {Gateway}\nDNS: {Dns}\nMAC local: {Mac}{(Error is null ? "" : $"\nLeitura parcial: {Error}")}";
}

public static class EthernetInventory
{
    public static IReadOnlyList<EthernetSnapshot> Read()
    {
        var result = new List<EthernetSnapshot>();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces().Where(n => n.NetworkInterfaceType is
                     NetworkInterfaceType.Ethernet or NetworkInterfaceType.GigabitEthernet or NetworkInterfaceType.FastEthernetT or NetworkInterfaceType.FastEthernetFx))
        {
            string addresses = "Indisponível", dns = "Indisponível", gateway = "Indisponível", speed = "Indisponível", mac = "Indisponível";
            string? error = null;
            try
            {
                var properties = nic.GetIPProperties();
                addresses = Join(properties.UnicastAddresses.Select(a => $"{a.Address}/{a.PrefixLength}"));
                dns = Join(properties.DnsAddresses.Select(a => a.ToString()));
                gateway = Join(properties.GatewayAddresses.Select(a => a.Address.ToString()));
                speed = nic.Speed > 0 ? $"{nic.Speed / 1_000_000d:0.##} Mbps" : "Indisponível";
                mac = nic.GetPhysicalAddress().ToString();
                if (mac.Length == 0) mac = "Indisponível";
            }
            catch (Exception exception) when (exception is NetworkInformationException or PlatformNotSupportedException)
            {
                error = exception.Message;
            }
            var state = nic.OperationalStatus == OperationalStatus.Up ? "Link ativo" : "Link inativo / indisponível";
            result.Add(new(nic.Id, nic.Name, nic.Description, state, speed, addresses, dns, gateway, mac, error));
        }
        return result.OrderBy(n => n.Name, StringComparer.CurrentCulture).ToArray();
    }

    private static string Join(IEnumerable<string> values) => values.Any() ? string.Join("; ", values) : "Não informado";
}
