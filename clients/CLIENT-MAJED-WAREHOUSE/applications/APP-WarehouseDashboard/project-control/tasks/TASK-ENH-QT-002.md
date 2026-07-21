# TASK-ENH-QT-002 — Frontend: Schema Browser + SELECT Builder

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** High

---

## 1. الوصف

تطوير واجهة Schema Browser و SELECT Builder لصفحة QueryTester (`/admin-secure-panel/QueryTester`).

**يعتمد على:** TASK-ENH-QT-001 (مكتمل ✅ — الـ Backend API جاهز)

---

## 2. التغييرات المطلوبة

### ملف واحد فقط: `Pages/admin-secure-panel/QueryTester/Index.cshtml`

أعد تصميم الصفحة إلى **تخطيط من جزئين**:
- **الجزء الأيسر (300px):** Schema Browser + Source Selector
- **الجزء الأيمن (main):** محرر SQL + SELECT Builder + النتائج

### A. Source Selector (أعلى اليسار أو أعلى الصفحة)
تبويبان (Tabs) لاختيار مصدر البيانات:
```
[ 🗄️ SQL Server ]  [ 🗄️ Oracle ]
```
- الافتراضي: SQL Server
- عند تغيير المصدر: إعادة تحميل قائمة الجداول

### B. Schema Browser (اللوحة اليسرى)
شجرة قابلة للطي:
```
📂 SQL Server (أو Oracle)
  ├── 📋 dbo.Items
  │    ├── 🔹 ItemId (int)
  │    ├── 🔹 ItemName (nvarchar)
  │    └── 🔹 Price (decimal)
  ├── 📋 dbo.Sales
  │    ├── 🔹 SaleId (int)
  │    └── 🔹 ...
  └── 📋 dbo.Stock
       └── ...
```

**الوظائف:**
- ضغطة على `📋 TableName` → توسّع لإظهار الأعمدة (API: `OnGetColumnsAsync`)
- ضغطة على `🔹 ColumnName` → تضاف إلى SELECT Builder (أو تضاف إلى الـ SQL النشط)
- أيقونات: `📂` للمصدر، `📋` للجدول، `🔹` للعمود
- مؤشر تحميل (loading spinner) أثناء جلب الأعمدة
- عند تبديل المصدر: إعادة تحميل الجداول

### C. SELECT Builder (في الجزء الأيمن، فوق محرر SQL)
شريط أدوات بصري لبناء جملة SELECT:
```
[ اختر جدول: dropdown list ↓ ]  [ ✔ أعمدة محددة ]

بعد اختيار الجدول، تظهر الأعمدة كـ Checkboxes:
☑ ItemId    ☑ ItemName    ☑ Price    ☐ Quantity    ☐ Category

[ إنشاء SELECT ] ← زر يولد: SELECT ItemId, ItemName, Price FROM Items
```

**الوظائف:**
- Dropdown لاختيار الجدول (يُعبّر من API `OnGetTablesAsync`)
- عند اختيار جدول: جلب الأعمدة وعرضها كـ Checkboxes
- اختيار أعمدة متعددة → زر "إنشاء SELECT" يضع SQL في المحرر
- اختيار الكل / إلغاء الكل

### D. دمج مع المحرر الحالي
- الـ SQL المُتولد يُوضع في `textarea#sqlInput` الموجود
- تبقى أزرار "تشغيل"، "مسح"، "نسخ" كما هي
- يبقى Toast container كما هو
- تبقى Skeleton loading و empty state كما هي

---

## 3. الـ API المتاح (من Task 1)

| الوظيفة | الطلب | الرد |
|---------|-------|------|
| **قائمة الجداول** | `GET ?handler=Tables&source=SqlServer` | `[{ schema:"dbo", tableName:"Items" }]` |
| **قائمة الأعمدة** | `GET ?handler=Columns&source=SqlServer&table=Items` | `[{ name:"ItemId", dataType:"int", nullable:"NO" }]` |
| **تشغيل استعلام** | `POST ?handler=Run` مع `{ sql:"...", source:"SqlServer" }` | `{ success, columns, rows, ... }` |

يمكن استخدام `fetch` عادي مع AntiForgeryToken.

---

## 4. Design Guidelines

- استخدم **نفس CSS Variables** من Blue Identity Palette (`--c-primary: #1F4E79`، إلخ)
- استخدم **SVG أيقونات** (كتب ترميز، مجلدات، جداول، أعمدة) — لا emoji
- اللوحة اليسرى: `width: 280px` أو `300px`، خلفية `--c-surface`، حد `--c-border`
- التخطيط: `display: flex; gap: 16px;` للصفحة الرئيسية
- متجاوب: تحت 768px → اللوحة اليسرى تختفي أو تتحول toggle
- استخدم `fadeInUp` للأنميشن
- نفس الخط (Cairo) ونظام الألوان

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml`

**ممنوع تعديل:** أي ملف آخر (خاصة ملف الـ Backend Index.cshtml.cs).

---

## 6. Acceptance Criteria

1. ✅ مصدران: SQL Server و Oracle (تبويبان)
2. ✅ Schema Browser: قائمة جداول مع expand/collapse للأعمدة
3. ✅ ضغطة على عمود → يستخدم في SELECT Builder
4. ✅ SELECT Builder: Dropdown جدول + Checkboxes أعمدة
5. ✅ زر "إنشاء SELECT" يضع SQL في `textarea#sqlInput`
6. ✅ اختيار الكل / إلغاء الكل للأعمدة
7. ✅ تصميم متجاوب (Desktop + Tablet + Mobile)
8. ✅ لا emoji — SVG أيقونات فقط
9. ✅ `dotnet build` PASS — 0 errors, 0 warnings

---

## 7. Fresh File Read Rule (إلزامي)

**اقرأ الملف من القرص قبل التعديل.** احتفظ بكل التغييرات الحالية (خاصة محتوى الصفحة الحالي: محرر SQL، الأزرار، النتائج، الـ toast، الـ skeleton).

---

## 8. After Completion

أعد:
1. ملخص التغييرات
2. تأكيد `dotnet build` PASS
3. وصف الهيكل الجديد للصفحة
4. Auditor Decision: NOT_REQUIRED
