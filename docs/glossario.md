# Glossário

| Termo | Significado no projeto |
|---|---|
| Sessão | Contexto local de medição, com nome, ponto, interface e protocolos |
| Protocolo | Estrutura/método de comunicação; eixo principal da aplicação |
| Perfil | Combinação de protocolos/regras; Dante/AES67/Q-LAN não são um único protocolo de fio |
| Catálogo | Entradas disponíveis para seleção; pode incluir módulos sem analisador |
| Analisador/parser | Código que reconhece e interpreta um perfil limitado da mensagem |
| Snapshot | Resumo imutável por valor de uma execução em determinado momento |
| Evidência | Registro associado a pacote/protocolo; amostra limitada no resumo |
| PCAPNG | Formato de captura em blocos; app suporta um subconjunto |
| BPF | Expressão de filtro de captura; diferente do filtro de exibição do Wireshark |
| Npcap | Componente externo de captura; não redistribuído com o app |
| Link type | Encapsulamento dos bytes; 1 representa Ethernet no perfil atual |
| Snaplen | Limite de bytes copiados por pacote, 65.535 no perfil atual |
| MiB | 1.048.576 bytes; distinto de MB decimal |
| Truncado | Menos bytes capturados que comprimento original informado |
| Malformed | Estrutura inválida/incompleta para o parser; não prova defeito do transmissor |
| Uninterpreted | Registro/caminho não interpretado pelo perfil atual |
| Descarte do driver | Contador informado pela captura; não mede perda ponta a ponta |
| Observado | Estrutura reconhecida, não conformidade ou saúde |
| Homologação | Aceite documentado dos critérios obrigatórios no ambiente definido |
| ADR | Registro de contexto, decisão, alternativas e consequências |
| RF / RNF | Requisito funcional / não funcional |
| PV / TP | Prova de viabilidade / caso de teste da documentação de validação |
| SHA-256 | Hash de integridade; sozinho não autentica autoria |
| Self-contained | Pacote inclui runtime; não inclui todo pré-requisito externo, como Npcap |
