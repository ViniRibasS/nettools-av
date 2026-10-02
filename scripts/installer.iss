#ifndef AppVersion
  #error AppVersion is required
#endif
#ifndef PublishDir
  #error PublishDir is required
#endif
[Setup]
AppId={{0E9F9169-574D-4D28-AB4D-D040A9E8269B}
AppName=NetTools AV
AppVersion={#AppVersion}
AppPublisher=NetTools AV
DefaultDirName={localappdata}\Programs\NetTools AV
DefaultGroupName=NetTools AV
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
OutputDir=..\artifacts\installers
OutputBaseFilename=NetToolsAV-{#AppVersion}-win-x64-setup
SetupIconFile=..\assets\brand\nettools-av.ico
UninstallDisplayIcon={app}\NetToolsAV.exe
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
AppMutex=Local\NetToolsAV.Application
CloseApplications=no
RestartApplications=no
DisableProgramGroupPage=yes
InfoBeforeFile=installer-info.txt
[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
[Tasks]
Name: "desktopicon"; Description: "Criar atalho na área de trabalho"; Flags: unchecked
[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
[Icons]
Name: "{group}\NetTools AV"; Filename: "{app}\NetToolsAV.exe"
Name: "{userdesktop}\NetTools AV"; Filename: "{app}\NetToolsAV.exe"; Tasks: desktopicon
[Run]
Filename: "{app}\NetToolsAV.exe"; Description: "Abrir NetTools AV"; Flags: nowait postinstall skipifsilent
