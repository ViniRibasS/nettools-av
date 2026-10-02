> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Base visual — NetTools AV

Revisão 0.1 • 01/10/2026 • Protótipo de UX anterior à interface WPF

## Direção

Pedido: usar o padrão de experiência do Codex/GPT Desktop. Adotar navegação lateral por projetos e sessões, área central de trabalho, painel contextual recolhível, tipografia discreta, superfícies neutras e ações compactas. A identidade continua NetTools AV. Esta é uma interpretação do padrão, não reprodução pixel a pixel de uma versão específica sem imagem de referência.

Referência consultada: [recursos do aplicativo, documentação oficial OpenAI](https://developers.openai.com/codex/app/features), redirecionada em 01/10/2026 para ChatGPT Learn. A referência orienta organização de projetos, trabalho e painéis; medidas, cores e componentes abaixo são decisões próprias deste protótipo.

## Entrega

Protótipo local em `prototype/`, feito com HTML/CSS/JavaScript sem dependências externas. Serve para validar aparência e fluxo sem instalar .NET ou antecipar driver, banco e rede. Não altera ADR-001: WPF permanece candidato para o produto Windows. Nenhum teste real de rede é executado. IPs, horários, métricas, eventos e capacidades são fictícios, indicados permanentemente como demonstração.

## Estrutura e comportamento

- Barra lateral: marca, nova sessão, busca, navegação principal e sessões do projeto Laboratório AV.
- Centro: sessão demonstrativa, seleção de contexto, abas de equipamentos/protocolos/eventos e área inferior de inclusão de alvo.
- Inspetor: equipamento selecionado, quatro dimensões de diagnóstico, origem e limitações.
- Fluxo inicial: abrir sessão → escolher interface demonstrativa → selecionar equipamento → inspecionar evidência → prévia de relatório.
- Nova sessão: formulário de nome e ponto de teste cria uma sessão vazia em memória.
- Busca: filtro de equipamentos pelo nome ou IP, com atalho Ctrl/Cmd+K.
- Tema claro/escuro, recolhimento de painéis e apresentação adaptável à largura.
- Relatório: prévia textual e download TXT demonstrativo, nunca apresentado como prova técnica real.

Sem assistente de IA funcional, autenticação, varredura, persistência ou captura real. Recarga reinicia os dados de demonstração. A ação de iniciar/pausar altera apenas o estado da simulação.

## Linguagem visual

| Elemento | Proposta |
|---|---|
| Navegação | 232 px, superfície levemente mais escura que o conteúdo |
| Inspetor | 296 px em desktop, recolhível |
| Fonte | Segoe UI / fontes do sistema; monospace para IP e valores técnicos |
| Corpo | 13–14 px; títulos de 20–26 px |
| Espaçamento | Escala 4, 8, 12, 16, 24 e 32 px |
| Controles | Cantos de 7–10 px; superfícies maiores de 12–16 px |
| Cor | Neutros quentes; verde/âmbar/vermelho apenas para estados com texto |
| Seleção | Realce neutro e borda sutil; não usar cor como único indicador |
| Acessibilidade | Controles nativos, rótulos, foco visível, Escape em diálogos, contraste e redução de movimento |

## Critérios de revisão

Verificar sessão demonstrativa, seleção de equipamento, mudança de abas, busca sem resultados, nova sessão vazia, inclusão validada de IPv4, pausar/retomar simulação, relatório, temas e painéis. Conferir desktop e largura estreita. As validações visuais não aprovam RFs de rede ou as provas PV do produto.

## Verificação realizada nesta revisão

Em 01/10/2026: sintaxe JavaScript aprovada por `node --check`; protótipo aberto no navegador do aplicativo; filtro de atenção retornou dois equipamentos; abas Protocolos/Eventos apresentaram conteúdo; busca sem resultado e por IP verificadas; seleção do Kramer atualizou o inspetor; nova sessão criada; IPv4 inválido recusado e válido cadastrado como não executado; prévia de relatório conferida com o novo alvo; simulação alternou iniciar/pausar; temas claro/escuro inspecionados; navegação e inspetor recolhidos/reabertos.

Tela desktop inspecionada em viewport padrão de 1280×720. Teste estreito solicitado ao navegador em 390×844, com área de conteúdo reportada em 375 px: largura de conteúdo igual à largura visível, sem overflow horizontal. A configuração temporária de viewport foi restaurada. Captura de tela em `prototype/screenshots/base-visual.png`.

Nenhum erro JavaScript apareceu na consulta ao console. O botão de download TXT foi acionado, mas a confirmação automatizada do evento de download excedeu o tempo da ferramenta: arquivo baixado não confirmado. A prévia textual foi verificada. Acessibilidade completa, comparação pixel a pixel com referência fornecida pelo usuário e implementação WPF continuam fora desta validação.


