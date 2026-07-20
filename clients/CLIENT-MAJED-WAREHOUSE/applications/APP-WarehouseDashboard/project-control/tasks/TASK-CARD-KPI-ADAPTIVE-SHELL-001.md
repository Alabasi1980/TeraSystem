# TASK-CARD-KPI-ADAPTIVE-SHELL-001

## المهمة: Adaptive KPI Shell — تخطيط واحد مرن لأي مقاس بطاقة

**الحالة:** Accepted  
**التاريخ:** 2026-07-20  
**العميل:** الماجد لادارة المستودعات  
**النوع:** UI Redesign (Frontend Only)  
**الأولوية:** High  

---

## الهدف

استبدال نظام S/M/L الصلب بـ **Adaptive KPI Shell**: تخطيط واحد متماسك يعمل لأي مقاس بطاقة بدون:
- فراغ أبيض كبير في الوسط (مشكلة الحجم المتوسط)
- تكدس/تداخل العناصر (مشكلة الحجم الصغير)

---

## المشاكل الحالية (من مراجعة العميل)

| المقاس | المشكلة | السبب التقني |
|--------|---------|--------------|
| Medium | فراغ كبير بين المحتوى والسبارك | `.wd-kpi__row { flex: 1 }` + `.wd-kpi__sparkline-section { margin-top: auto }` يفصلان المحتوى عن السبارك |
| Small | عناصر متراكمة ومتداخلة | إظهار عناصر كثيرة في مساحة ضيقة بدون أولوية صارمة |

---

## الحل المعتمد: Adaptive KPI Shell

### المبدأ

```
كتلة محتوى واحدة متماسكة (content cluster)
+ سبارك لاين ملاصق مباشرة تحتها
+ لا فراغ متعمد في الوسط
+ إخفاء بالأولوية عند ضيق المساحة (لا تداخل)
```

### الهيكل المستهدف

```
.wd-kpi                          ← container-type: size
  .wd-kpi__cluster               ← كتلة متماسكة (ليست flex:1 فارغة)
    .wd-kpi__row                 ← main + categories جنبًا إلى جنب عند الاتساع
      .wd-kpi__main              ← رقم + تغيير + مجاميع
      .wd-kpi__categories        ← أعلى التصنيفات (اختياري)
    .wd-kpi__sparkline-section   ← ملاصق تحت الـ cluster مباشرة (ليس margin-top:auto مع فراغ)
```

### قواعد ثابتة

1. **لا `flex: 1` على صف المحتوى** إذا كان يخلق فراغًا فارغًا فوق السبارك.
2. **السبارك ملاصق للمحتوى** — `margin-top: auto` ممنوع كحل افتراضي يفصل الفراغ.
3. **محاذاة الكتلة:** أعلى البطاقة (top-aligned cluster) مع padding متوازن — أو توسيط الكتلة كوحدة واحدة إذا بقي فراغ بسيط، لكن **بدون** فصل السبارك عن المحتوى.
4. **Container Queries** على `.wd-kpi` (أو `.wd-card__body`) للعرض والارتفاع.
5. **أولوية الإظهار** (من الأعلى للأقل):
   1. الرقم الرئيسي
   2. نسبة التغيير
   3. Sparkline (رفيع)
   4. المجاميع
   5. أعلى التصنيفات
6. **عند ضيق المساحة:** أخفِ الأقل أولوية بالكامل — لا تضغط عناصر فوق بعضها.
7. **عند تغيير S/M/L من Edit Layout Mode:** يجب إعادة تطبيق كثافة التخطيط فورًا (تحديث class + إن لزم إعادة قياس).

### كثافات مقترحة (عبر container queries + size class كمساعد)

| الكثافة | متى | ماذا يظهر |
|---------|-----|-----------|
| **compact** | عرض ضيق أو ارتفاع منخفض | رقم + تغيير مضغوط + سبارك 36–44px فقط. لا تصنيفات. لا مجاميع. |
| **cozy** | متوسط | رقم + تغيير + مجاميع + سبارك 48–56px. تصنيفات إن اتسع العرض. |
| **comfortable** | واسع/مرتفع | الكل: رقم + تغيير + مجاميع + تصنيفات (حتى 3) + سبارك 56–64px. |

### تفاصيل بصرية

