# DESIGN_REVIEW_STANDARDS.md

## الغرض

هذا الملف هو **قاعدة المعايير** التي يستخدمها DesignReviewer (ناقد) كمرجع قبل وأثناء كل مراجعة تصميم. يحتوي على قوائم تفتيش قابلة للتنفيذ تغطي جميع جوانب مراجعة الواجهات.

**العلاقة:** هذا ملف **مرجع معرفي** (Reference)، وليس Source of Truth لهوية العميل. Source of Truth هو `tera-system/TeraDesignReviewer.md`.

**الاستخدام:** يقرأ ناقد هذا الملف قبل كل مراجعة ويطبق القوائم المناسبة حسب Phase المراجعة (Pre-Implementation / Post-Implementation).

---

## الفهرس

| القسم | المحتوى |
|-------|---------|
| §1 | اكتمال الحالات والسيناريوهات (Functional & Edge Case Completeness) |
| §2 | الاتساق البصري (Visual Consistency) |
| §3 | الجودة البصرية (Visual Quality Principles) |
| §4 | نظافة التصميم (Design Hygiene) |
| §5 | تجربة المستخدم (UX Principles) |
| §6 | الاستجابة والعرض المتعدد (Responsive) |
| §7 | التدقيق الخاص بالعربية و RTL (RTL & Arabic Review) |
| §8 | فحص التوكينز (Design Token Verification) |
| §9 | الاتساق الوظيفي (Functional Consistency) |

---

## §1. اكتمال الحالات والسيناريوهات (Functional & Edge Case Completeness)

```
1.1 Empty/Loading/Error States
[ ] هل كل شاشة تعرض حالة "فارغ" (Empty) بشكل مناسب؟
[ ] هل هناك مؤشر تحميل (Loader/Skeleton) أثناء جلب البيانات؟
[ ] هل رسائل الخطأ مفهومة وودية — لا "Error 500" بل "تعذر الاتصال بالخادم"؟
[ ] هل الحالات القصوى مغطاة: 0 نتائج، 1000+ نتيجة، إدخال فارغ؟

1.2 Edge Cases
[ ] ماذا يحدث عند إدخال نصوص طويلة جداً؟
[ ] ماذا يحدث عند إدخال رموز خاصة أو emojis؟
[ ] ماذا يحدث عند فقدان الاتصال بالإنترنت أثناء معاملة؟
[ ] ماذا يحدث عند محاولة إجراء بدون صلاحية؟
[ ] ماذا يحدث عند إعادة تحميل الصفحة (Refresh) — هل الحالة محفوظة؟
```

## §2. الاتساق البصري (Visual Consistency)

```
[ ] الألوان: جميعها من التوكينز المعتمدة — لا ألوان عشوائية
[ ] الخطوط: نوع خط واحد للعناوين، واحد للنصوص — لا خلط
[ ] المسافات (Spacing): تتبع مقياساً موحداً (4px-8px-12px-16px-24px-32px)
[ ] الزوايا (Border-radius): موحّدة لكل نوع عنصر (أزرار 8px، بطاقات 12px، إلخ)
[ ] الظلال (Shadows): موحّدة لكل مستوى (رفيع، متوسط، عالي)
[ ] الأيقونات: من مجموعة واحدة، حجم واحد لكل استخدام

2.1 Modal Consistency
[ ] كل المودالات: عنوان + محتوى + أزرار إجراء في نفس الترتيب
[ ] كل المودالات: نفس العرض والحجم النسبي
[ ] كل المودالات: زر إغلاق في نفس المكان
[ ] التغطية: Open → Close → Error → Blocking (خلفية معتمة)

2.2 Table Consistency
[ ] هيدر مثبت عند التمرير
[ ] صفوف متناوبة (Stripped rows)
[ ] Hover effect على الصفوف
[ ] فرز (Sort) متاح على الأعمدة الرئيسية
[ ] هل الجدول يتصرف بشكل جيد على الشاشات الصغيرة؟ (scroll أفقي أو تحويل لبطاقات)

2.3 Button Consistency
[ ] Primary button: لون واحد، حجم واحد، شكل واحد في كل التطبيق
[ ] Secondary button: نمط واحد مختلف عن Primary
[ ] Danger button: لون خطر واحد لكل تأكيدات الحذف
[ ] Disabled state: موجود لجميع الأزرار
[ ] Loading state: موجود للأزرار التي تنفذ إجراءات
```

## §3. الجودة البصرية (Visual Quality Principles)

```
[ ] التدرج الهرمي البصري واضح: عنوان الصفحة > عنوان القسم > عنوان البطاقة > النص
[ ] المسافات البيضاء مناسبة: لا ازدحام، لا فراغات مهدرة
[ ] المحتوى منظم بتجميع منطقي: العناصر المرتبطة قريبة من بعضها
[ ] التناقض اللوني (Contrast): النصوص مقروءة على خلفياتها (WCAG AA minimum)
[ ] المحتوى غير مكرر بدون سبب
```

## §4. نظافة التصميم (Design Hygiene)

```
[ ] لا معلومات برمجية ظاهرة (Stack traces, console.log, debug text)
[ ] لا نصوص وهمية (Lorem ipsum, dummy content متروك)
[ ] لا أيقونات/صور Broken (placeholder متروك)
[ ] لا نصوص تقنية في واجهة المستخدم (مثل "User ID: 123" بدلاً من "اسم المستخدم")
[ ] لا أزرار/روابط تؤدي إلى nowhere
[ ] لا أخطاء إملائية في النصوص العربية أو الإنجليزية
```

## §5. تجربة المستخدم (UX Principles)

