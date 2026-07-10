# خريطة الطريق — TeraSystem Platform + TeraOpenCode Engine + Workspaces

## الملف: ROADMAP.md
## المسار: .tera-workspace/
## الإصدار: 1.2
## التاريخ: 2026-07-10
## الحالة: ✅ Approved — اعتمد من ماجد
## مرجع: Engine Contract v1.2

---

# الرؤية

**TeraSystem** منصة متكاملة لحوكمة وتطوير وإدارة تطبيقات العملاء.
**TeraOpenCode** محرك تنفيذ برمجي مركزي واحد تعمل به المنصة.
**Workspaces** مساحات عمل مستقلة لكل مشروع، منفصلة عن المنصة وعن المحرك.

**ملكية الكود:** العميل/المؤسسة تملك الكود. المحرك جهة مخوّلة بالتعديل ضمن حدود المهمة.

---

# المعمارية المستهدفة

```
┌──────────────────────────────────────────────────────┐
│                  TeraSystem Platform                  │
│                    (Control Plane)                    │
│                    تملك الحالة الدائمة                │
│                                                      │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌─────────┐ │
│  │ العملاء  │ │ المهام   │ │ السياسات│ │ الجودة  │ │
│  │ Customers│ │ Tasks    │ │ Policies│ │ Gates   │ │
│  └──────────┘ └──────────┘ └──────────┘ └─────────┘ │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌─────────┐ │
│  │ القرارات │ │ الجلسات │ │ التقارير│ │ الذاكرة │ │
│  │ Decisions│ │ Sessions │ │ Reports │ │ Memory  │ │
│  └──────────┘ └──────────┘ └──────────┘ └─────────┘ │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │           Engine Gateway (API)                  │  │
│  │  Context | Task | Result | Approval | (Events) │  │
│  │          Request/Response via IPC               │  │
│  └────────────────────────────────────────────────┘  │
└──────────────────────┬───────────────────────────────┘
                       │ Engine Contract v1.2
                       │ (Capability Envelope لكل مهمة)
                       ▼
┌──────────────────────────────────────────────────────┐
│                 TeraOpenCode Engine                   │
│              (Execution Engine — Stateless)            │
│              Runtime Session مؤقتة فقط                │
│                                                      │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌─────────┐ │
│  │ AI Models│ │ File Ops │ │ Shell    │ │ Git     │ │
│  └──────────┘ └──────────┘ └──────────┘ └─────────┘ │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌─────────┐ │
│  │ Search   │ │ Read-Only│ │ Tests    │ │ Prompts │ │
│  │ (Glob/   │ │ Adapter  │ │          │ │         │ │
│  │  Grep)   │ │ (read_   │ │          │ │         │ │
│  │          │ │ tera_ws) │ │          │ │         │ │
│  └──────────┘ └──────────┘ └──────────┘ └─────────┘ │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │  يُرسل ExecutionResult فقط                    │  │
│  │  لا يغير Task State — المنصة وحدها تقرّر      │  │
│  └────────────────────────────────────────────────┘  │
└──────────────────────┬───────────────────────────────┘
                       │ يعمل ضد (ضمن Capability Envelope)
                       ▼
┌──────────────────────────────────────────────────────┐
│                Project Workspace                      │
│                ملكية العميل/المؤسسة                   │
│                                                      │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌─────────┐ │
│  │ كود المشروع │ │ .tera-   │ │ التبعيات│ │ الإعدادات│ │
│  │ Source    │ │ workspace │ │ Deps    │ │ Config  │ │
│  │ (ملكية   │ │ (قراءة   │ │         │ │         │ │
│  │ العميل)  │ │ فقط من   │ │         │ │         │ │
│  │          │ │ المحرك)  │ │         │ │         │ │
│  └──────────┘ └──────────┘ └──────────┘ └─────────┘ │
└──────────────────────────────────────────────────────┘
```

---

# المراحل

---

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

### Phase 3.1 — Context Source (مكتمل ✅)

| المكون | الوصف | الحالة |
|---|---|---|
| SystemContext tera/system | سياق يُحقن في الـ model لوعيه بوجود TeraSystem | ✅ |

### Phase 3.2.1 — Read-Only Adapter (مكتمل ✅)

