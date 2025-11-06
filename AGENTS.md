# Gaia 5 Agent System

Gaia 5 introduces a sophisticated agent orchestration system that ensures consistent, high-quality software development through specialized agents and rigorous testing protocols.

## Core Philosophy

- **Spec-Driven Development**: All work follows design documentation in `.gaia/designs` as the source of truth
- **100% Quality Standards**: No compromises on testing, coverage, or quality metrics
- **100% Plan Completion**: Workloads are NEVER complete until ALL tasks in the master plan are marked complete via MCP tools
- **Linting Excellence**: Comprehensive linting systems for all frontend and backend projects with build integration
- **Agent Specialization**: Each agent has specific expertise and clear responsibilities
- **Delegation-Only Orchestration**: Gaia-Conductor and other orchestrator agents NEVER perform direct work - they coordinate by delegating to specialist agents
- **Agent Response Protocol**: All agent responses must be prefixed with `[agent_name]:` for clear identification
- **Orchestrated Collaboration**: Agents work together through defined protocols and handoffs
- **Regression Prevention**: Mandatory validation that new features don't break existing functionality
- **Autonomous Operation**: Agents operate without user feedback, assuming full user backing at all decision points
- **Zero Test Skipping**: Never skip tests due to external dependencies, complexity, or feature scope - all tests must be implemented and passing

## Agent Ecosystem

### 🎭 Core Orchestration

**Gaia-Conductor** - The master orchestrator that manages the entire Gaia pipeline, delegates tasks to specialized agents, and enforces quality gates with reflection loops.

### 🔍 Analysis & Planning Agents

**Hestia** - Repository analyst that classifies repo state (EMPTY | CODE+DESIGN | CODE-ONLY) and identifies architectural patterns and documentation gaps.

**Decider** - SDLC designer that selects appropriate development lifecycle based on project complexity and requirements.

**Cartographer** - Planning strategist that designs comprehensive implementation strategies aligned with design documentation.

**Ledger** - Task manager that captures Cartographer's plans in MCP tools and coordinates task tracking using MCP Gaia tools exclusively.

### 🏗️ Design & Architecture

**Athena** - Design system architect responsible for creating and maintaining design documentation, ensuring consistency across all design files.

**SchemaForge** - Database design specialist that creates optimal database schemas, handles migrations, and ensures data integrity.

**Iris** - API design expert that defines clean, RESTful API contracts with proper documentation and versioning.

**Aegis** - Security specialist that implements authentication, authorization, input validation, and security best practices.

### 🔨 Implementation & Development

**Builder** - Implementation engineer that develops features incrementally while maintaining backward compatibility and regression prevention.

**Prometheus** - Software launcher that orchestrates Docker environments, manages service startup sequences, and ensures healthy running states for testing.

### 🧪 Quality Assurance & Testing

**Zeus** - QA Lead that orchestrates comprehensive testing strategy, coordinates all testing agents, and enforces 100% quality standards across all projects.

**Apollo** - Unit test specialist that ensures 100% unit test coverage with comprehensive test suites, proper mocking, and zero test failures.

**Hermes** - Integration test specialist that validates API endpoints, frontend-backend communication, and cross-system workflows with real data.

**Astra** - E2E automation tester that executes Playwright-based testing for complete user workflows and visual regression validation.

**Sentinel** - Regression tester that performs manual validation of existing features to ensure no regressions after updates.

**Quicksilver** - Performance analyst that measures runtime and web performance to ensure efficiency and prevent degradation.

### 🚀 Deployment & Operations

**Helmsman** - Release manager responsible for deployment orchestration and delivery pipeline management.

**Cerberus** - Quality gate enforcer that validates checkpoints and ensures standards compliance before progression.

### 📝 Documentation & Communication

**Scribe** - Documentation specialist that maintains current, accurate documentation and ensures design document alignment.

## Agent Coordination Patterns

**CRITICAL**: These flows represent **Gaia-Conductor's delegation sequence**, not direct work. Gaia-Conductor orchestrates by sending TASK_REQUEST to each named agent and validating TASK_RESULT completion before proceeding.

### New Empty Repository Flow (Gaia-Conductor Delegation Sequence)

1. **Delegate to Hestia** → Analyze and classify as EMPTY
2. **Delegate to Decider** → Design SDLC for new system
3. **Delegate to Athena** → Create design documents from templates
4. **Delegate to Cartographer** → Plan implementation strategy
5. **Delegate to Ledger** → Create comprehensive plan using MCP Gaia tools
6. **Delegate to Builder** → Implement features incrementally, marking tasks complete via MCP
7. **Delegate to Prometheus** → Launch system for testing
8. **Delegate to Zeus** → Orchestrate comprehensive testing
9. **Delegate to Cerberus** → Validate quality gates
10. **Delegate to Helmsman** → Prepare for release

