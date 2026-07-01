# ENGINEERING_BEST_PRACTICES.md

## Tera Engineering Governance Layer — Best Practices

| Metadata | |
|---|---|
| **Status** | Active system policy |
| **Source** | Derived from Majed's `temp/Note01.md` and normalized for Tera governance |
| **Scope** | All future Tera-managed applications, adjusted by project size and technology profile |
| **Last Updated** | 2026-07-01 |

---

## 1. Purpose

This file defines Tera's engineering ideals for maintainable application code, architecture, and delivery.

The goal is to prevent applications from becoming:

- oversized and hard to change;
- randomly structured;
- tightly coupled;
- UI-heavy with hidden business logic;
- weak in validation, permissions, testing, or error handling;
- dependent on AI-generated code without governance.

This policy is not a license to over-engineer. It must be applied proportionally to the project size, domain risk, approved scope, and active Technology Profile.

---

## 2. Core Principle

```text
Every application must be structured for safe change.
```

Safe change means:

- modules are clear;
- responsibilities are separated;
- files remain reviewable;
- business rules are not hidden in UI;
- validation, permissions, errors, and tests are consistent;
- database and API changes are traceable;
- AI agents cannot expand or degrade architecture silently.

---

## 3. Project Size Levels

| Level | Use When | Engineering Governance Depth |
|---|---|---|
| **Compact** | small CRUD / prototype / internal utility | Basic modules, file-size awareness, no random shared/utils, minimal tests for important rules |
| **Standard** | typical web app with dashboard, API, auth, database | Feature/module structure, service/use-case separation where useful, validation/error/security rules, task-level review |
| **Full** | ERP, SaaS, multi-role, workflow-heavy, high data/security risk | Layered architecture, domain/application/infrastructure separation, audit logs, integration boundaries, stronger testing and review |

Tera decides the level during project preparation and records it in the active application workspace, normally in `project-preparation/08_TECHNICAL_ARCHITECTURE.md` or a project-specific engineering standards file.

---

## 4. Structure by Responsibility

Prefer module/feature-oriented structure over random flat folders.

Recommended conceptual structure:

```text
src/
  core/
    config/
    errors/
    logger/
    security/
    database/
  shared/
    components/
    utils/
    validators/
    types/
    constants/
  modules/
    [module-name]/
      ui/
      application/
      domain/
      infrastructure/
      tests/
```

Rules:

- Every important business feature belongs to an explicit module.
- `shared/` contains only code used by more than one module.
- Module-specific rules must not be moved into `shared/` merely for convenience.
- Technology-specific folder naming may differ, but responsibility boundaries must remain clear.

---

## 5. File Size and Responsibility

Any file approaching roughly `300–400` lines should be reviewed for responsibility creep.

Large files are not automatically forbidden, but they are suspicious when they combine:

- data fetching;
- validation;
- error handling;
- UI rendering;
- permissions;
- formatting;
- exports;
- notifications;
- database operations.

Rule:

```text
If a file cannot state its responsibility in one sentence, it must be reviewed.
```

---

## 6. Single Responsibility

Each file, function, component, class, service, or use case should have one primary reason to change.

Bad signal:

```text
createInvoiceAndValidateCustomerAndUpdateStockAndSendEmail()
```

Better pattern:

```text
validateCustomer()
createInvoice()
reserveStock()
sendInvoiceEmail()
```

An application service or use case may coordinate multiple operations, but should not hide every rule inside one large function.

---

## 7. UI and Business Logic Separation

UI receives input and displays output. It must not become the owner of business rules.

Business logic belongs in one of these areas according to stack and project size:

```text
application/
domain/
services/
use-cases/
rules/
validators/
repositories/
```

For example, invoice tax calculation, stock reservation, approval rules, customer credit checks, and permission checks must not be buried inside page components.

---

## 8. DRY With Judgment

Do not extract every small duplicate too early.

| Repetition | Decision |
|---:|---|
| once | leave it |
| twice | watch it |
| three times | usually extract |
| more than three | extract unless there is a clear reason not to |

Good shared candidates:

- date/currency formatting;
- common validation;
- API response helpers;
- pagination;
- current-user helpers;
- permission helpers;
- logging;
- upload/export utilities.

Forbidden pattern:

```text
shared/utils/ becomes a dumping ground.
```

---

## 9. Validation

Validation must not exist only in UI.

Rules:

- UI validation improves user experience.
- Backend/API/service validation protects the system.
- Common validators belong in shared validation only when genuinely shared.
- Module-specific validation stays inside the module.
- Database constraints are for data integrity; do not move business validation into database constraints unless explicitly approved.

