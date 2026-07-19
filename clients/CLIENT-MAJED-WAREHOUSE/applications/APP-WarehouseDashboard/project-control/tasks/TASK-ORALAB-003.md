# TASK-ORALAB-003 — Oracle Query Lab: Query History + 10K Limit + CSV Download

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ORALAB-003 |
| **المجموعة** | ORACLE-QUERY-LAB-ENHANCEMENT |
| **النوع** | Frontend JS/CSS |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | P0 — حرجة |
| **الحالة** | ✅ ACCEPTED |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | تقرير TeraAgent + تقرير العميل — P0 #6 و P1 #8 و P1 #9 |

---

## 1. الهدف

ثلاثة تحسينات إضافية تجعل الصفحة عملية للاستخدام اليومي:

### 1.1 Query History تلقائي (P0-#6)
**المشكلة:** الاستعلامات السابقة تختفي عند تحديث الصفحة. المستخدم لا يمكنه الرجوع لاستعلام نفذه قبل 5 دقائق.
**المطلوب:** حفظ آخر 20 استعلام تلقائياً في localStorage مع timestamp وإظهارهم في dropdown منفصل.

### 1.2 إظهار حد 10,000 صف (P1-#8)
**المشكلة:** الحد 10,000 صف مخفي في الكود. المستخدم لا يعرف أن النتائج مقطوعة.
**المطلوب:** إظهار "أقصى 10,000 صف" في شريط المعلومات + Toast إذا تم الوصول للحد.

### 1.3 CSV Download + Copy as SQL (P1-#7 + P2-#12)
**المشكلة:** لا يمكن تنزيل النتائج كملف — فقط نسخ clipboard.
**المطلوب:** زر "تنزيل CSV" ينشئ ملفاً ويبدأ التحميل + زر "نسخ SQL INSERT" ينشئ INSERT statements.

---

## 2. الملفات المسموح تعديلها

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml
```

ملاحظة: هذه المهمة frontend فقط — لا حاجة لتعديل Index.cshtml.cs.

---

## 3. معايير القبول

| # | المعيار | كيف يُختبر |
|---|---|---|
| AC-1 | الاستعلامات تُحفظ تلقائياً في localStorage بعد كل تنفيذ ناجح | تشغيل 3 استعلامات ← فتح dropdown التاريخي ← تظهر الثلاثة |
| AC-2 | آخر 20 استعلام كحد أقصى (الأقدم يُحذف) | تشغيل 22 استعلام ← أول استعلام اختفى |
| AC-3 | عرض "أقصى 10,000 صف" في meta info | تنفيذ استعلام ← meta يظهر "أقصى 10,000 صف" |
| AC-4 | Toast عند الوصول للحد الأقصى | استعلام مع 10,000+ صف ← Toast تحذيري |
| AC-5 | "تنزيل CSV" ينشئ ملف CSV ويبدأ التحميل | النقر على تنزيل ← ملف .csv يُنزّل |
| AC-6 | "نسخ SQL" ينسخ INSERT statements | النقر ← نسخ INSERT INTO ... VALUES (...); |
| AC-7 | لا ينكسر السلوك الحالي | اختبار: تشغيل، مسح، نسخ CSV (القديم)، حفظ، تحميل، إلغاء، Ctrl+Enter |
| AC-8 | بناء المشروع لا يتأثر (ملفات .cshtml فقط) | `dotnet build` |

---

## 4. تفاصيل التنفيذ

### 4.1 Query History — Index.cshtml

أضف dropdown جديد بجانب "استعلام محفوظ:" الموجود:
```html
<div class="wd-saved-row">
    <label class="wd-label" style="margin-bottom:0;white-space:nowrap;">تاريخ الاستعلامات:</label>
    <select id="historySelect" class="wd-saved-select" onchange="loadHistoryQuery(this.value)">
        <option value="">— استعلام سابق —</option>
    </select>
    <button class="wd-btn wd-btn--ghost" onclick="clearHistory()" title="مسح التاريخ">مسح</button>
</div>
```

كود JS للتاريخ:
```javascript
const HISTORY_KEY = 'oracleQueryLab_history';
const MAX_HISTORY = 20;

function getHistory() {
    try { return JSON.parse(localStorage.getItem(HISTORY_KEY)) || []; } catch { return []; }
}

