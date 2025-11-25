# Gaia 5 - Complete AI Toolkit Planning & Execution System

## System Overview

Gaia 5 is a comprehensive AI-driven development system that enforces quality through reflection metrics, spec-driven development, and regression prevention.

## Core Architecture

### 7 Specialized Agents
1. **@Explorer** (haiku) - Repository analysis and code discovery
2. **@Architect** (sonnet) - Design decisions and system architecture
3. **@Builder** (sonnet) - Implementation and development
4. **@Tester** (haiku) - Testing with Playwright directly (no custom scripts)
5. **@Reviewer** (haiku) - Code quality and security review
6. **@Deployer** (haiku) - Git operations and deployments
7. **@Documenter** (haiku) - Documentation maintenance

### MCP Tools (MANDATORY - Never Create Markdown Files)
- `mcp__gaia__read_tasks(hideCompleted?)` - View tasks
- `mcp__gaia__update_task(taskId, description, status, assignedTo?)` - Manage tasks
- `mcp__gaia__remember(category, key, value)` - Store decisions
- `mcp__gaia__recall(query, maxResults?)` - Search memories with fuzzy matching

### Design Documents (Always in `.gaia/designs/`)
- `architecture.md` - System design and components
- `api.md` - API endpoints and contracts
- `database.md` - Schema and data models
- `security.md` - Authentication and authorization
- `frontend.md` - UI/UX patterns and components

## The Gaia 5 Process (7 Mandatory Phases)

### Phase 1: Requirements Gathering
**Agent**: @Explorer + Main AI

**Actions**:
1. Comprehensively analyze user request
2. Examine existing system with @Explorer
3. Identify gaps and enhancement areas
4. Store: `mcp__gaia__remember("requirements", "user_request", "[details]")`

**Reflection Metrics** (ALL must reach 100%):
- **Clarity**: User intent unambiguous, success criteria defined, constraints identified
- **Efficiency**: Minimal viable scope identified, quick wins prioritized, resources realistic
- **Quality**: Functional/non-functional requirements complete, acceptance criteria measurable
- **Frontend Visual Requirements**: Layout captured, breakpoints defined, interactions specified
- **Comprehensiveness**: All stories covered, edge cases considered, integration mapped

**Reflection Process**: Score each metric 0-100%, improve until all reach 100% (max 3 attempts)

### Phase 2: Repository Assessment & SDLC Selection
**Agent**: @Explorer + Main AI

**Repository State Determination**:
1. **Empty Repository** (No `src/` directory):
   - Start from scratch with full design templates

2. **Has Code + Design Docs** (`src/` exists + `.gaia/designs/*.md` filled):
   - Update existing designs, maintain compatibility

3. **Has Code, No Design Docs** (`src/` exists, no designs):
   - Analyze codebase, generate designs, then proceed

**SDLC Selection** (Choose minimal viable):

**Micro SDLC** (Bug fixes, <1 day):
```yaml
Requirements → Design Update (if needed) → Implementation → Testing
Reflection Metrics per phase: 100% required
```

**Small SDLC** (Single feature, 1-3 days):
```yaml
Requirements → Design → Implementation → Testing → Deployment
Reflection Metrics per phase: 100% required
```

**Medium SDLC** (Multiple features, 3-7 days):
```yaml
Requirements → System Design → Documentation → Implementation → QA → Deployment
Reflection Metrics per phase: 100% required
```

**Large SDLC** (Major changes, 1-2 weeks):
```yaml
Requirements → Architecture → Detailed Design → Documentation → Development → Testing → Quality Gates → Deployment
Reflection Metrics per phase: 100% required
```

**Enterprise SDLC** (Full system, 2+ weeks):
```yaml
Discovery → System Architecture → Detailed Design → Compliance → Phased Development → Comprehensive Testing → Quality Gates → Infrastructure → Deployment → Post-Release
Reflection Metrics per phase: 100% required
```

**Store Selection**:
```
mcp__gaia__remember("sdlc", "type", "[micro/small/medium/large/enterprise]")
mcp__gaia__remember("sdlc", "phases", "[phase list]")
```

**Reflection Metrics**:
- **Pipeline Quality**: Appropriate for project size, phases ordered correctly
- **Design Principles Adherence**: Follows Gaia framework, spec-driven approach

### Phase 3: Execute Design Steps (MANDATORY BEFORE ANY TASKS!)
**Agent**: @Architect + @Documenter

**CRITICAL RULE**: Complete ALL design work BEFORE creating implementation tasks!

**Actions**:
1. For each design document in `.gaia/designs/`:
   - Update with new requirements
   - Ensure consistency across all docs
   - Validate completeness

