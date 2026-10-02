# Capacidades 0.2 e passagem para 1.0

Baseline consolidada em 02/10/2026, 0.2.0-preview.2. 1.0 somente após homologação completa. Duas capturas diretas e duas importações foram localizadas e verificadas; isso não substitui a campanha integral. [Evidências atuais](../qualidade/evidencias.md).

| Capacidade | Implementação atual | Validação atual / pendência |
|---|---|---|
| UI, logo RJ45/arroba e seletores | Logo SVG, DrawingImage, PNG e ICO; listas com tema e foco | Conferência visual local |
| Sessões SQLite | Gravação transacional, reabertura, isolamento | Testes automatizados; migração/recuperação completa pendente |
| Captura Ethernet Npcap | Host separado, filtro BPF, seleção por GUID exato, limites, segmentos, parada | Enumeração/filtros e erros de host testados; duas capturas diretas verificadas; carga e término abrupto pendentes |
| PCAPNG | Escrita SHB/IDB/EPB; importação com limites e metadados temporais | Round-trip, truncamento e múltiplas seções; leitura/gravação comparada com Wireshark |
| ARP, ICMPv4, TCP, UDP | Campos básicos de cabeçalho; sem remontagem/checksum/correlação | Corpus sintético positivo/truncado; confronto externo |
| DNS e mDNS | Estrutura de perguntas/registros e ponteiros DNS; contagens | Corpus e ponteiro cíclico; registros DNS-SD não interpretados semanticamente |
| IGMPv2/v3 | Tipos, grupo ou quantidade de registros; limites de estrutura | Corpus sintético; sem inferência de snooping/entrega |
| PTPv2 | Tipo/domínio/sequência e comprimento mínimo | Corpus sintético; sem eleição, lock, offset, precisão ou perfis homologados |
| ICMPv6 | Cabeçalho básico sem extensões IPv6 | Corpus sintético; NDP sem interpretação semântica |
| Relatórios e histórico | HTML/TXT/JSON do snapshot; evidências limitadas; arquivos preservados | Escape HTML, persistência e importação; longa duração pendente |
| PTPv1, QDP, RTP/RTCP, SAP/SDP, RTSP | Catálogo, sem analisador | Implementação e corpus pendentes |
| DHCP, SSDP, LLDP, NDI, Art-Net, sACN, Modbus, SNMP | Catálogo, sem analisador | Implementação/fontes/perfis e laboratório pendentes |
| Dante/AES67/Q-LAN | Perfis do catálogo, sem correlação implementada | Fontes e homologação física pendentes |
| Sondas ativas, rota/TTL, agente remoto | Não implementados | Provas de origem, autenticação e cancelamento pendentes |
| Instalador, atualização, assinatura e recuperação completa | Portátil e instalador por usuário com runtime; mutex protege uso | Instalação/reinstalação/remoção locais aprovadas; VM limpa, upgrade entre versões, backup/migração, assinatura e retenção pendentes |

## Histórico das evidências da prévia inicial

39 verificações automatizadas nesta revisão, incluindo 20.000 entradas aleatórias, todos os prefixos truncados do corpus dos dez analisadores declarados, endian alternativo e segunda interface com resolução temporal distinta. Corpus compartilhado com Wireshark: ARP, DNS, mDNS, PTPv2, ICMPv6, ICMPv4, TCP, IGMPv2 e IGMPv3 identificados sem indicação de pacote malformado pelo dissector; UDP aparece como camada de transporte nas mensagens DNS/mDNS.

`capinfos` reconheceu o arquivo de dois pacotes como PCAPNG Ethernet, 113 bytes de dados, 1 segundo de intervalo e precisão de microssegundos. Timestamps 1790858096.123456 e 1790858097.123456. SHA-256 do arquivo: `B0CB49033F53BFE5917829998E138FA459AF7BB88F99653D0E57BA6B93AAC0C1`.

Npcap enumerado pela API, sem abertura de interface. Compilação BPF válida/inválida verificada com contexto offline. Host separado respondeu com erro controlado para GUID inexistente e filtro inválido, sem produzir PCAPNG. Essas provas não substituem captura ao vivo e não aprovam PV-03.

UI nativa aberta com logo e nova lista suspensa no tema claro; criação de sessão sintética conferida. A automação de janelas ficou indisponível antes de concluir a importação pela UI (`foreground window did not report a process id`, seguido de ausência de janela retornada). Importação, persistência, relatórios e exportação ZIP foram verificados por código; conferência visual final de todos esses fluxos e do tema escuro permanece pendente. Não foi usada captura de tela de outra aplicação como evidência do produto.

Pacote portátil inclui avisos de dependências, guia, matriz de capacidades, manifesto dos arquivos e manifesto SHA-256 das fontes. É uma prévia interna não assinada; não corresponde ao aceite de instalação/atualização em máquina limpa.

## Bloqueios da 1.0

Antes de chamar de 1.0: completar módulos obrigatórios da revisão de escopo, corpus e casos por protocolo; executar PV-01 a PV-06 no alcance integral; validar interface/origem, descarte, sobrecarga, perda de link e encerramento sem host residual; executar provas AV no laboratório; instalação em máquina limpa, recuperação, migração, sigilo dos relatórios e ensaio RC de 24 horas. Registrar resultados medidos e versões. O diagnóstico por equipamento permanece removido.

As versões preview.1 e preview.2 são prévias de desenvolvimento. Não foi declarada homologação integral. A auditoria/revisão realizada é do próprio desenvolvimento, sem revisor independente ou CI remoto.


## Sequência de registros de 02/10/2026

Usuário informou ter aberto uma captura feita no Wireshark. Registro de importação manual, ainda sem arquivo ou resultado detalhado fornecido. Isso não comprova captura ao vivo. Naquele momento o instalador estava em preparação; depois foi concluído, conforme [registro 15](../15-instalador.md). Pendências de protocolos e homologação permanecem.


Após a instalação, foram localizadas e verificadas quatro evidências locais: duas capturas diretas (3.751 e 3.773 pacotes, zero descartes registrados pelo driver) e duas importações (35.483 e 9.501). Hashes preservados e reanálise consistente. O instalador passou por instalação/reinstalação/desinstalação e preservou os dados. Detalhes em ../15-instalador.md. Evidências de campo são parciais; protocolos pendentes e homologação integral continuam em aberto.

