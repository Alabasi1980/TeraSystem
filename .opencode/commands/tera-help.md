---
description: عرض جميع أوامر Tera المتاحة ووصف مختصر لكل أمر
---

أنت Tera Agent. المستخدم يطلب مساعدة في الأوامر المتاحة.

اعرض جميع أوامر Tera المتاحة (من `.opencode/commands/tera-*.md`) بتنسيق واضح:

```
## أوامر Tera المتاحة

| الأمر | الوصف |
|---|---|
| `/tera-new-project` | بدء مشروع جديد — تحويلك إلى TeraClientEngagementAgent أولاً |
| `/tera-resume` | استئناف مشروع قائم من آخر نقطة توقف |
| `/tera-status` | تقرير سريع عن حالة المشروع الحالي |
| `/tera-plan` | تأكيد Plan Mode — قراءة وتحليل فقط |
| `/tera-request-build` | طلب الدخول في Build Mode — مراجعة قبل الموافقة |
| `/tera-review` | مراجعة ما بعد التنفيذ لآخر مهمة |
| `/tera-gate` | تشغيل Pre-Execution Gate للمهمة الحالية |
| `/tera-approve` | اعتماد مهمة أو مرحلة وإغلاقها |
| `/tera-diagnose` | تشخيص ذاتي لـ Tera — فحص الحالة والوضوح |
| `/tera-help` | عرض هذه القائمة |

## أدلة سريعة

- **مشروع جديد؟** → `/tera-new-project` ثم ابدأ عبر `TeraClientEngagementAgent`
- **مشروع قائم؟** → `/tera-resume`
- **عند العودة للجلسة؟** → `/tera-resume`
- **تريد التنفيذ؟** → `/tera-plan` أولاً، ثم `/tera-request-build`
- **بعد التنفيذ؟** → `/tera-review`
- **تائه؟** → `/tera-diagnose`

## القاعدة الذهبية

Plan Mode افتراضيًا. لا تنفيذ بدون Build Mode.
```

ثم اسأل:

```
هل تريد تنفيذ أي من هذه الأوامر الآن؟
```
