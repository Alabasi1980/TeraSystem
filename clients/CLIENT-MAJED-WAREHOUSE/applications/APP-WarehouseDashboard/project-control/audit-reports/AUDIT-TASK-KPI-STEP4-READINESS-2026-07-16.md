# AUDIT-TASK-KPI-STEP4-READINESS-2026-07-16

**Audit ID:** QUAUD-TASK-KPI-STEP4-READINESS-2026-07-16-001  
**Task Reviewed:** Card Builder Step 4 KPI Settings Readiness  
**Invoked By:** Majed direct request for Auditor verification  
**Audit Mode:** Standard UI/code readiness audit  
**Scope:** Step 4 UI, card-builder JS, backend save path, DashboardCard model, CardBuilderService, DashboardService/KPI runtime usage  
**Report Path:** `project-control/audit-reports/AUDIT-TASK-KPI-STEP4-READINESS-2026-07-16.md`

## Audit Status

**FAIL** — fix first before meaningful user testing.

Step 4 is visible and partially wired, but the current save path does not persist cards, Step 4 dropdown typing is too permissive, simple KPI validation is too strict, and multiple Step 4 fields are saved/modeled but not consistently used by dashboard rendering.

## Evidence Sources Used

- `Pages/admin-secure-panel/Cards/Builder.cshtml`
- `wwwroot/js/card-builder.js`
- `Pages/admin-secure-panel/Cards/Builder.cshtml.cs`
- `Models/DashboardCard.cs`
- `Models/CardBuilderModels.cs`
- `Services/CardBuilderService.cs`
- `Data/WarehouseDashboardDbContext.cs`
- `Data/Migrations/20260715123122_AddAdvancedKpiFields.cs`
- `Pages/DashboardService.cs`
- `Pages/KpiQueryBuilder.cs`

## Findings Table

| ID | Severity | Evidence | Impact | Recommendation |
|---|---|---|---|---|
| KPI4-001 | **BLOCKER** | `Builder.cshtml.cs:354-367` simulates success and redirects; no DB save call. `CardBuilderService.cs:149-166` builds `DashboardCard` but omits all Advanced KPI fields. `CardBuilderModels.cs:9-57` `CardBuilderRequest` has no Step 4 fields. | User can complete Step 4 and see a success message/redirect, but the card is not actually persisted through this builder path; even the service builder path would drop Step 4 KPI values. Manual testing of saved Step 4 behavior will be misleading. | Implement/verify real persistence from Builder POST to `DashboardCards`; extend request/service mapping to include all Step 4 fields before testing save/render behavior. |
| KPI4-002 | **HIGH** | `card-builder.js:738-759` populates KPI value, date, and category dropdowns from all preview columns; value only annotates numeric columns, date/category are not type-filtered. | Users can choose text columns as KPI values or non-date columns as date columns, causing wrong calculations or runtime SQL/conversion errors in advanced KPI queries. | Restrict value dropdown to numeric columns where possible; detect/limit date dropdown to date/time-like fields; leave category all or text-like. If detection confidence is low, mark options and add validation. |
| KPI4-003 | **HIGH** | `Builder.cshtml:255-259` marks date column required in the always-visible column mapping area. `card-builder.js:244-249` and `915-923` require date column for every KPI step, including `simple`. | Simple KPI mode should only need a value column, but the wizard blocks progression unless a date column is selected. This creates invalid pressure to pick any date-like/non-date column just to continue. | Make `DateColumn` required only for `withChange` and `composite` when change/sparkline features need it; update visible required marker and validation logic accordingly. |
| KPI4-004 | **HIGH** | `KpiQueryBuilder.cs:104-123` derives periods only from `ChangeSource`; `DateFilterMode`, `FixedStartDate`, `FixedEndDate`, and `RelativeDays` are not used. `DashboardService.cs:129-205` calls `KpiQueryBuilder.Build(card)` but does not apply those fields elsewhere. | Step 4 date filter settings can be posted/modeled but have no runtime effect for dashboard KPI calculations. User testing of fixed/relative/dashboard date modes will fail functionally. | Implement DateFilterMode semantics in KPI query building, including fixed range and relative days; define expected behavior for dashboard date filters if not available yet. |
| KPI4-005 | **MEDIUM** | `DashboardService.cs:126-132` sets `KpiValue` to first cell and only enters advanced KPI handling when `KpiMode != "simple"`. `DashboardService.cs:136-149` extracts `ValueColumn` only in advanced modes. | For simple KPI, Step 4 `ValueColumn` is collected but not used; simple KPI display still depends on first result cell. If the selected value column is not first, dashboard output may not match user configuration. | Use `ValueColumn` for simple KPI as well, or remove/clarify value mapping requirement for simple mode and ensure SQL/preview guides make first cell behavior explicit. |
| KPI4-006 | **MEDIUM** | `Builder.cshtml:220-240` mode cards exist and `card-builder.js:941-970` toggles dependent sections. `syncKpiHiddenFields` at `card-builder.js:1044-1061` synchronizes hidden values and sets `ShowChange/ShowSparkline/ShowGrandTotal` from mode. However `buildPreviewRequest` at `card-builder.js:633-640` sends only chart/source/sql/limit. | Mode toggling and final POST hidden fields are partially correct, but live preview cannot validate Step 4 KPI settings before save because KPI mode/columns/options are not included in preview request. | Include Step 4 fields in preview request or clearly disable Step 4 live-preview expectations until backend preview supports them. |
| KPI4-007 | **LOW** | `Builder.cshtml:262-265` includes `CategoryColumn`; model/migration store it (`DashboardCard.cs:63-64`, migration lines `13-19`), but no use found in `DashboardService.cs`/`KpiQueryBuilder.cs`. | Category selection appears usable but currently does not affect KPI rendering/calculation. This may confuse users unless it is planned for future behavior. | Either implement category behavior or label it as reserved/optional with no current dashboard effect. |

