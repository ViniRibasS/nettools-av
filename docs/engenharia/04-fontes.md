> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Registro de fontes técnicas

Consulta em 01/10/2026 • Revisão 0.1

Foram priorizados mantenedores, fabricantes e organismos responsáveis pelas especificações. “Consultada” significa leitura da página ou trecho técnico disponibilizado, não revisão integral de toda a norma. Não foram baixados SDKs, adquiridas normas, incorporados códigos de terceiros ou arquivadas cópias integrais das páginas. URLs dinâmicas devem ser vinculadas a versão/commit e data antes de implementação.

## Plataforma e infraestrutura

| ID | Fonte primária | Achado ou uso | Situação |
|---|---|---|---|
| F01 | [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy) | Ciclo de suporte orienta alvo .NET 10 | Consultada |
| F02 | [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/) | Capacidades de UI Windows | Consultada |
| F03 | [WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) | Alternativa de UI | Consultada |
| F04 | [Avalonia](https://docs.avaloniaui.net/docs/welcome) | Alternativa multiplataforma | Consultada |
| F05 | [Npcap Users Guide](https://npcap.com/guide/npcap-users-guide.html) | Instalação, acesso administrativo e dependências | Consultada |
| F06 | [Npcap OEM](https://npcap.com/oem/) | Diferencia uso interno e redistribuição | Consultada; cenário interno confirmado, número de instalações pendente |
| F07 | [SharpPcap](https://github.com/dotpcap/sharppcap) e [projeto](https://github.com/dotpcap/sharppcap/blob/master/SharpPcap/SharpPcap.csproj) | Backend .NET; metadados do repositório | Consultada; pacote exato não auditado |
| F08 | [PacketDotNet](https://github.com/dotpcap/packetnet) | Parsers básicos e licença declarada MPL-2.0 | Consultada; adoção opcional |
| F09 | [PCAPNG, draft-ietf-opsawg-pcapng](https://datatracker.ietf.org/doc/draft-ietf-opsawg-pcapng/) | Consulta retornou revisão 06, setembro/2026; blocos e metadados | Consultada; Internet-Draft, não apresentar como RFC final |
| F10 | [pcap_dump_open](https://www.tcpdump.org/manpages/pcap_dump_open.3pcap.html) | Contrato de escrita deve ser distinguido da leitura de formatos | Consultada |
| F11 | [GetBestRoute2](https://learn.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-getbestroute2), [IcmpSendEcho2Ex](https://learn.microsoft.com/en-us/windows/win32/api/icmpapi/nf-icmpapi-icmpsendecho2ex), [opções IP](https://learn.microsoft.com/en-us/windows/win32/winsock/ipproto-ip-socket-options) | Rota, origem IPv4 e seleção de interface de sockets | Consultadas; prova multihomed pendente |
| F12 | [Microsoft.Data.Sqlite: async](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/async) | I/O síncrona exige afastar trabalho da thread visual | Consultada |
| F13 | [SQLite WAL](https://www.sqlite.org/wal.html) | Concorrência e restrições de armazenamento | Consultada |
| F14 | [SQLite Backup API](https://www.sqlite.org/backup.html) | Backup consistente | Consultada |
| F15 | [Named pipe security](https://learn.microsoft.com/en-us/windows/win32/ipc/named-pipe-security-and-access-rights) | Segurança do IPC local | Consultada |
| F16 | [Windows data protection](https://learn.microsoft.com/en-us/dotnet/standard/security/how-to-use-data-protection) | Proteção de credenciais | Consultada |

## Protocolos e fabricantes

| ID | Fonte primária | Aplicação | Limite da consulta |
|---|---|---|---|
| F17 | [RFC 6762](https://www.rfc-editor.org/rfc/rfc6762) | mDNS | Consultada; selecionar seções e erratas para parser |
| F18 | [RFC 6763](https://www.rfc-editor.org/rfc/rfc6763) | DNS-SD | Consultada |
| F19 | [RFC 2236](https://www.rfc-editor.org/rfc/rfc2236) | IGMPv2 | Consultada |
| F20 | [RFC 9776](https://www.rfc-editor.org/rfc/rfc9776) e [RFC 3376](https://www.rfc-editor.org/rfc/rfc3376) | IGMPv3 | RFC 9776 substitui RFC 3376; manter casos de equipamentos legados |
| F21 | [IEEE 1588 WG](https://sagroups.ieee.org/1588/), [IEEE 1588-2019](https://standards.ieee.org/ieee/1588/6825/) | PTP | Páginas institucionais consultadas; texto normativo integral de 2002/2008/2019 não adquirido/revisado |
| F22 | [Dante: portas](https://support.getdante.com/hc/en-gb/articles/5508285534239-Which-network-ports-does-Dante-use) | Perfil de descoberta, relógio e tráfego | Tabela consultada; não é especificação completa do protocolo |
| F23 | [Dante: informações para administradores](https://www.getdante.com/wp-content/uploads/2025/12/InfoNetworkAdmins_R1.pdf) | Arquitetura Dante e contextos de rede | Trecho técnico consultado; perfil AVIO exato ainda pendente |
| F24 | [Q-SYS 9.7: multicast](https://help.qsys.com/q-sys_9.7/Content/Networking/Multicast_Traffic.htm) | QDP e rede Q-LAN | Fonte explicitamente da versão 9.7; não comprova layout completo do payload QDP |
| F25 | [AES Standards Store](https://aes2.org/publications/standards-store/) e [prévia AES67](https://www.aes.org/publications/standards/preview.cfm?ID=96) | Referência AES67-2023 | Catálogo/prévia identificados; texto integral não revisado |
| F26 | [RFC 3550](https://www.rfc-editor.org/rfc/rfc3550) | RTP/RTCP, sequência e jitter | Consultada; implementar somente campos e métricas testados |
| F27 | [RFC 8866](https://www.rfc-editor.org/rfc/rfc8866) | SDP | Consultada; SAP/RTSP exigem fontes complementares |
| F28 | [NDI SDK](https://docs.ndi.video/all/developing-with-ndi/sdk) | Mecanismo de integração | Página consultada; SDK e termos do artefato não avaliados |
| F29 | [Especificação Art-Net](https://art-net.org.uk/downloads/art-net.pdf) | Mensagens de automação | Documento localizado/consultado; fixar revisão antes de implementar |
| F30 | [Modbus Specifications](https://www.modbus.org/modbus-specifications) | Protocolo e guia TCP | Índice consultado; selecionar revisão e funções de leitura |
| F31 | [ESTA: normas publicadas](https://tsp.esta.org/tsp/documents/published_docs.php) | sACN/E1.31 | Acesso falhou nesta consulta; edição e conteúdo pendentes |
| F32 | [Kramer KDS-100](https://k.kramerav.com/support/product_downloads.asp?pid=4666), [KDS-17](https://k.kramerav.com/support/product_downloads.asp?pid=4734) | Localização de manuais por família | Exemplos encontrados no fabricante; não identificam os KDS do usuário |
| F33 | [Cisco: referência SPAN](https://www.cisco.com/c/en/us/support/docs/switches/catalyst-6500-series-switches/10570-41.html) | Preparação de captura ampliada | Referência geral localizada; manual correto depende do modelo/firmware |
| F34 | [GetAdaptersAddresses](https://learn.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-getadaptersaddresses), [GetIfEntry2](https://learn.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-getifentry2) | Inventário de adaptadores e informações de interface | Consultadas; conferir estruturas e disponibilidade por campo em RF-01 |
| F35 | [GetIpForwardTable2](https://learn.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-getipforwardtable2), [GetIpNetTable2](https://learn.microsoft.com/en-us/windows/win32/api/netioapi/nf-netioapi-getipnettable2) | Tabelas de rotas e vizinhança | Consultadas; confronto com host em TP-001/004 |
| F36 | [RFC 826](https://www.rfc-editor.org/rfc/rfc826) | ARP | Consultada; selecionar campos e atualizações aplicáveis |
| F37 | [RFC 791](https://www.rfc-editor.org/rfc/rfc791), [RFC 792](https://www.rfc-editor.org/rfc/rfc792) | IPv4 e ICMP | Consultadas; revisar atualizações/erratas para funcionalidades implementadas |
| F38 | [RFC 768](https://www.rfc-editor.org/rfc/rfc768), [RFC 9293](https://www.rfc-editor.org/rfc/rfc9293) | UDP e TCP | Consultadas; TCP usa referência consolidada atual em lugar de apenas RFC 793 |

## Lacunas a resolver antes dos respectivos módulos

Complementar fontes normativas de DNS/DHCP, IPv6/NDP, SAP, RTSP, SNMP/MIBs e LLDP, conferindo atualizações e erratas. As bases de ARP/IPv4/ICMP/UDP/TCP foram consultadas em F36–F38; falta selecionar seções e atualizações aplicáveis ao parser. RF-01 tem APIs candidatas F34/F35, mas ainda precisa de mapeamento de estruturas por campo e prova de disponibilidade. A seleção de RFCs conhecida de memória não é considerada validação documental concluída.

QDP precisa de fonte verificável de estrutura e capturas de referência. A página Q-SYS sem versão falhou no acesso; foi usada a referência 9.7 com esse limite explícito. Não inferir parser a partir apenas de portas. Normas PTP/AES67 integrais e seus direitos de acesso permanecem pendentes. Fontes Cisco/Kramer serão substituídas por manuais do equipamento real.

## Critério para uma fonte sustentar implementação

Registrar ID, título, URL, versão/data/commit, seção relevante, capacidade sustentada, limitações e RF/PV/TP vinculados. Para fixtures, registrar também origem autorizada, tratamento de dados, hash e resultado esperado definido antes do teste. Quando houver conflito, confrontar norma, versão do fabricante e comportamento medido; manter a divergência registrada. Não transformar documentação publicitária em prova de conformidade.


