# SESSION HANDOFF — SESSION-002 → الجلسة القادمة
## المجلد: SESSION-002
## التاريخ: 2026-07-10
## كاتب الملف: المستشار الاستراتيجي

---

## 1. من نحن؟

منظومة **TeraSystem** — مشروع تحويل OpenCode إلى TeraOpenCode.

| الاسم | الدور | كيف ينادى |
|---|---|---|
| ماجد | المالك وصاحب القرار | ماجد |
| المستشار الاستراتيجي | يحلل ويخطط ويوصي | "يا مستشار استراتيجي" |
| TeraAgent | ينفذ التوجيهات بدقة | "يا تيرا" |

---

## 2. ما هو مشروعنا؟

تحويل **OpenCode** (AI Coding Agent) إلى **TeraOpenCode** — Fork مستقل بالكامل.

### المسار
`D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\TeraAi\`

### المستودع المستقل
`https://github.com/Alabasi1980/TeraOpenCode` — public, master branch

### Remote
- `origin` → `https://github.com/Alabasi1980/TeraSystem` (الريبو الرئيسي)
- `tera-opencode` → `https://github.com/Alabasi1980/TeraOpenCode.git` (subtree)

### TAG
`fork-baseline-v1.17.18`

---

## 3. أين نحن الآن؟

### ✅ المنجز في هذه الجلسة

| المهمة | التفاصيل | commit |
|---|---|---|
| Phase 2 Batch 1 | Root package.json (name → tera-engine), README جديد، حذف install/flake/sst.config | `6114f23` |
| Phase 2 Batch 2 | CLI binary rename (opencode → tera) | `00cb003` |
| Phase 2 Batch 3 | Scope @opencode-ai/ → @tera-system/ في 1286 ملف (5184 استبدال) | `a14c57f` + fix `6372729` |
| Phase 2 Batch 5 | حذف enterprise/, stats/, slack/, .github/, infra/, patches/ (42,362 سطر) | `6f9c273` |
| Technology Profile | TECHNOLOGY_PROFILE_EFFECT.md — مرجع لأنماط Effect TS | `a6a6f0d` |
| Phase 3.1 | TeraSystemContext source — سياق حوكمة TeraSystem للتطبيق | `0461263` |

### ✅ تم حلها
- UTF-8 BOM في package.json (EngineeringAgent)
- مشكلة 401 بعد Refresh في Web UI (تعطيل Basic Auth محلياً)

### ⏳ المعلق

| البند | الأولوية | ملاحظة |
|---|---|---|
| Phase 3.2 — أداة قراءة TeraSystem | 🥇 التالي | بناء على خطة `PLANS/03-phase3-context-source.md` |
| Phase 3.3 — Config Bridge | 🥈 | بعد 3.2 |
| Phase 2 Batch 4 — النصوص الظاهرية | 🥉 مؤجل | 1084 ملف، خطر على الكود — بعد فهم أعمق |

---

## 4. التطبيق يعمل حالياً

| الخدمة | الرابط | الحالة |
|---|---|---|
| Backend API | `http://127.0.0.1:4097` | ✅ |
| Web UI | `http://127.0.0.1:4445` | ✅ |
| Refresh (401 fix) | — | ✅ معطل Basic Auth محلياً |

---

## 5. خطة العمل للجلسة القادمة

```
1. قراءة هذا الملف ← فهم الموقف
2. قراءة PLANS/03-phase3-context-source.md ← مراجعة خطة Phase 3
3. استدعاء المستشار الاستراتيجي ← تحديد أولوية Phase 3.2
4. TeraAgent ينفذ Phase 3.2 (أداة قراءة TeraSystem)
5. العودة للمستشار ← تقييم وتحديد التالي
```

---

## 6. القرارات المهمة

| القرار | ملخص |
|---|---|
| الفصل الكامل عن upstream | لا عودة، لا تحديثات، لا PRs |
| نموذج العمل | مستشار → ماجد → TeraAgent |
| التوثيق في .tera-workspace/ | ليس في كود التطبيق |
| كل جلسة توثق | مجلد منفصل في SESSIONS/ مع handoff |
| Batch 4 مؤجل | النصوص الظاهرية بعد Phase 3 |
| Effect TS | Technology Profile موجود قبل أي تعديل في core |
| الـ Service keys تحتفظ بـ @opencode/... حالياً | عدم تغييرها في Phase 3 |

---

## 7. ملفات مهمة للجلسة القادمة

| الملف | الغرض |
|---|---|
| `.tera-workspace/TASKS/task-registry.md` | سجل المهام الكامل |
| `.tera-workspace/PLANS/03-phase3-context-source.md` | خطة Phase 3 التفصيلية |
| `.tera-workspace/RESEARCH/TECHNOLOGY_PROFILE_EFFECT.md` | مرجع أنماط Effect TS |
| `.tera-workspace/STRATEGY/02-surgical-fork-plan.md` | الخطة الإستراتيجية الكاملة |
| `.tera-workspace/DECISIONS/decisions-log.md` | سجل القرارات |

---

## 8. مصطلحات

| المصطلح | المعنى |
|---|---|
| Upstream | https://github.com/anomalyco/opencode |
| TeraOpenCode | اسمنا الجديد |
| Effect TS | نظام Functional Effects — أساس المشروع |
| Session V2 | نظام الجلسات — لا تلمسه حالياً |
| SystemContext | آلية إضافة سياق readable للـ model |
| LocationNode | نمط خدمة Effect محدد بالموقع (داخل المجلد) |
| GlobalNode | نمط خدمة Effect عام (process-wide) |

---

*تم إنشاء هذا الملف في نهاية SESSION-002. اقرأه أول شيء في الجلسة القادمة.*
