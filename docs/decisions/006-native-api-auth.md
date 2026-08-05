# ADR-006: Native API Authentication

## Status

Accepted — supersedes [ADR-004](004-jwt-auth-bridge.md)

## Context

ADR-004 proposed a JWT bridge endpoint in the legacy MVC `AccountController`. This was blocked because the legacy repository is read-only (see [BLOCKER-T020](../migration/blocker-t020.md)).

The new approach: implement authentication natively in the .NET 10 Web API. The legacy `Customer` table already exists in `LegacyEcommerceDb` and is accessible via ADO.NET. The API can verify credentials against this table and issue JWT tokens directly.

## Decision

Implement native authentication in the new backend:

1. **`AuthController`** with `POST /api/auth/login` and `POST /api/auth/register`.
2. **`JwtTokenService`** in `Ecommerce.Api.Core/Security/` — generates HS256 JWT tokens with `customer_id` and `email` claims.
3. **Credentials verification** — password hash comparison against the legacy `Customer` table (PBKDF2 via ASP.NET Identity format).
4. **Registration** — inserts new `Customer` rows with hashed passwords.
5. **`GET /api/auth/me`** — returns the authenticated user's profile.
6. **SPA auth store** — `useAuthStore.ts` manages JWT in `localStorage['jwt']`, attaches `Authorization: Bearer` header to all API calls, checks token expiry on app init.

```
Vue SPA → POST /api/auth/login { email, password }
                ↓
    AuthController → CustomerRepository → Customer table
                ↓
    JwtTokenService.GenerateToken(customerId, email)
                ↓
    { token: "eyJ..." } → SPA stores in localStorage
```

## Consequences

**Positive:**
- No dependency on legacy code — the legacy app can be fully decommissioned.
- Simpler architecture — one auth system, not two.
- Registration works natively — no need to register via legacy and bridge.

**Negative:**
- Password hashing must match the legacy ASP.NET Identity 2.0 format (PBKDF2 with base64 salt).
- The legacy identity database (`LegacyEcommerceIdentity`) is not used — customers are managed in `LegacyEcommerceDb.Customer`.

## Related

- [ADR-004: JWT Auth Bridge (superseded)](004-jwt-auth-bridge.md)
- [Blocker Report — T-020](../migration/blocker-t020.md)
- [Architecture Migration Plan — Auth Strategy](../architecture/architecture-migration-plan.md)