| المكون | الوصف | الحالة |
|---|---|---|
| `read_tera_workspace` | أداة قراءة من `.tera-workspace/` فقط | ✅ — Read-Only Adapter |

### Phase 3.2.2-3 — أدوات المنصة (معلقة ⏸️)

| المكون | السبب |
|---|---|
| `tera_list_tasks` | ⏸️ معلقة — تحتاج Engine Gateway |
| `tera_check_gates` | ⏸️ معلقة — تحتاج Engine Gateway |

### Phase 3.3 — Config Bridge (معلقة ⏸️)

| المكون | السبب |
|---|---|
| Config Bridge (tera-policy.ts) | ⏸️ معلقة — بعد Engine Gateway |

### Definition of Done — Phase 3 ✅

| # | البند | الحالة |
|---|---|---|
| 1 | Engine Contract v1.2 معتمد | ✅ |
| 2 | `read_tera_workspace` يعمل ومستقر | ✅ |
| 3 | `read_tera_workspace` مُصنّف كـ Read-Only Adapter | ✅ |
| 4 | Context Source يعمل ومستقر | ✅ |
| 5 | `.tera-workspace/` للقراءة فقط من المحرك | ✅ |
| 6 | Task Ownership واضح | ✅ |
| 7 | Capability Envelope معرّف + يُفرض وقائيًا | ✅ |
| 8 | Session Model موثّق | ✅ |
| 9 | ملكية الكود واضحة | ✅ |
| 10 | Roadmap محدَّث ومقبول | ✅ |
| 11 | لا يوجد تناقض في الوثائق | ✅ |
| 12 | لا يوجد أدوات جديدة تغير حالة النظام | ✅ |

**النتيجة:** Phase 3 مكتمل بالكامل.

---

## 🔵 Phase 4: تصميم وبناء Engine Gateway (مفتوح)

**الشرط المسبق:** اكتمال DoD لـ Phase 3 ✅

### المخرج الأول: TERA_GATEWAY_PROTOCOL_SPEC.md

بروتوكول الاتصال الرسمي قبل أي تنفيذ:

| البند | التفاصيل |
|---|---|
| stdout | رسائل البروتوكول فقط (JSON Lines) |
| stderr | logs فقط |
| Framing | JSON Lines (سطر واحد = رسالة واحدة) |
| Handshake | التحقق من contract_version و engine_version |
| correlation_id | ربط كل طلب بردّه |
| timeout | حد أقصى لكل طلب/رد |
| cancellation | إلغاء المهمة برسالة وحيدة |
| crash/restart | سلوك عند انهيار المحرك |
| حجم الرسائل | حد أقصى + تمرير الملفات الكبيرة كمراجع |
|Transport | stdio (Phase 4-5) → localhost HTTP (Phase 6+) |

### مراحل التنفيذ

| الخطوة | الوصف | الحالة |
|---|---|---|
| 4.0 | **TERA_GATEWAY_PROTOCOL_SPEC.md — البروتوكول الرسمي (Draft v1.1 — Pending Phase 4.1 Review)
| 4.1 | تصميم Gateway API بالتفصيل | 🔜 |
| 4.2 | مراجعة التصميم + approve | 🔜 |
| 4.3 | بناء Context API عبر stdio IPC | 🔜 |
| 4.4 | بناء Task/Result API | 🔜 |
| 4.5 | بناء Approval API | 🔜 |
| 4.6 | تحويل read_tera_workspace لـ fallback | 🔜 |
| 4.7 | اختبارات وتوثيق | 🔜 |
| 4.8 | مراجعة Phase 4 — إغلاق | 🔜 |

### معايير الإنجاز (DoD لـ Phase 4)

| # | البند |
|---|---|
| 1 | TERA_GATEWAY_PROTOCOL_SPEC.md مكتوب ومراجع |
| 2 | Gateway API مصمم ومراجع |
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
| Phase 3 (الآن) | **Active** — الأداة الوحيدة |
| Phase 4 (بناء Gateway) | **Fallback** — Gateway أساسي، read_tera_workspace احتياطي |
| Phase 4.5 (تثبيت Gateway) | **Deprecated** — تحذير عند الاستخدام |
| Phase 5 (Workspace Mgmt) | **Removed** — حذف نهائي |

