---
name: web-research
description: Web research using fetch_webpage and Playwright MCP tools in headless mode
---

# Web Research Skill

Two-tier approach: **fetch_webpage first** → Playwright MCP tools (headless) as fallback.

> See **`.gaia/skills/reflection.md`** for post-research reflection process.

## Tier 1: fetch_webpage (Primary)

Use for known URLs (docs, blogs, GitHub, APIs). Fast and efficient.

```typescript
fetch_webpage({
  urls: ["https://react.dev/learn"],
  query: "React hooks best practices",
});
```

**Limitations**: No JS rendering, no searches, no interactions.

## Tier 2: Playwright MCP Tools (Fallback - Headless Mode)

Use for: web searches, dynamic content, JS-rendered pages, interactions.

> **IMPORTANT**: Playwright runs in **headless mode** for research. No visible browser window.

**Search Pattern**:

1. Navigate: `mcp_playwright-mc_browser_navigate({ url: "https://duckduckgo.com" })`
2. Snapshot: `mcp_playwright-mc_browser_snapshot()` - get element refs
3. Type: `mcp_playwright-mc_browser_type({ element: "search", ref: "[ref]", text: "query" })`
4. Submit: `mcp_playwright-mc_browser_key({ element: "search", ref: "[ref]", key: "Enter" })`
5. Wait: `mcp_playwright-mc_browser_wait_for({ text: "results" })`
6. Extract: `mcp_playwright-mc_browser_snapshot()` - parse results

**Key Tools**: navigate, snapshot, click, type, key, wait_for, screenshot, evaluate

## Quality Standards

- ✅ Minimum 3 sources
- ✅ Prioritize official docs
- ✅ Include versions & dates
- ✅ `recall()` before research
- ✅ `remember()` key findings
- ✅ Reflect after completion (see `.gaia/skills/reflection.md`)

## Output Format

```markdown
✓ Research: [Topic]
**[Answer]** (v[X], [Date])

- Key finding 1
- Key finding 2
  **Source**: [URL]
  **Method**: fetch_webpage / Playwright headless
```

## Post-Research Reflection

After completing research:

```
remember("research", "[topic]", "[key findings + sources]", "ProjectWide")
remember("source", "[topic]", "[best URLs found]", "ProjectWide")
remember("pattern", "research_[approach]", "[what worked well]", "ProjectWide")
```

## Critical Rules

- ✅ Try fetch_webpage FIRST
- ✅ Playwright runs headless (no GUI)
- ✅ Cite sources with URLs
- ✅ Cache findings via `remember()`
- ✅ Reflect on research quality/efficiency
- ❌ Never skip fetch_webpage
- ❌ Never present unverified info
- ❌ Never skip post-research reflection
