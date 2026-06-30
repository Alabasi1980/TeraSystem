---
description: تشغيل بوابة ما قبل التنفيذ على المهمة الحالية
---

أنت Tera Agent.

طبّق `tera-system/TeraPreExecutionGate.md` على المهمة الحالية.

الخطوات:
1. اقرأ `tera-system/TeraPreExecutionGate.md`.
2. حدّد مساحة العمل النشطة للتطبيق ثم اقرأ ملف المهمة الحالية من `[active application workspace]/project-control/tasks/`.
3. نفّذ الـ Checklist الإلزامي (Section 5) بالكامل.
4. نفّذ فحص CLI Side Effects (Section 6.1) على كل أمر مقترح.

اعرض النتيجة بهذا الشكل:

```
Pre-Execution Gate — [TASK-ID]

Checklist:
| # | السؤال | النتيجة | ملاحظة |
|---|--------|---------|--------|
| 1 | ... | PASS/FAIL | ... |
...

CLI Side Effects:
| الأمر | الأثر | الحكم |
|-------|-------|-------|
| ... | ... | مسموح/ممنوع |

Gate Status: PASS / NEEDS_REVISION / BLOCKED

الإجراء المطلوب:
- ...
```

ثم:
- إذا كانت النتيجة **PASS**: أخبرني وانتظر موافقتي للبدء.
- إذا كانت **NEEDS_REVISION**: صحّح المهمة ذاتياً واعرض النسخة المصححة فقط.
- إذا كانت **BLOCKED**: أخبرني بالسبب المحدد فقط وانتظر قراري.
