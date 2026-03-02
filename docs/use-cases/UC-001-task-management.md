# UC-001 — Task Management

## Goal

Enable AI agents to create, track, update, and complete tasks with enforced quality gates and proof requirements.

## Actors

- Workload Orchestrator (creates/manages tasks)
- Developer Agent (works on tasks)
- Quality Gatekeeper (reviews completion)

## Preconditions

- MCP server is running and accessible
- Project identifier is known

## Main Flow

1. Orchestrator calls `tasks_create` with project, title, optional description and required gates.
2. Agent calls `tasks_update` to transition status to "doing".
3. Agent performs work, then calls `tasks_update` to record gates satisfied.
4. Agent calls `tasks_mark_done` with proof args (changedFiles, testsAdded, manualRegressionLabels).
5. Server validates: no blockers, all gates satisfied, all proof non-empty.
6. On success: status set to "done". On failure: structured error returned with code and message.

## Variants / Edge Cases

- `tasks_flag_needs_input`: blocks task with human-input questions
- `tasks_delete`: removes a task permanently
- `tasks_clear`: wipes all tasks for a project
- Invalid status values (not todo/doing/done) return `INVALID_STATUS` error
- Non-existent task IDs return `TASK_NOT_FOUND` error

## Acceptance Criteria

- Task IDs are unique 32-char hex strings
- Status validation rejects invalid values
- Completion requires all three proof arrays non-empty
- NEEDS_INPUT blockers checked before generic blockers
- All operations are thread-safe and persist atomically

## Notes

Tasks are stored as per-project JSON files in the data directory.
