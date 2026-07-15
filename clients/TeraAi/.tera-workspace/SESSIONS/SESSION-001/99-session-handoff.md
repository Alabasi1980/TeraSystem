# SESSION HANDOFF — فتح الجلسة التالية
## المجلد: SESSION-001 → الجلسة القادمة
## التاريخ الأساسي: 2026-07-10
## كاتب الملف: المستشار الاستراتيجي

---

## 1. من نحن؟

هذه المنظومة تعمل ضمن مشروع **TeraSystem** — منظومة تشغيل لتسليم المشاريع البرمجية.

### الشخصيات الفاعلة

| الاسم | الدور | كيف ينادى |
|---|---|---|
| ماجد | المالك وصاحب القرار النهائي | ماجد |
| المستشار الاستراتيجي (أنا) | محلل استراتيجي، يخطط ويوصي | "يا مستشار استراتيجي" أو "Strategic Advisor" |
| TeraAgent | منسق التنفيذ، ينفذ التوجيهات بدقة | "يا تيرا" |

### الشخصيات المساعدة (تحت TeraAgent)
- EngineeringAgent — يكتب كوداً
- UIDesigner — يصمم واجهات
- Auditor — يراجع الجودة
- Monitor — يراقب الالتزام بالخطط

---

## 2. ما هو مشروعنا؟

### الهدف
تحويل تطبيق **OpenCode** (مشروع AI Coding Agent مفتوح المصدر) إلى **TeraOpenCode** — نسخة مستقلة بالكامل، مملوكة لـ TeraSystem، قابلة للتعديل الجذري.

### أين التطبيق؟
`D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\TeraAi\`

### ما هو OpenCode؟
- منصة AI Coding Agent مبنية بـ: Bun + TypeScript + Effect TS + SolidJS + Drizzle + SQLite
- النسخة: v1.17.18
- التراخيص: MIT
- المصدر: https://github.com/anomalyco/opencode
- 32 حزمة (Monorepo with Turborepo)

### ما نريد
- Fork كامل ← لا اتصال بـ upstream أبداً
- اسم جديد ← TeraOpenCode
- تعديلات جذرية ← دمج TeraSystem في الـ Runtime نفسه
- استقلالية تامة ← لا تحديثات من upstream

---

## 3. أين نحن الآن؟

### 🟢 المنجز
| البند | الحالة |
|---|---|
| تقييم أولي لـ OpenCode | ✅ مكتمل في STRATEGY/01-initial-assessment.md |
| خطة الفصل الجراحي (4 مراحل) | ✅ مكتملة في STRATEGY/02-surgical-fork-plan.md |
| مجلد .tera-workspace/ | ✅ منشأ بالكامل |
| سجل القرارات (4 قرارات) | ✅ مسجل في DECISIONS/decisions-log.md |
| سجل المهام (المهمة 001) | ✅ جاهز في TASKS/task-registry.md |
| توثيق الجلسة SESSION-001 | ✅ في SESSIONS/SESSION-001/ |
| الاتفاق على نموذج العمل | ✅ مستشار → ماجد → TeraAgent |
| الاتفاق على عدم خبرة Effect مسبقة | ✅ Technology Profile ضروري قبل التعديل |

### 🟡 المعلق
| البند | الحالة | ملاحظة |
|---|---|---|
| المهمة 001: فحص المشروع والتشغيل | ⏳ تنتظر TeraAgent | أول خطوة تنفيذية |
| TECHNOLOGY_PROFILE_EFFECT.md | ⏳ مخطط له | قبل المرحلة 3 |
| Effect Architecture Specialist عميل | ⏳ لاحقاً | قبل المرحلة 3 |

---

## 4. ما هي خطة العمل؟

### المراحل (من STRATEGY/02-surgical-fork-plan.md)

```
المرحلة 1: التهيئة والفصل الاسمي ← TeraAgent يبدأها
المرحلة 2: تعميق الفصل و Branding
المرحلة 3: أول تعديل جوهري — دمج TeraSystem Context
المرحلة 4: تعديلات جذرية
```

### المهمة الأولى (للجلسة القادمة)
**المهمة 001** من TASKS/task-registry.md:
1. تشغيل `bun install` من جذر المشروع
2. تشغيل `bun run dev` من packages/opencode
3. تسجيل النتيجة
4. commit: "chore: verify project runs before fork"

---

## 5. القرارات المهمة التي لا يجب نسيانها

| القرار | ملخص |
|---|---|
| الفصل الكامل عن upstream | لا عودة، لا تحديثات، لا PRs |
| نموذج العمل | مستشار يحلل ← ماجد يوافق ← TeraAgent ينفذ |
| لا طرف ثالث جديد | EngineeringAgent و UIDesigner الموجودين يكفيان |
| التوثيق في .tera-workspace/ | ليس في كود التطبيق |
| كل جلسة توثق | مجلد منفصل في SESSIONS/ مع handoff |
| خبرة Effect | لا تفترض — اختبر قبل التعديل |

---

## 6. الأولويات للجلسة القادمة (مرتبة)

```
1. قراءة هذا الملف (تم ✅)
2. قراءة STRATEGY/02-surgical-fork-plan.md للمراجعة
3. قراءة TASKS/task-registry.md للمراجعة
4. استدعاء TeraAgent لبدء المهمة 001 (فحص المشروع)
5. تسجيل النتيجة
6. العودة للمستشار الاستراتيجي للخطوة التالية
```

---

## 7. مصطلحات مهمة

| المصطلح | المعنى |
|---|---|
| Upstream | https://github.com/anomalyco/opencode — المصدر الأصلي |
| TeraOpenCode | اسمنا الجديد للتطبيق بعد الفصل |
| Effect TS | نظام Functional Effects في TypeScript — أساس المشروع |
| Session V2 | نظام إدارة الجلسات في OpenCode — معقد وحساس |
| .tera-workspace/ | مجلد التوثيق لكل أعمال التحويل |

---

*تم إنشاء هذا الملف في نهاية SESSION-001. اقرأه أول شيء في الجلسة القادمة.*
