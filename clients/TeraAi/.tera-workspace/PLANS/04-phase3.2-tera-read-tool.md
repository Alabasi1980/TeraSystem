# Phase 3.2.1 — TeraSystem Read Tool: ead_tera_workspace

> **⚠️ ملاحظة استراتيجية (بعد Decision 007):**
> هذه الأداة هي أول مكون يُبنى ضمن رؤية **TeraSystem كمنصة**.
> الهدف النهائي: أن تكون ead_tera_workspace وغيرها من الأدوات
> جزءاً من منصة مستقلة، وليس مجرد إضافات داخل TeraOpenCode.
> للتفاصيل: راجع STRATEGY/02-surgical-fork-plan.md (القسم 0).
 04-phase3.2-tera-read-tool.md
## المسار: .tera-workspace/PLANS/
## التاريخ: 2026-07-10
## الحالة: 🟡 مقترحة — تنتظر موافقة ماجد

---

## 1. الهدف

إضافة أداة **للقراءة فقط (read-only)** في `packages/core/src/tool/` تسمح للـ AI agent بقراءة ملفات `.tera-workspace/` داخل المشروع.

هذه أول أداة TeraSystem مدمجة في نظام Tools الأصلي، وتمهّد الطريق للأدوات التالية:
- `tera_list_tasks` (Phase 3.2.2)
- `tera_check_gates` (Phase 3.2.3)

---

## 2. المبادئ التوجيهية

1. **للقراءة فقط** — لا يكتب، لا يعدّل، لا ينفذ شيئاً.
2. **يستخدم الآلية الموجودة فقط** — `Tool.make` + `Tools.Service.register` كما في `glob.ts` و `read.ts`.
3. **Location-scoped** — يشتغل ضمن نطاق المجلد الحالي فقط.
4. **صلاحية صريحة** — يستخدم `PermissionV2.assert` مثل باقي الأدوات.
5. **مسار محدد** — يقرأ فقط من `.tera-workspace/` ولا يسمح بالخروج منه.
6. **لا يلمس Session V2** — ولا config، ولا صلاحيات النظام.

---

## 3. الملفات المتأثرة

| الملف | نوع التعديل | الشرح |
|---|---|---|
| `packages/core/src/tool/read-tera-workspace.ts` | 🆕 جديد | ملف الأداة الجديد |
| `packages/core/src/tool/builtins.ts` | 🔄 تعديل | إضافة الاستيراد + deps |

لا تعديل لأي ملف آخر.

---

## 4. الاسم والمخطط

### 4.1 الاسم

- **اسم الأداة في الكود:** `read_tera_workspace`
- **اسم الملف:** `read-tera-workspace.ts`
- **اسم التصدير:** `ReadTeraWorkspaceTool`

### 4.2 واجهة الإدخال (Input Schema)

```ts
export const Input = Schema.Struct({
  path: Schema.String.pipe(
    Schema.filter(
      (p) => !p.includes(".."),
      { message: () => "Path must not contain '..'" }
    ),
    Schema.filter(
      (p) => p.startsWith(".tera-workspace/"),
      { message: () => "Path must start with .tera-workspace/" }
    )
  ).annotate({ description: "Relative path within .tera-workspace/ (e.g. .tera-workspace/TASKS/task-registry.md)" }),
})
```

### 4.3 واجهة الإخراج (Output Schema)

```ts
export const Output = Schema.Struct({
  path: Schema.String,
  content: Schema.String,
  truncated: Schema.Boolean,
})
```

### 4.4 وصف الأداة

```
"Read files from the .tera-workspace/ governance directory. "
"Use this to read TeraSystem task registry, decisions log, plans, "
"and any governance documents. Only .tera-workspace/ files are accessible."
```

---

## 5. التصميم التفصيلي

### 5.1 هيكل الملف الكامل (read-tera-workspace.ts)

```ts
export * as ReadTeraWorkspaceTool from "./read-tera-workspace"

import { Effect, Layer, Schema } from "effect"
import { ToolFailure } from "@tera-system/llm"
import { makeLocationNode } from "../effect/app-node"
import { FileSystem } from "../filesystem"
import { PermissionV2 } from "../permission"
import { Location } from "../location"
import { ToolRegistry } from "./registry"
import { Tool } from "./tool"
import { Tools } from "./tools"

export const name = "read_tera_workspace"

const MAX_BYTES = 512_000 // 500KB — حد معقول لملفات governance

export const Input = Schema.Struct({
  path: Schema.String.pipe(
    Schema.filter(
      (p) => !p.includes(".."),
      { message: () => "Path must not contain '..'" }
    ),
    Schema.filter(
      (p) => p.startsWith(".tera-workspace/"),
      { message: () => "Path must start with .tera-workspace/" }
    ),
  ).annotate({
    description:
      "Relative path within .tera-workspace/ (e.g. .tera-workspace/TASKS/task-registry.md)",
  }),
})

export const Output = Schema.Struct({
  path: Schema.String,
  content: Schema.String,
  truncated: Schema.Boolean,
})

const layer = Layer.effectDiscard(
  Effect.gen(function* () {
    const tools = yield* Tools.Service
    const fs = yield* FileSystem.Service
    const permission = yield* PermissionV2.Service
    const location = yield* Location.Service

    yield* tools
      .register({
        [name]: Tool.make({
          description:
            "Read files from the .tera-workspace/ governance directory. " +
            "Use this to read TeraSystem task registry, decisions log, plans, " +
            "and any governance documents. Only .tera-workspace/ files are accessible.",
          input: Input,
          output: Output,
          execute: (input, context) =>
            Effect.gen(function* () {
              yield* permission.assert({
                action: name,
                resources: [input.path],
                save: ["*"],
                metadata: {
                  root: location.directory,
                  path: input.path,
                },
                sessionID: context.sessionID,
                agent: context.agent,
                source: {
                  type: "tool" as const,
                  messageID: context.assistantMessageID,
                  callID: context.toolCallID,
                },
              })

              const result = yield* fs.read({ path: input.path as any }).pipe(
                Effect.mapError(
                  () => new ToolFailure({ message: `Unable to read ${input.path}` }),
                ),
              )

              const decoder = new TextDecoder("utf-8", { fatal: false })
              const text = decoder.decode(result.content)
              const truncated = result.content.byteLength > MAX_BYTES
              const display = truncated
                ? text.slice(0, MAX_BYTES) + "\n\n... [truncated at 500KB]"
                : text

              return {
                path: input.path,
                content: display,
                truncated,
              }
            }),
        }),
      })
      .pipe(Effect.orDie)
  }),
)

export const node = makeLocationNode({
  name: "tool/read-tera-workspace",
  layer,
  deps: [ToolRegistry.node, FileSystem.node, PermissionV2.node, Location.node],
})
```

