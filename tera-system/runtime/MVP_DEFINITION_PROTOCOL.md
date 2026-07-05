# MVP Definition & Phased Roadmap Protocol

## 1. Purpose

This protocol defines how Tera classifies application features after discovery and before MVP scope definition.

It prevents feature-bloat in MVP by enforcing a mandatory classification step between Intake Collection and Project Preparation.

---

## 2. The Golden Rule

```text
Discovery collects possibilities.
MVP includes only what is required for the smallest useful approved version.
Everything else must be classified into Extended MVP / Phase 2 / Phase 3 / Later / Out of Scope.
```

**User-selected features during discovery are not automatically MVP.**
Tera must classify each feature by necessity, dependency, risk, cost, and implementation size before placing it in MVP.

---

## 3. Classification Tiers

| Tier | Label | Definition |
|---|---|---|
| **Core MVP** | Phase 1A | The smallest version that delivers the primary workflow end-to-end. Without this feature, the app cannot function for its core purpose. |
| **Extended MVP** | Phase 1B | Important additions that significantly improve usability but can be added after Core MVP is stable. Users can start using Core MVP without them. |
| **Phase 2** | — | Meaningful improvements that depend on Core MVP data and workflows being operational first. |
| **Phase 3** | — | Advanced capabilities, integrations, or automations. Higher complexity, lower urgency. |
| **Later / Enterprise** | — | Expensive, complex, or enterprise-specific features. Deferred unless explicitly required for the target deployment. |
| **Out of Scope** | — | Not part of the project vision. Documented to prevent future scope creep. |

---

## 4. Mandatory Classification Criteria

For every feature collected during discovery, Tera must ask these questions in order:

| # | Question | If Yes → | If No → |
|---|---|---|---|
| 1 | Is this feature essential for the primary workflow to function? | **Core MVP** candidate | Ask #2 |
| 2 | Does the app still deliver its core value without this feature? | Ask #3 | **Core MVP** |
| 3 | Can this feature be added safely in weeks without rework? | **Extended MVP** candidate | Ask #4 |
| 4 | Does this feature depend on Core MVP data/workflows being stable? | **Phase 2** candidate | Ask #5 |
| 5 | Is this feature high complexity but low urgency? | **Phase 3** candidate | Ask #6 |
| 6 | Is this feature enterprise-only, expensive, or rarely used? | **Later / Enterprise** | **Out of Scope** |

### Additional weighting factors:
- **Necessity**: Does the primary actor need this to complete their goal?
- **Dependency**: Does this feature depend on another feature that is not yet built?
- **Risk**: Would deferring this cause technical debt or rework?
- **Cost**: How many screens, tables, and workflows does this feature require?
- **Implementation Size**: Can this be built in days or weeks?

---

## 5. Classification Process

This process runs AFTER enough discovery information is collected, BEFORE the MVP scope is finalized in `project-inputs/01_APPLICATION_IDEA.md`.

```text
1. Collect all candidate features from discovery.
2. For each feature, run the 6-question classification (Section 4).
3. Assign each feature to one of the 6 tiers.
4. Review the Core MVP list:
   - Is it the smallest version that delivers the core workflow?
   - Can a user complete their primary goal without leaving the app?
   - Can the Core MVP be built and delivered in a reasonable timeframe?
   - If Core MVP is still too large, reclassify borderline features downward.
5. Document the classified list in project-inputs/01_APPLICATION_IDEA.md
   under the appropriate sub-sections (Core MVP, Extended MVP, etc.).
6. Present the classified list to the user in the Application Understanding Summary.
7. The user reviews and approves/reclassifies before Tera proceeds to phased roadmap.
```

**The user may override classifications, but Tera must not default to putting features in Core MVP because the user mentioned them first.**

---

## 6. Relationship with Phased Roadmap

The classified features feed directly into the Phased Application Roadmap:

| Classification | Maps to Roadmap Section |
|---|---|
| Core MVP (Phase 1A) | Phase 1 / MVP |
| Extended MVP (Phase 1B) | Phase 1 / MVP (with note: "Extended — can follow Core") |
| Phase 2 | Phase 2 |
| Phase 3 | Phase 3 |
| Later / Enterprise | Later / Enterprise |
| Out of Scope | Out of scope |

---

## 7. Example: MRMS Feature Classification

This example shows how the protocol applies to the Material Request Management System.

| Feature | Classification | Rationale |
|---|---|---|
| Login + Roles (Employee, Manager, Admin) | **Core MVP** | Required for any workflow |
| Departments + Users basic management | **Core MVP** | Required to assign managers and route approvals |
| Material Catalog (add/list basic items) | **Core MVP** | Employee must select materials to request |
| Create material request (from catalog + manual) | **Core MVP** | The primary workflow |
| Manager approval (accept/reject) | **Core MVP** | The secondary workflow — without it no request is fulfilled |
| Senior management approval | **Core MVP** | Required by user as mandatory approval step |
| Request status tracking (Pending/Approved/Rejected) | **Core MVP** | User must know request outcome |
| Responsive Web (desktop + mobile browser) | **Core MVP** | The only delivery platform for Phase 1 |
| Arabic + English (RTL/LTR) | **Core MVP** | Required for both user groups |
| File attachments on requests | **Extended MVP** | Important but requests can be created and approved without files |
| In-app notifications (bell icon, notification list) | **Extended MVP** | Users can check request status manually; notifications improve UX |
| Dashboard with basic stats | **Extended MVP** | Useful but not blocking; users can use request list directly |
| Inventory management (quantities, stock tracking) | **Phase 2** | Depends on Core MVP being stable; adds warehouse workflow |
| Email notifications | **Phase 2** | Requires email service setup; in-app is sufficient initially |
| Low stock alerts | **Phase 2** | Depends on inventory management being active |
| Export to Excel/PDF | **Phase 2** | Useful but not core; manual reporting works initially |
| PWA + Push Notifications | **Phase 3** | Requires PWA service worker; adds complexity |
| Approval routing by request value | **Phase 3** | Advanced workflow rule; additional configuration |
| Advanced analytics dashboard | **Phase 3** | Depends on accumulated data from Core MVP |
| Audit log (detailed) | **Phase 3** | Phase 2 simple logging is sufficient |
| Native mobile app | **Later** | Expensive; PWA covers mobile needs initially |
| ERP integration | **Later** | Enterprise-only; not in current scope |
| Enforced budget limits per department | **Later** | Enterprise/complex; reporting-only is sufficient initially |
| External procurement/purchase orders | **Out of Scope** | Not part of this system's vision |

---

## 8. Protocol Enforcement

- This protocol activates **automatically** whenever Tera enters Intake Collection Mode and collects candidate features.
- Tera **must not** write a finalized `MVP Scope` in `01_APPLICATION_IDEA.md` without first running the classification process.
- Tera **must** reference this protocol in the Application Discovery section of `.opencode/agents/tera.md`.
- The classification **must** be visible to the user in the Application Understanding Summary.
- The user may reclassify, but the default is Tera's analysis-based classification, not automatic MVP inclusion.

---

## 9. References

- `.opencode/agents/tera.md` — Section 13: Application Discovery & Intake Dialogue
- `project-inputs/01_APPLICATION_IDEA.md` — Section 7: MVP Scope (classified)
- `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — Section 6: Application Discovery Protocol
- `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` — Section: MVP Definition Classification Checklist
- `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` — Template: Application Understanding Summary, Phased Application Roadmap
