# TASK-CARD-UX-006 — Visual DateFilterMode Indicator in Card Header

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-UX-006 |
| **النوع** | Backend + Frontend / UI |
| **الأولوية** | Medium |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — Phase B / CARD-BEH-04 |

---

## 1. الهدف

إظهار مؤشر خفيف في رأس البطاقة يوضح مصدر الفلترة الزمنية:

| الوضع | يظهر | مثال |
|---|---|---|
| `dashboard` | لا شيء (نظيف) | — |
| `fixed` | نص تاريخي | نطاق زمني ثابت |
| `relative` | نص وصفي | آخر 7 أيام |

---

## 2. التصميم

**المكان:** تحت عنوان البطاقة مباشرة، كالتالي:

```
┌─────────────────────────────────────┐
│ ℹ إجمالي الفواتير                   │
│ آخر ٧ أيام                           │ ← 11px, muted
│ ١,٢٣٤,٥٦٧ ر.س                        │
└─────────────────────────────────────┘
```

**أو للوضع الثابت:**

```
┌─────────────────────────────────────┐
│ ℹ إجمالي الفواتير                   │
│ 2026-01-01 ← 2026-06-30              │ ← 11px, muted
│ ١,٢٣٤,٥٦٧ ر.س                        │
└─────────────────────────────────────┘
```

---

## 3. التغييرات المطلوبة

### 3.1 `Index.cshtml.cs` — CardLayoutInfo

أضف الحقول التالية إلى `CardLayoutInfo` record:

```csharp
public record CardLayoutInfo(
    int Id,
    string Title,
    string Description,
    string ChartType,
    string ColorPalette,
    int GridPositionX,
    int GridPositionY,
    int GridWidth,
    int GridHeight,
    int RefreshInterval,
    string DateFilterMode,   // ← NEW
    string FixedStartDate,   // ← NEW
    string FixedEndDate,     // ← NEW
    int RelativeDays);       // ← NEW
```

وحدّث الـ LINQ Select في `OnGetAsync`:

```csharp
.Select(c => new CardLayoutInfo(
    c.Id,
    c.Title,
    c.Description,
    c.ChartType,
    c.ColorPalette,
    c.GridPositionX,
    c.GridPositionY,
    c.GridWidth,
    c.GridHeight,
    c.RefreshInterval,
    c.DateFilterMode ?? "dashboard",
    c.FixedStartDate ?? "",
    c.FixedEndDate ?? "",
    c.RelativeDays))
```

### 3.2 `Index.cshtml` — Card Header Template

في `wdRenderCard` (أو في قالب الـ HTML إذا كان static)، أضف العنصر بعد الـ `.wd-card__title`:

```javascript
// After rendering the title, add date filter indicator
var dateMode = card.dateFilterMode || 'dashboard';
var dateIndicator = '';
if (dateMode === 'fixed') {
    var from = card.fixedStartDate || '';
    var to = card.fixedEndDate || '';
    if (from && to) dateIndicator = '<span class="wd-card__date-mode wd-card__date-mode--fixed">' + from + ' ← ' + to + '</span>';
} else if (dateMode === 'relative') {
    var days = card.relativeDays || 30;
    dateIndicator = '<span class="wd-card__date-mode wd-card__date-mode--relative">آخر ' + days + ' أيام</span>';
}
// Insert after title
```

**المكان:** بعد عنوان البطاقة وقبل الـ Card body.

### 3.3 `blue-theme.css` — Styles

```css
.wd-card__date-mode {
    display: block;
    font-size: 11px;
    color: var(--c-text-muted, #888);
    line-height: 1.4;
    margin-top: 2px;
    font-weight: 400;
    direction: ltr;
}
.wd-card__date-mode--fixed {
    direction: ltr;
    unicode-bidi: embed;
}
.wd-card__date-mode--relative {
    direction: rtl;
}
```

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css
```

## 5. Acceptance Criteria

| # | المعيار | Status |
|---|---|---|
| AC-1 | بطاقة بـ `DateFilterMode = "fixed"` تعرض النطاق الثابت أسفل العنوان | ☐ |
| AC-2 | بطاقة بـ `DateFilterMode = "relative"` تعرض "آخر N أيام" أسفل العنوان | ☐ |
| AC-3 | بطاقة بـ `DateFilterMode = "dashboard"` لا تعرض أي مؤشر | ☐ |
| AC-4 | المؤشر خفيف وسلس (11px, muted) ولا يزعج | ☐ |
| AC-5 | `dotnet build --no-restore` ناجح | ☐ |

## 6. Notes for Agent

- اقرأ كل الملفات من القرص أولاً
- `CardLayoutInfo` هو `record` — إضافة الحقول تغيّر توقيع الـ constructor
- تأكد من تحديث كل مكان يستخدم `new CardLayoutInfo(...)` — حالياً في `OnGetAsync` فقط
- المؤشر يجب أن يظهر داخل الـ `.wd-card__header` بعد الـ title وقبل الـ `.wd-card__body`
- استخدم `direction: ltr` للتواريخ (fixed) لأنها أرقام إنكليزية، و `direction: rtl` للنصوص العربية

---

## 7. Handback

| البند | القيمة |
|---|---|
| **الحالة** | Submitted |
| **التاريخ** | 2026-07-19 |
| **المعرّف** | TASK-CARD-UX-006 |
| **التنفيذ** | engineering-agent-dotnet |

### التعديلات

1. **Index.cshtml.cs:** `CardLayoutInfo` record يحتوي على 4 حقول جديدة (`DateFilterMode`, `FixedStartDate`, `FixedEndDate`, `RelativeDays`) + LINQ Select محدّث
2. **Index.cshtml:** Razor template يعرض `<span class="wd-card__date-mode">` بعد العنوان مع فروع `fixed` و `relative`
3. **blue-theme.css:** تصحيح CSS token من `--wd-text-muted` → `--c-text-muted`

### Acceptance Criteria — تحقق

| # | المعيار | الحالة |
|---|---------|--------|
| AC-1 | بطاقة fixed تعرض النطاق الثابت | ✅ |
| AC-2 | بطاقة relative تعرض "آخر N أيام" | ✅ |
| AC-3 | بطاقة dashboard لا تعرض مؤشر | ✅ |
| AC-4 | المؤشر خفيف (11px, muted) | ✅ |
| AC-5 | dotnet build ناجح | ✅ (0 warnings, 0 errors) |
