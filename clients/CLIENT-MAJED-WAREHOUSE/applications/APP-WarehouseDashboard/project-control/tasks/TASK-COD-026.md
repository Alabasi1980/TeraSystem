# TASK-COD-026 — Card Builder Visual UX (Wizard + Live Preview)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-026 |
| **المجموعة** | B10 — UX Enhancement |
| **المرحلة** | Phase 6 — Implementation (Enhancement) |
| **الوكيل** | ui-designer + engineering-agent |
| **التقدير** | 25–35h |
| **التبعية** | TASK-COD-009 ✅, TASK-COD-010 ✅, TASK-COD-011 ✅ |
| **الأولوية** | High |
| **الحالة** | 🟢 Accepted (UI Complete) — HTML/CSS/Code-behind ✅ + JS (2 files) ✅ by re-delegated ui-designer |

---

## 1. الهدف

استبدال صفحة إنشاء البطاقة الحالية (`Cards/Create`) بـ **منشئ بصري تفاعلي (Card Builder Wizard)** سهل الاستخدام للمستخدم العادي: خطوات قليلة، اختيارات بالنقر، معاينة حية، وقوالب جاهزة.

## 2. المرجع التصميمي

`project-preparation/CARD_BUILDER_UX_PLAN.md` — الخطة المعتمدة.

## 3. المتطلبات الوظيفية

### 3.1 Wizard من 4 خطوات
| الخطوة | العنوان | التفاعل |
|---|---|---|
| 1 | **اختيار النوع** | 6 بطاقات قابلة للنقر (KPI, Bar, Line, Pie, Table, Gauge) مع أيقونة + اسم |
| 2 | **اختيار المصدر** | قائمة منسدلة مع بحث: Template / Saved Query / Oracle Table / Custom SQL (متقدم) |
| 3 | **الحقول الأساسية** | العنوان (نص)، اسم العرض (نص)، القياس/الحقل (من قائمة منسدلة حسب المصدر) |
| 4 | **الشكل** | الحجم (Tiles 1–12)، الموضع (X/Y أو auto)، الألوان (من الباليت الأزرق)، التحديث (دقيقة/ساعة/إيقاف) |

### 3.2 معاينة حية (Live Preview)
- لوحة جانبية أو سفلية تعرض البطاقة كما ستبدو فعليًا.
- تتحدث فوريًا عند أي تغيير في الخطوات.
- نفس محرك الرسم المستخدم في لوحة التحكم (`DashboardService` + Syncfusion).

### 3.3 قوالب جاهزة (Templates)
- 4–6 قوالب شائعة مخزنة مسبقًا (مثلاً: KPI إجمالي المخزون، Bar اتجاه المبيعات، Pie توزيع الأصناف، Table حركة المخزون).
- عند اختيار Template في الخطوة 2، تُملأ الخطوات 3–4 مسبقًا.

### 3.4 نسخ بطاقة موجودة (Clone)
- زر “نسخ” في قائمة البطاقات (`Cards/Index`) يفتح الـ Wizard مع إعدادات البطاقة المحددة.

### 3.5 خيارات متقدمة (Accordion)
- مخفية افتراضيًا.
- عند التوسيع: SQL يدوي، فلاتر، Drill-down config، تسميات مخصصة، استعلامات مخصصة.

### 3.6 أزرار التنقل
- **التالي / السابق** — تنقل بين الخطوات.
- **حفظ** — ينشئ البطاقة ويعود للقائمة.
- **حفظ وإضافة أخرى** — ينشئ البطاقة ويفتح Wizard جديدًا.

## 4. المكونات التقنية

