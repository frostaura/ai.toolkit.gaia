# SKILL: Stack Default — Python

## Baseline
- format: Black
- lint: Ruff
- tests: Pytest
- docker: compose-first for HTTP APIs
- CI: lint + tests

## Makefile targets (preferred)
- `make lint`
- `make test`
- `make up` / `make down` (if HTTP API)
