# SKILL: Manual Regression (API via curl)

## Goal
Manually validate API behavior against the docker-compose stack.

## Steps
1) `make up`
2) Run a small set of curl calls matching the changed use-cases
3) Validate expected status codes and critical payload fields
4) Record only the label `curl` in MCP proof args

## Done when
- API flows are manually validated for the changed use-cases
