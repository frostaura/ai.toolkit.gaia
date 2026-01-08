---
name: reflection
description: Systematic reflection on completed work to capture learnings, patterns, and improvements
---

# Reflection Skill

Mandatory post-task reflection to build institutional knowledge and prevent repeated mistakes.

## When to Reflect

- ✅ After completing any task
- ✅ After fixing an issue/bug
- ✅ After encountering a blocker
- ✅ After quality gate validation
- ✅ After each feature implementation

## Reflection Process

**1. Recall Context** (before reflection)

```
recall("[task/feature/issue]") - Check past learnings
recall("[technology]") - Review known patterns
```

**2. Analyze What Happened**

- What was the goal?
- What approach was taken?
- What worked well?
- What failed or caused issues?
- What was unexpected?

**3. Capture Learnings** (mandatory `remember()` calls)

**Success Patterns**:

```
remember("pattern", "[context]", "[what worked and why]", "ProjectWide")
remember("best_practice", "[area]", "[approach that succeeded]", "ProjectWide")
```

**Issues & Resolutions**:

```
remember("issue", "[problem_key]", "[what failed + how fixed]", "ProjectWide")
remember("workaround", "[limitation]", "[solution applied]", "ProjectWide")
```

**Decisions & Rationale**:

```
remember("decision", "[choice]", "[why chosen + alternatives considered]", "ProjectWide")
remember("tradeoff", "[decision]", "[benefits vs costs]", "ProjectWide")
```

**Warnings & Gotchas**:

```
remember("warning", "[technology/approach]", "[caveat/limitation]", "ProjectWide")
remember("antipattern", "[context]", "[what NOT to do + why]", "ProjectWide")
```

**4. Identify Improvements**

- What would you do differently next time?
- What can be automated or simplified?
- What documentation is missing?

## Reflection Template

```markdown
## Reflection: [Task/Feature Name]

**Goal**: [What was attempted]
**Outcome**: [Success/Partial/Blocked]

### What Worked

- [Pattern/approach that succeeded]
- [Effective tool/technique used]

### What Failed

- [Issue encountered]
- [Root cause]
- [How resolved]

### Key Learnings

- [Important discovery]
- [Pattern to reuse]
- [Antipattern to avoid]

### Stored Memories

- `remember("pattern", "X", "...")`
- `remember("issue", "Y", "...")`
- `remember("warning", "Z", "...")`

### Next Time

- [Improvement for future similar tasks]
```

## Critical Rules

- ✅ Reflect AFTER every task completion
- ✅ Use ProjectWide duration for permanent learnings
- ✅ Capture both successes AND failures
- ✅ Store specific, actionable knowledge
- ✅ Include context in memory keys
- ❌ Never skip reflection after issues
- ❌ Never store vague/generic learnings
- ❌ Never forget to check `recall()` first

## Integration with Other Skills

**With Web Research**:

- Reflect on research quality and efficiency
- Store best sources found
- Note research patterns that worked

**With Testing**:

- Reflect on test coverage and effectiveness
- Store common test patterns
- Note edge cases discovered

**With Implementation**:

- Reflect on code quality and design
- Store architectural patterns
- Note technical debt created

---

**Remember**: Reflection builds institutional memory. Every task is a learning opportunity.
