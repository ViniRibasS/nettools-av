# Captura real e candidato para uso interno

01/10/2026. Este incremento evolui a fundação, preservando protocolos como unidade de análise.

## Decisões antes da implementação

- Captura passiva por Npcap instalado separadamente. Não redistribuir ou instalar driver automaticamente. Adaptador Windows deve corresponder exatamente ao identificador enumerado pelo Npcap.
- Host de captura separado, iniciado pelo desktop com canais padrão redirecionados e sem shell. Não há endpoint IPC público. EOF do canal de controle encerra o host quando o desktop desaparece. Não elevar a interface gráfica.
- API nativa Npcap carregada somente do diretório System32/Npcap. Leitura não bloqueante permite cancelamento mesmo sem tráfego; timeout de buffer não é garantia de cancelamento.
- Gravação PCAPNG SHB/IDB/EPB, Ethernet, timestamps em microssegundos. Segmentos limitados e limite total; sem remoção automática de evidências antigas. Manifesto e resumo por captura.
- Importação com limites de tamanho, validação de blocos e reconhecimento explícito de capacidades. Preservar o original; relatório não se apresenta como exportação sem perdas dos metadados originais.
- Presença e estrutura observada não certificam funcionamento, sincronismo, entrega ou conformidade. Protocolos ainda sem analisador continuam identificados como não implementados.
- Listas suspensas com superfícies, foco, seleção e bordas do tema. Logo vetorial original: cabo em forma de arroba terminado em conector RJ45.

## Fontes de implementação

[Npcap API](https://npcap.com/guide/wpcap/pcap.html), [leitura não bloqueante](https://npcap.com/guide/wpcap/pcap_setnonblock.html), [estatísticas](https://npcap.com/guide/wpcap/pcap_stats.html), [filtros BPF](https://npcap.com/guide/wpcap/pcap_compile.html), [PCAPNG draft 05](https://www.ietf.org/archive/id/draft-ietf-opsawg-pcapng-05.html), [IPv4 RFC 791](https://www.rfc-editor.org/rfc/rfc791), [DNS RFC 1035](https://www.rfc-editor.org/rfc/rfc1035).

## Critérios da entrega

Testar parsers com pacotes válidos, inválidos e truncados; ler arquivo independente e abrir escrita própria em ferramenta independente; testar parada sem tráfego e erro de filtro; conferir tema e identidade visual; produzir pacote e matriz de capacidades. Prova passiva local depende da interface indicada pelo usuário. Homologação 1.0 exige os casos de laboratório e duração definidos no escopo, não apenas compilação.
