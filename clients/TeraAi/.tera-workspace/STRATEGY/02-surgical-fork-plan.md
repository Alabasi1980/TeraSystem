# خارطة الطريق الاستراتيجية — TeraSystem كمنصة
## ملف: 02-surgical-fork-plan.md (محدَّث)
## التاريخ: 2026-07-10
## الحالة: ✅ موافق عليها من ماجد — في ضوء Decision 007

---

## 0. الإطار الاستراتيجي (يُقرأ أولاً)

### الرؤية المنقّحة

**TeraSystem** هي منصة متكاملة لحوكمة وتطوير وإدارة تطبيقات العملاء.
**TeraOpenCode** هو أحد المحركات التنفيذية ضمنها (كاتب الكود الذكي).

### ماذا يعني هذا؟

- المنتج النهائي: **TeraSystem** — منصة متكاملة
- TeraOpenCode: محرك تنفيذي واحد ضمن المنصة
- الـ Fork: أساس المحرك التنفيذي، وسيبقى قيد التطوير
- الاستبدال مستقبلاً: وارد إذا ظهر محرك أفضل

### خريطة الطريق الكاملة

Phase 1-2: فصل OpenCode عن upstream ← هوية مستقلة (✅ مكتمل)
Phase 3: ربط TeraSystem بالمحرك ← وعي + أدوات (🔜 قيد التنفيذ)
Phase 4+: بناء منصة TeraSystem ← عملاء، مشاريع، جودة (🔮 مستقبل)
بعيد المدى: استبدال أو تطوير المحرك حسب الحاجة (🔮)

### علاقة هذه الخطة بالرؤية

الخطة أدناه (الفصل الجراحي) تبقى صحيحة. هي تصف **الخطوة 0** من خريطة الطريق.
كل ما فيها من مبادئ وقواعد أمان يبقى نافذاً.

---
# خطة الفصل الجراحي — OpenCode → TeraOpenCode
## ملف: 02-surgical-fork-plan.md
## المسار: .tera-workspace/STRATEGY/
## التاريخ: 2026-07-10
## الحالة: موافق عليها من ماجد

---

## 1. الهدف

تحويل OpenCode من مشروع تابع لـ upstream إلى مشروع مستقل بالكامل (TeraOpenCode) مع:
- هوية جديدة كاملة
- قدرة على التعديل الجذري دون قيود
- تكامل عميق مع TeraSystem

---

## 2. المبادئ

1. لا تلمس الـ Core Logic قبل فهمه
2. ابدأ بالـ Surface (الاسم، CLI، Branding) وانزل للعمق
3. كل خطوة تبقي المشروع قابلاً للتشغيل
4. كل خطوة توثّق مع قرارها وسببها
5. إذا لم تفهم قطعة كود — ارجع للمستشار الاستراتيجي قبل لمسها

---

## 3. الجدول الزمني التنفيذي

### المرحلة 1: التهيئة والفصل الاسمي (الأيام 1-3)
_المسؤول: TeraAgent (بتوجيه من المستشار)_

### المرحلة 2: تعميق الفصل و Branding (الأيام 4-7)
_المسؤول: TeraAgent_

### المرحلة 3: أول تعديل جوهري — دمج TeraSystem Context (الأيام 8-14)
_المسؤول: TeraAgent + EngineeringAgent (إذا لزم)_

### المرحلة 4: التعديلات الجذرية (الأسبوع 3+)
_المسؤول: TeraAgent + فريق العملاء_

---

## 4. المرحلة 1 — التهيئة والفصل الاسمي

هذه المرحلة لا تمس أي كود تشغيلي. هي مرحلة إعدادية بحتة.

### الخطوة 1.1: فحص المشروع والتأكد من التشغيل
- الهدف: نتأكد أن المشروع يشتغل حاليًا قبل أي تعديل
- الإجراء: تشغيل bun install ثم bun run dev من packages/opencode
- معيار النجاح: التطبيق يفتح بدون أخطاء

### الخطوة 1.2: إنشاء git repo جديد مع الاحتفاظ بالتاريخ
- الهدف: مستودع مستقل خاص بـ TeraOpenCode مع كل history OpenCode
- الإجراء: 
  1. إنهاء الارتباط بـ remote upstream
  2. إضافة remote جديد خاص بـ TeraOpenCode (حيثما قرر ماجد استضافته)
  3. إضافة tag: fork-baseline-v1.17.18
- معيار النجاح: git log يظهر كل تاريخ OpenCode، git remote يظهر فقط الـ remote الجديد

### الخطوة 1.3: إنشاء مجلد .tera-workspace/ (مكتمل)
- الهدف: مركز التوثيق لكل أعمال التحويل
- الإجراء: تمت

### الخطوة 1.4: تعريف الـ workspace في Git
- الهدف: مجلد .tera-workspace/ يكون ضمن repo TeraOpenCode
- الإجراء:
  1. git add .tera-workspace/
  2. git commit -m "chore: add .tera-workspace for TeraOpenCode transformation docs"

### الخطوة 1.5: إنشاء أول TAG
- الهدف: نقطة مرجعية ثابتة تعرف أين بدأنا
- الإجراء: git tag -a fork-baseline-v1.17.18 -m "Baseline: OpenCode v1.17.18 at fork point"

---

## 5. المرحلة 2 — تعميق الفصل و Branding

### الخطوة 2.1: إعادة تسمية جذرية واستراتيجية
_الترتيب الدقيق للتغيير:_

