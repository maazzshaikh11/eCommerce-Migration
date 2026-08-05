# ADR-002: YARP Reverse Proxy for Ingress

## Status

Accepted

## Context

The Strangler Fig pattern requires a reverse proxy that routes traffic between the legacy MVC application and the modern Vue SPA + .NET API during the hybrid phase. The proxy must support path-based routing, priority ordering, and gradual migration of routes from legacy to modern.

Options considered:
1. **YARP** (Yet Another Reverse Proxy) — Microsoft's .NET-native reverse proxy.
2. **Nginx** — battle-tested but external to the .NET ecosystem.
3. **HAProxy** — high performance but configuration is opaque to .NET developers.
4. **IIS URL Rewrite / ARR** — Windows-only, ties to IIS.

## Decision

Adopt **YARP** (`Yarp.ReverseProxy` 2.3.0). It is configured in `Program.cs` via `appsettings.json` and runs as part of the API process on port 5003.

### Route Configuration

Routes are defined in `appsettings.json` under `ReverseProxy.Routes`. Each route maps a path pattern to a cluster:

| Route | Cluster | Priority | Purpose |
|-------|---------|----------|---------|
| `/login/{**catch-all}` | spaCluster | 3 | Login page |
| `/register/{**catch-all}` | spaCluster | 3 | Register page |
| `/orders/{**catch-all}` | spaCluster | 3 | Order history |
| `/` | spaCluster | 3 | Home page |
| `/products/{**catch-all}` | spaCluster | 3 | Product catalog |
| `/product/{**catch-all}` | spaCluster | 3 | Product detail |
| `/checkout/{**catch-all}` | spaCluster | 3 | Checkout wizard |
| `/cart/{**catch-all}` | spaCluster | 3 | Shopping cart |
| `/app/{**catch-all}` | spaCluster | 2 | SPA app shell |
| `{**catch-all}` | spaCluster | 1 | Fallback |

### Deviation: apiCluster Omitted

The `apiCluster` route was intentionally omitted from the YARP config to prevent a self-proxy infinite loop. The API controllers handle `/api/*` natively; YARP only proxies SPA routes. See `docs/migration/progress.md` T-007 deviation note.

## Consequences

**Positive:**
- .NET-native — configured in C#, debuggable in the same process.
- Path-based routing with priority enables incremental migration.
- No external dependency during development.

**Negative:**
- Single process for API + proxy (acceptable for this scale).
- `apiCluster` omission is a known deviation from the original plan.

## Related

- [Architecture Migration Plan](../architecture/architecture-migration-plan.md)
- [Progress — T-007 Deviation](../migration/progress.md)