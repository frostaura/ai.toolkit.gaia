#!/usr/bin/env bash
# Gaia PostToolUse hook. Stamps drift markers when files under /docs/ change
# without a paired code change in the same session, so the architect role
# notices doc-only updates that may signal new architectural intent.

set -eu

LOG_DIR="${GAIA_LOG_DIR:-${PWD}/.gaia}"
mkdir -p "${LOG_DIR}"

ts="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
tool_name="${CLAUDE_TOOL_NAME:-}"
file_path="${CLAUDE_TOOL_FILE_PATH:-}"

printf '{"ts":"%s","event":"tool","tool":"%s","path":"%s"}\n' \
    "${ts}" "${tool_name}" "${file_path}" >> "${LOG_DIR}/tools.jsonl"

case "${file_path}" in
    *"/docs/"*|*"docs/architecture/"*)
        printf '{"ts":"%s","event":"docs-touched","path":"%s"}\n' \
            "${ts}" "${file_path}" >> "${LOG_DIR}/drift-watch.jsonl"
        ;;
esac

exit 0
