#define MyAppName "FFRK-LabMem"
#ifndef MyAppVersion
#define MyAppVersion GetFileVersion('..\FFRK-LabMem\bin\Release\FFRK-LabMem.exe') + '-Beta'
#endif
#define MyAppPublisher "HughJeffner"
#define MyAppURL "https://github.com/HughJeffner/FFRK-LabMem"
#define MyAppExeName "FFRK-LabMem.exe"
#define AppId "{142A5A54-2A68-4F73-81AE-7DBF5EC860FC}"
#define SetupReg "Software\Microsoft\Windows\CurrentVersion\Uninstall\" + AppId + "_is1"
#define SetupAppPathReg "Inno Setup: App Path"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
; Two curly brackets for directive
AppId={#StringChange(AppId, '{', '{{')}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userdocs}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=FFRK-LabMem-{#MyAppVersion}-Installer
Compression=lzma
SolidCompression=yes
AppendDefaultDirName=False
WizardStyle=modern
CloseApplications=no
UsedUserAreasWarning=no
SetupIconFile=setup.ico
UninstallDisplayIcon={app}\FFRK-LabMem.exe
RestartApplications=False

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\FFRK-LabMem\bin\Release\FFRK-LabMem.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\FFRK-LabMem\bin\Release\adb.exe"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "..\FFRK-LabMem\bin\Release\AdbWinApi.dll"; DestDir: "{app}"
Source: "..\FFRK-LabMem\bin\Release\AdbWinUsbApi.dll"; DestDir: "{app}"
Source: "..\FFRK-LabMem\bin\Release\blocklist.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\FFRK-LabMem\bin\Release\FFRK-LabMem.exe.config"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "..\FFRK-LabMem\bin\Release\FFRK-LabMem.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\FFRK-LabMem\bin\Release\Config\*.json"; DestDir: "{app}\Config"; Flags: recursesubdirs createallsubdirs onlyifdoesntexist; Check: not IsUpgrade
Source: "..\FFRK-LabMem\bin\Release\Sounds\*.*"; DestDir: "{app}\Sounds"; Flags: recursesubdirs createallsubdirs onlyifdoesntexist
Source: "..\FFRK-LabMem\bin\Release\Minicap\*.*"; DestDir: "{app}\Minicap"; Flags: recursesubdirs createallsubdirs onlyifdoesntexist
Source: "{app}\DataLog\*"; DestDir: "{app}\Data\"; Flags: external skipifsourcedoesntexist onlyifdoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\View GitHub Project"; Filename: "{#MyAppURL}";
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent unchecked; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"
Filename: "{app}\{#MyAppExeName}"; Flags: nowait skipifnotsilent; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[Code]
function IsUpgrade: Boolean;
var
  S: string;
begin
  Result :=
    RegQueryStringValue(HKLM, '{#SetupReg}', '{#SetupAppPathReg}', S) or
    RegQueryStringValue(HKCU, '{#SetupReg}', '{#SetupAppPathReg}', S);
end;