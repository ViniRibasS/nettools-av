> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Laboratório, provas de viabilidade e aceitação

Revisão 0.1 • 01/10/2026 • Plano, sem testes executados

## Inventário inicial informado pelo usuário

| Recurso | Disponibilidade informada | Informação ainda necessária |
|---|---|---|
| Dante AVIO | Disponível | Modelo, quantidade, firmware, funções e modo AES67 suportado/habilitado |
| Switch Cisco | Disponível | Modelo, firmware, portas, VLANs, SPAN, IGMP, roteamento e SNMP disponíveis |
| Kramer KDS | Disponível | Modelos TX/RX, quantidades, firmware, manual e tecnologia de transporte |
| Computadores | Disponíveis | Quantidade utilizável, Windows/build, CPU/RAM/disco, Ethernet/driver e privilégios |
| Q-SYS | Não informado | Obter equipamento/captura de referência e versão para homologar QDP |
| NDI/Art-Net/sACN/Modbus | Não confirmados | Identificar endpoints; simuladores para desenvolvimento e hardware para aceite aplicável |

Disponibilidade informada não equivale a equipamento inspecionado ou capacidade confirmada. Não houve conexão, consulta ou configuração dos equipamentos.

## Topologias propostas

T1 — mesma VLAN: PC-A executa o monitor; PC-B fornece serviço de teste e referência; AVIO e KDS conectam ao Cisco quando capacidades e plano de endereçamento forem confirmados. Não gerar tráfego de carga nos equipamentos AV antes de definir limites.

T2 — duas VLANs: PC-A na VLAN A, PC-B na VLAN B, com roteador ou switch L3 confirmado. A capacidade de roteamento do Cisco ainda é desconhecida. ACLs e bloqueios são preparados pelo responsável pelo laboratório, com backup/restauração, em ambiente isolado. Não modificar a rede de produção.

T3 — captura ampliada: espelhar portas selecionadas para interface de captura dedicada. Usar outra interface para gestão/testes ativos se a porta SPAN não aceitar tráfego normal. Registrar direção, oversubscription, filtros, limites de espelhamento e tratamento de tags VLAN do modelo real.

Cada topologia receberá diagrama, endereços, portas físicas, versões, configuração de referência e SHA-256 de seus arquivos. Confirmar o vínculo entre adaptador Windows e porta física antes das provas de origem.

## Provas de viabilidade da fase 1

São especificações para execução posterior. “Aprovada” exige artefatos e medições, não apenas ausência de erro.

| ID | Preparação e procedimento | Critério de aceite | Evidência / responsável |
|---|---|---|---|
| PV-01 | Instalar SDK escolhido em ambiente de desenvolvimento; protótipo WPF mínimo; listar interfaces sem elevação; executar operação lenta e cancelá-la | Build reproduzível; interface selecionável; UI atualiza em até 2 s sob cenário da prova; encerramento limpo | Manifesto SDK/SO, log, medição e captura de tela; DEV/QA |
| PV-02 | Duas interfaces com rotas concorrentes; Ethernet/Wi-Fi/VPN conforme disponibilidade; executar ICMP/TCP/UDP pela origem escolhida; repetir com mudança de rota e cabo removido | Captura externa confirma interface/origem para cada método; impossibilidade explícita impede atribuição falsa; cancelamento para novos envios em até 2 s | Capturas em interfaces e receptor, tabela de rotas e sequência de eventos; DEV/LAB |
| PV-03 | Backend candidato e host separado; usuário comum; acesso permitido/negado; driver indisponível em VM; encerrar desktop; gerar sobrecarga controlada | UI não elevada; erros específicos; cliente IPC indevido recusado; host encerra sem captura residual; descartes por camada visíveis | Versão/configuração Npcap, memória/CPU, logs IPC/handles, contagens de gerador; DEV/QA |
| PV-04 | Gravar captura conhecida, reabrir externamente e reimportar; corpus com duas interfaces/seções, resoluções distintas, endian e truncamentos | Arquivo PCAPNG real; conteúdo, interface e tempo preservados no perfil suportado; bloco desconhecido tratado; arquivo inválido não derruba processo | Arquivos, hashes, campos esperados, comparação com ferramenta independente; DEV/QA |
| PV-05 | Persistir sessão; comparar relatório e reabertura; interrupção inesperada, disco sem espaço em volume de laboratório, rotação, backup e restauração | Sem perda de dados já confirmados nas condições definidas; lacunas explícitas; snapshot coerente; evidência expirada sinalizada; restauração validada | Banco de teste, manifesto, logs e contagens antes/depois; DEV/QA |
| PV-06 | Instalar candidato em máquina/VM limpa sem SDK; usuário comum; testar dependência ausente, sem internet, reinício e remoção | Aplicativo inicia offline; dependências e privilégios claros; remoção não apaga projetos sem escolha explícita; pacote rastreável | Hash do pacote, versões, roteiro e resultados de instalação; DEV/REL |

PV-01 a PV-06 não foram executadas nesta entrega porque a etapa solicitada é anterior ao código. O serviço Npcap observado como ativo não aprova PV-03.

PCAPNG terá perfil de suporte explícito: inicialmente propor escrita de SHB/IDB/EPB e estatísticas disponíveis; leitura deverá definir tratamento de SPB, opções, blocos desconhecidos, múltiplas seções e link types não suportados. Não descartar metadados silenciosamente ao importar/exportar. Selecionar a revisão normativa e fixar corpus antes de implementar.

## Matriz de execução dos casos do escopo

Todos os casos abaixo estão **não executados**. Dependência pendente não é resultado “aprovado” nem “não aplicável”. As descrições completas permanecem no documento de escopo.