function addToHistory(sql) {
    if (!sql || !sql.trim()) return;
    var history = getHistory();
    // Remove duplicate if exists
    history = history.filter(function(h) { return h.sql !== sql; });
    // Add to front
    history.unshift({ sql: sql, date: new Date().toISOString() });
    // Trim to max
    if (history.length > MAX_HISTORY) history = history.slice(0, MAX_HISTORY);
    try { localStorage.setItem(HISTORY_KEY, JSON.stringify(history)); } catch { }
    populateHistoryDropdown();
}

function populateHistoryDropdown() {
    var history = getHistory();
    var sel = document.getElementById('historySelect');
    if (!sel) return;
    sel.innerHTML = '<option value="">— استعلام سابق —</option>';
    history.forEach(function(h, i) {
        var opt = document.createElement('option');
        opt.value = i;
        var preview = h.sql.length > 60 ? h.sql.substring(0, 60) + '...' : h.sql;
        opt.textContent = preview;
        sel.appendChild(opt);
    });
}

function loadHistoryQuery(index) {
    if (index === '' || index === null) return;
    var history = getHistory();
    var h = history[parseInt(index)];
    if (h && h.sql) {
        document.getElementById('sqlInput').value = h.sql;
        showToast('تم تحميل الاستعلام من التاريخ.', 'info');
    }
}

function clearHistory() {
    try { localStorage.removeItem(HISTORY_KEY); } catch { }
    populateHistoryDropdown();
    showToast('تم مسح تاريخ الاستعلامات.', 'success');
}
```

ثم في دالة `runQuery()` بعد النجاح، أضف:
```javascript
if (data.success) {
    addToHistory(sql);
    // ... بقية الكود
}
```

### 4.2 10K Limit Indicator

تعديل عرض meta info ليشمل الحد الأقصى:
```javascript
document.getElementById('meta').textContent =
    'النتائج: ' + data.rowCount + ' صف · الزمن: ' + data.elapsedMilliseconds + ' مللي ثانية' +
    (data.rowCount >= 10000 ? ' · ⚠ الحد الأقصى 10,000 صف' : ' · أقصى 10,000 صف');
