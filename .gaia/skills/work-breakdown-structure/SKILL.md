---
name: work-breakdown-structure
description: Guide for decomposing projects into Epics, Stories, Features, and Tasks. Use when planning features or capturing work in MCP tools.
---

# Work Breakdown Structure (WBS)

## Hierarchy

| Level       | Purpose                | Example                        |
| ----------- | ---------------------- | ------------------------------ |
| **Epic**    | Major objective        | "User Authentication System"   |
| **Story**   | User-facing capability | "Users can register and login" |
| **Feature** | Technical component    | "JWT token management"         |
| **Task**    | Atomic unit (1-4 hrs)  | "Create JWT signing service"   |

## ID Convention

```
Epic:    E-[n]                    → E-1
Story:   E-[n]/S-[n]              → E-1/S-1
Feature: E-[n]/S-[n]/F-[n]        → E-1/S-1/F-1
Task:    E-[n]/S-[n]/F-[n]/T-[n]  → E-1/S-1/F-1/T-1
```

## Minimum Decomposition by SDLC

| Tier       | Epics | Stories | Features | Tasks |
| ---------- | ----- | ------- | -------- | ----- |
| Small      | 1     | 2+      | 3+       | 5+    |
| Medium     | 2     | 4+      | 8+       | 15+   |
| Large      | 3     | 8+      | 15+      | 30+   |
| Enterprise | 5     | 15+     | 30+      | 60+   |

## MCP Task Format

**Pattern**: `[TYPE] Title | Refs: doc#section | AC: Acceptance criteria`

```bash
# Epic
mcp__gaia__update_task("E-1", "[EPIC] Auth System | Refs: security.md | AC: All auth flows work", "pending", "Architect")

# Story
mcp__gaia__update_task("E-1/S-1", "[STORY] User login | Refs: api.md#auth | AC: Login/logout E2E", "pending", "Builder")

# Feature
mcp__gaia__update_task("E-1/S-1/F-1", "[FEATURE] JWT tokens | Refs: security.md#jwt | AC: Token refresh works", "pending", "Builder")

# Task
mcp__gaia__update_task("E-1/S-1/F-1/T-1", "[TASK] JWT service | Refs: security.md#jwt | AC: Valid JWTs generated", "pending", "Builder")
```

## Rules

- Every item MUST reference design docs
- Every item MUST have testable acceptance criteria
- Tasks should be 1-4 hours (exceptions: research, debugging)
- **NEVER** create TODO.md or markdown task files - use MCP only
