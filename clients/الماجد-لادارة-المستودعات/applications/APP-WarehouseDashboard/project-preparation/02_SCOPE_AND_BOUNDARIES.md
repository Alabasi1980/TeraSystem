# 02_SCOPE_AND_BOUNDARIES.md — WarehouseDashboard

**Status:** `preparation`
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** TeraAgent — Task TASK-PREP-003
**Date:** 2026-07-12
**Source Documents:** 01_PROJECT_BRIEF.md, APPLICATION_BLUEPRINT.md, FEATURE_LIST.md, CLIENT_DECISION_LOG.md

---

## 1. In Scope — Phase 1 (Core MVP)

يشمل Phase 1 12 ميزة أساسية تمثل أصغر نسخة قابلة للاستخدام (MVP). جميعها مصنفة P1 — Must-have ومعتمدة من Majed.

### Sync Engine (API)

| الكود | الميزة | الوصف | المرجع |
|:-----:|--------|-------|:------:|
| **API-01** | Oracle Connection & Data Extraction | الاتصال بقاعدة Oracle عبر ODP.NET واستخراج الجداول مع Data Type Mapping | FEATURE_LIST.md, CLIENT_DECISION_LOG.md #5, #7, #10, #22 |
| **API-02** | SQL Server Data Loading (Full Refresh) | حذف جميع الصفوف + إدخال مجمّع عبر ADO.NET SqlBulkCopy في معاملة واحدة | FEATURE_LIST.md, CLIENT_DECISION_LOG.md #5, #21 |
| **API-03** | Synchronization Engine (Auto + Manual) | Background Service للمزامنة التلقائية + POST /api/sync/trigger يدوي + GET /api/sync/status | FEATURE_LIST.md, APPLICATION_BLUEPRINT.md §9 |
| **API-04** | Synchronization Logs | تسجيل وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة لكل عملية Sync | FEATURE_LIST.md, APPLICATION_BLUEPRINT.md §8 |

### Dashboard (Razor)

| الكود | الميزة | الوصف | المرجع |
|:-----:|--------|-------|:------:|
| **RZR-01** | Dashboard Layout & Rendering | ~20 بطاقة ديناميكية في تخطيط شبكي متجاوب مع Syncfusion والموضوع الأزرق (11 لوناً) | FEATURE_LIST.md, CLIENT_DECISION_LOG.md #3, #9 |
| **RZR-02** | Drill Down Functionality | التعمق في البيانات عبر مستويات قابلة للتكوين مع Breadcrumb Navigation | FEATURE_LIST.md, APPLICATION_BLUEPRINT.md §6 |
| **RZR-03** | Data Binding & SQL Execution | تنفيذ SQL Queries/Views من التكوين وربط النتائج بالبطاقات | FEATURE_LIST.md, APPLICATION_BLUEPRINT.md §4 |
| **RZR-04** | Sync Status & Controls | عرض آخر وقت مزامنة + مؤشر الحالة + زر مزامنة يدوية + عارض سجلات | FEATURE_LIST.md, 01_PROJECT_BRIEF.md §4.2 |
| **RZR-05** | Admin Panel (CRUD + Config) | إدارة البطاقات: إنشاء/تعديل/حذف، مصدر البيانات، نوع الرسم البياني، Drill Down، التخطيط، Query Tester. محمي بكلمة مرور، غير مرئي في القائمة | FEATURE_LIST.md, CLIENT_DECISION_LOG.md #4 |

### Infrastructure

| الكود | الميزة | الوصف | المرجع |
|:-----:|--------|-------|:------:|
| **INF-01** | Project Structure (.NET 8) | حل .NET 8 بمشروعين: WarehouseDashboard.API + WarehouseDashboard.Web | FEATURE_LIST.md, CLIENT_DECISION_LOG.md #1 |
| **INF-02** | Database Setup (Config + Logs) | جداول Config (DashboardCards, CardDrillDownLevels, SyncSettings) + جداول SyncLogs + جداول البيانات المنقولة (TBD) | FEATURE_LIST.md, APPLICATION_BLUEPRINT.md §8 |
| **INF-03** | IIS Hosting | نشر محلي على IIS مع Application Pool و URL Routing | FEATURE_LIST.md, CLIENT_DECISION_LOG.md #6 |

---

## 2. Out of Scope