### 5.2 التعديل في builtins.ts

في `packages/core/src/system-context/builtins.ts`:

**أضف الاستيراد:**
```ts
import { ReadTeraWorkspaceTool } from "./read-tera-workspace"
```

**أضف في مصفوفة `deps:`:**
```ts
ReadTeraWorkspaceTool.node,
```

---

## 6. أمان المسار

الأداة تمنع بشكل صارم:
- **Path traversal:** لا يسمح بـ `..` في المسار
- **نطاق محدد:** فقط `tera-workspace/` وما تحته
- **FileSystem.resolve** يمنع الخروج من مجلد الـ Location

هذا يعني أنه حتى لو حاول agent استخدام `../../../etc/passwd`، سيرفضه الفلتر الأول (`..`) قبل أن يصل إلى نظام الملفات.

---

## 7. الصلاحيات

- **الاسم في Permission system:** `read_tera_workspace`
- **المورد:** مسار الملف داخل `.tera-workspace/`
- **السلوك:** `ask` للمرة الأولى (يطلب موافقة المستخدم)، ثم `allow` أو `always` حسب رد المستخدم

---

## 8. خطة التنفيذ لـ TeraAgent

### الخطوة 1: إنشاء الملف الجديد

```bash
New-Item -Path "packages/core/src/tool/read-tera-workspace.ts" -ItemType File -Force
```

ثم كتابة محتويات الملف حسب القسم 5.1 أعلاه.

### الخطوة 2: تعديل builtins.ts

إضافة سطر الاستيراد + إضافة `ReadTeraWorkspaceTool.node` في deps.

### الخطوة 3: تشغيل bun install

```bash
bun install
```

### الخطوة 4: التحقق (موسع — بعد ملاحظات ماجد)

```bash
# 4.1 التحقق من النوع
bun run typecheck --cwd packages/core

# 4.2 تشغيل التطبيق (TUI)
bun run --cwd packages/opencode --conditions=browser ./src/index.ts
# اختبر أن TUI يبدأ بدون أخطاء

# 4.3 التحقق من Web UI
# افتح http://127.0.0.1:4445 وتأكد أنه يحمل

# 4.4 التحقق من CLI
bun run --cwd packages/opencode tera --help
# تأكد أن الأمر يعرض المساعدة بدون أخطاء
```

### الخطوة 5: commit + push

```bash
git add clients/TeraAi/packages/core/src/tool/read-tera-workspace.ts clients/TeraAi/packages/core/src/tool/builtins.ts
git commit -m "feat(core): add read_tera_workspace tool (Phase 3.2.1)"
git subtree push --prefix=clients/TeraAi tera-opencode master
```

### الخطوة 6: تحديث سجل المهام

تحديث `task-registry.md` بإضافة المهمة 005 (Phase 3.2.1).

---

## 9. ممنوع في هذا Batch

- ❌ لا تعدّل أي ملف خارج `tool/`
- ❌ لا تضيف أكثر من أداة واحدة
- ❌ لا تضيف صلاحيات جديدة للنظام
- ❌ لا تعدّل Session V2
- ❌ لا تعدّل `packages/opencode/src/config/`
- ❌ لا تستخدم `fs.readFileSync` أو أي API متزامن
- ❌ لا تكتب أو تعدّل أي شيء خارج `.tera-workspace/`

---

## 10. التحقق

| الاختبار | معيار النجاح |
|---|---|
| `bun install` | ✅ لا أخطاء |
| `bun run typecheck` (packages/core) | ✅ لا أخطاء نوع |
| TUI يبدأ | ✅ شاشة TUI تظهر |
| Web UI | ✅ http://127.0.0.1:4445 يحمل |
| `tera --help` | ✅ الأمر معروف |
| الأداة مسجلة | ✅ `read_tera_workspace` يظهر في قائمة الأدوات |
| الأمان | ✅ `..` ممنوع، الخروج من `.tera-workspace/` ممنوع |
| Git | ✅ commit + push إلى كلا الـ remote |

---

## 11. التراجع إن لزم

```bash
git revert HEAD --no-edit
git push origin master
git subtree push --prefix=clients/TeraAi tera-opencode master
```

---

## 12. ما بعد التنفيذ (Phase 3.2.2 و 3.2.3)

بعد نجاح `read_tera_workspace`، نضيف:
- **Phase 3.2.2:** `tera_list_tasks` — أداة متخصصة لعرض سجل المهام منسقاً
- **Phase 3.2.3:** `tera_check_gates` — أداة للتحقق من بوابات الجودة

كل أداة لاحقة ستكون أبسط لأن النمط سيكون مثبتاً.

---

*هذه الخطة جاهزة للتنفيذ من TeraAgent بعد موافقة ماجد.*

