# TASK-COD-022 — Admin Panel Home + Navigation

> **Status:** Approved
> **Type:** New Feature / Gap Closure
> **Assigned Agent:** engineering-agent + ui-designer
> **Batch:** B8 (Gap Closure)
> **Created:** 2026-07-13
> **Preceded By:** TASK-COD-FIX-001 (all bugs fixed)
> **Blocking:** User cannot navigate admin panel

---

## 1. Objective

Rebuild the Admin Panel home page (`admin-secure-panel/Index.cshtml`) from a useless placeholder into a functional admin dashboard with navigation cards linking to all admin sections. Ensure consistent layout and design across ALL admin pages.

---

## 2. Current State

**What exists:**
- `admin-secure-panel/Index.cshtml` — 10-line placeholder: "مرحباً بك في لوحة الإدارة. إدارة البطاقات وتكوينها ستتوفر في مهمة لاحقة."
- `admin-secure-panel/Cards/` — Full CRUD (Index, Create, Edit) with `_CardsLayout.cshtml`
- `admin-secure-panel/QueryTester/Index.cshtml` — Full implementation with inline CSS
- `admin-secure-panel/DrillDown/Index.cshtml` — Full implementation with inline CSS
- `admin-secure-panel/Login.cshtml` + `Logout.cshtml` — Working auth

**What's broken:**
- Admin Index has NO navigation links to sub-pages
- QueryTester and DrillDown use `_Layout` (simple) while Cards use `_CardsLayout` (full admin)
- No consistent admin experience

---

## 3. Deliverables

### 3.1 Rebuild Admin Index Page

Replace `admin-secure-panel/Index.cshtml` with a real admin dashboard:

**Content:**
- Welcome header with admin name/session info
- Navigation cards (grid layout) for each section:
  1. **إدارة البطاقات** → `/admin-secure-panel/Cards` — icon + description "إنشاء وتعديل وحذف بطاقات لوحة المعلومات"
  2. **مختبر الاستعلامات** → `/admin-secure-panel/QueryTester` — icon + description "اختبار استعلامات SQL للقراءة فقط"
  3. **تكوين التنقّل العميق** → `/admin-secure-panel/DrillDown` — icon + description "إدارة مستويات التنقّل العميق لكل بطاقة"
  4. **سجلات المزامنة** → `/admin-secure-panel/SyncLogs` — icon + description "عرض سجلات المزامنة الأخيرة" (TASK-COD-023)
  5. **إعدادات المزامنة** → `/admin-secure-panel/SyncSettings` — icon + description "تكوين فاصل المزامنة والتشغيل التلقائي" (TASK-COD-024)

**Design:**
- Use `_CardsLayout.cshtml` as the layout (consistent admin experience)
- Card-based grid layout (2-3 columns)
- Each card: icon + title + short description + hover effect
- Blue theme tokens from `_CardsLayout.cshtml`
- Responsive (1 column on mobile)

### 3.2 Unify Admin Layout

**Problem:** QueryTester and DrillDown use `_Layout` (bare bones) while Cards use `_CardsLayout` (full admin with topbar).

**Fix:**
- QueryTester and DrillDown pages should use `_CardsLayout` as their layout
- Update their `Layout` property in `@{ Layout = "_CardsLayout"; }` or via `_ViewStart.cshtml`
- Remove duplicated inline CSS from QueryTester and DrillDown (they should inherit styles from `_CardsLayout`)
- Keep page-specific styles minimal (only unique styles not in `_CardsLayout`)

### 3.3 Ensure All Admin Pages Have Back Navigation

Each sub-page should have a breadcrumb linking back to the admin home:
```
لوحة الإدارة « إدارة البطاقات
```

---

## 4. Allowed Write Targets

```
WarehouseDashboard.Web/Pages/admin-secure-panel/Index.cshtml (REPLACE)
WarehouseDashboard.Web/Pages/admin-secure-panel/Index.cshtml.cs (UPDATE — add session info)
WarehouseDashboard.Web/Pages/admin-secure-panel/QueryTester/Index.cshtml (UPDATE — layout + remove inline CSS)
WarehouseDashboard.Web/Pages/admin-secure-panel/DrillDown/Index.cshtml (UPDATE — layout + remove inline CSS)
WarehouseDashboard.Web/Pages/admin-secure-panel/_ViewStart.cshtml (CREATE — set default layout to _CardsLayout)
```

---

## 5. Acceptance Criteria

| # | Criterion |
|---|-----------|
| AC1 | Admin Index shows 5 navigation cards with correct links |
| AC2 | Clicking each card navigates to the correct page |
| AC3 | All admin pages use `_CardsLayout` (consistent topbar + styling) |
| AC4 | No duplicated inline CSS across admin pages |
| AC5 | Each sub-page has breadcrumb linking back to admin home |
| AC6 | `dotnet build -c Release` → 0 errors, 0 warnings |
| AC7 | Responsive design (works on mobile) |

---

## 6. Estimated Effort

**Total:** 2-3 hours
- Admin Index rebuild: 1 hour
- Layout unification: 1 hour
- Breadcrumb + cleanup: 30 min

---

> **Prepared by:** TeraAgent — 2026-07-13
