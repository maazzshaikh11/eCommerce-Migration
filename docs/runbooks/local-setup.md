# Local Development Setup

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0.302+ | Backend API, tests |
| Node.js | 22+ | Frontend SPA, E2E tests |
| npm | 10+ | Package management |
| SQL Server LocalDB | — | Database (included with Visual Studio / .NET SDK) |
| Git | 2.x | Version control |

## Step 1: Clone

```bash
git clone <repository-url>
cd ecommerce-migration
```

## Step 2: Database

The application uses SQL Server LocalDB with an existing `LegacyEcommerceDb` database. If the database does not exist:

```bash
# Create the database and seed data
./scripts/setup.sh       # Linux/macOS
.\scripts\setup.ps1       # Windows PowerShell
```

The connection string is in `src/backend/Ecommerce.Api/appsettings.json`:
```json
"ConnectionStrings": {
  "LegacyEcommerceDb": "Server=(localdb)\\MSSQLLocalDB;Database=LegacyEcommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Step 3: Backend

```bash
cd src/backend

# Restore packages
dotnet restore Ecommerce.sln

# Build
dotnet build Ecommerce.Api/Ecommerce.Api.csproj

# Run the API (port 5001)
dotnet run --project Ecommerce.Api/Ecommerce.Api.csproj
```

Verify: `GET http://localhost:5001/api/health` returns `200 OK`.

## Step 4: Frontend

```bash
cd src/frontend/spa

# Install dependencies
npm install

# Start dev server (port 5173 or 5002)
npm run dev
```

Verify: `http://localhost:5173` loads the SPA.

## Step 5: YARP Proxy (Optional)

The YARP reverse proxy is part of the API project. When the API runs on port 5001, YARP is available on the same process. It routes SPA paths to `http://localhost:5002`.

> **Note:** In development, you can access the SPA directly on port 5173 (Vite) without going through YARP. YARP is primarily for production-like testing.

## Environment Variables

| Variable | Location | Default | Purpose |
|----------|----------|---------|---------|
| `JwtSigningKey` | `appsettings.json` | (dev key) | JWT signing key (≥32 bytes) |
| `ConnectionStrings:LegacyEcommerceDb` | `appsettings.json` | LocalDB | SQL Server connection |
| `VITE_API_BASE_URL` | `.env.development` | `http://localhost:5001/api` | API base URL for SPA |

> **Warning:** Never use the development JWT signing key in production. Generate a new key with at least 32 bytes of entropy.

## Verification

```bash
# Backend tests
cd src/backend
dotnet test Ecommerce.Api.Tests/Ecommerce.Api.Tests.csproj
dotnet test Ecommerce.Api.Core.Tests/Ecommerce.Api.Core.Tests.csproj

# Frontend tests
cd src/frontend/spa
npm run build   # includes type check via vue-tsc
npx vitest run  # unit tests
```

## Troubleshooting

### Database Connection Failed
- Ensure LocalDB is running: `sqllocaldb start MSSQLLocalDB`
- Check connection string matches your LocalDB instance

### Port Already in Use
- API default: 5001, SPA default: 5173, YARP: 5003
- Change in `Properties/launchSettings.json` (API) or `vite.config.ts` (SPA)

### JWT Validation Failed
- Ensure `JwtSigningKey` in `appsettings.json` is at least 32 bytes
- Check that the SPA's `api/client.ts` reads from `localStorage['jwt']`

## Related

- [Architecture — Deployment Model](../architecture/deployment-model.md)
- [API Endpoint Catalog](../api/endpoint-catalog.md)
- [Runbook — Database Seeding](database-seeding.md)