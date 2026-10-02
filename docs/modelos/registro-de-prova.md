# Modelo de registro de prova

Modelo documental; copiar para um registro identificado somente quando preparar a execução. Nenhum campo em branco significa aprovado.

## Identificação

| Campo | Preenchimento |
|---|---|
| ID PV/TP e repetição | A preencher |
| RFs / ADRs relacionados | A preencher |
| Objetivo e critério de aceite | Definir antes da execução |
| Estado | Não executado |
| Implementador / operador / validador | A preencher; indicar acúmulo de papéis |
| Início/fim UTC e fuso local | A preencher |
| Aplicativo, commit, hash do binário | A preencher |
| SDK/runtime, perfis, regras, esquema e protocolo IPC/agente | A preencher |
| Windows/build/patch e hardware | A preencher |
| Adaptador, driver, interface e origem selecionadas | A preencher |
| Backend/driver de captura e opções | A preencher |
| Equipamentos, modelos e firmware | A preencher |
| Topologia/configuração de referência | Link e hash; não incluir segredos |
| Fonte normativa/documental e revisão | A preencher |

## Preparação e execução

1. Registrar pré-condições, alvos, taxa, duração, timeouts, limites de recursos e procedimento de cancelamento.
2. Confirmar ambiente isolado e configuração de referência; preservar baseline saudável.
3. Registrar passos exatos e resultado esperado por passo.
4. Introduzir uma alteração controlada quando aplicável, com instante e mecanismo.
5. Executar e medir; registrar perdas de captura e limitações.
6. Restaurar baseline e demonstrar recuperação.
7. Repetir falha/recuperação três vezes; ensaio longo segue duração integral prevista.

## Resultados

| Passo / repetição | Esperado | Observado | Medição/unidade | Evidência | Resultado |
|---|---|---|---|---|---|
| A preencher | A preencher | Não medido | Não medido | Não coletada | Não executado |

Conclusão da prova: aprovado, reprovado, bloqueado ou não aplicável com justificativa. Não confundir esses resultados com estados de diagnóstico do aplicativo. Registrar dados indisponíveis e impedimentos de preparação.

## Manifesto e acompanhamento

Relacionar logs, capturas, relatórios, snapshots, ferramentas independentes e seus hashes SHA-256; registrar janela, interface, número/offset dos pacotes e localização do arquivo. Indicar sanitização, responsável e retenção. Não armazenar credenciais ou capturas de clientes no repositório.

Defeitos encontrados: ID, gravidade, evidência e requisito afetado. Reexecução gera novo registro ligado ao anterior. Aceite informa nome/data e o alcance efetivamente demonstrado; resultado de simulação não é homologação de equipamento real.
