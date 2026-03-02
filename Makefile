.PHONY: build test lint up down test-e2e

## Build all projects
build:
	dotnet build Gaia.slnx

## Run all unit tests
test:
	dotnet test Gaia.slnx --verbosity normal

## Run linter (dotnet format check)
lint:
	dotnet format Gaia.slnx --verify-no-changes

## Start docker-compose stack
up:
	docker compose up -d --build

## Stop docker-compose stack
down:
	docker compose down

## Run integration / e2e tests (requires running stack)
test-e2e:
	dotnet test .github/mcp/tests/Gaia.Mcp.Server.IntegrationTests --verbosity normal
