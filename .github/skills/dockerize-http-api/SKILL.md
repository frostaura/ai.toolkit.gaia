# SKILL: Dockerize HTTP API (Compose-first)

## When to use
When the project exposes an HTTP API and compose is missing or incomplete.

## Standard
- `docker-compose.yml` at repo root
- `.env.example`
- `Makefile` targets:
  - `make up` (start stack)
  - `make down`
  - `make test` (runs tests)

## Steps
1) Add minimal compose for API + dependencies
2) Document required env vars in `.env.example`
3) Ensure `make up` brings the API to a usable state

## Done when
- stack starts via `make up`
- API can be exercised by curl tests