| Caso | Marco mínimo | Preparação principal | Evidência obrigatória |
|---|---|---|---|
| TP-001 | v0.2 | DHCP e IP fixo em T1 | Dados do host comparados ao painel |
| TP-002 | v0.2 | Interfaces simultâneas/rotas concorrentes | Origem capturada e rota registrada |
| TP-003 | v0.2 | PC-B com TCP ativo e ICMP bloqueável | Resultados separados por método |
| TP-004 | v0.2 | T2 e salto silencioso | Rota conhecida versus saltos observados |
| TP-005 | v0.2 IPv4; v0.5 IPv6 | Pacotes com TTL/Hop Limit controlados | Valores esperados versus decodificados |
| TP-006 | v0.4 | Descoberta bloqueada e IP acessível | Estado, hipótese e janela de captura |
| TP-007 | v0.3 | Queries/reports IGMP e captura parcial | Campos, grupos e limitação de visibilidade |
| TP-008 | v0.3 | PTP por versão e troca de líder controlada | Mensagens e eventos sem alegação de lock |
| TP-009 | v0.5 | RTP com sequência conhecida | Lacunas, duplicatas, reordenação e métricas |
| TP-010 | v0.3; repetir v0.5 | Gerador com carga crescente | Descartes, filas, UI e confiabilidade |
| TP-011 | v0.3 e cada expansão | Corpus de cada parser/perfil | Campos e estados positivo/negativo/incompleto |
| TP-012 | v0.6 | Cisco identificado e leitura autorizada | Comparação com administração; falhas específicas |
| TP-013 | v0.7 | Dois PCs, T1/T2 e bloqueio por sentido | Sequências, taxas e cancelamento |
| TP-014 | v0.2; repetir v0.4 | Remoção/reconexão do cabo | Linha de tempo e retomada |
| TP-015 | v0.3 | Arquivos e entradas inválidos | Erro controlado, recursos e dados preservados |
| TP-016 | v0.4 inicial; v0.8 completo | Volume isolado e encerramento abrupto | Recuperação do confirmado e lacunas |
| TP-017 | v0.4 | Sessão persistida e exportada | Contagens, horários, estados e versões |
| TP-018 | v0.8 | Base anterior e backup | Migração/restore e compatibilidade |
| TP-019 | v0.1 inicial; v0.8 completo | VM limpa sem SDK/driver e usuário comum | Instalação e mensagens verificadas |
| TP-020 | v0.4 e RC | 8 h MVP; 24 h RC | Séries de recursos, erros e lacunas |
| TP-021 | v0.7 | Agente indevido/incompatível | Recusa e ausência de teste remoto |
| TP-022 | v0.4; repetir nas integrações | Segredos sintéticos conhecidos | Inspeção de logs, relatório e pacote |

## Complementos propostos à bateria

TP-023: criar, editar e reabrir projeto/local/destino/perfil; validar faixas, deduplicação e alvo explícito. Complementa RF-02.

TP-024: executar consulta UDP documentada com resposta correta, resposta não correlacionada e silêncio; verificar taxa e cancelamento. Complementa RF-05 sem transformar silêncio em “porta fechada”.

TP-025: verificar filtro, round-trip PCAPNG e rotação de segmentos, incluindo preservação de referência ou marcação de expiração. Complementa RF-06. Estes IDs não modificam os TP-001 a TP-022 do escopo.

## Métricas e critérios

Adotar as metas originais como propostas: 100 alvos/5 s, timeout de 1 s, atenção após 3 ausências e recuperação após 2 respostas, UI em até 2 s, captura de 10 mil pacotes/s por 30 min, até 1 GB de memória e CPU média até 30% no hardware registrado. Não são resultados medidos. Confirmar limites em ENG-06.

Ausência ICMP: sondas efetivamente enviadas sem resposta válida até o timeout divididas pelas sondas enviadas elegíveis, por janela. Excluir cancelamentos anteriores ao envio e registrar falhas locais separadamente. RTT mínimo/médio/máximo usa somente respostas válidas; descrever amostra e unidade. Disponibilidade observada usa ciclos elegíveis, preservando intervalos sem medição. Não confundir variação de RTT com jitter RTP.

RTP: a especificação da métrica será fechada a partir de F26, com clock rate conhecido, janela de reordenação, sequência estendida, reinício e duplicatas. Até esse fechamento, não publicar uma fórmula improvisada de perda ou jitter. Perda na captura degrada a conclusão sobre perda do fluxo.

Registrar CPU como percentual da capacidade total e memória com métrica definida; capturar séries, picos e comparação de janelas após aquecimento. Definir duração de aquecimento antes do ensaio. Cada camada de descarte deve ter proveniência e semântica; não somar contadores sobrepostos nem interpretar contador ausente como zero.

## Procedimento e passagem de fase

Usar o [modelo de prova](../modelos/registro-de-prova.md). Registrar baseline saudável, uma falha isolada, resultado, restauração e recuperação; três repetições para falha/recuperação. Preservar resultado anterior quando houver correção. Provas longas têm uma execução integral por candidato relevante, repetida se sua validade mudar.

Fase 0 termina quando RFs têm responsáveis nominais/aceite/dependências/provas, fontes proprietárias têm alcance delimitado, versões e licenças são decididas, e laboratório/metas são definidos. A documentação atual cumpre a estrutura inicial, mas ainda não fecha esses itens.

Fase 1 exige PVs aprovadas no alcance definido e aplicação mínima instalável. MVP exige RF-01 a RF-09 e ensaio de 8 h; RC exige bateria obrigatória completa e 24 h. Hardware indisponível mantém o caso bloqueado quando chegar sua execução; sua exclusão requer mudança explícita de escopo. Nenhum gate foi aprovado nesta etapa.


