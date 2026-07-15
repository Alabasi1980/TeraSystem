# FEATURE_LIST.md

## 1. Metadata

| Field | Value |
|-------|-------|
| Client | شركة العمران الحديثة للمقاولات |
| Application | منظومة إدارة طلبات المصنع |
| Phase | Phase 1 (MVP) |
| Date | 2026-07-06 |
| Status | Approved — Client confirmed (2026-07-06) |

## 2. Feature List — Phase 1

> **Legend:** Status = `Approved` / `Deferred` / `Conditional` / `Rejected`

| # | Feature | Domain | Status | Source | Priority |
|:-:|:--------|:------:|:------:|:------:|:--------:|
| F01 | دخول فردي (username + password) | 2 (Users, Roles & Access) | ✅ Approved | [Confirmed by Majed] | 1 |
| F02 | 3 أدوار (مدخل طلب، مشرف مصنع، مدير) | 2 (Users, Roles & Access) | ✅ Approved | [Confirmed by Majed] | 1 |
| F03 | عزل صلاحيات (كل دور يرى ما يخصه) | 2 (Users, Roles & Access) | ✅ Approved | [Confirmed by Majed] | 1 |
| F04 | إنشاء طلب جديد برقم فريد | 3 (Process & Workflow) | ✅ Approved | [Confirmed by Majed] | 1 |
| F05 | إرفاق مخططات/قياسات مع الطلب | 4 (Data & Content) | ✅ Approved | [Confirmed by Majed] | 2 |
| F06 | 5 مراحل رئيسية (طلب جديد ← قيد التجهيز ← قيد التصنيع ← فحص الجودة ← تم التسليم) | 3 (Process & Workflow) | ✅ Approved | [Confirmed by Majed] | 1 |
| F07 | حالة "معلق" مع 4 أسباب محددة: ناقص مخطط، ناقص مادة، بانتظار موافقة المشروع، بانتظار مقاس/زيارة موقع | 3 (Process & Workflow) | ✅ Approved | [Confirmed by Majed] | 1 |
| F08 | تحديث حالة الطلب (يدوي) | 3 (Process & Workflow) | ✅ Approved | [Confirmed by Majed] | 1 |
| F09 | تسجيل ساعات عمل (إجمالي الفريق) | 4 (Data & Content) | ✅ Approved | [Confirmed by Majed] | 2 |
| F10 | صرف مواد بسيط (اسم + كمية + تكلفة) | 4 (Data & Content) | ✅ Approved | [Confirmed by Majed] | 2 |
| F11 | مصاريف إضافية (خانة واحدة) | 4 (Data & Content) | ✅ Approved | [Confirmed by Majed] | 2 |
| F12 | تسجيل إجراءات (Audit Trail) | 11 (Security & Audit) | ✅ Approved | [Confirmed by Majed] | 2 |
| F13 | لوحة متابعة (Dashboard) — حالة الطلبات بالألوان مع إظهار الطلبات المتأخرة تلقائياً | 8 (Reports & Dashboards) | ✅ Approved | [Confirmed by Majed] | 1 |
| F14 | تسليم الطلب (زر + تاريخ + اسم مسلّم) | 3 (Process & Workflow) | ✅ Approved | [Confirmed by Majed] | 1 |
| F15 | Web Responsive (Chrome — كمبيوتر + جوال) | 10 (Technical) | ✅ Approved | [Confirmed by Majed] | 1 |
| F16 | استضافة على سيرفر الشركة الداخلي + رابط داخلي | 10 (Technical) | ✅ Approved | [Confirmed by Majed] | 1 |
| F17 | نسخ احتياطي تلقائي | 10 (Technical) | ✅ Approved | [Confirmed by Majed] | 1 |
| F18 | Logo/شعار الشركة في التصميم | 9 (Design & Branding) | ✅ Approved | [Confirmed by Majed] | 3 |
| F19 | إجمالي التكلفة التقريبي (مواد + ساعات + مصاريف) | 4 (Data & Content) | ✅ Approved | بناءً على طلب العميل | 2 |

> **ملاحظة:** تقرير الطلبات المتأخرة (كان سابقاً F20 في Phase 2) أصبح مشمولاً ضمن F13 (لوحة المتابعة) في Phase 1 — إظهار تلقائي بدون تقرير منفصل.

### Phase 2 (Deferred)

| # | Feature | Domain | Source |
|:-:|:--------|:------:|:------|
| F20 | إشعارات داخل النظام | 7 (Notifications) | [Confirmed by Majed — Deferred] |
| F21 | تقرير التكلفة التفصيلية | 8 (Reports) | [Confirmed by Majed — Deferred] |
| F22 | مقارنة تكلفة مخطط vs فعلي | 8 (Reports) | [Confirmed by Majed — Deferred] |
| F23 | تفصيل ساعات العمال (كل عامل) | 4 (Data & Content) | [Confirmed by Majed — Deferred] |

### Phase 3 (Deferred)

| # | Feature | Domain | Source |
|:-:|:--------|:------:|:------|
| F24 | إشعارات واتساب | 7 (Notifications) | [Confirmed by Majed — Deferred] |
| F25 | الربط مع المخازن | 12 (Integrations) | [Assumption — Needs confirmation] |
| F26 | الربط مع المحاسبة | 12 (Integrations) | [Assumption — Needs confirmation] |
| F27 | الربط مع المشتريات | 12 (Integrations) | [Assumption — Needs confirmation] |

### Value-Added Proposals (A.6.10) — خارج النطاق

| # | Feature | Est. Value | Timing |
|:-:|:--------|:----------:|:------|
| V01 | لوحة تحكم متقدمة (تقارير ذكية) | 500-700 JOD | Phase 2 |
| V02 | مقارنة تكلفة مخطط vs فعلي | 300-400 JOD | Phase 2 |
| V03 | إشعارات واتساب | 400-600 JOD | Phase 2 (أو 3) |
| V04 | تفصيل ساعات كل عامل | 400-500 JOD | Phase 3 |

> **ملاحظة:** نموذج المراحل المنظم (5 مراحل) — كان سابقاً V02 في VALUE_ADDED_PROPOSALS.md — تم اعتماده ودخل في النطاق (F06 + F07).

## 3. Final Scope Reconciliation (B.3)

| Total Features | Approved (Phase 1) | Deferred (Phase 2) | Deferred (Phase 3) | Value-Added |
|:--------------:|:------------------:|:------------------:|:------------------:|:-----------:|
| 31 | 19 | 4 | 4 | 4 |

> **ملاحظات التحديث (بناءً على ملاحظات العميل):**
> - F08 (تحديث الحالة) + F13 (لوحة المتابعة): إظهار الطلبات المتأخرة تلقائياً بالألوان بدون تقرير منفصل.
> - F19 (جديد): إجمالي التكلفة التقريبي — (مواد + ساعات + مصاريف).
> - F20 (سابقاً تقرير الطلبات المتأخرة): أُزيل من Phase 2 لأنه أصبح ضمن F13 في Phase 1.
> - نموذج المراحل المنظم (5 مراحل): كان سابقاً V02 — تم اعتماده ودخل في النطاق (F06 + F07).

**Final Scope Reconciliation Gate (B.3):** ✅ **PASS** — جميع الميزات مصنّفة وحالتها واضحة.
