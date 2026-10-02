# Primeira execução da fundação Windows

Documento histórico da fundação 0.1. Para a prévia atual, consulte [capturas e operação 0.2](capturas.md).

Este é um incremento de desenvolvimento, não o MVP homologado.

## Abrir

Na pasta `artifacts/app`, execute `NetToolsAV.exe`. Preserve todos os arquivos ao lado do executável: o runtime está incluído na pasta, não em um arquivo único. Não é necessário instalar SDK para usar esse pacote. Ainda não há instalador ou assinatura digital do produto.

Normalmente as sessões ficam em `%LOCALAPPDATA%/NetToolsAV/nettools-av.sqlite`. Para laboratório é possível informar `NetToolsAV.exe --data-dir CAMINHO`, colocando caminhos com espaços entre aspas. A conferência visual desta entrega usou o diretório padrão; as verificações automatizadas usam bancos separados em `artifacts/checks`.

## Usar

1. Abra a sessão inicial ou crie uma sessão com nome e ponto de medição.
2. Escolha explicitamente uma interface Ethernet informada pelo Windows. O inspetor mostra configuração local e estado do link.
3. Selecione protocolos no catálogo inferior. Não há cadastro de equipamentos.
4. Clique em um protocolo para ver transporte, versão/perfil e limitações.
5. Use a busca para filtrar os protocolos já selecionados na sessão.
6. Salve um relatório textual pelo botão Relatório. Ele registra a configuração e a ausência de análise executada.

Sessões, protocolos selecionados e identificador da interface são persistidos. Se a interface gravada desaparecer, o programa informa a indisponibilidade e não escolhe outra silenciosamente. O botão de atualizar interfaces renova a leitura local; não altera IP, rota ou adaptador.

## Limites atuais

Sem captura, parsers, sondas ou métricas de rede. Todos os protocolos ficam Não executado. Campos sem medição são indisponíveis. O catálogo indica o planejamento, não suporte funcional. As informações de interface são lidas do Windows, não geradas por simulação.

SQLite usa esquema 1 e transações. Banco com versão de esquema superior é recusado e preservado. Backup com aplicativo aberto, migrações de versões anteriores e recuperação completa ainda precisam das respectivas provas; não tratar a gravação transacional isolada como aprovação de RF-15.

## Desenvolver

SDK definido em `global.json`; pacote SQLite e dependências registrados nos `packages.lock.json`.

No PowerShell, a partir da raiz:

```powershell
./scripts/build.ps1 -Target Check
./scripts/build.ps1 -Target Publish
```

O script usa o SDK em `.tools/dotnet` quando presente ou o SDK compatível instalado no sistema. O primeiro restore exige acesso ao NuGet. A execução do aplicativo é local e não exige internet.
