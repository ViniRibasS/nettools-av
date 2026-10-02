> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Matriz inicial de suporte e perfis

Revisão 0.1 • 01/10/2026 • Nenhum protocolo implementado/homologado

Fontes Fxx estão no [registro de fontes](04-fontes.md). “Planejado” refere-se à entrega do escopo. Limitação de fonte ou equipamento mantém requisito pendente; não reduz seu aceite.

| Família / RF | Capacidade planejada | Evidência mínima e limite | Fonte / dependência |
|---|---|---|---|
| ARP/ICMP, RF-03/07 | Campos, associação observada e respostas correlacionadas | Pacote íntegro ou resposta da API; timeout não prova equipamento desligado | F36/F37; PV-02 |
| TCP/UDP, RF-05 | Conexão TCP; consulta UDP definida por perfil | Transporte acessível não comprova função do serviço; silêncio UDP inconclusivo | F11/F38; selecionar solicitação UDP documentada |
| mDNS/DNS-SD, RF-07 | Registros e associação de serviços anunciados | Correlacionar registros, origem e validade; RR TTL difere de IP TTL | F17/F18; corpus com compressão e truncamento |
| IGMPv2/v3, RF-07 | Query/report, grupos e fontes quando presentes | Querier observado não comprova toda a rede/snooping | F19/F20 |
| PTPv1/v2, RF-07/08 | Versão, mensagens, identidades, domínio quando aplicável | Não declarar lock, precisão ou líder de toda a rede a partir de captura parcial | F21; normas e corpus por versão |
| QDP, RF-07 | Identificar anúncio/resposta interpretável | Portas são pista; identificação conclusiva exige estrutura verificável | F24; layout/corpus e equipamento pendentes |
| Dante/AES67, RF-08 | Correlacionar evidências iniciais de descoberta, relógio e tráfego | RTP/PTP isolados não provam AES67 nem saúde integral Dante | F22/F23/F25; AVIO disponível, modelo/firmware pendentes |
| RTP/RTCP, SAP/SDP, RTSP, RF-10 | Sessão e métricas decodificáveis, consulta RTSP prevista | Clock rate e associação de sessão necessários para jitter; sem decodificação de mídia | F26/F27; SAP/RTSP a catalogar |
| SSDP/LLDP/DNS/DHCP/IPv6, RF-11 | Campos, serviços, NDP e Hop Limit | Não inferir topologia completa; IPv6 por escopo/interface | Fontes específicas pendentes |
| NDI, RF-12 | Descoberta/consulta pelo mecanismo disponível | Não pressupor que KDS use NDI; SDK e endpoint exigidos | F28; equipamento NDI não confirmado |
| Art-Net/sACN, RF-12 | Identificar mensagens e universos | Não prova efeito físico | F29/F31; simulador mais equipamento de aceite |
| Modbus TCP, RF-12 | Leitura documentada, sem funções de escrita | Perfil com unit ID, registradores, escala e limites | F30; mapa real pendente |
| SNMP/API, RF-13 | Campos de switch homologados e consultas de leitura | Objeto ausente é indisponibilidade de dado, não ausência da função | F33; modelo Cisco, MIBs e credenciais pendentes |
| Kramer KDS, perfil de fabricante | Inventário, conectividade e tráfego observável inicialmente | Integração específica só após identificar família e documentação | F32; extensão de perfil a detalhar sem ampliar implicitamente RF-12 |

## Sementes documentais de filtros

Estas referências servem para desenhar capturas de laboratório; ainda não são um catálogo de portas executável.

| Candidato | Referência inicial | Regra de uso |
|---|---|---|
| mDNS | UDP 5353, 224.0.0.251 / FF02::FB | Interpretar DNS e contexto; F17 |
| PTP sobre UDP | UDP 319/320 nos perfis documentados | Considerar versão/transporte; não excluir transporte Ethernet por pressuposto |
| Dante | Grupos/portas de F22, incluindo mDNS e sincronismo | Selecionar somente entradas aplicáveis ao AVIO e modo de rede |
| QDP, referência Q-SYS 9.7 | 224.0.23.175, UDP 2467–2470 e unicast 6504 | Referência F24; portas não bastam para afirmar QDP |

## Perfil por modelo

Cada perfil futuro terá fabricante, modelo exato, firmware testado, capacidades, serviços/portas e transportes, versão da fonte, consultas autorizadas, taxa máxima, campos esperados, critérios de interpretação, limitações e provas associadas. Mudança de firmware pode invalidar homologação. Não criar perfis universais “Cisco”, “Kramer” ou “Dante” com todas as capacidades declaradas.

Disponibilidade informada: AVIO, Cisco, Kramer KDS e computadores. Não foi demonstrado se há pares TX/RX, AES67 habilitado, roteamento entre VLANs, SNMPv3 ou SPAN. Essas capacidades serão verificadas no inventário, antes de desenhar comandos ou configurar equipamentos.


