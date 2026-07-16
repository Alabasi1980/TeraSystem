# QUAUD Report — TASK-COD-TABLEMAPPINGS-UI

```text
Audit ID:          QUAUD-TASK-COD-TABLEMAPPINGS-UI-2026-07-16-001
Task Reviewed:     TASK-COD-TABLEMAPPINGS-UI
Invoked By:        Tera
Audit Mode:        Standard (UI change — code quality + UI accessibility + testing adequacy)
Scope:             Changed Code — Index.cshtml, Index.cshtml.cs, table-mapping-wizard.js, AddNameToTableMappings migration
Report Path:       project-control/audit-reports/QUAUD-TASK-COD-TABLEMAPPINGS-UI-2026-07-16-001.md
Evidence Sources:  Index.cshtml (1070 lines), Index.cshtml.cs (390 lines), table-mapping-wizard.js (1045 lines), AddNameToTableMappings.cs (37 lines), QUALITY_GATE_THRESHOLDS.md, ENGINEERING_REVIEW_CHECKLIST.md
```

---

## Overall Quality Gate: NEEDS_FIX

No STOP findings. Multiple CAUTION findings require resolution before acceptance.

```text
Findings Summary:
- STOP:      0
- CAUTION:   5
- FLAG:      8
- BASELINE_DEBT: 0
```

---

## Findings

### Finding 1

```text
Finding ID:           F-A1
Rule ID:              QG-ACC-001
Domain:               UI Accessibility
Severity:             CAUTION
Location:             Index.cshtml, lines 522-530 (toggle checkbox in table row)
Evidence:             <label class="wd-toggle"> wraps checkbox + slider span but contains NO text content.
                      Screen readers announce the control but cannot identify what the toggle does.
Expected Standard:    Interactive controls must have an accessible name (WCAG 4.1.2).
Observed Condition:   The <label> wrapping the toggle contains only a hidden submit input, the checkbox,
                      and a visual <span class="wd-toggle__slider">. No visible or visually-hidden text
                      describes the toggle's purpose (e.g., "تفعيل/تعطيل").
Impact:               Screen reader users cannot determine what the toggle controls.
Recommended Action:   Add visually-hidden text inside the label: <span class="sr-only">تفعيل/تعطيل</span>
                      or use aria-label on the checkbox.
Changed Code / Baseline: NEW — introduced in this diff as part of the table toggle column.
Confidence:           High
Blocking:             No
Blocking Reason:      CAUTION — significant but not critical; admin-only page.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task
Referral:             None
Status:               Open
```

### Finding 2

```text
Finding ID:           F-A2
Rule ID:              QG-ACC-002
Domain:               UI Accessibility
Severity:             CAUTION
Location:             Index.cshtml, lines 546-551 (error badge in table row)
Evidence:             <span class="wd-badge wd-badge--error" title="@m.ErrorMessage" style="cursor:pointer;">
                      has cursor:pointer suggesting interactivity, but is a <span> — not keyboard-focusable,
                      no role="button", no tabindex="0".
Expected Standard:    Elements styled as interactive must be keyboard-accessible (WCAG 2.1.1).
Observed Condition:   The error badge looks clickable (pointer cursor, title tooltip) but cannot be
                      reached or activated via keyboard.
Impact:               Keyboard-only users cannot access the error message tooltip.
Recommended Action:   Either (a) change to <button type="button"> with aria-label showing the error,
                      or (b) remove cursor:pointer if the element is not intended to be interactive.
Changed Code / Baseline: NEW — SVG error badge replacing emoji, introduced in this diff.
Confidence:           High
Blocking:             No
Blocking Reason:      CAUTION — affects keyboard users on admin page.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task
Referral:             None
Status:               Open
```

### Finding 3

```text
Finding ID:           F-A3
Rule ID:              QG-ACC-003
Domain:               UI Accessibility
Severity:             CAUTION
Location:             Index.cshtml + table-mapping-wizard.js (wizard modal overlay, lines 657-906 in cshtml,
                      wireKeyboard at lines 905-916 in wizard.js)
Evidence:             The wizard modal opens via .is-open class on #wm-overlay. The keyboard handler only
                      captures Escape to close. No focus trap is implemented — keyboard users can Tab
                      out of the modal into the background page content. When modal closes, focus is not
                      restored to the triggering element.
Expected Standard:    Modal dialogs should trap focus and restore focus on close (WAI-ARIA Authoring Practices).
Observed Condition:   No focus trap. Tab key leaves the modal. No focus restoration on close.
Impact:               Keyboard and screen reader users can lose context by tabbing into background content.
Recommended Action:   Implement a focus trap (cycle Tab/Shift+Tab within the modal) and restore focus
                      to the triggering button on close.
Changed Code / Baseline: EXISTING — the modal existed before this diff; this diff expanded it to 5 steps.
                      The focus trap gap is baseline, but the modal is materially changed by this diff.
Confidence:           High
Blocking:             No
Blocking Reason:      CAUTION — significant UX gap for keyboard users, but admin-only context.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task
Referral:             None
Status:               Open
```

