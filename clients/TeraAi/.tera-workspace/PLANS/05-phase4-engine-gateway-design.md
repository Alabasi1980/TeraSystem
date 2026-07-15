# خطة Phase 4 — تصميم Engine Gateway

## الملف: 05-phase4-engine-gateway-design.md
## المسار: .tera-workspace/PLANS/
## الإصدار: 1.2
## التاريخ: 2026-07-10
## الحالة: 🔵 Design Phase
## مرجع: TERA_GATEWAY_PROTOCOL_SPEC.md v1.2

---

# المقدمة

Phase 4 هو بناء **Engine Gateway** — قناة الاتصال الرسمية بين TeraSystem Platform و TeraOpenCode Engine.

**ملاحظة مهمة:** التنفيذ يبدأ تدريجيًا. المرحلة الحالية هي **Context API Limited Design** فقط، لا تنفيذ واسع.

---

# المفهوم

```
قبل Gateway:
  TeraSystem ←→ .tera-workspace/ (ملفات) ←→ TeraOpenCode
  (قراءة فقط من المحرك)

بعد Gateway:
  TeraSystem ←→ Engine Gateway ←→ TeraOpenCode
  (Request/Response عبر stdio IPC)
  (read_tera_workspace = fallback مؤقت)
```

---

# البروتوكول المعتمد

| البند | القيمة |
|---|---|
| Protocol Spec | TERA_GATEWAY_PROTOCOL_SPEC.md |
| Version | v1.2 |
| Status | ✅ Approved |
| Transport | stdio IPC / JSON Lines |
| stdout | protocol messages فقط |
| stderr | logs والتشخيص |
| Termination | cross-platform process tree termination |
| File References | محمية بـ canonicalization + Capability Envelope |

---

# آلية الاتصال المختارة

## الخيار المختار: stdio IPC

```
TeraSystem Platform
  │
  ├── يشغّل TeraOpenCode كـ child process
  │
  ├── يُرسل Requests عبر stdin (JSON Lines)
  │
  └── يستقبل Responses عبر stdout (JSON Lines)
```

**السبب:** أبسط، لا يفتح منافذ، مناسب لأول Gateway.

---

# التسلسل الزمني (Phase 4)

| الخطوة | الوصف | المدة التقريبية | الحالة |
|---|---|---|---|
| 4.0 | TERA_GATEWAY_PROTOCOL_SPEC.md — البروتوكول الرسمي | 2-3 أيام | ✅ مكتمل |
| 4.1 | مراجعة البروتوكول واعتماده | 1 يوم | ✅ مكتمل |
| 4.2 | Context API Limited Design | 1-2 يوم | 🔵 التالي |
| 4.3 | بناء Context API عبر stdio IPC | 3-4 أيام | 🔜 |
| 4.4 | بناء Task/Result API | 3-4 أيام | 🔜 |
| 4.5 | بناء Approval API | 2-3 أيام | 🔜 |
| 4.6 | تحويل read_tera_workspace لـ fallback | 1 يوم | 🔜 |
| 4.7 | اختبارات وتوثيق | 2-3 أيام | 🔜 |
| 4.8 | مراجعة Phase 4 — إغلاق المرحلة | 1 يوم | 🔜 |

---

# معايير الإنجاز (DoD لـ Phase 4)

| # | البند |
|---|---|
| 1 | TERA_GATEWAY_PROTOCOL_SPEC.md v1.2 مكتوب ومراجع ومعتمد ✅ |
| 2 | Context API Limited Design مكتمل ومراجع |
| 3 | Context API يعمل عبر stdio IPC |
| 4 | Task Assignment API يعمل |
| 5 | ExecutionResult API يعمل |
| 6 | Approval API يعمل |
| 7 | read_tera_workspace يتحول لـ fallback |
| 8 | لا يوجد Event Stream (مُؤجَّل) |
| 9 | اختبارات تكاملية تمر |
| 10 | توثيق Gateway مكتمل |

---

# ما لا يُنفَّذ في Phase 4

```
❌ REST API (يُؤجَّل لـ Phase 6)
❌ Event Stream (يُؤجَّل حتى تظهر حاجة)
❌ WebSocket (لا حاجة حالياً)
❌ Multi-engine support (Phase 7)
❌ تنفيذ واسع قبل Context API Limited Design
```

---

*هذه الخطة محدثة بعد اعتماد Protocol Spec v1.2.*