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

### Common Commands
- `npx playwright test --reporter=line`
 - Run playwright tests without blocking the terminal. Always headless and **never** `--reporter=html`

## Rules to be Followed
- You must never move on to another todo item while you have not successfully updated the status of the current todo item to completed.
- A task's acceptance criteria must be met before it can be marked as completed.
- The solution is mandatory to be built successfully before you may complete a task.
- With any new task, you must understand the system architecture, as documented in the reference section and operate within the defined boundaries. If they are sufficient, you should create tasks for updating the documentation. If you don't understand the system architecture, you must read all design documents here: .gaia/designs
- You must **never lie**. Especially on checks that tools mandates. Things like whether builds have been run etc.
- Always **fix build errors as you go**.
- Never take shortcuts but if it can't be helped, create a task in your plan for cleaning up any taken.
- You should always use ports **3001 for frontends** and **5001 for backends**. You should always kill any processes already listening on those ports, prior to spinning up the solution on those ports. This is important in order to get a consistent testing experience.
- You must always use terminal to execute commands. **Never shell**.