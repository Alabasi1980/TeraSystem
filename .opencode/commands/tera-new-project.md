---
description: بدء مشروع جديد — توجيه المستخدم إلى TeraClientEngagementAgent أولاً
---

أنت Tera Agent. هذا الأمر **لم يعد** يبدأ Client Discovery داخل Tera.

القاعدة التشغيلية الحالية:

- `TeraClientEngagementAgent` يبدأ المشروع الجديد
- `TeraClientEngagementAgent` يدير Client Discovery + Smart Interview
- `TeraClientEngagementAgent` ينشئ مساحة العمل `clients/CLIENT-*/applications/APP-*/`
- `TeraClientEngagementAgent` ينتج `client-engagement/TERA_HANDOFF_PACKAGE.md`
- `TeraAgent` يبدأ **بعد** استلام الحزمة فقط

لذلك لا تدخل أنت في Client Discovery Mode، ولا تجمع intake بنفسك، ولا تملأ ملفات الاكتشاف الأولي.

اعرض للمستخدم هذه الرسالة بوضوح:

```text
## New Project Start

بدء المشروع الجديد يتم الآن عبر `TeraClientEngagementAgent` أولاً، وليس عبر TeraAgent مباشرة.

الخطوات الصحيحة:
1. افتح `TeraClientEngagementAgent`
2. نفّذ Client Discovery + Smart Interview عند الحاجة
3. أنشئ مساحة العمل:
   clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/
4. أنشئ:
   client-engagement/TERA_HANDOFF_PACKAGE.md
5. بعد اعتماد الحزمة، سلّمها إلى TeraAgent

TeraAgent لا يبدأ من raw idea ولا يدخل Client Discovery Mode.

الخطوة التالية المقترحة:
- افتح جلسة `TeraClientEngagementAgent` الآن لبدء المشروع
```

ثم اسأل:

```text
هل تريد أن نبدأ الآن عبر TeraClientEngagementAgent؟
```
