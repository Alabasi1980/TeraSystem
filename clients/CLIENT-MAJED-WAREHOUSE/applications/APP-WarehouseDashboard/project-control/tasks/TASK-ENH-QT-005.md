# TASK-ENH-QT-005 — CodeMirror: Syntax Highlighting لمحرر SQL

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

إضافة **CodeMirror 5** إلى QueryTester لتحويل محرر SQL من textarea عادي إلى محرر متقدم مع:
- Syntax Highlighting لأكواد SQL
- Line Numbers
- Auto-indent
- ألوان للكلمات المفتاحية (SELECT, FROM, WHERE, JOIN...)
- اتجاه LTR للأرقام والنصوص

**المرجع:** `OracleQueryLab/Index.cshtml` — يستخدم نفس التقنية (CodeMirror 5.65.16).

---

## 2. التغييرات المطلوبة

### A. أضف CDN resources في بداية `<style>` (أو في `<head>`)

قبل الـ `<style>` block أو بدايته، أضف:

```html
<!-- CodeMirror 5 — SQL Editor -->
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/codemirror.min.css" />
<style>
  /* override CodeMirror height - match original textarea */
  .CodeMirror {
    height: auto;
    min-height: 140px;
    border: 1px solid var(--c-border);
    border-radius: var(--radius-md);
    font-size: 14px;
    direction: ltr;
  }
  .CodeMirror-gutters {
    background: var(--c-surface-muted);
    border-right: 1px solid var(--c-border);
  }
```

### B. استبدال `<textarea>` بـ `<div>`

**استبدل:**
```html
<textarea id="sqlInput" class="wd-textarea" placeholder="SELECT Column FROM Table">SELECT 1 AS [عيّنة]</textarea>
```
**بـ:**
```html
<div id="editor"></div>
```

### C. أضف CDN scripts + CodeMirror Init بعد toastContainer وقبل `<script>` الرئيسي مباشرة:

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/codemirror.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/codemirror/5.65.16/mode/sql/sql.min.js"></script>
```

ثم داخل `<script>` block، عند DOMContentLoaded:
```javascript
var editor = CodeMirror(document.getElementById('editor'), {
    value: 'SELECT 1 AS [عيّنة]',
    mode: 'text/x-mssql',
    lineNumbers: true,
    indentWithTabs: false,
    smartIndent: true,
    tabSize: 2,
    lineWrapping: true,
    extraKeys: {
        'Ctrl-Enter': function() { runQuery(); },
        'Ctrl-Shift-C': function() { clearEditor(); }
    }
});
```

### D. استبدال كل references إلى sqlInput

ابحث في كل ملف JavaScript عن:

| القديم | الجديد |
|--------|--------|
| `document.getElementById('sqlInput').value` | `editor.getValue()` |
| `document.getElementById('sqlInput').value = '...'` | `editor.setValue('...')` |
| `document.getElementById('sqlInput').value = ''` | `editor.setValue('')` |
| `textarea.value = ...` (حيث textarea هو sqlInput) | `editor.setValue(...)` |
| `textarea.value.trim()` | `editor.getValue().trim()` |

**أيضاً:** غيّر زر "مسح" ليكون `clearEditor()` بدلاً من onclick مباشر (أو تأكد أن clearEditor تستخدم editor.setValue('')).

### E. نقل Keyboard Shortcuts إلى CodeMirror

CodeMirror يلتقط أحداث لوحة المفاتيح ولا يمررها للـ document. لذلك نضيف الـ shortcuts في `extraKeys`:

```javascript
extraKeys: {
    'Ctrl-Enter': function() { runQuery(); },
    'Ctrl-Shift-C': function() { clearEditor(); },
    'Ctrl-Shift-H': function() { toggleHistory(); }
}
```

مع الاحتفاظ بالـ document-level shortcuts كبديل.

### F. تحديث clearEditor

تأكد أن `clearEditor()` تستخدم `editor.setValue('')` بدلاً من `document.getElementById('sqlInput').value = ''`.

### G. تحديث generateJoinQuery

الدالة `generateJoinQuery()` و `applyWhere()` تكتبان في textarea — تأكد من استخدام `editor.setValue()`.

### H. تحديث restoreHistory

الدالة `restoreHistory()` تستخدم `document.getElementById('sqlInput').value = ...` — غيّرها إلى `editor.setValue(...)`.

---

## 3. ⚠️ قواعد مهمة

- **لا تستخدم ORM أو مكتبات أخرى**
- **لا تلمس الـ Backend** (Index.cshtml.cs)
- **لا تلمس أي وظيفة حالية** — فقط استبدل الوصول إلى textarea بـ CodeMirror API
- **اقرأ الملف من القرص أولاً**
- **اختبر أن كل شيء يعمل**: runQuery، generateSelect، generateJoinQuery، applyWhere، clearEditor، restoreHistory، saveToHistory، keyboard shortcuts

### 4. CSS إضافية مطلوبة

أضف داخل `<style>` (يمكن في نهايته):
```css
/* CodeMirror override for dashboard theme */
.CodeMirror {
    height: auto;
    min-height: 140px;
    border: 1px solid var(--c-border);
    border-radius: var(--radius-md);
    font-size: 14px;
    direction: ltr;
    font-family: 'Courier New', Consolas, monospace;
    transition: border-color var(--dur-fast) var(--ease);
}
.CodeMirror-focused {
    border-color: var(--c-primary);
    box-shadow: 0 0 0 3px rgba(31,78,121,0.12);
    outline: none;
}
.CodeMirror-gutters {
    background: var(--c-surface-muted);
    border-right: 1px solid var(--c-border);
    border-radius: var(--radius-md) 0 0 var(--radius-md);
}
.CodeMirror-linenumber {
    color: var(--c-text-muted);
    font-size: 12px;
    padding: 0 6px 0 10px;
}
```

---

## 5. Allowed Write Target

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml`

---

## 6. Acceptance Criteria

1. ✅ CodeMirror محمّل من CDN (codemirror.min.css + codemirror.min.js + sql.min.js)
2. ✅ `<textarea id="sqlInput">` مستبدل بـ `<div id="editor">`
3. ✅ CodeMirror initialized مع SQL mode و line numbers
4. ✅ `editor.getValue()` / `editor.setValue()` مستخدم في كل مكان بدلاً من `sqlInput.value`
5. ✅ runQuery تعمل (تقرأ SQL من editor)
6. ✅ clearEditor تمسح المحتوى (editor.setValue(''))
7. ✅ generateSelect تضع SQL في editor
8. ✅ generateJoinQuery تضع SQL في editor
9. ✅ applyWhere تضع WHERE في editor
10. ✅ restoreHistory تستعيد SQL في editor
11. ✅ Ctrl+Enter تعمل (CodeMirror extraKeys)
12. ✅ build PASS 0 errors 0 warnings

---

## 7. After completion

أعد:
1. ملخص كل تغيير (أين وكيف تم الاستبدال)
2. تأكيد أن جميع الدوال تعمل: runQuery, clearEditor, generateSelect, generateJoinQuery, applyWhere, restoreHistory, keyboard shortcuts
3. تأكيد `dotnet build` PASS
4. Auditor: NOT_REQUIRED
