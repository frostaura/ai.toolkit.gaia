# Gaia MCP Error Codes

## Tasks
- `GAIA_TASKS_ERR_MISSING_PROOF_ARGS` — `mark_done` missing required proof fields.
- `GAIA_TASKS_ERR_MISSING_PROOF_PATHS` — referenced file paths do not exist.
- `GAIA_TASKS_ERR_BLOCKERS_UNRESOLVED` — task has blockers.
- `GAIA_TASKS_ERR_GATES_UNSATISFIED` — required gates not satisfied.
- `GAIA_TASKS_ERR_NEEDS_INPUT_UNRESOLVED` — needs-human-input blockers still present.

## Guidance
Errors should include:
- the code
- a human message explaining exactly what is missing
- suggested next action (what to provide or which tool to call)
