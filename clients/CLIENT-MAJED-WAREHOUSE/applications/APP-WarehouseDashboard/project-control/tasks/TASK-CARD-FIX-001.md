# TASK-CARD-FIX-001 — Normalize Description Metadata Output

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-FIX-001 |
| **المجموعة** | CARD-DESIGN-EXECUTION |
| **النوع** | Razor markup / metadata consistency fix |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | Accepted |
| **تاريخ الإنشاء** | 2026-07-19 |
| **السبب** | Follow-up from Auditor report `QUAUD-TASK-CARD-BEH-001-2026-07-19-001.md` |

---

## 1. المشكلة

المدقق وجد أن `Description` لا يُطبَّع بشكل موحّد:

- hint UI يتعامل مع النص بعد `Trim()` واعتبار الفراغات-only كأنها غير موجودة
- لكن `data-description` و`window.WD_CARDS.description` قد يخرجان القيمة الخام غير المطبعة

هذا يسبب inconsistency صغير لكنه حقيقي، ويجب إغلاقه قبل فتح المهمة التالية.

---

## 2. الهدف

توحيد إخراج `Description` في صفحة الداشبورد العامة بحيث:

1. تُحسب نسخة واحدة مطبعة (normalized)
2. تُستخدم هذه النسخة نفسها في:
   - hint rendering
   - `data-description`
   - `window.WD_CARDS.description`

---

## 3. النطاق

### المطلوب
- داخل `Index.cshtml` فقط:
  - استخدم نسخة واحدة مطبعة من `Description`
  - إذا كانت القيمة null/empty/whitespace فقط → تعامل معها كقيمة فارغة موحدة
  - استخدم نفس القيمة في كل المسارات الثلاثة المذكورة أعلاه

### غير المطلوب
- لا تغيير بصري جديد
- لا CSS جديد إلا إن كان ضرورياً جداً (يفترض غير مطلوب)
- لا backend changes
- لا JS logic changes خارج تهيئة `window.WD_CARDS` داخل نفس الملف
- لا ColorPalette work بعد

---

## 4. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

---

## 5. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `Description` تُطبَّع مرة واحدة فقط في الصفحة | ☐ |
| AC-2 | `data-description` يستخدم القيمة المطبعة نفسها | ☐ |
| AC-3 | `window.WD_CARDS.description` يستخدم القيمة المطبعة نفسها | ☐ |
| AC-4 | hint UI يستخدم نفس القيمة المطبعة | ☐ |
| AC-5 | `dotnet build --no-restore` ينجح بدون أخطاء | ☐ |

---

## 6. Pre-Execution Gate Result

**Result:** PASS

### سبب PASS
- أصغر وحدة إصلاح ممكنة
- هدف واحد واضح
- ناتجة مباشرة من تقرير المدقق
- ملف واحد فقط

---

## 7. Notes for Agent

1. اقرأ `Index.cshtml` الحالي من القرص أولاً.
2. لا تُوسّع المهمة.
3. لا تُدخل أي تحسينات إضافية غير مطلوبة.
4. شغّل:

```text
dotnet build --no-restore
```

من مجلد:

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web
```

---

## 8. Engineering Handback

**Status:** DONE

### File Changed
- `src/WarehouseDashboard.Web/Pages/Index.cshtml`

### Normalization Approach
- Added one local helper: `NormalizeDescription(string? value)`.
- Materialized `normalizedCards` once at the top of the page.
- Reused the same normalized description value for:
  - hint rendering
  - `data-description`
  - `window.WD_CARDS.description`

### Build Result
- Initial build attempt failed due to file lock from a running process.
- Tera stopped the locked process and re-ran `dotnet build --no-restore` successfully.

### Closure Statement
- The earlier auditor inconsistency on description normalization is closed code-wise.

---

## 9. Tera Post-Execution Review

**Result:** PASS

### Review Summary
- Fresh file read completed.
- Fix stayed within exact single-file scope.
- No visual expansion or unrelated changes introduced.
- Tera re-ran `dotnet build --no-restore`: PASS (0 warnings / 0 errors).

### Auditor Review Decision
- **REQUIRED → COMPLETED**
- Report: `project-control/audit-reports/QUAUD-TASK-CARD-FIX-001-2026-07-19-001.md`
- Auditor Result: PASS

### Final Decision
- **Accepted**
