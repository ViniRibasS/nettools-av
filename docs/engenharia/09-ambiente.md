> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Ambiente local observado

Data: 01/10/2026 • Inspeção somente leitura

| Item | Observação | Limitação |
|---|---|---|
| Plataforma do processo | Windows, RID win-x64, versão reportada 10.0.26100 | Não determina edição, patch completo ou homologação Windows 11 |
| SDK .NET | 8.0.319; MSBuild 17.10.46 | SDK 10 não apareceu em `dotnet --info` |
| Host .NET | 8.0.31, x64 | Não comprova execução do produto candidato |
| Runtimes | Famílias .NET 3.1, 6 e 8 presentes | Inventário não altera nem recomenda remoção de componentes de outros programas |
| Workloads | Nenhuma workload listada | Não equivale a ausência de todos os recursos desktop |
| Git | 2.53.0.windows.3 | Disponível; pasta não tinha `.git` na listagem inicial |
| Npcap | Serviço `npcap` encontrado em estado Running | Versão, licença, opções, acesso ao driver e captura ainda não verificados |
| TShark | Não localizado por `Get-Command` | Pode existir fora do PATH; não afirmar que não está instalado |
| Conteúdo inicial | Arquivo de escopo de implementação | Não existiam projetos/código na listagem inicial |

Métodos: listagem da pasta, localização de comandos, `dotnet --info`, `git --version` e consulta do estado do serviço Npcap. Não foram coletados IPs, MACs, credenciais ou capturas da rede. Não foi executado ping ou teste contra equipamento.

Para baseline futuro, coletar CPU/RAM/disco, edição/build/patch do Windows, identificação do adaptador, driver, versões exatas de bibliotecas/driver de captura e ferramentas de referência. Registrar antes do benchmark, sem aproveitar este inventário parcial como prova de compatibilidade.


