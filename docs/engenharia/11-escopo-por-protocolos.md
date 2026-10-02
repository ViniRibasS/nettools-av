# Revisão de escopo — protocolos como núcleo

Revisão 0.2 • 01/10/2026 • Mudança solicitada pelo responsável pelo produto

Esta revisão prevalece sobre descrições anteriores centradas em equipamentos, incluindo o escopo 0.1, ADRs, backlog e base visual 0.1. O layout foi aprovado e deve ser preservado. As tecnologias continuam candidatas; esta mudança não representa homologação técnica.

## Regra do produto

Protocolos são a raiz da navegação, análise, histórico e relatório. Não existe cadastro, inventário, perfil, estado de saúde ou diagnóstico por equipamento. Um IP, MAC, porta, grupo multicast ou identidade de relógio pode aparecer como atributo de uma evidência de protocolo, nunca como entidade principal de diagnóstico.

Hierarquia: Projeto → Sessão/ponto de medição → Protocolo/versão → Observações, fluxos, eventos e resultados. A interface de captura pertence à sessão. Fluxos pertencem ao protocolo; seus endereços são campos técnicos. A presença de um protocolo não significa funcionamento correto. Não observado é diferente de não executado e de falha de captura.

Dante, AES67 e Q-LAN permanecem tecnologias/perfis de correlação de protocolos, sem serem confundidos com um único protocolo de fio ou com uma marca/modelo de equipamento. Não inferir identidade apenas por porta.

## UX vigente

Preservar sidebar, área central, inspetor, temas e composição. Navegação principal: Protocolos, Capturas, Relatórios. Sessões contêm seleção de protocolos. Busca por protocolo, família ou versão. Abas: Protocolos, Fluxos, Eventos. Inclusão por catálogo de protocolos, sem entrada de IP ou cadastro de dispositivo.

O inspetor apresenta presença, integridade das mensagens, eventos/métricas específicas, janela, transporte, evidências e limitações do protocolo selecionado. Selecionar um protocolo no catálogo ainda não analisado cria estado Não executado. Nenhuma seleção gera dados ou validação ficticiamente bem-sucedida.

Relatórios se organizam por protocolo e incluem versão/perfil, método, estado de observação, resultado do critério, métricas, contexto e limitações. Fluxos e eventos identificam explicitamente o protocolo. Não há agrupamento por fabricante ou equipamento.

## Impacto nos requisitos anteriores

| RF | Definição vigente / mudança |
|---|---|
| RF-01 | Manter seleção/configuração da interface como contexto de medição, sem inventário de dispositivos remotos |
| RF-02 | Projetos, locais, sessões e seleção de protocolos; substituir cadastro de equipamentos, faixas e perfis de equipamento |
| RF-03 | Análise ICMP e métricas de consultas controladas; remover disponibilidade/saúde por equipamento |
| RF-04 | Rota/TTL como contexto de uma prova de protocolo; destino pode ser parâmetro técnico da prova, não cadastro de equipamento |
| RF-05 | Testes TCP/UDP por método/perfil de protocolo; endereços apenas parâmetros de execução |
| RF-06 | Manter captura/importação/exportação PCAPNG e filtros por protocolo |
| RF-07 | Parsers produzem observações e critérios agrupados por protocolo/versão |
| RF-08 | Correlação Dante/AES67 baseada em protocolos e evidências; sem diagnóstico de endpoints |
| RF-09 | Histórico, alertas e relatórios por protocolo/sessão, sem histórico por equipamento |
| RF-10 | RTP/RTCP/SAP/SDP/RTSP com sessões e fluxos como evidências do protocolo |
| RF-11 | SSDP/LLDP/DNS/DHCP/IPv6/NDP observados como protocolos, sem montar inventário remoto |
| RF-12 | NDI/Art-Net/sACN/Modbus por capacidade de protocolo; remover dependência de perfis de equipamento na experiência |
| RF-13 | Retirar o painel/inventário/diagnóstico de switches da entrega. SNMP pode ser analisado como protocolo; consulta de infraestrutura só volta por escopo específico |
| RF-14 | Agente remoto é infraestrutura de medição para provas entre pontos; não é objeto de diagnóstico |
| RF-15 | Manter instalação, retenção, recuperação e rastreabilidade |

TP-001 a TP-022 e PVs continuam referências históricas. Reinterpretar TP-003/004/006/008/014/017/023/024 pela prova de protocolo e sessão; retirar aceite de cadastro de equipamento de TP-023 e substituí-lo por catálogo/seleção sem duplicatas. TP-012 de integração de switch deixa de ser obrigatório para o produto revisado. Não registrar casos removidos como aprovados. Os demais critérios de segurança, origem, parser, persistência e perda de captura permanecem aplicáveis.

## Modelo e engenharia

Entidades propostas: Projeto, Local, Sessão, InterfaceSnapshot, ProtocolDefinition, ProtocolObservation, ProtocolResult, FlowEvidence, ProtocolEvent, CaptureSegment e VersionSet. Resultados referenciam protocolo, versão do analisador, método, janela e evidências. Não criar Equipment/Device/Profile por fabricante. Regras de integridade, presença e inferência são distintas.

O módulo Windows permanece como inventário da interface local e execução de sondas; não deve descobrir um inventário de equipamentos. Os módulos de protocolo/diagnóstico/persistência/relatório passam a adotar o contrato acima. Para testes ativos futuros, limites e origem são definidos por protocolo e prova.

AVIO, Cisco, KDS e computadores permanecem somente recursos de laboratório, para gerar/confrontar tráfego conhecido. Seus modelos e firmwares podem constar no dossiê de validação, sem integrar a entidade de diagnóstico do aplicativo.

## Aceite da base visual revisada

Abrir sessão, selecionar protocolo, inspecionar critérios e contexto, consultar fluxos/eventos, pesquisar catálogo, adicionar protocolo sem duplicar e exportar resumo por protocolo. Sessão nova inicia sem observações. Manter todos os dados demonstrativos identificados, sem captura ou emissão real. Não deve existir cadastro, busca ou diagnóstico por equipamento na UI ou no modelo do protótipo.
