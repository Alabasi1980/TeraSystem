# Phase 3.1 — Minimal TeraSystemContext Source
## ملف: 03-phase3-context-source.md
## المسار: .tera-workspace/PLANS/
## التاريخ: 2026-07-10
## الحالة: ✅ موافق عليها من ماجد

---

## 1. الهدف

إضافة مصدر سياق (Context Source) بسيط و**للقراءة فقط** في `packages/core/src/system-context/`.  
هذا المصدر يحقن معلومات عن TeraSystem في سياق الـ model عبر آلية `SystemContext` الموجودة أصلاً.

لا تعديل في Session V2.  
لا إضافة أدوات جديدة.  
لا تعديل صلاحيات.

---

## 2. المبادئ التوجيهية

1. **استخدم الآلية الموجودة فقط** — لا تنشئ نظام سياق موازياً.
2. **ابقَ للقراءة فقط** — أول Batch لا يكتب ولا يعدّل أي شيء.
3. **لا تلمس Session V2** — هذا Batch يضيف سياقاً فقط.
4. **لا تلمس `packages/opencode/src/config/`** — الـ Config يأتي لاحقاً.
5. **اختبر بعد كل تغيير** — `bun install` + تشغيل التطبيق.

---

## 3. نقطة التعديل الوحيدة

**الملف الوحيد الذي سنضيفه (جديد):**

| الملف | الغرض |
|---|---|
| `packages/core/src/system-context/tera-context.ts` | ملف جديد — مصدر سياق TeraSystem |

**الملف الذي سنعدّله:**

| الملف | التعديل |
|---|---|
| `packages/core/src/system-context/builtins.ts` | نضيف تسجيل المصدر الجديد في الـ registry (سطرين فقط) |

لا تعديل لأي ملف آخر في Phase 3.1.

---

## 4. التصميم

### 4.1 محتوى السياق المقترح

سيكشف السياق TeraSystem للمستخدم في بداية الجلسة:

```text
<tera-system>
  TeraSystem governance is active for this workspace.
  Version: 1
  Workspace: TeraSystem
  Governance files:
    - ./tera-system/ (policies, architecture, checklists)
  Agent roles are defined in .opencode/agents/
</tera-system>
```

### 4.2 هيكل الملف الجديد

```ts
// packages/core/src/system-context/tera-context.ts

import { Effect, Schema } from "effect"
import { SystemContext } from "./index"
import { SystemContextRegistry } from "./registry"

export const tera = SystemContext.make({
  key: SystemContext.Key.make("tera/system"),
  codec: Schema.toCodecJson(Schema.String),
  load: Effect.succeed([
    `<tera-system>`,
    `  TeraSystem governance is active for this workspace.`,
    `  Governance files are located in:`,
    `    - tera-system/`,
    `    - project-control/`,
    `    - .opencode/agents/`,
    `</tera-system>`,
  ].join("\n")),
  baseline: (text) => text,
  update: (_previous, text) => text,
})
```

### 4.3 التعديل في builtins.ts

في نهاية `builtins.ts`، نضيف سطر تسجيل واحد:

```ts
yield* registry.register({
  key: SystemContext.Key.make("tera/system"),
  load: Effect.succeed(tera),
})
```

ملاحظة: هذا يعني أن `Effect` و `Schema` يحتاجان إلى استيراد إضافي (إذا لم يكونا موجودين)، وسيكون الاستيراد كالتالي:

```ts
import { Effect, Schema } from "effect"
import { SystemContext } from "./index"
```

---

## 5. خطوات التنفيذ لـ TeraAgent

### الخطوة 1: إنشاء الملف الجديد

```bash
# إنشاء الملف
New-Item -Path packages/core/src/system-context/tera-context.ts -ItemType File -Force
```

محتويات الملف:

```ts
export * as TeraSystemContext from "./tera-context"

import { Effect, Schema } from "effect"
import { SystemContext } from "./index"

export const tera = SystemContext.make({
  key: SystemContext.Key.make("tera/system"),
  codec: Schema.toCodecJson(Schema.String),
  load: Effect.succeed([
    "<tera-system>",
    "  TeraSystem governance is active for this workspace.",
    "  Governance files:",
    "    - tera-system/ (policies, architecture, checklists)",
    "    - project-control/ (logs, gaps, proposals)",
    "    - .opencode/agents/ (agent definitions)",
    "</tera-system>",
  ].join("\n")),
  baseline: (text) => text,
  update: (_previous, text) => text,
})
```

### الخطوة 2: تعديل builtins.ts

في `packages/core/src/system-context/builtins.ts`:

1. أضف الاستيراد في أعلى الملف:
```ts
import { TeraSystemContext } from "./tera-context"
```

2. أضف سطر التسجيل بعد تسجيل `core/builtins` (قبل سطر إغلاق `})`):
```ts
yield* registry.register({ key: SystemContext.Key.make("tera/system"), load: Effect.succeed(TeraSystemContext.tera) })
```

### الخطوة 3: تشغيل `bun install`

من جذر المشروع `clients/TeraAi`:
```bash
bun install
```

### الخطوة 4: تشغيل التطبيق للتحقق

```bash
bun run --cwd packages/opencode --conditions=browser ./src/index.ts
```

يجب أن يبدأ التطبيق بدون أخطاء في الـ TUI.

### الخطوة 5: commit + push

```bash
git add clients/TeraAi/packages/core/src/system-context/tera-context.ts clients/TeraAi/packages/core/src/system-context/builtins.ts
git commit -m "feat(core): add TeraSystem context source (Phase 3.1)"
git subtree push --prefix=clients/TeraAi tera-opencode master
```

### الخطوة 6: تحديث سجل المهام

تحديث `task-registry.md` بإضافة المهمة 004.

---

## 6. ممنوع في هذا Batch

- ❌ لا تعدّل أي ملف خارج `system-context/`
- ❌ لا تضف أدوات (tools)
- ❌ لا تعدّل Session V2
- ❌ لا تعدّل `packages/opencode/src/config/`
- ❌ لا تمسح أي شيء
- ❌ لا تضيف صلاحيات
- ❌ لا تنشئ أكثر من ملف جديد واحد
- ❌ لا تستخدم `fs.readFile` أو أي عمليات قراءة ملفات — السياق ثابت مبدئياً

---

## 7. التحقق بعد التنفيذ

| الاختبار | معيار النجاح |
|---|---|
| `bun install` | ✅ لا أخطاء |
| `bun run dev` (packages/opencode) | ✅ TUI يبدأ ويعمل |
| نوع التعديل | ✅ ملف واحد جديد + سطرين في builtins.ts |
| Session V2 | ✅ لم يُلمس |
| Git | ✅ commit + push إلى كلا الـ remote |

---

## 8. التراجع إن لزم

```bash
git revert HEAD --no-edit
git push origin master
git subtree push --prefix=clients/TeraAi tera-opencode master
```

---

*هذه الخطة قابلة للتنفيذ من TeraAgent مباشرة.*
