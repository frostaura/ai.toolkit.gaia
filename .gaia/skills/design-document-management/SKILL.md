---
name: design-document-management
description: Guide for creating design documents on-demand based on task complexity. Documents are created when needed, not as empty templates upfront. Used primarily by @Planner for Standard+ complexity tasks.
---

# Design Document Management

## Design Documents (On-Demand Creation)

| Document | Purpose | Create When |
|----------|---------|-------------|
| `design.md` | Use cases + architecture combined | Standard+ complexity |
| `api.md` | API contracts and endpoints | API changes |
| `data.md` | Database schema + models | Database changes |
| `security.md` | Auth + access control | Security-related changes |

## Complexity-Based Requirements

| Complexity | Design Docs Needed |
|------------|-------------------|
| Trivial | None |
| Simple | None |
| Standard | design.md |
| Complex | design.md + relevant docs |
| Enterprise | Full set as needed |

## Quality Rules

- ✅ No `[TODO]` or `[TBD]` placeholders in active docs
- ✅ Consistent terminology across documents
- ✅ Every requirement maps to a design section
- ✅ Follow Clean Architecture / iDesign principles
- ❌ Don't create empty templates upfront

## Document Templates

### design.md Structure

```markdown
# [Project/Feature] Design

## Overview
Brief description of what this covers.

## Use Cases

### UC-1: [Name]
**Actor**: [Who]
**Goal**: [What they want]
**Flow**:
1. Step 1
2. Step 2
**Error cases**: [What can go wrong]

## Architecture

### Components
- Component A: [Purpose]
- Component B: [Purpose]

### Data Flow
[Description or diagram]

## Decisions

### D-1: [Decision]
**Options**: A, B, C
**Chosen**: B
**Rationale**: [Why]
```

### api.md Structure

```markdown
# API Design

## Base URL
`/api/v1`

## Endpoints

### POST /auth/login
**Request:**
```json
{ "email": "...", "password": "..." }
```

**Response:** 200
```json
{ "token": "...", "expiresIn": 900 }
```

**Errors:** 400, 401, 429
```

## Completion Checklist

Before implementation:
- [ ] Required docs for complexity exist
- [ ] No placeholders remain
- [ ] Designs capture requirements
- [ ] Terminology consistent

## Store Decisions

```javascript
remember("decision", "architecture", "[key decisions]", "ProjectWide")
remember("decision", "api_design", "[endpoint patterns]", "ProjectWide")
```
