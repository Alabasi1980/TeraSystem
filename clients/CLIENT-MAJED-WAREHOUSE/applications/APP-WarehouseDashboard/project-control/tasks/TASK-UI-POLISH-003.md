# TASK-UI-POLISH-003 — Dashboard العام: تحسين الفلاتر + Empty States

> **المرحلة:** UI Polish (حسب UI_POLISH_ROADMAP.md — رقم 5)
> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** Medium

---

## 1. الوصف

تحسين Dashboard الرئيسي (`Pages/Index.cshtml`) — تحسين responsive للفلاتر على الموبايل + استبدال emoji بأيقونات SVG في empty states.

⚠️ **ملف كبير جداً (~3100 سطر). تعامل بحذر.**

---

## 2. التغييرات المطلوبة

### A. تحسين Filter Bar على الموبايل
في قسم `<style>`، أضف (أو حسّن) responsive rules:
```css
/* تحسين الـ padding للفلاتر على الموبايل */
@media (max-width: 768px) {
    .wd-filterbar {
        padding: 12px;
        gap: 8px;
    }
    .wd-filterbar__search {
        flex: 1 1 100%;
    }
    .wd-filterbar__search input {
        padding: 8px 36px 8px 12px;
        font-size: 14px;
    }
    .wd-filterbar__select {
        flex: 1 1 calc(50% - 4px);
        min-width: 0;
        padding: 8px 10px;
        font-size: 13px;
    }
}

@media (max-width: 480px) {
    .wd-filterbar {
        padding: 10px;
        gap: 6px;
    }
    .wd-filterbar__select {
        flex: 1 1 100%;
    }
}
```

### B. استبدال emoji بأيقونات SVG في Empty States
استبدل 6 أيقونات emoji بـ SVG أيقونات (حافظ على `aria-hidden="true"` وفي CSS class names):

| الموقع | emoji الحالي | SVG المقترح |
|--------|-------------|-------------|
| **wd-filter-empty__icon** (سطر 1287) | 🔍 | أيقونة بحث/search (عدسة مكبرة مثل Login) |
| **wd-empty__icon** config error (سطر 1131) | ⚠ | أيقونة مثلث تحذير/warning |
| **wd-drill-modal-title-icon** (سطر 1298) | 📊 | أيقونة شريط بياني/bar chart |
| **wd-empty-state** عند عدم وجود نتائج | 🔍 (إذا وُجد) | أيقونة بحث/search |
| أي emoji آخر في empty/error/drill | ابحث عنه | حوّله لـ SVG |

**إرشادات SVG:**
- `viewBox="0 0 24 24"`
- `fill="none"` + `stroke="currentColor"`
- `stroke-width="1.5"` أو `"2"`
- `stroke-linecap="round"` + `stroke-linejoin="round"`
- `width`/`height` يناسب الـ container (عادة 32-48px)

### C. Drill Modal Header Icon (سطر 1298)
استبدل 📊 في `.wd-modal__title-icon` بـ SVG bar chart:
```html
<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <line x1="18" y1="20" x2="18" y2="10"/>
    <line x1="12" y1="20" x2="12" y2="4"/>
    <line x1="6" y1="20" x2="6" y2="14"/>
</svg>
```

### D. Config Error Icon (سطر 1131)
استبدل ⚠ بـ SVG warning:
```html
<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/>
    <line x1="12" y1="9" x2="12" y2="13"/>
    <line x1="12" y1="17" x2="12.01" y2="17"/>
</svg>
```

### E. Filter Empty Icon (سطر 1287)
استبدل 🔍 بـ SVG search:
```html
<svg width="34" height="34" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
    <circle cx="11" cy="11" r="8"/>
    <line x1="21" y1="21" x2="16.65" y2="16.65"/>
</svg>
```

---

## 3. ⚠️ قواعد صارمة (لا تتجاهلها)

1. **لا تغيّر أي CSS class names** أو بنية HTML (فقط استبدل محتوى داخل العناصر)
2. **لا تلمس أي JS** — لا تغيّر runQuery, renderGrid, renderCard, wdRenderGauge، إلخ
3. **لا تلمس تخطيط البطاقات** (KPI cards, charts, tables)
4. **لا تلمس شيفرة C#** (@page, @model, @functions, @code)
5. **فقط أضف** responsive CSS في نهاية البلوك `<style>` **واستبدل** محتوى بعض العناصر
6. **اقرأ الملف من القرص أولاً**

---

## 4. Allowed Write Target

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`

**ممنوع تعديل أي ملف آخر.**

---

## 5. Acceptance Criteria

1. ✅ Filter bar padding محسّن على 768px و 480px (أضيق وأكثر راحة)
2. ✅ Filter empty state icon 🔍 ← SVG search
3. ✅ Config error icon ⚠ ← SVG warning
4. ✅ Drill modal icon 📊 ← SVG bar chart
5. ✅ لا emoji متبقي في empty/error/drill states
6. ✅ `dotnet build` PASS — 0 errors, 0 warnings
7. ✅ لا تغيير في وظائف الصفحة (JS, C#, Layout)

---

## 6. After Completion

أعد:
1. ملخص التغييرات (ماذا استبدلت وأين)
2. تأكيد `dotnet build` PASS
3. Auditor Decision: NOT_REQUIRED
