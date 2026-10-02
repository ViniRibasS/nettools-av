param([string]$Executable = 'artifacts/releases/0.2.0-preview.1/NetToolsAV.exe')
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskExe = [IO.Path]::GetFullPath((Join-Path $taskRoot $Executable))
foreach ($taskCase in @('interface', 'filter')) {
    $taskOutput = Join-Path $taskRoot ('artifacts/qa/worker-' + $taskCase + '-' + [guid]::NewGuid().ToString('N'))
    $taskRequest = @{SessionId=[guid]::NewGuid();InterfaceId='00000000-0000-0000-0000-000000000099';Directory=$taskOutput;Filter='';Promiscuous=$false;DurationSeconds=1;LimitMiB=1}
    if ($taskCase -eq 'filter') { $taskRequest.Filter='udp and ( invalid' }
    $taskInfo = [Diagnostics.ProcessStartInfo]::new($taskExe)
    $taskInfo.ArgumentList.Add('--capture-worker')
    $taskInfo.UseShellExecute=$false
    $taskInfo.CreateNoWindow=$true
    $taskInfo.WindowStyle='Hidden'
    $taskInfo.RedirectStandardInput=$true
    $taskInfo.RedirectStandardOutput=$true
    $taskInfo.RedirectStandardError=$true
    $taskProcess=[Diagnostics.Process]::Start($taskInfo)
    try {
        $taskProcess.StandardInput.WriteLine(($taskRequest | ConvertTo-Json -Compress))
        $taskProcess.StandardInput.Close()
        if (-not $taskProcess.WaitForExit(10000)) { $taskProcess.Kill($true); throw 'Host não encerrou dentro do limite.' }
        $taskResult = $taskProcess.StandardOutput.ReadToEnd() | ConvertFrom-Json
        if ($taskProcess.ExitCode -ne 1 -or $taskResult.Status -ne 'Falha' -or $taskResult.Packets -ne 0) { throw 'Falha não propagada corretamente pelo host.' }
        if (Get-ChildItem -LiteralPath $taskOutput -Filter '*.pcapng') { throw 'Arquivo de captura inesperado no teste sem interface.' }
        Write-Output ('PASS: host isolado, caso ' + $taskCase + '; erro controlado; nenhuma captura iniciada.')
    } finally { $taskProcess.Dispose() }
}

