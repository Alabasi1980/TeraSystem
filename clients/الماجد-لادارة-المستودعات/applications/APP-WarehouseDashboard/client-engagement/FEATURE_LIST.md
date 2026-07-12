# FEATURE_LIST.md — WarehouseDashboard

## Metadata

| Field | Value |
|---|---|
| Client | الماجد لادارة المستودعات |
| Application | WarehouseDashboard |
| Prepared by | TCEA |
| Date | 2026-07-12 |
| Last Updated | 2026-07-12 |
| Project Classification | **Medium** |
| Phase | Phase 1 (MVP) — Phase 2 مؤجل |

---

## Feature Categories

| الكود | الفئة | الوصف |
|:-----:|-------|-------|
| **API** | WarehouseDashboard.API | تطبيق الـ API — استخراج البيانات ومزامنتها |
| **RZR** | WarehouseDashboard.Web | تطبيق Razor — Dashboard + Admin Panel |
| **INF** | Infrastructure | البنية التحتية العامة |

---

## Phase 1 — MVP (In Scope)

### API-01: Oracle Connection & Data Extraction `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | الاتصال بقاعدة Oracle واستخراج الجداول المطلوبة |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |
| **الملاحظات** | تفاصيل الجداول تُحدد أثناء التنفيذ — الزبون متاح |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| API-01.1 | Oracle Connection Setup | تكوين اتصال ODP.NET مع Oracle | P1 |
| API-01.2 | Table Discovery | اكتشاف الجداول المطلوبة (أثناء التنفيذ) | P1 |
| API-01.3 | Data Type Mapping | تحويل أنواع بيانات Oracle → SQL Server | P1 |
| API-01.4 | Error Handling | معالجة أخطاء الاتصال والاستخراج | P1 |

---

### API-02: SQL Server Data Loading (Full Refresh) `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | حذف البيانات الموجودة + إدخال البيانات الجديدة في SQL Server |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |
| **الملاحظات** | طريقة "حذف الموجود + إضافة الجديد" |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| API-02.1 | Full Refresh Mode | حذف جميع الصفوف + إدخال جديد | P1 |
| API-02.2 | Bulk Insert | إدخال مجمّع للأداء العالي | P1 |
| API-02.3 | Transaction Management | معاملة واحدة — إما كل شيء أو لا شيء | P1 |
| API-02.4 | Table Order Management | ترتيب الجداول حسب الـ FK dependencies | P1 |

---

### API-03: Synchronization Engine `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | محرك المزامنة — تلقائي + يدوي |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |
| **الملاحظات** | المزامنة التلقائية بفترة TBD — يدوية عبر زر |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| API-03.1 | Manual Sync Endpoint | POST /api/sync/trigger — مزامنة يدوية | P1 |
| API-03.2 | Auto-Sync Scheduler | Background Service للمزامنة التلقائية | P1 |
| API-03.3 | Sync Status API | GET /api/sync/status — حالة المزامنة الحالية | P1 |
| API-03.4 | Sync Configuration | إعدادات المزامنة (فترة التلقائي، الجداول) | P1 |

---

### API-04: Synchronization Logs `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | تسجيل كل عملية مزامنة |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| API-04.1 | Sync Log Entry | تسجيل: وقت البدء، وقت الانتهاء، الحالة، عدد السجلات، المدة | P1 |
| API-04.2 | Log Storage | حفظ السجلات في SQL Server | P1 |
| API-04.3 | Log API | GET /api/sync/logs — جلب السجلات | P1 |

---

### RZR-01: Dashboard Layout & Rendering `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | العرض الرئيسي للداشبورد مع ~20 بطاقة ديناميكية |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |
| **الملاحظات** | Syncfusion + Drill Down + لوحة ألوان محددة |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| RZR-01.1 | Responsive Grid Layout | تخطيط شبكي متجاوب | P1 |
| RZR-01.2 | Dynamic Card Rendering | قراءة التكوين + بناء البطاقات ديناميكياً | P1 |
| RZR-01.3 | Syncfusion Components | استخدام مكونات Syncfusion | P1 |
| RZR-01.4 | Blue Theme Application | تطبيق الهوية البصرية المحددة (11 لوناً) | P1 |

---

### RZR-02: Drill Down Functionality `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | التعمق في البيانات عبر مستويات متعددة |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| RZR-02.1 | Per-Card Drill Down | كل بطاقة لها مستويات قابلة للتكوين | P1 |
| RZR-02.2 | Breadcrumb Navigation | شريط تنقل يعرض المستوى الحالي | P1 |
| RZR-02.3 | Back Navigation | العودة للمستوى السابق | P1 |

---

### RZR-03: Data Binding & SQL Execution `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | تنفيذ الاستعلامات وربط النتائج بالبطاقات |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| RZR-03.1 | SQL Query Execution | تنفيذ استعلامات SQL من التكوين | P1 |
| RZR-03.2 | View Execution | تنفيذ Views من التكوين | P1 |
| RZR-03.3 | Data-to-Chart Binding | ربط النتائج بالأنواع المختلفة | P1 |