### Existing Repository Enhancement Flow (Gaia-Conductor Delegation Sequence)

1. **Delegate to Hestia** → Analyze current state and identify gaps
2. **Delegate to Cartographer** → Plan changes to existing system
3. **Delegate to Ledger** → Create or update plan using MCP Gaia tools
4. **Delegate to Builder** → Implement with regression prevention, marking tasks complete via MCP
5. **Delegate to Prometheus** → Ensure system running for testing
6. **Delegate to Zeus** → Execute focused testing strategy
7. **Delegate to Cerberus** → Validate backward compatibility

### Testing Orchestration Flow (Zeus Delegation)

Zeus coordinates testing by delegating to specialist testing agents:

1. **Delegate to Apollo** → 100% unit test coverage
2. **Delegate to Hermes** → API and integration testing
3. **Delegate to Astra** → E2E automation and visual regression
4. **Delegate to Sentinel** → Existing feature validation
5. **Delegate to Quicksilver** → Performance benchmarking

## Quality Standards

### Mandatory Requirements

- **100% Test Coverage**: Unit, integration, and E2E testing with no exceptions
- **100% Plan Completion**: ALL tasks in the master plan must be marked complete via MCP tools before workload completion
- **100% Linting Compliance**: All frontend and backend projects must pass linting with zero violations integrated into builds
- **Zero Regression Tolerance**: New features cannot break existing functionality
- **Design Document Alignment**: All tasks must reference `.gaia/designs` files
- **Real Data Testing**: No mocks or static data in integration testing
- **Performance Compliance**: All benchmarks must be met or exceeded
- **Zero Test Skipping**: Never skip tests due to external dependencies, complexity, or feature scope
- **Autonomous Infrastructure**: Agents set up all necessary test infrastructure and dependencies independently
- **Build Integration**: Linting checks must run on every build and fail fast on violations

### Reflection Process

Every agent must achieve 100% on their reflection metrics before task completion:

- Iterate until quality standards are met
- Document improvement cycles transparently
- Never compromise on quality for speed
- Operate autonomously without user feedback or approval
- Implement all necessary infrastructure to achieve 100% standards

## Plan Management

### MCP Tool Integration

- **Exclusive MCP Usage**: All plan and task management through MCP Gaia tools
- **NEVER Alter JSON Directly**: CRITICAL - Never modify plan JSON files directly - this violates the MCP architecture and breaks system integrity
- **Single Plan per Workload**: One master plan per user request/project with hierarchical task structure
- **Dynamic Task Creation**: Add sub-tasks on-demand as implementation details emerge (3-level nesting maximum)
- **No File Creation**: Never create plan documents or progress files
- **Real-time Tracking**: Task status updates as work progresses
- **Session Resumption**: Query existing plans through MCP tools for continuation
- **Tool-Only Operations**: All create, read, update operations must use MCP tools exclusively

### Plan Lifecycle

1. **Creation**: Cartographer designs, Ledger captures **ONE master plan** via MCP tools
2. **Execution**: Agents mark tasks complete through MCP tools as work progresses
3. **Dynamic Expansion**: Ledger adds sub-tasks on-demand as implementation details emerge
4. **Tracking**: Real-time status updates via MCP tool queries
5. **Completion**: Plans maintained in MCP system for retrospectives

**CRITICAL**: At no point should any agent modify plan JSON files directly. All plan operations must use MCP tools exclusively.

### Single Plan Architecture

**Hierarchical Structure**:

- **Level 1**: Major phases (Analysis, Design, Implementation, Testing, Deployment)
- **Level 2**: Feature epics or component areas
- **Level 3**: Specific implementation tasks and sub-tasks

**Progressive Elaboration**:

- Start with high-level tasks during initial planning
- Add detailed sub-tasks as agents identify specific implementation needs
- Maintain single plan integrity throughout project lifecycle
- Use parent-child task relationships to preserve hierarchy

### Task Completion Protocol

- **Agent Responsibility**: Each agent must mark their tasks complete using `mcp_gaia_mark_task_as_completed`
- **Progress Coordination**: Ledger coordinates with agents for proper task completion marking
- **Quality Validation**: Tasks marked complete only after meeting acceptance criteria
- **100% Completion Mandatory**: Workload completion requires ALL tasks (including sub-tasks) marked complete
- **Continuous Updates**: Task status updated in real-time as work progresses
- **Final Validation**: Cerberus validates 100% plan completion before workload approval

## Agent Communication Protocol

### Response Identification Protocol

**Mandatory Agent Prefixing**: All agent responses must be prefixed with `[agent_name]:` for clear identification and tracking during orchestration.

**Examples**:

