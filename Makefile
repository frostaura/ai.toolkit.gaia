.PHONY: build up down logs restart

build:
	docker compose build

up:
	docker compose up -d

down:
	docker compose down

logs:
	docker compose logs -f gaia-mcp

restart:
	docker compose down && docker compose up -d