---

### RZR-04: Sync Status & Controls `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | عرض حالة المزامنة + زر المزامنة اليدوية |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| RZR-04.1 | Last Sync Display | عرض آخر وقت مزامنة | P1 |
| RZR-04.2 | Sync Status Indicator | مؤشر الحالة (نجاح/فشل/قيد التنفيذ) | P1 |
| RZR-04.3 | Manual Sync Button | زر لتفعيل المزامنة اليدوية | P1 |
| RZR-04.4 | Sync Logs Viewer | عرض سجلات المزامنة | P1 |

---

### RZR-05: Admin Panel `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | لوحة تحكم ديناميكية لإدارة البطاقات |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |
| **الملاحظات** | صفحة منفصلة محمية بكلمة مرور، غير مرئية في القائمة |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| RZR-05.1 | Password Protection | كلمة مرور للدخول | P1 |
| RZR-05.2 | Hidden URL Access | غير مرئية في القائمة — وصول عبر الرابط فقط | P1 |
| RZR-05.3 | Card CRUD | إنشاء / تعديل / حذف بطاقات | P1 |
| RZR-05.4 | Data Source Config | تحديد مصدر البيانات (SQL Query / View) | P1 |
| RZR-05.5 | Chart Type Selection | اختيار نوع العرض (Bar, Line, Pie, KPI, Table) | P1 |
| RZR-05.6 | Drill Down Config | تحديد مستويات الـ Drill Down والربط | P1 |
| RZR-05.7 | Card Layout Config | تحديد حجم وترتيب البطاقات | P1 |
| RZR-05.8 | Config Save/Load | حفظ التكوين في قاعدة البيانات | P1 |
| RZR-05.9 | Query Tester | اختبار الاستعلام قبل الحفظ | P1 |

---

### INF-01: Project Structure & Setup `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | هيكل المشروع والإعداد الأساسي |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| INF-01.1 | .NET 8 Solution | حل يحتوي على مشروعين | P1 |
| INF-01.2 | API Project | مشروع WarehouseDashboard.API | P1 |
| INF-01.3 | Razor Project | مشروع WarehouseDashboard.Web | P1 |
| INF-01.4 | Shared Library (اختياري) | مكتبة مشتركة للـ Models والـ DTOs | P2 |

---

### INF-02: Database Setup `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | إعداد قاعدة البيانات لتخزين التكوين والسجلات |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| INF-02.1 | Config Database Schema | جداول تخزين تكوين البطاقات | P1 |
| INF-02.2 | Sync Logs Schema | جداول تخزين سجلات المزامنة | P1 |
| INF-02.3 | Data Tables Schema | جداول تخزين البيانات المنقولة (TBD أثناء التنفيذ) | P1 |

---

### INF-03: IIS Hosting `[Confirmed by Majed]`

| البند | التفاصيل |
|-------|----------|
| **الوصف** | إعداد النشر على IIS |
| **الأولوية** | **P1 — Must-have** |
| **المصدر** | Majed |

**المكونات الفرعية:**

| # | المكون | الوصف | الأولوية |
|:-:|--------|-------|:--------:|
| INF-03.1 | IIS Configuration | إعداد IIS للتطبيقين | P1 |
| INF-03.2 | Application Pool | إنشاء Application Pool مناسب | P1 |
| INF-03.3 | URL Routing | تكوين المسارات | P1 |

---

## Phase 2 — مؤجل (Deferred)

| # | الميزة | الوصف | السبب |
|:-:|--------|-------|-------|
| D-1 | شاشات تعديل البيانات | إضافة وتعديل وحذف بيانات في SQL Server | مؤجل مبكراً بتأكيد Majed |
| D-2 | صلاحيات المستخدمين | نظام login + Role-Based Access | غير محدد بعد |
| D-3 | Export to Excel/PDF | تصدير البيانات من Dashboard | P3 — يمكن إضافته لاحقاً |
| D-4 | Advanced Analytics | تحليلات متقدمة وتقارير | P3 — يمكن إضافته لاحقاً |

---

## Out of Scope (خارج النطاق)

| # | الميزة | السبب |
|:-:|--------|-------|
| O-1 | Cloud Hosting | المشروع محلي على IIS |
| O-2 | Mobile App | لا يوجد طلب |
| O-3 | Microservices | غير مطلوب — بنية بسيطة |
| O-4 | CI/CD Pipeline | مشروع محلي — غير مطلوب حالياً |
| O-5 | Automated Testing | يمكن إضافته لاحقاً إذا احتجنا |

---

## Feature Count Summary

| الحالة | العدد |
|:------:|:-----:|
| **In Scope (P1)** | 33 مكوناً فرعياً في 9 ميزات رئيسية |
| **Phase 2 (Deferred)** | 4 ميزات |
| **Out of Scope** | 5 ميزات |
| **المجموع** | 42 ميزة |

---

## تاريخ الإنشاء

2026-07-12
**آخر تحديث:** 2026-07-12 — إنشاء أولي
