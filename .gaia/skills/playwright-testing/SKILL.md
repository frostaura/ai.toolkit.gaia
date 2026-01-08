---
name: playwright-testing
description: Guide for using Playwright MCP tools for visual and functional regression testing. Use when testing features, validating UI, or running regression checks.
---

# Playwright Testing (via MCP Tools)

Use Playwright MCP tools directly for testing - **NOT** npm/npx commands.

## Testing Types

### Functional Regression (Manual Testing)

Interactively verify features work using Playwright MCP tools:

```
0. Spin up the entire stack via Docker Compose. Ensure no cache is used for front and backend projects. Database projects should not be cleared and retain data where possible.
1. Navigate to page/component
2. Interact with elements (click, type, select)
3. Verify expected behavior
4. Check for console errors
5. Test error states and edge cases
```

### Visual Regression (Screenshots)

Capture and compare screenshots at all breakpoints:
| Breakpoint | Width | Use Case |
|------------|-------|----------|
| Mobile | 320px | Small phones |
| Tablet | 768px | Tablets/iPad |
| Desktop | 1024px | Laptops |
| Large | 1440px+ | Desktop monitors |

### What to Test

- **All pages** at every breakpoint
- **Interactive states**: default, hover, focus, active, disabled, loading, error
- **User workflows**: Complete user journeys end-to-end
- **Data integrity**: Forms save correctly, displays show right data
- **Error handling**: Invalid inputs, network failures, edge cases

## Console Error Monitoring

Monitor browser console during all testing - flag ANY errors.

## Functional Test Checklist

For each affected feature:

- [ ] Navigation works
- [ ] Forms submit correctly
- [ ] Data displays properly
- [ ] Buttons/links respond
- [ ] Error states show correctly
- [ ] No console errors

## Visual Test Checklist

- [ ] Screenshot at 320px
- [ ] Screenshot at 768px
- [ ] Screenshot at 1024px
- [ ] Screenshot at 1440px
- [ ] Compare with baseline
- [ ] Flag unintended changes

## Key Rules

- ✅ Use Playwright **MCP tools** directly
- ✅ Test interactively (manual regression)
- ❌ Do NOT run `npx playwright test`
- ❌ Do NOT create separate test spec files for regression
