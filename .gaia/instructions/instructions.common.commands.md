# Common Repository Commands
## Creating Symlinks
This captures commands leveraged to create symlinks to shared files in order to reuse agent and instructions definitions.

### Share Instructions
- `ln -s .gaia/instructions/instructions.agents.md AGENTS.md`
- `git add AGENTS.md`
- `ln -s ../.gaia/instructions/instructions.project.md .github/copilot-instructions.md`
- `git add .github/copilot-instructions.md`
- `ln -s .gaia/instructions/instructions.project.md CLAUDE.md`
- `git add CLAUDE.md`

### MCP Configuration
- `ln -s ../.gaia/mcp-config.json .github/mcp-config.json`
- `git add .github/mcp-config.json`
- `ln -s .gaia/mcp-config.json .mcp.json`
- `git add .github/mcp-config.json`
- `ln -s ../.gaia/mcp-config.json .vscode/mcp.json`
- `git add .vscode/mcp-config.json`
- `ln -s /Users/deanmartin/Desktop/Projects/Experimentation/ai.toolkit.gaia/.github/mcp-config.json ~/.copilot/mcp-config.json`

### Share Agents
- `ln -s ../.gaia/agents .github/agents`
- `ln -s ../.gaia/agents .claude/agents`

### Prompts & Instructions
- `ln -s ../.gaia/prompts .github/prompts`
- `ln -s ../.gaia/instructions .github/instructions`

## Launching Github Copilot CLI

The below is an example of how to run Github Copilot CLI in YOLO mode and with the local MCP config.