### Finding 4

```text
Finding ID:           F-S3
Rule ID:              QG-DB-001
Domain:               Migration Integrity
Severity:             CAUTION
Location:             AddNameToTableMappings.cs, Down() method (lines 26-35)
Evidence:             Down() calls DropIndex("IX_TableMappings_Name") but Up() never creates this index.
                      The Up() method comment explicitly states: "a unique index on a max-length column
                      is not supported, so the Name uniqueness constraint is enforced at the application
                      layer." The index IX_TableMappings_Name does not exist in Up().
Expected Standard:    Migration Down() must be the inverse of Up(). Every operation in Down() should
                      correspond to an operation in Up().
Observed Condition:   Down() will fail at runtime because it tries to drop a non-existent index.
Impact:               EF Core migration rollback (Down) will throw an exception. Database cannot be
                      rolled back to the previous state without manual intervention.
Recommended Action:   Remove the DropIndex call from Down() since the index was never created.
                      Alternatively, if the index is created elsewhere, document the dependency.
Changed Code / Baseline: NEW — introduced in this diff.
Confidence:           High
Blocking:             No
Blocking Reason:      CAUTION — rollback failure is significant but does not affect forward migration
                      or normal operation.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task
Referral:             None
Status:               Open
```

### Finding 5

```text
Finding ID:           F-T1
Rule ID:              QG-TST-001
Domain:               Testing Adequacy
Severity:             CAUTION
Location:             Task scope — all changed files
Evidence:             No test files, QA reports, or test evidence were provided in the delegation package.
                      The diff introduces: new Name field with validation/uniqueness, new SyncMode and
                      IncrementalColumn fields, new wizard step 5, new backend OnGetMappingAsync endpoint,
                      and a database migration. All of these change observable behavior.
Expected Standard:    Behavior-changing code should have test evidence or a documented reason for deferral
                      (ENGINEERING_REVIEW_CHECKLIST §8).
Observed Condition:   Zero test artifacts provided. No unit tests for ValidateInput, uniqueness checks,
                      or OnGetMappingAsync. No integration tests for the migration. No QA report.
Impact:               Cannot verify correctness of validation logic, uniqueness enforcement, or
                      migration behavior without test evidence.
Recommended Action:   Either (a) provide QA/test evidence covering: Name validation, uniqueness (Name,
                      OracleSource, SqlTargetTable), toggle active/inactive, delete confirmation,
                      sync mode selection, and migration Up/Down; or (b) document explicit deferral
                      with a tracking issue.
Changed Code / Baseline: NEW — all changes in this diff are untested.
Confidence:           High
Blocking:             No
Blocking Reason:      CAUTION — no test evidence, but admin-only page with manual verification
                      possible. Deferral with tracking is acceptable.
Waiver Allowed:       Yes
Required Owner:       Tera → decision: provide tests or defer with record
Referral:             QA Agent
Status:               Open
```

### Finding 6

```text
Finding ID:           F-A6
Rule ID:              QG-ACC-006
Domain:               UI Accessibility
Severity:             FLAG
Location:             Index.cshtml, lines 736-739 (Step 2 query editor)
Evidence:             <label>استعلام SQL</label> is NOT associated with the textarea via for/id.
                      The textarea has id="wm-query-editor" but the label lacks for="wm-query-editor".
Expected Standard:    Form labels should be associated with their controls via for/id (WCAG 1.3.1).
Observed Condition:   The label is visually near the textarea but not programmatically linked.
Impact:               Screen readers may not announce the label when the textarea is focused.
Recommended Action:   Add for="wm-query-editor" to the label element.
Changed Code / Baseline: EXISTING — this label existed before the diff.
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — advisory; does not block PASS.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task (follow-up)
Referral:             None
Status:               Open
```

### Finding 7

