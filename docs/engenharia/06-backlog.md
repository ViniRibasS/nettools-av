> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Backlog e rastreabilidade

Revisão 0.1 • 01/10/2026 • Todos os RFs: planejados, sem execução

Papéis: DEV = Desenvolvimento; PROD = responsável pelo produto/integrador; LAB = responsável pela rede de laboratório; QA = validação; REL = release. São responsáveis por papel, ainda sem designação nominal. P0 = crítico para viabilidade/segurança; P1 = obrigatório no marco indicado. Prioridade não dispensa requisito.

## Requisitos

| RF | Marco / prioridade | Responsável | Aceite verificável resumido | Dependências | Provas |
|---|---|---|---|---|---|
| RF-01 | MVP / P0 | DEV + LAB | Interface, configuração e contadores conferem com host; mudança de link preserva identidade | APIs por campo; adaptador; PV-01/02 | TP-001/002/014 |
| RF-02 | MVP / P1 | DEV + PROD | Projeto/local/destino/perfil persistem e reabrem; IP/faixa inválida é recusada; seleção de alvos explícita | Contratos; PV-05 | TP-015/017 e TP-023 proposto |
| RF-03 | MVP / P0 | DEV + QA | Respostas, RTT e ausências coincidem com referência; estado respeita método e histerese | RF-01/02; PV-02 | TP-003/014/020 |
| RF-04 | MVP / P0 | DEV + LAB | Rota e origem corretas; TTL da sonda/recebido distintos; salto silencioso não vira falha certa | RF-01; PV-02 | TP-002/004/005 |
| RF-05 | MVP / P0 | DEV + QA | TCP e consulta UDP documentada correlacionam respostas; timeout UDP inconclusivo | RF-01/02/04; perfil de consulta | TP-003 e TP-024 proposto |
| RF-06 | MVP / P0 | DEV + QA | Capturar, filtrar e fazer round-trip PCAPNG; rotação e perdas visíveis; arquivo malformado controlado | PV-03/04; decisão do driver | TP-010/015/016 e TP-025 proposto |
| RF-07 | MVP / P0 | DEV + QA | ARP/ICMP/mDNS/IGMP/PTP/QDP com campos e evidências no corpus positivo/negativo/truncado | RF-06; fontes e QDP verificável | TP-006/007/008/011 |
| RF-08 | MVP / P0 | DEV + LAB | Evidências iniciais Dante/AES67 sem validação integral falsa; perfil testado em hardware | RF-07; AVIO; fontes PTP/AES67 | TP-006/008/011 |
| RF-09 | MVP / P1 | DEV + PROD | Histórico e eventos preservados; relatório/texto reproduzem snapshot e limitações | RF-02/03/06/07/08; PV-05 | TP-006/014/017/020/022 |
| RF-10 | 1.0 / P1 | DEV + QA | RTP/RTCP/SAP/SDP/RTSP nos casos declarados; perda/jitter com pré-condições e reordenação | RF-06; clock rate; fontes | TP-009/010/011 |
| RF-11 | 1.0 / P1 | DEV + LAB | Protocolos de descoberta e IPv6 básico conferem com fixtures/equipamento | RF-01/06; fontes e laboratório IPv6 | TP-005/011 |
| RF-12 | 1.0 / P1 | DEV + LAB | NDI/Art-Net/sACN/Modbus com matriz e casos positivo/negativo/incompleto; Modbus somente leitura | SDKs, normas, endpoints | TP-011; subcasos por módulo |
| RF-13 | 1.0 / P1 | DEV + LAB | Uma família Cisco identificada; campos suportados conferidos; autenticação/objeto ausente tratados | Modelo, MIBs/API, credencial de leitura | TP-012/022 |
| RF-14 | 1.0 / P1 | DEV + LAB | Agente autenticado; provas por sentido; multicast controlado, limites e parada | Dois PCs; VLANs; protocolo versionado | TP-013/021 |
| RF-15 | 1.0 / P0 | DEV + REL | Instalação, atualização, recuperação, migração e retenção comprovadas | PV-05/06; empacotamento; versões | TP-016/018/019/020/022 |

Os aceites acima complementam, sem substituir, os detalhes do escopo. QA registra resultados; PROD aceita o comportamento e REL verifica evidências de release. Se a mesma pessoa acumular papéis, registrar isso sem alegar revisão independente.

## Trabalho de engenharia antes do código de produto

| ID | Entrega | Responsável / prioridade | Dependência e conclusão |
|---|---|---|---|
| ENG-01 | Inventário de modelos, firmware, adaptadores e computadores | PROD + LAB / P0 | Parcialmente informado; fechar ficha do laboratório |
| ENG-02 | Decisão de uso e licença de captura | PROD + DEV / P0 | Uso interno confirmado; falta quantidade de instalações/edição |
| ENG-03 | Baseline de versões e avisos de dependências | DEV / P0 | .NET 10 candidato; ainda sem pacote fixado |
| ENG-04 | Fontes por protocolo/campo e perfis AVIO/Cisco/KDS | DEV + LAB / P0 | Registro inicial pronto; lacunas em 04-fontes |
| ENG-05 | Plano PV-01 a PV-06 e matriz RF–TP | DEV + QA / P0 | Documentado nesta entrega; provas não executadas |
| ENG-06 | Papéis nominais, metas finais, retenção e formatos de relatório | PROD + QA / P1 | Propostas registradas; fechamento pendente |
| ENG-07 | Revisão dos ADRs e passagem da fase 0 | DEV + PROD / P0 | Depende dos itens anteriores; fase 0 ainda aberta |

## Incrementos planejados

Fase 1 inicia pelas provas de UI/build, origem, captura, PCAPNG, persistência e instalação. Depois, organizar tarefas curtas por RF: inventário/cadastro → testes básicos → captura/parsers → diagnóstico/relatório. Infraestrutura gerenciada, agente e protocolos ampliados mantêm seus marcos originais.

Ao iniciar desenvolvimento: Git com branches curtas, revisão registrada, build limpo, análise estática e testes pertinentes. Pipeline sem hardware executa corpus sintético; laboratório possui execução separada. Não foi criada configuração de CI ou repositório remoto nesta etapa. Sem orçamento de prazo global antes das provas de viabilidade.