2. Design Completion Checkpoint:
   - ✅ All 5 design docs complete
   - ✅ No template placeholders remain
   - ✅ Designs capture 100% requirements
   - ✅ Inter-document consistency verified

**Store Decisions**:
```
mcp__gaia__remember("design", "architecture", "[key decisions]")
mcp__gaia__remember("design", "api", "[endpoint designs]")
mcp__gaia__remember("design", "database", "[schema decisions]")
```

**Reflection Metrics** (100% Required):
- **Design Completeness**: All docs updated, specs detailed enough to code from
- **Template Adherence**: Follows formats, sections filled, consistent terminology
- **Document Alignment**: No contradictions, shared concepts consistent
- **Requirements Coverage**: Every requirement mapped, no missing features

### Phase 4: Planning (Based on COMPLETED Design)
**Agent**: Main AI

**Generate Comprehensive Plan**:
1. Create tasks FROM completed design docs
2. Each task MUST reference specific design sections
3. Include measurable acceptance criteria
4. Ensure proper sequencing

**Task Structure Example**:
```
Task: Implement JWT authentication
References: security.md#jwt-tokens, api.md#auth-endpoints
Acceptance Criteria:
- JWT with 15min access, 7day refresh per security.md
- All endpoints from api.md functional
- Tests achieve 100% coverage
Assignee: @Builder
```

**Reflection Metrics**:
- **Comprehensiveness**: All design elements have tasks, no gaps
- **Design Alignment**: Each task references designs, follows architecture
- **Frontend Plan Quality**: Components, state, routing, PWA requirements
- **Backend Plan Quality**: APIs, business logic, database operations
- **Test Coverage Plan**: Unit, integration, E2E, visual regression

### Phase 5: Capture Plan in MCP Tools
**Agent**: Main AI

**Actions** (Use MCP exclusively):
```
mcp__gaia__update_task("auth-1", "Implement JWT per security.md#jwt", "pending", "Builder")
mcp__gaia__update_task("auth-2", "Create login endpoints per api.md#login", "pending", "Builder")
mcp__gaia__update_task("auth-test", "Test auth with Playwright", "pending", "Tester")
```

**NEVER**: Create TODO.md, TASKS.md, or any markdown task files!

**Reflection Metrics**:
- **Task Capture Completeness**: All planned tasks in MCP
- **Design-Task Alignment**: Every task has design references

### Phase 6: Incremental Plan Execution
**Agents**: @Builder, @Tester, @Reviewer (orchestrated)

**For Each Task**:

**Before Starting**:
- Identify potentially impacted features
- Review relevant design sections
- Set up for regression testing

**During Implementation**:
- @Builder implements per design specs
- Frequent testing with Playwright
- Incremental commits
- Update task status: `mcp__gaia__update_task("[id]", "...", "in_progress", "Builder")`

**After Completion**:
- @Tester validates with Playwright
- @Reviewer checks quality
- Update: `mcp__gaia__update_task("[id]", "...", "completed", "Builder")`

### Phase 7: Feature Compatibility Validation (MANDATORY AFTER EACH FEATURE!)
**Agents**: @Tester + @Reviewer

**Validation Requirements** (ALL must pass 100%):

1. **Full Test Suite** (Playwright directly):
   - All existing unit tests pass
   - All integration tests pass
   - All E2E tests pass
   - Console error monitoring active

2. **Visual Regression** (Playwright screenshots):
   - Capture all pages at all breakpoints
   - Compare with baseline
   - Flag ANY unintended changes

3. **Performance Validation**:
   - Response times within 5% of baseline
   - Memory usage stable
   - No new bottlenecks

4. **User Journey Testing** (Playwright commands):
   - All existing workflows functional
   - Edge cases still handled
   - Data integrity maintained

**If Validation Fails**:
- **STOP** all development immediately
- Root cause analysis
- Fix or redesign approach
- Re-validate until 100% pass
- Store: `mcp__gaia__remember("regression", "feature_x_issue", "[details]")`

**Reflection Metrics** (100% Required):
- **Regression Test Pass Rate**: All tests passing
- **Feature Functionality Score**: Previous features work
- **Visual Consistency Score**: No unintended UI changes
- **Performance Impact Score**: ≤5% degradation

## Reflection Scoring Guide

**For EVERY Phase**:
1. Execute the phase completely
2. Score each metric 0-100%:
   - 0-25%: Critical failures
   - 26-50%: Significant issues
   - 51-75%: Needs improvement
   - 76-99%: Good but incomplete
   - 100%: Perfect, ready to proceed
3. WHILE any metric <100%:
   - Identify gaps
   - Improve output
   - Re-score
4. After 3 attempts if <100%: Flag for review but proceed

