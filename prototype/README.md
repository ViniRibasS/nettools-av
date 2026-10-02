# Protótipo visual NetTools AV — revisão 0.2

**Diretriz vigente:** protocolos são a raiz da aplicação. O [escopo por protocolos](../docs/engenharia/11-escopo-por-protocolos.md) substitui os fluxos anteriores de cadastro e diagnóstico de equipamentos.

Agora: sessão → protocolos/perfis → observações, critérios, métricas, fluxos e eventos. Busca por protocolo/família/versão; inclusão pelo catálogo, sem duplicatas; seleções novas em Não executado. IPs são somente atributos técnicos dos fluxos. Relatórios agrupam exclusivamente protocolos. O layout, os temas e os painéis foram preservados.


Abra `index.html` diretamente em um navegador. Funciona offline, sem instalação, build ou recursos externos. Para uma prévia local via HTTP, execute nesta pasta `python -m http.server 8765 --bind 127.0.0.1` e abra `http://127.0.0.1:8765`.

Esta base explora o padrão de interação do Codex Desktop, adaptado a redes AV. Não é a aplicação WPF nem executa testes de rede. Todos os dados são fictícios. As alterações ficam em memória e são reiniciadas ao recarregar.

Fluxos: selecionar protocolo, consultar critérios e limites, filtrar estados, alternar abas de protocolos/fluxos/eventos, buscar no catálogo, criar sessão, incluir protocolo, alternar interface demonstrativa, simular/pausar, consultar/baixar resumo TXT, recolher painéis e alternar tema.

Atalhos: Ctrl/Cmd+K para busca; Ctrl/Cmd+N para nova sessão; Escape fecha diálogos; setas percorrem as abas quando focadas. Os atalhos do navegador podem prevalecer fora da página.

As áreas de captura deixam explícita a futura implementação. O protótipo não aceita arquivos PCAPNG, não usa credenciais e não persiste dados. Modelos e firmware de AVIO, Cisco e Kramer KDS permanecem a confirmar.

Especificação: [base visual](../docs/engenharia/10-base-visual.md).
