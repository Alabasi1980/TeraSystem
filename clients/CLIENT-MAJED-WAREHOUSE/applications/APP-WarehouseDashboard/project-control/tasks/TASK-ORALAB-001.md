# TASK-ORALAB-001 — Oracle Query Lab: رسائل الخطأ + N+1 Schema

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ORALAB-001 |
| **المجموعة** | ORACLE-QUERY-LAB-ENHANCEMENT |
| **النوع** | Backend C# + Frontend JS |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | P0 — حرجة |
| **الحالة** | ✅ Pre-Execution Gate PASS (see below) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | دراسة TeraAgent + تقرير العميل — P0 #1 و #2 |

---

## 1. الهدف

إصلاح فجوتين حرجتين تمنعان الاستخدام العملي للصفحة:

### 1.1 رسائل خطأ Oracle عامة جداً (P0-#1)
**المشكلة:** حاليًا عند فشل استعلام Oracle، تظهر رسالة عامة:
```
"تعذر تنفيذ الاستعلام. تأكد من صحة الصياغة وأن الجداول والحقول موجودة."
```
بدون رقم الخطأ ORA-XXXXX ولا تفاصيل الخطأ من Oracle — مما يجعل التصحيح مستحيلاً.

**المطلوب:**
- إظهار رقم الخطأ (`OracleException.Number`) + رسالة الخطأ (`OracleException.Message`) في رد JSON
- تحديث الواجهة لعرض الخطأ بشكل واضح (رقم ORA + النص)
- الاحتفاظ بالرسائل المترجمة كـ prefix مع التفاصيل التقنية بعدها

### 1.2 N+1 في متصفح المخطط (P0-#2)
**المشكلة:** حالياً يتم استعلام قائمة الجداول (استعلام واحد) ثم لكل جدول استعلام منفصل لجلب الأعمدة. مع 1071 جدولاً في قاعدة Oracle، هذا ينتج 1072 استعلام ويؤدي إلى Timeout (تم إثباته عملياً).

**المطلوب:**
- استبدال الـ foreach بـ **استعلام واحد مجمّع**:
```sql
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE
FROM ALL_TAB_COLUMNS
WHERE OWNER = USER
ORDER BY TABLE_NAME, COLUMN_ID
```
- تجميع النتائج في `Dictionary<string, List<OracleColumnInfo>>` في الـ C#
- الحفاظ على نفس بنية `OracleTableInfo` في الرد

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
| AC-1 | عند خطأ Oracle، يظهر رقم ORA-XXXXX ونص الخطأ في الرد | تنفيذ استعلام خاطئ مثل `SELECT * FROM NON_EXISTENT_TABLE` |
| AC-2 | رسالة الخطأ في الواجهة تعرض رقم ORA والتفاصيل بوضوح (وليس رسالة عامة فقط) | فحص الواجهة بعد AC-1 |
| AC-3 | تحميل المخطط لا يتجاوز 5 ثوانٍ مهما كبرت القاعدة | فتح الصفحة + النقر على "تحديث" في Schema Panel |
| AC-4 | قائمة الجداول تظهر كاملة مع أعمدة كل جدول | النقر على أي جدول وإظهار أعمدته |
| AC-5 | لا ينكسر أي سلوك موجود — تشغيل استعلام، مسح، نسخ CSV، حفظ/تحميل استعلامات | اختبار يدوي للوظائف الأساسية |
| AC-6 | `dotnet build` ينجح بدون أخطاء | `dotnet build` |

---

## 4. تفاصيل التنفيذ

### 4.1 رسائل الخطأ — Index.cshtml.cs

**قبل (حالياً):**
```csharp
catch (OracleException ex)
{
    _logger.LogError(ex, "Oracle Query Lab execution failed (Oracle error).");
    return Json(new { success = false, errorMessage = "تعذر تنفيذ الاستعلام. ..." });
}
```

**بعد:**
```csharp
catch (OracleException ex)
{
    _logger.LogError(ex, "Oracle Query Lab execution failed (Oracle error).");
    return Json(new { success = false, errorMessage = $"خطأ Oracle ({ex.Number}): {ex.Message}" });
}
```

### 4.2 رسائل الخطأ — Index.cshtml

تحديث عرض الخطأ لاستخدام SVG بدل رمز ⚠، وعرض رقم الخطأ بشكل واضح.

### 4.3 N+1 Schema — Index.cshtml.cs

استبدال الكود الحالي (خط 137-176 تقريباً):
```csharp
// 1. Get all tables
await using (var tableCmd = new OracleCommand("SELECT TABLE_NAME FROM ALL_TABLES ..."))
{
    await using var tableReader = await tableCmd.ExecuteReaderAsync();
    // ... foreach tableName in tableNames { query columns separately }
}
```

بـ:
```csharp
// Single query for all columns grouped by table
await using (var cmd = new OracleCommand(
    "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE " +
    "FROM ALL_TAB_COLUMNS WHERE OWNER = USER ORDER BY TABLE_NAME, COLUMN_ID", connection))
{
    await using var reader = await cmd.ExecuteReaderAsync();
    var tableDict = new Dictionary<string, OracleTableInfo>(StringComparer.OrdinalIgnoreCase);
    while (await reader.ReadAsync())
    {
        var tableName = reader.GetString(0);
        if (!tableDict.ContainsKey(tableName))
            tableDict[tableName] = new OracleTableInfo { Name = tableName };
        
        tableDict[tableName].Columns.Add(new OracleColumnInfo { ... });
    }
    var tables = tableDict.Values.OrderBy(t => t.Name).ToList();
}
```

---

## 5. توجيهات تنفيذية للوكيل

1. **Fresh File Read إلزامي:** اقرأ النسخ الحالية من `Index.cshtml.cs` و`Index.cshtml` من القرص أولاً قبل التعديل.
2. لا تمس محتوى غير متعلق بالمهمة.
3. حافظ على بقية السلوك (toast, skeleton, saved queries, copy CSV, schema search, filter, insertAtCursor).
4. في Schema الجديد، استخدم `StringComparer.OrdinalIgnoreCase` للـ Dictionary.
5. قبل إنهاء المهمة، شغّل:
   ```text
   dotnet build --no-restore
   ```

## 6. Handback المطلوب

1. الملفات المعدلة (المسارات الكاملة)
2. مقتطفات الكود الجديد الأساسية
3. نتيجة `dotnet build`
4. أي مخاطر أو ملاحظات ظهرت

---

## 7. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| أصغر وحدة آمنة | ✅ PASS | مهمتان في نفس الملف — لا تعارض |
| هدف واحد واضح | ✅ PASS | تحسين رسائل الخطأ + أداء Schema Browser |
| لا أعمال مؤجلة | ✅ PASS | كل ما هو مطلوب للفجوتين |
| لا تغيير في Auth/Security | ✅ PASS | لا مساس بالمصادقة أو الصلاحيات |
| لا تغيير في Schema | ✅ PASS | لا ميجريشن |
| لا API جديد | ✅ PASS | تعديل في handlers الموجودة |
| Allowed Write Targets ضيقة | ✅ PASS | ملفين فقط |
| معايير القبول قابلة للاختبار | ✅ PASS | 6 معايير واضحة |
| لا أسرار في المخرجات | ✅ PASS | لا كلمة مرور أو توكن |

**Gate Status:** ✅ PASS

---

## 8. Auditor Decision (initial)

**Expected:** AUDITOR_REVIEW_REQUIRED

**Reason:** مهمة تمس أداء وحساسية أخطاء الاتصال بقاعدة بيانات — تستحق تدقيقاً.
