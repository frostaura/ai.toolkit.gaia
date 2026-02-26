# SKILL: Spec Consistency

## Goal
Keep specs, code, and tests aligned.

## Checks
- every behavior change is reflected in `/docs`
- use-cases map to tests (UC IDs referenced)
- API contracts, data models, and UI flows match docs

## Common drift signals
- endpoints exist without docs
- docs promise outputs that code doesn’t deliver
- tests don’t reference the updated use-case

## Outputs
- small doc/code/test edits that restore alignment

## Done when
- docs, code, and tests describe the same reality
