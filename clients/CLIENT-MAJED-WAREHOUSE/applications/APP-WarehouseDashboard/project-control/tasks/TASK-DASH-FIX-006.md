# TASK-DASH-FIX-006 — Dashboard Layout Save + Dashboard Tabs Polish

## Status
Implemented / Awaiting User Runtime Verification

## User-Reported Issues
1. حفظ التخطيط لا يعمل:
   - Browser error: `POST http://localhost:5000/wh01?handler=SaveLayout 400 (Bad Request)`
2. تصميم تابات أكثر من داشبورد سيء:
   - Current visual output appears as plain repeated links: `📊 لوحة المعلومات - المستودعات 📊 لوحة المعلومات - المشتريات`

## Diagnosis
### SaveLayout 400
- `Pages/Index.cshtml` sends `RequestVerificationToken` from `input[name="__RequestVerificationToken"]`.
- The page currently does not render an anti-forgery token input.
- Razor Pages POST handlers are rejected with HTTP 400 before reaching `OnPostSaveLayoutAsync` when the anti-forgery token is missing.

### Dashboard Tabs
- Markup exists using `wd-tab-bar`, `wd-tab`, `wd-tab__icon`, and `wd-tab__label`.
- Shared stylesheet lacks complete tab styling for these classes, so links render as plain/weak inline links.

## Scope
Fix only these two dashboard issues:
1. Make SaveLayout POST succeed with valid anti-forgery handling.
2. Improve multi-dashboard tab bar visual design to look like professional RTL dashboard tabs/pills.

## Allowed Files
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`

## Acceptance Criteria
- `POST /{slug}?handler=SaveLayout` no longer returns 400 because of missing anti-forgery token.
- Layout save continues to send JSON and still reaches `OnPostSaveLayoutAsync`.
- If save succeeds, the current success toast remains visible.
- Dashboard tabs are visually styled as clear RTL tabs/pills, not plain repeated links.
- Active dashboard tab is visually distinct.
- Tabs wrap or scroll gracefully on narrower screens.
- No database schema changes.
- `dotnet build --no-restore` passes with 0 errors.

## Vitality & Polish Checklist
- [ ] N/A — Skeleton Loading / Shimmer — this fix does not add new cards/tables/charts.
- [ ] ✅ — Toast Notifications — preserve existing layout-saved/error toast behavior.
- [ ] N/A — Connection Status Indicator — unrelated to this fix.
- [ ] N/A — Search حقيقي — unrelated to this fix.
- [ ] ✅ — Micro-animations — tabs should include subtle hover/active transitions.
- [ ] N/A — Empty States — unrelated to this fix.
- [ ] N/A — Realistic Data — no sample/demo data changes.

## Notes for Implementer
- Before editing any existing file, read the current file from disk first.
- Preserve unrelated changes.
- Do not disable anti-forgery globally.
- Prefer rendering a valid token on the page and using it in the existing fetch request.
- Keep the fix small and localized.

## Handback
- Actor: engineering-agent-dotnet
- Files changed:
  - `Pages/Index.cshtml`
  - `wwwroot/css/blue-theme.css`
- Summary:
  - Added `@Html.AntiForgeryToken()` to `Index.cshtml`, allowing existing AJAX POST header to send a valid Razor Pages anti-forgery token.
  - Added professional RTL tab/pill styling for dashboard tabs, including active state, hover/focus behavior, dark-theme support, and mobile horizontal scrolling.
- Build:
  - Tera verification command: `dotnet build --no-restore`
  - Result: PASS — 0 warnings, 0 errors.

## Post-Execution Review
- Actual changed files reviewed: PASS
- Allowed Write Targets respected: PASS
- No secrets introduced: PASS
- Scope respected: PASS
- Build verification: PASS
- Runtime browser verification: Pending user test
- Auditor Review Decision: NOT_REQUIRED
- Reason: Small localized Razor/CSS fix; no auth policy change, no schema change, no shared infrastructure change, no public API contract change.

## User Runtime Test Needed
1. Open `/wh01`.
2. Move or resize a dashboard card.
3. Confirm DevTools Network no longer shows `400` for `POST /wh01?handler=SaveLayout`.
4. Confirm success toast appears.
5. Refresh page and confirm layout persisted.
