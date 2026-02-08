---
name: mcp-memory-management
description: Guide for using Gaia's MCP memory tools (remember/recall) with session-level and persistent storage. Used by all agents to recall past solutions before tasks and remember significant learnings after fixes.
---

# MCP Memory Management

## Tools

```javascript
// Search memories (fuzzy match across session + persistent)
recall(query, maxResults?)

// Store knowledge
remember(category, key, value, duration)
```

## Duration Types

| Duration | Persistence | Use For |
|----------|-------------|---------|
| **SessionLength** | Current session only | Debugging notes, temp context |
| **ProjectWide** | Permanent (memory.json) | Solutions, patterns, decisions |

## Categories

| Category | Purpose | Typical Duration |
|----------|---------|------------------|
| `fix` | Bug solutions | ProjectWide |
| `pattern` | Reusable approaches | ProjectWide |
| `config` | Working configurations | ProjectWide |
| `decision` | Architectural choices | ProjectWide |
| `warning` | Gotchas and caveats | ProjectWide |
| `context` | Current task state | SessionLength |

## Protocol

### When to Recall
- **At task START** - check for relevant past knowledge
- **Before debugging** - search for similar issues
- **Before research** - check if already known

### When to Remember
- **After fixing bugs** - document the solution
- **After finding patterns** - capture for reuse
- **After making decisions** - record rationale
- **After failed approaches** - prevent repeating

### Key Naming

Use descriptive, searchable keys:
- ✅ `typescript_path_error` - searchable
- ✅ `jwt_refresh_implementation` - descriptive
- ❌ `fix1` - not searchable

## Examples

```javascript
// Before starting work
recall("authentication")
recall("jwt")

// After solving a tricky problem
remember("fix", "jwt_token_expiry", 
  "Token was expiring early due to clock skew. Added 30s buffer.",
  "ProjectWide")

// After finding a working pattern
remember("pattern", "error_handling_api",
  "Use Result<T> pattern with typed errors. See src/utils/result.ts",
  "ProjectWide")

// After architectural decision
remember("decision", "state_management",
  "Chose Zustand over Redux: simpler API, smaller bundle, sufficient for our needs",
  "ProjectWide")

// Temporary debugging context
remember("context", "current_investigation",
  "Tracking down memory leak in auth service",
  "SessionLength")
```

## Best Practices

### DO
- ✅ Recall at task start, not every sub-task
- ✅ Remember significant learnings, not trivia
- ✅ Use ProjectWide for solutions worth keeping
- ✅ Use descriptive, searchable keys
- ✅ Include context in values

### DON'T
- ❌ Remember every minor fix
- ❌ Use vague keys like "temp" or "fix1"
- ❌ Duplicate existing memories (auto-upserts by category+key)
- ❌ Use ProjectWide for debugging notes
