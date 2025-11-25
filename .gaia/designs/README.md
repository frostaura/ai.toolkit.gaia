# Spec-Driven Development

**MANDATORY**: All features MUST have updated design specs BEFORE implementation.

## The Iron Rule
1. **DESIGN FIRST** - Update relevant design docs
2. **REVIEW** - Ensure specs are complete
3. **IMPLEMENT** - Build according to specs
4. **NEVER** skip step 1

## Available Design Documents

### Core Specs (Always Maintain)
- `architecture.md` - System design and components
- `api.md` - API endpoints and contracts
- `database.md` - Schema and data models
- `security.md` - Authentication and authorization
- `frontend.md` - UI/UX patterns and components

### Before ANY Implementation
Ask yourself:
- Which design docs does this feature touch?
- Have I updated ALL relevant sections?
- Are the specs detailed enough to code from?
- Would another developer understand the design?

## Design Stages

### 1. Prototype (Start Here)
Just build it. No design docs needed. README only.

### 2. MVP
When you have users, add:
- `api.md` - Endpoint documentation (if applicable)
- `architecture.md` - High-level system overview (1 page max)

### 3. Production
When scaling matters, add:
- `database.md` - Schema and relationships (if applicable)
- `security.md` - Authentication and authorization approach
- `deployment.md` - How to deploy and monitor

### 4. Enterprise
Only when absolutely required:
- `scalability.md` - Performance requirements and solutions
- `testing.md` - Test strategy and coverage goals
- Additional domain-specific docs

## Guidelines

- **Don't pre-create templates** - Add them when needed
- **Keep it simple** - 1-2 pages per document maximum
- **Use MCP tools** - Track design decisions with `remember()`
- **Update as you go** - Docs reflect current state, not future dreams

## Current Project Stage

Track the current stage in MCP:
```
mcp__gaia__remember("project", "stage", "prototype")
```

Then query it when needed:
```
mcp__gaia__recall("project stage")
```