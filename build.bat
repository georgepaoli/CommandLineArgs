@echo off

if not exist "artifacts" mkdir artifacts
if not exist "artifacts\dotnet.zip" tools\Download.exe https://dotnetcli.blob.core.windows.net/dotnet/dev/Binaries/Latest/dotnet-win-x64.latest.zip artifacts\dotnet.zip
if not exist "artifacts\dotnet" tools\Zip.exe extract artifacts\dotnet.zip artifacts\dotnet
