# Especificação de módulo de protocolo

Estado inicial: planejado. ID estável, nome, família, versão do analisador, requisito e responsável: preencher.

| Item | Definição requerida |
|---|---|
| Fonte | URL primária, revisão/commit, seção, data consultada e direito de uso |
| Perfil | Transporte/link type, versões, extensões suportadas e excluídas |
| Reconhecimento | Campos/assinaturas; distinguir heurística de confirmação estrutural |
| Validação | Comprimentos, contagens, checksums se aplicável, fragmentação/remontagem |
| Evidência | Campos, unidades, origem temporal, amostragem e limite de memória |
| Métrica | Fórmula, pré-condições, janela, clock rate e incerteza |
| Estados | Não executado, observado, não observado, inválido, inconclusivo |
| Corpus | Positivo, negativo, truncado, ambíguo, hostil, endian e limites |
| Referência externa | Ferramenta/versão e resultado esperado independente |
| Campo | Ponto/interface, tráfego autorizado e critérios de aceite |
| Compatibilidade | IDs, catálogo, snapshots, relatórios e migração |

Porta conhecida não basta quando payload precisa ser validado. Não transformar fonte comercial ou presença em conformidade. Promover de planejado a implementado/testado/homologado somente com evidências correspondentes.
