# Gaia PreToolUse hook (PowerShell twin of pre-tool-policy.sh).
$ErrorActionPreference = 'Stop'

$toolName  = if ($env:CLAUDE_TOOL_NAME)  { $env:CLAUDE_TOOL_NAME }  elseif ($env:COPILOT_TOOL_NAME)  { $env:COPILOT_TOOL_NAME }  else { '' }
$toolInput = if ($env:CLAUDE_TOOL_INPUT) { $env:CLAUDE_TOOL_INPUT } elseif ($env:COPILOT_TOOL_INPUT) { $env:COPILOT_TOOL_INPUT } else { '' }

if ($toolName -eq 'Bash' -and $toolInput -match 'git\s+push\s+(.*--force|.*-f(\s|$))') {
    $branch = (& git rev-parse --abbrev-ref HEAD 2>$null)
    if ($LASTEXITCODE -ne 0) { $branch = 'unknown' }
    if ($branch -in @('main', 'master', 'prod', 'production') -or $branch -like 'release/*') {
        Write-Error "Gaia policy: refusing 'git push --force' on protected branch '$branch'."
        exit 2
    }
}
exit 0
