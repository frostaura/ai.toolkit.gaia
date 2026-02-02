---
name: sdlc-tier-selection
description: Guide for auto-detecting task complexity (Trivial/Simple/Standard/Complex/Enterprise) and selecting the appropriate process. Determines required design documents and workflow phases based on scope indicators.
---

# SDLC Tier Selection

## Complexity Detection

| Complexity | Time Estimate | Scope | Files | Process |
|------------|---------------|-------|-------|---------|
| **Trivial** | < 1 hour | Typo, 1-line fix | 1-2 | Fix → Verify |
| **Simple** | < 1 day | Bug fix, tweak | 2-5 | Analyze → Fix → Verify |
| **Standard** | 1-3 days | Single feature | 5-15 | Plan → Implement → Test → Deploy |
| **Complex** | 3-7 days | Multiple features | 15+ | Design → Plan → Implement → Validate → Deploy |
| **Enterprise** | 1+ weeks | Full system | Major | Full phased development |

> ⚠️ Time estimates are complexity indicators only. Never refuse scope based on time.

## Process by Complexity

### Trivial
No planning needed. Execute directly.

### Simple
```
1. @Analyst: Quick investigation
2. @Developer: Fix
3. @Quality: Verify
```

### Standard
```
1. @Planner: Create task list
2. @Developer: Implement
3. @Quality: Validate
4. @Operator: Deploy
```

### Complex
```
1. @Planner: Create design docs
2. @Planner: Create task hierarchy
3. @Developer: Implement
4. @Quality: Full validation
5. @Operator: Deploy + Document
```

### Enterprise
Full phased development with all agents.

## Design Docs by Complexity

| Complexity | Design Docs Required |
|------------|---------------------|
| Trivial | None |
| Simple | None |
| Standard | design.md |
| Complex | design.md + relevant (api, data, security) |
| Enterprise | Full set as needed |

## Repository State Check

| State | Action |
|-------|--------|
| Empty (no src/) | Start fresh |
| Has code + designs | Update existing |
| Has code, no designs | Analyze first, create designs |

## Auto-Detection Indicators

**Trivial**: Single typo, comment fix, config value change
**Simple**: Bug with known cause, small UI tweak, dependency update
**Standard**: New endpoint, new component, feature addition
**Complex**: New subsystem, major refactor, multi-service change
**Enterprise**: New application, platform migration, major overhaul

## Store Selection

```javascript
remember("complexity", "current_task", "[tier]", "SessionLength")
```
