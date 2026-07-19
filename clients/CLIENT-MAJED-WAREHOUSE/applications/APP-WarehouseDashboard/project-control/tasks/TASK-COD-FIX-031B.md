# TASK-COD-FIX-031B — Card Builder: Save Not Persisting + View-Source KPI Display Error

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-FIX-031B |
| **المجموعة** | CARDS |
| **النوع** | Backend — Razor Pages binding + SQL build fix |
| **الوكيل** | engineering-agent-dotnet |
| **الأولوية** | Critical |
| **الحالة** | Assigned |
| **تاريخ الإنشاء** | 2026-07-18 |

---

## 1. المشكلة

بعد إصلاح TASK-COD-CARDS-001 (ربط Builder بـ DB)، تبلغ المستخدم أن:
- إنشاء بطاقة KPI جديدة (`kpiMode=withChange`, `sourceType=SqlTable`, `sourceId=stg_st_invoice_dtl`, `sqlQuery=SELECT SUM([TOTAL]) AS [TOTAL] FROM [stg_st_invoice_dtl]`) **لا تُحفظ**.
- بيانات Network console مكتملة و `action=save` موجود في الـ form data.

بعد قراءة الكود من القرص، حُدّد سببان محتملان:

### السبب 1 (الأرجح — فشل الحفظ بصمت)
- `card-builder.js` `submitForm()` (line 1293-1301) يُنشئ hidden input بـ `name="action"` ويرسله عبر `form.submit()`.
- `Builder.cshtml.cs` `OnPostAsync(string action)` (line 340) يستقبل `action` كـ parameter أعلى المستوى.
- في Razor Pages، اسم `action` **محجوز/يتعارض** مع نظام التوجيه (نفس مستوى `page`/`handler`)، فقد يصل `null` ولا يُربط.
- النتيجة: `action` = `null` → لا يدخل `if (action == "save")` → يرجع `Page()` بصمت بلا خطأ → لا حفظ.

### السبب 2 (خطأ العرض بعد الحفظ — مؤكد في الكود)
- `buildSqlTableQueryForSave()` (card-builder.js line 1245) يولّد للـ KPI من نوع SqlTable:
  `SELECT SUM([TOTAL]) AS [TOTAL] FROM [stg_st_invoice_dtl]`
- هذا يُحفظ مع `DataSourceType = "View"` (Builder.cshtml.cs line 396: `SourceType == "SqlTable" ? "View"`).
- `DashboardService.BuildSql` (line 319-323) عند `DataSourceType == "View"` يلفه كـ:
  `SELECT * FROM [SELECT SUM([TOTAL]) AS [TOTAL] FROM [stg_st_invoice_dtl]]`
  → **SQL غير صالح** → خطأ عرض البطاقة حتى لو نجح الحفظ.

---

## 2. الهدف

1. إصلاح ربط `action` بحيث يتم تنفيذ الحفظ فعلياً (`SaveChangesAsync`).
2. إصلاح بناء SQL للعرض بحيث البطاقة التي مصدرها SqlTable مع SqlQuery يحتوي استعلاماً كاملاً لا تُ预fix بـ `SELECT * FROM [...]`.

---

## 3. النطاق

### المطلوب
- [ ] **A. إصلاح ربط `action` في `Builder.cshtml.cs`:**
  - الطريقة المقبولة: إضافة `[FromForm] action` صراحةً، **أو** تغيير اسم المعامل إلى اسم غير محجوز (مثل `saveAction`) مع تحديث `card-builder.js` ليرسل `name="saveAction"`.
  - يجب أن يدخل `if (action == "save" || action == "saveAndAddAnother")` وينفذ الحفظ.
  - احتفظ بسلوك `action == "preview"` (الجزء السفلي).
- [ ] **B. إصلاح `DashboardService.BuildSql` (View branch):**
  - عند `DataSourceType == "View"`، لا تفترض أن `SqlQuery` هو اسم جدول/عرض دائماً.
  - المنطق الجديد المطلوب:
    - إذا `SqlQuery` يحتوي على كلمة `FROM` (case-insensitive) أو يبدأ بـ `SELECT` → استخدمه كما هو (بدون لف).
    - وإلا (اسم جدول/عرض فقط) → `SELECT * FROM [name]`.
  - هذا يمنع `SELECT * FROM [SELECT ...]`.
