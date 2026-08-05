# ADR-004: JWT Authentication Bridge (Option A)

## Status

Superseded by [ADR-006: Native API Authentication](006-native-api-auth.md)

## Context

The legacy application uses ASP.NET Identity 2.0 with OWIN cookie authentication. The migration needed a way to authenticate Vue SPA users against the legacy identity store during the hybrid phase.

Option A proposed adding a JWT bridge endpoint to the legacy MVC `AccountController` that would:
1. Read the OWIN cookie auth user.
2. Resolve the `CustomerId` from claims.
3. Issue a JWT signed with a shared symmetric key.
4. Return `{ token }` JSON to the SPA.

## Decision

Initially selected Option A. However, execution hit a blocker (see [BLOCKER-T020](../migration/blocker-t020.md)): the legacy repository is read-only by policy, and writing to `AccountController.cs` was not permitted.

## Consequences

The blocker led to a better architecture. Instead of bridging legacy cookies, native API authentication was implemented directly in the new backend (ADR-006). This eliminated the dependency on legacy code and simplified the auth flow.

## Related

- [Blocker Report — T-020](../migration/blocker-t020.md)
- [ADR-006: Native API Authentication](006-native-api-auth.md)