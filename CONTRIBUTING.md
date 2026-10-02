# Contribuição e manutenção

Projeto de uso interno, sem política pública de contribuição/licença definida. Não publicar código ou dados por pressuposição. [Índice](docs/README.md) e [guia de desenvolvimento](docs/engenharia/desenvolvimento.md).

## Mudanças

Abra uma alteração com problema, requisito afetado, comportamento esperado e limites. Use branches curtas quando houver fluxo Git estabelecido; neste ambiente, prefixo padrão `codex/`. Evite refatoração ampla sem relação com o objetivo. Preserve alterações de outros autores.

Código deve respeitar Nullable, warnings-as-errors e locks. Core não depende de UI/banco. Nomes/IDs de protocolo precisam ser estáveis; mudanças de schema ou interpretação exigem compatibilidade e documentação. Não adicionar recursos centrados em equipamentos.

## Revisão

- O comportamento corresponde ao requisito vigente e não fabrica resultados?
- Há limites para entrada hostil, cancelamento e falhas de I/O?
- Testes cobrem comportamento relevante e casos de erro, sem somente espelhar implementação?
- Logs/relatórios preservam privacidade, método e limites?
- Manual, matriz, contratos e changelog refletem a mudança?
- Resultados alegados têm evidência e não confundem teste local com homologação?

Não criar PR, tag ou publicação sem destino/escopo autorizado. Descrição da revisão deve apresentar problema, mudança, verificações executadas e pendências. [Modelo de defeito](docs/modelos/defeito.md); [modelo ADR](docs/modelos/adr.md).
