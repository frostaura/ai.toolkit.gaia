---
name: playwright-testing
description: Guide for using Playwright MCP tools directly for visual and functional testing. Test all pages at 4 breakpoints, all interactive states, and monitor console for errors. Use MCP tools only - NOT npm/npx commands or spec files.
---

# Playwright Testing

Use Playwright MCP tools directly - **NOT** npm/npx commands or spec files.

## Functional Testing (Interactive)

For each affected feature:
1. Navigate to page/component
2. Interact with elements (click, type, select)
3. Verify expected behavior
4. Check for console errors
5. Test error states and edge cases

## Visual Testing (Screenshots)

| Breakpoint | Width | Purpose |
|------------|-------|---------|
| Mobile | 320px | Small phones |
| Tablet | 768px | Tablets/iPad |
| Desktop | 1024px | Laptops |
| Large | 1440px+ | Monitors |

## Interactive States

Test ALL for interactive elements:

| State | Description |
|-------|-------------|
| Default | Normal appearance |
| Hover | Mouse over |
| Focus | Keyboard focus |
| Active | Being clicked |
| Disabled | Not interactive |
| Loading | Async operation |
| Error | Validation/error |

## Test Checklist

For each feature:
- [ ] Navigation works
- [ ] Forms submit correctly
- [ ] Data displays properly
- [ ] Buttons/links respond
- [ ] Error states show correctly
- [ ] No console errors

## Visual Checklist

- [ ] Screenshot at 320px
- [ ] Screenshot at 768px
- [ ] Screenshot at 1024px
- [ ] Screenshot at 1440px
- [ ] Compare with baseline

## Key Rules

- ✅ Use Playwright **MCP tools** directly
- ✅ Test interactively (manual regression)
- ✅ Check console for errors
- ❌ Do NOT run `npx playwright test`
- ❌ Do NOT create spec files for regression
