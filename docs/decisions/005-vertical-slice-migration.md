# ADR-005: Vertical Slice Migration Order

## Status

Accepted

## Context

The Strangler Fig pattern migrates a monolith incrementally. The question is: which slices to migrate first, and in what order?

The legacy application has these vertical slices (feature areas):
- Product Catalog (browse, search, detail)
- Shopping Cart (add, remove, update, persist)
- Checkout (address, shipping, payment, review, confirm)
- Order History (list, detail)
- Authentication (login, register, profile)
- Category Navigation (tree, filter)
- Home / Landing

## Decision

Migrate in this order, smallest risk to highest:

| Phase | Slice | Rationale |
|-------|-------|-----------|
| 1 | Infrastructure | YARP, solution, DB connectivity — no business logic |
| 2 | Auth | Foundation for all authenticated features |
| 3 | Product Catalog | Read-only, no transactions, validates the full stack |
| 4 | Category Nav | Depends on catalog data being available |
| 5 | Shopping Cart | First write operations — session + customer carts |
| 6 | Checkout | Most complex flow — address, shipping, payment, order |
| 7 | Order History | Depends on checkout producing orders |
| 8 | Home / Landing | Aggregates featured products |
| 9 | Full E2E | Regression across all flows |
| 10 | Decommission | Remove legacy fallback, route all to SPA |

Each phase has a corresponding task range in the [Master Task List](../migration/master-task-list.md) and a checklist in the [Implementation Roadmap](../migration/implementation-roadmap.md).

## Consequences

**Positive:**
- Each phase delivers a user-visible feature.
- Early phases validate infrastructure without business risk.
- Dependencies are satisfied in order (checkout needs cart, cart needs auth).

**Negative:**
- Sequential execution — no parallel workstreams.
- Later phases cannot start until earlier phases are verified.

## Related

- [Master Task List](../migration/master-task-list.md)
- [Implementation Roadmap](../migration/implementation-roadmap.md)
- [Orchestration](../migration/orchestration.md)