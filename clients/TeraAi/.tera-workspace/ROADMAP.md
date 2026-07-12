# خريطة الطريق — TeraSystem Platform + TeraOpenCode Engine + Workspaces

## الملف: ROADMAP.md
## المسار: .tera-workspace/
## الإصدار: 1.3
## التاريخ: 2026-07-12
## الحالة: ✅ Approved — اعتمد من ماجد
## مرجع: Engine Contract v1.2 + Gateway Protocol Spec v1.2

---

# الرؤية

**TeraSystem** منصة متكاملة لحوكمة وتطوير وإدارة تطبيقات العملاء.
**TeraOpenCode** محرك تنفيذ برمجي مركزي واحد تعمل به المنصة.
**Workspaces** مساحات عمل مستقلة لكل مشروع، منفصلة عن المنصة وعن المحرك.

**ملكية الكود:** العميل/المؤسسة تملك الكود. المحرك جهة مخوّلة بالتعديل ضمن حدود المهمة.

---

# المعمارية المستهدفة

```
TeraSystem Platform (Control Plane)
  ├── Customers / Projects / Tasks / Policies / Gates
  ├── Decisions / Sessions / Reports / Memory
  └── Engine Gateway (Request/Response via stdio IPC)
        │
        │ Engine Contract v1.2 + Capability Envelope
        ▼
TeraOpenCode Engine (Execution Engine)
  ├── Stateless business-wise
  ├── Ephemeral runtime state during task only
  ├── File/Shell/Git/Search/AI tools
  └── ExecutionResult only — no Task State changes
        │
        ▼
Project Workspace
  ├── Source code (owned by client/organization)
  └── .tera-workspace/ (read-only from engine until Gateway replaces it)
```

---

# المراحل

## ✅ Phase 1-2: الفصل عن upstream والهوية (مكتمل)

| المهمة | الحالة |
|---|---|
| إنشاء Fork مستقل | ✅ |
| تغيير الاسم إلى @tera-system/ | ✅ |
| تغيير CLI binary إلى tera | ✅ |
| حذف enterprise, stats, slack, .github, infra, patches | ✅ |
| إنشاء .tera-workspace/ | ✅ |
| إنشاء Technology Profile لـ Effect | ✅ |

---

## ✅ Phase 3: التكامل بين المنصة والمحرك (مكتمل)

| المكون | الوصف | الحالة |
|---|---|---|
| Context Source | سياق TeraSystem داخل المحرك | ✅ |
| read_tera_workspace | Read-Only Adapter مؤقت | ✅ |
| Engine Contract v1.2 | عقد معماري/تكاملي مع Schemas وCapability Envelope | ✅ |

### Definition of Done — Phase 3

| # | البند | الحالة |
|---|---|---|
| 1 | Engine Contract v1.2 معتمد | ✅ |
| 2 | read_tera_workspace يعمل ومستقر | ✅ |
| 3 | .tera-workspace/ للقراءة فقط من المحرك | ✅ |
| 4 | Task Ownership واضح | ✅ |
| 5 | Capability Envelope معرّف ويُفرض وقائيًا | ✅ |
| 6 | Session Model موثّق | ✅ |
| 7 | ملكية الكود واضحة | ✅ |
| 8 | Roadmap سليمة ومحدثة | ✅ |
| 9 | لا يوجد أدوات جديدة تغير حالة النظام | ✅ |

**النتيجة:** Phase 3 مغلقة بالكامل.

---

## ✅ Phase 4: Engine Gateway (مكتمل)

**الشرط المسبق:** اكتمال DoD لـ Phase 3 ✅

### المخرج الأول: TERA_GATEWAY_PROTOCOL_SPEC.md

| البند | التفاصيل |
|---|---|
| الإصدار | v1.2 |
| الحالة | ✅ Approved |
| stdout | رسائل البروتوكول فقط (JSON Lines) |
| stderr | logs والتشخيص والأخطاء النصية فقط |
| Framing | JSON Lines |
| Handshake | contract_version + engine_version |
| correlation_id | ردود البروتوكول تحمل نفس id للطلب |
| Approval correlation | approval.response يحمل نفس id الخاص بـ approval.request؛ لا request_id مستقل في Phase 4 |
| timeout | timeout → task.cancel → grace period → graceful termination → forceful termination |
| cancellation | cooperative cancellation + process tree termination |
| crash/restart | المنصة تقيم الحالة وتعيد التشغيل عند الحاجة |
| حجم الرسائل | sender check + receiver check + MESSAGE_TOO_LARGE أو إغلاق الجلسة |
| File References | canonicalization + no absolute + no ../ + no symlink escape + workspace_id + Capability Envelope |
| Transport | stdio (Phase 4-5) → localhost HTTP (Phase 6+) |

### مراحل التنفيذ

| الخطوة | الوصف | الحالة |
|---|---|---|
| 4.0 | TERA_GATEWAY_PROTOCOL_SPEC.md v1.2 — البروتوكول الرسمي | ✅ مكتمل |
| 4.1 | مراجعة البروتوكول واعتماده | ✅ مكتمل |
| 4.2 | Context API Limited Design | ✅ مكتمل |
| 4.3 | بناء Context API عبر stdio IPC | ✅ مكتمل |
| 4.4 | بناء Task/Result API | ✅ مكتمل |
| 4.5 | بناء Approval API | ✅ مكتمل |
| 4.6 | تحويل read_tera_workspace لـ fallback | ✅ مكتمل |
| 4.7 | اختبارات وتوثيق | ✅ مكتمل |
| 4.8 | مراجعة Phase 4 — إغلاق | ✅ مكتمل |

