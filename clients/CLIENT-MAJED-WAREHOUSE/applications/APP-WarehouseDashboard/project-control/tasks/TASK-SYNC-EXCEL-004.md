# TASK-SYNC-EXCEL-004 — Public Data Exports Page + Dashboard Link

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-EXCEL-004 |
| **المجموعة** | Sync Enhancement — Export Excel |
| **النوع** | Frontend (New Razor Page + Link) |
| **الوكيل** | ui-designer |
| **الأولوية** | High |
| **الحالة** | 🟡 Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-21 |
| **التبعية** | TASK-SYNC-EXCEL-003 ✅ يجب أن يكون مكتملاً أولاً |

---

## 1. الهدف

إنشاء صفحة عامة (غير محمية) لتنزيل ملفات Excel لجداول المزامنة، مع رابط من الصفحة الرئيسية.

**الجمهور:** مستخدمي اللوحة الرئيسية (غير الأدمن)
**الرابط المقترح:** `/DataExports`

---

## 2. ما يجب تنفيذه

### 2.1 إنشاء صفحة Razor جديدة

**الملف:** `src/WarehouseDashboard.Web/Pages/DataExports/Index.cshtml`
**الملف:** `src/WarehouseDashboard.Web/Pages/DataExports/Index.cshtml.cs`

#### PageModel (Index.cshtml.cs)

```
namespace WarehouseDashboard.Web.Pages.DataExports;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger);

    /// <summary>List of exportable mappings from the API.</summary>
    public List<ExportableMapping> Mappings { get; set; } = new();

    /// <summary>True if the API could not be reached.</summary>
    public bool ApiError { get; set; }

    public async Task<IActionResult> OnGetAsync();

    public class ExportableMapping
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SqlTargetTable { get; set; }
        public string SourceType { get; set; }
        public bool HasData { get; set; }
        public int? RowCount { get; set; }
        public DateTime? LastSyncTime { get; set; }
    }
}
```

**التفاصيل:**
- اقرأ `SyncApi:BaseUrl` من `_configuration` (موجود في Index.cshtml.cs كنمط)
- اتصل بـ `GET {apiBase}/api/sync/exportable-mappings`
- خزّن النتائج في `Mappings`
- إذا فشل الاتصال → `ApiError = true`

### 2.2 تصميم الواجهة (Index.cshtml)

**استخدم `_DashboardLayout.cshtml`** (نفس Layout اللوحة الرئيسية)

**التصميم:**

```
┌─────────────────────────────────────────────────────────────┐
│  📥 تنزيل البيانات                                           │
│  اختر الجدول الذي تريد تنزيل بياناته بصيغة Excel منسقة وجاهزة│
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────────┐│
│  │ 🗂️  stg_ST_ITEM_CARD                                   ││
│  │    بطاقات الأصناف — Query                               ││
│  │    📅 آخر مزامنة: 21/07/2026 09:48                       ││
│  │    📊 45,230 سجل                                        ││
│  │                                   [📥 تنزيل Excel]      ││
│  └─────────────────────────────────────────────────────────┘│
│                                                             │
│  ┌─────────────────────────────────────────────────────────┐│
│  │ 🗂️  stg_ST_WAREHOUSES                                  ││
│  │    المستودعات — Query                                    ││
│  │    📅 آخر مزامنة: 21/07/2026 09:48                       ││
│  │    📊 12,450 سجل                                        ││
│  │                                   [📥 تنزيل Excel]      ││
│  └─────────────────────────────────────────────────────────┘│
│                                                             │
│  (إذا ما في بيانات → رسالة "لا توجد جداول للتصدير بعد")     │
│  (إذا خطأ API → رسالة "تعذر الاتصال بالخادم")               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**المتطلبات البصرية:**
- كل جدول في **Card** منفصل (نفس نمط `wd-card`)
- أيقونة 🗂️ أو SVG للمجلد
- اسم الجدول (sqlTargetTable) بخط عريض `code`
- وصف/اسم التعيين بخط عادي
- شارة نوع المصدر (Query/Table/View) — استخدم دالة `getSourceTypeBadge` الموجودة
- 📅 آخر مزامنة مع تنسيق التاريخ
- 📊 عدد السجلات مع تنسيق فاصل الآلاف
- زر 📥 تنزيل Excel (نفس style `wd-btn wd-btn--primary`)
- **Skeleton loading** أثناء جلب البيانات (3-4 بطاقات shimmer)
- **Empty state** إذا ما في جداول
- **Enter/Exit animations** (`wdFadeUp`)
- **Responsive**: للجوال بطاقة بعرض كامل، للتابلت/dekstop شبكة بعمودين

### 2.3 إضافة رابط في الصفحة الرئيسية

**الملف:** `src/WarehouseDashboard.Web/Pages/Shared/_Header.cshtml`

في قسم `wd-topbar__actions` لغير الأدمن (`@if (isAdmin)`...`else`)، أضف زر/رابط قبل زر التحديث:

```html
<a class="wd-btn wd-btn--on-dark wd-btn--sm" href="/DataExports" title="تنزيل ملفات Excel">
    📥 تنزيل البيانات
