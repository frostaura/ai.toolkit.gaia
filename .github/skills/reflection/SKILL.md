---
name: reflection
description: Lightweight post-task reflection to capture significant learnings. Store patterns that worked, issues resolved, and decisions made. Not required for trivial tasks.
---

# Reflection Skill

Capture significant learnings after task completion.

## When to Reflect

- ✅ After solving tricky problems
- ✅ After fixing non-obvious bugs
- ✅ After making architectural decisions
- ✅ After failed approaches (to prevent repeating)
- ❌ Not needed for trivial fixes

## Reflection Process

### 1. Quick Analysis
- What was the goal?
- What approach worked?
- What didn't work?

### 2. Store Learnings

**Success patterns:**
```javascript
remember("pattern", "[context]", "[what worked]", "ProjectWide")
```

**Issues resolved:**
```javascript
remember("fix", "[issue_key]", "[problem + solution]", "ProjectWide")
```

**Decisions made:**
```javascript
remember("decision", "[choice]", "[rationale]", "ProjectWide")
```

**Warnings discovered:**
```javascript
remember("warning", "[context]", "[gotcha to avoid]", "ProjectWide")
```

## Examples

```javascript
// After fixing a tricky bug
remember("fix", "jwt_clock_skew", 
  "Token validation failed due to clock skew between servers. Added 30s buffer.",
  "ProjectWide")

// After finding good pattern
remember("pattern", "error_handling_api",
  "Use Result<T> pattern with typed errors. Controllers map to HTTP codes.",
  "ProjectWide")

// After trying something that failed
remember("warning", "vite_ssr_library_x",
  "Library X incompatible with SSR mode. Use client-only import.",
  "ProjectWide")
```

## Keep It Lightweight

- One `remember()` call per significant learning
- Descriptive keys for searchability
- Concise but complete values
- Don't over-document trivial work
