@echo off
set "folder=%~dp0"

if exist "%folder%\_site" (rmdir /S /Q "%folder%\_site")
if exist "%folder%\api" (rmdir /S /Q "%folder%\api")

:: Run docfx build
docfx docfx.json

:: Check if the previous command failed
if ERRORLEVEL 1 (
    echo.
    echo ERROR: docfx build failed! Exiting script.
    echo.
    goto :eof
)

echo.
echo Script finished successfully.