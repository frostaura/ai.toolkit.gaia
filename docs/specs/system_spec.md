# System Specification — Gaia AI Toolkit

## Overview

The Gaia AI Toolkit is a Model Context Protocol (MCP) server ecosystem that provides task management, project memory, and self-improvement capabilities for AI agent orchestration workflows.

## Components

### 1. Gaia MCP Server (`Gaia.Mcp.Server`)

An HTTP-based MCP server exposing three tool families:

- **Tasks** (`tasks_*`): Project-scoped task tracking with completion enforcement (gates, blockers, proof).
- **Memory** (`memory_*`): Persistent key-value fact storage per project.
- **Self-Improvement** (`self_improve_*`): Global improvement suggestion store with project/category filtering.

**Transport**: HTTP (ASP.NET Core), endpoint at `/mcp`.

### 2. Gaia Workflows Server (`Gaia.Workflows.Server`)

A stdio-based MCP server for executing YAML-defined workflows:

- **Workflows** (`workflows_list`, `workflows_execute`): Parse and execute step-based YAML workflows from `.github/.agaia-workflows/`.

**Transport**: stdio.

## Storage

Both servers use `ThreadSafeJsonStore<T>`, a thread-safe JSON file store with per-key locking and atomic writes (temp-file-then-move pattern).

- Tasks: `{GAIA_DATA_DIR}/{project}.tasks.json`
- Memory: `{GAIA_DATA_DIR}/{project}.memory.json`
- Self-Improvement: `{GAIA_DATA_DIR}/__global.improvements.json`

Project names are sanitized to prevent path traversal attacks.

## Task Completion Policy

`tasks_mark_done` enforces:
1. No unresolved blockers (NEEDS_INPUT checked first for specificity)
2. All proof arrays non-empty: `changedFiles`, `testsAdded`, `manualRegressionLabels`
3. All required gates satisfied

Status values are validated: only `todo`, `doing`, `done` are accepted.

## Error Codes

All structured errors use `GAIA_TASKS_ERR_*` namespace. See `schemas/error-codes.md`.

## Docker

The MCP server is containerized via `docker-compose.yml` with:
- Port mapping: `${GAIA_MCP_PORT:-5059}:8080`
- Persistent volume: `gaia-data` mounted at `/data`
- Health check: HTTP probe to `/mcp` endpoint

## CI/CD

GitHub Actions workflow (`build-gaia-mcp.yml`):
- **PR/push**: lint, build, test (solution-scoped)
- **main/dispatch only**: Docker image build and push to Docker Hub
