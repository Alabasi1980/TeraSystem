# التقييم الاستراتيجي الأولي — OpenCode → TeraOpenCode
## ملف: 01-initial-assessment.md
## المسار: .tera-workspace/STRATEGY/
## التاريخ: 2026-07-10
## المستوى: سري — خاص بـ TeraSystem

---

## 1. ملخص تنفيذي

OpenCode هو منصة AI Coding Agent مفتوحة المصدر (MIT) مبنية باستخدام:
- **Bun** كبيئة تشغيل
- **TypeScript** كلغة تطوير
- **Effect TS** كنظام برمجي وظيفي (Functional Effect System)
- **SolidJS** لواجهات المستخدم
- **Drizzle ORM + SQLite** لإدارة البيانات
- **SST (Serverless Stack)** للبنية التحتية السحابية (Cloudflare + AWS)

الهدف: فصل OpenCode عن upstream بالكامل وتحويله إلى TeraOpenCode مع:
1. هوية مستقلة (اسم، علامة، CLI)
2. تعديلات جوهرية تدريجية
3. دمج عمق TeraSystem في الـ Runtime نفسه
4. قدرة على الصيانة الذاتية

---

## 2. هيكل المشروع الحالي

### المستودع
- المصدر: https://github.com/anomalyco/opencode
- النسخة: 1.17.18 (workspace)
- الترخيص: MIT

### الـ Monorepo — 32 حزمة

### الطبقات (من الأسفل للأعلى):

| المستوى | الحزمة | الوظيفة | أهمية التعديل |
|---|---|---|---|
| L0 | schema/ | أنواع أساسية، Schema Definition | لا تلمس — أساس كل شيء |
| L0 | protocol/ | تعريف API Contract | لا تلمس — استقرار الواجهة |
| L1 | core/ | المحرك: جلسات، أدوات، صلاحيات، DB | تعديل استراتيجي — هنا تضاف قوانين TeraSystem |
| L2 | server/ | خادم HTTP, Routing | تعديل خفيف |
| L3 | plugin/ | API التوسيع الخارجي | لا تلمس — استغلها كما هي |
| L3 | sdk/, sdk-next/ | SDK للعملاء الخارجيين | تعديل أو حذف — حسب حاجتك |
| L4 | opencode/ | CLI, TUI, Config, Models | تعديل جوهري — هنا Branding والـ Agents |
| UI | app/, desktop/, console/, tui/, web/ | واجهات المستخدم | تعديل حسب الحاجة |

### البنى التحتية (لن تحتاجها):
- infra/ → SST (Cloudflare + AWS)
- .github/ → CI/CD upstream
- patches/ → تصحيحات upstream
- install → سكريبت تثبيت upstream

---

## 3. نقاط التعديل الرئيسية

### 3.1 الـ CLI والـ Branding (تغيير اسمي)
- packages/opencode/bin/opencode ← tera
- packages/opencode/package.json → name, bin
- جميع الإشارات النصية إلى OpenCode

### 3.2 الـ Agents و Config
- packages/opencode/src/config/ → إضافة قوانين TeraSystem
- packages/opencode/src/agent/ → تغيير سلوك الـ Agents
- packages/opencode/src/system-context/ → إضافة سياق TeraSystem

### 3.3 الـ Core (التعديل الجوهري)
- packages/core/src/session/ → ربط الجلسات بمشاريع TeraSystem
- packages/core/src/tool/ → إضافة أدوات TeraSystem
- packages/core/src/system-context/ → مصادر سياق TeraSystem

### 3.4 حذف أو تعطيل
- packages/enterprise/ (خاص بـ upstream)
- packages/stats/ (إحصائيات upstream)
- packages/slack/ (تكامل Slack)
- infra/ (SST — سنحتاج بديلاً)

---

## 4. التحديات الرئيسية

| التحدي | الشرح | الحل |
|---|---|---|
| Effect TS | OpenCode مبني بالكامل على Effect. أي تعديل في core يحتاج فهم Effect. | دراسة Effect أولاً، تعديل بعدها |
| Session V2 | نظام الجلسات معقد وموزع. تعديله خطير. | لا تلمس Session V2 إلا بعد فهم كامل |
| اعتماديات Windows | node-pty, @parcel/watcher | لا تلمس — ابقَها كما هي |
| Bun | OpenCode مصمم لـ Bun حصراً تقريباً | حافظ على Bun كبيئة أساسية |
| التوثيق الداخلي | الكود موثق جيداً بـ AGENTS.md داخل كل حزمة | استغلها كمرجع للفهم |

---

## 5. المبادئ التوجيهية للتعديل

1. تعديل تصاعدي (Bottom-Up): ابدأ من core → server → opencode
2. لا تلمس ما لا تفهم: أي تعديل في Session V2 أو Effect Layers دون فهم سيسبب انهياراً
3. ابقه مشغلاً: قبل وبعد كل تعديل، شغّل bun run dev لتتأكد أن المشروع لم ينكسر
4. التوثيق أولاً: كل تعديل يسجل في .tera-workspace/DECISIONS/
5. لا تسأل upstream: هذا Fork كامل — لا Issues ولا PRs

---

## 6. الرؤية النهائية — TeraOpenCode

### ما سيكون عليه TeraOpenCode بعد التحول:

```
TeraOpenCode (محرك AI Coding Agent مستقل)
├── TeraSystem معمق في الـ Runtime نفسه
│   ├── Context Sources: Client, Project, Policy
│   ├── Tools: Task Registry, Decision Log, Gate Checker
│   └── Agents: TeraBuilder, TeraPlanner, TeraGovernor
├── Integration → مشاريع TeraSystem
└── Standalone → لا اعتماد على أي مشروع خارجي
---
*هذا التقييم سيُحدث مع تقدم العمل. يعامل كوثيقة حية.*
