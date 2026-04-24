---
name: gaia-ui-engineering
description: Provides React UI implementation guidance that enforces the design system, shadcn/ui composition, semantic Tailwind tokens, and responsive layout discipline. Use it by composing shadcn/ui primitives with semantic Tailwind tokens (no raw color/spacing literals), respecting the responsive layout rules, and removing dead tokens as you go. Use it when building or refactoring user-facing React components, screens, forms, dialogs, or styling where UI consistency and token cleanup matter.
license: MIT
---

# Gaia UI Engineering

## Scope and when to use

Use this skill to implement frontend work without breaking the shared UI
system.

Use this skill when:

- a branch adds or changes React UI components, screens, forms, dialogs, or navigation
- existing frontend code needs design-system cleanup during delivery
- a request includes styling, spacing, typography, color, or layout decisions
- the work must preserve accessibility and visual consistency through shared primitives

Do not use this skill when:

- the main task is backend behavior, API design, or non-UI configuration
- the work is architecture definition rather than concrete frontend implementation
- the change is pure visual QA with no implementation ownership

## Required inputs

- the active UI requirements and acceptance criteria
- the installed component library and Tailwind token model
- the current React component structure and any touched legacy styles
- accessibility expectations for focus, semantics, keyboard flow, and contrast

## Owned outputs

- React UI changes that stay inside the design system
- refactors that replace legacy arbitrary values with approved tokens or utilities
- responsive layouts that grow from content instead of fixed dimensions
- concise notes when a requested UI pattern must be redirected to a compliant implementation

## Decision tree

- If the branch touches user-facing React UI, invoke this skill before editing markup or classes.
- If an interactive primitive is needed, compose it from `@/components/ui/` instead of authoring a raw replacement.
- If the requested styling depends on raw hex values, bespoke CSS, inline styles, or arbitrary Tailwind values, push back and translate it into existing tokens and utilities.
- If a layout request depends on fixed heights or rigid dimensions, redesign it around content flow unless a documented performance reason requires the constraint.
- If existing files already contain arbitrary values in the touched area, normalize them while making the requested change.

## Core workflow

1. Identify the user-facing components and the interactive states the branch changes.
2. Choose the closest existing `shadcn/ui` primitives and compose from them before considering any custom wrapper.
3. Map colors, spacing, typography, borders, and elevation to approved semantic tokens and standard Tailwind utilities.
4. Build layouts with flexible grid or flex primitives so content can wrap, grow, and reflow naturally.
5. Refactor touched legacy UI classes that use arbitrary values into the nearest approved token or utility.
6. Check accessibility basics such as labeling, focus order, keyboard reachability, and semantic structure.

## UI implementation guardrails

- use installed `shadcn/ui` primitives for buttons, dialogs, dropdowns, badges, tabs, and inputs
- import interactive building blocks from `@/components/ui/` instead of hand-rolling raw equivalents
- favor composition with shared layout primitives such as cards, headers, content blocks, and form controls
- use semantic color tokens such as `primary`, `secondary`, `muted`, `accent`, `background`, `surface`, `border`, `success`, `warning`, and `destructive`
- use the standard Tailwind spacing scale and text-size utilities; avoid arbitrary values like `gap-[10px]`, `p-[15px]`, or `text-[18px]`
- use standard shadow utilities for elevation instead of custom box-shadow definitions
- prefer responsive flex and grid layouts, `flex-1`, `min-w-0`, and standard min-height utilities over hard size locks
- avoid rigid container or card heights unless virtualization or another documented runtime constraint truly requires them

## Enforcement behavior

- explain design-system constraints clearly when a request conflicts with them
- provide the closest compliant implementation instead of simply refusing the UI work
- treat bespoke CSS and arbitrary utility values as exceptions that need a documented reason, not as the default path
- leave touched files more compliant than you found them by cleaning up nearby legacy arbitrary values when practical

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| missing primitive | compose from existing `shadcn/ui` parts first | UI engineer | route upstream only if the library truly lacks the pattern |
| token mismatch | map to the nearest semantic token and document the substitution | UI engineer | involve design-system ownership if no approved token exists |
| rigid layout request | redesign around content flow and responsive wrapping | UI engineer | require explicit justification for fixed sizing |
| legacy arbitrary values in touched code | refactor the local slice to approved utilities | UI engineer | widen only if cleanup becomes a separate branch |

## Anti-patterns

- do not create raw interactive primitives that duplicate `@/components/ui/`
- do not introduce bespoke CSS files, inline styles, raw hex values, or default palette utility classes when approved tokens exist
- do not lock cards or containers to arbitrary heights just to make a layout look stable
- do not preserve nearby arbitrary values in touched UI code when they can be normalized safely

## Handoff and downstream impact

- tell testing which UI states, breakpoints, and accessibility behaviors deserve extra attention
- tell planning when a UI request depends on a missing design-system primitive or token
- tell architecture only when the request reveals a broader design-system model gap
- tell engineering peers which legacy values were normalized during the branch

## Examples

- **Good fit:** implement a new React settings panel using shared dialog, tabs, form, and card primitives while cleaning up old arbitrary spacing classes.
- **Good fit:** refactor a dashboard card layout from rigid pixel heights to responsive content-driven sections.
- **Not a fit:** define a new cross-product design language from scratch; that belongs upstream.

## Completion checklist

- interactive UI uses shared `shadcn/ui` primitives
- styling uses approved semantic tokens and standard utilities
- layouts expand from content instead of rigid arbitrary sizing
- touched legacy arbitrary values have been normalized where practical
- accessibility basics were considered before handoff

## References

- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
