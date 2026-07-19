# TASK-ORALAB-005 — OracleQueryLab: Syntax Highlighting + Line Numbers

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ORALAB-005 |
| **المجموعة** | ORACLE-QUERY-LAB-ENHANCEMENT |
| **النوع** | Frontend JS/CSS |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | P2 |
| **الحالة** | ✅ ACCEPTED (build PASS) |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

**المشكلة:** محرر SQL الحالي هو `<textarea>` عادي بدون تلوين — صعب قراءة الاستعلامات الطويلة.

**المطلوب:** استبدال الـ textarea بمحرر SQL احترافي يدعم:
- Syntax highlighting (Keywords, Functions, Strings, Numbers)
- Line numbers
- RTL-safe (الكود يبقى LTR مع واجهة عربية)

---

## 2. الحل المعتمد: CodeMirror 5 (CDN)

**المبرر:**
- lightweight (~100KB) — لا يُثقل الصفحة
- SQL mode جاهز مع syntax highlighting كامل
- Line numbers مدمجة
- RTL-safe (المحرر LTR افتراضياً)
- مكتبة مستقرة و厤نة واسعة
- لا Syncfusion — متوافق مع القيد المعتمد

**CDN:**
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/codemirror.min.css">
<script src="https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/codemirror.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/mode/sql/sql.min.js"></script>
```

---

## 3. النطاق

### المطلوب

1. **CSS/HTML:** إضافة CDN links في `@section Head` أو `<style>`/`<script>` في الصفحة
2. **HTML:** استبدال `<textarea id="sqlInput" ...>` بـ `<div id="editor">` + init CodeMirror
3. **JS Init:**
```javascript
var editor = CodeMirror(document.getElementById('editor'), {
    value: 'SELECT 1 AS test_value FROM DUAL',
    mode: 'text/x-plsql',  // Oracle SQL mode
    lineNumbers: true,
    rtlMoving: true,
    direction: 'ltr',
    theme: 'default',
    indentWithTabs: false,
    smartIndent: true,
    tabSize: 2,
    lineWrapping: true
});
```
4. **JS Updates:** كل مكان يستخدم `document.getElementById('sqlInput').value` يجب أن يستخدم `editor.getValue()` / `editor.setValue()` بدلاً منها
5. **CSS:** تخصيص مظهر المحرر ليتناسب مع الـ theme الأزرق (حدود، خطوط، ارتفاع)

### غير المطلوب

- لا تغيير Backend
- لا مكتبات إضافية غير CodeMirror
- لا تغيير في逻辑ية التشغيل

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml
```

---

## 5. معايير القبول

| # | المعيار | كيف يُختبر |
|---|---|---|
| AC-1 | Syntax highlighting يعمل — Keywords زرقاء، Strings خضراء، Numbers برتقالية | فتح الصفحة → كتابة `SELECT * FROM DUAL` → التلوين ظاهر |
| AC-2 | Line numbers ظاهرة | أرقام الأسطر ظاهرة على يسار المحرر |
| AC-3 | Ctrl+Enter يعمل مع المحرر الجديد | كتابة استعلام → Ctrl+Enter → يُنفَّذ |
| AC-4 | زر "مسح" يفرغ المحرر | النقر → المحرر يُفرّغ |
| AC-5 | Query History يعمل — تحميل استعلام من التاريخ يملأ المحرر | dropdown التاريخ → المحرر يُملأ بالنص |
| AC-6 | placeholder يظهر عند فراغ المحرر | مسح كل النص → placeholder ظاهر |
| AC-7 | `dotnet build` ناجح | 0 errors, 0 warnings |
| AC-8 | لا تراجع على الميزات السابقة | Connection Status, Ctrl+Enter, Cancel, History, CSV, Copy SQL — جميعها تعمل |

---

## 6. المواقع التي تحتاج تعديل (sqlInput → editor)

| الموقع | الكود الحالي | التعديل |
|---|---|---|
| `runQuery()` | `document.getElementById('sqlInput').value` | `editor.getValue()` |
| `clearEditor()` | `document.getElementById('sqlInput').value = ''` | `editor.setValue('')` |
| `loadSavedQuery()` | `document.getElementById('sqlInput').value = q.sql` | `editor.setValue(q.sql)` |
| `loadHistoryQuery()` | `document.getElementById('sqlInput').value = h.sql` | `editor.setValue(h.sql)` |
| `copyQuery()` | `document.getElementById('sqlInput').value` | `editor.getValue()` |
| HTML `<textarea>` | `<textarea id="sqlInput" ...>` | يُستبدل بـ `<div id="editor">` |

---

## 7. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ Frontend فقط — ملف واحد |
| لا تغيير Backend | ✅ |
| لا تغيير Auth | ✅ |
| Allowed Write Targets ضيقة | ✅ ملف واحد |
| معايير القبول قابلة للاختبار | ✅ 8 معايير |

**Gate Status:** ✅ PASS
