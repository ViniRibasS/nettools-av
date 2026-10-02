# Diagramas funcionais e técnicos

Baseline 0.2.0-preview.2. Mermaid editável; visualize em renderizador compatível. Diagramas representam o código atual, salvo seção explicitamente planejada. Fontes: MainWindow, CaptureWorker, NpcapCapture, SessionStore e CaptureArchive.

## Contexto funcional

```mermaid
flowchart LR
    U[Operador] --> A[NetTools AV Desktop]
    F[PCAPNG existente] -->|Importação local| A
    N[Tráfego visível na interface Ethernet] --> D[Npcap externo]
    D -->|Captura passiva| A
    A --> B[(Sessões SQLite)]
    A --> E[PCAPNG e resumos locais]
    A --> R[Relatórios e ZIP]
    R --> U
```

Sem transmissão ativa de teste, nuvem ou inventário de equipamento neste fluxo.

## Fluxo do usuário

```mermaid
flowchart TD
    A[Abrir aplicativo] --> B[Criar ou selecionar sessão]
    B --> C[Selecionar protocolos]
    C --> D{Fonte da observação}
    D -->|Arquivo| E[Importar PCAPNG]
    D -->|Rede| F[Escolher Ethernet e parâmetros]
    F --> G[Iniciar captura]
    G --> H[Parar ou atingir limite]
    E --> I{Análise concluída?}
    H --> I
    I -->|Sim| J[Inspecionar protocolos e limites]
    I -->|Falha| K[Registrar erro e preservar evidência]
    J --> L[Consultar histórico]
    L --> M[Exportar relatório ou ZIP]
```

## Componentes e dependências

```mermaid
flowchart TB
    subgraph Desktop
      UI[MainWindow e diálogos]
      CLIENT[CaptureClient]
      WORKER[CaptureWorker em processo filho]
    end
    subgraph Infrastructure
      STORE[SessionStore]
      INV[EthernetInventory]
      CAP[NpcapCapture]
      ARCH[CaptureArchive]
      PCAP[PcapNgReader e Writer]
    end
    subgraph Core
      SESSION[AnalysisSession e catálogo]
      ANALYZER[TrafficAnalyzer]
      SNAP[CaptureSnapshot e ProtocolView]
    end
    UI --> STORE
    UI --> INV
    UI --> CLIENT
    CLIENT -. JSON por streams .-> WORKER
    WORKER --> CAP
    WORKER --> ARCH
    UI --> ARCH
    CAP --> PCAP
    CAP --> ANALYZER
    ARCH --> PCAP
    PCAP --> ANALYZER
    STORE --> SESSION
    UI --> SNAP
    ANALYZER --> SNAP
```

## Sequência da captura

```mermaid
sequenceDiagram
    actor Operador
    participant UI as Desktop / CaptureClient
    participant Host as CaptureWorker
    participant Driver as Npcap
    participant Disco as Arquivos locais
    Operador->>UI: Selecionar interface e iniciar
    UI->>Host: Iniciar EXE --capture-worker
    UI->>Host: CaptureRequest em stdin
    Host->>Driver: Validar filtro e GUID; abrir Ethernet
    loop Enquanto há tempo, espaço e não cancelado
      Host->>Driver: pcap_next_ex não bloqueante
      Driver-->>Host: Pacote ou ausência temporária
      Host->>Disco: Gravar PCAPNG e resumo periódico
      Host-->>UI: Snapshot JSON em stdout
    end
    Operador->>UI: Parar (ou limite automático)
    UI->>Host: EOF stdin quando solicitado
    Host->>Disco: Finalizar arquivo, hashes e resumo
    Host-->>UI: Snapshot terminal e término
```

Erros podem terminar o fluxo antes do loop. Se a parada solicitada exceder 5 s, o cliente pode matar o host; não é uma conclusão normal. Hash/último resumo podem ficar incompletos em falha abrupta.

## Importação e exportação

