# TASK-CARD-KPI-01-FIX-01 — Fix Missing KPI Change Percentage

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-01-FIX-01 |
| **المجموعة** | CARD-DESIGN-EXECUTION / Phase C Fix |
| **النوع** | Backend KPI calculation + Frontend fallback message |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ Accepted — Implementation + Auditor PASS |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | Follow-up after visual review of `TASK-CARD-KPI-01` |

---

## 1. سبب المهمة

بعد تنفيذ `TASK-CARD-KPI-01` ظهرت بطاقة **السندات** بشكل أفضل من ناحية الرقم والنص التفسيري، لكن **نسبة المقارنة نفسها لم تظهر**.

المعاينة البصرية أظهرت:

```text
148.4K
مقارنة بالشهر السابق
```

لكن لم يظهر:

```text
↑ +12.5%
```

هذا يعني أن الواجهة استلمت على الأرجح:

```text
changeSource = previousMonth
kpiChangePercent = null
```

---

## 2. التشخيص المتوقع

هناك سببان محتملان يجب إصلاحهما أو تحصينهما:

1. حساب التغير في `KpiQueryBuilder.Build()` مشروط بـ `card.ShowChange == true`، بينما `KpiMode = composite` في Builder يعني وظيفياً أن البطاقة تحتاج التغير. إذا لم يُحفظ `ShowChange` بشكل صحيح، تختفي النسبة رغم اختيار "متقدم".
2. عند وجود `dateRange` من `DateFilterMode` أو فلتر الداشبورد، `BuildChangeQuery()` يحسب دائماً الفترة السابقة مباشرة، ولا يحترم `ChangeSource = previousMonth / previousYear` فعلياً.

---

## 3. الهدف

إصلاح حساب وعرض نسبة المقارنة لبطاقات KPI بحيث:

1. بطاقة `withChange` أو `composite` تحاول حساب التغير إذا توفرت `ValueColumn` و `DateColumn`.
2. `ChangeSource` يؤثر فعلياً على نطاق المقارنة.
3. إذا تعذّر حساب المقارنة بسبب عدم وجود بيانات أو previous = 0، لا تختفي المنطقة بصمت؛ تظهر رسالة خفيفة: `لا توجد بيانات مقارنة كافية`.

---

## 4. النطاق

### In Scope

1. تعديل منطق بناء change query بحيث لا يعتمد فقط على `ShowChange` إذا كان `KpiMode` هو `withChange` أو `composite`.
2. تعديل منطق نطاق المقارنة:
   - `previousPeriod`: نفس مدة النطاق الحالي مباشرة قبل بدايته.
   - `previousMonth`: نفس النطاق الحالي مزاح شهر واحد للخلف، أو الشهر السابق الكامل عند عدم وجود نطاق حالي.
   - `previousYear`: نفس النطاق الحالي مزاح سنة للخلف، أو السنة السابقة/نطاق مناسب عند عدم وجود نطاق حالي.
   - `customQuery`: لا يتم تنفيذ custom query الآن؛ يستخدم fallback آمن أو يترك المقارنة غير متاحة مع رسالة كافية.
3. عند عدم توفر `KpiChangePercent` في بطاقة `withChange` أو `composite`، تعرض الواجهة نصاً خفيفاً: `لا توجد بيانات مقارنة كافية` بدلاً من اختفاء badge بالكامل.
4. الحفاظ على ما تم في KPI-01: الرقم 48px، SVG arrows، comparison text، ColorPalette/blue-theme rule.

### Out of Scope

1. لا تعديل لـ Sparkline — مؤجل إلى `TASK-CARD-KPI-02`.
2. لا تعديل لـ Card Builder.
3. لا إضافة Unit field.
4. لا إضافة custom comparison SQL الآن.
5. لا schema/migration/package/config/auth changes.
6. لا Syncfusion.

---

## 5. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-CARD-KPI-01-FIX-01.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 6. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\KpiQueryBuilder.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 7. Acceptance Criteria

1. `KpiMode = withChange` أو `composite` ينتج change query إذا توفرت `ValueColumn` و `DateColumn` حتى لو `ShowChange` لم يُحفظ بشكل صحيح.
2. `previousMonth` لا يحسب فقط الفترة السابقة المباشرة؛ بل يحترم المقارنة بالشهر السابق.
3. `previousYear` يحترم المقارنة بالسنة السابقة.
4. إذا لم توجد بيانات مقارنة أو كانت قيمة الفترة السابقة = 0، تعرض الواجهة `لا توجد بيانات مقارنة كافية`.
5. إذا وجدت بيانات مقارنة، تظهر النسبة مع SVG arrow كما في KPI-01.
6. لا تتغير Sparkline أو Card Builder.
7. لا توجد ألوان prototype hardcoded.
8. `dotnet build` ينجح بـ 0 errors.

---

## 7.1 Vitality & Polish Checklist

