$ErrorActionPreference="Stop"
$root=Split-Path $PSScriptRoot -Parent
dotnet publish "$root/01-IFAS-VMS-SERVER/IFAS.Server.csproj" -c Release -r win-x64 --self-contained true -o "$PSScriptRoot/Published/Server"
dotnet publish "$root/03-IFAS-VMS-CLIENT/IFAS.VMS.Client.csproj" -c Release -r win-x64 --self-contained true -o "$PSScriptRoot/Published/Client"
dotnet publish "$root/08-IFAS-VMS-UPDATER/IFAS.VMS.Updater.csproj" -c Release -r win-x64 --self-contained true -o "$PSScriptRoot/Published/Updater"
Write-Host "IFAS VMS publish complete."
