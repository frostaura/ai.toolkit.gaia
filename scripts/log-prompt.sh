#!/usr/bin/env bash
# Gaia UserPromptSubmit hook. Appends a JSONL record locally so the orchestrator
# can audit prompt history without leaking content to remote services. Never
# blocks the prompt; always exits 0.

set -eu

LOG_DIR="${GAIA_LOG_DIR:-${PWD}/.gaia}"
mkdir -p "${LOG_DIR}"

ts="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
prompt_chars="${#CLAUDE_USER_PROMPT:-0}"

printf '{"ts":"%s","event":"prompt","chars":%s}\n' \
    "${ts}" "${prompt_chars}" >> "${LOG_DIR}/prompts.jsonl"

exit 0
