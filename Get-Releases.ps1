<#
.SYNOPSIS
    Generates a DocFX-compatible Markdown list of GitHub releases.

.DESCRIPTION
    Retrieves the most recent GitHub releases (default: 10) from a specified repository using the GitHub REST API.
    Produces a clean, responsive, borderless HTML layout styled to roughly approximate GitHub's native releases page.
    Designed specifically for integration into DocFX-generated documentation sites using Markdig parser.
    Automatically downgrades H1–H3 headers in release bodies to H4 to prevent unwanted entries in the sidebar table of contents.
    Includes responsive design: two-column layout on desktop, content-first stacked layout on mobile devices.

.PARAMETER Owner
    The GitHub organization or username that owns the repository (e.g., "dotnet").

.PARAMETER RepoName
    The name of the repository (e.g., "docfx").

.PARAMETER APIKey
    Optional GitHub Personal Access Token. Required for private repositories or to avoid rate limiting on public ones.

.PARAMETER Output
    Path to the output Markdown file. Defaults to "Releases.md" in the current directory.

.EXAMPLE
    .\Get-DocFxReleases.ps1 -Owner "microsoft" -RepoName "vscode" -Output "docs/releases.md"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$Owner,

    [Parameter(Mandatory=$true)]
    [string]$RepoName,

    [string]$APIKey,

    [string]$Output = "Releases.md"
)

# ------------------------------------------------------------------
# Configuration
# ------------------------------------------------------------------
$Limit = 10                                      # Number of latest releases to include
$BaseUrl = "https://api.github.com"              # GitHub REST API base endpoint
$RepoUrl = "https://github.com/$Owner/$RepoName" # Canonical repository URL for links

# ------------------------------------------------------------------
# YAML Template Configuration - Disable Unwanted Navigation Features
# ------------------------------------------------------------------
$yamlHeader = @'
---
_customNavTitle: "History"
_disableToc: true
_disableBreadcrumb: true
_disableNextArticle: true
_disableContribution: true
---
'@;

# ------------------------------------------------------------------
# Embedded CSS – Provides GitHub-like styling within DocFX constraints
# ------------------------------------------------------------------
$cssBlock = @'
<style>
.github-style-releases {
    max-width: 920px;
    width: 100%;
    margin-left: auto;
    margin-right: auto;
    padding: 0 16px;
    box-sizing: border-box;
}

@media (max-width: 768px) {
    .github-style-releases { padding: 0 16px; }
}
@media (max-width: 480px) {
    .github-style-releases { padding: 0 8px; }
}

/* Each release row – flex container with optional vertical divider */
.release-row {
    display: flex;
    flex-wrap: wrap;
    gap: 0 32px;
    margin-bottom: 48px;
    align-items: flex-start;
    position: relative;                     /* Required for ::before divider */
}

/* Vertical divider visible only on desktop (≥769px) */
@media (min-width: 769px) {
    .release-row::before {
        content: "";
        position: absolute;
        top: 0;
        bottom: 0;
        left: 192px;                        /* 160px meta width + 32px gap */
        width: 1px;
        background-color: #7e7e7e7a;
        z-index: 1;
    }
}

/* Left column – fixed-width metadata (appears second on mobile) */
.release-meta {
    flex: 0 0 160px;
    font-size: 14px;
    color: #57606a;
    line-height: 1.5;
    order: 1;
    padding-right: 32px;                    /* Visual spacing; actual divider sits in gap */
}

/* Right column – flexible release body (appears first on mobile) */
.release-body {
    flex: 1;
    min-width: 0;
    order: 2;
    margin-left: 32px;                      /* Maintains gap symmetry */
}

/* Mobile layout – stack vertically, content first */
@media (max-width: 768px) {
    .release-row {
        flex-direction: column;
        gap: 16px;
        margin-bottom: 40px;
    }
    .release-body  { order: 1; margin-left: 0; }
    .release-meta  { order: 2; margin-top: 8px; padding-right: 0; }
    .release-row::before { display: none; }
}

