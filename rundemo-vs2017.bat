@echo off

for /f "usebackq tokens=1* delims=: " %%i in (`.\tools\vswhere -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (
  echo "Building..."
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" AnyQDemo.sln /t:Build /p:Configuration=Release /verbosity:quiet
  echo "Starting demo..."
  AnyQDemo\bin\Release\AnyQDemo.exe
) else (
  echo "Error: Could not find MSBuild."
)