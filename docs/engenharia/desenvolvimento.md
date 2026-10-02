# Guia de desenvolvimento

Baseline 0.2.0-preview.2. [Contribuição](../../CONTRIBUTING.md), [arquitetura](arquitetura-atual.md), [contratos](dados-e-contratos.md).

## Preparar ambiente

Windows com PowerShell 7 para reproduzir os scripts atuais: eles usam APIs modernas como Path.GetRelativePath e ProcessStartInfo.ArgumentList, não qualificadas para Windows PowerShell 5.1. SDK definido em `global.json`: 10.0.401 com rollForward latestPatch. Build WPF e testes do instalador são Windows.

O script procura `.tools/dotnet/dotnet.exe`, depois `dotnet` do PATH. Configura DOTNET_CLI_HOME e NUGET_PACKAGES dentro de `.tools`, desliga telemetria CLI e exige restore locked. Dependências locais não são versionadas. Instale SDK pela distribuição oficial; este repositório não contém bootstrap completo de máquina nova.

Npcap não é necessário para testes puros de parser/persistência; testes de enumeração, BPF e host dependem do driver/biblioteca instalada. Wireshark é ferramenta de comparação de laboratório, não dependência do produto. Inno Setup 6.7.3 em `.tools/inno` é necessário para compilar instalador; confira origem/assinatura conforme [registro 15](../15-instalador.md).

## Comandos existentes

```powershell
./scripts/build.ps1 -Target Build
./scripts/build.ps1 -Target Check
./scripts/build.ps1 -Target Publish -OutputDirectory artifacts/releases/0.2.0-preview.2
./scripts/package.ps1
./scripts/verify-worker.ps1 -Executable 'artifacts/releases/0.2.0-preview.2/NetToolsAV.exe'
./scripts/installer.ps1 -SkipPackage
```

`Check` compila e roda um console de verificações; não é `dotnet test`. Use o caminho explícito no verify-worker: seu default histórico aponta para preview.1. Installer sem SkipPackage publica/empacota antes. Não republicar versão existente para uma entrega nova; [release](release.md).

Na máquina preparada, comandos auxiliares do harness:

```powershell
./.tools/dotnet/dotnet.exe tests/NetToolsAV.Checks/bin/Release/net10.0/NetToolsAV.Checks.dll --npcap-list
./.tools/dotnet/dotnet.exe tests/NetToolsAV.Checks/bin/Release/net10.0/NetToolsAV.Checks.dll --npcap-check
./.tools/dotnet/dotnet.exe tests/NetToolsAV.Checks/bin/Release/net10.0/NetToolsAV.Checks.dll --analyze 'C:\CAMINHO\arquivo.pcapng'
```

São enumeração/compilação offline/análise de arquivo, não captura ao vivo. Substitua caminho de exemplo; não disponibilize arquivo sensível como fixture pública.

## Fluxo de alteração

1. Ler requisito vigente e código afetado; verificar Git sem descartar mudanças.
2. Documentar mudança de comportamento/contrato antes do código quando houver nova decisão.
3. Alterar a camada correta; manter dependências do Core independentes de UI/driver/banco.
4. Incluir testes de fronteira relevantes. Executar Check para C#; worker quando IPC/Npcap mudar; instalador somente em perfil apropriado.
5. Atualizar matriz, guia, changelog e evidências; distinguir realizado de planejado.
6. Revisar diff e conteúdo do pacote antes de release. Sem CI remoto configurado, não alegar pipeline aprovado.

## Adicionar protocolo

Preencher [modelo de protocolo](../modelos/protocolo.md): fonte primária/revisão/seção, transportes, condições de reconhecimento e limites. Atualizar catálogo só quando ID estável definido. Acrescentar à lista Supported apenas com parser efetivo. Validar comprimentos antes de acesso, limites de contagem/recursão e ambiguidade. Cobrir positivos, negativos, truncados, fragmentos e payloads em porta conhecida que não sejam o protocolo. Comparar corpus com ferramenta independente sem tomar heurística externa como norma.

Preservar amostra limitada, identidade por protocolo e estados inconclusivos. Métricas como jitter exigem clock rate/método e ensaio próprios. UI deve dizer o que foi medido, nunca deduzir saúde de presença.

## Falhas e práticas

Warnings são erros. Se NuGet/auditoria falhar por rede, registre causa e restaure acesso autorizado; não retire locks nem desative auditoria para declarar sucesso. Os scripts usam compilação determinística, mas ZIP/instalador possuem timestamps: não prometer reprodução byte a byte.

Não editar `artifacts/releases` como fonte nem empacotar `.tools`, banco ou PCAPNG de campo. Não usar teste de instalação no perfil operacional. Alteração documental exige validação de links/consistência, sem recompilar produto desnecessariamente.
