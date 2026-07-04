---
description: طلب الدخول في Build Mode — المراجعة قبل الموافقة على التنفيذ
---

أنت Tera Agent. المستخدم يطلب الدخول في Build Mode للتنفيذ.

**لا تبدأ التنفيذ مباشرة.** هذا الأمر يطلب الموافقة فقط.

اتبع الخطوات التالية بالترتيب:

1. تأكد من وجود Approved TASK-ID في `project-control/TASK_REGISTRY.md`.
2. اقرأ ملف المهمة من `project-control/tasks/[TASK-ID].md`.
3. تحقق من اكتمال Pre-Execution Gate لهذه المهمة.
4. تحقق من Allowed Write Targets.
5. إذا كانت المهمة UI/Frontend، تحقق من Design Source Decision.
6. تحقق من Active Technology Profile.

ثم اعرض:

```
## Build Mode — طلب الموافقة

TASK-ID: ...
الوصف: ...

متطلبات ما قبل التنفيذ:
| البند | الحالة |
|---|---|
| Approved TASK-ID | Yes / No |
| Pre-Execution Gate | PASS / NEEDS_FIX / BLOCKED |
| Allowed Write Targets | Yes / No |
| Design Source Decision (إن وجد UI) | Yes / N/A |
| Active Technology Profile | Yes / No |

حالة الطلب: READY_FOR_BUILD / NOT_READY

ملاحظات:
- ...

هل أوافق على بدء Build Mode للمهمة [TASK-ID]؟
```

قواعد:
- إذا كانت NOT_READY، اشرح العوائق فقط ولا تطلب الموافقة.
- إذا كانت READY_FOR_BUILD، انتظر كلمة موافقة صريحة قبل أي تنفيذ.
