---
name: quality-gate-validation
description: Guide for running quality gates with tiered coverage requirements based on task complexity. Gates are binary pass/fail with zero warnings allowed. Uses 3-attempt retry before marking blocked.
---

# Quality Gate Validation

## Gate Commands

| Gate | Frontend | Backend |
|------|----------|---------|
| **Build** | `npm run build` | `dotnet build --no-incremental` |
| **Lint** | `npm run lint` | `dotnet format --verify-no-changes` |
| **Test** | `npm test` | `dotnet test` |
| **Coverage** | `npm test -- --coverage` | `dotnet test --collect:"XPlat Code Coverage"` |
| **Validate** | `npm run validate` | Build + Format combined |

## Tiered Requirements

| Complexity | Coverage Target | Gates Required |
|------------|-----------------|----------------|
| Trivial | Manual verify | None |
| Simple | 50% touched code | Build + Lint |
| Standard | 70% touched code | Build + Lint + Test |
| Complex | 80% all code | All + E2E |
| Enterprise | 90%+ all code | All + Security + Perf |

## Execution Rules

- All gates are **binary**: exit 0 = pass, else = fail
- **Zero warnings** allowed (strict linting)
- Run in order: Build → Lint → Test → Coverage

## Retry Strategy

```
Attempt 1: Fix identified issue → re-run
Attempt 2: Simplify approach → re-run
Attempt 3: Escalate for architectural review
Still failing: Mark BLOCKED, document reason
```

After blocking:
```javascript
remember("gate", "blocked_[feature]", 
  "[Gate] failed: [reason]. Tried: [approaches]",
  "ProjectWide")
```

## Functional Testing (Playwright)

Use MCP tools directly - NOT npm/npx or spec files.

For each feature:
1. Navigate to page
2. Interact with elements
3. Verify behavior
4. Check console errors
5. Test edge cases

## Visual Testing

| Breakpoint | Width |
|------------|-------|
| Mobile | 320px |
| Tablet | 768px |
| Desktop | 1024px |
| Large | 1440px+ |

## Response Formats

### Passed
```markdown
✅ QUALITY GATES PASSED

Build: ✅ | Lint: ✅ | Tests: ✅ 47/47 | Coverage: 82%

→ Ready for deployment
```

### Failed
```markdown
❌ GATE FAILED: [Gate]

Error: [Issue]
Location: [file:line]

→ @Developer: [Fix instruction]
```

## Best Practices

- ✅ Run gates in order
- ✅ Use severity levels appropriately
- ✅ Provide actionable feedback
- ❌ Skip gates for "small" changes
- ❌ Approve with critical issues