- الرقم محاذى لليمين (RTL start).
- التصنيفات بخط 10–11px، عمود جانبي ضيق.
- السبارك: ارتفاع منخفض، بدون صندوق ضخم، بدون أخذ نصف البطاقة.
- لا تداخل مع أزرار الهيدر (تفاصيل / تحديث).
- الهوية الزرقاء فقط من التوكنز.

### عند resize البطاقة (S/M/L)

- بعد تغيير `gridWidth` / class الحجم، حدّث `wd-kpi--size-*` و/أو كثافة الـ container.
- أعد `sparkline.resize()` إن وُجدت instance.
- لا تعتمد فقط على class محسوب عند أول render.

---

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

(يُسمح بتعديل CSS داخل `<style>` في نفس الملف فقط. لا ملفات أخرى.)

---

## UI Source / Rules / Acceptance

- **UI Source:** `28_UI_UX_GUIDELINES.md` + ملاحظات العميل (لقطات الشاشة 2026-07-20)
- **UI Rules:** توكنز زرقاء فقط، RTL، لا منطق بيانات جديد، لا API
- **UI Acceptance:** `UI_ACCEPTANCE_GATE.md`
- **Design Gap Handling:** سجّل في المهمة إن ظهرت فجوة

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | مهمة واحدة: Adaptive KPI Shell |
| One objective only | PASS | إصلاح فراغ/تكدس + تخطيط مرن |
| No deferrable work included | PASS | — |
| No API / Auth / Schema | PASS | UI فقط |
| No real secrets | PASS | — |
| Allowed Write Targets narrow | PASS | Index.cshtml فقط |
| Acceptance criteria testable | PASS | أدناه |
| Design Source Decision | PASS | 28_UI_UX_GUIDELINES + قرار العميل |

**Gate Status: PASS**

---

## Acceptance Criteria

- [x] لا يوجد فراغ أبيض كبير بين كتلة المحتوى والسبارك في الحجم المتوسط
- [x] لا يوجد تراكب/تكدس عناصر في الحجم الصغير
- [x] السبارك ملاصق تحت المحتوى (cluster متماسك)
- [x] العناصر تُخفى بالأولوية عند ضيق المساحة (لا تداخل)
- [x] يعمل مع تغيير S/M/L فورًا بعد تغيير الحجم
- [x] Container queries أو آلية كثافة مكافئة مستخدمة
- [x] `dotnet build` ينجح (0 errors)
- [x] لا كسر لـ simple / withChange / composite modes

---

## Vitality & Polish Checklist

- [x] N/A — Skeleton Loading — لا تغيير على skeleton العام (مبرر: نطاق KPI body فقط)
- [x] N/A — Toast — لا تغيير
- [x] N/A — Connection Status — لا تغيير
- [x] N/A — Search — لا تغيير
- [x] ✅ — Micro-animations — keyframes محفوظة
- [x] ✅ — Empty States — categories `is-empty` / hidden
- [x] N/A — Realistic Data — البيانات عشوائية حسب توجيه العميل

---

## Notes

- يبني فوق `TASK-CARD-KPI-LAYOUT-RESPONSIVE-001` ويصلح عيوبه.
- لا تغيّر `DashboardService` أو API أو منطق البيانات.
- اقرأ الملف من القرص قبل التعديل.

---

## Handback + Post-Execution Review — TeraAgent

**Agent:** ui-designer  
**Reviewer:** TeraAgent  
**Date:** 2026-07-20

### What changed (verified in Index.cshtml)
1. `.wd-kpi__cluster` — كتلة متماسكة `flex: 0 1 auto`
2. Removed void pattern: no `flex:1` on row, no `margin-top:auto` on sparkline
3. `container-type: size` + `@container kpi` density rules
4. `wdSyncKpiDensity` on render + S/M/L resize
5. Fixed grid width attribute read (`data-grid-w`)
6. Spark heights capped 36–60px
7. Priority hide: categories → details → thinner spark

### Post-Execution Review

| Check | Result |
|---|---|
| Allowed Write Targets only | PASS — Index.cshtml |
| No unauthorized packages | PASS |
| Acceptance criteria | PASS |
| Build | PASS — succeeded 0 errors |
| UI Acceptance Gate | PASS (structure/density) |
| Auditor | NOT_REQUIRED — UI density fix, low risk |

**Gate Status: PASS**  
**Final Decision: Accepted ✅**

> ملاحظة: التحقق البصري النهائي عند العميل مطلوب بعد Hard Refresh.