```mermaid
flowchart LR
    ORIGINAL[Arquivo escolhido] --> COPY[Cópia original.pcapng]
    COPY --> READER[PcapNgReader com limites]
    READER --> ANALYZE[TrafficAnalyzer]
    ANALYZE --> SUMMARY[summary.json]
    COPY --> HASH[sha256.json]
    SUMMARY --> VIEW[UI e histórico]
    VIEW --> REPORT[HTML / TXT / JSON]
    COPY --> ZIP[ZIP de evidências]
    SUMMARY --> ZIP
    HASH --> ZIP
```

Relatório não inclui necessariamente todos os bytes dos pacotes; ZIP preserva os arquivos de evidência.

## Estados de execução

```mermaid
stateDiagram-v2
    [*] --> Preparando
    Preparando --> Capturando: Interface aberta
    Preparando --> Falha: Validação ou abertura falhou
    Capturando --> Concluida: Cancelamento tratado
    Capturando --> Limite: Tempo ou volume
    Capturando --> Falha: Erro
    Preparando --> Interrompida: Reabertura sem término confirmado
    Capturando --> Interrompida: Reabertura sem término confirmado
    [*] --> Importado: Importação bem-sucedida
    Concluida --> [*]
    Limite --> [*]
    Falha --> [*]
    Interrompida --> [*]
    Importado --> [*]
```

Rótulos reais: `Concluída`, `Limite atingido`, `Interrompida`. Interrompida é classificação de leitura de resumo antigo. Não confundir estado de execução com presença/estado de análise do protocolo.

## Modelo de dados

```mermaid
erDiagram
    SESSION ||--o{ SESSION_PROTOCOL : seleciona
    PROTOCOL_DEFINITION ||--o{ SESSION_PROTOCOL : referencia
    SESSION ||--o{ CAPTURE_SNAPSHOT : associa
    CAPTURE_SNAPSHOT ||--o{ PROTOCOL_COUNT : conta
    CAPTURE_SNAPSHOT ||--o{ PACKET_EVIDENCE : amostra
    CAPTURE_SNAPSHOT ||--o{ CAPTURE_FILE : referencia
    SESSION {
      string id PK
      string name
      string location
      string interface_id
      string created_utc
      string catalog_version
    }
    SESSION_PROTOCOL {
      string session_id FK
      string protocol_id
      int ordinal
    }
    CAPTURE_SNAPSHOT {
      string id
      string session_id
      int schema
      string analyzer
      string status
      long packets
    }
```

Modelo lógico: somente SESSION e SESSION_PROTOCOL são tabelas SQLite. Definições estão em código; snapshots/contagens/evidências em JSON, arquivos no disco. As associações JSON não são FKs do banco.

## Implantação

```mermaid
flowchart TB
    SETUP[Instalador Inno por usuário] --> BIN[Programs / NetTools AV: EXE e runtime]
    SETUP --> SHORT[Atalho no menu Iniciar]
    subgraph Windows
      BIN --> UI[Processo UI]
      UI --> HOST[Processo de captura oculto]
      HOST --> NPCAP[Npcap instalado separadamente]
      UI --> DATA[LocalAppData / NetToolsAV]
      HOST --> DATA
      DATA --> DB[(SQLite)]
      DATA --> FILES[PCAPNG e JSON]
    end
    MUTEX[Mutex de aplicação] -. Bloqueia atualização e remoção em uso .-> SETUP
    UI --> MUTEX
    HOST --> MUTEX
```

## Liberação planejada

```mermaid
flowchart LR
    CODE[Código e documentação] --> CHECK[Build e verificações]
    CHECK --> PACK[Pacote e instalador]
    PACK --> QA[Ensaios offline, UI e campo]
    QA --> GATE{Todos os critérios obrigatórios aceitos?}
    GATE -->|Não| PREVIEW[Prévia com pendências]
    GATE -->|Sim| APPROVE[Aceite de produto, QA e release]
    APPROVE --> RELEASE[1.0 homologada]
```

Esta última figura é o processo exigido de liberação; a etapa 1.0 ainda não ocorreu.
