# Requisitos, rastreabilidade e backlog atual

Baseline 02/10/2026. Interpretação vigente em [revisão 11](11-escopo-por-protocolos.md); requisitos históricos em 06-backlog.md não devem ser lidos como estado atual. “Parcial” não é aceite completo.

| ID | Capacidade vigente | Estado e código | Evidência/aceite restante |
|---|---|---|---|
| RF-01 | Interface como contexto local | Parcial: EthernetInventory, seleção por GUID | Enumeração e campo; origem de sondas, troca de link e APIs ampliadas pendentes |
| RF-02 | Sessões/protocolos; projetos/locais | Parcial: AnalysisSession, SessionStore | Persistência testada; entidade Projeto e gestão ampliada pendentes |
| RF-03 | ICMP e consultas controladas | Parcial: cabeçalho ICMPv4 | Corpus; RTT/sondas/histerese não implementados |
| RF-04 | Rota/TTL como contexto de prova | Não implementado | Provar interface/origem e interpretação de timeout |
| RF-05 | Provas TCP/UDP por protocolo | Parcial: inspeção passiva | Sondas e correlação de respostas pendentes |
| RF-06 | Captura/importação/exportação | Parcial: NpcapCapture, PcapNg, CaptureArchive | Corpus/campo/ZIP; carga, perda de link e formatos completos pendentes |
| RF-07 | Parsers por protocolo/versão | Parcial: TrafficAnalyzer | Dez IDs básicos testados; QDP/PTPv1 e critérios completos pendentes |
| RF-08 | Correlação Dante/AES67 | Catálogo somente | Fontes/perfis e provas físicas pendentes |
| RF-09 | Histórico/relatório/eventos | Parcial: Archive, HistoryWindow | Histórico e relatórios testados; alertas/eventos persistidos pendentes |
| RF-10 | RTP/RTCP/SAP/SDP/RTSP | Catálogo somente | Implementação/corpus/métricas pendentes |
| RF-11 | Descoberta e IPv6 | Parcial: DNS/mDNS/ICMPv6 | DHCP/SSDP/LLDP e NDP semântico pendentes |
| RF-12 | NDI/Art-Net/sACN/Modbus | Catálogo somente | Fontes, implementação e laboratório pendentes |
| RF-13 | Diagnóstico/inventário de switch | Removido do escopo | Não marcar como aprovado; SNMP permanece somente entrada de protocolo |
| RF-14 | Agente remoto de medição | Não implementado | Contrato, autenticação, limites e provas por sentido |
| RF-15 | Instalação/recuperação/retenção | Parcial: scripts/Inno | Ciclo de instalação testado; máquina limpa, migração, assinatura e retenção pendentes |

## Requisitos não funcionais

| ID local | Critério | Situação |
|---|---|---|
| RNF-01 | Resultados reproduzíveis com versão, método e evidência | Snapshot e hashes presentes; provenance de release ainda sem commit/tag |
| RNF-02 | Limites e cancelamento sem congelar UI | Host isolado/background; sobrecarga e fechamento sob carga pendentes |
| RNF-03 | Persistência sem perda silenciosa | Transações testadas; recuperação de desastre não homologada |
| RNF-04 | Dados locais e exportação controlada | Sem upload automático; sem criptografia/redação próprias |
| RNF-05 | Leitura honesta de observação/inconclusão | Estados implementados; revisar cada novo parser |
| RNF-06 | Usabilidade e acessibilidade em ambos os temas | Conferência parcial; QA visual integral pendente |
| RNF-07 | Estabilidade prolongada | Ensaio MVP 8 h e RC 24 h ainda não registrados como aprovados |

IDs RNF acima organizam esta revisão e não substituem aceites do escopo original.

## Próximos incrementos e dependências

| Prioridade | Trabalho | Conclusão verificável |
|---|---|---|
| P0 | Consolidar critérios obrigatórios por módulo e fontes pendentes | Matriz RF–casos–fonte–resultado, sem “catálogo = suporte” |
| P0 | Robustez de captura/cancelamento/origem e disco | Ensaios com limite, desconexão, carga e término do host; dados íntegros |
| P0 | Recuperação/migração e máquina limpa | Backup/restauração, upgrade real e rollback testados |
| P1 | RTP/RTCP e associação SAP/SDP/RTSP | Corpus positivo/negativo e métricas com clock rate conhecido |
| P1 | Descoberta/IPv6 e protocolos AV adicionais | Perfil versionado por protocolo e fontes suficientes |
| P1 | Sondas e agente, se mantidos obrigatórios | Origem/interface, autenticação, limites e cancelamento comprovados |
| P1 | UI/acessibilidade e desacoplamento de apresentação | Fluxos conferidos e regras testáveis fora das janelas |
| P0 para release | Homologação, assinatura e rastreabilidade | Aceite registrado sem pendência bloqueante |

Prioridade não reduz escopo. Produto define prioridades/aceite; DEV implementa, QA registra evidência, LAB prepara ponto de prova, REL controla distribuição. Sem prazos ou responsáveis nominais inventados.