```
[ ] النماذج الطويلة مجزّأة: خطوات (Wizard) أو تبويبات (Tabs)
[ ] كل إجراء مهم له تأكيد: حذف، إلغاء، حفظ، إرسال
[ ] رسائل النجاح/الخطأ واضحة وقابلة للتنفيذ
[ ] التغذية الراجعة الفورية: Loader, Toast, Confirmation
[ ] تكبير الأهداف اللمسية (Touch targets): 44×44px minimum
[ ] العناصر القابلة للنقر واضحة: أزرار، روابط، أيقونات تفاعلية
[ ] الـ Navigation منطقي: عدد الخطوات للوصول للهدف قليل
[ ] البحث: متاح وفعّال حيثما توجد كميات كبيرة من البيانات
```

## §6. الاستجابة والعرض المتعدد (Responsive)

```
[ ] المحتوى لا يخرج عن الشاشة في أي حجم
[ ] الجداول تتصرف جيداً على الشاشات الصغيرة
[ ] القوائم (Nav/Sidebar) لا تتراكض
[ ] الأزرار الرئيسية لا تختفي ولا تصبح غير قابلة للنقر
[ ] الـ Forms قابلة للاستخدام على الموبايل
[ ] الصور تتكيف مع حجم الشاشة
[ ] تم الاختبار على: 360px, 768px, 1024px, 1440px (كحد أدنى)
```

## §7. التدقيق الخاص بالعربية و RTL (RTL & Arabic Review)

```
[ ] layout: dir="rtl" مضبوط على <html> وليس فقط بعض العناصر
[ ] CSS Logical Properties: margin-inline-start/end بدلاً من margin-left/right
[ ] padding-inline / margin-inline / inset-inline مستخدمة بدلاً من البدائل الفيزيائية
[ ] text-align: start | end بدلاً من left | right
[ ] Flexbox/Grid: الاعتماد على auto-flow وليس تحديد أعمدة hardcoded

7.1 Icon Mirroring
[ ] الأسهم وchevrons: mirror (scaleX(-1)) في RTL
[ ] أزرار الـ navigation (السابق/التالي): mirror
[ ] Breadcrumb arrows: mirror
[ ] أيقونات عالمية (search, checkmark, gear, star, bell): لا تلمس
[ ] استخدام class .icon-directional مع [dir="rtl"] selector

7.2 Arabic Typography
[ ] Font size: الخط العربي أكبر 10-15% من الإنجليزي (مثلاً 16px EN → 18px AR)
[ ] Line height: 1.8 كحد أدنى للنصوص العربية (لوجود التشكيل)
[ ] Font selection: Cairo, Tajawal, Noto Sans Arabic, IBM Plex Sans Arabic
[ ] تجنب justified text (text-align: justify) في العربية — يسبب فراغات غير منتظمة

7.3 Form Fields in RTL
[ ] حقول الإيميل ورقم الهاتف والـ URL: dir="ltr" صراحة
[ ] الـ placeholders: اختبار عكسي في RTL
[ ] Validation messages: محاذاتها صحيحة (تبدأ من اليمين)
[ ] Mixed content: استخدام dir="auto" للنصوص غير المعروفة الاتجاه

7.4 Animations
[ ] Animations: استخدام CSS logical properties أو متغيرات (custom properties) بدلاً من translateX- المباشر
[ ] Slide-in/slide-out panels: الاتجاه معكوس في RTL
[ ] Progress bars: flexbox من البداية، ليس width- على element واحد

7.5 Common RTL Mistakes Checklist
[ ] Physical margin/padding → استخدام inline-start/end
[ ] text-align: left → استخدام text-align: start
[ ] Physical position (left:0, right:0) → استخدام inset-inline-start/end
[ ] translateX في animations → استخدام CSS custom property
[ ] flex-direction: row-reverse لأغراض LTR → استخدام row + direction auto
[ ] Hardcoded LTR icons → mirror باستخدام scaleX(-1) في RTL
[ ] dir attribute missing → موجود على <html>
[ ] Email/phone fields ترث RTL → dir="ltr" صراحة
[ ] لا يوجد RTL test locale → إضافة ar/he لبيئة الاختبار
[ ] استخدام أعلام الدول لاختيار اللغة → استخدام أسماء اللغات أو الرموز
```

## §8. فحص التوكينز (Design Token Verification)

```
[ ] مصدر التوكينز محدّد: tailwind.config / variables.css / tokens.json / DESIGN.md
[ ] القائمة المعتمدة مستخرجة: colors, spacing, fonts, radii, shadows
[ ] الـ grep كشف: أي hardcoded values بدلاً من التوكينز؟
[ ] المكونات المتشابهة تستخدم نفس التوكينز؟
[ ] هل بنية التوكينز منظمة في 3 طبقات؟ (للمشاريع المتوسطة والكبيرة)

8.1 Primitive Tokens (القيم الخام)
مثال: --color-blue-500: #3B82F6

8.2 Semantic Tokens (المعنى السياقي)
مثال: --color-primary: var(--color-blue-500)
القاعدة: المكونات تشير إلى Semantic tokens وليس Primitive مباشرة

8.3 Component Tokens (مستوى المكون — اختياري)
مثال: --btn-primary-bg: var(--color-primary)
```

## §9. الاتساق الوظيفي (Functional Consistency)

```
[ ] هل جميع الـ tooltips تتصرف بنفس الطريقة؟ (hover/mouseover/timeout)
[ ] هل جميع الـ toasts تختفي بعد نفس المدة؟
[ ] هل جميع الـ drawers تفتح/تغلق بنفس الآلية؟
[ ] هل كل نموذج له زر "حفظ" و "إلغاء" في نفس الترتيب؟
[ ] هل الضغط على ESC يغلق جميع المودالات والقوائم المنسدلة؟
[ ] هل الـ validation trigger (عند التركيز/عند الخروج/عند الإرسال) موحّد؟
```