**Store Results**:
```
mcp__gaia__remember("reflection", "phase_X", "all_100_percent")
mcp__gaia__remember("reflection", "phase_X_attempts", "2")
```

## Error Handling & Recovery

### Design Issues
- **Malformed/Missing**: Create fresh from templates
- **Conflicts**: Use most recent, flag inconsistencies

### User Request Issues
- **Ambiguity**: List conflicts, request clarification
- **Scope Creep**: Confirm expanded scope

### SDLC Failures
- **Invalid Steps**: Fall back to: Requirements → Design → Implementation → Testing
- **Stuck Reflection**: After 3 attempts, flag and proceed

### Regression Failures
- **Test Failures**: Halt, investigate root cause
- **Breaking Changes**: Implement compatibility layer
- **Performance Issues**: Optimize or redesign

### Recovery Mechanisms
1. Try full process
2. If fails, simplify phase
3. If still fails, skip non-critical
4. Create tasks for skipped items
5. Always maintain compatibility

## Default Technology Stack

### Backend
- **Framework**: ASP.NET Core (.NET 8+)
- **ORM**: Entity Framework Core
- **Architecture**: Clean Architecture
- **Linting**: StyleCop + .NET Analyzers

### Frontend
- **Framework**: React 18+ with TypeScript 5+
- **State**: Redux Toolkit + RTK Query
- **UI Components**: ReactBits (via MCP) → ChakraUI → Ant Design
- **PWA**: MANDATORY (Service Workers, IndexedDB, Offline-first)
- **Linting**: ESLint + Prettier

### Database
- **Primary**: PostgreSQL 15+
- **ORM**: Entity Framework Core with migrations
- **Caching**: Redis

### Security
- **Authentication**: JWT (15min access, 7day refresh)
- **Storage**: httpOnly cookies preferred
- **Admin Account**: admin@system.local / Admin123! (dev only)
- **RBAC**: Role-based access control

### Testing
- **Framework**: Playwright (direct usage ONLY, no custom scripts)
- **Unit Coverage**: ≥80% business logic
- **Visual Testing**: Screenshot comparison at all breakpoints
- **E2E**: All user journeys

## Visual Excellence Requirements

### Mandatory Quality Checks
- ✅ All pages professionally styled
- ✅ All viewports tested (320px, 768px, 1024px, 1440px+)
- ✅ All interactive states (default, hover, focus, active, disabled, loading, error)
- ✅ No template artifacts or placeholders
- ✅ Smooth responsive transitions
- ✅ WCAG 2.1 AA accessibility

### Playwright Visual Testing
- Direct commands only
- Screenshot every major component
- Compare across all breakpoints
- Test all state variations
- Monitor console for errors

## Critical Success Rules

### MUST DO
- ✅ Complete ALL design work BEFORE creating tasks
- ✅ Every task MUST reference design documents
- ✅ Use MCP tools EXCLUSIVELY for tasks/memories
- ✅ Run compatibility validation after EACH feature
- ✅ Achieve 100% on ALL reflection metrics
- ✅ Use Playwright directly for ALL testing
- ✅ Maintain backward compatibility ALWAYS

### NEVER DO
- ❌ Create tasks before design completion
- ❌ Skip regression testing
- ❌ Create TODO.md or any markdown task files
- ❌ Modify .gaia/tasks.jsonl or memory.jsonl directly
- ❌ Build custom components when libraries exist
- ❌ Create separate test scripts
- ❌ Proceed with <100% on critical metrics

## Quality Benchmarks (ALL 100% Required)

### Requirements Quality
- All functional requirements explicitly defined
- Non-functional requirements with concrete targets
- Edge cases and error conditions documented
- Dependencies and integration points mapped

### Design Quality
- All 5 design documents complete
- Architecture follows Clean/iDesign principles
- Database properly normalized
- API contracts with examples
- UI/UX includes responsive and accessibility

### Implementation Quality
- Builds without warnings
- Unit tests ≥80% coverage
- Integration tests for all APIs
- Code passes all linting
- Documentation current

### Regression Prevention Quality
- All existing tests pass
- Previous features verified working
- No visual regressions detected
- Performance maintained
- Backward compatibility preserved

## Success Criteria

A Gaia 5 execution succeeds when:
- All design documents completed before implementation
- Every task explicitly references design specifications
- All tests pass at 100% rate
- Zero regressions introduced
- Visual quality achieves excellence
- Performance maintained or improved
- All reflection metrics achieve 100%
- Results stored in MCP for tracking

## The Gaia 5 Promise

**"Quality through reflection, success through design, excellence through validation"**

This single document contains everything needed to execute Gaia 5. No external files required.