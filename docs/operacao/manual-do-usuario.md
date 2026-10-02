# Manual do usuário

Versão 0.2.0-preview.2 · Windows x64 · uso interno · homologação completa pendente.

## Instalação e primeira abertura

1. Use o instalador `NetToolsAV-0.2.0-preview.2-win-x64-setup.exe` fornecido pela equipe. Compare SHA-256 com o manifesto da entrega quando disponível.
2. Finalize capturas e feche NetTools AV antes de instalar ou atualizar. O instalador bloqueia aplicativo/host aberto; não encerra a captura automaticamente.
3. Execute o instalador. A instalação é por usuário, sem instalação de driver e sem alteração de rede. O atalho da área de trabalho é opcional; há atalho no menu Iniciar.
4. Abra NetTools AV pelo menu Iniciar. O runtime .NET já acompanha a aplicação. SDK e Wireshark não são necessários para uso normal.

Para captura ao vivo, Npcap precisa estar instalado e acessível ao usuário, conforme o ambiente da empresa. Importação funciona sem Npcap. O instalador desta prévia não tem assinatura digital de produção; se a política corporativa bloquear execução, encaminhe à TI, sem contornar a política.

## Conhecer a tela

| Área | Uso |
|---|---|
| Barra lateral | Nova sessão, busca, Protocolos, Capturas, Relatórios e tema |
| Sessões | Alternar conjuntos de observações e contexto |
| Centro | Protocolos selecionados/observados, contagens, presença e análise |
| Interface Ethernet | Escolha explícita do ponto local para capturar |
| Inspetor | Transporte, limites e observações do protocolo selecionado |
| Catálogo | Acrescentar protocolo à sessão sem iniciar testes |

Logo RJ45/arroba identifica o aplicativo. Não há painel de equipamentos. “Projeto local” é contexto visual da prévia; não existe gerenciamento de vários projetos na versão atual.

## Criar uma sessão

Clique em **Nova sessão**. Informe nome (até 70 caracteres) e ponto/local (até 100), sem caracteres de controle. Use nomes que identifiquem a prova, por exemplo “Multicast — bancada A”. Acrescente protocolos no catálogo; duplicatas não são adicionadas. Sessão nova não contém resultado aprovado ou observação fictícia.

Selecionar protocolos não aplica automaticamente um filtro de captura. O filtro BPF é configurado separadamente. Protocolos reconhecidos na análise podem aparecer mesmo sem seleção anterior.

## Importar uma captura existente

1. Selecione a sessão que receberá o resultado.
2. Clique em **Importar** e escolha arquivo `.pcapng` de até 512 MiB.
3. Aguarde a análise. O app copia o original para sua pasta de dados e analisa a cópia.
4. Confira estado, total de pacotes, janela temporal e limitações no inspetor/relatório.

O importador suporta um perfil do PCAPNG, não todo arquivo que o Wireshark consegue abrir. PCAP clássico, ZIP e certos blocos não são aceitos. Arquivo inválido não recebe resultado concluído. Não renomeie `.pcap` para `.pcapng` esperando conversão.

## Capturar ao vivo

Use rede/ponto de medição autorizado. O aplicativo não configura espelhamento do switch, não ativa modo monitor Wi-Fi e não gera pacotes de teste.

1. Selecione a sessão e a interface **Ethernet**. Use atualizar interfaces se necessário; confira o identificador e os dados locais.
2. Clique em **Capturar**. Confira a interface mostrada no diálogo.
3. Informe BPF opcional, duração (1–3.600 s) e limite (1–1.024 MiB). Defaults da UI: 60 s, 256 MiB, modo promíscuo desligado.
4. Clique em **Iniciar captura**. Acompanhe contadores e estado; para terminar antes do limite, clique em **Parar**.
5. Aguarde estado terminal e consulte **Capturas**. Não trate uma execução em andamento como evidência final.

Modo promíscuo não garante visibilidade de tráfego de outras portas. Visibilidade depende do ponto e da rede; limite de observação não é falha de protocolo. O app não troca automaticamente para Wi-Fi ou outro adaptador.

## Interpretar resultados

| Rótulo | Significado |
|---|---|
| Não executado | Sem análise anexada |
| Observado | Estrutura reconhecida na janela; não comprova saúde ou conformidade |
| Não observado | Nenhuma mensagem reconhecida; considere filtro, ponto e alcance do parser |
| Sem analisador | Entrada de catálogo ainda sem interpretação implementada |
| Inspeção parcial | Apenas o perfil de campos implementado foi examinado |
| Inconclusivo | Falha/interrupção impede conclusão completa |

Contagens de camadas se sobrepõem: DNS também pode contar em UDP. Não some a coluna de protocolos como total de pacotes. `indisponível` em descartes não equivale a zero. Zero descartes do driver não mede perda antes do ponto de captura. Datas devem ser comparadas com fuso/offset; relatórios preservam contexto temporal.

## Histórico, relatórios e evidências

Em **Capturas**, escolha uma execução. A janela carrega até 200 resumos e mostra até 80 evidências por snapshot; os PCAPNG podem conter muito mais pacotes. A UI não oferece exclusão nem retenção automática.

**Relatório** exporta HTML, TXT ou JSON do contexto selecionado. Para compartilhar evidências completas, use **Exportar evidências ZIP** no histórico, escolhendo nome novo fora da pasta da captura. Revise destinatário e conteúdo: PCAPNG pode conter payload, endereços e nomes internos. Exportar não envia arquivos pela rede.

Sem captura, relatório registra somente a configuração. ZIP não substitui backup de todas as sessões. [Backup e recuperação](suporte-e-recuperacao.md).

## Atualização e remoção

Faça backup antes de atualizar, feche o aplicativo e use o instalador aprovado. Não há atualização automática. Desinstale pelo gerenciamento de aplicativos do Windows; sessões/capturas são preservadas em `%LOCALAPPDATA%\NetToolsAV`. Não remova essa pasta para resolver um erro sem preservar cópia.

Para limitações técnicas, consulte [capturas](capturas.md) e [matriz de capacidades](../engenharia/14-matriz-0.2-e-homologacao.md).
