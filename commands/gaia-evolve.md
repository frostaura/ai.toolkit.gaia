---
description: Review and apply Gaia evolution lessons. Logs new lessons learned and applies pending ones.
argument-hint: "<optional: lesson to log, or 'apply <id>'>"
---

You are running Gaia's continuous-evolution loop.

If $ARGUMENTS starts with "apply", call `mcp__gaia-remote__evolve_apply` with the
supplied id and confirm the application.

Otherwise:
1. Call `mcp__gaia-remote__evolve_list` to load pending lessons for this project.
2. If $ARGUMENTS is non-empty, log it as a new lesson via
   `mcp__gaia-remote__evolve_log` (project, suggestion, category if obvious).
3. Summarize the open lessons by category and recommend one to apply next.

Per AGENTS.md §12: log evolution after mistakes/inefficiencies, apply when the
loop closes. Keep lessons specific and actionable.
