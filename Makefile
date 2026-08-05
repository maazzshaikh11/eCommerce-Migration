.PHONY: help \
	restore \
	build build-backend build-frontend \
	test test-backend test-frontend test-e2e \
	lint format verify \
	docker-up docker-down \
	clean

help: ## Show available commands
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS=":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

# ------------------------
# Restore Dependencies
# ------------------------

restore: ## Restore backend and frontend dependencies
	cd src/backend && dotnet restore Ecommerce.sln
	cd src/frontend/spa && npm ci

# ------------------------
# Build
# ------------------------

build: build-backend build-frontend ## Build entire solution

build-backend: ## Build .NET backend
	cd src/backend && dotnet build Ecommerce.sln --no-restore

build-frontend: ## Build Vue frontend
	cd src/frontend/spa && npm run build

# ------------------------
# Testing
# ------------------------

test: test-backend test-frontend ## Run backend and frontend tests

test-backend: ## Run .NET tests
	cd src/backend && dotnet test Ecommerce.sln --no-build

test-frontend: ## Run Vue unit tests
	cd src/frontend/spa && npx vitest run

test-e2e: ## Run Playwright end-to-end tests
	cd tests/e2e && npx playwright test

# ------------------------
# Quality
# ------------------------

lint: ## Type check frontend
	cd src/frontend/spa && npx vue-tsc --noEmit

format: ## Verify .NET formatting
	cd src/backend && dotnet format Ecommerce.sln --verify-no-changes

verify: ## Run full verification (same as CI)
	cd src/backend && dotnet build Ecommerce.sln
	cd src/backend && dotnet test Ecommerce.sln
	cd src/frontend/spa && npm run build
	cd src/frontend/spa && npx vitest run
	cd tests/e2e && npx playwright test

# ------------------------
# Docker
# ------------------------

docker-up: ## Start application stack
	docker compose -f docker/compose/docker-compose.yml up -d

docker-down: ## Stop application stack
	docker compose -f docker/compose/docker-compose.yml down

# ------------------------
# Cleanup
# ------------------------

clean: ## Remove build artifacts
	cd src/backend && dotnet clean Ecommerce.sln
