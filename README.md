# IFAS VMS — Full Coding Source

Standalone newly generated source package based on the complete feature scope discussed for the IFAS VMS customer product.

## Build
Windows + .NET 8 SDK:
dotnet restore IFAS-VMS.sln
dotnet build IFAS-VMS.sln -c Release

## Run
Start `01-IFAS-VMS-SERVER` first, then `03-IFAS-VMS-CLIENT`.

## Scope
Camera management, ONVIF discovery, RTSP, vendor protocol extension points, NVR/channel management, live grids 1/4/8/16/32/64, drag/drop, multi-monitor foundation, recording storage/schedule/retention, playback search, licensing, updater and installer scripts.

## Important
This is a newly generated standalone implementation/source scaffold, not a copy of the existing repository. Real camera/NVR interoperability, LibVLC runtime packaging, FFmpeg packaging, installer signing, multi-monitor hardware behavior and full end-to-end Windows testing must be validated on Windows hardware before production release.
