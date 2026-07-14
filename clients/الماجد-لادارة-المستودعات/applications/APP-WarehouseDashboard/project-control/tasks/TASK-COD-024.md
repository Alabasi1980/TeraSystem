# TASK-COD-024 — Sync Settings Admin Page

> **Status:** Approved
> **Type:** New Feature / Gap Closure
> **Assigned Agent:** engineering-agent
> **Batch:** B8 (Gap Closure)
> **Created:** 2026-07-13

---

## 1. Objective

Build a Sync Settings page at `/admin-secure-panel/SyncSettings/Index` that allows the admin to configure sync behavior (interval, auto-sync toggle) from the UI instead of editing the database directly.

---

## 2. Current State

- `SyncSettings` table exists in SQL Server (EF Core: `SyncSetting` model)
- `SyncEngineService` reads from this table via ADO.NET
- **No admin UI exists** to edit these settings
- Admin must use SQL Server Management Studio to change interval or toggle auto-sync

---

## 3. Deliverables

### 3.1 Create SyncSettings Razor Page

**File:** `admin-secure-panel/SyncSettings/Index.cshtml` + `Index.cshtml.cs`

**Features:**
- Display current settings (read-only fields):
  - فاصل المزامنة (الدقائق): `IntervalMinutes`
  - المزامنة التلقائية: `IsAutoSyncEnabled` (toggle/checkbox)
  - آخر مزامنة: `LastSyncTimestamp` (read-only display)
- Edit form to update settings:
  - `IntervalMinutes` — numeric input (min: 1, max: 1440)
  - `IsAutoSyncEnabled` — checkbox/toggle
- Save button → updates `SyncSettings` table (Id=1) via EF Core
- Success/error toast notifications
- Confirmation dialog before enabling auto-sync

**Data Access:**
- Use `WarehouseDashboardDbContext` (EF Core) to read and update `SyncSettings`
- This is a Config table — EF Core is the correct access method per `06_DATA_MODEL_PREPARATION.md §5`

### 3.2 Page Design

- Use `_CardsLayout` as layout
- Breadcrumb: `لوحة الإدارة « إعدادات المزامنة`
- Page title: `إعدادات المزامنة`
- Form card with current values
- Save/Cancel buttons
- Status display (last sync time, current interval)

---

## 4. Allowed Write Targets

```
WarehouseDashboard.Web/Pages/admin-secure-panel/SyncSettings/Index.cshtml (CREATE)
WarehouseDashboard.Web/Pages/admin-secure-panel/SyncSettings/Index.cshtml.cs (CREATE)
```

---

## 5. Acceptance Criteria

| # | Criterion |
|---|-----------|
| AC1 | Page loads at `/admin-secure-panel/SyncSettings` |
| AC2 | Current settings displayed correctly |
| AC3 | Admin can change `IntervalMinutes` and save |
| AC4 | Admin can toggle `IsAutoSyncEnabled` and save |
| AC5 | Save updates the database (verified by re-reading) |
| AC6 | Success/error toast shown after save |
| AC7 | Page uses `_CardsLayout` |
| AC8 | `dotnet build -c Release` → 0 errors, 0 warnings |

---

## 6. Estimated Effort

**Total:** 1-1.5 hours

---

> **Prepared by:** TeraAgent — 2026-07-13
