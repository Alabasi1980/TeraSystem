# BLUEPRINT_OPEN_QUESTIONS.md — TeraWare

| # | Open Item | Current Status | Why It Matters | Preparation Impact | Classification |
|---:|---|---|---|---|---|
| 1 | ما هي الحقول المسموح تعديلها بالضبط في جدول المواد داخل Oracle؟ | Unconfirmed | يحدد حدود UPDATE وآثارها على ERP | ضروري للتحضير التقني وقواعد التحقق | Blocking for formal technical preparation |
| 2 | هل توجد Triggers / Constraints / Audit rules على جدول المواد أو جداول المرفقات؟ | Unconfirmed | قد تكسر التعديل المباشر أو تغيّر السلوك | ضروري للمعمارية وقواعد الأمان والاختبار | Blocking for formal technical preparation |
| 3 | ما هي خريطة الأدوار الفعلية بين Desktop وWeb؟ | Partial | الصلاحيات حالياً “تُقرأ من Oracle” دون تفصيل تشغيلي | ضروري لملف الصلاحيات والشاشات | Blocking for security/screen preparation |
| 4 | ما هي الحالات الرسمية لطلب المواد داخل TeraWare قبل ربطه بـ Oracle؟ | Partial | لدينا جديد/مربوط/ملغي/مرفوض فقط بشكل مبدئي | ضروري لتصميم workflow وSQL Server model | Blocking for workflow/data preparation |
| 5 | ما هو الحد الأدنى المطلوب من سجل التتبع (Audit) لطلبات المواد وربطها؟ | Unconfirmed | دعم التشغيل المفتوح يحتاج trace واضح | يؤثر على data model والدعم والتحقيق في المشاكل | Non-blocking for blueprint / blocking for detailed design |
| 6 | ما هو إصدار Oracle APEX الفعلي وما هي الصفحات المستهدفة أولاً لحقن JavaScript؟ | Partial | البحث الحالي استرشادي فقط | ضروري للـ technical spike ولتقليل المخاطر | Blocking for browser-enhancement preparation |
| 7 | هل صور المواد تُسترجع مباشرة من Oracle attachment table بصيغة ملائمة للعرض دون خدمة وسيطة؟ | Partial | التجربة تعتمد على عرض صور سريع وواضح | يؤثر على UX والأداء | Non-blocking for blueprint / blocking for image implementation |
| 8 | هل يوجد شرط قبول رسمي لمرحلة Desktop ومرحلة Web بشكل منفصل؟ | Unconfirmed | القبول الحالي عام وغير مفصل | ضروري للتحضير والاختبار والتسليم المرحلي | Blocking for acceptance preparation |
| 9 | ما الحد الفاصل بين “إصلاح الأخطاء” و“التطوير الجديد” ضمن الدعم المفتوح؟ | Unconfirmed | يمنع تضخم الالتزام التشغيلي لاحقاً | يؤثر على maintenance/support preparation | Non-blocking for blueprint / important for delivery prep |

---

## Notes

- هذه الأسئلة لا تمنع إنتاج الـ blueprint نفسه.
- لكنها تمنع تحويل الـ blueprint مباشرة إلى تحضير تنفيذي مكتمل دون استكمالها في ملفات التحضير الرسمية اللاحقة.
