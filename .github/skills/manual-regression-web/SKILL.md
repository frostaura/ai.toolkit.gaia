# SKILL: Manual Regression (Web via Playwright MCP)

## Goal
Manually validate key flows with Playwright MCP tools **without** dumping large logs.

## Steps
1) Use Playwright MCP to navigate the key use-case flows
2) Validate:
   - success path
   - one meaningful failure/edge path
3) Record only the label `playwright-mcp` in MCP proof args

## Done when
- web flows are manually validated for the changed use-cases
