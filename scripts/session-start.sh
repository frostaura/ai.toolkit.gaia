#!/usr/bin/env bash
# Gaia SessionStart hook (Claude Code).
# Records that a Gaia-enabled session started. Reminds the agent (via stdout
# additionalContext, where supported) to call memory_recall and evolve_list
# per AGENTS.md section 12.

set -eu

LOG_DIR="${GAIA_LOG_DIR:-${PWD}/.gaia}"
mkdir -p "${LOG_DIR}"

ts="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
project="$(basename "${PWD}")"

printf '{"ts":"%s","event":"session-start","project":"%s","env":"%s"}\n' \
    "${ts}" "${project}" "${GAIA_ENV:-unknown}" >> "${LOG_DIR}/session.jsonl"

cat <<'EOF'
Gaia is active.

Session-start checklist (per AGENTS.md):
  - Call memory_recall(project) to load persisted facts.
  - Call evolve_list() to load lessons from prior sessions.
  - Run Repo Explorer first if this is new work.

If /docs/ is the source of truth and code disagrees, fix drift before features.
EOF
