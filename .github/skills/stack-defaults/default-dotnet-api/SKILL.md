# SKILL: Stack Default — .NET HTTP API

## Baseline
- format/lint: `dotnet format` (or analyzers) + editorconfig
- tests: xUnit/NUnit + HTTP integration checks (curl or test host)
- docker: compose-first for API + deps
- CI: restore/build/test + lint/format check

## Makefile targets (preferred)
- `make lint`
- `make test`
- `make up` / `make down`