---

## 🔜 Phase 5: Workspace Management

| الأولوية | المكون | الوصف |
|---|---|---|
| 🥇 1 | **Workspace Registry** | سجل المشاريع النشطة |
| 🥇 2 | **Multi-Client Isolation** | عزل كامل بين العملاء |
| 🥈 3 | **Workspace Templates** | قوالب مشاريع جاهزة |
| 🥈 4 | **Project Lifecycle** | إنشاء → تطوير → مراجعة → تسليم |
| 🥉 5 | **Artifact Storage** | تخزين المخرجات |

---

## 🔜 Phase 6: Quality Gates & Governance Automation

| الأولوية | المكون | الوصف |
|---|---|---|
| 🥇 1 | **Gate Framework** | نظام قابل للتوسع |
| 🥇 2 | **Security Gate** | منع كلمات السر والمفاتيح |
| 🥈 3 | **Quality Gate** | اختبارات وتغطية |
| 🥈 4 | **Naming Gate** | نمط التسمية |
| 🥉 5 | **Doc Gate** | توثيق APIs |
| 🥉 6 | **Decision Proposal Gate** | تسجيل القرارات |

---

## 🔮 Phase 7: Multi-Engine Expansion

| المحرك | الوظيفة |
|---|---|
| **TeraOpenCode** (موجود) | كتابة وتعديل الكود |
| **TeraDesignEngine** (مستقبل) | تصميم واجهات المستخدم |
| **TeraTestEngine** (مستقبل) | اختبار آلي |
| **TeraDeployEngine** (مستقبل) | نشر وتوزيع |
| **TeraDocEngine** (مستقبل) | توثيق |

---

# الجدول الزمني

| المرحلة | الحالة |
|---|---|
| Phase 1-2 | ✅ مكتمل |
| Phase 3 | ✅ مكتمل |
| Phase 4 (تصميم + تنفيذ) | 🔵 مفتوح |
| Phase 5 | 🔜 معلق |
| Phase 6 | 🔜 معلق |
| Phase 7 | 🔮 مستقبلي |

---

# ما هو ممنوع فعله الآن

```
❌ بناء tera_list_tasks أو tera_check_gates
❌ البدء في Config Bridge
❌ نسخ المحرك داخل أي مشروع
❌ خلط بيانات العملاء مع مستودع المنصة
❌ إضافة أدوات تغير حالة النظام
❌ تعديل Engine Contract دون مراجعة
❌ كتابة المحرك في .tera-workspace/
❌ استخدام Event Stream قبل الحاجة التشغيلية المثبتة
❌ تنفيذ Phase 4 بدون Protocol Spec معتمد
```

# ما هو مسموح فعله الآن

```
✅ صيانة وتحسين read_tera_workspace (قراءة فقط)
✅ تطوير Context Source (Phase 3.1)
✅ كتابة TERA_GATEWAY_PROTOCOL_SPEC.md
✅ تصميم Engine Gateway
✅ تحسين جودة الكود الحالي
✅ إصلاح أخطاء
✅ تحضير وثائق Phase 4
✅ دراسة Capability Envelope designs
```

---

# المبادئ التوجيهية

1. **تثبيت العقد قبل البناء** — Engine Contract أولاً
2. **المحرك Stateless** — Runtime Session تنتهي عند إرسال النتيجة
3. **المنصة تقرر، المحرك ينفذ** — Control Plane vs Execution Engine
4. **المحرك يُرسل النتيجة فقط** — لا يغير Task State
5. **الفصل الكامل** — كود المنصة ≠ كود المحرك ≠ بيانات العملاء
6. **ملكية الكود للعميل** — المحرك مخوّل بالتعديل
7. **`.tera-workspace/` للقراءة فقط** — لا كتابة أبداً
8. **Request/Response أولاً** — Event Stream يتأجل
9. **Capability Envelope يُفرض وقائيًا** — قبل كل عملية
10. **Protocol Spec قبل التنفيذ** — لا IPC بدون بروتوكول موثّق

---

*هذه الخريطة **v1.2 Approved** — اعتمد من ماجد.*
*آخر تحديث: 2026-07-10.*

