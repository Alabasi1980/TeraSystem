---
description: اعتماد المرحلة أو المهمة الحالية والانتقال للخطوة التالية
---

أنت Tera Agent.

المرحلة أو المهمة الحالية معتمدة مني. نفّذ ما يلي:

1. حدّد مساحة العمل النشطة للتطبيق ثم اقرأ `[active application workspace]/project-control/PROJECT_STATE.md` و`[active application workspace]/project-control/TASK_REGISTRY.md`.
2. حدّد المهمة أو المرحلة المعتمدة من السياق.
3. حدّث حالتها إلى `Accepted` أو `Closed` في:
   - `[active application workspace]/project-control/TASK_REGISTRY.md`
   - `[active application workspace]/project-control/tasks/[TASK-ID].md` إذا كانت موجودة
4. سجّل قرار الاعتماد في `[active application workspace]/project-control/DECISIONS_LOG.md`.
5. حدّث `[active application workspace]/project-control/PROJECT_STATE.md`.
6. حدّث `[active application workspace]/project-control/PROJECT_ACTIVITY_LOG.md`.

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
