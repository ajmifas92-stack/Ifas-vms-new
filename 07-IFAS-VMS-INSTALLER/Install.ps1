$ErrorActionPreference="Stop"
$target="$env:ProgramFiles\IFAS VMS"
New-Item -ItemType Directory -Force -Path $target | Out-Null
Copy-Item "$PSScriptRoot\Published\*" $target -Recurse -Force
New-Item -ItemType Directory -Force -Path "C:\ProgramData\IFAS-VMS\Recordings" | Out-Null
Write-Host "IFAS VMS installed to $target"
