# Workspace Instructions v2.0

## Repository Structure

This repository uses a streamlined AI-enhanced development workflow with 6 specialized agents.

### Core Directories

**`.gaia/`** - AI Framework Core
- `agents/` - 6 specialized agent specifications (Explorer, Architect, Builder, Tester, Reviewer, Deployer, Documenter)
- `designs/` - Adaptive design documents (grow with project maturity)
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

### Streamlined Workflow

1. **EXPLORE** - Use @Explorer to understand the codebase
2. **ARCHITECT** - Use @Architect for design decisions (only as needed)
3. **BUILD** - Use @Builder to implement features
4. **TEST** - @Builder delegates to @Tester for validation
5. **REVIEW** - Use @Reviewer for quality checks
6. **DEPLOY** - Use @Deployer for git operations and deployments
7. **DOCUMENT** - Use @Documenter to keep docs current

### Agent Coordination

Since agents cannot call other agents, the main AI instance coordinates workflows:
```markdown
1. AI calls: @Explorer to analyze code
2. AI calls: @Builder to implement feature
3. AI calls: @Tester to validate
4. AI passes context between agents
```

See `.gaia/prompts/gaia.prompt.md` for orchestration instructions.

### MCP Tools (Just 4)

1. `read_tasks` - Get current tasks from JSONL
2. `update_task` - Update task status
3. `remember` - Store important decisions
4. `recall` - Search previous decisions

### Progressive Design

Start simple, add complexity only when needed:
- **Prototype**: Basic README only
- **MVP**: Add API docs and basic tests
- **Production**: Add architecture and security docs
- **Enterprise**: Full design documentation

### Key Principles

- **No orchestrator** - Agents communicate directly
- **JSONL storage** - Git tracks everything
- **Model optimization** - Haiku for simple tasks (10x cheaper)
- **Parallel execution** - Run independent tasks simultaneously
- **Minimal documentation** - Only what developers actually use
