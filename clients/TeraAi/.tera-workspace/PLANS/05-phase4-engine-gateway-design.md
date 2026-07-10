# خطة Phase 4 — تصميم Engine Gateway

## الملف: 05-phase4-engine-gateway-design.md
## المسار: .tera-workspace/PLANS/
## الإصدار: 1.0
## التاريخ: 2026-07-10
## الحالة: 🔵 Design Phase

---

# المقدمة

Phase 4 هو بناء **Engine Gateway** — قناة الاتصال الرسمية بين TeraSystem Platform و TeraOpenCode Engine.

**ملاحظة مهمة:** هذه مرحلة **تصميم** فقط. لا تنفيذ واسع حتى تتم مراجعة التصميم والموافقة عليه.

---

# المفهوم

```
الآن (Phase 3):
  TeraSystem ←→ .tera-workspace/ (ملفات) ←→ TeraOpenCode
  (قراءة فقط من المحرك)

بعد Phase 4:
  TeraSystem ←→ Engine Gateway ←→ TeraOpenCode
  (Request/Response عبر Local IPC)
  (read_tera_workspace = fallback مؤقت)
```

---

# ما يحله Engine Gateway

| المشكلة | الحل |
|---|---|
| لا يوجد فصل رسمي بين المنصة والمحرك | Gateway = نقطة اتصال رسمية |
| المحرك يقرأ ملفات مباشرة | Gateway يُرسل السياق عبر IPC |
| لا يوجد Task API واضح | Assignment/Result API عبر Gateway |
| لا يوجد Approval mechanism | Approval API عبر Gateway |
| read_tera_workspace أداة مؤقتة | Gateway يحل محلها تدريجياً |

---

# المكونات (تصميم)

## 1. Context API

```
Platform → ContextRequest → Gateway → ContextResponse → Engine
```

| الحقل | النوع | الوصف |
|---|---|---|
| workspace_id | string | معرّف المشروع |
| context_type | "system" \| "project" \| "task" | نوع السياق |
| context_data | string | محتوى السياق (JSON/Markdown) |
| capabilities | CapabilityEnvelope | صلاحيات هذه الجلسة |

## 2. Task Assignment API

```
Platform → TaskAssignment → Gateway → Engine
Engine → ExecutionResult → Gateway → Platform
```

(مبني على Schemas في Engine Contract Section 6.1 و 6.2)

## 3. Approval API

```
Engine → ApprovalRequest → Gateway → Platform
Platform → ApprovalResponse → Gateway → Engine
```

(مبني على Schema في Engine Contract Section 6.3)

## 4. Error Reporting API

```
Engine → EngineError → Gateway → Platform
```

(مبني على Schema في Engine Contract Section 6.5)

---

# آلية الاتصال

## الخيار A: stdio (الأبسط)

```
TeraSystem Platform
  │
  ├── يشغّل TeraOpenCode كـ child process
  │
  ├── يُرسل Requests عبر stdin (JSON)
  │
  └── يستقبل Responses عبر stdout (JSON)
```

**المزايا:**
- لا يحتاج خادم منفصل
- لا يحتاج منفذ شبكة
- أبسط في التشغيل والصيانة
- آمن (لا فتح منافذ)

**العيوب:**
- اتصال أحادي الجلسة (لا يتصل بعدة جلسات)
- يحتاج إعادة تشغيل للاتصالات المتعددة

## الخيار B: localhost HTTP

```
TeraSystem Platform
  │
  ├── تشغّل Gateway Server على localhost:PORT
  │
  ├── TeraOpenCode يتصل كـ client
  │
  └── Request/Response عبر HTTP
```

**المزايا:**
- يدعم جلسات متعددة
- أسهل في التشخيص
- يمكن إضافة Auth لاحقاً

**العيوب:**
- يحتاج منفذ شبكة
- تعقيد أكثر

**التوصية:** البدء بـ **stdio** (الخيار A) لـ Phase 4، والانتقال لـ localhost HTTP في Phase 6 عند الحاجة.

---

# التسلسل الزمني (Phase 4)

| الخطوة | الوصف | المدة التقريبية |
|---|---|---|
| 4.1 | **تصميم Gateway API** — تحديد الواجهات بالتفصيل | 2-3 أيام |
| 4.2 | **مراجعة التصميم** — مراجعة و approve قبل التنفيذ | 1 يوم |
| 4.3 | **بناء Context API** — أول تكليف عبر IPC | 3-4 أيام |
| 4.4 | **بناء Task/Result API** — التكليف والنتيجة | 3-4 أيام |
| 4.5 | **بناء Approval API** — الموافقات | 2-3 أيام |
| 4.6 | **تحويل read_tera_workspace لـ fallback** | 1 يوم |
| 4.7 | **اختبارات وتوثيق** | 2-3 أيام |
| 4.8 | **مراجعة Phase 4** — إغلاق المرحلة | 1 يوم |

**المجموع التقريبي:** 15-20 يوم عمل

---

# معايير الإنجاز (DoD لـ Phase 4)

| # | البند |
|---|---|
| 1 | Gateway API مصمم ومراجع |
| 2 | Context API يعمل عبر stdio IPC |
| 3 | Task Assignment API يعمل |
| 4 | ExecutionResult API يعمل |
| 5 | Approval API يعمل |
| 6 | read_tera_workspace يتحول لـ fallback |
| 7 | لا يوجد Event Stream (مُؤجَّل) |
| 8 | جميع Schemas تتضمن CommonFields |
| 9 | اختبارات تكاملية تمر |
| 10 | توثيق Gateway مكتمل |

---

# ما لا يُنفَّذ في Phase 4

```
❌ REST API (يُؤجَّل لـ Phase 6)
❌ Event Stream (يُؤجَّل حتى تظهر حاجة)
❌ WebSocket (لا حاجة حالياً)
❌ Auth على Gateway (مبسط في Phase 4)
❌ Multi-engine support (Phase 7)
```

---

*هذه وثيقة تصميم — تُراجع قبل التنفيذ.*
