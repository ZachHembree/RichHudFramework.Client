@echo off

set "folder=%~dp0"
if exist "%folder%\_site\" (rmdir /S /Q "%folder%\_site\")
if exist "%folder%\api" (rmdir /S /Q "%folder%\api")
:: Releases page generation
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\Get-Releases.ps1" -Owner "ZachHembree" -RepoName "RichHudFramework.Client"

:: Get source code
git clone -b 'master' git@github.com:ZachHembree/RichHudFramework.Client.git _src

:: Run docfx build
docfx docfx.json

:: Google Analytics Verification
copy googlefe53ea247c41569b.html _site

:: Publish
git config --global core.autocrlf true
git init _site
cd _site
git checkout --orphan docfx-gh-pages
git remote add origin git@github.com:ZachHembree/RichHudFramework.Client.git
git add --all
git commit -m "DocFX Build"
git push --set-upstream origin docfx-gh-pages --force

echo.
echo Script finished successfully.