</a>
```

أو أيقونة أصغر:
```html
<a class="wd-btn wd-btn--ghost wd-btn--sm" href="/DataExports" title="تنزيل ملفات Excel" style="color:#fff;border-color:rgba(255,255,255,0.2);">
    📥
</a>
```

### 2.4 وظيفة التحميل

كل زر تنزيل يستدعي دالة JS:

```javascript
window.downloadFromExport = async function(mappingId, tableName) {
    try {
        var resp = await fetch(apiBase + '/api/sync/' + mappingId + '/export-excel');
        if (!resp.ok) throw new Error('HTTP ' + resp.status);
        
        // Get filename
        var disposition = resp.headers.get('Content-Disposition');
        var filename = (tableName || 'export') + '.xlsx';
        // ... (extract filename from disposition if available)
        
        var blob = await resp.blob();
        var url = URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        
        showToast('✅ تم تحميل ' + tableName, 'success');
    } catch (err) {
        showToast('❌ فشل التحميل: ' + (err.message || ''), 'error');
    }
};
```

---

## 3. الملفات المسموح كتابتها

| الملف | الإجراء |
|---|---|
| `src/WarehouseDashboard.Web/Pages/DataExports/Index.cshtml` | **إنشاء جديد** — صفحة التحميل (HTML + CSS + JS) |
| `src/WarehouseDashboard.Web/Pages/DataExports/Index.cshtml.cs` | **إنشاء جديد** — Page Model |
| `src/WarehouseDashboard.Web/Pages/Shared/_Header.cshtml` | تعديل — إضافة رابط لصفحة DataExports |
| `src/WarehouseDashboard.Web/Pages/Index.cshtml` | تعديل — إضافة رابط (اختياري إذا بديل عن الـ Header) |

**ممنوع:**
- لا تعديل أي API endpoint
- لا إضافة مكتبات خارجية
- لا تغيير هيكل الموقع الأساسي

---

## 4. معايير القبول

- [ ] **AC1:** صفحة `/DataExports` تظهر ولها تصميم جميل ومنظم
- [ ] **AC2:** كل جدول معروض بـ Card مع: اسم الجدول، النوع، آخر مزامنة، عدد السجلات
- [ ] **AC3:** كل Card فيها زر 📥 تحميل Excel
- [ ] **AC4:** الضغط على 📥 يحمّل ملف Excel منسق
- [ ] **AC5:** شاشة Skeleton أثناء التحميل
- [ ] **AC6:** Empty state إذا ما في جداول جاهزة
- [ ] **AC7:** رسالة خطأ إذا API مش شغال
- [ ] **AC8:** Toast ✅ عند نجاح التحميل، ❌ عند الفشل
- [ ] **AC9:** رابط "تنزيل البيانات" يظهر في الـ Header للصفحة الرئيسية
- [ ] **AC10:** Build 0 Errors / 0 Warnings
- [ ] **AC11:** لا أخطاء JS في Console
- [ ] **AC12:** متجاوب مع الجوال
- [ ] **AC13:** Animations دخول البطاقات (wdFadeUp)

---

## 5. Pre-Execution Gate

| البند | الحالة |
|---|---|
| التكنولوجي بروفايل | `dotnet-razorpages-adonet` ✅ |
| الملفات المرجعية مقروءة | ✅ |
| النطاق محدد وواضح | ✅ |
| المسموح والممنوع محدد | ✅ |
| الـ AC كامل | ✅ |
| التبعية: TASK-SYNC-EXCEL-003 | ⏳ يجب أن يكون Accepted أولاً |
| الـ Auditor: NOT_REQUIRED | ✅ |

**PASS ✅ — جاهز للتفويض بعد إكمال TASK-SYNC-EXCEL-003**

---

## 6. ملاحظات إضافية

- **قبل الكتابة:** اقرأ الملفات من القرص (Fresh File Read Rule)
- `SyncApi:BaseUrl` يقرأ من `appsettings.json` — استخدم `_configuration["SyncApi:BaseUrl"]`
- اعمل `TryParse` للرابط (default: `http://localhost:5001`)
- `_DashboardLayout.cshtml` موجود وبيحتوي كل الـ CSS والـ fonts
- استخدم نفس ألوان `blue-theme.css` (متغيرات CSS)
- لا تنسَ `@Html.AntiForgeryToken()` في صفحة Razor

