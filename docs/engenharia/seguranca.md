# Segurança, privacidade e limites de confiança

Baseline 0.2.0-preview.2. Descreve controles existentes e lacunas; não representa certificação ou auditoria independente.

## Ativos e fronteiras

Dados: banco de sessões, PCAPNG com payload, endereços, nomes internos, relatórios e hashes. Entradas não confiáveis: pacotes, PCAPNG importado, JSON arquivado, texto de sessão e filtro BPF. Fronteiras: UI → processo filho → DLL nativa/driver; sistema de arquivos do perfil; exportação manual a terceiros.

| Risco | Controle atual | Limitação/ação pendente |
|---|---|---|
| Parser com dados truncados/hostis | Checagem de comprimentos, limites e corpus aleatório/truncado | Não é fuzzing contínuo nem prova de segurança completa |
| DLL substituída em diretório de trabalho | Caminho System32/Npcap e flags de carregamento | Instalação/atualização do driver depende do ambiente |
| Captura na interface errada | Correspondência GUID exata, sem fallback | Confirmar ponto/interface no ensaio de campo |
| HTML injetado em relatório | HtmlEncode do conteúdo textual | Revisar novos formatos ao acrescentá-los |
| Escrita indevida por exportação | Nome novo, destino fora da captura e temporário | Arquivos .partial podem persistir após falha |
| Perda de dados na atualização | Binário/dados separados e mutex | Sem backup automático, rollback completo ou anti-downgrade geral |
| Exposição de tráfego | Armazenamento local e exportação manual | Sem criptografia/redação próprias; PCAP pode conter segredos |
| Esgotamento de disco/memória | Limites por captura/arquivo, 80 evidências | Sem retenção automática/quota global; IPC limita depois de ler a linha |
| Adulteração de evidência | Manifesto SHA-256 | Não assina autoria, não cobre summary, não é verificado automaticamente no histórico |

## Privilégios e rede

App e instalador operam no usuário. Não há elevação automática do app, alteração de firewall/adaptador, serviço remoto ou envio de teste. Npcap pode impor permissões próprias. Não solicitar que usuários desativem controles de segurança para executar a prévia.

Isolamento do host ajuda estabilidade, mas não é sandbox: ele opera com permissões do usuário e acessa arquivos/driver. IPC não é API pública autenticada. Não reutilizá-lo como agente de rede.

## Tratamento de dados

Não incluir captura de campo em Git, pacotes ou ferramenta externa sem autorização específica. Prefira fixtures sintéticas e casos mínimos. Anonimização, quando necessária, deve gerar cópia com novo hash e descrição da transformação, preservando original restrito. O app não anonimiza automaticamente.

Retenção, controle de acesso, criptografia de volume e canal de distribuição devem ser definidos pela organização. Não há prazo de retenção ou classificação corporativa embutidos. Consulte [backup](../operacao/suporte-e-recuperacao.md).

## Dependências e incidentes

Avisos de dependências em assets/licenses; Npcap separado. Manter locks, conferir avisos de vulnerabilidade na restauração, registrar versões exatas. A versão da DLL libpcap não comprova a versão do instalador/driver Npcap.

Em incidente, interromper a operação afetada sem destruir evidências, registrar versão/horário/ação e acionar responsável interno. [SECURITY.md](../../SECURITY.md). Sem canal externo ou SLA definidos; não inventar endereço de contato ou alegar cumprimento de norma.