/* Title section styling */
.release-header {
    padding-bottom: 8px;
    margin-bottom: 16px;
    border-bottom: 1px solid #7e7e7e7a;
}
.release-header h3 {
    margin: 0;
    font-size: 1.25em;
    font-weight: 600;
}
</style>
'@

# ------------------------------------------------------------------
# HTTP Headers for GitHub API
# ------------------------------------------------------------------
$Headers = @{
    "Accept" = "application/vnd.github.v3.html+json"  # Requests pre-rendered HTML for release bodies
}
if (-not [string]::IsNullOrWhiteSpace($APIKey)) {
    $Headers["Authorization"] = "token $APIKey"       # Authenticate if token provided
}

Write-Host "Fetching releases for $Owner/$RepoName..." -ForegroundColor Cyan

try {
    # ------------------------------------------------------------------
    # 1. Retrieve latest releases
    # ------------------------------------------------------------------
    $ReleasesUri = "$BaseUrl/repos/$Owner/$RepoName/releases?per_page=$Limit"
    $Releases = Invoke-RestMethod -Uri $ReleasesUri -Headers $Headers -Method Get

    if ($Releases.Count -eq 0) {
        Write-Warning "No releases found for $Owner/$RepoName."
        return
    }

    # ------------------------------------------------------------------
    # 2. Retrieve tag objects to resolve accurate commit SHAs
    # ------------------------------------------------------------------
    Write-Host "Fetching tag references for accurate commit SHA resolution..." -ForegroundColor DarkGray
    $TagsUri = "$BaseUrl/repos/$Owner/$RepoName/tags?per_page=$Limit"
    $Tags = Invoke-RestMethod -Uri $TagsUri -Headers $Headers -Method Get
    
    # Build lookup table: tag name → full commit SHA
    $TagLookup = @{}
    foreach ($t in $Tags) {
        $TagLookup[$t.name] = $t.commit.sha
    }

    # ------------------------------------------------------------------
    # 3. Begin HTML/Markdown document construction
    # ------------------------------------------------------------------
    $sb = [System.Text.StringBuilder]::new()
    $sb.AppendLine($yamlHeader) | Out-Null
    $sb.AppendLine($cssBlock) | Out-Null
    $sb.AppendLine('<div class="github-style-releases">') | Out-Null
    $sb.AppendLine('<h1>Releases</h1>') | Out-Null
    $sb.AppendLine('') | Out-Null

    $IsFirst = $true  # Used to mark the latest release with a "Latest" badge

    foreach ($Release in $Releases) {
        # --------------------------------------------------------------
        # Pre-process release metadata
        # --------------------------------------------------------------
        $DateObj   = [DateTime]::Parse($Release.published_at)
        $DateStr   = $DateObj.ToString("dd MMM yyyy")

        # Resolve full commit SHA (prefer tag object over target_commitish)
        $ShaFull   = $TagLookup[$Release.tag_name]
        if (-not $ShaFull) { $ShaFull = $Release.target_commitish }
        $ShaShort  = if ($ShaFull.Length -gt 7) { $ShaFull.Substring(0, 7) } else { $ShaFull }
        $ShaLink   = "$RepoUrl/commit/$ShaFull"

        # "Latest" badge for the most recent release only
        $BadgeHtml = ""
        if ($IsFirst) {
            $BadgeHtml = '&nbsp;<span style="background-color: #28a745; color: white; padding: 2px 6px; border-radius: 4px; font-size: 0.75em; vertical-align: middle;">Latest</span>'
            $IsFirst = $false
        }

        # Use release name if provided; otherwise fall back to tag name
        $Title = if ([string]::IsNullOrWhiteSpace($Release.name)) { $Release.tag_name } else { $Release.name }

        # Downgrade H1–H3 headers in body to H4 to avoid polluting DocFX TOC
        $ProcessedHtml = $Release.body_html -replace '<(/?)h[1-3]\b', '<$1h4'

        # --------------------------------------------------------------
        # Construct release row (two-column responsive layout)
        # --------------------------------------------------------------
        $sb.AppendLine('<div class="release-row">') | Out-Null

        # ── Metadata column (left on desktop, bottom on mobile) ──
        $sb.AppendLine('  <div class="release-meta">') | Out-Null
        $sb.AppendLine("    <strong>$DateStr</strong><br/>") | Out-Null
        $sb.AppendLine("    <a href='$($Release.author.html_url)'>@$($Release.author.login)</a><br/>") | Out-Null
        $sb.AppendLine("    <code>$($Release.tag_name)</code><br/>") | Out-Null
        $sb.AppendLine("    <code><a href='$ShaLink'>$ShaShort</a></code>") | Out-Null
        $sb.AppendLine('  </div>') | Out-Null

        # ── Content column (right on desktop, top on mobile) ──
        $sb.AppendLine('  <div class="release-body">') | Out-Null

        # Title with link to GitHub release page and optional "Latest" badge
        $sb.AppendLine('    <div class="release-header">') | Out-Null
        $sb.AppendLine("      <h3>") | Out-Null
        $sb.AppendLine("        <a href='$($Release.html_url)' style='text-decoration:none; color:inherit;'>[$($Release.tag_name)] $Title</a>$BadgeHtml") | Out-Null
        $sb.AppendLine("      </h3>") | Out-Null
        $sb.AppendLine('    </div>') | Out-Null

        # Release body (pre-rendered HTML from GitHub)
        if (-not [string]::IsNullOrWhiteSpace($ProcessedHtml)) {
            $sb.AppendLine($ProcessedHtml) | Out-Null
            $sb.AppendLine('<div style="margin-bottom:16px;"></div>') | Out-Null
        }

        # Downloadable assets (collapsible <details> section)
        $assetCount = $Release.assets.Count + 2   # uploaded assets + zip + tar.gz

        $sb.AppendLine('    <details style="margin-top:16px;">') | Out-Null
        $sb.AppendLine("      <summary style=""cursor:pointer;""><strong>Assets <code>$assetCount</code></strong></summary>") | Out-Null
        $sb.AppendLine('      <ul style="margin-top:8px;">') | Out-Null

        # Always add source code links
        $sb.AppendLine("        <li><a href='$RepoUrl/archive/refs/tags/$($Release.tag_name).zip'>Source code (zip)</a></li>") | Out-Null
        $sb.AppendLine("        <li><a href='$RepoUrl/archive/refs/tags/$($Release.tag_name).tar.gz'>Source code (tar.gz)</a></li>") | Out-Null

        # Then add any uploaded assets
        foreach ($Asset in $Release.assets) 
        {
            $SizeStr = if ($Asset.size -gt 1mb) {
                "{0:N2} MB" -f ($Asset.size / 1mb)
            } else {
                 "{0:N2} KB" -f ($Asset.size / 1kb)
            }
            $sb.AppendLine("        <li><a href='$($Asset.browser_download_url)'>$($Asset.name)</a> ($SizeStr)</li>") | Out-Null
        }

        $sb.AppendLine('      </ul>') | Out-Null
        $sb.AppendLine('    </details>') | Out-Null

        $sb.AppendLine('  </div>') | Out-Null   # end release-body
        $sb.AppendLine('</div>') | Out-Null      # end release-row
    }

    # Footer link to full release history
    $sb.AppendLine('') | Out-Null
    $sb.AppendLine("[View all releases on GitHub]($RepoUrl/releases)") | Out-Null
    $sb.AppendLine('</div>') | Out-Null

    # ------------------------------------------------------------------
    # Write final output file
    # ------------------------------------------------------------------
    $FinalContent = $sb.ToString()
    $FinalContent | Set-Content -Path $Output -Encoding utf8
    Write-Host "Successfully generated release notes at: $Output" -ForegroundColor Green

} catch {
    Write-Error "Failed to fetch releases: $($_.Exception.Message)"
    Write-Error $_.ScriptStackTrace
}