```

### 4.3 CSV Download + Copy SQL

أضف أزرار بعد "نسخ CSV":
```html
<button class="wd-btn wd-btn--ghost" id="downloadCsvBtn" onclick="downloadCsv()" disabled>تنزيل CSV</button>
<button class="wd-btn wd-btn--ghost" id="copySqlBtn" onclick="copyAsSql()" disabled>نسخ SQL</button>
```

دوال JS:
```javascript
function downloadCsv() {
    if (!currentRows || currentRows.length === 0) return;
    var cols = currentColumns;
    var csv = cols.map(function(c) { return '"' + (c.name || '') + '"'; }).join(',') + '\n';
    currentRows.forEach(function(row) {
        csv += cols.map(function(c) {
            var val = row[c.name];
            return val != null ? '"' + String(val).replace(/"/g, '""') + '"' : '""';
        }).join(',') + '\n';
    });
    var blob = new Blob(["\uFEFF" + csv], { type: 'text/csv;charset=utf-8;' }); // BOM for Excel Arabic
    var link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = 'oracle_query_result.csv';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(link.href);
    showToast('تم تنزيل ' + currentRows.length + ' صف كملف CSV.', 'success');
}

function copyAsSql() {
    if (!currentRows || currentRows.length === 0) return;
    var cols = currentColumns;
    var colNames = cols.map(function(c) { return '"' + (c.name || '') + '"'; }).join(', ');
    var lines = [];
    currentRows.forEach(function(row) {
        var vals = cols.map(function(c) {
            var val = row[c.name];
            if (val === null || val === undefined) return 'NULL';
            if (typeof val === 'number') return String(val);
            return "'" + String(val).replace(/'/g, "''") + "'";
        });
        lines.push('INSERT INTO oracle_result (' + colNames + ') VALUES (' + vals.join(', ') + ');');
    });
    var sql = lines.join('\n');
    navigator.clipboard.writeText(sql).then(function() {
        showToast('تم نسخ ' + currentRows.length + ' INSERT statements.', 'success');
    }).catch(function() {
        showToast('تعذر النسخ إلى الحافظة.', 'error');
    });
}
```

تعديل `renderGrid` لتمكين الأزرار الجديدة عند وجود نتائج:
```javascript
document.getElementById('downloadCsvBtn').disabled = false;
document.getElementById('copySqlBtn').disabled = false;
```

وتعطيلها عند Clear أو Error.

---

## 5. توجيهات تنفيذية للوكيل

1. **Fresh File Read إلزامي:** اقرأ `Index.cshtml` من القرص أولاً.
2. لا تمس كود المهمتين السابقتين (رسائل الخطأ، N+1 Schema، Connection Status، Cancel، Ctrl+Enter).
3. استخدم BOM `\uFEFF` في الـ CSV لضمان عرض العربية في Excel.
4. أضف كل الكود الجديد في نهاية السكربت الموجود (أو في أماكن مناسبة).
5. **لا تعدل Index.cshtml.cs — هذه المهمة frontend فقط.**
6. قبل إنهاء المهمة، شغّل `dotnet build --no-restore`.

## 6. Handback المطلوب

1. الملفات المعدلة (المسارات الكاملة)
2. مقتطفات الكود الجديد الأساسية
3. نتيجة `dotnet build`
4. أي ملاحظات أو مخاطر

---

## 7. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| أصغر وحدة آمنة | ✅ PASS | Frontend فقط — ملف واحد |
| لا تغيير في Backend | ✅ PASS | لا Index.cshtml.cs ولا Program.cs ولا Middleware |
| لا تغيير في Auth | ✅ PASS | — |
| Allowed Write Targets ضيقة | ✅ PASS | ملف واحد |
| معايير القبول قابلة للاختبار | ✅ PASS | 8 معايير |

**Gate Status:** ✅ PASS

---

## 8. Auditor Decision (initial)

**Expected:** AUDITOR_REVIEW_REQUIRED
**Reason:** تغيير في localStorage + CSV download + SQL generation — يستحق تدقيقاً.

---

## 9. Handback

| البند | القيمة |
|---|---|
| **Agent** | engineering-agent-dotnet |
| **Handback Date** | 2026-07-19 |
| **Files Modified** | `Index.cshtml` (ملف واحد — Frontend فقط) |
| **Build Result** | `dotnet build --no-restore` → ✅ Build succeeded (0 errors, 0 warnings) |
| **New Features** | Query History (localStorage, last 20, dropdown), 10K Row Limit indicator, CSV Download (BOM), Copy as SQL INSERT, Toast 10K warning |
| **No Backend Changes** | ✅ Index.cshtml.cs unchanged |

---

## 10. Post-Execution Review

| البند | النتيجة |
|---|---|
| Fresh File Read | ✅ قُرئ Index.cshtml من القرص |
| Scope | ✅ ملف واحد فقط — Frontend |
| Allowed Write Targets | ✅ م恪守 |
| No Secrets | ✅ localStorage فقط — بيانات غير حساسة |
| AC-1 (History saves) | ✅ addToHistory() تُستدعى بعد نجاح الاستعلام |
| AC-2 (Max 20) | ✅ MAX_HISTORY = 20 مع slice |
| AC-3 (10K in meta) | ✅ يظهر "أقصى 10,000 صف" |
| AC-4 (Toast 10K) | ✅.showToast('warning') عند rowCount >= 10000 |
| AC-5 (CSV Download) | ✅ Blob + BOM + filename oracle_query_result.csv |
| AC-6 (Copy SQL) | ✅ INSERT INTO oracle_result VALUES (...) |
| AC-7 (No regression) | ✅ كود ORALAB-001 و ORALAB-002 سليم |
| AC-8 (Build) | ✅ 0 errors, 0 warnings |
| No regression | ✅ Connection Status, Ctrl+Enter, Cancel, Oracle errors, N+1 Schema — جميعها سليمة |
| Auditor Review | ✅ QUAUD-ORALAB-003 — ACCEPTED after fix (Toast 10K) |
| Functional Test | ✅ HTTP GET page loads, all 9 elements verified in HTML |

**Gate Result:** ✅ PASS

---

## 11. Acceptance

| البند | القيمة |
|---|---|
| **Accepted By** | TeraAgent |
| **Acceptance Date** | 2026-07-19 |
| **Condition** | None — all AC met, Auditor PASS, Build PASS |
| **Deferred Items** | F-002 (FLAG — أزرار بـ title for accessibility) — اختياري، غير حرج |

---

## 12. Auditor Decision (final)

**Status:** AUDITOR_REVIEW_PASS (after fix)
**Report:** `project-control/audit-reports/QUAUD-ORALAB-003-2026-07-19.md`
**Result:** ✅ ACCEPTED — 9/9 gaps PASS, build 0 errors, no regression.
