---
name: web-research
description: Two-tier web research using fetch_webpage as primary for known URLs and Playwright MCP (headless) as fallback for searches. Requires 3+ sources, prioritizes official docs, includes versions/dates.
---

# Web Research

Two-tier approach: **fetch_webpage first** → Playwright MCP (headless) fallback.

## Tier 1: fetch_webpage (Primary)

Fast and efficient for known URLs:

```javascript
fetch_webpage({
  urls: ["https://react.dev/learn"],
  query: "React hooks best practices"
})
```

**Best for**: Official docs, blogs, GitHub, API references
**Limitations**: No JS rendering, no searches

## Tier 2: Playwright MCP (Fallback)

Use for searches and dynamic content:

```javascript
browser_navigate({ url: "https://duckduckgo.com" })
browser_snapshot()  // Get element refs
browser_type({ ref: "[ref]", text: "query" })
browser_press_key({ key: "Enter" })
browser_wait_for({ text: "results" })
browser_snapshot()  // Extract results
```

**Note**: Runs **headless** (no visible browser).

## Quality Standards

- ✅ Minimum 3 sources for claims
- ✅ Prioritize official documentation
- ✅ Include version numbers
- ✅ Include publication dates
- ✅ Cache findings via `remember()`

## Output Format

```markdown
✓ Research: [Topic]

**Recommendation**: [Choice] (v[X.Y], [Date])

Key findings:
- Finding 1
- Finding 2

Sources: [URLs]
```

## Memory Integration

```javascript
// Before research
recall("[topic]")

// After research
remember("research", "[topic]", "[findings + sources]", "ProjectWide")
```

## Rules

- ✅ Try fetch_webpage first
- ✅ Cite sources with URLs
- ✅ Cache key findings
- ❌ Don't skip to Playwright for known URLs
- ❌ Don't present unverified info
