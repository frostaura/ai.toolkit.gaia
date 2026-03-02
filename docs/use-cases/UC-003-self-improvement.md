# UC-003 — Self-Improvement

## Goal

Log process improvement suggestions so agents learn from mistakes and get better over time.

## Actors

- Any Gaia Agent (logs improvements)
- Workload Orchestrator (reviews and applies improvements)

## Preconditions

- MCP server is running and accessible

## Main Flow

1. Agent identifies a recurring problem or lesson learned.
2. Agent calls `self_improve_log` with project, suggestion, and optional category.
3. At session start, orchestrator calls `self_improve_list` to review backlog.
4. After applying a lesson, orchestrator calls `self_improve_mark_applied`.

## Variants / Edge Cases

- `self_improve_clear` with project: removes only that project's suggestions
- `self_improve_clear` without project: wipes all suggestions
- Category filter is case-insensitive
- Project filter is exact match

## Acceptance Criteria

- IDs are unique 32-char hex strings
- Category filtering is case-insensitive
- `mark_applied` sets `Applied=true`
- `mark_applied` returns `ok=false` for non-existent ID
- All operations are thread-safe

## Notes

Self-improvement items are stored globally in `__global.improvements.json`.
