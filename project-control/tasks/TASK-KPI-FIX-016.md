# TASK-KPI-FIX-016 — Move KPI Value Selection to Step 4

## Task Info
| Field | Value |
|---|---|
| **Task ID** | TASK-KPI-FIX-016 |
| **Status** | Submitted / Tera Reviewed |
| **Priority** | High |
| **Type** | UX/Data-flow Fix |
| **Requested By** | Majed |
| **Created** | 2026-07-17 |

## User Decision
Majed approved simplifying the wizard flow and gave Tera discretion to choose the best UX.

## Tera UX Decision
Step 2 should be only for selecting the data source. It should not ask for a measurement/value field.

For KPI cards, the canonical value-field selection belongs in Step 4 because it is part of KPI configuration.

## Problem
The wizard currently asks for a measurement/value-like field in Step 2 and then asks again for `عمود القيمة` in Step 4. This creates duplicate user work and confusion.

Additionally, `عمود القيمة` can appear empty because numeric detection depends on preview sample values and does not give a clear user-facing message when no numeric columns are detected.

## Scope
Implement the UX/data-flow change for Card Builder:

1. Step 2 becomes source-only:
   - Remove/hide the SQL table measurement dropdown from Step 2.
   - Remove/hide the Custom SQL measurement input from Step 2.
   - Do not require or post `measurement` from Step 2.

2. Step 4 becomes the KPI value mapping step:
   - `عمود القيمة` remains in Step 4 and becomes the canonical KPI value field.
   - Populate it from detected numeric columns in the selected source preview.
   - Auto-select the first detected numeric column if no value is already selected.
   - Keep user selection if still valid after preview refresh.

3. Empty dropdown handling:
   - If no numeric columns are detected, show a clear inline message near `عمود القيمة`.
   - Do not leave the dropdown empty silently.

4. Robust detection:
   - Numeric detection should scan available preview rows, not only the first row.
   - Numeric strings may be treated as numeric if safely parseable.
   - Date detection should remain safe and not break existing date column behavior.

5. Save behavior:
   - `valueColumn` hidden field must receive the Step 4 value column.
   - For KPI save, `ValueColumn` must be populated.
   - Avoid relying on `Measurement` for KPI value selection.

## Acceptance Criteria
- [ ] Step 2 shows only source selection controls, not `الحقل/القياس`.
- [ ] Step 4 `عمود القيمة` is populated after selecting a SQL table/source and successful preview.
- [ ] Step 4 auto-selects the first numeric column when no previous value exists.
- [ ] If no numeric columns are available, the UI shows a clear message instead of an unexplained empty dropdown.
- [ ] KPI simple mode requires `عمود القيمة` but does not require `عمود التاريخ`.
- [ ] KPI with-change/composite modes require `عمود القيمة` and `عمود التاريخ`.
- [ ] Saving a KPI card posts `valueColumn` correctly.
- [ ] Build succeeds with 0 compilation errors. If blocked by running app file locks, report that separately.

## Allowed Write Targets
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js`

## Vitality & Polish Checklist
- N/A — Skeleton Loading / Shimmer: preview loading already exists; this task does not add new preview containers.
- N/A — Toast Notifications: not required; inline field message is more appropriate for this validation problem.
- N/A — Connection Status Indicator: existing preview connection indicator remains unchanged.
- N/A — Search الحقيقي: no table search scope change in this task.
- N/A — Micro-animations: not relevant to this focused data-flow fix.
- ✅ / Required — Empty States: show a clear message when no numeric value columns are detected.
- N/A — Realistic Data: no sample-data content changes in this task.

## Mandatory Implementation Notes
- Before editing any existing file, read the current file from disk first.
- Preserve unrelated changes, including concurrent work from another Tera session.
- Do not modify files outside Allowed Write Targets.
- Do not introduce secrets or hardcoded credentials.

## Engineering Handback
- **Files Changed:**
  - `Builder.cshtml`
  - `card-builder.js`
- **Summary:** Step 2 measurement controls removed. Step 4 `عمود القيمة` is now the canonical KPI value mapping field. Numeric detection scans all preview rows and supports safely parseable numeric strings. If no numeric columns are detected, the dropdown is disabled and an inline Arabic message is shown.
- **Build:** Normal `dotnet build` blocked by running app file locks. Fallback compile check succeeded with 0 warnings and 0 errors using temp output folder.
- **Environment Lock:** Running `WarehouseDashboard.Web` process locked `bin\Debug\net8.0\WarehouseDashboard.Web.exe` and `.dll`.

## Tera Post-Execution Review
- **Allowed Write Targets:** PASS — changes limited to the two approved application files.
- **Scope:** PASS — Step 2 source-only flow and Step 4 KPI value mapping implemented.
- **Secrets:** PASS — no secrets added.
- **Acceptance Criteria:** PASS by code review and compile fallback; browser validation still required after app restart/hard refresh.
- **Auditor Review Decision:** NOT_REQUIRED
- **Reason:** Focused two-file UI/data-flow fix with successful compile fallback, no auth/security/database/API surface changes.
- **Auditor Report:** N/A

## Next Validation Required
Majed should stop the running app, restart/rebuild, hard refresh the browser, then test Card Builder Step 2 → Step 4 with a SQL table source.
