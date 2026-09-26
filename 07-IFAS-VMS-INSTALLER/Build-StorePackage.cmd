@echo off
powershell -ExecutionPolicy Bypass -File "%~dp0Build-Publish.ps1"
echo Publish finished. Use your chosen Windows installer builder to package Published\Server, Client and Updater.
pause
