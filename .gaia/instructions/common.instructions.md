---
applyTo: "**"
---
<!-- Custom instructions that should be applied to any prompt (all directories, files and file types). -->

# Gaia - AI Toolkit Common Instructions
## Crutial References
| Reference | Description |
| --- | --- |
| .gaia/designs/*.md | A collection of important design documentation for the solution, together with overview for the design system, expected repository structure and Docker setup. |
| .gaia/examples/ | Integration testing examples and templates demonstrating console error monitoring, beauty analysis, and comprehensive Playwright test setup. |

## Instructions
### Planning and Task Management
- Whenever you're unsure about what to do next, instead of just stopping or asking the user, use your plan.
- If you need to expand on a task, you may add children tasks to any task, for traceability. **Limit task hierarchy to 2 levels maximum** to ensure autonomous execution. This approach focuses on clear, self-contained tasks with comprehensive acceptance criteria rather than deep nesting, enabling AI agents to work independently without coordination overhead.
- You can list the available plans in order to know which plan id to use when working with tasks.
- Complete the plan list one-item-at-a-time, sequentially.
- After every good milestone, show plan execution progress. Brief and interesting numbers.
- If you get lost on which task you're on, you can always refer to the plans.
- Update your current task status as completed after completing each task, before moving onto the next task.
- **MANDATORY**: After completing each feature implementation task, execute regression validation before proceeding to the next feature.

### Continuous Integration Testing Workflow (ENHANCED - MANDATORY)
**CRITICAL**: This workflow ensures ZERO regressions and maintains system integrity throughout development.

#### After Every Task Completion:
1. **Immediate Integration Test**: Before marking any task complete
   - Run COMPLETE test suite: `npm test && npx playwright test --reporter=line`
   - Validate ALL existing features still function perfectly
   - Capture screenshots of ALL components and analyze for visual regression
   - Monitor browser console for ANY errors/warnings during testing

2. **Complete System Validation**: Before starting next task  
   - **MANDATORY**: Execute end-to-end testing of ENTIRE application
   - **MANDATORY**: Test ALL user journeys from start to finish
   - **MANDATORY**: Validate ALL interactive elements across ALL viewport sizes
   - **MANDATORY**: Ensure browser console remains error-free throughout entire test suite

3. **Beauty & Function Analysis**: For every UI component
   - Score visual quality 0-100% using enhanced beauty criteria
   - Validate responsive behavior across mobile/tablet/desktop
   - Test ALL interactive states (hover, focus, active, disabled, loading, error)
   - Ensure professional polish with zero placeholder content or debug elements

#### Issue Discovery & Resolution Workflow:
1. **Issue Detection**: When any test fails or visual quality scores below 100%
   - **HALT**: Stop all development work immediately
   - **ANALYZE**: Identify root cause and impact scope
   - **DOCUMENT**: Record issue details and affected components

2. **Fix Implementation**: Address discovered issues
   - Apply minimal changes to resolve the specific issue
   - Avoid introducing new changes during bug fixing
   - Focus on surgical fixes that don't impact other functionality

3. **Retest Everything**: After each fix
   - **MANDATORY**: Re-run COMPLETE test suite before proceeding
   - **MANDATORY**: Validate ALL previously working functionality still works
   - **MANDATORY**: Confirm the fix doesn't introduce new regressions
   - **MANDATORY**: Repeat until 100% pass rate achieved across all tests

4. **Only Then Proceed**: Move to next task only after achieving
   - 100% test pass rate
   - 100% visual quality scores
   - Zero console errors or warnings
   - Complete end-to-end functionality validation

### Regression Prevention Workflow (NEW - MANDATORY)
**Execute this workflow after EVERY feature implementation AND before starting any new task:**

1. **Pre-Validation**: Capture baseline state before testing
   - Run complete test suite: `npm test` (or `dotnet test` for .NET)
   - Record test results and performance metrics
   - Capture visual snapshots if UI changes were made
   - Monitor and record console error baseline

2. **Compatibility Testing**: Validate existing functionality
   - Execute all automated tests for existing features
   - Manually test critical user journeys that could be affected
   - Verify API endpoints maintain backward compatibility
   - Validate all previous features still function correctly

3. **Regression Detection**: Identify any breaks in existing functionality
   - Compare test results with baseline
   - Check for performance degradation (>5% slowdown triggers investigation)
   - Review visual regression test results for unintended UI changes
   - Validate zero new console errors or JavaScript exceptions

4. **Complete Integration Validation (ENHANCED)**: Before proceeding to next task
   - **MANDATORY**: Re-run COMPLETE test suite and achieve 100% pass rate
   - **MANDATORY**: Test ALL previously implemented features end-to-end
   - **MANDATORY**: Capture screenshots of ALL pages/components and validate visual quality scores remain 100%
   - **MANDATORY**: Monitor browser console during ALL tests and ensure zero errors/warnings
   - **MANDATORY**: Validate complete user journeys across the entire application

5. **Resolution**: Fix any detected regressions before proceeding
   - If regressions found, halt feature development immediately
   - Investigate root cause and implement compatibility fix
   - Re-run full validation until 100% pass rate achieved
   - Document fixes and update preventive measures

6. **Documentation**: Record validation results
   - Update task with regression test results
   - Document any compatibility issues discovered and resolved
   - Note any preventive measures implemented for future development
   - Log console error patterns and resolutions

### Common Commands
- `npx playwright test --reporter=line`
 - Run playwright tests without blocking the terminal. Always headless and **never** `--reporter=html`
- `npx playwright test --reporter=line --project=chromium --grep="visual"`
 - Run visual regression tests specifically
- `npx playwright test --reporter=line --headed --project=chromium`
 - Run tests in headed mode for debugging visual issues
- `npx playwright test --reporter=line --trace=on`
 - Run tests with trace collection for detailed debugging

#### Regression Testing Commands (NEW - MANDATORY)
**Feature Compatibility Validation**:
- `npm test` or `dotnet test` - Run complete test suite for all existing features
- `npx playwright test --reporter=line --grep="regression"` - Run regression-specific tests
- `npm run test:coverage` - Verify test coverage remains above 80% after new feature implementation
- `npm run build && npm run test:e2e` - Full build and end-to-end testing pipeline

**Performance Regression Detection**:
- `npm run test:performance` - Run performance benchmarks (if configured)
- `lighthouse --output=json --output-path=./lighthouse-report.json http://localhost:3000` - Performance audit
- Compare performance metrics before/after feature implementation

**Visual Regression Validation**:
- `npx playwright test --reporter=line --project=chromium --grep="visual"` - Run all visual regression tests
- Capture screenshots before feature work: `mkdir -p screenshots/baseline && npx playwright test --update-snapshots`
- Compare screenshots after feature work: `npx playwright test --project=chromium --grep="visual"`

**Console Error Detection (NEW - MANDATORY)**:
- `npx playwright test --reporter=line --grep="console-errors"` - Run console error detection tests
- Browser console monitoring: Enable console error capture in ALL Playwright tests
- JavaScript exception tracking: Configure automatic test failure on unhandled JS exceptions
- Network error monitoring: Track and report failed HTTP requests during test execution
- Warning validation: Log and analyze console warnings for potential issues

#### Visual Testing Commands
**Screenshot Analysis Commands**:
- Take screenshots at multiple viewports: Use `page.setViewportSize()` with mobile (375px), tablet (768px), desktop (1024px+)
- Critical analysis command pattern: `await page.screenshot({ path: 'analysis-[pagename]-[viewport].png', fullPage: true })`
- Interactive state testing: Capture screenshots for hover, focus, loading, error states using `page.hover()`, `page.focus()`, etc.

**Console Error Monitoring Commands (NEW - MANDATORY)**:
- Capture console errors: `page.on('console', msg => { if (msg.type() === 'error') console.log('Console Error:', msg.text()) })`
- Monitor network failures: `page.on('response', response => { if (!response.ok()) console.log('Network Error:', response.url(), response.status()) })`
- Track JavaScript exceptions: `page.on('pageerror', exception => console.log('JS Exception:', exception.message))`
- Log console warnings: `page.on('console', msg => { if (msg.type() === 'warning') console.log('Console Warning:', msg.text()) })`
- MANDATORY: Fail tests on ANY console errors or unhandled JavaScript exceptions during test execution

**Beauty Analysis Criteria Commands (ENHANCED)**:
- **Visual Hierarchy Analysis**: Verify clear information hierarchy, logical content flow, and appropriate element sizing
- **Spacing & Alignment Validation**: Confirm consistent spacing (8px, 16px, 24px grid), perfect alignment, no cramped/overlapping elements
- **Typography Excellence**: Validate font sizes (min 14px for body text), proper line-height (1.4-1.6), adequate contrast ratios (4.5:1 minimum)
- **Color & Brand Consistency**: Ensure consistent color palette, sufficient contrast, professional color combinations, no jarring color conflicts
- **Component State Perfection**: Test and validate ALL interactive states with proper visual feedback and smooth transitions
- **Responsive Behavior Excellence**: Validate smooth breakpoint transitions, no horizontal scrolling, proper mobile touch targets (44px minimum)
- **Professional Polish Standards**: Zero unstyled components, no placeholder content, no debug elements, perfect loading states

**Template Cleanup Validation Commands**:
- Check for placeholder content: `grep -r "Lorem ipsum\|Welcome to\|placeholder\|TODO\|console\.log" src/`
- Verify no debug statements: `grep -r "console\.\|debugger\|alert(" src/`
- Validate no unused imports: Use ESLint with unused-imports rules
- Clean build artifacts: `rm -rf dist/ build/ .tmp/` before final validation

### Package Management
**Always use auto-confirming commands to prevent terminal blocking on package installations:**

#### Node.js Package Managers
- **npm**: Use `npm install --no-audit --no-fund` to suppress interactive prompts, or set `CI=true` environment variable
- **yarn**: Use `yarn install --non-interactive` to prevent prompts
- **pnpm**: Use `pnpm install --reporter=silent` to suppress interactive prompts

#### System Package Managers
- **apt/apt-get**: Always use `apt-get install -y` to auto-confirm installations
- **yum**: Use `yum install -y` for auto-confirmation
- **brew**: Use `brew install --quiet` to reduce prompts

#### Language-Specific Package Managers
- **pip**: Set `PIP_YES=1` environment variable or use `pip install --quiet` to reduce prompts
- **dotnet**: Use `dotnet add package --accept-license` when licenses require acceptance
- **go**: Use `go mod download` and `go mod tidy` (typically no confirmation needed)
- **composer**: Use `composer install --no-interaction` to prevent prompts

#### Environment Variable Setup for Auto-Confirmation
Set these environment variables at the beginning of scripts to ensure non-interactive behavior:
```bash
export CI=true                    # For npm and other tools
export PIP_YES=1                  # For pip auto-confirmation
export DEBIAN_FRONTEND=noninteractive  # For apt/apt-get
export npm_config_audit=false     # Disable npm audit prompts
export npm_config_fund=false      # Disable npm funding prompts
```

#### Package Installation Best Practices
- **Always verify package installation success** with `npm list` or equivalent verification commands
- **Use lock files** (package-lock.json, yarn.lock) to ensure consistent installations
- **Clean package caches** if installation fails: `npm cache clean --force`
- **Handle installation failures gracefully** with retry logic and fallback strategies

### Terminal and Process Management
When working with development servers and long-running processes, you **must** follow these guidelines to prevent terminal blocking:

#### Development Servers (Frontend & Backend)
- **Always run development servers in background processes** using `&` suffix or in separate terminal sessions
- For frontend servers (e.g., `npm run dev`, `vite dev`): Use `npm run dev &` or run in async terminal session
- For backend servers (e.g., `dotnet run`, `node server.js`): Use `dotnet run &` or run in async terminal session
- **Never let development servers occupy the main terminal session** where subsequent commands need to be executed

#### Process Management Rules
- Before starting any long-running service, kill existing processes on the target ports (3001 for frontend, 5001 for backend)
- Use `kill -9 $(lsof -t -i:3001)` and `kill -9 $(lsof -t -i:5001)` to clean up existing processes
- After starting background processes, always verify they are running with `ps aux | grep <process_name>`
- When testing is complete, clean up background processes to avoid port conflicts

#### Terminal Session Strategy
- Use separate terminal sessions for long-running processes when background execution (`&`) is not suitable
- Main terminal session must remain available for build commands, tests, and other operations
- Always document which processes are running in background in your progress updates

## Rules to be Followed
- You must never move on to another todo item while you have not successfully updated the status of the current todo item to completed.
- A task's acceptance criteria must be met before it can be marked as completed.
- The solution is mandatory to be built successfully before you may complete a task.
- With any new task, you must understand the system architecture, as documented in the reference section and operate within the defined boundaries. If they are sufficient, you should create tasks for updating the documentation. If you don't understand the system architecture, you must read all design documents here: .gaia/designs
- You must **never lie**. Especially on checks that tools mandates. Things like whether builds have been run etc.
- Always **fix build errors as you go**.
- Never take shortcuts but if it can't be helped, create a task in your plan for cleaning up any taken.
- You should always use ports **3001 for frontends** and **5001 for backends**. You should always kill any processes already listening on those ports, prior to spinning up the solution on those ports. This is important in order to get a consistent testing experience.
- You must always use terminal to execute commands. **Never shell**. Follow the Terminal and Process Management guidelines above to prevent terminal blocking when running development servers.
- You **must** use CURL to perform your comprehensive testing of the solution. This is important in order to get a consistent testing experience.
- You **must** use the `npx playwright test --reporter=line` command to run your frontend stack tests. This is important in order to get a consistent testing experience.
  - It is **crutial** that as part of your testing, you test the frontend and backend stacks together, as a whole. This is important in order to get a consistent testing experience.
- You **must** use Playwright, as above, to perform comprehensive integration testing for all frontend solutions.
  - **Visual Quality Mandate**: Take screenshots at ALL viewport sizes (mobile: 375px, tablet: 768px, desktop: 1024px+) for EVERY major page/component
  - **Iterative Screenshot Analysis**: Analyze screenshots like a UI/UX specialist, scoring visual quality 0-100% until ALL criteria achieve 100%
  - **Console Error Monitoring**: MANDATORY capture and analysis of browser console errors, warnings, and network failures during ALL test execution
  - **Human-like E2E Testing**: Navigate application like a human automation tester - test ALL interactions, flows, and edge cases
  - **Template Cleanup Validation**: Verify removal of ALL default template code, placeholder content, debug statements, and development artifacts
  - **State Coverage**: Test and screenshot ALL interactive states (default, hover, focus, active, disabled, loading, error, empty)
  - **Continuous Integration Testing**: After EVERY task completion, re-run the COMPLETE test suite before proceeding to next task
- You **must** use the `src/` directory for all source code. This is important in order to get a consistent testing experience.
- You **must** use the `.gaia/designs/` directory for all design documentation. This is important in order to get a consistent testing experience.