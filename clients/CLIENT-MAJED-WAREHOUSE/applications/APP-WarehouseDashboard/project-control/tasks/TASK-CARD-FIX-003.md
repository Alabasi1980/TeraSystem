# TASK-CARD-FIX-003 — Use asp-append-version for Card Builder JavaScript

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-FIX-003 |
| **النوع** | Bug Fix / Razor script cache-busting |
| **الأولوية** | Critical |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## المشكلة

صفحة Card Builder ما زالت تحمل نسخة قديمة من:

```text
/js/card-builder.js?v=20260718-023A
```

لذلك التعديل الأخير في `buildSqlTableQueryForSave()` لا يصل للمتصفح، وما زالت الدالة في runtime ترجع:

```sql
SELECT SUM([FINAL_AFTER_DISC]) AS [FINAL_AFTER_DISC] FROM [stg_st_invoice]
```

بدلاً من:

```text
[stg_st_invoice]
```

---

## المطلوب

استخدام Razor cache busting الرسمي:

```html
asp-append-version="true"
```

بدلاً من version يدوي ثابت.

---

## Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
```

---

## Acceptance Criteria

| # | المعيار | Status |
|---|---|---|
| AC-1 | `card-builder.js` يستخدم `asp-append-version="true"` | ☐ |
| AC-2 | لا يبقى `?v=20260718-023A` في رابط السكربت | ☐ |
| AC-3 | بعد hard refresh، `buildSqlTableQueryForSave()` يرجع `[stg_st_invoice]` لبطاقة KPI من جدول SQL | ☐ |
| AC-4 | `dotnet build --no-restore` بدون أخطاء C# | ☐ |