### معايير الإنجاز (DoD لـ Phase 4)

| # | البند | الحالة |
|---|---|---|
| 1 | TERA_GATEWAY_PROTOCOL_SPEC.md v1.2 مكتوب ومراجع ومعتمد | ✅ |
| 2 | Context API Limited Design مكتمل ومراجع | ✅ |
| 3 | Context API يعمل عبر stdio IPC | ✅ |
| 4 | Task Assignment API يعمل | ✅ |
| 5 | ExecutionResult API يعمل | ✅ |
| 6 | Approval API يعمل | ✅ |
| 7 | read_tera_workspace يتحول لـ fallback | ✅ |
| 8 | لا يوجد Event Stream (مُؤجَّل) | ✅ |
| 9 | اختبارات تكاملية تمر | ✅ (39/39 tests pass) |
| 10 | توثيق Gateway مكتمل | ✅ (GATEWAY_API_REFERENCE.md) |

### خطة انتقال read_tera_workspace

| المرحلة | الحالة |
|---|---|
| Phase 3 | Active — الأداة الوحيدة |
| Phase 4 | Fallback — Gateway أساسي، read_tera_workspace احتياطي |
| Phase 4.5 | Deprecated — تحذير عند الاستخدام |
| Phase 5 | Removed — حذف نهائي |

---

## 🔄 Phase 5: Workspace Management (قيد التنفيذ)

> التفصيل التنفيذي في `PLANS/07-phase5-workspace-management-design.md` (5.1–5.5).

| الأولوية | المكون | الوصف | الحالة |
|---|---|---|---|
| 🥇 1 | Workspace Registry | سجل المشاريع النشطة | ✅ (TASK-COD-006) |
| 🥇 2 | Gateway-Workspace Binding | ربط handlers بـ WorkspaceRecord | ✅ (TASK-COD-007) |
| 🥇 3 | TaskStore/ApprovalStore Isolation | عزل لكل Workspace | ✅ (ضمن TASK-COD-007) |
| 🥈 4 | Multi-Client Isolation | عزل نظام الملفات بين العملاء | 🔜 (TASK-COD-008) |
| 🥈 5 | Workspace Cleanup/Lifecycle | إغلاق وتنظيف شامل | 🔜 (TASK-COD-009) |
| 🥉 6 | Workspace Templates | قوالب مشاريع جاهزة | ⏸ مؤجل |
| 🥉 7 | Artifact Storage | تخزين المخرجات | ⏸ مؤجل |

---

## 🔜 Phase 6: Quality Gates & Governance Automation

| الأولوية | المكون | الوصف |
|---|---|---|
| 🥇 1 | Gate Framework | نظام قابل للتوسع |
| 🥇 2 | Security Gate | منع كلمات السر والمفاتيح |
| 🥈 3 | Quality Gate | اختبارات وتغطية |
| 🥈 4 | Naming Gate | نمط التسمية |
| 🥉 5 | Doc Gate | توثيق APIs |
| 🥉 6 | Decision Proposal Gate | تسجيل القرارات |

---

## 🔮 Phase 7: Multi-Engine Expansion

| المحرك | الوظيفة |
|---|---|
| TeraOpenCode | كتابة وتعديل الكود |
| TeraDesignEngine | تصميم واجهات المستخدم |
| TeraTestEngine | اختبار آلي |
| TeraDeployEngine | نشر وتوزيع |
| TeraDocEngine | توثيق |

---

# ما هو ممنوع فعله الآن

```
❌ بناء tera_list_tasks أو tera_check_gates
❌ البدء في Config Bridge
❌ نسخ المحرك داخل أي مشروع
❌ خلط بيانات العملاء مع مستودع المنصة
❌ إضافة أدوات تغير حالة النظام خارج Gateway scope
❌ كتابة المحرك في .tera-workspace/
❌ استخدام Event Stream قبل الحاجة التشغيلية المثبتة
❌ إضافة Task State persistence (ephemeral only)
❌ إضافة Approval State persistence (ephemeral only)
```

# ما هو مسموح فعله الآن

```
✅ Gateway يعمل كطريقة اتصال أساسية (handshake + context + task + approval + workspace)
✅ اختبارات شاملة لـ Gateway API (51/51 tests pass)
✅ توثيق Gateway API بالعربية (GATEWAY_API_REFERENCE.md)
✅ read_tera_workspace كـ fallback مع تحذير deprecated
✅ Phase 5 قيد التنفيذ: Workspace Registry + Gateway Binding منجزان (5.1/5.2/5.3)
```

---

# سجل التعديلات (Changelog)

| الإصدار | التاريخ | التعديل |
|---|---|---|
| v1.0 | 2026-07-10 | النسخة الأولى |
| v1.1 | 2026-07-10 | إضافة Phase 4.2-4.8 مراحل التنفيذ |
| v1.2 | 2026-07-10 | اعتماد Gateway Protocol Spec v1.2 |
| v1.3 | 2026-07-12 | إغلاق Phase 4 — جميع مراحل التنفيذ (4.0–4.8) مكتملة، 39 اختبارًا تمر، توثيق Gateway API مكتمل |
| v1.4 | 2026-07-12 | تقدم Phase 5 — Workspace Registry (5.1) + Gateway Binding & Isolation (5.2/5.3) منجزان، 51/51 اختبار يمر |

---

*هذه الخريطة **v1.4** — محدثة بعد تقدم Phase 5 (5.1/5.2/5.3).*