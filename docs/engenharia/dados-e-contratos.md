# Dados e contratos

Baseline 0.2.0-preview.2. Fonte: AnalysisSession, TrafficAnalysis, SessionStore, CaptureWorker e CaptureArchive. [Modelo visual](../diagramas/README.md#modelo-de-dados).

## SQLite

`%LOCALAPPDATA%\NetToolsAV\nettools-av.sqlite`, `user_version=1`, chaves estrangeiras habilitadas. Esquema maior que 1 é recusado, sem downgrade automático.

| Campo | Tipo e significado |
|---|---|
| sessions.id | TEXT PK, GUID |
| sessions.name | TEXT obrigatório, 1–70 caracteres sem controles |
| sessions.location | TEXT obrigatório, 1–100 caracteres sem controles |
| sessions.created_utc | TEXT, DateTimeOffset formato O |
| sessions.interface_id | TEXT nullable, interface local |
| sessions.catalog_version | TEXT, versão de catálogo |
| session_protocols.session_id | TEXT FK para sessions, ON DELETE CASCADE |
| session_protocols.protocol_id | TEXT, ID validado pelo catálogo do domínio |
| session_protocols.ordinal | INTEGER, ordem de seleção |

PK composta `(session_id, protocol_id)`. Catálogo está no código, não em tabela. Save insere metadados da nova sessão; em conflito atualiza só a interface e substitui seleção em transação. Renomear/excluir sessão na UI não está implementado.

## Arquivos

```text
NetToolsAV/                         # raiz de dados
  nettools-av.sqlite
  captures/<session-guid-N>/<timestamp-UTC>-<guid-N>/
    capture-001.pcapng              # captura direta, segmentos seguintes possíveis
    original.pcapng                 # alternativa para importação
    summary.json
    sha256.json
```

Binários ficam separados em `%LOCALAPPDATA%\Programs\NetTools AV`. Histórico consulta arquivos, não tabela de capturas. Backup só do banco não preserva evidências.

## CaptureSnapshot — JSON esquema 1

| Campos | Contrato |
|---|---|
| Schema / Analyzer | 1 / headers-0.2 |
| Id / SessionId | Identidade da captura e associação à sessão |
| Source / Filter | Interface Npcap ou nome do arquivo / BPF |
| Status / Error | Estado da execução / erro opcional |
| StartedUtc / UpdatedUtc | DateTimeOffset; preserve offset e normalize comparações |
| Packets / Bytes | Registros processados / bytes capturados |
| Truncated / Uninterpreted / Malformed | Contadores distintos, não necessariamente disjuntos |
| DriverReceived / DriverDropped | uint nullable; null é indisponível, não zero |
| Protocols | `{Id, Packets}`, no máximo uma contagem por protocolo por pacote |
| Evidence | Até 80 `{Number, Time, Protocol, Source, Destination, Detail}` |
| Files / Limitations | Nomes locais / limites da observação |

Resumo arquivado limitado a 1 MiB, com validação de esquema, sessão, IDs, listas e comprimentos. Evidências são últimas observações, não lista completa de pacotes. Camadas se sobrepõem.

`sha256.json`: `{File, Bytes, SHA256}` de cada PCAPNG. Não cobre resumo, não autentica autoria, não impede alteração conjunta de arquivo e manifesto. Histórico não verifica hashes automaticamente em cada abertura.

## IPC interno

`--capture-worker` não é API pública. Primeira linha stdin é CaptureRequest: SessionId, InterfaceId, Directory, Filter, Promiscuous, DurationSeconds, LimitMiB. Limite de 16.384 caracteres verificado após ReadLine. Próxima linha ou EOF solicita cancelamento; cliente fecha stdin. Defaults C#: vazio, falso, 900 s, 256 MiB; UI envia 60 s.

stdout: CaptureSnapshot compacto por linha; limite de 1 MiB conferido após leitura. Saída 0 sem Falha; saída 1 para falha tratada. Sem handshake/versionamento próprio, autenticação ou porta pública. Os limites pós-leitura não garantem resistência a alocações por linha hostil.

## Formatos

HTML/TXT registram contexto, métricas e limites; HTML escapa conteúdo. JSON de relatório tem raiz `{Session, Capture}`, diferente de summary.json. Sem captura, relatório só registra configuração.

ZIP contém PCAPNG, resumo e hashes; destino novo fora da pasta de captura; escrita `.partial` antes de renomear. Não há importação ZIP.

PCAPNG suporta SHB/IDB/EPB, endian por seção, múltiplas seções/interfaces e resolução/offset temporal. PB/SPB recusados; outros blocos podem ser ignorados com contagem, preservando o original. PCAP clássico não é aceito.

## Evolução

Mudança de esquema exige compatibilidade, dados anteriores, backup/rollback e testes. Não reutilizar ID de protocolo para outra semântica. Alteração de interpretação exige revisar Analyzer; evolução do catálogo precisa preservar sessões ou oferecer migração explícita.

## Perfil efetivo dos analisadores

| ID | Reconhecimento atual | Exclusões importantes |
|---|---|---|
| arp | Ethernet/IPv4, tamanhos 6/4, request/reply | Outros tipos de hardware/protocolo e diagnóstico de identidade |
| icmp | Cabeçalho ICMPv4, tipo/código | RTT, associação entre consulta/resposta e checksum |
| tcp | Cabeçalho, portas e flags | Remontagem, estado de conexão e diagnóstico de aplicação |
| udp | Comprimento e portas | Entrega, perda e semântica do payload genérico |
| dns | Estrutura DNS em UDP 53, nomes/ponteiros e limites | DNS sobre TCP, interpretação completa dos RDATA |
| mdns | Estrutura DNS em UDP 5353 | Semântica DNS-SD e conformidade multicast completa |
| igmp2 | Mensagens v2 reconhecidas e limites | Snooping, entrega em outras portas e estado global |
| igmp3 | Query/report, contagens e comprimentos | Estado multicast global e conformidade integral |
| ptp2 | Cabeçalho v2, tipo/domínio/sequência, mínimos por tipo; UDP 319/320 ou EtherType 0x88f7 | Eleição, offset, precisão, lock e homologação de perfil |
| ndp | Cabeçalho ICMPv6 básico | Semântica NDP, extensão/fragmentação IPv6 |

Ethernet aceita até quatro tags VLAN no caminho implementado. Fragmentação IPv4 não é remontada nem interpretada como transporte completo. Cabeçalhos de extensão IPv6 não são percorridos. A descrição de transporte do catálogo pode ser mais ampla que o parser; esta tabela e a matriz de capacidades definem o alcance efetivo.
