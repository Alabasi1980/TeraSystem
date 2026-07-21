# TASK-ENH-QT-003 — Frontend: WHERE Builder + Enhanced Results + History

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

إضافة 3 تحسينات رئيسية لصفحة QueryTester:
1. **WHERE Builder** — منشئ الشروط البصري
2. **Enhanced Results** — ترتيب، تصدير CSV، تنسيق
3. **Query History** — سجل الاستعلامات + اختصارات keyboard

**ملف واحد فقط:** `Pages/admin-secure-panel/QueryTester/Index.cshtml`

---

## 2. التغييرات المطلوبة

### A. WHERE Builder (بين SELECT Builder ومحرر SQL)

شريط لبناء شروط WHERE بصرياً:

```
┌─────────────────────────────────────────────────────────────┐
│  🌐 شروط التصفية (WHERE)                                     │
│                                                              │
│  [ عمود ▼ ] [ = ▼ ] [ القيمة    ]  [➕] [🗑]                 │
│  [ ItemId  ] [ >  ] [ 100        ]       🗑                  │
│  [ Date    ] [ >= ] [ 2026-01-01 ]       🗑                  │
│                                                              │
│  [➕ إضافة شرط]  [🔗 الكل AND]  [🔄 تطبيق WHERE]              │
└─────────────────────────────────────────────────────────────┘
```

**الوظائف:**
- Dropdown لاختيار العمود (يُعبّر من الجدول المحدد في SELECT Builder)
- Dropdown لعامل المقارنة: =, >, <, >=, <=, <>, LIKE, NOT LIKE, IS NULL, IS NOT NULL, IN, BETWEEN
- حقل إدخال القيمة:
  - `=` / `>` / `<` / `>=` / `<=` → Text input
  - `LIKE` / `NOT LIKE` → Text input مع تلميح `%pattern%`
  - `IS NULL` / `IS NOT NULL` → يخفي حقل القيمة
  - `IN` → Text input مع تنسيق `(val1, val2)`
  - `BETWEEN` → حقلين: "من" و "إلى"
- أزرار: ➕ إضافة شرط، 🗑 حذف شرط
- AND/OR toggle بين الشروط
- 🔄 "تطبيق WHERE" → يضيف `WHERE ...` إلى الاستعلام في textarea

### B. Enhanced Results (تحسين عرض النتائج)

#### B.1 ترتيب بالضغط على رأس العمود
- عند الضغط على رأس العمود: ترتيب تصاعدي → تنازلي → إلغاء
- مؤشر السهم (▲/▼) بجانب اسم العمود المُرتَّب

#### B.2 تنسيق الخلايا
- **الأرقام:** محاذاة لليمين، تنسيق آلاف (1,234.56)
- **التواريخ:** تنسيق `YYYY-MM-DD HH:mm` (أو `ar-SA` locale)
- **الـ null:** عرض `—` (شرطة) بلون `--c-text-muted`
- **Truncated indicator:** إذا `truncated: true` في response، عرض رسالة "⚡ تم عرض أول 1000 صف فقط. استخدم WHERE لتضييق النتائج."

#### B.3 CSV Export
- زر موجود بالفعل "نسخ" (copy) — حافظ عليه
- أضف زر جديد "📥 CSV" (تنزيل ملف CSV):
  ```javascript
  function downloadCSV() {
    if (!currentRows || currentRows.length === 0) return;
    const cols = currentColumns;
    let csv = '\uFEFF'; // BOM for Excel Arabic
    csv += cols.map(c => '"' + (c.name || '') + '"').join(',') + '\n';
    currentRows.forEach(row => {
      csv += cols.map(c => {
        const val = row[c.name];
        return val != null ? '"' + String(val).replace(/"/g, '""') + '"' : '""';
      }).join(',') + '\n';
    });
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = 'query_results_' + new Date().toISOString().slice(0,10) + '.csv';
    link.click();
    URL.revokeObjectURL(link.href);
    showToast('تم تنزيل ' + currentRows.length + ' صف.', 'success');
  }
  ```

### C. Query History (سجل الاستعلامات)

#### C.1 التخزين
- استخدم `localStorage` مع مفتاح `qtHistory`
- احفظ آخر 50 استعلام مع: { sql, source, timestamp, rowCount }

#### C.2 واجهة السجل
- أيقونة/زر "📋 السجل" في شريط الأدوات (بجانب أزرار التشغيل)
- عند الضغط: يظهر Panel منسدل أو جانبي
- يعرض: timestamp + SQL مختصر + source badge
- ضغطة على عنصر → يستعيد الاستعلام في textarea

