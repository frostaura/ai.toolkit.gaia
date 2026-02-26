# SKILL: Tasking and Proof (MCP-Enforced)

## Task model
- statuses: `todo | doing | done`
- `blockers[]`: non-empty means the task is blocked
- `required_gates[]`: explicitly set by orchestrator when task is created

## Required gates (examples)
- `lint`, `build`, `ci`, `unit`, `integration`, `e2e`, `manual-regression`, `dockerize`

## Proof args (required for mark_done)
- `changed_files[]`: file paths
- `tests_added[]`: file paths
- `manual_regression[]`: labels (`curl`, `playwright-mcp`)

## In-flight capture
- discovered TODOs become tasks or blockers
- no orphan TODO comments

## Needs human input
Use MCP `flag_needs_input(task_id, questions[])` to add blockers and prevent completion.

## Done when
- tasks are comprehensive
- mark_done calls include required proof args and pass validation
