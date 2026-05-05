# Gaia UserPromptSubmit hook (PowerShell twin of log-prompt.sh).
$ErrorActionPreference = 'Stop'

$logDir = if ($env:GAIA_LOG_DIR) { $env:GAIA_LOG_DIR } else { Join-Path $PWD '.gaia' }
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$ts    = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
$chars = if ($env:CLAUDE_USER_PROMPT) { $env:CLAUDE_USER_PROMPT.Length } else { 0 }

$line = '{{"ts":"{0}","event":"prompt","chars":{1}}}' -f $ts, $chars
Add-Content -Path (Join-Path $logDir 'prompts.jsonl') -Value $line
exit 0