```text
Finding ID:           F-A7
Rule ID:              QG-ACC-007
Domain:               UI Accessibility
Severity:             FLAG
Location:             Index.cshtml — multiple SVG elements (lines 459-475, 549, 700-713, 848-858, 912)
Evidence:             Decorative SVGs (empty state illustration, source type card icons, toast icons,
                      error badge icon) lack aria-hidden="true". Screen readers may attempt to traverse
                      or announce SVG internals.
Expected Standard:    Decorative images should be hidden from assistive technology (WCAG 1.1.1).
Observed Condition:   SVGs are purely decorative (paired with text or illustrative) but not marked
                      as aria-hidden.
Impact:               Minor — screen readers may announce meaningless SVG path data.
Recommended Action:   Add aria-hidden="true" to all decorative SVG elements.
Changed Code / Baseline: MIXED — some SVGs are NEW (error badge, toast icons, empty state), some EXISTING
                      (source type cards in wizard).
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — advisory.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task (follow-up)
Referral:             None
Status:               Open
```

### Finding 8

```text
Finding ID:           F-A9
Rule ID:              QG-ACC-009
Domain:               UI Accessibility
Severity:             FLAG
Location:             Index.cshtml, lines 722-723 (Step 2 search input)
Evidence:             <input type="text" class="wm-search-input" id="wm-search-input" placeholder="..." />
                      relies on placeholder text for labeling. No associated <label> element.
                      Placeholder text is not a reliable accessible name (WCAG 1.3.1, 3.3.2).
Expected Standard:    Form inputs should have associated labels; placeholders are supplementary only.
Observed Condition:   Search input has no label, only a placeholder with emoji + Arabic text.
Impact:               Screen readers may not announce a meaningful label for the search field.
Recommended Action:   Add a visually-hidden label: <label for="wm-search-input" class="sr-only">بحث</label>
Changed Code / Baseline: EXISTING — search input existed before the diff.
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — advisory.
Waiver Allowed:       Yes
Required Owner:       Tera → implementation task (follow-up)
Referral:             None
Status:               Open
```

### Finding 9

```text
Finding ID:           F-S1
Rule ID:              QG-SEC-010
Domain:               Security
Severity:             FLAG
Location:             Index.cshtml, line 560 (edit button onclick)
Evidence:             @JsonSerializer.Serialize(m.Name) is used inside a single-quoted onclick attribute.
                      Razor HTML-encodes the output (double quotes to &amp;quot;, single quotes to
                      &amp;#39;), making it safe in the current context. Values originate from the
                      database (admin-only page, not public user input).
Expected Standard:    Inline JS with user-derived data should use a safe serialization/encoding strategy.
Observed Condition:   The pattern is safe in the current context (Razor encoding + database data + admin page),
                      but relies on the combination of Razor HTML encoding + JSON serialization.
                      If the context changes (e.g., moved to a non-Razor context), safety guarantees change.
Impact:               Low in current context. Fragile pattern that could become unsafe if refactored
                      without understanding the encoding chain.
Recommended Action:   Consider using data-* attributes + JS dataset reading for robustness. Not urgent
                      for this diff.
Changed Code / Baseline: NEW — expanded parameters in onclick for edit button.
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — safe in current context, advisory for future robustness.
Waiver Allowed:       Yes
Required Owner:       Tera → follow-up consideration
Referral:             None
Status:               Open
```

### Finding 10

```text
Finding ID:           F-D1
Rule ID:              QG-HEUR-001
Domain:               Code Quality — File Size
Severity:             FLAG
Location:             Index.cshtml (1070 lines total: ~437 lines CSS + ~220 lines HTML + ~150 lines inline JS)
Evidence:             File exceeds QUALITY_GATE_THRESHOLDS Caution candidate (500+ lines). However, the
                      file is a Razor page with embedded CSS, HTML template, and inline JS — a common
                      pattern in ASP.NET Razor Pages.
Expected Standard:    Files approaching 500+ lines should be reviewed for responsibility creep
                      (QUALITY_GATE_THRESHOLDS §4).
Observed Condition:   1070 lines. CSS (~40%), HTML (~20%), inline JS (~15%), Razor logic (~25%).
                      Responsibilities are coherent (single admin page).
Impact:               Maintainability — large files are harder to navigate. But the content is
                      structurally coherent as a single Razor page.
Recommended Action:   Consider extracting CSS to a dedicated stylesheet and inline JS to a page-specific
                      script file in a follow-up. Not blocking for this diff.
Changed Code / Baseline: GROWTH — file grew from ~800 to ~1070 lines in this diff (added Step 5,
                      sync mode CSS, mapping name input, summary row).
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — exceeds threshold but coherent; not a responsibility-creep failure.
Waiver Allowed:       Yes
Required Owner:       Tera → follow-up refactoring
Referral:             None
Status:               Open
```

