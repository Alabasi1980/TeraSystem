# TASK-ORALAB-002 — Oracle Query Lab: Connection Status + Ctrl+Enter + Cancel

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ORALAB-002 |
| **المجموعة** | ORACLE-QUERY-LAB-ENHANCEMENT |
| **النوع** | Backend C# + Frontend JS/CSS |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | P0 — حرجة |
| **الحالة** | ✅ Pre-Execution Gate PASS |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | تقرير العميل — P0 #3 و #4 و #1 (Connection Status) |

---

## 1. الهدف

إضافة ثلاث تحسينات في تجربة المستخدم:

### 1.1 مؤشر حالة الاتصال بـ Oracle (P0-#3)
**المشكلة:** المستخدم لا يعرف هل Oracle متصل أصلاً قبل أن يكتب استعلاماً.
**المطلوب:** مؤشر حي في رأس الصفحة (أو في الشريط العلوي) يظهر:
- 🟢 **متصل** — Oracle تستجيب
- 🔴 **غير متصل** — تعذر الاتصال
- 🟡 **جارٍ الفحص** — أثناء التحقق

**التنفيذ:**
- إضافة `OnGetHealthAsync()` handler في `Index.cshtml.cs` يحاول `SELECT 1 FROM DUAL` مع مهلة 5 ثوانٍ
- إضافة كود JS يفحص الحالة كل 30 ثانية
- إظهار المؤشر في واجهة الصفحة

### 1.2 Ctrl+Enter (P0-#4)
**المشكلة:** اختصار Ctrl+Enter مذكور في الـ HUD (Keyboard Shortcuts) لكنه لا يعمل في OracleQueryLab لأنه مربوط بـ `QueryTester` فقط.
**المطلوب:** إضافة مستمع Ctrl+Enter خاص بالصفحة.

### 1.3 زر إلغاء تنفيذ (P0-#5)
**المشكلة:** إذا استعلام أخذ وقتاً طويلاً، لا يوجد زر لإيقافه. المستخدم عالق 30 ثانية.
**المطلوب:** زر "إلغاء" يظهر أثناء التنفيذ ويستخدم `AbortController` لإلغاء طلب الـ fetch.

---

## 2. الملفات المسموح تعديلها

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml
```

---

## 3. معايير القبول

| # | المعيار | كيف يُختبر |
|---|---|---|
| AC-1 | المؤشر يظهر 🟢 عندما Oracle متاح | فتح الصفحة — ظهور المؤشر الأخضر خلال 5 ثوانٍ |
| AC-2 | المؤشر يظهر 🔴 عندما Oracle غير متاح | إيقاف Oracle مؤقتاً — يتغير المؤشر للأحمر |
| AC-3 | Ctrl+Enter يشغّل الاستعلام في OracleQueryLab | فتح الصفحة + Ctrl+Enter → تشغيل الاستعلام |
| AC-4 | زر "إلغاء" يظهر أثناء تنفيذ الاستعلام | تشغيل استعلام بطيء → ظهور زر الإلغاء |
| AC-5 | زر "إلغاء" يلغي الطلب ويعيد الزر لحالته الطبيعية | النقر على إلغاء → عودة الأزرار |
| AC-6 | لا ينكسر السلوك الحالي (تشغيل، مسح، نسخ، حفظ) | اختبار جميع الوظائف |
| AC-7 | `dotnet build` ينجح بدون أخطاء | `dotnet build` |

---

## 4. تفاصيل التنفيذ

### 4.1 Health Check Handler — Index.cshtml.cs
أضف handler جديد:
```csharp
public async Task<IActionResult> OnGetHealthAsync()
{
    try
    {
        var connectionString = ConnectionStringHelper.ResolveOracle(_configuration);
        if (string.IsNullOrEmpty(connectionString))
            return Json(new { connected = false });

        await using var connection = new OracleConnection(connectionString);
        await connection.OpenAsync();
        await using var cmd = new OracleCommand("SELECT 1 FROM DUAL", connection);
        cmd.CommandTimeout = 5;
        await cmd.ExecuteScalarAsync();
        return Json(new { connected = true });
    }
    catch
    {
        return Json(new { connected = false });
    }
}
```

### 4.2 Connection Status UI — Index.cshtml
أضف في رأس الصفحة (تحت breadcrumb أو بجانب العنوان):
```html
<div id="oraStatus" class="wd-ora-status" title="حالة الاتصال بقاعدة Oracle">
    <span class="wd-ora-status__dot"></span>
    <span class="wd-ora-status__text" id="oraStatusText">جارٍ الفحص...</span>
