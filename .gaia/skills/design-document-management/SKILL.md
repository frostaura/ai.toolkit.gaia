---
name: design-document-management
description: Guide for managing the 5 design documents in .gaia/designs/. Use when creating, updating, or validating design documentation.
---

# Design Document Management

## Design Documents (in `.gaia/designs/`)

| Document          | Purpose                                      | Required From |
| ----------------- | -------------------------------------------- | ------------- |
| `use-cases.md`    | Use cases, user flows, API/UI journeys       | Micro+        |
| `architecture.md` | System design, components, patterns          | Small+        |
| `api.md`          | API endpoints, contracts, requests/responses | Small+        |
| `database.md`     | Schema, models, relationships, migrations    | Medium+       |
| `security.md`     | Auth, authorization, RBAC, tokens            | Large+        |
| `frontend.md`     | UI/UX patterns, components, state            | Large+        |

## Tiered Requirements by SDLC

| SDLC Tier        | Required Documents                      |
| ---------------- | --------------------------------------- |
| Micro            | use-cases.md (if needed)                |
| Small            | use-cases.md + architecture.md + api.md |
| Medium           | + database.md                           |
| Large/Enterprise | All 6 documents                         |

## Quality Rules

- ✅ No `[TODO]` or `[TBD]` placeholders in required docs
- ✅ Consistent terminology across all docs
- ✅ Every requirement maps to a design section
- ✅ Follow Clean Architecture / iDesign principles

## Completion Checklist

Before creating tasks, verify:

- [ ] All required docs for SDLC tier exist
- [ ] No template placeholders remain
- [ ] Designs capture 100% of requirements
- [ ] Entity names match across docs
- [ ] API paths consistent
- [ ] Database schema aligns with API contracts

## Store Design Decisions

```bash
mcp__gaia__remember("design", "use-cases", "[user flows and journeys]", "ProjectWide")
mcp__gaia__remember("design", "architecture", "[key decisions]", "ProjectWide")
mcp__gaia__remember("design", "api", "[endpoint designs]", "ProjectWide")
mcp__gaia__remember("design", "database", "[schema decisions]", "ProjectWide")
mcp__gaia__remember("design", "security", "[auth approach]", "ProjectWide")
mcp__gaia__remember("design", "frontend", "[component patterns]", "ProjectWide")
```

## Document Update Process

1. Identify which docs need updates based on requirements
2. Update each doc with new requirements
3. Cross-check consistency across all docs
4. Validate no placeholders remain
5. Store key decisions in MCP memory
