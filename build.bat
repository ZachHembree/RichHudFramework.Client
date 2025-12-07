@echo off
set "folder=%~dp0"

if exist "%folder%\_site\api" (rmdir /S /Q "%folder%\_site\api")
if exist "%folder%\_site\articles" (rmdir /S /Q "%folder%\_site\articles")
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

copy googlefe53ea247c41569b.html _site

echo.
echo Script finished successfully.