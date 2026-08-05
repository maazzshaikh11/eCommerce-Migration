.PHONY: help build build-backend build-frontend test test-backend test-frontend test-e2e lint docker-up docker-down clean restore

help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

build: build-backend build-frontend ## Build everything (backend + frontend)

build-backend: ## Build the .NET solution
	cd src/backend && dotnet build Ecommerce.sln

build-frontend: ## Build the Vue SPA
	cd src/frontend/spa && npm run build

restore: ## Restore all dependencies
	cd src/backend && dotnet restore Ecommerce.sln
	cd src/frontend/spa && npm install

test: test-backend test-frontend ## Run all tests (backend + frontend)

test-backend: ## Run backend tests
	cd src/backend && dotnet test Ecommerce.sln

test-frontend: ## Run frontend unit tests
	cd src/frontend/spa && npx vitest run

test-e2e: ## Run Playwright E2E tests
	cd tests/e2e && npx playwright test

lint: ## Lint everything
	cd src/frontend/spa && npx vue-tsc --noEmit

docker-up: ## Start all services via Docker
	docker compose -f docker/compose/docker-compose.yml up -d

docker-down: ## Stop all Docker services
	docker compose -f docker/compose/docker-compose.yml down

clean: ## Clean build artifacts
	cd src/backend && dotnet clean Ecommerce.sln
	cd src/frontend/spa && rm -rf dist node_modules/.vite
	find . -type d -name bin -exec rm -rf {} + 2>/dev/null || true
	find . -type d -name obj -exec rm -rf {} + 2>/dev/null || true