| بند الحيوية | الحالة المطلوبة | التبرير |
|---|---|---|
| Skeleton Loading / Shimmer | N/A | لا يتم تغيير loading state |
| Toast Notifications | N/A | لا يوجد user action مباشر |
| Connection Status Indicator | N/A | خارج النطاق |
| Search حقيقي | N/A | لا توجد جداول |
| Micro-animations | ✅ مطلوب | الحفاظ على rhythm الموجود من KPI-01 وعدم كسر badge animation |
| Empty States | ✅ مطلوب | رسالة `لا توجد بيانات مقارنة كافية` عند عدم توفر المقارنة |
| Realistic Data | ✅ مطلوب | لا بيانات وهمية؛ الحساب من بيانات API فقط |

---

## 8. Security Sensitivity

- **Level:** Low/Medium
- **Reason:** Touches SQL query construction for configured KPI calculations. No user-entered raw SQL should be introduced. Existing identifier sanitation must be preserved.

---

## 9. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Fixes one observed KPI comparison issue |
| One objective only | PASS | Missing comparison percentage |
| No deferrable work included | PASS | Sparkline and Builder preview excluded |
| No UI unless explicitly requested | PASS | Small fallback message only |
| No API unless explicitly requested | PASS | No API route change |
| No Auth unless explicitly requested | PASS | No auth changes |
| No schema/migration unless explicitly requested | PASS | No DB changes |
| No real secrets outside approved local environment files | PASS | No secrets |
| CLI side effects checked | PASS | Build only |
| Allowed Write Targets are narrow | PASS | Three files only |
| Acceptance criteria are testable | PASS | Visual + API payload/build |

**Gate Status:** PASS

---

## 10. Delegation Notes

EngineeringAgent must:

1. Read all existing target files from disk before editing.
2. Preserve unrelated changes.
3. Keep edits minimal and only for missing KPI change percentage.
4. Do not change Card Builder or Sparkline.
5. Do not add packages, migrations, schema changes, config changes, auth changes, or Syncfusion.
6. Preserve existing SQL identifier sanitation.
7. Return exact evidence for how `previousMonth`, `previousYear`, and no-data states are handled.

---

## 11. Engineering Handback

- **Task ID:** TASK-CARD-KPI-01-FIX-01
- **Agent:** engineering-agent-dotnet
- **Status:** Completed — reviewed by Tera and Auditor
- **Files Changed:**
  - `src/WarehouseDashboard.Web/Pages/KpiQueryBuilder.cs`
  - `src/WarehouseDashboard.Web/Pages/Index.cshtml`
  - `DashboardService.cs` read/reviewed, not changed in final follow-up
- **Exact Change Summary:**
  - `withChange` / `composite` KPI modes now build a change query when `ValueColumn` and `DateColumn` exist, even if `ShowChange` is false/missing.
  - `ChangeSource` handling with active date range:
    - `previousPeriod`: same duration immediately before current range.
    - `previousMonth`: current range shifted one month back.
    - `previousYear`: current range shifted one year back.
    - `customQuery`: intentionally no change SQL; frontend fallback displays.
  - Added `NormalizeBaseQuery()` in `KpiQueryBuilder` so bare table names like `[stg_st_invoice]` become `SELECT * FROM [stg_st_invoice]` before helper SQL wraps them.
  - Frontend now shows `لا توجد بيانات مقارنة كافية` when a change-aware KPI has no calculable percentage.
- **Commands Run:**
  - EngineeringAgent: normal build had process-lock warnings; fallback temp build passed 0 warnings / 0 errors.
  - Tera verification: `dotnet build "WarehouseDashboard.Web.csproj"` — PASS, 0 warnings, 0 errors.
- **Issues/Risks:**
  - Runtime visual/API verification still recommended on live dashboard.
  - Auditor flagged optional QA spot-check due lack of runtime sample evidence.

---

## 12. Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | Final code changes are within `KpiQueryBuilder.cs` and `Index.cshtml`; `DashboardService.cs` was not changed in final follow-up. |
| No unauthorized files created/deleted | PASS | No application files outside scope created/deleted. |
| No unauthorized packages/migrations/schema/config/auth | PASS | None. |
| No Syncfusion added | PASS | None. |
| Acceptance criteria satisfied | PASS | Change query trigger, range handling, bare table normalization, and fallback message implemented. |
| Build verification | PASS | Tera build PASS: 0 warnings, 0 errors. |
| Auditor review decision | PASS | REQUIRED; Auditor PASS. |

**Auditor Review Decision:** REQUIRED

**Reason:** Majed requested auditor-first workflow after every card task/fix.

**Auditor Report:** `project-control/audit-reports/QUAUD-TASK-CARD-KPI-01-FIX-01-2026-07-19-001.md`

**Auditor Result:** PASS — STOP 0, CAUTION 0, FLAG 1 (optional runtime/API/UI spot-check recommended).

**Final Task Decision:** ✅ Accepted. Visual/browser re-check required before proceeding to KPI-02.

---

> **Prepared by:** TeraAgent
> **Mode:** Plan Mode — No application code written by TeraAgent
