# Gaia SessionStart hook (PowerShell twin of session-start.sh).
$ErrorActionPreference = 'Stop'

$logDir = if ($env:GAIA_LOG_DIR) { $env:GAIA_LOG_DIR } else { Join-Path $PWD '.gaia' }
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$ts      = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
$project = Split-Path -Leaf $PWD
$envName = if ($env:GAIA_ENV) { $env:GAIA_ENV } else { 'unknown' }

$line = '{{"ts":"{0}","event":"session-start","project":"{1}","env":"{2}"}}' -f $ts, $project, $envName
Add-Content -Path (Join-Path $logDir 'session.jsonl') -Value $line

@'
Gaia is active.

Session-start checklist (per AGENTS.md):
  - Call memory_recall(project) to load persisted facts.
  - Call evolve_list() to load lessons from prior sessions.
  - Run Repo Explorer first if this is new work.

If /docs/ is the source of truth and code disagrees, fix drift before features.
'@
