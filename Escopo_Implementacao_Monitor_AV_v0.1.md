> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](docs/engenharia/11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Escopo de implementação — Monitor de Redes AV

Documento: revisão 0.1 • Data: 01/10/2026 • Situação: proposta inicial de implementação

Este documento define trabalho futuro. As versões, metas e provas aqui descritas são planejadas; nenhum teste de software ou equipamento foi executado nesta etapa.

## 1. Objetivo e limites do produto

Desenvolver um aplicativo desktop para Windows que monitore redes audiovisuais a partir de uma interface Ethernet selecionada, realize testes ativos controlados, analise tráfego recebido e gere evidências de conectividade, descoberta, sincronismo, mídia e controle.

O produto deverá permitir ao integrador identificar sintomas, comparar pontos de rede e produzir relatórios reproduzíveis. Toda conclusão deverá informar origem da evidência, instante, janela de observação e limitações.

Modos previstos:

| Modo | Fonte de dados | Resultado possível |
|---|---|---|
| Local | Interface Ethernet e testes do computador | Acessibilidade, rota local, serviços e tráfego recebido |
| Captura ampliada | Porta SPAN ou TAP disponibilizada pela infraestrutura | Análise dos fluxos efetivamente espelhados |
| Integrado | SNMP/API com credenciais de leitura | Informações de portas, VLANs e tabelas que o equipamento fornecer |
| Entre pontos | Agente remoto instalado em outro computador | Provas controladas de comunicação e entrega de tráfego entre origens e destinos |

Ficam fora da versão 1.0: configuração automática de switches, alteração de IP/MAC/VLAN, roteamento de áudio nos fabricantes, atualização de firmware, certificação física de cabos, certificação de conformidade AES67/PTP e garantia de diagnóstico completo de qualquer produto proprietário. Reprodução/decodificação de áudio e vídeo não é necessária ao monitoramento inicial.

## 2. Premissas e decisões da fase inicial

- Plataforma inicial proposta: Windows 11 x64; builds e adaptadores suportados serão registrados em matriz de compatibilidade.
- Operação local sem dependência de internet para monitorar e gerar relatórios.
- Interface de captura e endereço de origem explicitamente selecionados; uma rota por outra interface deverá ser sinalizada e não poderá produzir um resultado atribuído à Ethernet escolhida.
- Interface gráfica sem elevação permanente; operações que exigirem privilégio ficarão em componente separado e restrito.
- Testes ativos deverão ter alvos, taxa, tempo limite e cancelamento definidos. Perfis de carga e falhas serão usados em laboratório ou janela de manutenção.
- Informações não acessíveis permanecerão como indisponíveis ou inconclusivas.
- Escopo de fabricante será por modelo, versão e capacidade documentada, sem promessa genérica de integração universal.

A fase 0 deverá fechar linguagem, interface gráfica, mecanismo de captura, dependências, condições de distribuição e instalação. Uma arquitetura candidata é C#/.NET com interface WPF, serviço de coleta separado e SQLite local. A escolha e as versões serão confirmadas por prova técnica e registro de decisão, sem tratar esta sugestão como dependência já aprovada.

## 3. Requisitos identificados

| ID | Requisito | Entrega |
|---|---|---|
| RF-01 | Selecionar Ethernet, ler configuração, link e contadores disponíveis | MVP |
| RF-02 | Cadastrar projetos, locais, destinos e perfis de equipamento | MVP |
| RF-03 | Monitorar ICMP, tempos de resposta e disponibilidade observada | MVP |
| RF-04 | Exibir rota escolhida, traceroute, TTL observado e caminho incompleto | MVP |
| RF-05 | Testar TCP e consultas UDP específicas sem equiparar silêncio a porta fechada | MVP |
| RF-06 | Capturar, filtrar, importar e exportar PCAPNG | MVP |
| RF-07 | Identificar ARP, ICMP, mDNS, IGMP, PTP e QDP com evidência | MVP |
| RF-08 | Mostrar evidências iniciais de Dante/AES67 sem afirmar saúde integral | MVP |
| RF-09 | Histórico, eventos, relatório técnico e resumo para comunicação | MVP |
| RF-10 | Analisar RTP/RTCP, SAP/SDP, RTSP e perfis ampliados AV | 1.0 |
| RF-11 | SSDP, LLDP, DNS/DHCP e IPv6 básico com NDP e Hop Limit | 1.0 |
| RF-12 | NDI, Art-Net, sACN e Modbus TCP conforme matriz de suporte | 1.0 |
| RF-13 | Integração SNMP/API de leitura para modelos selecionados | 1.0 |
| RF-14 | Agente remoto e provas unicast/multicast entre pontos | 1.0 |
| RF-15 | Instalação, atualização, retenção, recuperação e compatibilidade de dados | 1.0 |

Requisitos transversais: cancelamento de operações; limitação de tráfego ativo; filas limitadas; proteção de credenciais; registro de descartes na captura; uso de relógio monotônico para durações; gravação de horários com fuso/UTC; validação de arquivos e entradas; funcionamento degradado quando faltar acesso.

## 4. Arquitetura de implementação

| Componente | Responsabilidade |
|---|---|
| Aplicativo desktop | Painéis, projetos, configuração, execução e consulta dos resultados |
| Inventário local | Interfaces, endereços, rotas, vizinhança e contadores |
| Agendador de testes | Periodicidade, concorrência, taxa, timeout e cancelamento |
| Coletor de pacotes | Captura, filtros, rotação de arquivos e contabilização de descartes |
| Decodificadores | Interpretar protocolos e produzir observações estruturadas |
| Motor de diagnóstico | Aplicar regras versionadas e separar fato, hipótese e limitação |
| Integrações | Consultas a switches, APIs e agente remoto |
| Persistência | Projetos, sessões, observações, resultados e referências às evidências |
| Relatórios | Exportação técnica e síntese textual com o mesmo conjunto de resultados |

Contratos comuns deverão incluir: projeto, sessão, ponto de teste, interface, alvo, protocolo, método, estado, horário, duração, valor/unidade, referência de evidência e versão do analisador.

Estados mínimos: validado, atenção, falha, não observado, inconclusivo e não executado. “Falha” deverá se referir ao teste, sem transformar ausência de ping em certeza de equipamento desligado.

## 5. Fases, entregas e critérios de passagem

### Fase 0 — Fechamento dos requisitos e laboratório

Atividades: detalhar RF-01 a RF-15; escolher equipamentos de referência; definir matriz Windows/adaptadores; coletar documentação primária dos protocolos; registrar portas por perfil; fechar métricas e limites de carga; avaliar licenças e disponibilidade de SDKs; definir tratamento de credenciais e capturas.

Entregáveis: backlog com IDs, matriz de suporte, plano de provas, topologia de laboratório, decisões de arquitetura e registro de riscos.

Critério de passagem: cada requisito tem responsável, prioridade, critério de aceite, dependências e prova associada; protocolos proprietários têm capacidade verificável delimitada. SDK indisponível implica declaração de suporte parcial, e não integração fictícia.

### Fase 1 — Fundação e provas de viabilidade — v0.1.0

Atividades: criar repositório, estrutura modular, integração contínua, armazenamento inicial, logs e protótipo de interface. Provar seleção de Ethernet, captura, vinculação dos testes à origem e execução sem bloquear a interface gráfica.

Entregáveis: aplicação mínima instalável, decisões técnicas, esquema inicial de dados e corpus inicial de capturas de referência.

Critério de passagem: em máquina limpa, o aplicativo identifica interfaces, captura um cenário conhecido, executa teste pela origem correta e encerra sem deixar processos ou capturas ativos. Dependência ausente gera orientação clara.

### Fase 2 — Rede básica e caminhos — v0.2.0

Atividades: implementar RF-01 a RF-05, incluindo rotas sobrepostas, múltiplas interfaces, destino local/remoto, timeouts e mudanças de link. Separar TTL recebido, TTL da sonda e saltos identificados. Implementar seleção explícita de alvos.

Entregáveis: inventário local, cadastro de destinos, monitor de disponibilidade, traceroute e testes de serviços.

Critério de passagem: provar destino local e roteado; gateway ausente; ICMP bloqueado com TCP acessível; salto silencioso; Wi-Fi/VPN simultâneos; cabo removido. Nenhum teste poderá ser atribuído à interface errada.

### Fase 3 — Captura e protocolos fundamentais — v0.3.0

Atividades: implementar RF-06 a RF-08; captura com filtros; ARP, ICMP, mDNS/DNS-SD, IGMPv2/v3, PTPv1/v2 e QDP; identificação inicial de Dante/AES67; taxa de tráfego, grupos, portas, TTL e DSCP. Criar rotação e limites de armazenamento.

Entregáveis: painel de protocolos, lista de fluxos, importador PCAPNG e biblioteca de casos positivos, negativos e truncados.

Critério de passagem: campos dos casos de referência conferem com valores esperados e inspeção independente; captura parcial não produz diagnóstico de bloqueio confirmado; arquivo malformado não derruba o aplicativo.

### Fase 4 — Diagnóstico, histórico e MVP — v0.4.0

Atividades: implementar RF-09; regras de correlação; alertas com limiares configuráveis; deduplicação; recuperação; relatórios; filtros por equipamento e comparação de sessões.

Entregáveis: MVP utilizável em campo, guia rápido, limitações e relatório de qualificação do MVP.

Critério de passagem: todos os requisitos MVP aprovados; demonstrar IP acessível com descoberta ausente, degradação e recuperação; executar prova contínua de 8 horas; relatório reproduz exatamente os resultados da sessão.

### Fase 5 — Ampliação AV e descoberta — v0.5.0

Atividades: implementar RF-10 a RF-12. Analisar RTP/RTCP e anúncios SAP/SDP; consultar RTSP quando disponível; adicionar DNS, DHCP, SSDP, LLDP e IPv6 básico; integrar NDI conforme mecanismo disponível; interpretar Art-Net e sACN; executar consultas Modbus somente de leitura.

Entregáveis: analisadores e perfis versionados, documentação da profundidade do suporte e respectivas provas.

Critério de passagem: cada módulo possui ao menos um caso positivo, um negativo e um incompleto; nenhum módulo usa apenas uma porta conhecida como prova definitiva de identidade. Recursos que dependam de SDK/API só serão declarados suportados após validação com a combinação real.

### Fase 6 — Infraestrutura gerenciada — v0.6.0

Atividades: implementar RF-13; credenciais protegidas; SNMP preferencialmente v3; adaptadores por família/modelo; consulta de interfaces, erros, descartes, VLANs, PoE, vizinhos e grupos quando expostos. Distinguir configuração consultada de tráfego observado.

Entregáveis: integração de leitura com ao menos uma família de switch escolhida na fase 0, matriz de capacidades e correlação equipamento/porta com proveniência.

Critério de passagem: comparar campos suportados com a administração do switch; demonstrar credencial incorreta, timeout e objeto ausente; nenhuma escrita de configuração; ausência de uma tabela não significa ausência da funcionalidade.

### Fase 7 — Agente remoto e provas entre pontos — v0.7.0

Atividades: implementar RF-14; pareamento autenticado, comunicação protegida, controle de alvos, sessão com limites e cancelamento; testes em ambos os sentidos; emissão e recepção controladas de unicast/multicast; sequência e contagem de pacotes; comparação por ponto.

Entregáveis: agente instalável, protocolo de comunicação versionado e assistente de prova entre VLANs/andares.

Critério de passagem: testar mesma VLAN, VLANs roteadas, bloqueio unidirecional, multicast permitido/bloqueado e perda do agente. Agente incompatível ou não autenticado é recusado. Medição de atraso unidirecional só é publicada com base de tempo e incerteza adequadas; caso contrário, usar RTT.

### Fase 8 — Robustez e versão beta — v0.8.0-beta.1

Atividades: completar RF-15; recuperação de falhas, limites de disco, migração de dados, acessibilidade básica, desempenho, instalador, atualização e desinstalação. Exercitar sobrecarga e privilégio mínimo.

Entregáveis: beta, guia de operação, procedimento de recuperação, inventário de dependências/licenças e relatório de desempenho.

Critério de passagem: cumprir metas acordadas no hardware de referência; passar prova contínua de 24 horas; reiniciar e recuperar sessões persistidas; lidar com disco cheio e driver ausente; manter contadores de perda e limitações visíveis.

### Fase 9 — Homologação em laboratório e campo — v1.0.0-rc.1

Atividades: executar bateria completa de aceitação em instalação limpa; FAT em laboratório e SAT em rede representativa; conferir relatórios; coletar avaliação do integrador; corrigir e repetir apenas provas afetadas, além da regressão crítica.

Entregáveis: dossiê de provas, matriz requisito–teste–evidência, lista de defeitos e termo de aceite técnico.

Critério de passagem: 100% dos casos obrigatórios aprovados; nenhum defeito crítico/alto aberto; limitações conhecidas documentadas; integrações reais testadas. Hardware/SDK indisponível deixa requisito bloqueado: não equivale a aprovação e exige revisão explícita do escopo se removido da entrega.

### Fase 10 — Liberação e manutenção — v1.0.0

Atividades: gerar release do commit homologado; publicar pacote e notas; testar instalação do artefato final; preservar evidências e hashes; acompanhar uso e organizar correções.

Entregáveis: instalador, documentação, changelog, manifestos de versões, checksums, política de atualização e backup.

Critério de passagem: o binário distribuído corresponde ao candidato aprovado; smoke test do pacote final aprovado; restauração de backup comprovada; responsáveis pela manutenção definidos. Assinatura digital deverá ser definida na fase 0 e usada quando o certificado estiver disponível, sem declarar pacote assinado antes disso.

## 6. Matriz de profundidade dos protocolos

| Família | Prova mínima até 1.0 | Limite obrigatório |
|---|---|---|
| ICMP/TCP/UDP | Consultas controladas e respostas correlacionadas | Silêncio UDP é inconclusivo; ICMP não define saúde do serviço |
| mDNS/SSDP/QDP | Anúncio ou resposta interpretável e sua origem | Descoberta não comprova transporte de mídia |
| IGMP | Queries/reports observados e grupos identificados | Captura local não comprova todas as tabelas de snooping |
| PTP | Mensagens, versões e identidades interpretáveis; domínio quando aplicável | Presença não comprova lock ou precisão nos endpoints |
| Dante/Q-LAN | Evidências de descoberta, relógio e tráfego conforme perfil | Saúde completa e subscrições dependem de integração documentada |
| AES67/RTP/RTCP/SAP/SDP | Sessão, sequência e métricas dos fluxos decodificáveis | Métrica afetada por descarte local deve ser marcada; não certifica áudio |
| RTSP/NDI | Serviço/sessão identificada e consulta suportada | Sem promessa de decodificar mídia ou validar qualquer implementação |
| Art-Net/sACN | Mensagens, origens e universos observados | Não garante resposta física de luminárias |
| Modbus TCP | Resposta de leitura conhecida em simulador/equipamento | Não executar funções de escrita |
| SNMP/LLDP | Campos confrontados com equipamento de referência | Apenas objetos e modelos explicitamente homologados |

## 7. Ambiente de provas

Preparar dois computadores, adaptadores Ethernet de referência, switch gerenciável com VLAN/IGMP e espelhamento, roteamento entre duas VLANs, dispositivos Dante/Q-SYS e endpoints das integrações declaradas. Usar simuladores para protocolos abertos e cenários destrutivos; a simulação não substitui a homologação das integrações reais.

Manter três topologias: rede simples na mesma VLAN; rede roteada com duas VLANs e políticas controladas; captura SPAN com tráfego conhecido. Arquivar configurações de referência, versões de firmware e diagrama de cada cenário.

O laboratório deverá permitir provocar, de forma isolada: perda de link, bloqueio de descoberta, bloqueio ICMP, alterações de multicast, atraso/perda artificial, serviço indisponível, falha de autenticação e falta de recursos no computador.

## 8. Rotina padrão de prova e registro

Cada caso receberá ID TP-xxx e referência aos RFs correspondentes. O registro conterá objetivo, preparação, topologia, versões, passos, resultado esperado, medição obtida, estado final e evidências.

Procedimento obrigatório:

1. Registrar versão do aplicativo, commit, versão dos perfis/regras, Windows, adaptador/driver, firmware, configuração e operador.
2. Restaurar a configuração de referência e confirmar que não há falha residual.
3. Executar a condição saudável e preservar a linha de base.
4. Introduzir uma única alteração controlada; registrar seu instante e mecanismo.
5. Executar o teste, guardar captura/log/resultado e confrontar com a verdade conhecida do laboratório.
6. Remover a alteração e demonstrar recuperação.
7. Classificar como aprovado, reprovado, bloqueado ou não aplicável, com justificativa. Falha de preparação não será registrada como sucesso.
8. Vincular defeitos às evidências e repetir o caso depois da correção. O resultado antigo permanece no histórico.

Provas de falha e recuperação terão três repetições para verificar consistência. Provas de longa duração terão uma execução completa por candidato relevante, repetida se houver mudança que afete sua validade.

## 9. Bateria mínima de testes de aceitação

| ID | Cenário/procedimento | Resultado esperado | Requisitos |
|---|---|---|---|
| TP-001 | Ethernet com DHCP; repetir com IP fixo | Dados conferem com a configuração do host | RF-01 |
| TP-002 | Ethernet, Wi-Fi e VPN simultâneos | Origem correta ou impossibilidade explicitamente informada | RF-01/04 |
| TP-003 | Ping a alvo estável; bloquear apenas ICMP mantendo TCP | Registrar ICMP sem resposta e serviço TCP acessível | RF-03/05 |
| TP-004 | Percurso roteado conhecido com um salto silencioso | Rota local e saltos disponíveis corretos; caminho incompleto sinalizado | RF-04 |
| TP-005 | Captura com TTL/Hop Limit conhecidos | Valor observado correto, sem inferir saltos exatos pelo TTL recebido | RF-04/11 |
| TP-006 | Bloquear descoberta mantendo acesso IP | Separar conectividade de descoberta; hipótese identificada como hipótese | RF-07/08/09 |
| TP-007 | Receber queries/reports IGMP; repetir com captura parcial | Identificar mensagens e preservar limitação de visibilidade | RF-07 |
| TP-008 | Introduzir mensagens PTP e troca controlada de líder | Interpretar eventos sem afirmar lock dos endpoints | RF-07/08 |
| TP-009 | RTP de referência com lacunas, reordenação e duplicatas | Métricas conferem com fixture; duplicatas não contadas como novas perdas | RF-10 |
| TP-010 | Saturar coletor acima do limite caracterizado | Contabilizar descartes e degradar conclusões sem travar a interface | RF-06/10 |
| TP-011 | Corpus positivo/negativo de cada protocolo | Classificação e campos esperados; ausência de falso “validado” | RF-07/08/10/11/12 |
| TP-012 | Consultas SNMP/API; credencial inválida e objeto ausente | Dados corretos ou erro específico; nenhuma alteração no switch | RF-13 |
| TP-013 | Agentes em duas VLANs; bloquear uma direção | Resultados separados por sentido; taxa limitada e cancelamento eficaz | RF-14 |
| TP-014 | Remover e reconectar cabo durante sessão | Detectar mudança, retomar coleta e preservar histórico | RF-01/03/09 |
| TP-015 | Importar arquivos truncados e entradas inválidas | Recusar ou interpretar parcialmente com erro claro, sem crash | RF-02/06 |
| TP-016 | Disco cheio e encerramento inesperado | Aviso, interrupção controlada da gravação e recuperação do que foi confirmado | RF-09/15 |
| TP-017 | Exportar relatório e reabrir sessão | Contagens, horários e estados coincidem com os dados persistidos | RF-09 |
| TP-018 | Atualizar base de versão anterior e restaurar backup | Migração correta; restauração preserva dados e versão compatível | RF-15 |
| TP-019 | Máquina limpa, usuário comum e dependência ausente | Instalação e mensagens corretas; operações permitidas funcionam | RF-15 |
| TP-020 | Monitoramento por 8 h no MVP e 24 h no RC | Sem falha não tratada, vazamento progressivo ou lacuna silenciosa | Transversais |
| TP-021 | Agente não autenticado ou versão incompatível | Conexão recusada com diagnóstico; nenhum teste remoto iniciado | RF-14 |
| TP-022 | Inspecionar logs, relatórios e pacote exportado | Sem senhas/tokens; dados técnicos sensíveis identificados para exportação | Transversais |

## 10. Metas mensuráveis e desempenho

Valores abaixo são metas iniciais de aceitação; devem ser fechados na fase 0 e não representam desempenho já medido.

| Item | Meta proposta |
|---|---|
| Carga funcional de referência | 100 alvos cadastrados, ciclo ICMP de 5 s por alvo e timeout de 1 s, com concorrência limitada |
| Disponibilidade | Estado de atenção após 3 ciclos consecutivos sem resposta; recuperação após 2 respostas; distinguir o método testado |
| Cancelamento | Parar novos envios em até 2 s; operações em andamento terminam pelo timeout definido |
| Interface gráfica | Atualização visual em até 2 s sob carga homologada |
| Captura de referência | Caracterizar com tráfego misto a 10 mil pacotes/s por 30 minutos no hardware registrado; sem descartes reportados pelo pipeline para aprovação desse perfil |
| Sobrecarga | Quando excedida a capacidade, declarar descartes e perda de confiabilidade das métricas afetadas |
| Memória | Meta de até 1 GB no perfil de referência, com filas e retenção limitadas; investigar crescimento sustentado entre janelas após aquecimento |
| CPU | Meta média de até 30% da capacidade total no hardware de referência; registrar também picos |
| Armazenamento | Captura circular com limite inicial de 2 GB por sessão, configurável; ao atingir limite, rotacionar e informar sobrescrita |
| Precisão dos parsers | 100% dos campos obrigatórios corretos no corpus homologado |
| Diagnóstico | Nenhum falso “validado” nos cenários negativos obrigatórios |
| Estabilidade | 8 h para MVP e 24 h para RC sem crash e sem falha silenciosa |

As métricas de jitter RTP, variação de RTT ICMP e precisão de relógio terão nomes e fórmulas diferentes. As fórmulas, unidades e condições de validade deverão ser documentadas e testadas com dados conhecidos. O produto não deverá tratar RTT como latência unidirecional ou confundir atraso de captura com atraso da rede.

## 11. Estratégia de testes durante o desenvolvimento

- Em cada alteração: testes das regras, parsers e fluxos afetados; validação de formato e análise estática.
- Em cada solicitação de integração: build limpo, testes unitários relevantes, replay do corpus crítico, migração quando alterada e revisão técnica.
- Em cada marco: integração de módulos, provas de laboratório previstas para a fase, instalador e relatório.
- Em cada candidato de release: regressão completa obrigatória, longa duração, atualização, recuperação e prova do binário final.
- Em campo: fluxo guiado de seleção da interface, cadastro, monitoramento, diagnóstico, exportação e repetição em outro ponto.

Priorizar testes que protejam interpretação de protocolos, seleção de origem, persistência e diagnósticos. Cobertura percentual isolada não substitui os casos críticos. CI sem hardware executa corpus/simuladores; ensaios com equipamentos têm execução e evidências próprias.

## 12. Versionamento e rastreabilidade

### Código e releases

Usar Git, branch principal protegida e branches curtas por tarefa: feature/RF-04-traceroute, fix/TP-009-sequencia-rtp e release/1.0. Aprovar integrações por revisão e pipeline; se houver um único desenvolvedor, registrar revisão própria e resultado automático sem alegar revisão independente.

Convenção de commits: feat, fix, test, docs, refactor, build e chore, sempre vinculando requisito ou defeito quando pertinente.

Versionamento MAJOR.MINOR.PATCH:

- MAJOR: quebra de compatibilidade pública de APIs, agente ou contratos publicados.
- MINOR: funcionalidades compatíveis acrescentadas.
- PATCH: correções compatíveis, sem novo escopo funcional.

Antes de 1.0, registrar explicitamente qualquer incompatibilidade. Usar pré-releases como 0.8.0-beta.1 e 1.0.0-rc.1. Tags de release são imutáveis; correção exige nova versão, nunca substituir o pacote mantendo o número.

### Dados, regras e documentação

Versionar separadamente aplicativo, protocolo do agente, esquema da base, catálogo de protocolos, regras de diagnóstico e revisão documental. Toda sessão guarda o conjunto de versões usado. Reanálise de captura por regra nova cria novo resultado sem apagar o anterior.

Migrações serão ordenadas e verificáveis, com backup anterior. Não presumir que uma versão antiga abre uma base migrada: reversão usa backup compatível ou migração reversa expressamente testada.

Cada release deverá conter versão, commit completo, data, changelog, requisitos atendidos, limitações, dependências travadas, matriz de compatibilidade, relatório de testes e hashes SHA-256 dos artefatos. A revisão 0.1 deste documento não corresponde à versão 0.1.0 do programa.

## 13. Pipeline e processo de liberação

1. Abrir tarefa vinculada a requisito, defeito ou mudança de escopo.
2. Implementar em branch curta e atualizar provas/documentação impactadas.
3. Executar build, análise estática, testes e corpus de referência.
4. Revisar e integrar na branch principal.
5. Gerar candidato rastreável com dependências fixadas e hash.
6. Executar testes de hardware/campo aplicáveis ao candidato.
7. Corrigir defeitos e gerar novo candidato quando necessário.
8. Registrar aceite e publicar o mesmo artefato aprovado; se recompilado, verificar novamente o pacote resultante.
9. Preservar release anterior e backup para recuperação.

Capturas de clientes, credenciais e configurações sensíveis não entram no repositório. Fixtures compartilhadas devem ser sintéticas ou adequadamente tratadas. Credenciais ficam no mecanismo seguro do sistema; logs e relatórios não as incluem.

## 14. Homologação final e classificação de defeitos

FAT: verificação formal em laboratório, incluindo cenários positivos, negativos, limites, atualização e longa duração. SAT: aceitação em rede representativa, com integrador executando rotinas reais e confrontando resultados com equipamentos e ferramentas de referência.

Crítico: perda de dados, exposição de credenciais, escrita indevida em equipamento ou conclusão falsa com impacto grave. Alto: falha de requisito obrigatório, diagnóstico incorreto relevante ou instabilidade recorrente. Médio/baixo: impacto limitado com alternativa documentada.

Liberação 1.0 exige todos os casos obrigatórios aprovados, nenhum crítico/alto aberto, aceite documentado dos médios/baixos remanescentes, documentação atualizada e demonstração de recuperação. Ausência de equipamento necessário será tratada como bloqueio ou mudança explícita de escopo, nunca como teste aprovado.

## 15. Responsabilidades e gestão da implementação

| Papel | Responsabilidade |
|---|---|
| Responsável pelo produto/integrador AV | Priorizar casos de campo, definir perfis e aceitar fluxos de operação |
| Desenvolvimento | Arquitetura, implementação, testes automatizados, instalador e correções |
| Responsável pela rede de laboratório | Topologia, configuração de referência e injeção controlada de falhas |
| Validação/QA | Executar provas, conferir evidências e rastrear defeitos |
| Responsável pela release | Conferir critérios, versões, pacote e documentação |

Os papéis podem ser acumulados; o relatório deverá identificar quem implementou e quem validou. Mudanças de escopo devem registrar motivo, impacto nos requisitos, dependências, provas e versão alvo.

Planejar por marcos, com estimativa de esforço após as provas da fase 1. O prazo depende principalmente de SDKs, acesso aos modelos de switches e disponibilidade de equipamentos para homologação. Não fixar prazo global antes de resolver essas dependências.

## 16. Entregáveis de encerramento

- Código-fonte organizado e histórico Git com tags de release.
- Aplicativo e agente remoto instaláveis, com compatibilidade declarada.
- Catálogo de protocolos, perfis de equipamentos e regras versionados.
- Manual de instalação, uso, coleta SPAN e limitações de diagnóstico.
- Plano de provas, corpus de referência e scripts de preparação do laboratório.
- Matriz de requisitos com resultados e evidências FAT/SAT.
- Relatório de desempenho, longa duração e defeitos conhecidos.
- Modelos de relatório técnico e resumo textual.
- Política de backup, retenção, atualização e recuperação.
- Changelog, manifestos, hashes e termo de aceite da versão 1.0.

O projeto estará concluído quando a versão 1.0 cumprir esses entregáveis e critérios, com cada funcionalidade declarada ligada a uma prova verificável. A manutenção posterior seguirá versões de correção e novos ciclos de qualificação conforme o impacto da mudança.

