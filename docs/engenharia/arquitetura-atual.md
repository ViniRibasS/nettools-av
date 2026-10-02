# Arquitetura implementada

Baseline 0.2.0-preview.2. [Diagramas](../diagramas/README.md). A proposta histórica 02-arquitetura.md não descreve integralmente o código atual.

| Arquivo/camada | Responsabilidade e limite |
|---|---|
| Core/AnalysisSession.cs | Invariantes de sessão e relatório sem captura; sem I/O de banco |
| Core/ProtocolCatalog.cs | Definições/famílias/versões/limites; seleção não habilita parser |
| Core/TrafficAnalysis.cs | Snapshot, contadores, evidências, parsers e ProtocolView; sem remontagem |
| Infrastructure/SessionStore.cs | SQLite e transações; sem migração evolutiva ou backup automático |
| Infrastructure/EthernetInventory.cs | Interfaces locais; não varre equipamentos |
| Infrastructure/NpcapCapture.cs | Interop, BPF, limites, estatísticas; GUID exato, driver externo |
| Infrastructure/PcapNg.cs | Leitor/escritor de perfil limitado do formato |
| Infrastructure/CaptureArchive.cs | Importação, resumo, hashes, ZIP e relatórios |
| Desktop/MainWindow.xaml.cs | UI, comandos, sessão e seleção; code-behind |
| Desktop/CaptureWorker.cs | Host e cliente por streams; não é sandbox de segurança |
| Desktop/App.xaml(.cs) | Recursos, temas, startup e mutex |
| tests/NetToolsAV.Checks | Harness console; não usa xUnit; sem CI remoto configurado |

Todos os caminhos de produto acima estão em `src/NetToolsAV.*`. Desktop referencia Core/Infrastructure; Infrastructure referencia Core e SQLite; Core não referencia WPF/SQLite/Npcap.

## Fluxos

Startup normal resolve raiz padrão ou `--data-dir`, carrega sessões/último resumo e enumera interfaces locais. Na ausência de sessões, cria uma inicial. Captura exige interface explícita; importação não exige interface.

Cliente inicia o próprio executável com `--capture-worker`, oculto, e envia CaptureRequest. Host valida parâmetros e correspondência GUID, sem fallback de adaptador. Loop não bloqueante grava PCAPNG e publica resumo aproximadamente a cada segundo. Fechamento de stdin solicita parada. Saída terminal registra falha, limite ou conclusão; hashes são gerados ao final.

Importação copia o original, analisa a cópia em background e grava resumo/hash. Erro pode deixar arquivo preservado sem análise concluída. Relatórios usam snapshot, não consultas de rede.

## Concorrência e consistência

- Atualização visual via Dispatcher; captura nativa em processo separado.
- SessionStore usa lock por instância, conexões sem pooling e transações. Não foi qualificado para vários processos concorrentes ou vários usuários.
- Resumo usa temporário e substituição de arquivo; não é garantia de durabilidade de todo o conjunto sob queda de energia.
- UI/worker mantêm `Local\NetToolsAV.Application`; instalador/desinstalador detectam processo aberto. Esse mutex não implementa instância única do app.
- Resumo em Preparando/Capturando é apresentado como Interrompida na reabertura; não se fabrica conclusão.

## Fronteiras e dívida

Sem API pública, serviço HTTP/Windows, autenticação, telemetria implementada ou agente remoto. O protótipo localhost não participa do desktop.

Pendentes: ViewModels/comandos, tratamento uniforme de falhas de startup, migração/recuperação, pressão de I/O, término sob carga, novos analisadores e QA visual/acessibilidade integral. Não apresentar esses itens como recursos existentes.
