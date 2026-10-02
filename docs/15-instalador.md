# Primeira entrega instalável

Decisão de engenharia — 2026-10-02, antes da implementação.

Entrega: 0.2.0-preview.2, Windows x64, instalação por usuário com Inno Setup 6.7.3. Inclui runtime .NET, aplicativo, licenças e documentação; não inclui Npcap, dados pessoais ou capturas. Mantém o layout aprovado e diagnóstico por protocolos.

O instalador cria atalhos e registro de desinstalação. Atualização e desinstalação exigem fechar aplicativo e processo de captura; não devem encerrar esses processos automaticamente. Os dados ficam em `%LOCALAPPDATA%\NetToolsAV`, separados dos binários em `%LOCALAPPDATA%\Programs\NetTools AV`. A desinstalação preserva os dados. Não modifica adaptadores, firewall ou serviços de rede.

O usuário informou ter aberto uma captura produzida pelo Wireshark. Isso registra execução manual da importação, sem resultado detalhado ou arquivo de evidência fornecido. Não comprova captura física ao vivo nem homologação integral. A matriz 14 continua determinando as capacidades e pendências; 1.0 permanece condicionada à homologação completa.

Validação prevista: verificações automatizadas existentes; geração e hash do instalador; instalação, reinstalação e desinstalação em diretório de teste; comparação dos arquivos instalados com manifesto; verificação do processo de captura instalado sem acesso a interface física. Homologação em máquina limpa e assinatura de código permanecem pendentes.

Fontes: [Inno Setup 6.7.3 e assinatura do distribuidor](https://jrsoftware.org/isdl.php), [licença da versão utilizada](https://raw.githubusercontent.com/jrsoftware/issrc/is-6_7_3/license.txt), [instalação sem elevação](https://jrsoftware.org/ishelp/topic_setup_privilegesrequired.htm), [controle de aplicações abertas](https://jrsoftware.org/ishelp/topic_setup_appmutex.htm). O instalador oficial do compilador foi validado por Authenticode: assinatura válida de Pyrsys B.V. A licença dessa versão permite uso comercial; o fornecedor solicita aquisição de licença comercial no site.

## Resultado da validação — 02/10/2026

- Build Release: zero erros e avisos; 39 verificações aprovadas.
- Instalador gerado: `artifacts/installers/NetToolsAV-0.2.0-preview.2-win-x64-setup.exe`, 47.408.084 bytes.
- SHA-256: `50B2EE1C646B17C32458DCD81B5C8689E3C76283B79B2B8499C9F891514F4F84`.
- Bloqueio por mutex, instalação, reinstalação e desinstalação aprovados nesta máquina. 417 arquivos comparados com o manifesto após cada instalação; 13 arquivos de dados existentes preservados com hashes idênticos após desinstalação.
- Dois cenários de erro do processo de captura instalado aprovados, sem abrir interface de rede.
- Evidência automatizada: `artifacts/qa/installer-result.json`; logs `artifacts/qa/install-*-pass*.log`.
- Máquina limpa, captura ao vivo, estresse prolongado, assinatura digital e protocolos ausentes continuam pendentes. Este resultado não autoriza a versão 1.0.
- Reproduzir: `./scripts/installer.ps1`; testar em perfil sem instalação registrada: `./scripts/verify-installer.ps1`. O teste recusa substituir uma instalação de uso.

## Evidências locais encontradas após a instalação

A inspeção somente de leitura dos arquivos já salvos em `%LOCALAPPDATA%\NetToolsAV\captures` encontrou duas capturas diretas, de 3.751 e 3.773 pacotes, encerradas respectivamente por limite e como concluída. Os resumos registram zero descartes do driver. Também encontrou duas importações de 35.483 e 9.501 pacotes. Os quatro SHA-256 conferiram; a reanálise pela versão atual reproduziu os totais, e capinfos confirmou PCAPNG Ethernet e contagens (a contagem de 35.483 aparece arredondada como 35 k na saída padrão). Evidência: `artifacts/qa/field-evidence.json`. Não foi iniciada nova captura de rede nesta entrega.

Isso comprova existência e integridade de resultados dessas execuções, sem qualificar carga, cobertura completa de protocolos, origem de sondas, precisão temporal ou perda ponta a ponta. Substitui a hipótese inicial de ausência total de evidência física; não substitui homologação completa.

Instalação final em `%LOCALAPPDATA%\Programs\NetTools AV`, com menu Iniciar. Abertura e carregamento dos dados existentes confirmados pela árvore de acessibilidade da janela, exibindo a importação de 9.501 pacotes. A ferramenta de automação teve atraso ao localizar a janela e retornou imagem inconsistente com o aplicativo selecionado; por isso não foi usada como evidência visual nem para ações adicionais. Sem falha do aplicativo identificada.
