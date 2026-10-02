param([string]$Version = '0.2.0-preview.2')
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskRegistration = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\{0E9F9169-574D-4D28-AB4D-D040A9E8269B}_is1'
if (Test-Path $taskRegistration) { throw 'Execute o teste em usuário sem instalação registrada do NetTools AV; não substituir uma instalação de uso.' }
$taskInstaller = Join-Path $taskRoot "artifacts/installers/NetToolsAV-$Version-win-x64-setup.exe"
$taskTarget = Join-Path $taskRoot ('artifacts/qa/install-' + [guid]::NewGuid().ToString('N'))
$taskManifest = Get-Content (Join-Path $taskRoot "artifacts/releases/$Version/BUILD-MANIFEST.json") -Raw | ConvertFrom-Json
$taskDataDir = Join-Path ([Environment]::GetFolderPath('LocalApplicationData')) 'NetToolsAV'
$taskBefore = @(Get-ChildItem $taskDataDir -File -Recurse -ErrorAction SilentlyContinue | ForEach-Object { @{ Path=$_.FullName; Hash=(Get-FileHash $_.FullName).Hash } })
function Invoke-Setup([string]$File, [string[]]$Arguments) {
    $taskProcess = Start-Process -FilePath $File -ArgumentList $Arguments -WindowStyle Hidden -PassThru -Wait
    if ($taskProcess.ExitCode -ne 0) { throw "Instalador retornou $($taskProcess.ExitCode): $File" }
}
$taskMutex = [Threading.Mutex]::new($false, 'Local\NetToolsAV.Application')
try {
    $taskBlocked = Start-Process -FilePath $taskInstaller -ArgumentList @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART','/NOICONS',('/DIR="' + $taskTarget + '"')) -WindowStyle Hidden -PassThru -Wait
    if ($taskBlocked.ExitCode -eq 0 -or (Test-Path (Join-Path $taskTarget 'NetToolsAV.exe'))) { throw 'Instalador não bloqueou aplicativo aberto.' }
} finally { $taskMutex.Dispose() }
foreach ($taskPass in 1..2) {
    Invoke-Setup $taskInstaller @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART','/NOICONS',('/DIR="' + $taskTarget + '"'),('/LOG="' + $taskTarget + "-pass$taskPass.log" + '"'))
    foreach ($taskFile in $taskManifest.Files) {
        if ((Get-FileHash (Join-Path $taskTarget $taskFile.File)).Hash -ne $taskFile.SHA256) { throw "Hash inválido: $($taskFile.File)" }
    }
}
& (Join-Path $PSScriptRoot 'verify-worker.ps1') -Executable ([IO.Path]::GetRelativePath($taskRoot,(Join-Path $taskTarget 'NetToolsAV.exe')))
Invoke-Setup (Join-Path $taskTarget 'unins000.exe') @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART')
if (Test-Path (Join-Path $taskTarget 'NetToolsAV.exe')) { throw 'Executável não removido na desinstalação.' }
foreach ($taskFile in $taskBefore) {
    if ((Get-FileHash -LiteralPath $taskFile.Path).Hash -ne $taskFile.Hash) { throw "Dados de usuário alterados: $($taskFile.Path)" }
}
$taskResult = [ordered]@{ Version=$Version; CheckedUtc=[DateTimeOffset]::UtcNow.ToString('O'); RunningAppBlocked=$true; Install=$true; Reinstall=$true; Uninstall=$true; VerifiedFiles=$taskManifest.Files.Count; PreservedUserFiles=$taskBefore.Count; CleanMachine=$false; LiveCapture=$false }
$taskResult | ConvertTo-Json | Set-Content (Join-Path $taskRoot 'artifacts/qa/installer-result.json') -Encoding utf8
$taskResult
