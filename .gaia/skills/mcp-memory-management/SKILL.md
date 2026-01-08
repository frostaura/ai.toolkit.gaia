---
name: mcp-memory-management
description: Guide for using Gaia's MCP memory tools (remember/recall) with session-level and persistent storage. Use when debugging, implementing features, or making decisions.
---

# MCP Memory Management

## Tools

- `mcp__gaia__remember(category, key, value, duration)` - Store knowledge with optional persistence
- `mcp__gaia__recall(query, maxResults?)` - Search memories (fuzzy matching, aggregates session + persistent)

## Memory Duration Types

### SessionLength (Default)

- **Duration**: Current terminal session only
- **Persistence**: Lost when MCP service restarts
- **Use For**: Temporary context, debugging notes, session-specific state

### ProjectWide

- **Duration**: Permanent (survives restarts)
- **Persistence**: Stored in `.gaia/memory.json`
- **Use For**: Long-term knowledge, architectural decisions, important patterns

## Workflow

1. **BEFORE** any task: `recall("[keywords]")` - check past knowledge (searches both session + persistent)
2. **AFTER** any fix/learning: `remember(category, key, value, duration)` - store it
3. **Choose duration**:
   - Use `SessionLength` for temporary/debugging context
   - Use `ProjectWide` for permanent knowledge that should persist

## When to Use Each Duration

### Use SessionLength (Default) For:

- ❌ Temporary debugging notes
- ❌ Current session state
- ❌ Work-in-progress context
- ❌ Exploratory findings that might change
- ❌ User-specific ephemeral data

### Use ProjectWide For:

- ✅ Bug fixes and their root causes
- ✅ Working configurations that should be reused
- ✅ Architectural decisions
- ✅ Performance optimizations
- ✅ Library/dependency compatibility notes
- ✅ Reusable code patterns
- ✅ Environment-specific quirks

## Categories

| Category      | Use For                | Example Key            | Recommended Duration |
| ------------- | ---------------------- | ---------------------- | -------------------- |
| `issue`       | Bug fixes              | `"null_pointer_user"`  | ProjectWide          |
| `workaround`  | Temporary solutions    | `"docker_arm_compat"`  | ProjectWide          |
| `config`      | Working configurations | `"vite_proxy_setup"`   | ProjectWide          |
| `pattern`     | Reusable code patterns | `"retry_logic"`        | ProjectWide          |
| `performance` | Optimizations          | `"db_index_user_id"`   | ProjectWide          |
| `test_fix`    | Test solutions         | `"jest_async_timeout"` | ProjectWide          |
| `dependency`  | Library notes          | `"react_18_types"`     | ProjectWide          |
| `environment` | Platform quirks        | `"gh_actions_node20"`  | ProjectWide          |
| `decision`    | Architecture choices   | `"rest_vs_graphql"`    | ProjectWide          |
| `debug`       | Debugging context      | `"current_test_run"`   | SessionLength        |
| `state`       | Session state          | `"last_api_response"`  | SessionLength        |

## Key Naming

Use descriptive, searchable keys:

- ✅ `"typescript_path_error"` - searchable by "typescript", "path", "error"
- ❌ `"fix1"` - not searchable

## Examples

### Session-Level Memories (Default)

```javascript
// Temporary debugging context
remember("debug", "current_user_id", "user-123", "SessionLength");

// Session state
remember("state", "last_build_result", "success", "SessionLength");

// Work-in-progress notes
remember(
  "debug",
  "investigating_timeout",
  "Occurs only on Chrome",
  "SessionLength"
);
```

### Persistent Memories

```javascript
// Before debugging - searches both session AND persistent memories
recall("timeout");
recall("typescript error");

// After fixing an issue - PERSIST the solution
remember(
  "issue",
  "ts_module_resolution",
  "Fixed by setting moduleResolution to bundler",
  "ProjectWide"
);

// After failed attempt (prevents repeating mistakes) - PERSIST
remember(
  "issue",
  "vite_ssr_failed",
  "SSR mode incompatible with this library",
  "ProjectWide"
);

// After finding working config - PERSIST
remember(
  "config",
  "eslint_react",
  "Needs plugin:react/recommended for JSX",
  "ProjectWide"
);

// Performance optimization - PERSIST
remember(
  "performance",
  "db_query_users",
  "Added index on email field, 10x speedup",
  "ProjectWide"
);

// Architecture decision - PERSIST
remember(
  "decision",
  "auth_approach",
  "Using JWT with 15min access + 7day refresh tokens",
  "ProjectWide"
);
```

## Best Practices

### DO:

- ✅ Use `recall()` BEFORE every task to check for past knowledge
- ✅ Use `ProjectWide` for permanent knowledge (bug fixes, configs, patterns)
- ✅ Use `SessionLength` for temporary context (default behavior)
- ✅ Store after EVERY fix or learning
- ✅ Store after EVERY failed attempt to prevent repeating
- ✅ Use descriptive, searchable keys

### DON'T:

- ❌ Skip `recall()` - always check past knowledge first
- ❌ Use `ProjectWide` for temporary debugging notes
- ❌ Use vague keys like "fix1" or "temp"
- ❌ Forget to document failed attempts
- ❌ Store duplicate knowledge (upserts automatically by category+key)

## Technical Details

- **Session Storage**: In-memory ConcurrentDictionary (cleared on restart)
- **Persistent Storage**: `.gaia/memory.json` (survives restarts)
- **File Safety**: Semaphore-protected writes prevent locking issues
- **Recall**: Automatically aggregates results from both storage types
- **Upsert**: Category+Key uniqueness prevents duplicates
