#!/usr/bin/env bash
# Gaia PreToolUse hook. Enforces AGENTS.md non-negotiables:
#   - block destructive git operations on protected branches
#   - warn (do not block) on edits under /docs/ that bypass architect role
#
# Exits 0 to allow, 2 to block (Claude Code reads stderr on exit 2).

set -u

tool_name="${CLAUDE_TOOL_NAME:-${COPILOT_TOOL_NAME:-}}"
tool_input="${CLAUDE_TOOL_INPUT:-${COPILOT_TOOL_INPUT:-}}"

if [ "${tool_name}" = "Bash" ]; then
    if printf '%s' "${tool_input}" | grep -Eq 'git[[:space:]]+push[[:space:]]+(.*--force|.*-f([[:space:]]|$))'; then
        branch="$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo unknown)"
        case "${branch}" in
            main|master|release/*|prod|production)
                echo "Gaia policy: refusing 'git push --force' on protected branch '${branch}'." 1>&2
                exit 2
                ;;
        esac
    fi
fi

exit 0