## Handback — 2026-07-21
**الحالة:** ✅ مكتمل
**Build:** 0 errors / 0 warnings
**الملفات المنشأة:**
- Pages/DataExports/Index.cshtml
- Pages/DataExports/Index.cshtml.cs
**الملفات المعدلة:**
- Pages/Shared/_Header.cshtml
**المراجع البصرية:**
- design-source/REFERENCES.md

### ملخص التنفيذ

#### Index.cshtml.cs (Page Model)
- يقرأ `SyncApi:BaseUrl` من `appsettings.json` (الافتراضي: `http://localhost:5001`)
- يتصل بـ `GET {apiBase}/api/sync/exportable-mappings`
- يخزّن النتائج في `Mappings`، ويضبط `ApiError` إذا فشل الاتصال
- يعرض `SyncApiBaseUrl` للـ JavaScript client-side

#### Index.cshtml (UI)
- **Breadcrumb:** الرئيسية › تنزيل البيانات (نمط wd-breadcrumb-row)
- **Skeleton Loading:** 4 بطاقات shimmer (wd-export-skel-card) مع wdShimmer وwdFadeUp
- **Cards Grid:** شبكة بعمودين (Desktop) / عمود واحد (Mobile) — `var(--sp-4)` gap
- **كل Card تحتوي:**
  - أيقونة 🗂️، اسم الجدول `<code>`، اسم التعيين، شارة النوع (Query/Table/View) مع تلوين
  - 📅 آخر مزامنة، 📊 عدد السجلات (بصيغة N0)
  - زر 📥 تنزيل Excel (نمط wd-btn--download)
- **Empty State:** "لا توجد جداول للتصدير بعد" مع أيقونة 📭
- **Error State:** "تعذر الاتصال بخادم البيانات" مع زر إعادة محاولة
- **Animations:** wdFadeUp مع تأخير 70ms لكل بطاقة
- **JS:** `downloadFromExport(mappingId, tableName)` — تحميل blob، حفظ باسم `tableName.xlsx`، Toast ✅/❌
- **Loading spinner** على زر التنزيل أثناء التحميل

#### _Header.cshtml
- أضيف رابط 📥 تنزيل (wd-btn--on-dark wd-btn--sm) قبل زر التحديث لغير الأدمن

#### الـ CSS
- كل الأنماط تستخدم CSS variables من blue-theme.css (لا Bootstrap)
- RTL كامل مع logical properties
- متجاوب: 768px كسر لعمود واحد
