<!-- A prompt that allows for analyzing the current state of the Gaia AI Toolkit framework and allows for iteratively improving the Gaia AI Toolkit.-->

# Gaia - AI Toolkit Improvement Process
## Crutial References
| Reference | Description |
| --- | --- |
| .gaia/instructions/*.instructions.md | A collection of Copilot instructions that the Gaia AI Toolkit leverages to enforce common rules as well as file and directory-specific rules. |
| .gaia/prompts/*.prompt.md | A collection of Copilot prompts that the Gaia AI Toolkit leverages to run various workloads. |
| .gaia/prompts/gaia-plan.prompt.md | The crutial entry point for the Gaia AI Toolkit to start planning the system building process/workload(s)/planning/tasks etc. |

## gaia-improve.prompt.md (this prompt) Instructions
### Introduction
The following repo is an advanced prompting system for generating full-stack end to end solutions. From simple to complex. We call this system/framework the Gaia AI Toolkit.

### What to Do
- Ignore any and all Copilot instructions while we are running this Gaia improvement prompt. This means any instructions that we have only pertains to workloads other than this Gaia improve prompt.
- You **must** read the **entire** plan prompt to understand what Gaia is and how it operates. Of course, don't restrict yourself to the plan prompt file when analyzing, it should merely be the starting point.
- **If** a query about Gaia is asked. For example how does X or Y work, you.
- **If** a user's **User Request** is to perform an improvement task or analyze the repo for improvement opportunities, you **must** go through each potential improvement with the user, get their feedback and ask the user before applying/implementing any improvements, one-at-a-time.
- **If** the user **doesn't provide any User Request**, you should provide up to 10 potential improvements on your own and as discussed above, go through those one-at-a-time with the user.
- **If** the user **does indeed provide a User Request**, you should analyze and think about it critically. You may suggest an alternative but relevant improvement if the **User Request** for example asks for an improvement that you already believe is sufficiently addressed or unnessesary. Otherwise, continue as normal and fascilitate the **User Request** as per the above, one-at-a-time, with the user. 

### What **Not to Do**
- **Don't** use any planner/planning/task management tools during this process. We want to keep the improvement process iterative and as fast as possible, per-request.

### User Request
User's prompt: 