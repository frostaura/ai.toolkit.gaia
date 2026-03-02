# Gaia MCP Error Codes

## Tasks

- `GAIA_TASKS_ERR_NEEDS_INPUT_UNRESOLVED` — needs-human-input blockers still present (checked first for specificity).
- `GAIA_TASKS_ERR_BLOCKERS_UNRESOLVED` — task has generic blockers.
- `GAIA_TASKS_ERR_MISSING_PROOF_ARGS` — `mark_done` missing required proof fields.
- `GAIA_TASKS_ERR_GATES_UNSATISFIED` — required gates not satisfied.
- `GAIA_TASKS_ERR_TASK_NOT_FOUND` — task ID does not exist in the specified project.
- `GAIA_TASKS_ERR_INVALID_STATUS` — status value is not one of `todo`, `doing`, `done`.

## Guidance

Errors should include:

- the code
- a human message explaining exactly what is missing
- suggested next action (what to provide or which tool to call)
