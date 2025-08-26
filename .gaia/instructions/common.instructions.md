---
applyTo: "**"
---
<!-- Custom instructions that should be applied to any prompt (all directories, files and file types). -->

# Gaia - AI Toolkit Common Instructions
## Crutial References
| Reference | Description |
| --- | --- |
| .gaia/designs/*.md | A collection of important design documentation for the solution, together with overview for the design system, expected repository structure and Docker setup. |

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

### Regression Prevention Workflow (NEW - MANDATORY)
**Execute this workflow after EVERY feature implementation:**

1. **Pre-Validation**: Capture baseline state before testing
   - Run complete test suite: `npm test` (or `dotnet test` for .NET)
   - Record test results and performance metrics
   - Capture visual snapshots if UI changes were made

2. **Compatibility Testing**: Validate existing functionality
   - Execute all automated tests for existing features
   - Manually test critical user journeys that could be affected
   - Verify API endpoints maintain backward compatibility

3. **Regression Detection**: Identify any breaks in existing functionality
   - Compare test results with baseline
   - Check for performance degradation (>5% slowdown triggers investigation)
   - Review visual regression test results for unintended UI changes

4. **Resolution**: Fix any detected regressions before proceeding
   - If regressions found, halt feature development immediately
   - Investigate root cause and implement compatibility fix
   - Re-run full validation until 100% pass rate achieved

5. **Documentation**: Record validation results
   - Update task with regression test results
   - Document any compatibility issues discovered and resolved
   - Note any preventive measures implemented for future development

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

#### Visual Testing Commands
**Screenshot Analysis Commands**:
- Take screenshots at multiple viewports: Use `page.setViewportSize()` with mobile (375px), tablet (768px), desktop (1024px+)
- Critical analysis command pattern: `await page.screenshot({ path: 'analysis-[pagename]-[viewport].png', fullPage: true })`
- Interactive state testing: Capture screenshots for hover, focus, loading, error states using `page.hover()`, `page.focus()`, etc.

**Template Cleanup Validation Commands**:
- Check for placeholder content: `grep -r "Lorem ipsum\|Welcome to\|placeholder\|TODO\|console\.log" src/`
- Verify no debug statements: `grep -r "console\.\|debugger\|alert(" src/`
- Validate no unused imports: Use ESLint with unused-imports rules
- Clean build artifacts: `rm -rf dist/ build/ .tmp/` before final validation

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
  - **Human-like E2E Testing**: Navigate application like a human automation tester - test ALL interactions, flows, and edge cases
  - **Template Cleanup Validation**: Verify removal of ALL default template code, placeholder content, debug statements, and development artifacts
  - **State Coverage**: Test and screenshot ALL interactive states (default, hover, focus, active, disabled, loading, error, empty)
- You **must** use the `src/` directory for all source code. This is important in order to get a consistent testing experience.
- You **must** use the `.gaia/designs/` directory for all design documentation. This is important in order to get a consistent testing experience.