```html
<div class="qt-history-panel" id="historyPanel" hidden>
  <div class="qt-history__header">
    <span>سجل الاستعلامات</span>
    <button onclick="clearHistory()" class="qt-btn qt-btn--ghost qt-btn--xs">مسح الكل</button>
  </div>
  <div class="qt-history__list" id="historyList"></div>
</div>
```

#### C.3 JavaScript
```javascript
function saveToHistory(sql, source, rowCount) {
  let history = JSON.parse(localStorage.getItem('qtHistory') || '[]');
  history.unshift({ sql, source, rowCount, timestamp: new Date().toISOString() });
  if (history.length > 50) history = history.slice(0, 50);
  localStorage.setItem('qtHistory', JSON.stringify(history));
}

function loadHistory() {
  const list = document.getElementById('historyList');
  const history = JSON.parse(localStorage.getItem('qtHistory') || '[]');
  if (history.length === 0) {
    list.innerHTML = '<div class="qt-history__empty">لا توجد استعلامات سابقة.</div>';
    return;
  }
  list.innerHTML = history.map((h, i) => `
    <div class="qt-history__item" onclick="restoreHistory(${i})">
      <span class="qt-history__time">${new Date(h.timestamp).toLocaleString('ar-SA')}</span>
      <code class="qt-history__sql">${escapeHtml(h.sql.substring(0, 80))}${h.sql.length > 80 ? '...' : ''}</code>
      <span class="qt-history__badge">${h.source}</span>
    </div>
  `).join('');
}
```

### D. Keyboard Shortcuts

```javascript
document.addEventListener('keydown', function(e) {
  // Ctrl+Enter → Run
  if (e.ctrlKey && e.key === 'Enter') {
    e.preventDefault();
    runQuery();
  }
  // Ctrl+Shift+C → Clear
  if (e.ctrlKey && e.shiftKey && (e.key === 'c' || e.key === 'C')) {
    e.preventDefault();
    clearEditor();
  }
  // Ctrl+Shift+H → Toggle History
  if (e.ctrlKey && e.shiftKey && (e.key === 'h' || e.key === 'H')) {
    e.preventDefault();
    toggleHistory();
  }
});
```

أضف تلميحات الاختصارات بجانب الأزرار:
- تشغيل `[Ctrl+Enter]`
- مسح `[Ctrl+Shift+C]`

---

## 3. Design Guidelines

- استخدم نفس CSS Variables (Blue Identity Palette)
- نفس نمط الصفحة الحالية (wd-card, wd-btn, wd-empty)
- WHERE Builder: خلفية `--c-surface-muted`، حدود `--c-border`
- History Panel: خلفية `--c-surface`، ظل `--shadow-md`، أقصى ارتفاع 400px مع scroll
- لا emoji في CSS Classes — استخدم SVG أو رموز بسيطة
- متجاوب: تحت 768px → WHERE Builder يصفّ الأعمدة عمودياً

---

## 4. Allowed Write Target

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml`

**ممنوع تعديل:** أي ملف آخر.

---

## 5. Acceptance Criteria

1. ✅ WHERE Builder: إضافة/حذف شروط مع Column + Operator + Value
2. ✅ WHERE Builder: عوامل مقارنة متنوعة (=, >, <, LIKE, IS NULL, IN, BETWEEN)
3. ✅ WHERE Builder: زر "تطبيق WHERE" يضيف شرط WHERE للاستعلام
4. ✅ Enhanced Results: ترتيب بالضغط على رأس العمود (▲/▼)
5. ✅ Enhanced Results: تنسيق الأرقام والتواريخ والـ null
6. ✅ Enhanced Results: زر CSV Export (تنزيل ملف مع BOM للعربية)
7. ✅ Enhanced Results: رسالة truncated إذا `truncated: true`
8. ✅ Query History: localStorage — آخر 50 استعلام
9. ✅ Query History: لوحة السجل مع إعادة تشغيل استعلام سابق
10. ✅ Keyboard Shortcuts: Ctrl+Enter, Ctrl+Shift+C, Ctrl+Shift+H
11. ✅ `dotnet build` PASS — 0 errors, 0 warnings

---

## 6. Fresh File Read Rule

**اقرأ الملف من القرص أولاً.** احتفظ بكل الوظائف الحالية (Schema Browser، SELECT Builder، المصادر، النتائج، Toast، إلخ). أضف التحسينات الجديدة **فوق** الوظائف الموجودة.

---

## 7. After Completion

أعد:
1. ملخص التغييرات الجديدة
2. تأكيد `dotnet build` PASS (أظهر الـ output)
3. كيف يعمل كل من WHERE Builder، Enhanced Results، History
4. Auditor Decision: NOT_REQUIRED