### 4.1 الواجهة (ui-designer)
| الملف | الوصف |
|---|---|
| `Pages/admin-secure-panel/Cards/Builder.cshtml` | الصفحة الرئيسية للـ Wizard (Layout: `_CardsLayout`) |
| `Pages/admin-secure-panel/Cards/Builder.cshtml.cs` | PageModel مع منطق الخطوات، التحقق، الحفظ |
| `Pages/admin-secure-panel/Cards/BuilderSteps/*.cshtml` | Partials لكل خطوة (اختياري للتنظيم) |
| `wwwroot/js/card-builder.js` | منطق الجانب العميل: معاينة حية، تنقل، تحقق |
| `wwwroot/css/card-builder.css` | تنسيقات خاصة بالـ Wizard (تستخدم توكنات `blue-theme.css`) |

### 4.2 الخدمة/المنطق (engineering-agent)
| الملف | الوصف |
|---|---|
| `Services/CardBuilderService.cs` | خدمة بناء كائن `DashboardCard` من بيانات الـ Wizard |
| `Pages/Api/Dashboard/CardBuilder.cshtml.cs` | Endpoint للمعاينة الحية (POST JSON → يعيد HTML/JSON للبطاقة) |
| تعديل `DashboardService` | إضافة دالة `GetPreviewAsync(CardPreviewRequest)` |

### 4.3 تحديثات قائمة البطاقات
- `Cards/Index.cshtml` — إضافة زر “نسخ” لكل صف + تغيير “جديد” ليفتح Builder.
- `Cards/Create.cshtml` — **إزالة** أو إعادة توجيه للـ Builder.

## 5. Allowed Write Targets

```
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/BuilderSteps/
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/wwwroot/js/card-builder.js
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/wwwroot/css/card-builder.css
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Services/CardBuilderService.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/Api/Dashboard/CardBuilder.cshtml.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/Shared/DashboardService.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Index.cshtml
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Create.cshtml
```

## 6. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | Wizard من 4 خطوات يعمل بالكامل | ⬜ |
| AC-2 | معاينة حية تتحدث عند كل تغيير | ⬜ |
| AC-3 | 4+ قوالب جاهزة متاحة | ⬜ |
| AC-3 | زر “نسخ” في القائمة يفتح Builder بإعدادات البطاقة | ⬜ |
| AC-4 | خيارات متقدمة مخفية (Accordion) | ⬜ |
| AC-5 | لا حقول SQL إجبارية للمستخدم العادي | ⬜ |
| AC-6 | التصميم متماشٍ مع `_CardsLayout` + `blue-theme.css` | ⬜ |
| AC-7 | RTL، عربي، خطوط Cairo | ⬜ |
| AC-8 | `dotnet build -c Release` = 0 errors / 0 warnings | ⬜ |
| AC-9 | لا secrets hardcoded | ⬜ |

## 7. ملاحظات تنفيذية

- **لا تغيير** على نموذج `DashboardCard` أو جداول DB.
- المعاينة الحية تستدعي نفس المنطق المستخدم في لوحة التحكم لضمان التطابق 100%.
- الـ Templates تخزن كبيانات ثابتة (JSON/Resource) أو في DB لاحقًا — حاليًا في كود الخدمة.
- `CardBuilderService` تعيد كائن `DashboardCard` جاهز للحفظ عبر EF Core المعتاد.

## 8. التبعية

- تعتمد على `DashboardService` الحالي + `DashboardCard` model + Syncfusion Grid/Charts.
- `ui-designer` مسؤولة عن: UI، Wizard flow، Preview pane، CSS، JS.
- `engineering-agent` مسؤولة عن: `CardBuilderService`، Preview API، دمج الخدمة، تحديث Index/Create.

## 9. التسلسل المقترح

1. `ui-designer` — Builder UI (Layout, Steps, Preview pane, CSS, JS)
2. `engineering-agent` — `CardBuilderService` + Preview API
3. دمج: ربط UI بالخدمة، اختبار المعاينة الحية
4. تحديث `Index` (إضافة Clone، تحويل New → Builder)
5. إزالة/إعادة توجيه `Create.cshtml`
6. بناء نهائي + تحقق القبول