# TASK-CARD-BEH-002 — Per-Card Auto-Refresh with Visual Indicator

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-BEH-002 |
| **المجموعة** | CARD-DESIGN-EXECUTION |
| **النوع** | UI / JavaScript + CSS |
| **الوكيل المقترح** | ui-designer |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (Auditor PASS) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — Phase B / CARD-BEH-01 |
| **القرارات المعتمدة** | RefreshInterval يُستخدم كما هو محدد في إعدادات البطاقة + تحديث مع مؤشر بصري |

---

## 1. الهدف

تفعيل **التحديث التلقائي لكل بطاقة** حسب `RefreshInterval` الخاص بها، مع **مؤشر بصري خفيف** يظهر أثناء التحديث.

---

## 2. الوضع الحالي

- `RefreshInterval` لكل بطاقة:
  - موجود في قاعدة البيانات ✅
  - موجود في `CardLayoutInfo` ✅
  - موجود في `data-refresh-interval` على عنصر الـ DOM ✅
  - موجود في `window.WD_CARDS[i].refreshInterval` ✅
- **لكن لا يوجد أي مؤقت تلقائي** يستخدم هذه القيمة — التحديث يدوي فقط عبر زر التحديث.

---

## 3. النطاق

### المطلوب

1. بعد تحميل البطاقات الأولي (في `DOMContentLoaded`)، لكل بطاقة حيث `refreshInterval > 0`:
   - تشغيل `setInterval` بقيمة `refreshInterval` (بالثواني × 1000).
2. عند تنفيذ التحديث التلقائي:
   - إضافة indicator بصري خفيف للبطاقة (مثل شريط تحميل رفيع في أعلى البطاقة أو نبضة خفيفة).
   - استدعاء `wdLoadCard(id, false)` لوضع "لا Skeleton" لتجنب الوميض.
3. بعد انتهاء تحميل البطاقة (أو بعد 1.5 ثانية)، إزالة المؤشر البصري.
4. إضافة CSS كلاس `.wd-card--refreshing` مع animation بسيطة:
   - شريط أزرق رفيع (2-3px) في أعلى البطاقة
   - يظهر ويتحرك من اليمين لليسار (RTL-native)
   - يختفي بعد اكتمال التحميل
5. عدم تعطيل أي وظيفة حالية (زر التحديث اليدوي، drag, resize, drill-down تمرين).

### غير المطلوب

- لا تغيير في الـ API
- لا تغيير في الـ Backend
- لا تغيير في هيكل البطاقة
- لا إضافة مكتبات جديدة
- لا تغيير في فلتر التاريخ
- لا تغيير في ColorPalette
- لا استخدام Syncfusion

---

## 4. التسلسل المنطقي

```
DOMContentLoaded:
  لكل بطاقة في window.WD_CARDS:
    إذا refreshInterval > 0:
      بدء setInterval(fn, refreshInterval * 1000)

دالة autoRefresh(id):
  1. إضافة كلاس .wd-card--refreshing للعنصر
  2. استدعاء wdLoadCard(id, false)   // بدون Skeleton
  3. بعد 1500ms: إزالة الكلاس
```

---

## 5. UI Rules

1. indicator يجب أن يكون خفيفاً جداً — شريط تقدم رفيع أزرق، غير متعب بصرياً.
2. لا Skeleton عند التحديث التلقائي (Skeleton فقط للتحميل الأولي وإعادة التحميل اليدوي).
3. عدم تعطيل صفحة المستخدم أثناء التحديث (الصامت نسبياً).
4. يجب أن ينسجم مع RTL والهوية الزرقاء.

---

## 6. UI Acceptance

| # | المعيار | Status |
|---|---|---|
| AC-1 | البطاقات ذات `refreshInterval > 0` تُحدّث تلقائياً بهذا التردد | ☐ |
| AC-2 | البطاقات ذات `refreshInterval = 0` لا تُحدّث تلقائياً | ☐ |
| AC-3 | يظهر مؤشر بصري خفيف أثناء التحديث | ☐ |
| AC-4 | التحديث التلقائي لا يعرض Skeleton (لا وميض مزعج) | ☐ |
| AC-5 | الوظائف الحالية (التحميل الأولي، زر التحديث اليدوي، التمرين، Resize, Drag) لا تنكسر | ☐ |
| AC-6 | `dotnet build --no-restore` ينجح بدون أخطاء | ☐ |

---

## 7. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css
```

---

## 8. Pre-Execution Gate Result

**Result:** PASS

### سبب PASS
- القرارات معتمدة
- البنية جاهزة
- هدف واحد واضح
- لا حاجة لتعديل API أو Backend
- أصغر وحدة سلوكية آمنة
- لا مكتبات جديدة

---

## 9. Vitality & Polish Checklist

- [x] N/A — Skeleton Loading / Shimmer — التحديث التلقائي لا يستخدم Skeleton
- [x] N/A — Toast Notifications — لا إشعارات للتحديث التلقائي
- [x] ✅ Connection Status Indicator — لا يتأثر
- [x] N/A — Search — خارج النطاق
- [x] ✅ Micro-animations — شريط التحميل الخفيف هو التلميح البصري
- [x] N/A — Empty States — لا يتأثر
- [x] N/A — Realistic Data — لا يتأثر

---

## 10. Notes for Agent

1. اقرأ `Index.cshtml` و `blue-theme.css` الحاليين من القرص أولاً.
2. حافظ على كل السلوك القائم.
3. لا تغير وظيفة `wdLoadCard` نفسها — استخدمها كما هي مع `false` للـ showSkeleton.
4. `setInterval` يحتاج معرف لكل بطاقة ليُحفظ; استخدم متغير `_autoRefreshTimers` أو أضف خاصية للكارد.
5. لا تضف مكتبة خارجية.
6. شغّل `dotnet build --no-restore` بعد التعديل.
7. اختبر منطقياً: قم بوضع console.log مؤقت للتأكد من تنفيذ auto refresh.
