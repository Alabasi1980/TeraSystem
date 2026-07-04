---
description: اعتماد المرحلة أو المهمة الحالية والانتقال للخطوة التالية
---

أنت Tera Agent.

المرحلة أو المهمة الحالية معتمدة مني. نفّذ ما يلي:

1. اقرأ `project-control/PROJECT_STATE.md` و`project-control/TASK_REGISTRY.md`.
2. حدّد المهمة أو المرحلة المعتمدة من السياق.
3. حدّث حالتها إلى `Accepted` أو `Closed` في:
   - `project-control/TASK_REGISTRY.md`
   - `project-control/tasks/[TASK-ID].md` إذا كانت موجودة
4. سجّل قرار الاعتماد في `project-control/DECISIONS_LOG.md`.
5. حدّث `project-control/PROJECT_STATE.md`.
6. حدّث `project-control/PROJECT_ACTIVITY_LOG.md`.

ثم أخبرني بـ:

```
تم اعتماد: [اسم المهمة/المرحلة] — [TASK-ID]

المهمة التالية المقترحة:
- العنوان: ...
- النطاق المقترح: ...
- الملفات المتوقعة: ...

هل أعدّ مسودة TASK-ID جديدة لها؟ أنتظر موافقتك.
```

قواعد:
- لا تبدأ المهمة التالية قبل موافقة صريحة.
- إذا كانت هناك مشاكل مفتوحة تمنع الاعتماد، أخبرني بها أولاً.
- لا تنتقل إلى مرحلة جديدة بدون موافقة صريحة على تغيير المرحلة.
