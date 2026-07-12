# 35_ROADMAP_AND_FUTURE_PHASES.md — WarehouseDashboard

**Status:** `preparation`
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** TeraAgent — Task TASK-PREP-019
**Date:** 2026-07-12
**Source Documents:** 01_PROJECT_BRIEF.md, 02_SCOPE_AND_BOUNDARIES.md, 03_MODULES_AND_FEATURES.md, CLIENT_DECISION_LOG.md

---

## 1. مقدمة (Introduction)

هذه الوثيقة تُ formalize خريطة طريق (Roadmap) مشروع **WarehouseDashboard** عبر مراحل تطوير منظمة. الهدف منها توفير رؤية واضحة لـ:

- نطاق **Phase 1 (Core MVP)** الحالي وتسليمه.
- الميزات المؤجلة في **Phase 2 (Deferred)**.
- التقدير الزمني (Timeline Estimate) لكل مرحلة.
- الاعتبارات المستقبلية (Future Considerations) المحتملة.
- مرجع سجل القرارات (Decision Log) لأي تغييرات على الخريطة.

> **مبدأ أساسي:** الخريطة قابلة للتطوير المدروس — أي إضافة أو إلغاء يجب أن يكون موثّقاً ومعتمداً من Majed (انظر §6).

---

## 2. Phase 1 (Core MVP) — النطاق الحالي

Phase 1 يمثل **أصغر نسخة قابلة للاستخدام (Minimum Viable Product)** ويتكون من 12 ميزة رئيسية مصنفة P1 — Must-have. تشمل ثلاثة مكونات أساسية:

### 2.1 Sync Engine (API) — محرك المزامنة

| الكود | الميزة | الوصف المختصر |
|:----:|--------|---------------|
| API-01 | Oracle Connection & Data Extraction | الاتصال بـ Oracle عبر ODP.NET واستخراج الجداول مع Data Type Mapping |
| API-02 | SQL Server Data Loading (Full Refresh) | حذف الصفوف + إدخال مجمّع عبر ADO.NET SqlBulkCopy في معاملة واحدة |
| API-03 | Synchronization Engine (Auto + Manual) | Background Service + POST /api/sync/trigger + GET /api/sync/status |
| API-04 | Synchronization Logs | تسجيل وقت البدء/الانتهاء، الحالة، عدد السجلات، المدة |

### 2.2 Dashboard (Razor) — لوحة العرض

| الكود | الميزة | الوصف المختصر |
|:----:|--------|---------------|
| RZR-01 | Dashboard Layout & Rendering | ~20 بطاقة ديناميكية، تخطيط شبكي متجاوب، موضوع أزرق (11 لوناً) |
| RZR-02 | Drill Down Functionality | التعمق عبر مستويات قابلة للتكوين + Breadcrumb Navigation |
| RZR-03 | Data Binding & SQL Execution | تنفيذ SQL Queries/Views من التكوين وربط النتائج بالبطاقات |
| RZR-04 | Sync Status & Controls | عرض آخر وقت مزامنة + مؤشر حالة + زر مزامنة يدوية + عارض سجلات |

### 2.3 Admin Panel (Razor) — لوحة الإدارة

| الكود | الميزة | الوصف المختصر |
|:----:|--------|---------------|
| RZR-05 | Admin Panel (CRUD + Config) | إدارة البطاقات (إنشاء/تعديل/حذف)، مصدر البيانات، نوع الرسم، Drill Down، التخطيط، Query Tester — محمي بكلمة مرور |

### 2.4 Infrastructure — البنية التحتية

| الكود | الميزة | الوصف المختصر |
|:----:|--------|---------------|
| INF-01 | Project Structure (.NET 8) | حل .NET 8 بمشروعين: `WarehouseDashboard.API` + `WarehouseDashboard.Web` |
| INF-02 | Database Setup (Config + Logs) | جداول Config (DashboardCards, CardDrillDownLevels, SyncSettings) + SyncLogs + جداول البيانات المنقولة |
| INF-03 | IIS Hosting | نشر محلي على IIS مع Application Pool و URL Routing |

> **الحالة:** هذه هي المرحلة قيد التنفيذ حالياً. لا تُطور أي ميزة من Phase 2 قبل اكتمالها واستقرارها.

