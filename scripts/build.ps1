param(
    [ValidateSet('Build', 'Check', 'Publish')][string]$Target = 'Check',
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [string]$OutputDirectory = 'artifacts/app'
)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskDotnet = Join-Path $taskRoot '.tools/dotnet/dotnet.exe'
if (-not (Test-Path -LiteralPath $taskDotnet)) { $taskDotnet = (Get-Command dotnet -ErrorAction Stop).Source }
$env:DOTNET_CLI_HOME = Join-Path $taskRoot '.tools/cli-home'
$env:NUGET_PACKAGES = Join-Path $taskRoot '.tools/nuget-packages'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
Push-Location $taskRoot
try {
    if ($Target -eq 'Publish') {
        & $taskDotnet publish 'src/NetToolsAV.Desktop/NetToolsAV.Desktop.csproj' --configuration $Configuration --runtime win-x64 --self-contained true --output $OutputDirectory --nologo --disable-build-servers -m:1 -p:RestoreLockedMode=true
    } else {
        & $taskDotnet build NetToolsAV.slnx --configuration $Configuration --nologo --disable-build-servers -m:1 -p:RestoreLockedMode=true
    }
    if ($LASTEXITCODE -ne 0) { throw 'Compilação falhou; verifique o diagnóstico acima.' }
    if ($Target -eq 'Check') {
        & $taskDotnet "tests/NetToolsAV.Checks/bin/$Configuration/net10.0/NetToolsAV.Checks.dll"
        if ($LASTEXITCODE -ne 0) { throw 'Verificações falharam.' }
    }
} finally { Pop-Location }
