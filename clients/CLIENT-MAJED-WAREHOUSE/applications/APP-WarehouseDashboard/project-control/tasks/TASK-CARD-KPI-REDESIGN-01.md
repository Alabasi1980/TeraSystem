# TASK-CARD-KPI-REDESIGN-01 — No-Scroll Professional KPI Dashboard Card Layout

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-REDESIGN-01 |
| **النوع** | Frontend UI / Dashboard Card Layout |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |
| **السبب** | التصميم السابق أصبح مزدحماً ويخرج من حدود البطاقة بعد إضافة المقارنة والمنحنى والمجاميع والتصنيفات |

---

## 1. الهدف

إعادة تصميم بطاقة KPI المعروضة في الداشبورد لتستقبل كل معلومات البطاقة بدون scroll وبدون تراكب أو خروج من البطاقة.

الهدف ليس ضغط العناصر عمودياً، بل بناء توزيع احترافي واضح:

- العنوان والأزرار في الأعلى.
- الرقم الرئيسي في مركز البطاقة بصرياً.
- المقارنة والمجاميع في صف أفقي تحت الرقم.
- المنحنى مثبت أسفل البطاقة دائماً.
- جدول التصنيفات جانبي/مضغوط عند توفره، وليس مكدساً أسفل كل شيء.
- زر التعمق في أعلى البطاقة بجانب الرموز، وليس أسفل البطاقة.

---

## 2. التصميم المقترح

### 2.1 بطاقة KPI مركبة — Wide / Medium

```text
┌──────────────────────────────────────────────────────┐
│ [KPI] [↻] [L M S] [› drill]              العنوان ⓘ │
│                                                      │
│                    397.8K                            │
│      مقارنة: +5565.2% ↑        الإجمالي: 1.6M       │
│      مقارنة بالشهر السابق       إجمالي 2026: 1.6M   │
│                                                      │
│ ┌ تصنيفات مختصرة ┐        sparkline fixed bottom     │
│ │ 35.7% 137.2K   │ ────────────────────────────────  │
│ │ 32.8% 126.3K   │                                  │
└──────────────────────────────────────────────────────┘
```

### 2.2 قواعد التصميم

1. **No vertical scroll inside KPI card.** ممنوع استخدام scroll داخل البطاقة كحل أساسي.
2. **Sparkline bottom docked.** المنحنى مثبت في أسفل البطاقة دائماً.
3. **Main value owns visual center.** الرقم الرئيسي في المركز ويمتلك أعلى وزن بصري.
4. **Comparison + totals are peer blocks.** المقارنة جهة، والمجاميع جهة مقابلة، تحت الرقم.
5. **Breakdown is compact side module.** جدول التصنيفات يكون مختصر وجانبي/سفلي مضغوط بخط أصغر، بحد أقصى 3 صفوف في البطاقة.
6. **Drill affordance moves to header controls.** زر التعمق ينتقل للأعلى بجانب refresh/size controls، وليس أسفل البطاقة.
7. **Small cards degrade gracefully.** في البطاقات الصغيرة: أخفِ التصنيفات أو اعرض أول صفين فقط، لكن لا تكسر التخطيط.

---

## 3. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`

---

## 4. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css`

---

## 5. Acceptance Criteria

| # | المعيار |
|---|---|
| AC-1 | لا يوجد scroll داخل بطاقة KPI في الوضع الطبيعي |
| AC-2 | الرقم الرئيسي لا يتداخل مع المقارنة أو المجاميع أو المنحنى |
| AC-3 | المنحنى مثبت أسفل البطاقة دائماً |
| AC-4 | المقارنة والمجاميع تظهران ككتلتين أفقيتين تحت الرقم |
| AC-5 | جدول التصنيفات مختصر ولا يتجاوز 3 صفوف داخل البطاقة |
| AC-6 | زر التعمق يظهر في أعلى البطاقة بجانب controls عند وجود drill |
| AC-7 | البطاقات الصغيرة لا يحدث فيها خروج محتوى خارج حدود البطاقة |
| AC-8 | لا يتم كسر Chart/Gauge/Table cards |
| AC-9 | `dotnet build` ينجح |

