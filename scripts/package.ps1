$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskVersion = ([xml](Get-Content (Join-Path $taskRoot 'Directory.Build.props'))).Project.PropertyGroup.Version
$taskRelativeOutput = 'artifacts/releases/' + $taskVersion
& (Join-Path $PSScriptRoot 'build.ps1') -Target Publish -OutputDirectory $taskRelativeOutput
$taskOutput = Join-Path $taskRoot $taskRelativeOutput
$taskSourceFiles = @('Directory.Build.props','global.json','NetToolsAV.slnx') + @(Get-ChildItem -Path (Join-Path $taskRoot 'src'),(Join-Path $taskRoot 'tests'),(Join-Path $taskRoot 'scripts'),(Join-Path $taskRoot 'assets'),(Join-Path $taskRoot 'docs') -File -Recurse | Where-Object FullName -NotMatch '[\\/](bin|obj)[\\/]' | ForEach-Object { [IO.Path]::GetRelativePath($taskRoot,$_.FullName) })
$taskSourceHashes = $taskSourceFiles | Sort-Object | ForEach-Object { [ordered]@{File=$_;SHA256=(Get-FileHash -LiteralPath (Join-Path $taskRoot $_) -Algorithm SHA256).Hash} }
$taskSourceHashes | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath (Join-Path $taskOutput 'SOURCE-MANIFEST.json') -Encoding utf8
$taskFiles = Get-ChildItem -LiteralPath $taskOutput -File -Recurse | Where-Object Name -ne 'BUILD-MANIFEST.json'
$taskHashes = $taskFiles | ForEach-Object { [ordered]@{File=[IO.Path]::GetRelativePath($taskOutput,$_.FullName);Bytes=$_.Length;SHA256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash} }
$taskManifest = [ordered]@{
 Version=$taskVersion;BuiltUtc=[DateTimeOffset]::UtcNow.ToString('O');Platform='win-x64';Sdk='10.0.401';Runtime='10.0.12';Homologated=$false;DriverIncluded=$false;SourceRevision='working tree; uncommitted';Files=$taskHashes
}
$taskManifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $taskOutput 'BUILD-MANIFEST.json') -Encoding utf8
$taskZip = Join-Path $taskRoot ('artifacts/NetToolsAV-' + $taskVersion + '-win-x64.zip')
if (Test-Path -LiteralPath $taskZip) { $taskZip = Join-Path $taskRoot ('artifacts/NetToolsAV-' + $taskVersion + '-' + [DateTime]::UtcNow.ToString('yyyyMMddHHmmss') + '-win-x64.zip') }
[IO.Compression.ZipFile]::CreateFromDirectory($taskOutput,$taskZip,[IO.Compression.CompressionLevel]::Fastest,$true)
Get-FileHash -LiteralPath $taskZip -Algorithm SHA256 | Select-Object Hash,@{n='File';e={[IO.Path]::GetFileName($_.Path)}} | ConvertTo-Json | Set-Content -LiteralPath ($taskZip + '.sha256.json') -Encoding utf8
Write-Output ('Pacote: ' + $taskZip)
Write-Output ('Executável: ' + (Join-Path $taskOutput 'NetToolsAV.exe'))