### Finding 11

```text
Finding ID:           F-D2
Rule ID:              QG-HEUR-001
Domain:               Code Quality — File Size
Severity:             FLAG
Location:             table-mapping-wizard.js (1045 lines)
Evidence:             File exceeds QUALITY_GATE_THRESHOLDS Caution candidate (500+ lines). It contains
                      a single class (TableMappingWizard) with all wizard logic in one file.
Expected Standard:    Files approaching 500+ lines should be reviewed for responsibility creep.
Observed Condition:   1045 lines. Single class with clear single responsibility (wizard controller).
                      Methods are well-named and focused. No unrelated responsibilities mixed in.
Impact:               Maintainability — large single file. But the class is cohesive.
Recommended Action:   Consider splitting into modules (e.g., WizardState, WizardUI, WizardAPI) in a
                      follow-up. Not blocking for this diff.
Changed Code / Baseline: GROWTH — file grew with new SyncMode card wiring, loadDateColumns, etc.
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — exceeds threshold but cohesive class.
Waiver Allowed:       Yes
Required Owner:       Tera → follow-up refactoring
Referral:             None
Status:               Open
```

### Finding 12

```text
Finding ID:           F-D3
Rule ID:              QG-HEUR-005
Domain:               Code Quality — Duplication
Severity:             FLAG
Location:             Index.cshtml.cs — OnPostAddAsync (lines 91-146) and OnPostEditAsync (lines 148-206)
Evidence:             Both methods share the same structure: ValidateInput → uniqueness checks for Name,
                      OracleSource, SqlTargetTable → execute operation → reload → return Page().
                      The three uniqueness-check blocks are structurally identical (6 total instances).
Expected Standard:    Repeated business logic should be extracted when practical repetition is clear
                      (ENGINEERING_REVIEW_CHECKLIST §5).
Observed Condition:   The uniqueness check pattern is repeated 6 times across 2 methods. A helper
                      method like CheckUniquenessAsync(excludeId?) could reduce this.
Impact:               Maintenance — changes to uniqueness logic require updates in multiple places.
Recommended Action:   Extract a private helper: async Task<(bool exists, string field, string value)>
                      CheckUniquenessAsync(int? excludeId = null). Not blocking for this diff.
Changed Code / Baseline: EXISTING pattern — the duplication existed before but was extended with Name
                      uniqueness in this diff.
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — suspected duplication; no analyzer percentage.
Waiver Allowed:       Yes
Required Owner:       Tera → follow-up refactoring
Referral:             None
Status:               Open
```

### Finding 13

```text
Finding ID:           F-D4
Rule ID:              QG-HEUR-005
Domain:               Code Quality — Duplication
Severity:             FLAG
Location:             Index.cshtml inline JS (lines 997-1021, syncSyncSettingsUI) and
                      table-mapping-wizard.js (lines 766-797, setSyncMode)
Evidence:             Both functions update: (1) sync mode card selection classes,
                      (2) incremental options visibility, (3) sync mode summary text.
                      The inline version is called from the overridden goToStep in the inline script.
Expected Standard:    Duplicate logic between inline and external JS should be consolidated.
Observed Condition:   Two separate implementations of the same UI update logic. Changes to one
                      won't automatically apply to the other.
Impact:               Maintenance risk — sync settings UI logic diverges between two locations.
Recommended Action:   Remove the inline syncSyncSettingsUI() and call wizard.setSyncMode() from the
                      overridden goToStep instead. Not blocking for this diff.
Changed Code / Baseline: NEW — the inline function was introduced in this diff; the wizard.js method
                      is EXISTING.
Confidence:           High
Blocking:             No
Blocking Reason:      FLAG — suspected duplication.
Waiver Allowed:       Yes
Required Owner:       Tera → follow-up refactoring
Referral:             None
Status:               Open
```

### Finding 14

