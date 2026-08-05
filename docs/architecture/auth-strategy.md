# Authentication Strategy

## Overview

The modern application uses **JWT Bearer authentication** with HS256 symmetric signing. Authentication is implemented natively in the .NET 10 Web API — no dependency on the legacy MVC authentication system.

> **History:** An initial plan (ADR-004) proposed a JWT bridge endpoint in the legacy `AccountController`. This was blocked because the legacy repo is read-only (BLOCKER-T020). Native API auth was implemented instead (ADR-006).

## Architecture

```mermaid
flowchart TB
    subgraph SPA["Vue 3 SPA"]
        AuthStore[useAuthStore.ts]
        Client[api/client.ts]
        AuthStore -->|reads/writes| LS[localStorage 'jwt']
        Client -->|reads token| LS
    end

    subgraph API[".NET 10 Web API"]
        Middleware[JWT Bearer Middleware]
        AuthCtrl[AuthController]
        JwtSvc[JwtTokenService]
        CustRepo[CustomerRepository]
    end

    subgraph DB["SQL Server"]
        CustTable[Customer table]
    end

    AuthStore -->|POST /api/auth/login| AuthCtrl
    AuthCtrl -->|Find by email| CustRepo
    CustRepo -->|SELECT| CustTable
    AuthCtrl -->|Verify password hash| CustTable
    AuthCtrl -->|Generate token| JwtSvc
    JwtSvc -->|HS256 signed JWT| AuthCtrl
    AuthCtrl -->|{ token }| AuthStore

    Client -->|Authorization: Bearer| Middleware
    Middleware -->|Validate signature + expiry| JwtSvc
    Middleware -->|Set User.Identity claims| AuthCtrl
```

## Token Claims

| Claim | Type | Purpose |
|-------|------|---------|
| `customer_id` | int | Identifies the customer in `LegacyEcommerceDb` |
| `email` | string | Customer email |

## Configuration

**`appsettings.json`:**
```json
{
  "JwtSigningKey": "bXlzZWNyZXRrZXltdXN0YmVleGFjdGx5MzJieXRlc2xvbmc="
}
```

**`Program.cs`:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });
```

> Key must be at least 32 bytes. The default key in `appsettings.json` is for development only.

## SPA Auth Flow

1. **Login** — `POST /api/auth/login` with `{ email, password }` → receives `{ token, customerId, email }`
2. **Storage** — Token stored in `localStorage['jwt']`
3. **Request header** — `api/client.ts` reads token from localStorage and attaches `Authorization: Bearer <token>` to every request
4. **Profile** — `GET /api/auth/me` with Bearer token → returns customer profile
5. **Expiry check** — On app init, `useAuthStore` decodes the JWT and checks `exp` claim; expired tokens are auto-cleared
6. **Logout** — Token removed from localStorage; `authStore` reset

## Endpoints

| Method | Path | Auth | Purpose |
|--------|------|------|---------|
| `POST` | `/api/auth/login` | None | Verify credentials, issue JWT |
| `POST` | `/api/auth/register` | None | Create customer, issue JWT |
| `GET` | `/api/auth/me` | Bearer | Return authenticated user profile |

## Protected Endpoints

All endpoints under `/api/cart/*`, `/api/checkout/*`, and `/api/orders/*` require `[Authorize]` (JWT Bearer). The `/api/products/*` and `/api/categories/*` endpoints are anonymous (read-only browsing).

## Related

- [ADR-004: JWT Auth Bridge (superseded)](../decisions/004-jwt-auth-bridge.md)
- [ADR-006: Native API Auth](../decisions/006-native-api-auth.md)
- [Blocker Report — T-020](../migration/blocker-t020.md)