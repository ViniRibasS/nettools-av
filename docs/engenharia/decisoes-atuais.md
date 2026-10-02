# Decisões arquiteturais atuais

Registro da implementação, 02/10/2026. IDs DEC distinguem estas decisões observadas dos ADRs propostos históricos. Mudanças futuras usam [modelo ADR](../modelos/adr.md).

| ID | Decisão e razão | Consequência/alternativa |
|---|---|---|
| DEC-01 | Protocolos como raiz por orientação do produto | Equipamentos só no laboratório; IP/MAC como atributos |
| DEC-02 | WPF/.NET para aplicativo Windows local | Reutiliza APIs Windows; WinUI/Avalonia não adotados |
| DEC-03 | Core, Infrastructure e Desktop separados | Domínio independente; UI ainda tem code-behind, MVVM é dívida |
| DEC-04 | Captura em processo filho do mesmo EXE | Falha nativa separada da janela; aumenta complexidade de IPC e encerramento |
| DEC-05 | Interop Npcap direto | SharpPcap/PacketDotNet não usados; ABI/memória nativa ficam sob responsabilidade do projeto |
| DEC-06 | Escritor/leitor PCAPNG próprios e limitados | Preserva formato necessário; não alegar suporte universal nem confundir PCAP com PCAPNG |
| DEC-07 | SQLite para sessão, arquivos para captura | Simplicidade local; backup precisa preservar ambos |
| DEC-08 | Snapshot e últimas 80 evidências | Memória limitada; detalhe completo exige PCAPNG, sem banco de todos os eventos |
| DEC-09 | Npcap externo | Sem redistribuição pressuposta; driver/permissões são pré-requisitos separados |
| DEC-10 | Inno, instalação por usuário, runtime incluído | Menos pré-requisitos do app; driver continua separado; assinatura pendente |
| DEC-11 | Prévia até homologação completa | Instalador funcional não muda sozinho o status para 1.0 |

Essas decisões não significam que todas as alternativas foram benchmarkadas. Fontes de tecnologia/protocolo estão em [04-fontes](04-fontes.md), [engenharia da captura](13-captura-real.md) e [instalador](../15-instalador.md), com seus limites de consulta. Ao usar fonte dinâmica para novo código, fixe versão/seção e revalide; não tratar uma URL histórica como revisão atual de norma.
