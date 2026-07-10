# APPLICATION_BLUEPRINT.md — TeraWare

| Field | Value |
|---|---|
| Client | شركة حسين عطية للمقاولات |
| Application | TeraWare |
| Prepared By | ApplicationBlueprintAgent (مُهندس) |
| Date | 2026-07-09 |
| Source Handoff | `client-engagement/TERA_HANDOFF_PACKAGE.md` |

Self-Verification Gate:
- High confidence sections: [Application Overview, Confirmed Handoff Reference, Proposed Modules / Major Capabilities, Proposed Workflow Shape, Proposed Data / Entity Landscape, Risks and Constraints, Recommended Next Preparation Focus]
- Medium confidence sections: [Proposed User Roles / Operational Actors, Proposed Screen / Interface Landscape, Technical Decision Candidates, Open Questions]
- Low confidence sections: []
- Assumptions made: 4
- Gate result: PASS

---

## 1. Application Overview

TeraWare هو تطبيق مساعد داخلي/تشغيلي لشركة حسين عطية للمقاولات لمعالجة فجوة تشغيلية في موديول المستودعات داخل NatejSoft ERP المبني على Oracle APEX.

شكل الحل المقترح على مستوى الـ blueprint هو حل ثنائي المسار:
- مسار Desktop لتشغيل موظفي المستودعات والإدارة التشغيلية على Oracle مباشرة مع متصفح APEX معزز.
- مسار Web/Mobile لطلبات المواد مع حفظ مستقل في SQL Server ثم ربط يدوي مع طلبات Oracle ومتابعتها لاحقاً من النظام الأصلي.

---

## 2. Confirmed Handoff Reference

- Handoff package status: **Confirmed / Approved**
- Primary handoff file: `client-engagement/TERA_HANDOFF_PACKAGE.md`
- Supporting references used:
  - `client-engagement/CLIENT_INTAKE.md`
  - `client-engagement/DISCOVERY_COVERAGE_SUMMARY.md`
  - `client-engagement/FEATURE_LIST.md`
  - `client-engagement/DRAFT_QUOTATION.md`
  - `client-engagement/DOMAIN_RESEARCH_REPORT_ORACLE_APEX.md`

Confirmed upstream direction from handoff:
- Oracle remains the source system.
- SQL Server is a helper persistence layer, not a replacement ERP database.
- Material records are the only Oracle data area intended for direct update by the helper app.
- Material request lifecycle starts in TeraWare and is later linked manually to Oracle ERP.

---

## 3. Blueprint Status

| Item | Value |
|---|---|
| Current blueprint status | **draft** |
| Usable for formal preparation? | **No** |
| Blueprint Confirmation Gate state | pending_confirmation |
| Last gate record | 2026-07-09 — draft produced and awaiting Majed confirmation |

Confirmation question:

> هذا هو مخطط التطبيق المقترح بناءً على handoff المعتمد. هل توافق على استخدامه كأساس للتحضير التفصيلي؟

---

## 4. Proposed Modules / Major Capabilities

1. **Oracle Access & Material Control Module**
   - direct Oracle reads for reference and operational data
   - controlled Oracle updates for material records only

2. **Material Operations Desktop Hub**
   - material browsing
   - single edit / bulk edit
   - operational query screens
   - Excel export

3. **APEX Enhancement Browser Module**
   - embedded WebView2 host for Oracle APEX
   - JavaScript injection layer for labels, buttons, page augmentation, and selected DOM capture

4. **Material Request Web Portal**
   - request creation
   - request item selection with image preview
   - request list, details, print, cancel/reject

5. **SQL Server Helper Persistence Module**
   - material request headers and items
   - browser settings and captured operational snapshots
   - request-to-Oracle linking state

6. **Role Mapping & Access Module**
   - use Oracle user/permission source as the current operational identity basis

7. **Reporting & Export Module**
   - material lookup
   - warehouse balances
   - daily movement views
   - Excel exports

---

## 5. Proposed User Roles / Operational Actors

> Exact role inventory is not yet fixed in handoff; the following is a structured operational proposal.

- **[Assumption] Warehouse Operator / Storekeeper**
  - browses materials
  - reviews balances and movements
  - may perform permitted material updates

- **[Assumption] Inventory Controller / Materials Admin**
  - performs controlled single and bulk material edits
  - oversees query accuracy and helper data capture behavior

- **[Assumption] Project Requester / Site Engineer / Site Representative**
  - creates material requests from web/mobile
  - selects materials and quantities

- **[Assumption] Request Coordinator / Procurement Follow-up**
  - links helper requests to Oracle requests
  - changes request state to linked / cancelled / rejected

- **IT / System Support**
  - manages deployment, connection settings, and browser enhancement rollout

---

## 6. Proposed Workflow Shape

### A. Desktop material and query flow
1. User authenticates/enters operating context using Oracle-based identity assumptions.
2. Desktop app reads Oracle reference/master data directly.
3. User browses materials, images, balances, and movement views.
4. Authorized users update material data individually or in bulk.
5. Query outputs can be filtered and exported to Excel.

### B. APEX enhancement browser flow
1. WebView2 opens Oracle APEX ERP pages.
2. A global injected JavaScript layer identifies page context and enhances target pages.
3. Enhanced page behaviors may relabel controls, add buttons, or surface additional data.
4. Selected page data can be captured and stored in SQL Server for helper use.

