# خريطة الطريق — TeraSystem Platform + TeraOpenCode Engine + Workspaces

## الملف: ROADMAP.md
## المسار: .tera-workspace/
## الإصدار: 1.2
## التاريخ: 2026-07-10
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

## 🔵 Phase 4: Engine Gateway (مفتوح — تصميم محدود ثم تنفيذ تدريجي)

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
| 4.2 | Context API Limited Design | 🔵 التالي |
| 4.3 | بناء Context API عبر stdio IPC | 🔜 |
| 4.4 | بناء Task/Result API | 🔜 |
| 4.5 | بناء Approval API | 🔜 |
| 4.6 | تحويل read_tera_workspace لـ fallback | 🔜 |
| 4.7 | اختبارات وتوثيق | 🔜 |
| 4.8 | مراجعة Phase 4 — إغلاق | 🔜 |

### معايير الإنجاز (DoD لـ Phase 4)

| # | البند |
|---|---|
| 1 | TERA_GATEWAY_PROTOCOL_SPEC.md v1.2 مكتوب ومراجع ومعتمد |
| 2 | Context API Limited Design مكتمل ومراجع |
| 3 | Context API يعمل عبر stdio IPC |
| 4 | Task Assignment API يعمل |
| 5 | ExecutionResult API يعمل |
| 6 | Approval API يعمل |
| 7 | read_tera_workspace يتحول لـ fallback |
| 8 | لا يوجد Event Stream (مُؤجَّل) |
| 9 | اختبارات تكاملية تمر |
| 10 | توثيق Gateway مكتمل |

### خطة انتقال read_tera_workspace

| المرحلة | الحالة |
|---|---|
| Phase 3 | Active — الأداة الوحيدة |
| Phase 4 | Fallback — Gateway أساسي، read_tera_workspace احتياطي |
| Phase 4.5 | Deprecated — تحذير عند الاستخدام |
| Phase 5 | Removed — حذف نهائي |

---

## 🔜 Phase 5: Workspace Management

| الأولوية | المكون | الوصف |
|---|---|---|
| 🥇 1 | Workspace Registry | سجل المشاريع النشطة |
| 🥇 2 | Multi-Client Isolation | عزل كامل بين العملاء |
| 🥈 3 | Workspace Templates | قوالب مشاريع جاهزة |
| 🥈 4 | Project Lifecycle | إنشاء → تطوير → مراجعة → تسليم |
| 🥉 5 | Artifact Storage | تخزين المخرجات |

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
❌ تنفيذ واسع لـ Phase 4 قبل إكمال Context API Limited Design
```

# ما هو مسموح فعله الآن

```
✅ تصميم Context API ضمن نطاق محدود
✅ تجهيز واجهة Context Request/Response
✅ تحديد payloads والحدود والاختبارات المطلوبة
✅ صيانة read_tera_workspace كـ fallback مؤقت فقط
```

---

*هذه الخريطة **v1.2 Approved** — محدثة بعد اعتماد Gateway Protocol Spec v1.2.*