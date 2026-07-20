# TASK-CARD-KPI-REDESIGN-01-FIX-01 — Drill Modal Regression Fix After KPI Redesign

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-REDESIGN-01-FIX-01 |
| **النوع** | Frontend JS/Razor regression fix |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | QUAUD-TASK-CARD-KPI-REDESIGN-01-2026-07-19-001 |

---

## 1. الهدف

إصلاح CAUTION الناتج عن مراجعة Auditor بعد إعادة تصميم بطاقة KPI.

التصميم الجديد للـ KPI مقبول، لكن ظهر تغيير جانبي في drill modal قد يسبب regression.

---

## 2. المشكلة

Auditor وجد أن بعض أزرار drill modal تستخدم inline handlers مثل:

- `onclick="wdLoadLevel()"`
- `onclick="wdNavigateToLevel(...)"`
- `onclick="wdNextLevel()"`

بينما هذه الدوال معرّفة داخل IIFE وليست global، لذلك قد لا تعمل من inline HTML.

كما وجد أن `Gauge` داخل drill modal قد يُعامل كـ KPI بدل `wdRenderGauge`.

---

## 3. المطلوب

1. إزالة أو إصلاح الاعتماد على inline handlers غير global.
2. إما:
   - ربط الدوال المطلوبة صراحةً على `window`، أو
   - استبدال inline handlers بعناصر DOM مع `addEventListener`.
3. استعادة مسار Gauge drill rendering بحيث يستخدم `wdRenderGauge` وليس KPI-style display.
4. عدم تغيير تصميم KPI الجديد إلا إذا كان ضرورياً جداً.

---

## 4. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

---

## 5. Acceptance Criteria

| # | المعيار |
|---|---|
| AC-1 | لا توجد inline handlers تستدعي دوال غير global |
| AC-2 | أزرار retry / breadcrumb / previous / next تعمل من داخل drill modal |
| AC-3 | Gauge drill rendering يستخدم `wdRenderGauge` |
| AC-4 | KPI dashboard card redesign لا يتراجع |
| AC-5 | `dotnet build` ينجح أو ينجح بإخراج مؤقت إذا كان التطبيق يقفل bin |

---

## 6. Handback

| البند | القيمة |
|---|---|
| **الحالة** | Accepted |
| **التنفيذ** | engineering-agent-dotnet |
| **Audit** | PASS |
| **Audit Report** | `project-control/audit-reports/QUAUD-TASK-CARD-KPI-REDESIGN-01-FIX-01-2026-07-20-001.md` |

### التعديلات

- استبدال inline handlers داخل drill modal بدوال DOM + `addEventListener`.
- إضافة `wdAppendDrillRetry()` لإنشاء زر retry بدون inline onclick.
- بناء breadcrumbs/footer buttons عبر DOM بدلاً من HTML strings مع onclick.
- فصل Gauge rendering قبل KPI branch بحيث يستخدم `wdRenderGauge`.

### Verification

- `dotnet build "WarehouseDashboard.sln"` نجح: `0 Warning(s)`, `0 Error(s)`.
- Auditor re-check: PASS.
