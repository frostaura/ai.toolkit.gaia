# Gaia MCP Server (Custom)

This directory contains a custom **.NET-based MCP server** that provides tools for:
- **tasks**: project-scoped task tracking with hard completion enforcement
- **memories**: stable project facts (how to run, env vars, conventions)
- **evolve**: agent self-evolution backlog — lessons learned, loop breakers, process upgrades

## Core enforcement
`tasks.mark_done` must refuse with a **clear, code+message** error unless:
- task has no `blockers[]`
- proof args are provided (paths/labels only)
- required gates are satisfied (as declared by the orchestrator)

`tasks.update` validates:
- status must be one of `todo`, `doing`, `done`
- task ID must exist (returns `TASK_NOT_FOUND` error)

## Proof args (required)
- `changed_files[]`: file paths
- `tests_added[]`: file paths
- `manual_regression[]`: labels like `curl`, `playwright-mcp`

## Needs human input
`tasks.flag_needs_input(task_id, questions[])`:
- appends to `blockers[]`
- prevents completion until resolved

## Error codes
Namespaced codes: `GAIA_TASKS_ERR_*`.
See `schemas/error-codes.md`.
