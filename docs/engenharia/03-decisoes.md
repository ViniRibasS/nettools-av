> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Registro de decisões de arquitetura

Revisão 0.1 • 01/10/2026

Estados usados: proposta, aceita para prova, confirmada por prova, substituída. Todas as decisões abaixo estão em **proposta**. Responsável técnico: Desenvolvimento; aceite de impacto no produto: Integrador. Pessoas ainda não designadas. A solicitação de estruturar/documentar não é tratada como homologação das tecnologias.

## ADR-001 — Windows, C#/.NET 10 e WPF

Contexto: aplicação de campo offline e Windows 11 x64. Decisão proposta: WPF/MVVM sobre .NET 10 LTS, com APIs nativas isoladas. Alternativas: WinUI 3 e Avalonia. Motivo: concentrar a primeira versão na plataforma exigida e manter a UI separada da coleta. Consequência: portabilidade da interface não é objetivo inicial; interop exige testes. Fontes F01–F04. Fechamento: PV-01 e PV-02, SDK fixado e matriz Windows/adaptador preenchida.

## ADR-002 — Backend de captura substituível

Contexto: driver tem requisitos próprios de instalação e licença. Decisão proposta: adaptar SharpPcap/Npcap atrás de contrato; PacketDotNet é opcional. Não acoplar regras aos objetos da biblioteca. Alternativas: outro wrapper ou backend Windows, ainda não qualificados. Consequência: trabalho de adaptação e inventário de dependências; distribuição do driver não é presumida. Fontes F05–F08. Fechamento: PV-03, versão exata auditada e decisão D01 de distribuição.

## ADR-003 — Captura em processo separado

Contexto: interface não deve funcionar permanentemente elevada. Decisão proposta: host restrito sob demanda e IPC local autenticado; serviço persistente apenas se necessário. Alternativa: captura no desktop, rejeitada para configurações que exijam elevação do processo. Consequência: protocolo IPC, supervisão, fila limitada e encerramento precisam de prova. Fontes F05/F15. Fechamento: PV-03/06, inclusive usuário comum e acesso negado.

## ADR-004 — SQLite e arquivos segmentados

Contexto: operação offline e sessões reproduzíveis. Decisão proposta: SQLite local para metadados, gravador dedicado e PCAPNG externo. Alternativas: banco servidor adiciona operação sem necessidade demonstrada; arquivo único de sessão dificulta consulta incremental. Consequência: sincronizar manifesto e segmentos, testar falhas e backup. Fontes F12–F14. Fechamento: PV-05 e TP-016/017/018 nas fases previstas.

## ADR-005 — PCAPNG como capacidade independente

Contexto: RF-06 exige importar e exportar PCAPNG. Decisão proposta: validar leitura e escrita separadamente, incluindo múltiplas interfaces/seções e timestamps. Se wrapper não escrever o perfil necessário, comparar biblioteca dedicada com escritor mínimo conforme formato; escolha só após prova e manutenção estimada. Não aceitar PCAP renomeado. Fontes F09/F10. Fechamento: PV-04. RF-06 continua obrigatório, mesmo se uma biblioteca falhar.

## ADR-006 — Diagnósticos baseados em evidências

Contexto: porta conhecida ou ausência de pacote não bastam para concluir saúde. Decisão proposta: observações imutáveis e regras versionadas com limitações, estados e referência ao ponto de medição. Alternativa rejeitada: semáforo geral calculado somente por ping ou portas. Consequência: resultados parciais são parte normal do produto; parsers proprietários dependem de fontes verificáveis. Referência: escopo, F17–F26. Fechamento: corpus negativo e TP-006/007/008/011/017.

## Política de alteração

Uma decisão fechada registra data, autor, versão testada, prova e consequências. Mudança mantém histórico e indica qual ADR substitui. Se afetar um RF ou marco, abrir mudança de escopo com motivo, impacto e aceite explícito do produto. Não excluir requisito obrigatório apenas por indisponibilidade de laboratório ou SDK.


