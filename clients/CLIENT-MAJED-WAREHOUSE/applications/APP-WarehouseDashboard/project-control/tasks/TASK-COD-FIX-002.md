# TASK-COD-FIX-002 — Razor Encoding Normalization (UTF-16LE → UTF-8)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-FIX-002 |
| **المجموعة** | FIX |
| **النوع** | Encoding / Rendering Fix |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | 🟡 Assigned |
| **تاريخ الإنشاء** | 2026-07-14 |

---

## 1. المشكلة

بعض صفحات Razor داخل `admin-secure-panel` تُعرض بمحتوى عربي مشوّه (mojibake) مثل:

```
┘ä┘ê╪¡╪⌐ ╪º┘ä╪Ñ╪»╪º╪▒╪⌐ ...
```

التحليل أوضح أن الملفات التالية محفوظة بترميز **UTF-16LE** (مع BOM) بدل UTF-8:

- `Pages/admin-secure-panel/_ViewStart.cshtml`
- `Pages/admin-secure-panel/SyncLogs/Index.cshtml`
- `Pages/admin-secure-panel/SyncSettings/Index.cshtml`

## 2. الهدف

تحويل الملفات المتأثرة إلى **UTF-8** (يفضّل بدون BOM)، مع الحفاظ الكامل على المحتوى، بحيث تظهر العربية بشكل صحيح في المتصفح.

## 3. النطاق

### المطلوب
- [ ] فحص كل ملفات Razor تحت `Pages/admin-secure-panel`
- [ ] تحويل أي ملف UTF-16LE أو غير UTF-8 إلى UTF-8
- [ ] عدم تغيير النصوص أو المنطق أو الهيكل
- [ ] التأكد أن `_ViewStart.cshtml` ما زال يحدد `_CardsLayout.cshtml`
- [ ] التحقق من أن صفحات `SyncLogs` و`SyncSettings` تُعرض بالعربية بشكل صحيح

### غير المطلوب
- لا تعديل على المنطق الوظيفي
- لا إعادة تصميم
- لا تغيير في الـ CSS أو JS إلا إذا كان ملفًا تالفًا بنفس المشكلة

## 4. Allowed Write Targets

```
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/_ViewStart.cshtml
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/SyncLogs/Index.cshtml
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/SyncSettings/Index.cshtml
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/**
```

## 5. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | الملفات الثلاثة على UTF-8 | ⬜ |
| AC-2 | لا توجد Null Bytes أو UTF-16LE BOM في الملفات المتأثرة | ⬜ |
| AC-3 | `SyncLogs` تعرض العربية بشكل صحيح في المتصفح | ⬜ |
| AC-4 | `SyncSettings` تعرض العربية بشكل صحيح في المتصفح | ⬜ |
| AC-5 | `dotnet build -c Release` = 0 errors / 0 warnings | ⬜ |

## 6. ملاحظة تنفيذية

إذا وُجد أي ملف Razor آخر بنفس المشكلة داخل `admin-secure-panel`، يتم توحيده أيضاً إلى UTF-8 ضمن نفس المهمة، لكن **فقط** الملفات المتأثرة فعلاً.
