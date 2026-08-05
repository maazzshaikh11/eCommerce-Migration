# Rollback Procedures

## Overview

This runbook describes how to roll back deployments of the eCommerce application.

## Rollback Strategy

| Component | Strategy | Downtime |
|-----------|---------|----------|
| API | Blue/green — previous image kept warm | ~0 |
| SPA | Immutable static assets — previous version served by CDN | ~0 |
| Database | Forward-only migrations (no auto-rollback) | N/A |

## API Rollback

```bash
# 1. Identify the previous healthy image
docker images ecommerce-api --format "{{.ID}} {{.CreatedAt}}"

# 2. Stop current container
docker stop ecommerce-api-current

# 3. Start previous image
docker run -d --name ecommerce-api -p 5001:5001 <previous-image-id>

# 4. Verify
curl http://localhost:5001/api/health
```

## SPA Rollback

```bash
# 1. Identify previous build in CDN/storage
# 2. Update nginx/CDN to serve previous dist/ directory
# 3. Verify: curl https://<domain>/
```

## Database Rollback

> **Warning:** Database rollbacks are manual and require DBA involvement. The application uses forward-only migrations only.

If a migration causes data loss:

1. Stop the API immediately
2. Restore from the most recent backup:
   ```bash
   sqlcmd -S "<server>" -Q "RESTORE DATABASE LegacyEcommerceDb FROM DISK = '<backup-path>'"
   ```
3. Roll back the API to the version compatible with the restored schema
4. Verify data integrity before resuming traffic

## Configuration Rollback

```bash
# Revert appsettings.json
git checkout HEAD~1 -- src/backend/Ecommerce.Api/appsettings.json

# Revert .env
git checkout HEAD~1 -- src/frontend/spa/.env.development

# Restart services
```

## Verification After Rollback

1. `GET /api/health` → 200
2. `GET /api/products` → 200 with product list
3. `POST /api/auth/login` → 200 with token
4. Browse SPA → home page loads, products visible

## Related

- [Incident Response](incident-response.md)
- [Architecture — Deployment Model](../architecture/deployment-model.md)