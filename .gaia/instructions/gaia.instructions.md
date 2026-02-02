# Gaia 5 - Complete AI Toolkit Planning & Execution System

## System Overview

Gaia 5 is a comprehensive AI-driven development system that enforces quality through objective quality gates, spec-driven development, and regression prevention.

> 🤖 **AUTONOMOUS EXECUTION MANDATE**
>
> Gaia 5 agents operate **autonomously without user intervention**. Agents MUST:
>
> - **ACT, don't ask** - Execute tasks directly instead of asking for permission
> - **DECIDE, don't suggest** - Make decisions and implement them immediately
> - **PROCEED, don't pause** - Continue through all phases without waiting for approval
> - **FIX, don't report** - When issues arise, fix them autonomously; only report blockers after 3 failed attempts
>
> The only acceptable reasons to pause and ask the user:
>
> 1. Genuine ambiguity in requirements that cannot be reasonably inferred
> 2. A task is BLOCKED after 3 fix attempts (report what was tried)
> 3. User explicitly requested a review checkpoint
>
> **Default behavior**: Full autonomous execution from request to completion.

## Core Architecture

### 8 Specialized Agents

1. **@Explorer** (haiku) - Repository analysis and code discovery
2. **@Architect** (sonnet) - Design decisions and system architecture
3. **@Builder** (sonnet) - Implementation and development
4. **@Tester** (haiku) - Testing with Playwright directly (no custom scripts)
5. **@Reviewer** (haiku) - Code quality and security review
6. **@Researcher** (opus) - Web research, product analysis, documentation discovery
7. **@Deployer** (haiku) - Git operations and deployments
8. **@Documenter** (haiku) - Documentation maintenance

### MCP Tools (MANDATORY - Never Create Markdown Files)

- `mcp__gaia__read_tasks(hideCompleted?)` - View tasks
- `mcp__gaia__update_task(taskId, description, status, assignedTo?)` - Manage tasks
- `mcp__gaia__remember(category, key, value, duration)` - Store decisions, learnings, and resolutions (SessionLength or ProjectWide)
- `mcp__gaia__recall(query, maxResults?)` - Search memories with fuzzy matching (aggregates session + persistent)

### 🧠 Continuous Memory Usage (MANDATORY)

> **THE MEMORY MANDATE**: Agents MUST actively use `remember()` and `recall()` throughout execution!
>
> See **`.gaia/skills/mcp-memory-management.md`** for detailed usage patterns.

**Core Rules**:

- `recall()` BEFORE every task - check for past knowledge (searches both session + persistent)
- `remember()` AFTER every fix - document solutions (use ProjectWide for permanent knowledge)
- `remember()` AFTER every failed attempt - prevent repeating mistakes (use ProjectWide)
- **Choose duration**: SessionLength (default, temporary) or ProjectWide (permanent, survives restarts)

### Design Documents (Always in `.gaia/designs/`)

- `use-cases.md` - Use cases, user flows, API/UI journeys
- `architecture.md` - System design and components
- `api.md` - API endpoints and contracts
- `database.md` - Schema and data models
- `security.md` - Authentication and authorization
- `frontend.md` - UI/UX patterns and components

> See **`.gaia/skills/design-document-management.md`** for document requirements by SDLC tier.

### Skills Documentation

- `reflection.md` - Systematic post-task reflection for continuous learning
- `web-research.md` - Web research using fetch_webpage and Playwright MCP tools
- `playwright-testing.md` - Visual and functional regression testing guide
- `mcp-memory-management.md` - Memory usage patterns for recall/remember
- `work-breakdown-structure.md` - Hierarchical WBS planning guide
- `sdlc-tier-selection.md` - SDLC tier selection criteria
- `design-document-management.md` - Design document requirements
- `quality-gate-validation.md` - Quality gate execution guide
- `visual-excellence.md` - Visual quality and user flow guide
- `default-tech-stack.md` - Default technology stack details
- `strict-linting.md` - **MANDATORY** strict linting configuration (zero warnings, build fails on violations)

## Orchestrator Execution

Orchestrator-specific workflow and execution rules live in `.gaia/agents/gaia.md`.

## Default Technology Stack

> See **`.gaia/skills/default-tech-stack.md`** for full stack details.

### Summary

- **Backend**: ASP.NET Core (.NET 8+), EF Core, Clean Architecture
- **Frontend**: React 18+ with TypeScript 5+, Redux Toolkit
- **Database**: PostgreSQL 15+, Redis caching
- **Security**: JWT (15min access, 7day refresh), RBAC
- **Testing**: Playwright MCP tools, 100% coverage

### Testing

- **Framework**: Playwright MCP tools (direct usage ONLY, no npm/npx scripts)
- **Unit Coverage**: 100% (frontend + backend)
- **Visual Testing**: Screenshot comparison at all breakpoints
- **Functional Regression**: Interactive manual testing via Playwright MCP
- **E2E**: All user journeys

> See **`.gaia/skills/playwright-testing.md`** for detailed testing guide.

## Visual Excellence Requirements

> See **`.gaia/skills/visual-excellence.md`** for detailed visual quality and user flow guide.

### Mandatory Quality Checks

- ✅ All pages professionally styled
- ✅ All viewports tested (320px, 768px, 1024px, 1440px+)
- ✅ All interactive states (default, hover, focus, active, disabled, loading, error)
- ✅ All user flows covered (happy path, error path, edge cases)
- ✅ WCAG 2.1 AA accessibility

### Playwright Visual Testing

- Direct commands only
- Screenshot every major component
- Compare across all breakpoints
- Test all state variations
- Monitor console for errors

## The Gaia 5 Promise

**"Quality through validation, success through design, excellence through gates and memory"**

This single document contains everything needed to execute Gaia 5. No external files required.
