# Incident Response

## Overview

This runbook covers common production incidents and their resolution steps.

## Incident Categories

| Severity | Description | Response Time |
|----------|-------------|---------------|
| P0 — Critical | Checkout, auth, or API fully down | Immediate |
| P1 — High | Major feature broken (cart, product browse) | < 1 hour |
| P2 — Medium | Minor feature broken, degraded experience | < 4 hours |
| P3 — Low | Cosmetic, non-user-facing | Next business day |

## Common Incidents

### API Returns 500 on All Requests

1. Check API process is running: `GET /api/health`
2. Check database connectivity: `sqlcmd -S "<connection-string>" -Q "SELECT 1"`
3. Check `JwtSigningKey` is set and ≥32 bytes
4. Check application logs for unhandled exceptions

### Authentication Failing (401 on All Protected Endpoints)

1. Verify JWT signing key matches between API config and token issuer
2. Check token expiry — `exp` claim in JWT payload
3. Ensure SPA reads token from `localStorage['jwt']` (not `localStorage['token']`)
4. Check CORS policy — dev origin `http://localhost:5173` must be allowed

### Checkout Returns 400 "Idempotency-Key header is required"

1. Ensure SPA sends `Idempotency-Key` as an HTTP header (not request body)
2. Each checkout attempt should generate a new UUID key
3. Duplicate key returns the original order (idempotent)

### Order Totals Incorrect

1. Verify GST rate: `18%` (0.18m constant in `CheckoutService`)
2. Shipping: Standard ₹99, Express ₹199
3. Formula: `GrandTotal = SubTotal + Shipping + (SubTotal + Shipping) × 0.18`

### Cart Empty After Login

1. Cart is customer-based — anonymous session cart does not auto-merge
2. This is a known limitation (cart merge on login is not implemented)
3. Workaround: add items after logging in

## Rollback

See [Rollback Procedures](rollback-procedures.md).

## Related

- [Rollback Procedures](rollback-procedures.md)
- [Architecture — Deployment Model](../architecture/deployment-model.md)
- [Local Setup](local-setup.md)