# سجل المهام — بناء منصة TeraSystem
## ملف: task-registry.md
## المسار: .tera-workspace/TASKS/
## التاريخ: 2026-07-10

---

## المهمة 001: فحص المشروع والتأكد من التشغيل ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ مكتملة |
| المسؤول | TeraAgent |
| الأولوية | عالية |
| الوصف | تشغيل bun install والتأكد أن المشروع يشتغل حالياً قبل أي تعديل |

### النتيجة:
| الخطوة | النتيجة |
|---|---|
| تثبيت Bun | ✅ Bun v1.3.14 مثبت |
| تشغيل bun install | ✅ 4196 حزمة — نجاح |
| تشغيل bun run dev | ✅ الـ TUI بدأ واستمر في العمل |
| حفظ التغييرات | ✅ commit b936f5c — .tera-workspace/ مضاف للـ git |

### ملاحظات:
- Bun لم يكن مثبتاً مسبقاً — تم تثبيته عبر npm install -g bun
- الـ git repo الرئيسي هو TeraSystem (وليس TeraAi/)
- المشكلة: "husky: .git can't be found" وهذا متوقع لأننا في subfolder

---

## المهمة 002: إنشاء مستودع git مستقل ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ مكتملة |
| المسؤول | TeraAgent |
| الأولوية | عالية |

### النتيجة:

| الخطوة | التفاصيل |
|---|---|
| استخراج التاريخ | `git subtree split` — 117 commit من تاريخ OpenCode |
| مستودع GitHub | **https://github.com/Alabasi1980/TeraOpenCode** |
| النوع | Public |
| الوصف | TeraOpenCode — Fork of OpenCode AI Coding Agent. Standalone engine for TeraSystem. |
| الفرع | `master` (من `tera-opencode-extracted`) |
| TAG | `fork-baseline-v1.17.18` — علامة نقطة الفصل |
| Remote | `tera-opencode` → https://github.com/Alabasi1980/TeraOpenCode.git |

---

## المهمة 003: إعادة تسمية Branding (المرحلة 2) — قيد التنفيذ

| الحقل | القيمة |
|---|---|
| الحالة | 🔄 قيد التنفيذ |
| المسؤول | TeraAgent |
| الأولوية | عالية |

### ما تم:
| الدفعة | التفاصيل | الحالة |
|---|---|---|
| 1 | Root package.json, README, حذف install/flake/sst.config | ✅ commit 6114f23 |
| 2 | CLI binary rename (opencode → tera) | ✅ commit 00cb003 |
| 3 | Scope @opencode-ai/ → @tera-system/ في 1286 ملف | ✅ commit a14c57f + fix 6372729 |
| 4 | النصوص الظاهرية | ⏳ مؤجلة (بعد Phase 3) |
| 5 | حذف enterprise/, stats/, slack/, .github/, infra/, patches/ | ✅ commit 6f9c273 |

---
## المهمة 004: Phase 3.1 — إضافة TeraSystemContext Source ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ مكتملة |
| المسؤول | TeraAgent |
| الأولوية | عالية |
| المرجع | `.tera-workspace/PLANS/03-phase3-context-source.md` |
| الملف الجديد | `packages/core/src/system-context/tera-context.ts` |
| التعديل | `packages/core/src/system-context/builtins.ts` (سطران) |
| commit | `0461263` |

### التحقق:
- ✅ `bun install` — 2483 package, no errors
- ✅ `bun run dev` — TUI يشتغل بدون أخطاء
- ✅ commit + push إلى كلا الـ remote

### ملاحظات هامة:
- المشروع لا يزال يعمل (تم اختبار bun install + bun run dev)
- 5184 استبدال في 1253 ملف (Batch 3)
- 42,362 سطر محذوفة (Batch 5)
- Phase 3.1 ✅ — TeraSystemContext source مضاف للمشروع
- بقي: Batch 4 (نصوص ظاهرية — مؤجلة)، ثم Phase 3.3 (Config Bridge)

---

## المهمة 005: Phase 3.2.1 — أداة `read_tera_workspace` ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ مكتملة |
| المسؤول | TeraAgent + EngineeringAgent |
| الأولوية | عالية |
| المرجع | `.tera-workspace/PLANS/04-phase3.2-tera-read-tool.md` |
| الملف الجديد | `packages/core/src/tool/read-tera-workspace.ts` |
| التعديل | `packages/core/src/tool/builtins.ts` (import + deps) |
| الاسم | `read_tera_workspace` |
| commit | `ee2888d` |

### التحقق:
- ✅ `bun install` — 2483 packages, no errors
- ✅ `bun run typecheck` (packages/core) — no errors
- ✅ TUI يبدأ بدون أخطاء
- ✅ Git commit + push إلى كلا الـ remote
- ⚠️ `tera --help` — مشكلة قديمة (من Phase 2) لا تؤثر على Phase 3.2

### ملاحظات:
- استُخدم `RelativePath.make()` بدلاً من `as any` لسلامة الأنواع
- أُضيف `Effect.catchTag` لمعالجة أخطاء الصلاحيات (BlockedError, CorrectedError) بشكل صريح
- التحقق من المسار يتم داخل `execute()` بدلاً من Schema (لعدم توفر `Schema.filter` في هذه النسخة من Effect)

---

## المهمة 006: TASK-COD-001 — Phase 4.2 Context API Prototype ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ Accepted with external follow-up issue |
| المسؤول | TeraAgent + EngineeringAgent |
| الأولوية | عالية |
| المرجع | `project-control/tasks/TASK-COD-001.md` |
| الملفات الجديدة | `packages/opencode/src/gateway/protocol.ts`, `packages/opencode/src/gateway/stdio.ts`, `packages/opencode/src/cli/cmd/gateway.ts`, `packages/opencode/test/gateway/context-api.test.ts` |
| التعديل | `packages/opencode/src/index.ts` |

### التحقق:

- ✅ `bun test test/gateway/context-api.test.ts` — 7/7 passed
- ⚠️ `bun run typecheck` — يفشل بسبب `src/plugin/index.ts` خارج نطاق المهمة، مسجل في `project-control/ISSUES_AND_GAPS.md` كـ GAP-0001

### ملاحظات:

- التنفيذ محدود بـ Handshake + ContextRequest + ContextResponse فقط.
- لا يوجد Task API أو Approval API أو Event Stream أو Config Bridge.
- Gateway يعلن `supported_methods: ["context"]` فقط.

---

## المهمة 007: TASK-COD-FIX-001 — Fix plugin type mismatch ✅

| الحقل | القيمة |
|---|---|
| الحالة | ✅ Accepted / Closed |
| المسؤول | TeraAgent + EngineeringAgent |
| الأولوية | عالية |
| المرجع | `.tera-workspace/TASKS/TASK-COD-FIX-001.md` |
| الملف المعدل | `packages/opencode/src/plugin/index.ts` |
| السبب | إصلاح فشل `bun run typecheck` الناتج عن اختلاف أنواع plugins خارجية مبنية على `@opencode-ai/plugin` مع الأنواع المحلية `@tera-system/plugin` |

### التحقق:

- ✅ `bun run typecheck` من `packages/opencode` — PASS
- ✅ `bun test test/gateway/context-api.test.ts` — 7/7 passed

### ملاحظات:

- الإصلاح ضيق في ملف واحد فقط.
- لا توجد dependencies جديدة.
- تم حل GAP-0001.
