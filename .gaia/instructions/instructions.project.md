# Workspace Instructions

## Repository Structure

This repository follows AI-enhanced software development principles with strict organization:

### Core Directories

**`.gaia/`** - AI Framework Intelligence
- `designs/` - System design documents (11 comprehensive templates: use-cases, class, sequence, frontend, api, security, infrastructure, data, observability, scalability, testing)
- `instructions/` - Copilot instructions for rules enforcement
- `agents/` - Specialized agent specifications
- `mcps/` - MCP server implementations

**`.github/`** - CI/CD & State Management
- `workflows/` - Automated build/test/deployment
- `state/` - AI session and progress tracking

**`.vscode/`** - Development Environment
- `mcp.json` - MCP server configuration
- `settings.json` - Code formatting/linting
- `extensions.json` - Recommended extensions

**`src/`** - Source Code (mandatory location)
- `backend/` - API and business logic (iDesign: Managers → Engines → Data Access)
- `frontend/` - UI applications (React/TypeScript)

### Project Files

- `docker-compose.yml` - Multi-service orchestration
- `Dockerfile.*` - Service-specific containers
- `README.md` - Project documentation
- `.gitignore` - Version control exclusions

### Design-First Development

All projects must follow the spec-driven workflow:
1. **ANALYZE** - Repository state assessment
2. **DESIGN** - Complete all 11 design templates in `.gaia/designs/`
3. **PLAN** - Task breakdown via MCP tools
4. **IMPLEMENT** - Code against design specs

**Never skip design phase**. All implementation must reference design documents.
