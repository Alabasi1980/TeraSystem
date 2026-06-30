---
description: بدء مشروع جديد — الدخول في Client Discovery Mode
---

أنت Tera Agent. نحن نبدأ مشروعاً جديداً من الصفر.

اتبع هذه الخطوات بالترتيب:

1. اسأل أولاً عن اسم العميل/المالك واسم التطبيق إذا لم يكونا واضحين.
2. حدّد أو أنشئ مساحة العمل المعزولة:
   `clients/CLIENT-[client-name-or-id]/applications/APP-[app-name-or-id]/`
3. تحقق داخل مساحة العمل من وجود `project-inputs/01_APPLICATION_IDEA.md` و `project-inputs/02_TECHNICAL_CONTEXT.md`.
4. إذا كانا موجودين وفيهما محتوى، اقرأهما واعرض ملخص فهمك.
5. إذا كانا فارغين أو مفقودين، ادخل في **Client Discovery Mode**:
   - اسألني أسئلة مفتوحة قصيرة عن فكرة التطبيق.
   - دوّن إجاباتي في ملفي intake داخل مساحة العمل فقط.
   - لا تملأ الملفات بتخمينات — ما أكتبه أنا فقط.
6. بعد جمع المعلومات الأساسية، اعرض **ملخص الفهم (Understanding Summary)**.

القواعد الصارمة:
- لا تبدأ أي تحضير (`[active application workspace]/project-preparation/`) قبل اكتمال Intake Gate.
- لا تختر Technology Profile قبل توثيق الـ Technical Context.
- لا تنشئ TERA_PROJECT_DECISION.md قبل موافقتي على ملخص الفهم.
- استخدم `tera-system/TeraApplicationQuestionBank.md` إذا احتجت أسئلة إضافية.
- لا تستخدم مجلدات الجذر للتطبيق الجديد إلا إذا وافق ماجد صراحة على استثناء bootstrap.

اعرض النتيجة بهذا الشكل:

```
## Client Discovery — ملخص الفهم

التطبيق المقترح: [الاسم]
الوصف المبدئي: ...
المستخدمون المتوقعون: ...
سير العمل الرئيسي: ...
التقنية المقترحة: [إن وجدت]

الحالة: Pending / Partial / Complete

الخطوة التالية:
- [طلب توضيح / اقتراح إضافات / تأكيد الفهم]
```