</div>
```

مع CSS:
```css
.wd-ora-status {
    display: inline-flex; align-items: center; gap: 6px;
    font-size: 12px; color: var(--c-text-muted); margin-right: 16px;
}
.wd-ora-status__dot {
    width: 8px; height: 8px; border-radius: 50%;
    background: var(--c-warning); /* yellow = checking */
    transition: background var(--dur-fast) var(--ease);
}
.wd-ora-status--connected .wd-ora-status__dot { background: var(--c-success); }
.wd-ora-status--disconnected .wd-ora-status__dot { background: var(--c-error); }
```

وكود JS للفحص الدوري:
```javascript
async function checkOracleStatus() {
    const statusEl = document.getElementById('oraStatus');
    const textEl = document.getElementById('oraStatusText');
    try {
        const resp = await fetch('?handler=Health');
        const data = await resp.json();
        if (data.connected) {
            statusEl.className = 'wd-ora-status wd-ora-status--connected';
            textEl.textContent = 'Oracle متصل';
        } else {
            statusEl.className = 'wd-ora-status wd-ora-status--disconnected';
            textEl.textContent = 'Oracle غير متصل';
        }
    } catch {
        statusEl.className = 'wd-ora-status wd-ora-status--disconnected';
        textEl.textContent = 'تعذر الفحص';
    }
}
// Check on load and every 30s
checkOracleStatus();
setInterval(checkOracleStatus, 30000);
```

### 4.3 Ctrl+Enter — Index.cshtml
أضف في السكربت (ضمن init):
```javascript
document.addEventListener('keydown', function(e) {
    if (e.ctrlKey && e.key === 'Enter') {
        var runBtn = document.getElementById('runBtn');
        if (runBtn && !runBtn.disabled) {
            e.preventDefault();
            runBtn.click();
        }
    }
});
```
ملاحظة: الـ _CardsLayout.cshtml لديه مستمع Ctrl+Enter عالمي، لكنه يعمل فقط عندما لا يكون التركيز على INPUT/TEXTAREA. مستمعنا الجديد سيعمل حتى عندما يكون التركيز على textarea.

### 4.4 Cancel Button — Index.cshtml
تعديل أزرار الإجراءات لإضافة زر إلغاء:
```html
<button id="cancelBtn" class="wd-btn wd-btn--ghost" onclick="cancelQuery()" hidden>إلغاء</button>
```

تعديل JS:
```javascript
let abortController = null;

async function runQuery() {
    // ...existing validation...
    abortController = new AbortController();
    
    // Show cancel button, hide it after
    document.getElementById('cancelBtn').hidden = false;
    
    try {
        const resp = await fetch('?handler=Run', {
            method: 'POST',
            headers: { ... },
            body: JSON.stringify({ sql: sql }),
            signal: abortController.signal
        });
        // ...existing handling...
    } catch (err) {
        if (err.name === 'AbortError') {
            showToast('تم إلغاء الاستعلام.', 'warning');
            document.getElementById('emptyState').hidden = false;
            document.getElementById('skeleton').hidden = true;
            setRunning(false);
            document.getElementById('cancelBtn').hidden = true;
            return;
        }
        // ...existing error handling...
    } finally {
        document.getElementById('cancelBtn').hidden = true;
    }
}

function cancelQuery() {
    if (abortController) {
        abortController.abort();
        abortController = null;
    }
}
```

---

## 5. توجيهات تنفيذية للوكيل

1. **Fresh File Read إلزامي:** اقرأ النسخ الحالية من `Index.cshtml.cs` و`Index.cshtml` من القرص أولاً.
2. لا تمس كود TASK-ORALAB-001 السابق (رسائل الخطأ + N+1 Schema).
3. استخدم `await using` للموارد (OracleConnection, OracleCommand).
4. لا تنس معالجة الـ AbortError بشكل منفصل عن بقية الأخطاء.
5. قبل إنهاء المهمة، شغّل `dotnet build --no-restore`.

## 6. Handback المطلوب

1. الملفات المعدلة (المسارات الكاملة)
2. مقتطفات الكود الجديد الأساسية
3. نتيجة `dotnet build`
4. أي مخاطر أو ملاحظات

---

## 7. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| أصغر وحدة آمنة | ✅ PASS | ثلاث تحسينات في ملفين فقط — لا تعارض |
| هدف واحد واضح | ✅ PASS | تحسين تجربة المستخدم: status + shortcut + cancel |
| لا أعمال مؤجلة | ✅ PASS | كل ما هو مطلوب لهذه الفجوات |
| لا تغيير في Auth/Security | ✅ PASS | Health check للقراءة فقط |
| لا تغيير في Schema/Database | ✅ PASS | لا ميجريشن |
| Allowed Write Targets ضيقة | ✅ PASS | ملفين فقط |
| معايير القبول قابلة للاختبار | ✅ PASS | 7 معايير واضحة |

**Gate Status:** ✅ PASS

---

## 8. Auditor Decision (initial)

**Expected:** AUDITOR_REVIEW_REQUIRED
**Reason:** إضافة handler جديد (Health) + تغيير في سلوك الـ fetch يتطلب تدقيقاً.