#### 2.1.1 ملفات الجذر
- README.md → استبداله بـ README جديد خاص بـ TeraOpenCode
- package.json → تغيير name إلى tera-engine
- bunfig.toml → لا تغيير (إعدادات Bun عامة)
- sst.config.ts → تعطيل أو حذف (SST خاص بـ upstream)
- flake.nix → حذف (Nix خاص بـ upstream)
- install → حذف (سكريبت تثبيت upstream)

#### 2.1.2 السكوب العام (@opencode-ai/ → @tera-system/)
تغيير name في كل packages/*/package.json:
- @opencode-ai/core → @tera-system/core
- @opencode-ai/server → @tera-system/server
- @opencode-ai/opencode → @tera-system/engine
- @opencode-ai/plugin → @tera-system/plugin
- @opencode-ai/protocol → @tera-system/protocol
- @opencode-ai/schema → @tera-system/schema
- @opencode-ai/sdk → @tera-system/sdk
- @opencode-ai/sdk-next → @tera-system/sdk-next
- @opencode-ai/codemode → @tera-system/codemode
- @opencode-ai/llm → @tera-system/llm
- @opencode-ai/tui → @tera-system/tui
- إلخ...

**مع تحديث التبعيات (dependencies) في كل package.json للإشارة للأسماء الجديدة**

#### 2.1.3 الـ Binary
- packages/opencode/bin/opencode → packages/opencode/bin/tera
- تغيير محتوى الملف ليستخدم الاسم الجديد
- packages/opencode/package.json → bin: { "tera": "./bin/tera" }

#### 2.1.4 الرسائل النصية
- البحث عن "OpenCode" في كل النصوص الظاهرية للمستخدم (وليس الكود)
- تغييرها إلى "TeraOpenCode"
- البحث عن "opencode" في أسماء الملفات والمسارات الظاهرية

### الخطوة 2.2: حذف ما لا نحتاج
_بحذر — كل حزمة تحذفها يجب فحص تبعياتها أولاً:_

ذات أولوية عالية للحذف:
- packages/enterprise/ ← تحقق أولاً إن كان أحد يحتاجها
- packages/stats/ ← تحقق من التبعيات
- packages/slack/ ← تحقق من التبعيات
- infra/ ← آمنة الحذف
- .github/ ← آمنة الحذف (لكن قد نحتاج CI/CD خاص بنا)

احذر من حذف:
- packages/desktop/ ← قد تحتاجه
- packages/console/ ← قد تحتاجه كلوحة تحكم
- packages/app/ ← واجهة الويب

### الخطوة 2.3: تنظيف البنية التحتية
- إزالة SST configs إذا لم نخطط لاستخدام Cloudflare/AWS
- تعديل أو إزالة sst-env.d.ts
- إزالة GitHub Actions workflows

---

## 6. المرحلة 3 — أول تعديل جوهري

### الخطوة 3.1: إضافة Context Source لـ TeraSystem
_في packages/core/src/system-context/_

إضافة:
- TeraPolicyContext.ts → يقرأ ويوفر سياسات TeraSystem الحالية
- TeraClientContext.ts → يوفر بيانات العميل النشط
- TeraProjectContext.ts → يوفر حالة المشروع الحالي

### الخطوة 3.2: إضافة TeraSystem Tools
_في packages/core/src/tool/_

إضافة:
- tera-task-registry.ts → أداة لإدارة سجل المهام
- tera-decision-log.ts → أداة لتسجيل القرارات
- tera-gate-checker.ts → أداة للتحقق من بوابات الجودة

### الخطوة 3.3: تعديل Config ليشمل سياسات TeraSystem
_في packages/opencode/src/config/_

إضافة config module جديد:
- tera-policy.ts → يربط TeraSystem Policies مع OpenCode Config

---

## 7. المرحلة 4 — التعديلات الجذرية (لاحقاً)

هذه المرحلة تبدأ فقط بعد استقرار المراحل 1-3 وفهم كامل للنظام:

- تغيير Session V2 ليكون TeraSystem-aware
- تعديل الـ Agent system (build, plan → TeraBuilder, TeraPlanner)
- إضافة TeraGovernor Agent
- إزالة الـ Model Providers التي لا نحتاجها
- تعديل واجهات المستخدم
- إضافة Dashboard خاص بـ TeraSystem

---

## 8. قواعد الأمان أثناء التنفيذ

| القاعدة | الشرح |
|---|---|
| Commit بعد كل خطوة ناجحة | كل خطوة تنتهي بـ git commit مع رسالة واضحة |
| لا تغيير + لا تشغيل في نفس الخطوة | commit بعد كل تعديل، اختبر، ثم تابع |
| التراجع أسهل من الإصلاح | إذا كسرت شيئاً، استخدم git revert ولا تحاول إصلاحه يدوياً |
| وثّق كل قرار في DECISIONS/ | قبل أي تغيير غير بديهي، اكتب القرار وسببه |

---

## 9. أول خطوة لـ TeraAgent

الخطوة الأولى التي سينفذها TeraAgent هي:

**فحص المشروع والتأكد من أنه يشتغل حالياً:**
1. تشغيل `bun install` في جذر المشروع
2. محاولة تشغيل `bun run dev` من packages/opencode
3. تسجيل النتيجة في TASKS/task-registry.md

---

*هذه الوثيقة هي الخطة التنفيذية. تحدث مع تقدم العمل.*

