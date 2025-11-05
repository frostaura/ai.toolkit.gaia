# Gaia 5 Agent System

Gaia 5 introduces a sophisticated agent orchestration system that ensures consistent, high-quality software development through specialized agents and rigorous testing protocols.

## Core Philosophy

- **Spec-Driven Development**: All work follows design documentation in `.gaia/designs` as the source of truth
- **100% Quality Standards**: No compromises on testing, coverage, or quality metrics
- **Agent Specialization**: Each agent has specific expertise and clear responsibilities
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

### New Empty Repository Flow

1. **Hestia** → Analyze and classify as EMPTY
2. **Decider** → Design SDLC for new system
3. **Athena** → Create design documents from templates
4. **Cartographer** → Plan implementation strategy
5. **Ledger** → Create comprehensive plan using MCP Gaia tools
6. **Builder** → Implement features incrementally, marking tasks complete via MCP
7. **Prometheus** → Launch system for testing
8. **Zeus** → Orchestrate comprehensive testing
9. **Cerberus** → Validate quality gates
10. **Helmsman** → Prepare for release

### Existing Repository Enhancement Flow

1. **Hestia** → Analyze current state and identify gaps
2. **Cartographer** → Plan changes to existing system
3. **Ledger** → Create or update plan using MCP Gaia tools
4. **Builder** → Implement with regression prevention, marking tasks complete via MCP
5. **Prometheus** → Ensure system running for testing
6. **Zeus** → Execute focused testing strategy
7. **Cerberus** → Validate backward compatibility

### Testing Orchestration Flow (Zeus Coordination)

1. **Apollo** → 100% unit test coverage
2. **Hermes** → API and integration testing
3. **Astra** → E2E automation and visual regression
4. **Sentinel** → Existing feature validation
5. **Quicksilver** → Performance benchmarking

## Quality Standards

### Mandatory Requirements

- **100% Test Coverage**: Unit, integration, and E2E testing with no exceptions
- **Zero Regression Tolerance**: New features cannot break existing functionality
- **Design Document Alignment**: All tasks must reference `.gaia/designs` files
- **Real Data Testing**: No mocks or static data in integration testing
- **Performance Compliance**: All benchmarks must be met or exceeded
- **Zero Test Skipping**: Never skip tests due to external dependencies, complexity, or feature scope
- **Autonomous Infrastructure**: Agents set up all necessary test infrastructure and dependencies independently

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
- **No File Creation**: Never create plan documents or progress files
- **Real-time Tracking**: Task status updates as work progresses
- **Session Resumption**: Query existing plans through MCP tools for continuation

### Plan Lifecycle

1. **Creation**: Cartographer designs, Ledger captures via MCP tools
2. **Execution**: Agents mark tasks complete through MCP tools as work progresses
3. **Tracking**: Real-time status updates via MCP tool queries
4. **Completion**: Plans maintained in MCP system for retrospectives

### Task Completion Protocol

- **Agent Responsibility**: Each agent must mark their tasks complete using `mcp_gaia_mark_task_as_completed`
- **Progress Coordination**: Ledger coordinates with agents for proper task completion marking
- **Quality Validation**: Tasks marked complete only after meeting acceptance criteria
- **Continuous Updates**: Task status updated in real-time as work progresses

## Agent Communication Protocol

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

### For Plan Management

- Use MCP Gaia tools exclusively for all plan and task management
- Never create plan files or progress documents
- Ensure agents mark tasks complete as work progresses using MCP tools
- Maintain real-time task tracking through MCP tool integration

---

_Refer to `.gaia/designs` for all design documentation and specific implementation guidelines._