الميزات التالية **خارج نطاق** WarehouseDashboard نهائياً (لا Phase 1 ولا Phase 2):

| الميزة | السبب |
|--------|-------|
| **Cloud Hosting** | المشروع محلي بالكامل على IIS داخل شبكة العميل — لا سحابة (CLIENT_DECISION_LOG.md #7) |
| **Mobile App** | لا يوجد طلب من العميل (FEATURE_LIST.md O-2) |
| **Microservices** | غير مطلوب — بنية بسيطة كافية (FEATURE_LIST.md O-3) |
| **CI/CD Pipeline** | مشروع محلي — لا حاجة لنشر مستمر (FEATURE_LIST.md O-4, 01_PROJECT_BRIEF.md §8 C9) |
| **Automated Testing** | يمكن إضافته لاحقاً إذا احتجنا — خارج النطاق الحالي (FEATURE_LIST.md O-5, 01_PROJECT_BRIEF.md §8 C10) |

---

## 3. Deferred — Phase 2

الميزات التالية مؤجلة **بشكل متعمد** إلى Phase 2 بعد استقرار Core MVP:

| الميزة | الوصف | سبب التأجيل |
|--------|-------|-------------|
| **شاشات تعديل البيانات** (Data Editing Screens) | إضافة وتعديل وحذف بيانات في SQL Server عبر واجهة مستخدم | مؤجل مبكراً بتأكيد Majed — CLIENT_DECISION_LOG.md D4 |
| **صلاحيات المستخدمين** (User Roles / Auth) | نظام login + Role-Based Access لفصل Admin عن Viewer | مؤجل — غير محدد بعد (01_PROJECT_BRIEF.md §3 Viewer, §8 C2) |
| **تصدير Excel/PDF** (Export) | تصدير بيانات Dashboard و图表 إلى Excel أو PDF | P3 — يمكن إضافته لاحقاً (FEATURE_LIST.md D-3) |
| **تحليلات متقدمة** (Advanced Analytics) | تحليلات متقدمة وتقارير إضافية | P3 — يمكن إضافته لاحقاً (FEATURE_LIST.md D-4) |

> **ملاحظة:** الـ Sync Engine مصمم لدعم Incremental Sync من البداية (CLIENT_DECISION_LOG.md #20) — جاهزية تقنية فقط، غير مفعّل في Phase 1 وقد يُفعل في Phase 2 عند الحاجة.

---

## 4. Technical Boundaries

الحدود التقنية للتطبيق:

| البند | الحدود | المرجع |
|-------|--------|:------:|
| **نطاق النشر** | **IIS محلي فقط** — لا سحابة، لا Docker، لا Kubernetes | CLIENT_DECISION_LOG.md #6, #7 |
| **Oracle** | **قراءة فقط (read-only)** — لا كتابة على Oracle نهائياً | CLIENT_DECISION_LOG.md #7 |
| **SQL Server** | **وجهة (destination)** — يخزن البيانات المنقولة + التكوين + السجلات | CLIENT_DECISION_LOG.md #7 |
| **ADO.NET** | **للـ Bulk Insert فقط** — إدخال مجمّع للبيانات المنقولة (CLIENT_DECISION_LOG.md #21) | قرار استراتيجي |
| **Entity Framework Core** | **للتكوين والسجلات فقط** — لا يُستخدم للبيانات المنقولة | CLIENT_DECISION_LOG.md #21 |
| **Syncfusion** | يُستخدم فقط في مشروع Razor Dashboard — ليس في API | CLIENT_DECISION_LOG.md #3 |
| **مزامنة البيانات** | **Full Refresh** فقط في Phase 1 — حذف الكل + إعادة إدخال | CLIENT_DECISION_LOG.md #5 |
| **أمان Admin Panel** | **كلمة مرور واحدة فقط** — لا Role-Based Access — وصول عبر الرابط المباشر فقط | CLIENT_DECISION_LOG.md #4 |
| **قاعدة البيانات** | Oracle و SQL Server على **نفس السيرفر المحلي** | CLIENT_DECISION_LOG.md #7 |
| **سائق Oracle** | **ODP.NET Managed Driver** فقط — لا Oracle Client | 01_PROJECT_BRIEF.md §8 C6 |

---

## 5. Scope Assumptions

الافتراضات التي بُني عليها النطاق:

| # | الافتراض | التأثير | المصدر |
|:-:|----------|:-------:|--------|
| A1 | **تفاصيل جداول Oracle مؤجلة** — تُحدد أثناء التنفيذ. العميل متاح للإجابة عن الأسئلة. | يحدد تعقيد Sync Engine و DB Schema | CLIENT_DECISION_LOG.md #10 |
| A2 | **الفترة الافتراضية للمزامنة التلقائية: 30 دقيقة** — قابلة للتكوين عبر SyncSettings. | يؤثر على تكرار تحميل الخادم | 01_PROJECT_BRIEF.md §10 Q3 |
| A3 | **المسؤول (Admin) موجود على-site** أثناء التطوير للإجابة عن الاستفسارات الفنية. | يقلل مخاطر التأخير | CLIENT_DECISION_LOG.md #10 |
| A4 | **عدد البطاقات ~20 تقريباً** — يُحدد بدقة أثناء التطوير. | يؤثر على حجم التخطيط | 01_PROJECT_BRIEF.md §10 Q4 |
| A5 | **Oracle و SQL Server على نفس السيرفر** — لا حاجة لتكوين شبكة معقد. | يبسط الاتصال | CLIENT_DECISION_LOG.md #7 |
| A6 | **Full Refresh كافٍ لـ Phase 1** — المحرك يدعم Incremental من البداية ولكنه غير مفعّل. | قد نحتاج Incremental إذا كانت البيانات كبيرة (100K+ صف) | CLIENT_DECISION_LOG.md #20 |
| A7 | **لا حاجة لدعم متصفحات قديمة** — التطبيق يستهدف المتصفحات الحديثة (Chrome, Edge, Firefox). | يبسط التوافق مع Syncfusion | افتراض تقني |

---

## 6. Scope Change Process

**أي تغيير في النطاق يجب أن يمر بالعملية التالية:**

1. **طلب الميزة** — يُقدّم طلب الميزة الجديدة أو التعديل من قبل العميل أو الفريق.
2. **توثيق القرار** — يُسجّل القرار في أحد الملفات التالية:
   - `CLIENT_DECISION_LOG.md` — للقرارات المعتمدة من Majed.
   - `CHANGE_REQUESTS.md` — لطلبات التغيير التي تحتاج تقييم قبل الاعتماد.
3. **تقييم الأثر** — يُقيّم أثر التغيير على:
   - الجدول الزمني (ساعات إضافية).
   - الميزانية (تكلفة إضافية).
   - الميزات الأخرى (تبعيات / تعارضات).
4. **اعتماد Majed** — أي تغيير في النطاق يتطلب موافقة خطية من Majed.
5. **تحديث الوثائق** — بعد الاعتماد، تُحدّث جميع الوثائق المتأثرة (PROJECT_BRIEF، SCOPE_AND_BOUNDARIES، TECHNICAL_ARCHITECTURE، إلخ).

### أمثلة على تغييرات النطاق

| الحالة | الإجراء |
|--------|---------|
| ميزة جديدة (مثل Export Excel) | تسجيل في CHANGE_REQUESTS.md → تقييم → اعتماد Majed → تحديث الوثائق |
| تغيير في الميزة الحالية (مثل إضافة Chart Type جديد) | تسجيل في CLIENT_DECISION_LOG.md → اعتماد Majed → تحديث الوثائق |
| إلغاء ميزة من Phase 1 | تسجيل في CLIENT_DECISION_LOG.md مع السبب → نقل إلى Deferred |
| إضافة جدول Oracle جديد | يُحدد أثناء التنفيذ — لا حاجة لتغيير النطاق (ضمن A1) |

> **مبدأ أساسي:** النطاق ليس جامداً — ولكن أي توسع يجب أن يكون مدروساً وموثقاً ومعتمداً.

---

## 7. Scope Summary

| البند | العدد |
|:-----:|:-----:|
| **In Scope (Phase 1)** | 12 ميزة رئيسية (API-01 to API-04, RZR-01 to RZR-05, INF-01 to INF-03) |
| **Out of Scope** | 5 ميزات |
| **Deferred (Phase 2)** | 4 ميزات |
| **Scope Assumptions** | 7 افتراضات |
| **Technical Boundaries** | 10 حدود تقنية |

---

> **Prepared by:** TeraAgent — TASK-PREP-003
> **Date:** 2026-07-12
> **Status:** `preparation`
