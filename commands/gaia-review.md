---
description: Run the Gaia Quality Gatekeeper checklist on the current branch — gates, proof, drift, CI status.
argument-hint: "<optional: task id to review>"
---

You are running Gaia's QA Gatekeeper review. Delegate to `gaia-tester` for
test-evidence review and to `gaia-release-engineer` for gate readiness.

Checklist (per AGENTS.md §11 "Definition of Done"):
1. Required docs/specs are updated when behavior changed.
2. CI exists and is green (or will be once merged).
3. Required gates pass for the task: lint, build, unit/integration/e2e as
   declared in `required_gates[]`.
4. Proof recorded via `mcp__gaia-remote__tasks_complete` — `changed_files[]`,
   `tests_added[]`, `manual_regression[]` are non-empty.
5. No "TODO left behind" — every TODO has a tracked task or blocker.

If a task id was supplied ($ARGUMENTS), review that task in particular. Otherwise
list all `doing` tasks via `mcp__gaia-remote__tasks_list` and review each.

Output: pass | fail | blocked, with the owning blocker named when fail.