---

## 10. Error Handling

Applications must use consistent error handling instead of ad-hoc messages everywhere.

Recommended concepts:

```text
core/errors/
  app-error
  validation-error
  authorization-error
  not-found-error
  error-handler
```

User-facing errors should be consistent and should not expose internal technical details.

---

## 11. Permissions and Security Enforcement

Do not rely on hiding UI buttons as security.

Permissions must be enforced where relevant:

- frontend for UX;
- backend/API/server actions for actual enforcement;
- service/domain layer where business-sensitive rules require it;
- database policies only when explicitly approved for the stack and deployment model.

Examples:

```text
canApproveInvoice(user, invoice)
canDeleteCustomer(user, customer)
canViewCost(user)
```

---

## 12. Configuration and Secrets

Configuration must be centralized and environment-aware.

Rules:

- Do not scatter URLs, sizes, currencies, or flags across the codebase.
- Real secrets must never be written into source code, project-control records, task files, logs, handbacks, or fallback config values.
- Use `.env`, environment variables, or approved local secret storage only.

---

## 13. Logging and Audit

Do not log everything, but important actions must be traceable.

Relevant events include:

- failed login attempts;
- deletes;
- approvals;
- permission changes;
- sensitive business operations;
- API errors;
- integration failures;
- system exceptions.

For ERP and administration-heavy systems, Audit Log is often essential.

---

## 14. Database Design and Migrations

Database design must be traceable and consistent.

Rules:

- names are clear and consistent;
- relations use explicit IDs / foreign keys where supported;
- indexes exist for important search/filter paths;
- status values are explicit enums or reference records;
- created/updated metadata exists when useful;
- soft delete is considered for sensitive administrative data;
- schema changes use migrations or the approved stack-specific equivalent;
- no manual production database changes without trace.

---

## 15. API Design

APIs should be predictable and consistent.

Rules:

- endpoint naming is stable;
- responses have a consistent shape;
- errors have a consistent format;
- pagination/filtering/sorting are explicit when needed;
- versioning is used only when needed;
- API changes must not silently break planned consumers.

---

## 16. Testing

Testing must focus on important risk, not test-count vanity.

Default priorities:

- Unit tests for business rules and calculations.
- Integration/API tests for workflows and endpoints.
- UI/E2E tests only where they provide real confidence and cost is justified.

Examples that often need tests:

- permissions;
- approvals/status transitions;
- invoice/tax/stock calculations;
- validation rules;
- deletion prevention;
- negative quantity prevention;
- critical API behavior.

---

## 17. Documentation and Decisions

Projects do not need bloated documentation, but they need governing documentation.

Recommended project-level documents when justified:

```text
ARCHITECTURE.md
CODING_STANDARDS.md
DATABASE_GUIDELINES.md
API_GUIDELINES.md
TESTING_GUIDELINES.md
SECURITY_GUIDELINES.md
DECISIONS.md
CHANGELOG.md
```

Within Tera-managed projects, these may be represented by project-preparation and project-control files instead of literal docs, unless the application needs standalone engineering docs.

---

## 18. Git and CI/CD

Rules:

- commits should be small and understandable;
- one commit should not mix unrelated topics;
- never commit secrets or local temporary files;
- at minimum, important projects should support lint/test/build checks before release;
- CI/CD complexity must match project size.

---

## 19. Performance

Avoid premature optimization, but prevent obvious structural performance mistakes.

Watch for:

- screens loading thousands of records without pagination;
- missing indexes on common filters;
- heavy reports without bounded queries;
- unnecessary data fetching;
- unoptimized images;
- expensive operations hidden in UI rendering.

---

## 20. AI Governance

AI-generated work must be constrained by official project files and task scope.

Agents must:

1. understand the approved structure before changing code;
2. not create duplicate files or folders;
3. not edit outside the task scope;
4. not break APIs silently;
5. not change database structure without a schema/migration task;
6. add tests when important logic changes;
7. update or request documentation updates when behavior changes;
8. report exactly what files were changed.

---

## 21. The Ten Non-Negotiables

1. Use clear modules.
2. Separate business logic from UI.
3. Prevent oversized files.
4. Extract shared code carefully, not blindly.
5. Test important business logic.
6. Enforce permissions beyond the frontend.
7. Use consistent error handling.
8. Record important architecture decisions.
9. Use migrations or traceable database changes.
10. Govern AI-generated code with task scope, gates, and review.
