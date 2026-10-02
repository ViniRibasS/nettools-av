# Build, distribuição e release

Baseline 0.2.0-preview.2, não homologada como 1.0. [Registro do instalador](../15-instalador.md) e [modelo de liberação](../modelos/release.md).

## Entradas controladas

Versão do produto em Directory.Build.props; SDK em global.json; NuGet em locks; compilador Inno conforme registro. CatalogVersion, Analyzer e Schema são contratos separados: não incrementar automaticamente todos ao mudar somente embalagem.

O repositório local inicial estava sem commits; o manifesto atual registra `working tree; uncommitted`. Não existe tag/commit de release ou CI remoto comprovado. Antes de release formal, estabelecer revisão rastreável e preencher identidade verdadeira; o script ainda não calcula SHA de Git automaticamente.

## Pipeline manual

1. Congelar escopo, atualizar documentação e registrar pendências/aceite. Escolher versão nova para alteração de entrega.
2. Executar Check. Executar testes de host/formatos pertinentes; registrar logs e ambiente.
3. `package.ps1`: publish self-contained win-x64; SOURCE-MANIFEST; BUILD-MANIFEST; ZIP e SHA-256.
4. `installer.ps1 -SkipPackage`: conferir versão/hashes do manifesto e compilar EXE Inno; gerar hash do instalador.
5. Conferir conteúdo: sem capturas, banco, credenciais, driver ou arquivos temporários indevidos. Runtime/licenças/guia/matriz devem estar presentes.
6. Testar `verify-installer.ps1 -Version <versão>` em perfil sem instalação registrada. O script instala, reinstala e remove o produto, verifica hashes/preservação e bloqueio por mutex. Não executar no perfil de uso.
7. Complementar com máquina limpa, UI, upgrade de versão anterior, recuperação, assinatura e ensaios de campo exigidos pelo marco. Não confundir reinstalação da mesma versão com migração/upgrade homologado.
8. Preencher release e distribuir pelo canal autorizado. Não há publicador ou atualização automática implementados.

## Artefatos

| Saída | Uso |
|---|---|
| artifacts/releases/versão | Binários portáteis com runtime |
| artifacts/NetToolsAV-…-win-x64.zip | Distribuição portátil |
| artifacts/installers/NetToolsAV-…-win-x64-setup.exe | Instalação por usuário |
| *.sha256.json | Integridade do arquivo distribuído |
| BUILD-MANIFEST.json | Hashes dos arquivos do pacote, versão, ambiente, Homologated/DriverIncluded |
| SOURCE-MANIFEST.json | Hashes das fontes selecionadas pelo script; não é SBOM completo |

O package preserva ZIP anterior usando sufixo temporal, mas reutiliza a pasta de publicação. O compilador pode substituir EXE de mesmo nome. Por isso não usar mesma versão para nova entrega. Diretório de publish deve ser limpo/novo e seu conteúdo auditado; não há garantia automática de ausência de arquivos residuais.

## Instalação e compatibilidade

Por usuário, destino padrão `%LOCALAPPDATA%\Programs\NetTools AV`; AppId fixo em installer.iss. Dados separados em `%LOCALAPPDATA%\NetToolsAV`, preservados na remoção. Mutex bloqueia setup/uninstall com UI/worker aberto. Não força encerramento nem altera rede.

Installer declara x64compatible e mínimo Windows 10 build 17763. Isso é uma condição do instalador, **não uma matriz homologada de compatibilidade .NET/Windows**. Windows ARM64 por emulação, versões antigas do Windows e outros ambientes não estão qualificados; plataforma de entrega anunciada é Windows x64.

Não há anti-downgrade geral nem migração completa. Faça backup antes de upgrade. Banco futuro é recusado pelo código; nunca contorne schema para forçar downgrade. Assinatura digital, máquina limpa e rollback homologado continuam pendentes.

## Documentação entregue

O csproj copia licenças, guia de capturas e matriz ao pacote. Os demais documentos desta revisão ficam no repositório; não são automaticamente incluídos. Alterar documentos depois de gerar o instalador não altera a cópia já distribuída. Não editar binários/manifestos publicados para esconder essa diferença.
