# NetTools AV 0.2.0-preview.2

Primeira prévia instalável Windows x64. Ainda não é a versão 1.0 homologada.

## Downloads

- `NetToolsAV-0.2.0-preview.2-win-x64-setup.exe`: instalador por usuário, com runtime .NET.
- `NetToolsAV-0.2.0-preview.2-win-x64.zip`: versão portátil; extraia e mantenha todos os arquivos juntos.
- Arquivos `.sha256.json`: hashes para conferir a integridade dos downloads.

Npcap é externo e necessário somente para captura ao vivo; não acompanha esta distribuição. O instalador não tem assinatura digital de produção. Atualização/desinstalação exigem fechar o aplicativo; dados de sessão e captura são preservados.

## Capacidades e evidência

Sessões SQLite, captura passiva Ethernet, importação PCAPNG, inspeção básica de dez identificadores de protocolo, histórico, relatórios HTML/TXT/JSON e exportação ZIP. Protocolos são o eixo da aplicação, sem diagnóstico por equipamento.

Baseline: 39 verificações automatizadas; testes de erro do host; instalação/reinstalação/desinstalação com integridade de 417 arquivos e preservação de 13 arquivos locais. Quatro arquivos de campo verificados por hash/reanálise. Esses dados de campo não são distribuídos.

Ainda faltam analisadores e homologação integral, incluindo máquina limpa, recuperação e ensaios prolongados. Consulte a matriz de capacidades na documentação. Presença de um protocolo não comprova conformidade.

## Proveniência

Instalador produzido em 02/10/2026 antes do primeiro commit Git. SHA-256: `50B2EE1C646B17C32458DCD81B5C8689E3C76283B79B2B8499C9F891514F4F84`.

O repositório inicial inclui documentação consolidada depois desse build. A tag desta distribuição identifica a importação inicial das fontes/documentação; não deve ser interpretada como commit de build original. Os manifestos internos preservam a proveniência disponível. Não houve recompilação dos binários para esta publicação.
