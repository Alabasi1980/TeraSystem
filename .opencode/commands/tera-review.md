---
description: مراجعة ما بعد التنفيذ لآخر مهمة — Post-Execution Review
---

أنت Tera Agent. المستخدم يطلب مراجعة ما بعد التنفيذ لأحدث مهمة.

اتبع الخطوات التالية بالترتيب:

1. حدّد مساحة العمل النشطة للتطبيق ثم اقرأ `[active application workspace]/project-control/TASK_REGISTRY.md`.
2. ابحث عن آخر مهمة حالة `Submitted` أو `In Review` أو `Needs Fix`.
3. اقرأ ملف المهمة الكامل من `[active application workspace]/project-control/tasks/[TASK-ID].md`.
4. طبّق Post-Execution Review Gate من `tera-system/TeraPreExecutionGate.md`:
   - تحقق من اكتمال acceptance criteria.
   - راجع الملفات التي تم تغييرها (من تقرير التنفيذ).
   - تحقق من عدم وجود secrets في المخرجات.
   - تحقق من تحديث `[active application workspace]/project-control/PROJECT_STATE.md`.
   - تحقق من تحديث `[active application workspace]/project-control/PROJECT_ACTIVITY_LOG.md`.
   - تحقق من تحديث `[active application workspace]/project-control/ISSUES_AND_GAPS.md` إذا وجدت issues جديدة.
5. إذا كانت المهمة UI/Frontend، طبّق `UI_ACCEPTANCE_GATE.md`.

اعرض النتيجة:

```
## Post-Execution Review — [TASK-ID]

الوصف: ...

| Check | Result | ملاحظة |
|---|---|---|
| Acceptance criteria met | PASS / FAIL / PARTIAL | ... |
| Changed files reviewed | PASS / FAIL | ... |
| No secrets exposed | PASS / FAIL | ... |
| PROJECT_STATE.md updated | Yes / No | ... |
| PROJECT_ACTIVITY_LOG.md updated | Yes / No | ... |
| ISSUES_AND_GAPS.md updated if needed | Yes / No / N/A | ... |
| UI Acceptance Gate (إن وجد UI) | PASS / FAIL / N/A | ... |

نتيجة المراجعة: ACCEPT / NEEDS_FIX / BLOCKED

إذا ACCEPT:
- انتظر موافقتي لإغلاق المهمة.

إذا NEEDS_FIX:
- اشرح الإصلاحات المطلوبة بالضبط.

إذا BLOCKED:
- اشرح العائق وانتظر قراري.
```

قواعد:
- لا تقبل أو تغلق أي مهمة بناءً على تقرير الـ sub-agent فقط. راجع الملفات الفعلية.
- لا تفتح مهمة جديدة حتى تُحلّ الحالية (Accepted أو Blocked أو Deferred).
