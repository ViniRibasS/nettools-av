> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Riscos, pendências e decisões do produto

Revisão 0.1 • 01/10/2026

## Informações confirmadas pelo usuário

- Uso interno inicialmente. Distribuição/comercialização futura não foi solicitada nem decidida.
- Laboratório disponível: dispositivos Dante AVIO, switch Cisco, dispositivos de vídeo Kramer KDS e computadores.
- A etapa atual deve criar estrutura e documentação de engenharia antes do código.

## Riscos técnicos

| ID | Risco / impacto | Tratamento proposto e condição de fechamento | Responsável |
|---|---|---|---|
| R01 | Teste sair por Wi-Fi/VPN e ser atribuído à Ethernet | PV-02 com observação externa; resultado inconclusivo/impedido quando origem não puder ser garantida | DEV/LAB |
| R02 | Captura ampliada confundida com visão completa da rede | Registrar ponto/direção/filtro/limites do SPAN; TP-007/010 | DEV/LAB |
| R03 | Política de Npcap incompatível com quantidade de instalações | Contar máquinas e verificar edição/termos aplicáveis ao uso interno; não empacotar driver por presunção | PROD/DEV |
| R04 | Biblioteca lê PCAPNG, mas não produz o arquivo requerido | PV-04; selecionar leitor e escritor com contratos separados | DEV |
| R05 | QDP sem layout verificável ou equipamento | Obter fonte/corpus e hardware; preservar RF-07 como pendente | DEV/PROD |
| R06 | Precisão/lock PTP inferidos indevidamente | Separar mensagens observadas de estado do endpoint; corpus negativo | DEV/QA |
| R07 | Fila ilimitada ou disco cheio causar falha silenciosa | Limites por bytes/itens, descartes e retenção rastreável; PV-03/05 | DEV |
| R08 | Nomes genéricos de fabricantes criarem promessa de integração | Identificar modelos/firmwares AVIO/Cisco/KDS e perfis; não presumir NDI em KDS | PROD/LAB |
| R09 | Backup inconsistente ou rotação perder prova | Snapshot e backup consistente; segmento expirado marcado; prova fixada fora da rotação | DEV/QA |
| R10 | UI elevada ou IPC permitir ações indevidas | Host restrito, ACL explícita e comando limitado; PV-03/06 | DEV/QA |
| R11 | Dependência proprietária atrasar homologação | Acesso a normas/SDK e equipamentos registrado antes do módulo; sem declarar integração fictícia | PROD/DEV |
| R12 | SDK local perto do fim de suporte | Preparar .NET 10 após seleção; não iniciar produto novo ancorado no SDK 8 existente | DEV |

## Decisões pendentes

| ID | Decisão | Estado e encaminhamento |
|---|---|---|
| D01 | Uso e distribuição | Uso interno confirmado; falta número de PCs/instalações e edição Npcap aplicável |
| D02 | Equipamentos de referência | Famílias informadas; coletar modelos, firmware, quantidades, adaptadores e capacidades |
| D03 | Base tecnológica | .NET 10/WPF/SQLite propostos; confirmar após PVs e fixar versões |
| D04 | Captura e instalação | Definir host sob demanda/serviço, acesso do driver e estratégia de instalador após PV-03/06 |
| D05 | Q-SYS e demais integrações | Definir acesso aos equipamentos/SDKs para homologação; não remover do escopo automaticamente |
| D06 | Relatórios e retenção | HTML/TXT/JSON propostos; confirmar necessidade de PDF e retenção além da captura circular |
| D07 | Responsáveis e aceite | Designar quem implementa, opera laboratório, valida e aceita; papéis podem se acumular |
| D08 | Assinatura e atualização | Definir certificado, canal interno e política offline; pacote assinado só quando comprovado |
| D09 | Metas e hardware | Confirmar CPU/RAM/disco/adaptador de referência, limites e métrica de memória |

As pendências não impedem a presente documentação. Antes de uma prova dependente, obter a informação necessária. Não é necessário solicitar nova autorização para consultas documentais ou revisão dos documentos já solicitados.

## Próxima sequência de engenharia

1. Completar D01/D02/D07/D09, detalhar seções das fontes básicas e resolver a fonte de estrutura QDP.
2. Revisar ADRs, selecionar versões concretas e condições de distribuição interna.
3. Preparar ambiente de desenvolvimento e laboratório para as PVs, mantendo o estado não executado até obter evidência.
4. Fechar decisões por resultados e então iniciar fundação e funcionalidades do aplicativo.

Nenhuma instalação, alteração de rede, captura, tráfego ativo ou aquisição foi feita nesta entrega.


