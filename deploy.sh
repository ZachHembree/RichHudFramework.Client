#!/bin/bash

# Set the current directory (equivalent to %~dp0 in Batch)
folder="$(dirname "$0")"

# Remove existing directories
# Bash uses rm -rf for recursive forced removal
if [ -d "$folder/_site" ]; then
    rm -rf "$folder/_site"
fi

if [ -d "$folder/api" ]; then
    rm -rf "$folder/api"
fi

if [ -d "$folder/_src" ]; then
    rm -rf "$folder/_src"
fi

# Get source code
git clone -b 'master' git@github.com:ZachHembree/RichHudFramework.Client.git _src

# Releases page generation
# Executing PowerShell from Bash requires wrapping the command in 'powershell.exe -Command ...'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ".\Get-Releases.ps1" -Owner "ZachHembree" -RepoName "RichHudFramework.Client"

# Run docfx build
docfx docfx.json

# Google Analytics Verification
# Bash uses 'cp' for copy
cp googlefe53ea247c41569b.html _site

# Publish
git config --global core.autocrlf true
git init _site
cd _site
git checkout --orphan docfx-gh-pages
git remote add origin git@github.com:ZachHembree/RichHudFramework.Client.git
git add --all
git commit -m "DocFX Build"
git push --set-upstream origin docfx-gh-pages --force

echo ""
echo "Script finished successfully."