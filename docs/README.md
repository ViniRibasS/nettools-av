# Documentação do NetTools AV

Revisão de 02/10/2026, baseline 0.2.0-preview.2. Papéis de operação, desenvolvimento, QA, produto e release ainda sem designação nominal formalizada.

## Roteiros de leitura

| Público | Ordem sugerida |
|---|---|
| Usuário | [Manual](operacao/manual-do-usuario.md) → [capturas](operacao/capturas.md) → [suporte](operacao/suporte-e-recuperacao.md) |
| Desenvolvimento | [Resumo técnico](engenharia/resumo-tecnico.md) → [arquitetura](engenharia/arquitetura-atual.md) → [contratos](engenharia/dados-e-contratos.md) → [desenvolvimento](engenharia/desenvolvimento.md) |
| QA e laboratório | [Requisitos](engenharia/requisitos-rastreabilidade.md) → [plano](qualidade/plano-de-validacao.md) → [evidências](qualidade/evidencias.md) |
| Release | [Processo](engenharia/release.md) → [instalador](15-instalador.md) → [segurança](engenharia/seguranca.md) → [recuperação](operacao/suporte-e-recuperacao.md) |
| IA | [AGENTS.md](../AGENTS.md) → [guia](ia/guia-de-trabalho.md) → documentos do módulo |
| Produto | [Escopo vigente](engenharia/11-escopo-por-protocolos.md) → [capacidades](engenharia/14-matriz-0.2-e-homologacao.md) → [backlog atual](engenharia/requisitos-rastreabilidade.md) |

## Catálogo complementar

- [GitHub e organização da distribuição](operacao/github.md), [notas da prévia instalável](qualidade/release-0.2.0-preview.2.md).

- [Diagramas funcionais, técnicos e de dados](diagramas/README.md).
- [Registro da revisão documental](qualidade/revisao-documental.md).
- [Decisões implementadas](engenharia/decisoes-atuais.md) e [glossário](glossario.md).
- [Changelog](../CHANGELOG.md), [contribuição](../CONTRIBUTING.md) e [segurança](../SECURITY.md).
- Modelos: [prova](modelos/registro-de-prova.md), [defeito](modelos/defeito.md), [ADR](modelos/adr.md), [protocolo](modelos/protocolo.md), [release](modelos/release.md).

## Precedência e manutenção

Instruções atuais do produto definem escopo; a revisão 11 formaliza protocolos como raiz. Código e evidências comprovam implementação. Matriz 14, registro 15 e documentos atuais sem prefixo numérico consolidam a situação. Documentos 01–10, fundação 12, escopo 0.1 e protótipo são históricos; documento 13 registra decisões da captura. Não interpretar arquitetura proposta como implementada.

Toda mudança relevante atualiza comportamento/contrato, requisitos, operação, testes e changelog. Decisão arquitetural recebe ADR. Registre versão, data e evidência; preserve resultados históricos, sem transformar pendência em aprovação. Diagramas devem acompanhar o código.

Evidências brutas ficam em `artifacts/` ou no perfil do usuário, podendo faltar em outro checkout. Não copiar capturas de campo para o repositório. Documentos dentro do instalador são uma fotografia anterior a esta revisão; atualizar o pacote exige outro ciclo de release. Esta tarefa altera somente documentação.