---

## 3. Phase 2 (Deferred) — الميزات المؤجلة

الميزات التالية مؤجلة **بشكل متعمد** إلى Phase 2 بعد استقرار Core MVP. لا تُطور أو تُخطط في Phase 1.

| الكود | الميزة | الوصف | سبب التأجيل |
|:----:|--------|-------|-------------|
| D-1 | **شاشات تعديل البيانات** (Data Editing Screens) | إضافة وتعديل وحذف بيانات في SQL Server عبر واجهة مستخدم | مؤجل مبكراً بتأكيد Majed |
| D-2 | **صلاحيات المستخدمين** (User Roles / Auth — RBAC) | نظام login + Role-Based Access لفصل Admin عن Viewer | غير محدد — سقف زمني في Phase 1 (C2) |
| D-3 | **تصدير Excel/PDF** (Export) | تصدير بيانات Dashboard والرسوم البيانية إلى Excel أو PDF | P3 — يمكن إضافته لاحقاً |
| D-4 | **تحليلات متقدمة** (Advanced Analytics) | تحليلات متقدمة وتقارير إحصائية إضافية | P3 — يمكن إضافته لاحقاً |
| D-5 | **Shared Library (.NET Class Lib)** | مكتبة مشتركة للـ Models والـ DTOs | اختياري — عند الحاجة |

