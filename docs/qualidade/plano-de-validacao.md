# Plano de qualidade e homologação

Baseline 0.2.0-preview.2. Plano complementa [07-validação](../engenharia/07-validacao.md) e [requisitos atuais](../engenharia/requisitos-rastreabilidade.md). Não redefine critérios para declarar resultados existentes suficientes.

## Níveis

| Nível | Método | Estado |
|---|---|---|
| Domínio/persistência | Harness Check, validação, isolamento, transação/rollback | Executado na baseline |
| Parser/formato | Corpus, prefixos truncados, 20 mil entradas aleatórias, endian/interfaces | Executado no alcance documentado |
| Comparação independente | tshark/capinfos/editcap | Executado em corpus e arquivos registrados |
| Host | GUID inexistente e BPF inválido sem abrir rede | Executado; não testa captura sob carga |
| Instalador | Bloqueio, instalação, reinstalação, remoção, hashes e preservação | Executado na máquina de desenvolvimento |
| Campo | Evidências salvas, hashes e reanálise | Parcial, quatro arquivos; sem campanha integral |
| UI | Abertura, sessão, tema claro e dados existentes | Parcial; escuro, diálogos e exportação visual integral pendentes |
| Aceite | RF/PV/TP aplicáveis + assinatura de responsáveis | Não concluído |

## Procedimento de teste

Antes: definir requisito, build exata, ambiente, pré-condições, fonte e resultado esperado. Escolher fixture sintética quando possível. Durante: registrar comando/ação, observado, duração, falhas, logs e hash. Depois: classificar aprovado/reprovado/bloqueado/não executado/removido, preservando evidência e limitações.

Use [registro de prova](../modelos/registro-de-prova.md). “Não executado” não é aprovação; requisito removido também não. Quem desenvolveu pode registrar teste, mas não alegar revisão independente.

## Casos obrigatórios antes de 1.0

- Completar parsers e métricas obrigatórios com corpus positivo/negativo/truncado e perfis/fontes definidos.
- Interface e origem das provas; mudança/perda de link; nenhum fallback silencioso.
- Filtros, tráfego vazio, limite de tempo/volume, segmentação, disco cheio, permissão negada, alta taxa e descartes observáveis.
- Parada, fechamento da janela e término abrupto do pai/host, sem processo residual ou resultado final falso.
- PCAPNG externo, metadados/endereçamento suportados e arquivos hostis; limites explícitos.
- Persistência, migração, recuperação, backup/restauração, interrupção de gravação e esquema futuro.
- Windows x64 em máquina limpa; sem SDK, com/sem Npcap, política de acesso; upgrade de versão anterior, desinstalação, preservação de dados e assinatura.
- UI em temas claro/escuro, teclado/foco, escala/DPI, mensagens, importação, histórico, relatórios e exportação.
- Laboratório AV e agente remoto/provas ativas quando aplicáveis ao escopo obrigatório.
- Estabilidade: 8 h para marco MVP e 24 h para RC, com memória/CPU/disco/contadores e critérios definidos antes do ensaio.

PV-01 a PV-06 e TP históricos permanecem rastreáveis conforme revisão 11. Não aprovar toda PV por um subcaso. Não há meta numérica de throughput homologada; deve ser definida junto de hardware/perfil antes do teste.

## Critério de saída

Requisitos obrigatórios aceitos, defeitos bloqueantes resolvidos, evidências reproduzíveis, limites conhecidos explícitos, pacote íntegro e aprovação de produto/QA/release registrada. Se uma pessoa acumula papéis, registrar. A alteração de escopo só ocorre por decisão explícita do produto, não por conveniência do teste.