### C. Material request flow
1. Requester opens TeraWare.Web from desktop/mobile.
2. Request header is created using Oracle-fed project/warehouse/reference data.
3. Request items are selected from Oracle material data with image preview.
4. Request is saved to SQL Server in helper state.
5. Request later appears in a “not linked” operational list.
6. Coordinator manually fills Oracle request number or marks request cancelled/rejected.
7. Downstream follow-up continues in NatejSoft ERP, not in TeraWare.

---

## 7. Proposed Screen / Interface Landscape

### Desktop interface landscape
- Startup / connection / operator context screen
- Materials explorer grid
- Material detail / single edit screen
- Bulk material update workspace
- Stock balances query screen
- Daily movements query screen
- Material lookup / quick detail screen
- Embedded Oracle APEX browser screen
- Browser capture / operational settings screen **[Assumption]**

### Web interface landscape
- Request list / status filter screen
- Create material request screen
- Material picker with image preview
- Request details / review screen
- Request print view
- Link / cancel / reject operational action screen or panel **[Assumption]**

### UX direction shape
- design starts from scratch
- must remain Arabic-first / RTL-friendly
- should preserve operational familiarity with current ERP concepts while reducing friction in material selection and request entry

---

## 8. Proposed Data / Entity Landscape

### Oracle-side entities (source system)
- Materials
- Sizes
- Material Groups
- Warehouses
- Projects
- Material Requests
- Users
- Permissions / role mapping
- Voucher Headers
- Voucher Lines
- Attachments / material images

### SQL Server-side helper entities
- MaterialRequestHeader
- MaterialRequestItem
- RequestLinkStatus / OracleRequestReference **[Assumption]**
- BrowserSettings
- CapturedVoucherData / SavedVoucherSnapshot **[Assumption]**
- RequestAuditTrail **[Assumption]**

### Entity relationship shape
- Oracle remains authoritative for inventory master/reference and ERP movement context.
- SQL Server becomes authoritative for helper-owned request lifecycle before Oracle linking.
- Linking between helper request and Oracle request should be explicit, traceable, and non-destructive.

---

## 9. Technical Decision Candidates

> No item below is a final technical decision. These are current candidates/recommendations based on handoff direction.

1. **Application split candidate**
   - Preferred direction: Desktop + Web dual application model
   - Reason: operational desktop use and external/mobile request entry have different interaction patterns

2. **Desktop framework candidate**
   - Preferred direction: WPF + WebView2
   - Reason: aligns with current handoff preference and embedded APEX enhancement needs

3. **Web application candidate**
   - Preferred direction: ASP.NET Core Razor Pages + Bootstrap RTL
   - Reason: form-heavy request experience, rapid internal operations delivery, mobile responsiveness

4. **Data access candidate**
   - Preferred direction: direct Oracle access via Oracle.ManagedDataAccess plus SQL Server helper persistence
   - Reason: upstream confirmed no replication/sync layer is desired

5. **Browser enhancement candidate**
   - Preferred direction: global injected JavaScript with page-aware branching using APEX page identity and URL patterns
   - Reason: multiple pages need enhancement without rewriting ERP screens

6. **Security/auth candidate**
   - Preferred direction: reuse Oracle-origin users/roles as the first access model
   - Open consideration: whether helper app should mirror roles only or enforce a separate mapped policy layer

7. **Request linking candidate**
   - Preferred direction: helper request stored first, then manual Oracle reference linking
   - Reason: avoids premature direct write into Oracle request workflow

8. **Support model candidate**
   - Preferred direction: initial delivery + operational follow-up + open bug-fix/development support policy
   - Open consideration: formal boundary between free defect resolution and paid change development

---

## 10. Risks and Constraints

1. **Direct Oracle update risk**
   - material updates must respect unknown constraints, triggers, and ERP side effects

2. **Oracle APEX variability risk**
   - actual APEX version, DOM structure, dialog behavior, and partial refresh behavior may differ from expectations

3. **Multi-surface operational consistency risk**
   - Desktop, Web, SQL Server helper state, and Oracle ERP tracking must remain semantically aligned

4. **Manual linking dependency**
   - helper request success depends on disciplined manual linking back to Oracle request records

5. **Unfixed role/count constraint**
   - exact user counts and role matrix remain undefined, which affects security modeling and screen permissions

6. **Image retrieval dependency**
   - material image availability depends on actual Oracle attachment structure and retrieval performance

7. **Open support expectation constraint**
   - “open support” wording may create downstream ambiguity unless formalized during preparation and contracting

---

## 11. Open Questions

Open questions exist and are tracked in:

`project-preparation/BLUEPRINT_OPEN_QUESTIONS.md`

Summary of major open items:
- exact Oracle fields allowed for material update
- exact helper request statuses and audit expectations
- exact user/role matrix across desktop and web
- APEX version and target page inventory for JS enhancement
- exact attachment/image retrieval structure and performance considerations
- post-delivery support boundary definition at preparation depth

---

## 12. Recommended Next Preparation Focus

1. **Oracle Schema Validation Pack**
   - confirm editable material fields, constraints, triggers, attachment structure, and request-link reference points

2. **Roles / Permissions Preparation**
   - produce a concrete role matrix for desktop and web actions

3. **Workflow & Request State Formalization**
   - define request statuses, link/cancel rules, and minimal audit trail expectations

4. **Screen Map & UI Structure Preparation**
   - convert the landscape above into a concrete screen map with fields/actions per screen

5. **APEX Technical Spike Planning**
   - validate WebView2 injection strategy on actual target pages before locking downstream technical preparation

6. **Support / Acceptance Preparation**
   - formalize go-live follow-up, defect support boundary, and acceptance criteria package
