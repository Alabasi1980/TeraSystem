# TASK-COD-023 — Sync Logs Page

> **Status:** Approved
> **Type:** New Feature / Gap Closure
> **Assigned Agent:** engineering-agent
> **Batch:** B8 (Gap Closure)
> **Created:** 2026-07-13
> **Spec Reference:** `07_SCREENS_AND_UI_STRUCTURE.md` §5.8

---

## 1. Objective

Build a Sync Logs page at `/admin-secure-panel/SyncLogs/Index` that displays sync history from the `SyncRunLogStore` (in-memory ring buffer). This page is specified in `07_SCREENS_AND_UI_STRUCTURE.md §5.8` but was never implemented.

---

## 2. Current State

- `SyncRunLogStore` (Api project) stores recent sync runs in memory (ring buffer, max ~50 records)
- `SyncController.Logs()` endpoint returns these records as JSON
- **No UI page exists** to display them

---

## 3. Deliverables

### 3.1 Create SyncLogs Razor Page

**File:** `admin-secure-panel/SyncLogs/Index.cshtml` + `Index.cshtml.cs`

**Features:**
- Display sync run history in a Syncfusion Grid
- Columns: تاريخ البداية، تاريخ الانتهاء، النوع (يدوي/تلقائي)، الحالة (ناجحة/فاشلة)، عدد السجلات، رسالة الخطأ (إن وُجدت)
- Filter by status (All / Success / Failed)
- Sort by date (newest first)
- Empty state when no logs exist
- Skeleton loading while fetching

**Data Source:**
- Read from `SyncRunLogStore` (injected via DI)
- The Api project's `SyncRunLogStore` is a Singleton — but the Web project doesn't have access to it
- **Solution:** Create a simple `SyncLogService` in the Web project that calls the Api's `GET /api/sync/logs` endpoint via `HttpClient`
- OR: Share the `SyncRunLogStore` between projects (not recommended — tight coupling)
- **Best approach:** The Web project should have its own lightweight in-memory log store that mirrors the Api's logs. When the SyncStatusBar polls `/api/sync/status`, it can also fetch logs. OR simply call the Api endpoint from the page's JavaScript.

**Simplest approach (recommended):**
- The page loads and calls `fetch('/api/sync/logs')` via JavaScript
- The Api project already has this endpoint (`SyncController.Logs()`)
- Render the results in a Syncfusion Grid client-side
- This avoids cross-project dependency issues

### 3.2 Page Design

- Use `_CardsLayout` as layout
- Breadcrumb: `لوحة الإدارة « سجلات المزامنة`
- Page title: `سجلات المزامنة`
- Syncfusion Grid with columns listed above
- Status badge (green for success, red for failed)
- Toast for errors
- Auto-refresh every 30 seconds (like SyncStatusBar)

---

## 4. Allowed Write Targets

```
WarehouseDashboard.Web/Pages/admin-secure-panel/SyncLogs/Index.cshtml (CREATE)
WarehouseDashboard.Web/Pages/admin-secure-panel/SyncLogs/Index.cshtml.cs (CREATE)
```

---

## 5. Acceptance Criteria

| # | Criterion |
|---|-----------|
| AC1 | Page loads at `/admin-secure-panel/SyncLogs` |
| AC2 | Sync logs displayed in Syncfusion Grid with correct columns |
| AC3 | Status filter works (All / Success / Failed) |
| AC4 | Empty state shown when no logs exist |
| AC5 | Skeleton loading while fetching |
| AC6 | Page uses `_CardsLayout` |
| AC7 | `dotnet build -c Release` → 0 errors, 0 warnings |

---

## 6. Estimated Effort

**Total:** 1.5-2 hours

---

> **Prepared by:** TeraAgent — 2026-07-13
