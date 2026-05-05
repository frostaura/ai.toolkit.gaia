#!/usr/bin/env bash
# Gaia Stop / sessionEnd hook. Flushes a session-summary record so a future
# Repo Explorer run can correlate with prior sessions. Never blocks.

set -eu

LOG_DIR="${GAIA_LOG_DIR:-${PWD}/.gaia}"
mkdir -p "${LOG_DIR}"

ts="$(date -u +%Y-%m-%dT%H:%M:%SZ)"

prompts=0
tools=0
[ -f "${LOG_DIR}/prompts.jsonl" ] && prompts="$(wc -l < "${LOG_DIR}/prompts.jsonl" | tr -d ' ')"
[ -f "${LOG_DIR}/tools.jsonl" ]   && tools="$(wc -l < "${LOG_DIR}/tools.jsonl"   | tr -d ' ')"

printf '{"ts":"%s","event":"session-end","prompts":%s,"tool_calls":%s}\n' \
    "${ts}" "${prompts}" "${tools}" >> "${LOG_DIR}/session.jsonl"

exit 0