## Audit Questions Answered

1. **UI vs model fields:** UI includes all listed Step 4 fields and hidden POST fields (`Builder.cshtml:220-340`, `490-503`), and `DashboardCard`/EF migration include them. However service/request persistence is incomplete.
2. **Dropdown population:** Populated from preview columns, but not correctly type-restricted. Value is annotated but not filtered; date is all columns; category is all columns.
3. **KPI mode toggling:** Basic toggling works for `simple`, `withChange`, and `composite` (`card-builder.js:941-970`), and hidden booleans derive from mode on submit (`1044-1061`).
4. **Hidden synchronization:** Hidden Step 4 values are synchronized before submit (`card-builder.js:1074` calls `syncHiddenInputs`; KPI fields sync at `1044-1061`). This does not compensate for missing backend persistence.
5. **Validation risks:** Yes. Date column is required even for simple KPI; value/date type validity is not enforced; save button eligibility ignores Step 4 validity (`canSave` at `card-builder.js:885-887`).
6. **Backend save:** Not ready. Builder POST currently simulates success; CardBuilderService mapping omits KPI fields.
7. **Dashboard rendering usage:** Partial. Advanced modes use several fields; date filter fields and category are ignored; simple KPI ignores `ValueColumn`.
8. **Blockers before testing:** Yes — KPI4-001 blocks meaningful save/render testing. KPI4-002 through KPI4-005 are high/medium functional readiness risks.

## Final Tera-facing Recommendation

**Fix first.** Do not ask the user to perform Step 4 save/render testing yet. After fixing persistence and validation/dropdown/runtime alignment, QA should run a focused manual test matrix for: simple KPI, withChange, composite, numeric/date column enforcement, fixed/relative date modes, and saved-dashboard rendering.

## Handback to Orchestrator

- **Status:** FAIL
- **Report Path:** `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\audit-reports\AUDIT-TASK-KPI-STEP4-READINESS-2026-07-16.md`
- **Blocking Findings:** KPI4-001
- **Recommended Next Action:** Create a fix task before manual testing.
