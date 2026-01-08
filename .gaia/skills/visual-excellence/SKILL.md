---
name: visual-excellence
description: Visual quality standards, user flows, and use case coverage requirements. Use when building UI, reviewing frontend, or validating user experiences.
---

# Visual Excellence Requirements

## Mandatory Quality Checks

- ✅ All pages professionally styled
- ✅ All viewports tested (320px, 768px, 1024px, 1440px+)
- ✅ All interactive states tested
- ✅ No template artifacts or placeholders
- ✅ Smooth responsive transitions
- ✅ WCAG 2.1 AA accessibility

## Interactive States

Test ALL of these for every interactive element:
| State | Description |
|-------|-------------|
| Default | Normal appearance |
| Hover | Mouse over |
| Focus | Keyboard focus |
| Active | Being clicked/pressed |
| Disabled | Not interactive |
| Loading | Async operation in progress |
| Error | Validation/error state |

## User Flow Coverage

Every feature must have complete user flows documented and tested:

1. **Happy Path**: Standard successful completion
2. **Error Path**: Validation failures, API errors
3. **Edge Cases**: Empty states, max limits, special characters
4. **Interruptions**: Network loss, session timeout, back button

## Use Case Testing Matrix

| Use Case | Entry Point         | Expected Outcome | Error Handling  |
| -------- | ------------------- | ---------------- | --------------- |
| [Action] | [Where user starts] | [Success state]  | [Failure state] |

## Accessibility (WCAG 2.1 AA)

- Keyboard navigable (tab order logical)
- Screen reader compatible (ARIA labels)
- Color contrast ratio ≥ 4.5:1
- Focus indicators visible
- Alt text for images

## Visual Testing with Playwright MCP

1. Screenshot at each breakpoint (320, 768, 1024, 1440px)
2. Capture all interactive states
3. Compare with baseline
4. Test complete user flows
5. Monitor console for errors
