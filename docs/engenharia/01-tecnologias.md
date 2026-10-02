> **Documento histórico de planejamento.** Para o estado implementado e validado em 0.2.0-preview.2, consulte [documentação vigente](../README.md) e [matriz atual](14-matriz-0.2-e-homologacao.md). Propostas abaixo não constituem implementação ou homologação.

> **Atualização de escopo 0.2:** protocolos são a raiz do produto. Trechos centrados em equipamentos foram substituídos pela [revisão vigente](11-escopo-por-protocolos.md). Equipamentos permanecem apenas como recursos de laboratório.

# Tecnologias candidatas

Revisão 0.1 • 01/10/2026 • Recomendação de engenharia, sem homologação

## Critérios de escolha

Priorizar integração com Windows, controle explícito da interface de rede, operação offline, uso por técnico em campo, isolamento de privilégios, manutenção e distribuição reproduzível. Desempenho deverá ser medido no perfil do escopo. As avaliações de adequação abaixo são julgamento de engenharia, não benchmarks.

## Plataforma e interface

| Opção | Adequação ao projeto | Custo ou limite | Encaminhamento |
|---|---|---|---|
| C#/.NET 10 + WPF | Interface desktop Windows, bindings e acesso a APIs nativas | Interop de rede e separação da thread visual precisam de prova | Preferida para PV-01/02 |
| C#/.NET + WinUI 3 | Alternativa Windows com controles atuais | Adiciona decisões de Windows App SDK e empacotamento | Reserva se surgir necessidade visual específica |
| C#/.NET + Avalonia | Alternativa caso outros sistemas se tornem requisito | Captura e inventário continuarão específicos por SO | Reavaliar se escopo multiplataforma for aprovado |
| C++ com UI nativa | Controle direto das APIs e memória | Maior custo estimado de desenvolvimento e segurança de parsers | Não priorizar sem limitação comprovada do .NET |

WPF é uma tecnologia Windows; WinUI 3 integra o Windows App SDK; Avalonia oferece uma alternativa multiplataforma. Referências: [Microsoft WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/), [Microsoft WinUI](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/), [Avalonia](https://docs.avaloniaui.net/docs/welcome).

A política consultada informa suporte ao .NET 10 LTS até 14/11/2028 e ao .NET 8 até 10/11/2026. Recomenda-se .NET 10 para o novo produto. O SDK 8.0.319 existente nesta máquina não comprova prontidão para esse alvo. Não foi instalado outro SDK. [Política oficial](https://dotnet.microsoft.com/en-us/platform/support/policy)

Fixar SDK 10.0.x e patches dos pacotes somente ao preparar PV-01, após verificar disponibilidade e compatibilidade. Não confundir a versão do runtime com a do SDK. A futura configuração terá `global.json`, gerenciamento central de pacotes e arquivo de lock; nenhum foi criado nesta etapa.

## Captura e interpretação

| Componente | Base documental | Proposta e prova necessária |
|---|---|---|
| Npcap | Driver Windows com API de captura; opções de instalação e privilégio documentadas | Backend preferencial de laboratório; validar versão, acesso, descarte e encerramento |
| SharpPcap | Biblioteca .NET com captura, filtros e leitura de arquivos | Adaptador candidato; fixar release e testar com .NET 10 |
| PacketDotNet | Parsers de rede; repositório informa MPL-2.0 | Uso opcional para camadas básicas, condicionado a corpus e licença do pacote |
| PCAPNG | Formato documentado em Internet-Draft | Contrato independente de leitura/escrita; selecionar implementação em PV-04 |
| Wireshark/TShark | Referência externa de inspeção proposta | Ferramenta de laboratório; não é dependência de execução escolhida |

Fontes: [Npcap](https://npcap.com/guide/npcap-users-guide.html), [SharpPcap](https://github.com/dotpcap/sharppcap), [PacketDotNet](https://github.com/dotpcap/packetnet), [PCAPNG](https://datatracker.ietf.org/doc/draft-ietf-opsawg-pcapng/).

O repositório consultado do SharpPcap declara MIT e mostra versão 6.3.1 no projeto. Isso é evidência do conteúdo consultado, não confirmação do pacote mais recente ou de todos os seus componentes. Auditar o artefato exato e dependências transitivas antes de travar a versão. [Projeto oficial](https://github.com/dotpcap/sharppcap/blob/master/SharpPcap/SharpPcap.csproj)

O suporte anunciado a arquivos de captura não deve ser usado como prova de exportação PCAPNG com metadados. A API `pcap_dump_open` tem contrato próprio de escrita; exigir inspeção dos blocos e abertura independente do resultado. Não basta trocar a extensão de um PCAP. [libpcap](https://www.tcpdump.org/manpages/pcap_dump_open.3pcap.html)

Alternativas a investigar somente se necessário: coleta nativa Windows ou outro wrapper libpcap. Não foram qualificadas para substituir Npcap; devem atender ao mesmo conjunto de protocolos e evidências antes de adoção.

## Persistência, relatórios e distribuição

SQLite com Microsoft.Data.Sqlite é a proposta inicial, com um gravador dedicado, transações curtas e capturas fora do banco. A documentação informa que os métodos assíncronos do provedor executam sincronamente; chamá-los com `await` não garante UI livre. [Microsoft](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/async)

Avaliar WAL e backup consistente em PV-05. O banco ativo ficará em disco local, e não em compartilhamento de rede. As escolhas de sincronização e checkpoint serão medidas antes do aceite. [SQLite WAL](https://www.sqlite.org/wal.html), [Backup](https://www.sqlite.org/backup.html)

Para o MVP, propor relatório técnico HTML offline, resumo TXT e dados estruturados JSON, todos derivados do mesmo snapshot. PDF não foi imposto no escopo; eventual biblioteca será escolhida depois da decisão de formato, evitando dependência sem necessidade.

Comparar instalação por usuário com runtime incluído e instalação com pré-requisitos durante PV-06. Driver e eventual helper privilegiado têm ciclo próprio. Tecnologia do instalador, atualização, certificado e licença do código do produto permanecem abertas. Não há decisão de publicar como código aberto.

## Dependências e licenças

O fornecedor informa que a edição gratuita do Npcap não permite redistribuição e possui limites de instalações, com exceções. A licença OEM diferencia uso interno e redistribuição. Isso torna a distribuição um item de engenharia a fechar, sem presumir que o driver existente pode ser empacotado. [Npcap OEM](https://npcap.com/oem/)

Antes de incorporar cada artefato, registrar nome, versão, origem, hash, licença declarada, dependências transitivas, avisos obrigatórios e finalidade. Para PacketDotNet, revisar obrigações MPL-2.0 aplicáveis ao uso/modificação planejados. Para SDKs proprietários, registrar termos aceitos e permissões de redistribuição. A seleção documental não substitui essa conferência do pacote concreto.

## Resultado da seleção

A combinação recomendada é adequada para uma prova de viabilidade, ainda não aprovada para release. Os fatores com maior potencial de mudança são vínculo real à interface, escrita PCAPNG, modelo de privilégios, distribuição do driver e profundidade verificável de QDP. Esses fatores possuem provas e responsáveis nos documentos seguintes.