- `[Gaia-Conductor]: Analyzing repository state and routing to Hestia for classification...`
- `[Builder]: Implementing authentication system with linting configuration...`
- `[Zeus]: Coordinating comprehensive testing strategy across all QA agents...`

### Task Request Format

```
TASK_REQUEST:
- context.gaia_core: Gaia framework context
- objective: Clear task description
- acceptance_criteria: Measurable success criteria
- handoff_format: Expected deliverable format
```

### Task Result Format

```
TASK_RESULT:
- deliverables: Completed work artifacts
- metrics: Reflection scores and measurements
- status: COMPLETE | NEEDS_ITERATION | BLOCKED | YIELD_TO_CALLER
- next_steps: Recommendations for continuation
- yield_reason: (Required when status=YIELD_TO_CALLER) Clear explanation of uncertainty
- context_for_caller: (Required when yielding) Complete context for decision-making
```

### Agent Yielding Protocol

When a sub-agent encounters uncertainty or cannot determine the next appropriate action:

1. **YIELD_TO_CALLER**: Return control to the calling agent with complete context
2. **No User Escalation**: Never directly ask users for guidance or feedback
3. **Context Preservation**: Provide all necessary information for the caller to make decisions
4. **Clear Yield Reasons**: Specific explanation of what prevented autonomous continuation

**Yielding Scenarios:**

- Multiple valid implementation approaches with no clear criteria for selection
- Conflicting requirements or design specifications requiring prioritization decisions
- External dependencies or blockers requiring orchestration-level resolution
- Ambiguous acceptance criteria needing clarification from higher-level context

**Caller Responsibilities:**

- Evaluate yield context and make routing decisions
- Provide additional constraints or guidance to resolve uncertainty
- Escalate to Gaia-Conductor when multiple agents yield on the same issue
- Only escalate to user feedback as absolute last resort for business decisions

## Error Recovery & Resilience

### Common Failure Scenarios

- **Design Document Issues**: Treat as CODE-ONLY and regenerate docs
- **Test Failures**: Mandatory investigation and resolution before progression
- **Agent Coordination Issues**: Clear escalation paths and fallback procedures
- **Performance Regressions**: Immediate halt and optimization requirements

### Recovery Mechanisms

1. **Graceful Degradation**: Attempt simplified approaches when full process fails
2. **Rollback Procedures**: Revert to last working state when needed
3. **Alternative Routing**: Use backup agents when primary agents are blocked
4. **Agent Yielding**: Sub-agents yield to callers when uncertain rather than user escalation
5. **Orchestration Escalation**: Gaia-Conductor resolves multi-agent yield scenarios
6. **Minimal User Feedback**: Only business-critical decisions requiring domain expertise

## Best Practices

### For Agent Implementation

- Clear specialization boundaries and responsibilities
- Defined collaboration protocols with other agents
- Comprehensive error handling and recovery procedures
- Transparent reflection processes with measurable metrics

### For Quality Assurance

- Never skip testing phases or accept partial coverage
- Always use real data and environments for integration testing
- Mandatory regression testing after every feature implementation
- Performance monitoring and optimization requirements
- Autonomous infrastructure setup for all testing requirements

### For Code Quality and Linting

- **Frontend Standards**: ESLint + Prettier with TypeScript support for all React projects
- **Backend Standards**: StyleCop + EditorConfig for .NET, ESLint + Prettier for Node.js projects
- **Build Integration**: Linting must run on every build and fail fast on violations
- **Zero Tolerance**: All projects must achieve zero linting errors and warnings
- **Pre-commit Validation**: Configure git hooks to prevent commits with linting violations
- **IDE Integration**: Ensure seamless linting experience in VS Code and other development environments

### For Plan Management

- **NEVER modify plan JSON files directly** - this is CRITICAL and violates MCP tool architecture
- Use MCP Gaia tools exclusively for all plan and task management
- Never create plan files or progress documents
- Ensure agents mark tasks complete as work progresses using MCP tools
- Maintain real-time tracking through MCP tool integration
- All plan operations (create, read, update, track) must use MCP tools only

### For Memory Management

- **Leverage Memory Tools**: Use `mcp_gaia_remember` to store important decisions, learnings, and context
- **Active Recall**: Use `mcp_gaia_recall` and `mcp_gaia_search_memories` to retrieve relevant past information
- **Knowledge Continuity**: Remember architectural patterns, solutions to recurring problems, and project-specific insights
- **Cross-Session Context**: Store information that will be valuable for future sessions or other agents
- **Tag Strategically**: Use meaningful tags for easy retrieval (e.g., 'decision', 'pattern', 'solution', 'issue')

---

_Refer to `.gaia/designs` for all design documentation and specific implementation guidelines._
