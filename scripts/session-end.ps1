# Gaia Stop / sessionEnd hook (PowerShell twin of session-end.sh).
$ErrorActionPreference = 'Stop'

$logDir = if ($env:GAIA_LOG_DIR) { $env:GAIA_LOG_DIR } else { Join-Path $PWD '.gaia' }
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$ts = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')

function Count-Lines($path) {
    if (Test-Path $path) { (Get-Content $path | Measure-Object -Line).Lines } else { 0 }
}

$prompts = Count-Lines (Join-Path $logDir 'prompts.jsonl')
$tools   = Count-Lines (Join-Path $logDir 'tools.jsonl')

$line = '{{"ts":"{0}","event":"session-end","prompts":{1},"tool_calls":{2}}}' -f $ts, $prompts, $tools
Add-Content -Path (Join-Path $logDir 'session.jsonl') -Value $line
exit 0
