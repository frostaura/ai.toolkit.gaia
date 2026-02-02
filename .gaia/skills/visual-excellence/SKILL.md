---
name: visual-excellence
description: Visual quality standards for professional UI. Test all pages at 4 breakpoints, all 7 interactive states, and complete user flows. Enforce WCAG 2.1 AA accessibility.
---

# Visual Excellence

## Quality Checks

- ✅ All pages professionally styled
- ✅ All viewports tested (320px, 768px, 1024px, 1440px+)
- ✅ All interactive states tested
- ✅ No template artifacts or placeholders
- ✅ WCAG 2.1 AA accessibility

## Interactive States

Test ALL for every interactive element:

| State | Description |
|-------|-------------|
| Default | Normal appearance |
| Hover | Mouse over |
| Focus | Keyboard focus |
| Active | Being clicked |
| Disabled | Not interactive |
| Loading | Async operation |
| Error | Validation/error |

## User Flow Coverage

Every feature needs:
1. **Happy path**: Standard success
2. **Error path**: Validation failures
3. **Edge cases**: Empty states, limits
4. **Interruptions**: Network loss, back button

## Accessibility (WCAG 2.1 AA)

- Keyboard navigable (logical tab order)
- Screen reader compatible (ARIA labels)
- Color contrast ≥ 4.5:1
- Visible focus indicators
- Alt text for images

## Playwright Visual Testing

1. Screenshot at each breakpoint
2. Capture all interactive states
3. Compare with baseline
4. Test complete user flows
5. Monitor console for errors