```text
Finding ID:           F-B3
Rule ID:              QG-CODE-001
Domain:               Code Quality — Dead Code
Severity:             FLAG
Location:             Index.cshtml.cs, OnGetMappingAsync (lines 68-89)
Evidence:             OnGetMappingAsync(int id) is defined but appears unused in the current flow.
                      The edit button in Index.cshtml passes data via inline JsonSerializer.Serialize
                      in the onclick attribute, not via this endpoint. The wizard JS bootstrapEditMode
                      receives data from the inline JS call, not from a fetch to this endpoint.
Expected Standard:    Dead or unused code should be removed or documented as intentional.
Observed Condition:   The endpoint is defined but no JS code or form action calls it.
Impact:               Minor — dead code adds maintenance surface. May have been intended for a
                      future API-based edit flow.
Recommended Action:   Either (a) remove the unused endpoint, or (b) document it as planned for a
                      future refactor (e.g., replacing inline onclick with AJAX edit loading).
Changed Code / Baseline: NEW — introduced in this diff but not wired into the UI flow.
Confidence:           Medium — possible that a future step was planned but not yet connected.
Blocking:             No
Blocking Reason:      FLAG — dead code, not harmful.
Waiver Allowed:       Yes
Required Owner:       Tera → decide: remove or document as planned
Referral:             None
Status:               Open
```

---

## Positive Observations

1. **EF Core parameterized queries** — All database operations use LINQ/EF Core, which provides parameterized queries. No raw SQL concatenation with user input in the backend.
2. **Backend validation exists** — ValidateInput checks Name, OracleSource, SqlTargetTable, and SourceType. Not frontend-only validation.
3. **Uniqueness enforcement** — Name, OracleSource, and SqlTargetTable are checked for uniqueness in both Add and Edit operations.
4. **Escape function in JS** — table-mapping-wizard.js includes a proper `escapeHtml()` helper used when rendering dynamic content (lines 29-33).
5. **Toast uses role="status"** — The server-rendered toast (line 911) correctly uses `role="status"` for screen reader announcements.
6. **Step validation** — The wizard validates each step before advancing (validateCurrentStep in wizard.js).
7. **Confirm dialogs use native browser confirm** — Accessible to screen readers by default.
8. **Migration backfills data** — The UPDATE statement in Up() ensures existing records get a Name value from OracleSource.
9. **CSS uses CSS custom properties** — Consistent with design system variables (--c-*, --radius-*, --shadow-*).
10. **Sync mode architecture** — Step 5 (Sync Settings) is well-structured with clear Full/Incremental mode cards and conditional incremental column selection.

---

## Handback to Orchestrator

```text
Status:                NEEDS_FIX
Report Path:           project-control/audit-reports/QUAUD-TASK-COD-TABLEMAPPINGS-UI-2026-07-16-001.md
Blocking Findings:     0 STOP (none)

CAUTION Findings (5):
  1. F-A1 — Toggle checkbox missing accessible name (UI Accessibility)
  2. F-A2 — Error badge <span> styled as interactive without keyboard support (UI Accessibility)
  3. F-A3 — Wizard modal missing focus trap (UI Accessibility — baseline, modal materially changed)
  4. F-S3 — Migration Down() drops non-existent index — rollback will fail (Migration Integrity)
  5. F-T1 — No test evidence provided for behavior-changing code (Testing Adequacy)

FLAG Findings (8):
  6. F-A6 — Step 2 query label not associated via for/id
  7. F-A7 — Decorative SVGs missing aria-hidden="true"
  8. F-A9 — Search input relies on placeholder only
  9. F-S1 — JsonSerializer.Serialize in onclick — safe but fragile pattern
  10. F-D1 — Index.cshtml at 1070 lines exceeds Caution threshold
  11. F-D2 — table-mapping-wizard.js at 1045 lines exceeds Caution threshold
  12. F-D3 — Suspected duplication in uniqueness check patterns (cshtml.cs)
  13. F-D4 — Suspected duplication between inline JS and wizard.js
  14. F-B3 — OnGetMappingAsync appears unused (dead code)

Recommended Next Actions:
  1. Fix F-S3: Remove DropIndex from migration Down() — this is a correctness bug.
  2. Fix F-A1: Add accessible name to toggle checkbox.
  3. Fix F-A2: Make error badge keyboard-accessible or remove cursor:pointer.
  4. Address F-T1: Provide test evidence or defer with tracking issue.
  5. Address F-A3: Implement focus trap for wizard modal (can be deferred to follow-up).
  6. FLAGs are advisory — address in follow-up refactoring tasks.
```
