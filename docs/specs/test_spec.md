# Test Specification — Gaia AI Toolkit

## Scope

Comprehensive test coverage for the Gaia MCP Server (`Gaia.Mcp.Server`) and Gaia Workflows Server (`Gaia.Workflows.Server`).

## Strategy (unit / integration / e2e)

### Unit Tests

#### CompletionValidator
- Returns null for valid task (all gates satisfied, all proof non-empty, no blockers)
- Returns NEEDS_INPUT error when NEEDS_INPUT blockers present (checked first)
- Returns BLOCKERS_UNRESOLVED error when generic blockers present
- Returns MISSING_PROOF_ARGS when changedFiles is empty
- Returns MISSING_PROOF_ARGS when testsAdded is empty
- Returns MISSING_PROOF_ARGS when manualRegression is empty
- Returns MISSING_PROOF_ARGS listing all missing arrays when multiple are empty
- Returns GATES_UNSATISFIED when required gates not all satisfied
- Prioritizes NEEDS_INPUT over generic blockers when both present
- Passes gate check when no required gates exist
- NEEDS_INPUT check is case-insensitive

#### TasksTool
- Create: generates unique 32-char hex ID, sets status to "todo", stores in project
- Create: stores optional description and required gates
- Create: defaults to empty gates when none provided
- Create: multiple tasks get unique IDs
- Create: persists task
- List: returns empty list for non-existent project
- List: returns all tasks for a project
- Update: changes title when provided
- Update: changes description when provided
- Update: changes status when provided
- Update: replaces requiredGates when provided
- Update: replaces gatesSatisfied when provided
- Update: replaces blockers when provided (including clearing with empty array)
- Update: leaves fields unchanged when null passed
- Update: returns INVALID_STATUS error for invalid status values
- Update: returns TASK_NOT_FOUND error for non-existent task ID
- Update: accepts status case-insensitively
- MarkDone: sets status to "done" when all validation passes
- MarkDone: returns error with ok=false when validation fails
- MarkDone: reverts proof args on validation failure
- MarkDone: succeeds with all gates satisfied
- MarkDone: fails with unsatisfied gates
- MarkDone: fails with blockers present
- MarkDone: returns TASK_NOT_FOUND error for non-existent task ID
- FlagNeedsInput: appends "NEEDS_INPUT: <question>" blockers
- FlagNeedsInput: supports multiple questions in one call
- FlagNeedsInput: returns TASK_NOT_FOUND error for non-existent task ID
- Delete: removes task and returns ok=true
- Delete: returns ok=false for non-existent task
- ClearAll: removes all tasks for project

#### MemoryTool
- Remember: creates new memory item with key, value, project
- Remember: upserts (updates value and UpdatedUtc) when key exists
- Remember: updates timestamp on upsert
- Recall: returns all items for project
- Recall: filters by key prefix (case-insensitive)
- Recall: key prefix filter is case-insensitive
- Recall: returns empty list for non-existent project
- Forget: removes item by exact key, returns ok=true
- Forget: returns ok=false for non-existent key
- Clear: removes all items for project

#### SelfImproveTool
- Log: creates item with unique ID, project, suggestion, optional category
- Log: with category stores it
- Log: without category stores null
- List: returns all items globally
- List: filters by project
- List: filters by category (case-insensitive)
- List: filters by both project and category
- List: category filter is case-insensitive
- MarkApplied: sets Applied=true, returns ok=true
- MarkApplied: returns ok=false for non-existent ID
- Clear: with project removes only that project's items
- Clear: without project removes all items

#### ThreadSafeJsonStore
- LoadAsync: returns empty list for non-existent file
- SaveAsync: persists and LoadAsync retrieves the data
- SaveAsync: overwrites existing data
- MutateAsync: applies mutation and persists atomically
- MutateAsync: atomic write does not corrupt data
- Concurrent access: different keys don't corrupt each other
- Concurrent access: same key doesn't lose data
- SanitizeKey: removes path traversal characters
- SanitizeKey: allows valid characters (alphanumeric, dash, underscore, dot)
- SanitizeKey: throws on empty input
- SanitizeKey: throws on null input
- PathTraversal: file is created within data directory, not outside

#### JsonTaskStore
- Delegates to ThreadSafeJsonStore with ".tasks.json" suffix

#### WorkflowParser
- Parse: returns null for non-existent file
- Parse: returns null for invalid YAML
- Parse: returns null for YAML without description
- Parse: sets name from filename when name is empty
- Parse: correctly deserializes name, description, params, steps
- ScanDirectory: returns empty for non-existent directory
- ScanDirectory: finds and parses all .yml files
- ScanDirectory: sorts results by name
- ScanDirectory: excludes invalid workflows

#### WorkflowsTool
- ListWorkflows: delegates to WorkflowParser.ScanDirectory
- ExecuteWorkflow: returns error for non-existent workflow
- ExecuteWorkflow: returns error for workflow with no steps
- ExecuteWorkflow: returns error for invalid args JSON
- ExecuteWorkflow: executes steps sequentially
- ExecuteWorkflow: applies ${{ params.X }} substitution
- ExecuteWorkflow: applies ${{ steps.id.output }} substitution
- ExecuteWorkflow: sets env vars from args
- ExecuteWorkflow: returns ok=false with failed step info on non-zero exit
- ExecuteWorkflow: returns ok=true with output on success

### Integration Tests

#### HTTP MCP Server
- Server starts and responds on /mcp endpoint
- Full task lifecycle: create → list → update → mark_done
- Full memory lifecycle: remember → recall → forget → clear
- Full self-improvement lifecycle: log → list → mark_applied → clear
- Concurrent task creation produces unique IDs

## Prerequisites

- .NET 10.0 SDK
- Docker (for docker-compose stack)

## How to Run

```bash
# All tests
make test

# Unit tests only
dotnet test .github/mcp/tests/Gaia.Mcp.Server.Tests
dotnet test .github/mcp-workflows/tests/Gaia.Workflows.Server.Tests

# Integration tests only
dotnet test .github/mcp/tests/Gaia.Mcp.Server.IntegrationTests
```

## Expected Results

All tests pass. Zero failures.

## Regression Notes

- CompletionValidator checks NEEDS_INPUT blockers before generic blockers for specificity
- Proof args validation requires ALL THREE arrays non-empty
- MemoryTool.Recall key prefix match is case-insensitive
- SelfImproveTool category filter is case-insensitive
- ThreadSafeJsonStore uses temp-file-then-move for atomic writes
- Project names are sanitized to prevent path traversal
- Status validation rejects values not in {todo, doing, done}
- Non-existent task IDs return structured TASK_NOT_FOUND errors

## Notes

Test isolation: each unit test creates a fresh temp directory for storage to avoid cross-test contamination.