---

## 6. Vitality & Polish Checklist

| البند | الحالة المطلوبة |
|---|---|
| Skeleton Loading / Shimmer | N/A — لا يتم تغيير skeleton في هذه المهمة |
| Toast Notifications | N/A — لا توجد أفعال مستخدم جديدة |
| Connection Status Indicator | N/A — خارج نطاق بطاقة KPI نفسها |
| Search حقيقي | N/A — خارج نطاق البطاقة |
| Micro-animations | ✅ انتقالات خفيفة فقط عند ظهور القيمة/الشارة/المنحنى بدون تشويش |
| Empty States | N/A — لا يتم تغيير empty/error states الحالية |
| Realistic Data | ✅ التصميم يجب أن يتحمل أرقام كبيرة مثل `397.8K` و `1.6M` ونسب مثل `5565.2%` |

---

## 7. Delegation Notes

- Before editing any existing file, read the current file from disk first.
- Preserve unrelated dashboard behavior.
- Do not add new libraries.
- Do not use scroll as the primary solution.
- Prefer CSS grid/flex layout and adaptive classes based on `kpiMode` and card grid size.
- Keep the result elegant, executive, clean, RTL-aware, and close to Majed’s sketch while improving it professionally.

---

## 8. Handback

| البند | القيمة |
|---|---|
| **الحالة** | Accepted |
| **التنفيذ** | ui-designer + engineering-agent-dotnet fix |
| **Audit** | PASS بعد FIX-01 |
| **Audit Reports** | `QUAUD-TASK-CARD-KPI-REDESIGN-01-2026-07-19-001.md`, `QUAUD-TASK-CARD-KPI-REDESIGN-01-FIX-01-2026-07-20-001.md` |

### نتيجة التصميم

- أعيد بناء `wdKpiHtml` إلى مناطق واضحة: hero value، metrics، visual row، sparkline dock.
- أزيل حل الـ internal scroll واستبدل بـ no-scroll layout مع graceful degradation.
- المقارنة والمجاميع تظهران ككتلتين أفقيتين تحت الرقم الرئيسي.
- المنحنى مثبت أسفل البطاقة داخل `.wd-kpi__sparklineDock`.
- التصنيفات مختصرة وتعرض أعلى 3 صفوف فقط.
- زر التعمق نُقل إلى header controls كزر واضح `تفاصيل`.

### إصلاح ما بعد التدقيق

Auditor وجد CAUTION جانبي في drill modal بسبب inline handlers ودعم Gauge. تم إنشاء وتنفيذ `TASK-CARD-KPI-REDESIGN-01-FIX-01`:

- إزالة inline handlers المسببة للخطر داخل drill modal.
- استبدالها بـ DOM-created buttons + `addEventListener`.
- ضمان أن Gauge drill يستخدم `wdRenderGauge` قبل KPI branch.
- إعادة تدقيق Auditor انتهت بـ PASS.

### Acceptance Criteria

| # | المعيار | الحالة |
|---|---|---|
| AC-1 | لا يوجد scroll داخل KPI card في الوضع الطبيعي | ✅ |
| AC-2 | الرقم الرئيسي لا يتداخل مع العناصر الأخرى | ✅ |
| AC-3 | المنحنى مثبت أسفل البطاقة | ✅ |
| AC-4 | المقارنة والمجاميع peer blocks تحت الرقم | ✅ |
| AC-5 | التصنيفات مختصرة بحد أقصى 3 صفوف | ✅ |
| AC-6 | زر التعمق في header controls | ✅ |
| AC-7 | البطاقات الصغيرة degrade gracefully | ✅ |
| AC-8 | Chart/Gauge/Table غير مكسورة | ✅ بعد FIX-01 |
| AC-9 | build ناجح | ✅ |
