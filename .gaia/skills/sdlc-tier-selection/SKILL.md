---
name: sdlc-tier-selection
description: Guide for selecting the appropriate SDLC tier based on project complexity. Use when planning features, assessing scope, or starting new work.
---

# SDLC Tier Selection

## Repository State (Check First)

| State                | Action                                           |
| -------------------- | ------------------------------------------------ |
| Empty (no `src/`)    | Start fresh with full design templates           |
| Has code + designs   | Update existing designs, maintain compatibility  |
| Has code, no designs | Analyze codebase, generate designs, then proceed |

## SDLC Tiers

> ⚠️ Time estimates are **HUMAN hours** for complexity assessment only. Never refuse scope based on time.

| Tier           | Scope                        | Design Docs Required                    |
| -------------- | ---------------------------- | --------------------------------------- |
| **Micro**      | Bug fixes (<1 day)           | use-cases.md (if needed)                |
| **Small**      | Single feature (1-3 days)    | use-cases.md + architecture.md + api.md |
| **Medium**     | Multiple features (3-7 days) | + database.md                           |
| **Large**      | Major changes (1-2 weeks)    | All 6 docs                              |
| **Enterprise** | Full system (2+ weeks)       | All 6 docs                              |

## Phase Sequences

**Micro**: Requirements → Design Update (if needed) → Implementation → Testing

**Small**: Requirements → Design → Implementation → Testing → Deployment

**Medium**: Requirements → System Design → Documentation → Implementation → QA → Deployment

**Large**: Requirements → Architecture → Detailed Design → Documentation → Development → Testing → Quality Gates → Deployment

**Enterprise**: Discovery → System Architecture → Detailed Design → Compliance → Phased Development → Comprehensive Testing → Quality Gates → Infrastructure → Deployment → Post-Release

## Design Documents (in `.gaia/designs/`)

- `use-cases.md` - Use cases, user flows, API/UI journeys
- `architecture.md` - System design and components
- `api.md` - API endpoints and contracts
- `database.md` - Schema and data models
- `security.md` - Authentication and authorization
- `frontend.md` - UI/UX patterns and components

## After Selection

```bash
mcp__gaia__remember("sdlc", "type", "[micro/small/medium/large/enterprise]", "ProjectWide")
mcp__gaia__remember("sdlc", "phases", "[phase list]", "ProjectWide")
```
