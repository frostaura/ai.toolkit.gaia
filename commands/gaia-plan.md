---
description: Turn the current architecture into a branch-aware execution plan with dependencies, gates, and proof expectations.
argument-hint: "<optional: scope hint or branch name>"
---

You are running Gaia's planning flow. Delegate to the `gaia-implementation-planner`
agent (Task / Agent tool with `subagent_type: gaia-implementation-planner`).

Inputs the planner needs:
- current state of `docs/architecture/` (assume it is current; if not, stop and
  invoke the architect first),
- any scope hint the user provided: $ARGUMENTS,
- the existing task graph if one exists (call `mcp__gaia-remote__tasks_list`).

The planner must produce an actionable plan that:
- exposes safe parallel branches and explicit serial dependencies,
- declares `required_gates` per task (lint, build, ci, unit, integration, e2e,
  manual-regression, docs-updated, docker-ready),
- creates the tasks via `mcp__gaia-remote__tasks_create`,
- never marks anything `done` here — that is the release engineer's call.