> **جاهزية تقنية:** الـ Sync Engine مصمم لدعم **Incremental Sync** من البداية (CLIENT_DECISION_LOG.md #20) — جاهزية هيكلية فقط، غير مفعّل في Phase 1 ويمكن تفعيله في Phase 2 عند الحاجة (خاصة إن وُجدت جداول كبيرة 100K+ صف — Q5).

---

## 4. Timeline Estimate — التقدير الزمني

| المرحلة | النطاق | التقدير الزمني | الحالة |
|:-------:|--------|:--------------:|:-----:|
| **Phase 1 (Core MVP)** | 12 ميزة رئيسية (33 مكوّناً فرعياً) | **~430 — 625 ساعة** | قيد التنفيذ |
| **Phase 2 (Deferred)** | 4–5 ميزات مؤجلة | **TBD** (يُحدد بعد استقرار Phase 1) | غير مجدول |

### ملاحظات التقدير

- تقدير Phase 1 مأخوذ من معايير النجاح التجاري (SC11 في 01_PROJECT_BRIEF.md).
- التقدير يشمل التطوير، الإعداد، والاختبار الأساسي ضمن النطاق المحلي (IIS).
- **Phase 2 TBD** لأنه يعتمد على:
  - تعقيد Phase 1 الفعلي بعد كشف تفاصيل جداول Oracle (Q1, Q2).
  - حجم البيانات الفعلي (Q5) الذي قد يستدعي Incremental Sync أولاً.
  - أولويات العميل بعد الاستلام الأولي.

---

## 5. Future Considerations — اعتبارات مستقبلية

موضوعات محتملة خارج خريطة الطريق الحالية (لا Phase 1 ولا Phase 2 المعتمدة) وتُناقش لاحقاً:

| الاعتبار | الوصف | الحالة الحالية |
|----------|-------|:--------------:|
| **Mobile App** | تطبيق جوال للوصول إلى Dashboard من الهواتف | خارج النطاق حالياً (O-2) — لا يوجد طلب من العميل |
| **Multi-Warehouse** | دعم مستودعات متعددة / مصادر Oracle متعددة في نفس النظام | غير مطروح — يُقيّم عند توسع العمليات |
| **Predictive Analytics** | تحليلات تنبؤية (توقّع المخزون، اتجاهات ذكية) | خارج النطاق — مرشّح ضمن Advanced Analytics مستقبلاً |
| **Incremental Sync Activation** | تفعيل المزامنة التزايدية إن زادت البيانات | جاهز هيكلياً — يُفعّل عند الحاجة |
| **Automated Testing** | إضافة مجموعة اختبارات آلية | خارج النطاق (O-5) — يمكن إضافته لاحقاً |

> **ملاحظة:** هذه الاعتبارات **ليست التزاماً** — تُقيّم وتُعتمد عبر سجل القرارات قبل أي إدراج في الخريطة.

---

## 6. Decision Log Reference — مرجع سجل القرارات

أي تغيير في خريطة الطريق (إضافة/إلغاء/تأجيل ميزة، تعديل الجدول الزمني) يجب أن يمر بالعملية التالية:

1. **طلب التغيير** — يُقدّم من العميل أو الفريق.
2. **توثيق القرار** — في أحد الملفات:
   - `CLIENT_DECISION_LOG.md` — للقرارات المعتمدة من Majed.
   - `CHANGE_REQUESTS.md` — لطلبات التغيير التي تحتاج تقييم قبل الاعتماد.
3. **تقييم الأثر** — على الجدول الزمني، الميزانية، والميزات الأخرى.
4. **اعتماد Majed** — موافقة خطية لأي تغيير في النطاق.
5. **تحديث الوثائق** — تحديث الخريطة والوثائق المتأثرة (PROJECT_BRIEF، SCOPE_AND_BOUNDARIES، إلخ).

### مراجع القرارات ذات الصلة بالخريطة

| القرار | الملف المرجعي | التأثير على الخريطة |
|:-------|:--------------:|---------------------|
| #4 — Admin Panel بكلمة مرور فقط (لا RBAC في Phase 1) | CLIENT_DECISION_LOG.md | ولّد D-2 في Phase 2 |
| #5 — Full Refresh فقط في Phase 1 | CLIENT_DECISION_LOG.md | Incremental مؤجل إلى Phase 2 |
| #6, #7 — نشر محلي IIS / لا سحابة | CLIENT_DECISION_LOG.md | استبعاد Cloud Hosting و Multi-Region |
| #20 — المحرك يدعم Incremental من البداية | CLIENT_DECISION_LOG.md | جاهزية لـ Future Considerations |
| D4 — تأجيل شاشات تعديل البيانات | CLIENT_DECISION_LOG.md | ولّد D-1 في Phase 2 |

> **قاعدة:** الخريطة ليست جامدة — لكن أي توسّع يجب أن يكون **مدروساً، موثّقاً، ومعتمداً**.

---

## 7. Roadmap Summary — ملخص الخريطة

```
Phase 1 (Core MVP) — 430~625 ساعة  ⬛⬛⬛⬛  قيد التنفيذ
  ├─ Sync Engine (API-01 ~ API-04)
  ├─ Dashboard (RZR-01 ~ RZR-04)
  ├─ Admin Panel (RZR-05)
  └─ Infrastructure (INF-01 ~ INF-03)

Phase 2 (Deferred) — TBD  ⬛⬛⬛⬛
  ├─ D-1 Data Editing Screens
  ├─ D-2 User Roles / Auth (RBAC)
  ├─ D-3 Export to Excel/PDF
  ├─ D-4 Advanced Analytics
  └─ D-5 Shared Library (اختياري)

Future Considerations (غير مجدولة)  ⬛⬛⬛
  ├─ Mobile App
  ├─ Multi-Warehouse
  └─ Predictive Analytics
```

| البند | القيمة |
|:------|:------:|
| Phase 1 — الميزات | 12 ميزة (P1) |
| Phase 2 — الميزات المؤجلة | 5 ميزات (D-1 ~ D-5) |
| التقدير الزمني Phase 1 | ~430 — 625 ساعة |
| التقدير الزمني Phase 2 | TBD |
| خارج النطاق | 5 ميزات (O-1 ~ O-5) |

---

## References

| الوثيقة | المسار |
|---------|--------|
| 01_PROJECT_BRIEF.md | `project-preparation/01_PROJECT_BRIEF.md` |
| 02_SCOPE_AND_BOUNDARIES.md | `project-preparation/02_SCOPE_AND_BOUNDARIES.md` |
| 03_MODULES_AND_FEATURES.md | `project-preparation/03_MODULES_AND_FEATURES.md` |
| CLIENT_DECISION_LOG.md | `client-engagement/CLIENT_DECISION_LOG.md` |

---

> **Prepared by:** TeraAgent — TASK-PREP-019
> **Date:** 2026-07-12
> **Status:** `preparation`
