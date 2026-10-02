# Resumo técnico

Baseline observada: 02/10/2026, 0.2.0-preview.2. Uso interno inicial. Objetivo: observações por protocolo com contexto e evidência local para apoiar diagnóstico de redes AV.

| Elemento | Implementação |
|---|---|
| Plataforma | C#, WPF, XAML, code-behind e janelas auxiliares em C# |
| Alvo | net10.0 no domínio/infra/testes; net10.0-windows no Desktop; win-x64 |
| SDK/runtime da entrega | 10.0.401 / 10.0.12 |
| Banco | Microsoft.Data.Sqlite 10.0.12; SQLitePCLRaw 2.1.12; esquema 1 |
| Captura | Interop direto Npcap/libpcap; processo filho do próprio executável |
| IPC | stdin/stdout redirecionados, JSON por linha; sem API HTTP |
| Evidências | PCAPNG próprio, JSON, SHA-256 e ZIP |
| Instalador | Inno Setup 6.7.3 por usuário; runtime incluído, Npcap excluído |
| Licenças | Avisos em assets/licenses; sem licença pública definida para o produto |

Versões descrevem a entrega, não as mais recentes disponíveis. Atualizações exigem conferência de suporte, licença e segurança.

## Capacidades e limites

Sessão contém nome, local/ponto, interface e seleção. Dez identificadores possuem inspeção de cabeçalhos: ARP, ICMPv4, TCP, UDP, DNS, mDNS, IGMPv2, IGMPv3, PTPv2 e ICMPv6 sob `ndp`; este último não significa NDP semanticamente validado. Catálogo tem 26 entradas, incluindo perfis/tecnologias sem analisador.

Sem sondas ativas, agente remoto, remontagem TCP/IP, checksums, perda/jitter RTP, precisão PTP ou correlação Dante/AES67/Q-LAN. O rótulo “Projeto local” não equivale a uma entidade Projeto persistida. [Matriz](14-matriz-0.2-e-homologacao.md).

| Item | Limite |
|---|---|
| Duração | 1–3.600 s; UI propõe 60 s; default do contrato C# é 900 s |
| Volume | 1–1.024 MiB; UI propõe 256; não é quota de toda a pasta |
| Segmentos | Aproximadamente até 32 MiB, sem exclusão circular |
| Pacote | Até 65.535 bytes; captura direta Ethernet link type 1 |
| Filtro | BPF até 1.024 caracteres; vazio não filtra tráfego visível |
| Importação | Até 512 MiB/arquivo, 4 MiB/bloco; SHB/IDB/EPB |
| Evidências no resumo | Últimas 80; PCAPNG preserva os registros gravados |
| Histórico | Até 200 resumos na janela; não é política de retenção |
| Parada | Fecha stdin; após 5 s o cliente pode encerrar host forçadamente |

## Maturidade

39 verificações aprovadas, dois testes de erro do host, ciclo do instalador na máquina de desenvolvimento, 417 arquivos conferidos e 13 arquivos de usuário preservados. Quatro arquivos de campo com hashes e reanálise consistentes: 3.751, 3.773, 35.483 e 9.501 pacotes. Dois resumos de captura direta registram zero descartes do driver; não comprova ausência de perda na rede.

Sem assinatura digital e sem homologação integral em máquina limpa. [Evidências](../qualidade/evidencias.md) e [passagem para 1.0](../qualidade/plano-de-validacao.md).
