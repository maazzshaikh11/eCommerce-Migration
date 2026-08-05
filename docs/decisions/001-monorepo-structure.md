# ADR-001: Monorepo Structure

## Status

Accepted

## Context

The migration needs a clean separation between legacy reference code and the new implementation. The legacy ASP.NET MVC 5 application lives in a separate repository and remains read-only. All new code — backend, frontend, gateway, migration tools, shared contracts, tests, infrastructure, and documentation — must live in a single, well-organized repository that supports parallel development by multiple engineers.

Options considered:
1. **Polyrepo** — one repository per component (API, SPA, gateway, shared).
2. **Monorepo** — a single repository with all components in subdirectories.

## Decision

Adopt a **monorepo** structure. The root contains only configuration and entry points. All source code lives under `src/`, all tests under `tests/`, all documentation under `docs/`, all infrastructure under `infra/`, and all container definitions under `docker/`.

```
ecommerce-migration/
├── .github/
├── config/
├── docker/
├── docs/
├── infra/
├── scripts/
├── src/
│   ├── backend/
│   ├── frontend/
│   ├── gateway/
│   ├── migration/
│   └── shared/
├── tests/
├── tools/
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
└── Ecommerce.sln
```

## Consequences

**Positive:**
- Single clone gives the entire project.
- Atomic cross-component changes (API + SPA + tests in one commit).
- Shared build configuration via `Directory.Build.props` and `Directory.Packages.props`.
- Simplified CI/CD — one pipeline builds everything.

**Negative:**
- Repository size is larger.
- Requires discipline to keep components decoupled.

## Related

- [Monorepo Design](../architecture/monorepo-design.md)
- [ADR-005: Vertical Slice Migration](005-vertical-slice-migration.md)