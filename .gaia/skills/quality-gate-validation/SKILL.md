---
name: quality-gate-validation
description: Guide for running and validating quality gates (build, lint, test, coverage, regression). Use when validating code, running tests, or checking if a phase can proceed.
---

# Quality Gate Validation

## Gate Commands

| Gate         | .NET                                          | Node.js                                     |
| ------------ | --------------------------------------------- | ------------------------------------------- |
| **Build**    | `dotnet build --no-incremental`               | `npm run build`                             |
| **Lint**     | `dotnet format --verify-no-changes --severity error` | `npm run lint` (must use `--max-warnings 0`) |
| **Test**     | `dotnet test`                                 | `npm test` / `npx playwright test`          |
| **Coverage** | `dotnet test --collect:"XPlat Code Coverage"` | `npm test -- --coverage`                    |
| **Validate** | `dotnet build && dotnet format --verify-no-changes` | `npm run validate` (typecheck + lint + format) |

> **STRICT LINTING REQUIRED**: See **`.gaia/skills/strict-linting/SKILL.md`** for mandatory configuration.
> - **Frontend**: ESLint with `--max-warnings 0`, TypeScript strict mode, Prettier format check
> - **Backend**: `TreatWarningsAsErrors=true`, StyleCop, Roslynator, SonarAnalyzer, .NET Analyzers

## Coverage Requirement

**100% code coverage** required for both frontend and backend. No exceptions.

## Validation Rule

All gates are **binary pass/fail** - exit code 0 = pass, anything else = fail.

## Retry Strategy

```
Attempt 1: Fix the identified issue → re-run gate
Attempt 2: Simplify the approach → re-run gate
Attempt 3: Suggest architectural refactor to retain functionality but simplify testability of the code -> get user approval -> implement the changes → re-run gate
Attempt 4: Reduce scope (remove problematic feature) → re-run gate
Still failing: Mark task as "blocked", store reason, continue with other tasks
```

## Phase 7 Validation Checklist

After each feature, ALL must pass:

1. **Test Suite**: Unit + Integration + E2E pass (exit 0)
2. **Coverage**: 100% code coverage (frontend + backend)
3. **Functional Regression**: Manually test features with Playwright (not spec files)
4. **Visual Regression**: Playwright screenshots match baseline
5. **Performance**: Response times within 5% of baseline
6. **User Journeys**: All existing workflows functional

## Functional Regression (Manual Testing)

Use Playwright **interactively** to verify features work - this is NOT writing spec files:

- Navigate to each affected page/component
- Test all user interactions (clicks, forms, navigation)
- Verify data displays correctly
- Check error states and edge cases
- Confirm no console errors

## If Validation Fails

1. **STOP** development immediately
2. Root cause analysis
3. Fix or redesign approach
4. Re-validate until 100% pass
5. Store: `remember("regression", "feature_x_issue", "[details]")`

## Store Gate Results

```bash
# On success
mcp__gaia__remember("gate", "phase_X", "passed", "ProjectWide")

# On blocked (after 3 attempts)
mcp__gaia__remember("gate", "phase_X_blocked", "[reason and what was tried]", "ProjectWide")
```

## Gate Summary

| Gate       | Validates            | Failure Action             |
| ---------- | -------------------- | -------------------------- |
| Build      | Compilation + Lint   | Fix syntax/type/lint errors |
| Lint       | Code style (STRICT)  | Fix violations (zero warnings allowed) |
| Test       | Functionality        | Debug failing tests        |
| Coverage   | 100% coverage        | Add missing tests          |
| Functional | Features work        | Fix broken functionality   |
| Regression | No breakage          | Investigate root cause     |

## Strict Linting Mandate

**All projects MUST configure strict linting that fails builds:**

### Frontend Requirements
- ESLint with `--max-warnings 0`
- TypeScript `strict: true` with all strict flags
- Prettier format checking in CI
- No `any` types allowed
- No unused variables/imports

### Backend Requirements  
- `TreatWarningsAsErrors=true` in Directory.Build.props
- StyleCop.Analyzers package
- Roslynator.Analyzers package
- SonarAnalyzer.CSharp package
- `dotnet format --verify-no-changes --severity error` in CI

> See **`.gaia/skills/strict-linting/SKILL.md`** for complete configuration files.
