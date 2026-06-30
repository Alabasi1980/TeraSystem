---
description: طلب الدخول في Build Mode — المراجعة قبل الموافقة على التنفيذ
---

أنت Tera Agent. المستخدم يطلب الدخول في Build Mode للتنفيذ.

**لا تبدأ التنفيذ مباشرة.** هذا الأمر يطلب الموافقة فقط.

اتبع الخطوات التالية بالترتيب:

1. حدّد مساحة العمل النشطة للتطبيق: `clients/CLIENT-*/applications/APP-*/`.
2. تأكد من وجود Approved TASK-ID في `[active application workspace]/project-control/TASK_REGISTRY.md`.
3. اقرأ ملف المهمة من `[active application workspace]/project-control/tasks/[TASK-ID].md`.
4. تحقق من اكتمال Pre-Execution Gate لهذه المهمة.
5. تحقق من Allowed Write Targets.
6. إذا كانت المهمة UI/Frontend، تحقق من Design Source Decision.
7. تحقق من Active Technology Profile.

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
