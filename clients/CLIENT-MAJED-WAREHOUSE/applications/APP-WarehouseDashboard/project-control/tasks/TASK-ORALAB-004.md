# TASK-ORALAB-004 — OracleQueryLab: Horizontal Scroll for Wide Results

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ORALAB-004 |
| **المجموعة** | ORACLE-QUERY-LAB-ENHANCEMENT |
| **النوع** | Frontend CSS |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | P1 |
| **الحالة** | ✅ ACCEPTED (build PASS) |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

**المشكلة:** عندما يُرجع الاستعلام أعمدة كثيرة أو بيانات عريضة، الجدول يتجاوز عرض الحاوية ويُقصّ بدون أن يعرف المستخدم.

**المطلوب:** تفعيل `overflow-x: auto` على حاوية النتائج بحيث يمكن التمرير أفقياً.

---

## 2. النطاق

### المطلوب

1. إضافة `overflow-x: auto` على `.wd-grid` (حاوية `#gridContainer`)
2. إضافة `min-width` على الجدول لضمان عدم انكماش الأعمدة
3. `white-space: nowrap` على خلايا `td` لمنع التفاف النص

### غير المطلوب

- لا تغيير في Backend
- لا تغيير في JS logic
- لا مكتبات جديدة

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\OracleQueryLab\Index.cshtml
```

---

## 4. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | `.wd-grid` يحتوي `overflow-x: auto` |
| AC-2 | الجدول يحتوي `min-width` (مثلاً `min-width: max-content`) |
| AC-3 | خلايا `td` و `th` تحتوي `white-space: nowrap` |
| AC-4 | `dotnet build` ناجح بدون أخطاء |
| AC-5 | لا تراجع على الميزات السابقة |

---

## 5. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ CSS فقط — تعديل بسيط |
| لا تغيير Backend | ✅ |
| لا تغيير Auth | ✅ |
| Allowed Write Targets ضيقة | ✅ ملف واحد |
| معايير القبول قابلة للاختبار | ✅ |

**Gate Status:** ✅ PASS
