#define MyAppName "JiraTask"
#define MyAppChineseName "JiraTask"
#define MyAppVersion "1.0"
#define MyAppPublisher "dotnet school"
#define MyAppURL "https://dotnet-campus.github.io/"

[Setup]
AppId={{AEDA7075-70DC-479E-B796-344517C2C954}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppChineseName}
OutputDir=..\build\bin
OutputBaseFilename={#MyAppChineseName}
SetupIconFile=..\{#MyAppName}\Images\Jira.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\{#MyAppName}\bin\Debug\{#MyAppName}.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\{#MyAppName}\bin\Debug\*";  Excludes: "*.bak,*.xml,*.pdb,*.dll.config,*.ax,*\Log\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppChineseName}"; Filename: "{app}\{#MyAppName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppChineseName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppChineseName}"; Filename: "{app}\{#MyAppName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppName}"; Description: "{cm:LaunchProgram,{#MyAppChineseName}}";Flags: nowait postinstall skipifsilent
