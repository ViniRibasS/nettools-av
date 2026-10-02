param([switch]$SkipPackage)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskVersion = ([xml](Get-Content (Join-Path $taskRoot 'Directory.Build.props'))).Project.PropertyGroup.Version
$taskCompiler = Join-Path $taskRoot '.tools/inno/ISCC.exe'
if (-not (Test-Path -LiteralPath $taskCompiler)) { throw 'Instale Inno Setup 6.7.3 em .tools/inno para gerar o instalador.' }
if (-not $SkipPackage) { & (Join-Path $PSScriptRoot 'package.ps1') }
$taskPublish = Join-Path $taskRoot ('artifacts/releases/' + $taskVersion)
if (-not (Test-Path (Join-Path $taskPublish 'BUILD-MANIFEST.json'))) { throw 'Pacote publicado e manifesto obrigatórios.' }
$taskManifest = Get-Content (Join-Path $taskPublish 'BUILD-MANIFEST.json') -Raw | ConvertFrom-Json
if ($taskManifest.Version -ne $taskVersion) { throw 'Versão do manifesto não corresponde ao projeto.' }
foreach ($taskFile in $taskManifest.Files) {
    if ((Get-FileHash -LiteralPath (Join-Path $taskPublish $taskFile.File) -Algorithm SHA256).Hash -ne $taskFile.SHA256) { throw "Arquivo publicado divergente: $($taskFile.File)" }
}
& $taskCompiler ('/DAppVersion=' + $taskVersion) ('/DPublishDir=' + $taskPublish) (Join-Path $PSScriptRoot 'installer.iss')
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar instalador.' }
$taskInstaller = Join-Path $taskRoot ('artifacts/installers/NetToolsAV-' + $taskVersion + '-win-x64-setup.exe')
Get-FileHash -LiteralPath $taskInstaller -Algorithm SHA256 | Select-Object Hash,@{n='File';e={[IO.Path]::GetFileName($_.Path)}} | ConvertTo-Json | Set-Content ($taskInstaller + '.sha256.json') -Encoding utf8
Write-Output $taskInstaller
