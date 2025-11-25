# Workspace Instructions v2.0

## Repository Structure

This repository uses a streamlined AI-enhanced development workflow with 7 specialized agents.

### Core Directories

**`.gaia/`** - AI Framework Core
- `agents/` - 7 specialized agent specifications (Explorer, Architect, Builder, Tester, Reviewer, Deployer, Documenter)
- `designs/` - Progressive design docs (start with README, add only when needed)
- `mcps/` - Core MCP server with 4 essential tools
- `tasks.jsonl` - Task tracking in JSONL format
- `memory.jsonl` - Decision and context storage
- `prompts/gaia.prompt.md` - Orchestrator instructions
- `instructions/` - Project and workflow guidance

**`.claude/`** - Claude Configuration
- `hooks/` - Event handlers with audio feedback
- `settings.json` - Claude project settings

**`.github/`** - CI/CD & Automation
- `workflows/` - GitHub Actions pipelines
- `state/` - Session tracking

**`.vscode/`** - Development Environment
- `mcp.json` - MCP server configuration
- `settings.json` - Code formatting rules
- `extensions.json` - Recommended extensions

**`src/`** - Source Code
- `backend/` - API and business logic
- `frontend/` - UI applications

### Spec-Driven Workflow (MANDATORY)

1. **EXPLORE** - Use @Explorer to understand the codebase
2. **DESIGN** - Update `.gaia/designs/` specs BEFORE coding
3. **ARCHITECT** - Use @Architect for new design decisions
4. **BUILD** - Use @Builder to implement FROM SPECS
5. **TEST** - Use @Tester for validation
6. **REVIEW** - Use @Reviewer for quality checks
7. **DEPLOY** - Use @Deployer for git operations
8. **DOCUMENT** - Use @Documenter to keep docs current

**CRITICAL**: Never skip step 2. All features require design updates first.

### Agent Coordination

Since agents cannot call other agents, the main AI instance coordinates workflows:
```markdown
1. AI calls: @Explorer to analyze code
2. AI calls: @Builder to implement feature
3. AI calls: @Tester to validate
4. AI passes context between agents
```

See `.gaia/prompts/gaia.prompt.md` for orchestration instructions.

### MCP Tools (Just 4) - USE THESE EXCLUSIVELY

**IMPORTANT**: Use ONLY these MCP tools for ALL task and memory management:
1. `read_tasks(hideCompleted?)` - Get current tasks from JSONL
2. `update_task(taskId, description, status, assignedTo?)` - Update task status
3. `remember(category, key, value)` - Store important decisions
4. `recall(query, maxResults?)` - Search previous decisions with fuzzy matching

**DO NOT (CRITICAL)**:
- Create TODO.md, TASKS.md, or any markdown files for task tracking
- Use Write/Edit tools to create task lists in files
- Track tasks anywhere except via MCP tools
- Create memory/decision files outside of MCP tools
- **NEVER directly edit .gaia/tasks.jsonl or .gaia/memory.jsonl files**
- **NEVER use Read/Write/Edit tools on JSONL files**

All task and memory management MUST go through the GAIA MCP server.
The MCP server handles file creation, corruption recovery, and all JSONL operations.

### Progressive Design

Start simple, add complexity only when needed:
- **Prototype**: Basic README only
- **MVP**: Add API docs and basic tests
- **Production**: Add architecture and security docs
- **Enterprise**: Full design documentation

### Key Principles

- **Main AI as orchestrator** - Agents cannot call each other, main AI coordinates
- **JSONL storage** - Git tracks everything via MCP tools
- **Model optimization** - Haiku for simple tasks (10x cheaper)
- **Parallel execution** - Run independent agents simultaneously
- **Minimal documentation** - Only what developers actually use
- **MCP tools only** - No markdown files for tasks/memories
