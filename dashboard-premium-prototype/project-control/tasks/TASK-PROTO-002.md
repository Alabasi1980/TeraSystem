# TASK-PROTO-002: إضافة مستوى ثالث للـ DrillDown

## Overview
- **Task ID:** TASK-PROTO-002
- **Agent:** UI Designer Agent (مصمم)
- **Stage:** Prototype Enhancement
- **Priority:** P1 — High

## Objective
تحسين البروتوتايب ليدعم **3 مستويات من DrillDown** بدلاً من مستويين حاليًا.

## التدفق المطلوب

```
Level 0: بطاقة KPI في Dashboard
  → Click
Level 1: Analytic Modal (موجود حالياً)
  - جدول تفصيلي حسب الفرع
  - تبويب تحليل رسومي
  → Click على أي صف في الجدول
Level 2: Record Detail Modal (جديد)
  - تفاصيل أكثر عمقاً للفرع/السجل المختار
  - بيانات إضافية (مثلاً: أعلى العملاء، المنتجات، أداء شهري للفرع)
  - زر "رجوع" للعودة إلى Level 1
```

## الملفات المطلوبة

### 1. `src/components/RecordDetailModal.jsx` (جديد)
- نفس الشكل البصري للـ AnalyticModal (نحاسي/خمري/كريمي)
- عنوان: اسم الفرع المختار + عنوان الـ KPI
- محتويات مقترحة:
  - بطاقات فرعية صغيرة (3-4 KPIs خاصة بالفرع)
  - جدول "أعلى 5 عملاء/منتجات" لهذا الفرع
  - رسم بياني خطي: أداء الفرع الشهري
- زر "رجوع" في الأعلى/الأسفل
- Framer Motion: slide/scale transition

### 2. تعديل `src/components/AnalyticModal.jsx`
- جعل صفوف الجدول (`<tr>`) قابلة للنقر
- عند النقر على صف → استدعاء `onRowSelect(row)` → فتح `RecordDetailModal`
- الاحتفاظ بالـ AnalyticModal مفتوحاً خلفه، أو إغلاقه وإعادة فتحه عند الرجوع
  - **الاقتراح:** إغلاق Level 1 عند فتح Level 2، والرجوع يعيد فتح Level 1 بنفس الـ KPI
- تغيير مؤشر الماوس على الصفوف ليبدو أنها قابلة للنقر
- إضافة hover effect على الصفوف

### 3. تعديل `src/data/mockData.js`
- إضافة بيانات وهمية تفصيلية لكل فرع
- مثال:
```js
export const branchDetailData = {
  'الرياض': {
    kpiSummary: { topProduct: 'منتج أ', activeClients: 142, avgOrder: 30500 },
    topClients: [...],
    monthlyTrend: [...],
  },
  'جدة': { ... },
  // ...
};
```

## Design Source
- `project-preparation/28_UI_UX_GUIDELINES.md`
- `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md`
- `src/index.css` — لا تغيير في الألوان

## Allowed Write Targets
- `src/components/RecordDetailModal.jsx`
- `src/components/AnalyticModal.jsx`
- `src/data/mockData.js`

## Forbidden Actions
- ❌ لا تغيير `index.css`
- ❌ لا TypeScript
- ❌ لا router جديد
- ❌ لا توسع النطاق خارج هذا التدفق
- ❌ لا تغيير في تصميم Level 0 أو Level 1

## Acceptance Criteria
1. النقر على أي صف في جدول Level 1 يفتح Level 2
2. Level 2 يعرض تفاصيل إضافية مقنعة للفرع المختار
3. زر "رجوع" يعيد إلى Level 1 بنفس KPI
4. الانتقالات سلسة بـ Framer Motion
5. الألوان والستايل متناسقة مع باقي البروتوتايب
6. RTL سليم
7. Build يمر بدون أخطاء

## Pre-Execution Gate
- [x] Design Source Decision exists
- [x] `28_UI_UX_GUIDELINES.md` exists
- [x] Task is smallest safe executable unit

## Status: ✅ Pre-Execution Gate PASS