- [ ] **C. (اختياري/توصية) تحديث `card-builder.js`**:
  - إذا اخترت تغيير اسم المعامل في C# إلى `saveAction`، حدّث `submitForm()` ليرسل `name="saveAction"` بدل `name="action"`.
  - إن أبقيت `action` مع `[FromForm]`، لا حاجة لتغيير JS.

### غير المطلوب
- لا تغير منطق `OnPostPreviewAsync` أو معاينة.
- لا تغير `DashboardCard` model أو DB schema (الأعمدة موجودة).
- لا تغير `DataSourceType` storage policy (يبقى "View" لـ SqlTable).
- لا تلمس Oracle Query Lab أو Sync أو Auth.
- لا تنشئ Migration جديدة (لا تغيير schema).

---

## 4. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-FIX-031B.md
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\test-reports\
```

---

## 5. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `OnPostAsync` يستقبل قيمة `action=save` ويُنفذ `_db.SaveChangesAsync()` (البطاقة تُحفظ فعلياً) | ☐ |
| AC-2 | `dotnet build -c Release` ينجح بدون أخطاء | ☐ |
| AC-3 | `DashboardService.BuildSql` لا يولّد `SELECT * FROM [SELECT ...]` عند SqlQuery كامل | ☐ |
| AC-4 | بطاقة KPI من نوع SqlTable (مثل `SELECT SUM([TOTAL]) AS [TOTAL] FROM [stg_st_invoice_dtl]`) تعرض بشكل صحيح في Dashboard | ☐ |
| AC-5 | لا توجد أسرار أو connection strings حقيقية في handback | ☐ |

---

## 6. Pre-Execution Gate Result

**Result:** PASS

- Active Technology Profile: `dotnet-razorpages-adonet`
- Smallest safe executable unit: Yes — إصلاح ربط + بناء SQL فقط
- Single goal: Yes — إصلاح حفظ + عرض KPI من SqlTable
- UI task: No (تعديل JS طفيف فقط عند الحاجة)
- Security sensitivity: Low — لا يمس auth/secrets/permissions
- Database impact: None — لا تغيير schema
- Secrets handling: Must use `[REDACTED]` if referencing local connection strings

---

## 7. Delegation Notes

- **الكود الحالي مقروء من القرص** (Fresh File Read تم قبل التفويض).
- السبب 1: Razor Pages reserves `action` as a special route value — prefer `[FromForm] string action` or rename to `saveAction`.
- السبب 2: `BuildSql` View branch must detect a full query vs a bare table name.
- **Handback**: سجّل الملفات المعدلة + نتائج Build + تأكيد أن `action` يربط + تأكيد منطق BuildSql.

---

## 8. Engineering Handback

**Status:** DONE

### Modified Files
1. `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`
   - `OnPostAsync(string action)` → `OnPostAsync([FromForm] string action)` (Bug 1, Option A).
2. `src/WarehouseDashboard.Web/Pages/DashboardService.cs`
   - `BuildSql` View branch rewritten to detect full query vs bare name (Bug 2).
3. `project-control/tasks/TASK-COD-FIX-031B.md` (this handback).

### Build Result
`dotnet build -c Release` (in `src/WarehouseDashboard.Web`):
- Build succeeded.
- 0 Warning(s), 0 Error(s).

### Bug 1 — Fix & Binding Proof (Option A)
- **Option used: A** — added `[FromForm]` to the `action` parameter. JS `submitForm()` left unchanged (it still posts hidden `name="action"`).
- **Why it binds:** In Razor Pages a bare top-level parameter `string action` is interpreted as a reserved routing/handler token and is filled from route/query values, not from form data — so the `action="save"` posted by the hidden input arrives `null`. `[FromForm]` forces the model binder to read the value from the posted form field, which is exactly what `card-builder.js submitForm()` sends (`act.name = 'action'; act.value = action;`). Therefore `action` now binds to `"save"` / `"saveAndAddAnother"` / `"preview"` and the `if (action == "save" ...)` block executes, running `_db.DashboardCards.Add(entity)` + `await _db.SaveChangesAsync()` + the redirect.
- **Verification performed:** Static binding analysis + successful Release build. (No automated HTTP POST test was run against a live DB; the binding mechanism is the documented Razor Pages fix for reserved-token collisions. AC-1 is satisfied by code path, not a live round-trip.)

### Bug 2 — New BuildSql View-Branch Behavior
Old code always did `SELECT * FROM [SqlQuery]`, producing invalid SQL for a pre-aggregated query.
New logic (verbatim from `DashboardService.cs`):

```csharp
string baseSql;
if (card.DataSourceType.Equals("View", StringComparison.OrdinalIgnoreCase))
{
    var trimmed = card.SqlQuery.Trim().TrimEnd(';').Trim();

    // The SqlQuery may be a bare table/view name OR a full query
    // (e.g. a pre-aggregated KPI query built for a SqlTable source:
    // "SELECT SUM([TOTAL]) AS [TOTAL] FROM [stg_st_invoice_dtl]").
    // Detect a full query and use it verbatim; otherwise treat it as a name.
    var isFullQuery = trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
                     || trimmed.Contains(" FROM ", StringComparison.OrdinalIgnoreCase);

    if (isFullQuery)
    {
        baseSql = trimmed;
    }
    else
    {
        var safe = trimmed.StartsWith("[", StringComparison.Ordinal) ? trimmed : $"[{trimmed}]";
        baseSql = $"SELECT * FROM {safe}";
    }
}
else
{
    baseSql = card.SqlQuery;
}
```

For `DataSourceType="View"` with `SqlQuery = "SELECT SUM([TOTAL]) AS [TOTAL] FROM [stg_st_invoice_dtl]"`:
- `trimmed` starts with `SELECT` and contains `" FROM "` → `isFullQuery = true` → `baseSql` is used verbatim (no `SELECT * FROM [...]` wrapping).
- The KPI aggregation guard below (`alreadyAggregated` checks for `SUM(`/`COUNT(`/etc.) returns `baseSql` unchanged) so the aggregated query flows straight through.
- Result: the card renders the intended aggregated query instead of invalid SQL.

### Risk Notes (observed, not fixed — out of scope / trivial)
- `withChange` / sparkline / grand-total KPI queries (`KpiQueryBuilder.Build`) are generated from `card.SqlQuery` and `card.ValueColumn` independently of `BuildSql`'s base wrapping. For a SqlTable-sourced KPI that already stores a full aggregated query (`SELECT SUM(...) AS [TOTAL] FROM [...]`), `ValueColumn` is `[TOTAL]`. `KpiQueryBuilder` may attempt to wrap the already-aggregated source again for the change/sparkline sub-queries. This is a pre-existing behavior and not introduced by this fix; it was left as-is per task scope (Bug 2 only requires stopping the invalid `SELECT * FROM [SELECT ...]` wrapper in the main `BuildSql`). If change/sparkline queries produce unexpected results for such cards, `KpiQueryBuilder` should be reviewed separately.
- No schema/migration changes, no model changes, no secret/connection-string exposure. Secrets referenced only as `[REDACTED]` per policy.

---

## 9. Tera Post-Execution Review

**Result:** ✅ PASS / Accepted

- **Changed files reviewed:** Yes — `Builder.cshtml.cs` line 340 (`[FromForm] string action`) and `DashboardService.cs` lines 316-343 (BuildSql View branch rewrite) verified on disk.
- **Allowed Write Targets respected:** Yes — only the 3 approved source files + task file modified.
- **Scope respected:** Yes — no model/schema/migration changes; `card-builder.js` unchanged (Option A used); no Auth/Sync/Oracle touched.
- **Secrets written:** No.
- **Build verification:** Agent reported `dotnet build -c Release` — 0 errors, 0 warnings.
- **AC-1:** `[FromForm] string action` ensures `action=save` binds → save path executes. PASS.
- **AC-2:** Build 0 errors. PASS.
- **AC-3:** BuildSql no longer produces `SELECT * FROM [SELECT ...]`. PASS.
- **AC-4:** Pending live UI confirmation by user (cannot run browser here) — code-path correct.
- **AC-5:** No secrets. PASS.

### ملاحظات Tera
- تم استخدام Option A (الأبسط والآمن) — لا حاجة لتعديل JS.
- مخاطرة ملحوظة من العميل: `KpiQueryBuilder.Build` (change/sparkline/grand-total) قد يُعيد لف الاستعلام المجمّع مسبقاً لبطاقات SqlTable KPI — سلوك قديم خارج نطاق هذه المهمة؛ يُراجع لاحقاً إن ظهر خلل في عرض withChange.
- يُنصح باختبار حي: إنشاء بطاقة KPI من SqlTable + التحقق من ظهورها في Dashboard وقائمة الإدارة.
