# Gaia PostToolUse hook (PowerShell twin of post-tool-log.sh).
$ErrorActionPreference = 'Stop'

$logDir = if ($env:GAIA_LOG_DIR) { $env:GAIA_LOG_DIR } else { Join-Path $PWD '.gaia' }
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$ts       = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
$toolName = if ($env:CLAUDE_TOOL_NAME)      { $env:CLAUDE_TOOL_NAME }      elseif ($env:COPILOT_TOOL_NAME)      { $env:COPILOT_TOOL_NAME }      else { '' }
$filePath = if ($env:CLAUDE_TOOL_FILE_PATH) { $env:CLAUDE_TOOL_FILE_PATH } elseif ($env:COPILOT_TOOL_FILE_PATH) { $env:COPILOT_TOOL_FILE_PATH } else { '' }

$line = '{{"ts":"{0}","event":"tool","tool":"{1}","path":"{2}"}}' -f $ts, $toolName, $filePath
Add-Content -Path (Join-Path $logDir 'tools.jsonl') -Value $line

if ($filePath -match '/docs/' -or $filePath -match 'docs/architecture/') {
    $drift = '{{"ts":"{0}","event":"docs-touched","path":"{1}"}}' -f $ts, $filePath
    Add-Content -Path (Join-Path $logDir 'drift-watch.jsonl') -Value $drift
}
exit 0
