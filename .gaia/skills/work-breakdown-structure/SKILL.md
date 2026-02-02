---
name: work-breakdown-structure
description: Guide for adaptive work breakdown based on task complexity. Uses flat task lists for Standard tasks, two-level hierarchy for Complex, and full WBS (Epic→Story→Feature→Task) for Enterprise only. Prevents over-engineering simple tasks.
---

# Work Breakdown Structure (WBS)

## Adaptive Depth

Match breakdown depth to task complexity:

| Complexity | Breakdown Style |
|------------|-----------------|
| Trivial | None - just do it |
| Simple | None - analyze → fix → verify |
| Standard | Flat task list |
| Complex | Two-level hierarchy |
| Enterprise | Full 4-level WBS |

## Breakdown Examples

### Trivial/Simple
```markdown
No breakdown needed. Execute directly.
```

### Standard (Flat List)
```markdown
- [ ] Implement JWT service
- [ ] Add login endpoint
- [ ] Add refresh endpoint
- [ ] Write tests
```

### Complex (Two Levels)
```markdown
## Feature: Authentication
- [ ] JWT token service
- [ ] Login/logout endpoints
- [ ] Refresh token flow

## Feature: User Management
- [ ] Registration endpoint
- [ ] Profile endpoints
```

### Enterprise (Full WBS)
```markdown
E-1: Epic - User Authentication System
  E-1/S-1: Story - Users can login
    E-1/S-1/F-1: Feature - JWT tokens
      E-1/S-1/F-1/T-1: Task - Create JWT service
      E-1/S-1/F-1/T-2: Task - Add token validation
```

## ID Convention (Enterprise Only)

```
Epic:    E-[n]                    → E-1
Story:   E-[n]/S-[n]              → E-1/S-1
Feature: E-[n]/S-[n]/F-[n]        → E-1/S-1/F-1
Task:    E-[n]/S-[n]/F-[n]/T-[n]  → E-1/S-1/F-1/T-1
```

## MCP Task Format

For Standard+ complexity:

```javascript
// Standard: Simple task
update_task("T-1", 
  "Implement JWT service | Refs: design.md#auth | AC: Valid tokens generated",
  "Pending", "Developer")

// Complex: Feature with tasks
update_task("F-1", 
  "[FEATURE] Authentication | Refs: design.md#auth | AC: Login flow works",
  "Pending", "Developer")

// Enterprise: Full hierarchy
update_task("E-1/S-1/F-1/T-1",
  "[TASK] JWT service | Refs: security.md#jwt | AC: Tokens valid",
  "Pending", "Developer")
```

## Task Description Format

```
[TYPE] Title | Refs: doc#section | AC: Acceptance criteria
```

Types: `[EPIC]`, `[STORY]`, `[FEATURE]`, `[TASK]`

## Rules

- ✅ Match depth to complexity
- ✅ Reference design docs
- ✅ Include acceptance criteria
- ✅ Use MCP tools only
- ❌ Don't over-engineer simple tasks
- ❌ Never create TODO.md files
