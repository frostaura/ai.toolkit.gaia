# UC-002 — Project Memory

## Goal

Persist stable facts (build commands, env vars, conventions) per project so all agents share context across sessions.

## Actors

- Any Gaia Agent (reads/writes memory)
- Repo Explorer (populates initial facts)

## Preconditions

- MCP server is running and accessible
- Project identifier is known

## Main Flow

1. Agent calls `memory_remember` with project, key, and value.
2. If key exists: value is updated (upsert), `UpdatedUtc` is refreshed.
3. If key is new: a new memory item is created.
4. Agent calls `memory_recall` to retrieve all facts or filter by key prefix.

## Variants / Edge Cases

- `memory_forget`: removes a single fact by exact key
- `memory_clear`: wipes all facts for a project
- Key prefix filtering is case-insensitive
- Non-existent project returns empty list

## Acceptance Criteria

- Upsert semantics: same key updates rather than duplicates
- Key prefix match is case-insensitive
- Operations are thread-safe and persist atomically

## Notes

Memory is stored as per-project JSON files in the data directory.
