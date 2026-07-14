# 04_USERS_ROLES_PERMISSIONS.md — WarehouseDashboard

**Status:** `preparation`
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** TeraAgent — Task TASK-PREP-005
**Date:** 2026-07-12
**Source Documents:** 01_PROJECT_BRIEF.md, APPLICATION_BLUEPRINT.md (§5), CLIENT_DECISION_LOG.md (#4, #8, #9)

---

## 1. Overview

يُعرّف هذا المستند أدوار المستخدمين والصلاحيات وأمن الدخول لتطبيق WarehouseDashboard. يعتمد النظام في **Phase 1** على دور واحد (Admin) مع أمان بكلمة مرور فقط، بينما يُضاف **Phase 2** نظام RBAC كامل بأدوار متعددة ومصادقة متطورة.

| البند | القيمة |
|:-----:|--------|
| **Phase 1 — نموذج الأمان** | Password-protected Admin Panel (hidden URL) + session-based auth |
| **Phase 1 — الأدوار** | Admin فقط (دور واحد) |
| **Phase 2 — النموذج** | RBAC (Role-Based Access Control) مع مصادقة كاملة |
| **Phase 2 — الأدوار** | Admin + Viewer |

---

## 2. Admin Role — Phase 1

### 2.1 الوصف

| البند | التفاصيل |
|:-----:|----------|
| **الدور** | Admin — مسؤول النظام الرئيسي |
| **المرحلة** | Phase 1 (Core MVP) |
| **المصادقة** | كلمة مرور واحدة (BCrypt) عبر صفحة Login مخفية |
| **الوصول** | Dashboard + Admin Panel + Sync Controls + Logs |

### 2.2 الصلاحيات

| # | الصلاحية | النطاق | التفاصيل |
|:-:|----------|:------:|----------|
| P1 | View Dashboard | Dashboard | عرض جميع بطاقات Dashboard مع Drill Down و Breadcrumb |
| P2 | Configure Cards | Admin Panel | إنشاء / تعديل / حذف البطاقات (CRUD) |
| P3 | Configure Data Source | Admin Panel | تحديد SQL Query / View لكل بطاقة |
| P4 | Configure Chart Type | Admin Panel | اختيار نوع الرسم البياني (Bar, Line, Pie, KPI, Table, Gauge) |
| P5 | Configure Drill Down | Admin Panel | تحديد مستويات الـ Drill Down والربط بين البطاقات |
| P6 | Configure Card Layout | Admin Panel | تحديد حجم وترتيب البطاقات |
| P7 | Test Queries | Admin Panel | اختبار الاستعلام مع معاينة النتائج |
| P8 | Trigger Manual Sync | Dashboard | تفعيل المزامنة اليدوية (POST /api/sync/trigger) |
| P9 | View Sync Status | Dashboard | عرض آخر وقت مزامنة + مؤشر الحالة |
| P10 | View Sync Logs | Dashboard/Logs | عرض سجلات المزامنة كاملة (وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة) |
| P11 | Access Admin Panel | Admin Panel | الدخول إلى صفحة الإدارة عبر الرابط المباشر |

### 2.3 تدفق الدخول (Admin Flow)

```
1. المستخدم يفتح الرابط المباشر: /Admin (مخفي — غير موجود في القائمة)
2. تظهر صفحة Login تطلب كلمة المرور
3. إدخال كلمة المرور → التحقق من BCrypt Hash
4. إنشاء Session Cookie (ASP.NET Core Session)
5. إعادة التوجيه إلى صفحة Admin Panel الرئيسية
6. الجلسة صالحة طالما المتصفح مفتوح أو حتى انتهاء Timeout
7. عند الخروج → مسح الـ Session
```

---

## 3. Viewer Role — Phase 2 (Deferred)

### 3.1 الوصف

| البند | التفاصيل |
|:-----:|----------|
| **الدور** | Viewer — مستخدم عادي لعرض Dashboard فقط |
| **المرحلة** | Phase 2 (مؤجل — خارج نطاق Core MVP) |
| **المصادقة** | اسم مستخدم + كلمة مرور (Identity Framework) |
| **الوصول** | Dashboard فقط — بدون Admin Panel أو Sync Controls |

### 3.2 الصلاحيات

| # | الصلاحية | النطاق | التفاصيل |
|:-:|----------|:------:|----------|
| V1 | View Dashboard | Dashboard | عرض جميع بطاقات Dashboard مع Drill Down و Breadcrumb |
| V2 | View Sync Status | Dashboard | عرض آخر وقت مزامنة + مؤشر الحالة (قراءة فقط — لا يمكن تفعيل المزامنة) |

### 3.3 القيود

- لا يمكن الدخول إلى Admin Panel (حتى لو عرف الرابط)
- لا يمكن تفعيل المزامنة اليدوية
- لا يمكن عرض سجلات المزامنة
- لا يمكن تعديل أي تكوين

---

## 4. Permissions Matrix

| # | الصلاحية | Admin (Phase 1) | Viewer (Phase 2) |
|:-:|----------|:----------------:|:-----------------:|
| 1 | View Dashboard | ✅ | ✅ |
| 2 | Drill Down داخل Dashboard | ✅ | ✅ |
| 3 | Configure Cards (CRUD) | ✅ | ❌ |
| 4 | Configure Data Source | ✅ | ❌ |
| 5 | Configure Chart Type | ✅ | ❌ |
| 6 | Configure Drill Down Levels | ✅ | ❌ |
| 7 | Configure Card Layout | ✅ | ❌ |
| 8 | Test Queries (Query Tester) | ✅ | ❌ |
| 9 | Trigger Manual Sync | ✅ | ❌ |
| 10 | View Sync Status | ✅ | ✅ (قراءة فقط) |
| 11 | View Sync Logs | ✅ | ❌ |
| 12 | Access Admin Panel | ✅ | ❌ |
| 13 | Modify Sync Settings | ✅ | ❌ |

---

## 5. Security Rules

### 5.1 Admin Panel Hidden URL

| القاعدة | التفاصيل |
|---------|----------|
| **الرابط** | `/Admin` (أو مسار مخصص يُحدد أثناء التطوير) |
| **الظهور** | غير موجود في قائمة التنقل الرئيسية — لا يمكن الوصول إلا عبر الرابط المباشر |
| **الحماية** | أي محاولة وصول إلى `/Admin` بدون Session صالحة → إعادة توجيه إلى صفحة Login |
| **المصدر** | CLIENT_DECISION_LOG.md #4 — ✅ Approved |

> **تطبيق تقني:** إما via middleware يتحقق من Session قبل الوصول إلى Admin Pages، أو via Authorize attribute مع Policy مخصصة.

### 5.2 Password Hashing — BCrypt

| القاعدة | التفاصيل |
|---------|----------|
| **الخوارزمية** | BCrypt (BCrypt.Net) كما هو موصى به في APPLICATION_BLUEPRINT.md §9 |
| **التخزين** | Hash فقط — لا تُحفظ كلمة المرور نصاً صريحاً |
| **المكان** | تكوين التطبيق (appsettings.json) أو متغير بيئة |
| **التحقق** | `BCrypt.Verify(password, hash)` عند تسجيل الدخول |
| **التعقيد** | 12+ Salt Rounds |

### 5.3 Session-Based Authentication

| القاعدة | التفاصيل |
|---------|----------|
| **النوع** | ASP.NET Core Session (Server-side) |
| **الصلاحية** | مدة الجلسة قابلة للتكوين (افتراضياً 20 دقيقة) |
| **التخزين** | In-Memory (افتراضي) — أو SQL Server لتطبيقات الإنتاج (اختياري) |
| **المفتاح** | Session ID محفوظ في Cookie آمن (HttpOnly + Secure) |
| **الحماية** | Anti-Forgery Token على جميع صفحات Admin Panel |

### 5.4 No API Authentication (Internal)

| القاعدة | التفاصيل |
|---------|----------|
| **API Endpoints** | جميع endpoints تعمل داخلياً (localhost/LAN) — لا تحتاج مصادقة |
| **المبرر** | التطبيق بالكامل داخل شبكة العميل المحلية — لا وصول خارجي |
| **الاستثناء** | Admin Panel محمي بكلمة مرور فقط (لأنه واجهة مستخدم، لا API) |
| **التوصية** | إذا تم فتح API خارجياً مستقبلاً → إضافة API Key أو JWT |

### 5.5 ملخص القواعد الأمنية

| # | القاعدة | الحالة |
|:-:|---------|:------:|
| S1 | Admin Panel عبر رابط مباشر فقط (hidden URL) | ✅ Phase 1 |
| S2 | BCrypt لكلمة مرور Admin | ✅ Phase 1 |
| S3 | Session-based Auth مع HttpOnly Cookie | ✅ Phase 1 |
| S4 | Anti-Forgery Token على Admin Pages | ✅ Phase 1 |
| S5 | لا مصادقة على API (شبكة داخلية) | ✅ Phase 1 |
| S6 | Timeout الجلسة لخروج تلقائي | ✅ Phase 1 |
| S7 | Role-Based Access (RBAC) | ⏳ Phase 2 |
| S8 | اسم مستخدم + كلمة مرور لكل مستخدم | ⏳ Phase 2 |
| S9 | Logout صريح | ⏳ Phase 2 |

---

## 6. Phase 2 Upgrade Path — RBAC

### 6.1 خطة الترقية

عند الانتقال إلى Phase 2، يتم استبدال نموذج الأمان البسيط (كلمة مرور واحدة) بنظام RBAC كامل:

| الخطوة | الوصف |
|:------:|--------|
| 1 | إضافة **ASP.NET Core Identity** أو Claims-based Authorization |
| 2 | إنشاء جدول `Users` (Id, Username, PasswordHash, RoleId, CreatedAt, IsActive) |
| 3 | إنشاء جدول `Roles` (Id, Name, Description) — بالقيم: Admin, Viewer |
| 4 | إنشاء جدول `RolePermissions` (RoleId, Permission) لتخزين الصلاحيات بشكل مرن |
| 5 | ترحيل Admin الحالي إلى مستخدم في جدول Users مع Role = Admin |
| 6 | تحويل `[Authorize]` attributes من Policy بسيطة إلى Role-based Policies |
| 7 | تحديث واجهة Login لدعم اسم مستخدم + كلمة مرور |
| 8 | إضافة صفحة Logout صريحة + إدارة الجلسات |

### 6.2 هيكل الأدوار المقترح (Phase 2)

| الـ Role | الوصف | الصلاحيات |
|:--------:|-------|:----------:|
| **Admin** | مسؤول النظام الكامل | جميع الصلاحيات (P1–P13) |
| **Viewer** | مستخدم عادي للعرض فقط | V1–V2 فقط (عرض Dashboard + حالة المزامنة) |

### 6.3 مخطط الصلاحيات المفصّل (Phase 2)

```
Admin:
  ├── Dashboard: View, DrillDown, SyncStatus, TriggerSync
  ├── AdminPanel: Cards CRUD, DataSource, ChartType, DrillDownConfig, Layout, QueryTester
  ├── Logs: View, Filter, Export (إن وجد)
  └── Settings: SyncConfig Edit

Viewer:
  └── Dashboard: View, DrillDown, SyncStatus (read-only)
```

### 6.4 التوافق مع Phase 1

| البند | Phase 1 | Phase 2 |
|:-----:|:-------:|:-------:|
| **المصادقة** | كلمة مرور واحدة (BCrypt) | Identity Framework (Username + Password) |
| **الأدوار** | Admin فقط (ضمني) | Admin + Viewer (RBAC) |
| **Admin Panel** | محمي بكلمة مرور + Session | محمي بـ [Authorize(Roles="Admin")] |
| **Dashboard** | مفتوح للجميع | مفتوح للجميع أو محمي حسب المتطلب |
| **API** | بدون مصادقة | بدون مصادقة (داخلي) |
| **إدارة المستخدمين** | غير موجودة | صفحة إدارة مستخدمين (Admin only) |

---

## 7. ملاحظات إضافية

| # | الملاحظة |
|:-:|----------|
| 1 | عدد المستخدمين الفعلي غير محدد بعد — يُحدد أثناء التطوير (المصدر: 01_PROJECT_BRIEF.md §3) |
| 2 | API لا يحتاج مصادقة لأنه يعمل داخلياً — إذا تغير المتطلب، يُضاف JWT أو API Key |
| 3 | في Phase 2، يمكن استخدام Claims بدلاً من Roles للحصول على صلاحيات أكثر مرونة |
| 4 | يُوصى بتخزين كلمة مرور Admin في متغير بيئة (Environment Variable) بدلاً من appsettings.json للإنتاج |

---

## References

| الوثيقة | المسار |
|---------|--------|
| 01_PROJECT_BRIEF.md | `project-preparation/01_PROJECT_BRIEF.md` |
| APPLICATION_BLUEPRINT.md (§5) | `project-preparation/APPLICATION_BLUEPRINT.md` |
| CLIENT_DECISION_LOG.md (#4, #8, #9) | `client-engagement/CLIENT_DECISION_LOG.md` |
| 08_TECHNICAL_ARCHITECTURE.md | `project-preparation/08_TECHNICAL_ARCHITECTURE.md` |

---

> **Prepared by:** TeraAgent — TASK-PREP-005
> **Date:** 2026-07-12
> **Status:** `preparation`
