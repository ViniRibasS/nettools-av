# Fundação nativa — primeira implementação

01/10/2026 • Incremento de desenvolvimento, não release homologada

Autorização: iniciar a implementação, após aprovação do layout e mudança para protocolos como núcleo. Esta etapa concretiza C#/.NET 10 e WPF em uma aplicação executável, mantendo os demais gates de viabilidade abertos.

## Alcance deste incremento

- Shell Windows com sidebar, sessões, tabela de protocolos e inspetor.
- Catálogo de protocolos/perfis, busca e inclusão sem duplicação.
- Sessões e seleção de protocolos persistidas localmente em SQLite.
- Enumeração real de interfaces Ethernet, configuração IPv4/IPv6, estado e velocidade disponíveis no Windows.
- Seleção de interface explícita por sessão, sem alterar configuração do sistema.
- Relatório textual dos dados da sessão, sem resultados inventados.
- Build, verificação automatizada de domínio/persistência e prova de abertura da UI.

Não inclui captura, parsers, sondas, classificação de tráfego ou inferência de saúde. Todos os protocolos começam Não executado, com contagens indisponíveis, não zero. Botões de captura deixam explícita a indisponibilidade. Somente o protótipo web contém observações fictícias.

## Estrutura concreta

`src/NetToolsAV.Core`: catálogo, sessões, invariantes e relatório.
`src/NetToolsAV.Infrastructure`: SQLite e leitura de interfaces locais.
`src/NetToolsAV.Desktop`: aplicação WPF e apresentação.
`tests/NetToolsAV.Checks`: verificações executáveis de integridade e persistência.

Escolher a pasta de dados pelo argumento `--data-dir` para testes. Operação normal usa a pasta LocalApplicationData do usuário. O programa não exige elevação. Gravação SQLite tem transação e versão de esquema; banco mais novo é recusado. Aplicativo não usa internet para executar.

## Critérios de término do incremento

Compilar a solução, executar verificações de cadastro/duplicação, isolamento de sessões, reabertura e esquema incompatível; abrir aplicativo e conferir navegação e dados locais. Registrar o que foi efetivamente executado. Não declarar RF-06/07 ou PV-02/03/04 aprovados pela existência da UI.

## Evidências deste incremento

- SDK local 10.0.401, Microsoft.Data.Sqlite 10.0.12 e dependências fixadas em arquivos de lock. Runtime alvo win-x64 declarado para manter build e publicação consistentes.
- `scripts/build.ps1 -Target Check`: Release compilado sem avisos ou erros; 12 verificações aprovadas. Incluem reabertura dos dados, isolamento, rollback após falha induzida e preservação integral de banco com esquema futuro.
- Pacote com runtime incluído gerado em `artifacts/app`; aplicativo aberto no Windows local. Não foi validado em máquina limpa; instalador e assinatura continuam pendentes.
- Conferência nativa de abertura, tabela e inspetor; busca PTP restringiu a tabela a PTPv2 sem alterar a seleção persistida de protocolos. Ajustado o estilo explícito das janelas para aplicação do fundo e texto do tema.
- UI ainda usa eventos de apresentação no code-behind; domínio e persistência estão separados. A evolução para MVVM permanece no plano antes de incorporar captura e processamento contínuo.
- Não executadas provas de rede, medições de precisão, captura, sondas ou homologação RF/PV. Catálogo não equivale a parsers implementados.
- Publicação final pelo script com dependências bloqueadas concluída; reabertura e legibilidade do catálogo no tema escuro conferidas. Prévia em `artifacts/qa/nettools-av.jpg`; hashes SHA-256 dos arquivos do pacote em `artifacts/package-sha256.json`.
