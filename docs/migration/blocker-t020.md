# Blocker Report

## Task ID
T-020: Add JWT Bridge Endpoint to Legacy MVC

## Root Cause
Execution instructions explicitly state: "The legacy repository is read-only and serves only as a reference during migration. All new work must be created inside eCommerce-Modernization." T-020 requires writing to `C:\Users\Administrator\Desktop\eCommerce\Ecommerce.Web\Controllers\AccountController.cs` (legacy repo). This directly conflicts with the read-only constraint.

## Recovery Attempts
- Attempted to locate an alternative bridge implementation path within the modern repo — none specified in project documents.
- The JWT bridge depends on the legacy MVC cookie authentication system; a modern-repo-only bridge would need the legacy machine key to decrypt cookies, which is neither available nor architecturally sound per the existing plans.

## Current Status
Blocked at T-020. Phase 2 (JWT Auth Bridge & Vue SPA Shell) cannot proceed past T-019 without resolving the bridge.

## Logs
- Legacy repo path: `C:\Users\Administrator\Desktop\eCommerce\Ecommerce.Web\Controllers\AccountController.cs` — confirmed read-only by policy.

## Suggested Resolution
1. **Option A:** Explicitly grant one-time write access to legacy `AccountController.cs` for T-020 only, then restore read-only status.
2. **Option B:** Implement the JWT bridge as a separate mini-service deployed alongside legacy, so legacy code is untouched.
3. **Option C:** Skip the bridge entirely and implement native API auth (login/register endpoints) earlier, making the bridge unnecessary.

**Awaiting user decision before proceeding.**