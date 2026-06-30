# PROJECT_DETAILED_EXECUTION_PLAN.md
## CockingApp — خطة التنفيذ التفصيلية

| Metadata | |
|----------|-|
| **Phase** | 5 — Execution Planning |
| **Status** | Draft v1 |
| **Source** | `09_IMPLEMENTATION_PLAN.md` + `PROJECT_MASTER_PLAN.md` |
| **Profile** | `nextjs-prisma` |
| **Date** | 2026-06-30 |

---

## TASK-COD-001: Scaffold Next.js + TypeScript + Prisma

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M1 — Foundation |
| **التقدير** | 30 min |
| **التبعية** | — (أول مهمة) |
| **Sub-Agent** | Tera مباشر |

### الوصف
إنشاء مشروع Next.js مع TypeScript، إعداد Prisma مع PostgreSQL، تثبيت الاعتماديات الأساسية.

### الملفات المستهدفة
- `package.json`, `tsconfig.json`, `next.config.ts`, `tailwind.config.ts`
- `prisma/schema.prisma` (basic: generator + datasource فقط)
- `src/app/layout.tsx` (RTL setup)
- `src/lib/prisma.ts` (singleton)
- `.env.example`

### الممنوع في هذه المهمة (حسب `nextjs-prisma` profile)
- ❌ Prisma models
- ❌ `db push` / migrations
- ❌ UI components
- ❌ API routes
- ❌ Auth
- ❌ `.env` بقيم حقيقية

### معايير القبول
- [ ] `npm run dev` يعمل بدون أخطاء
- [ ] RTL مضبوط في layout.tsx
- [ ] Prisma client ينشئ بدون مشاكل
- [ ] `.env.example` مع DATABASE_URL placeholder

### أمر الـ Scaffold
```bash
npx create-next-app@latest cocking-app --typescript --eslint --app --src-dir --no-tailwind --import-alias "@/*" --use-npm
```

---

## TASK-COD-002: Prisma Schema كامل

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M1 — Foundation |
| **التقدير** | 45 min |
| **التبعية** | 001 |
| **Sub-Agent** | Tera مباشر |

### الوصف
إضافة جميع Prisma models إلى `schema.prisma` استناداً إلى `19_DATABASE_DESIGN.md`.

### الملفات المستهدفة
- `prisma/schema.prisma` — إضافة Category, Ingredient, Recipe, RecipeIngredient, RecipeImage, RecipeStep, Comment, Unit Enum

### معايير القبول
- [ ] جميع الـ 9 models + Enum معرفة
- [ ] العلاقات صحيحة (1:M, M:M عبر جدول وسيط)
- [ ] `@@index` معرفة على الحقول المطلوبة
- [ ] `@@map` لتحويل أسماء الجداول إلى snake_case
- [ ] `npx prisma generate` يعمل بدون أخطاء

---

## TASK-COD-003: هيكل المجلدات + المكونات الأساسية

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M1 — Foundation |
| **التقدير** | 30 min |
| **التبعية** | 001 |
| **Sub-Agent** | Tera مباشر |

### الوصف
إنشاء هيكل المجلدات حسب `08_TECHNICAL_ARCHITECTURE.md` وإعداد المكونات الأساسية (ui components) + Tailwind مع ألوان Claude design.

### الملفات المستهدفة
- `tailwind.config.ts` — ألوان Claude (cream, coral, dark navy)
- `src/components/ui/Button.tsx`, `Input.tsx`, `Select.tsx`, `Card.tsx`, `Modal.tsx`, `Toast.tsx`, `Spinner.tsx`
- `src/lib/utils.ts`

### معايير القبول
- [ ] هيكل المجلدات مطابق للـ architecture
- [ ] ألوان Claude مضبوطة في Tailwind
- [ ] Tajawal font مضبوط
- [ ] المكونات الأساسية موجودة وتعمل

---

## TASK-COD-004: إدارة التصنيفات (Admin)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M2 — Admin Core CRUD |
| **التقدير** | 45 min |
| **التبعية** | 002, 003 |
| **Sub-Agent** | Tera مباشر |

### الوصف
إنشاء CRUD كامل للتصنيفات في لوحة التحكم: API routes + صفحة قائمة + صفحة إنشاء/تعديل.

### الملفات المستهدفة
- `src/app/api/admin/categories/route.ts` (GET, POST)
- `src/app/api/admin/categories/[id]/route.ts` (GET, PUT, DELETE)
- `src/app/admin/categories/page.tsx` (قائمة)
- `src/app/admin/categories/new/page.tsx` (إنشاء)
- `src/app/admin/categories/[id]/edit/page.tsx` (تعديل)

### معايير القبول
- [ ] إضافة تصنيف مع اسم عربي و slug تلقائي
- [ ] تعديل تصنيف
- [ ] حذف تصنيف (مع حماية إذا كان يحتوي وصفات)
- [ ] ترتيب التصنيفات (sortOrder)
- [ ] التحقق من صحة البيانات (اسم فريد)

---

## TASK-COD-005: إدارة المكونات (Admin)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M2 — Admin Core CRUD |
| **التقدير** | 45 min |
| **التبعية** | 002, 003 |

### الوصف
إنشاء CRUD كامل للمكونات في لوحة التحكم: API routes + صفحة قائمة + صفحة إنشاء/تعديل مع اختيار وحدة القياس.

### الملفات المستهدفة
- `src/app/api/admin/ingredients/route.ts`
- `src/app/api/admin/ingredients/[id]/route.ts`
- `src/app/admin/ingredients/page.tsx`
- `src/app/admin/ingredients/new/page.tsx`
- `src/app/admin/ingredients/[id]/edit/page.tsx`

### معايير القبول
- [ ] إضافة مكون مع اسم، وحدة قياس افتراضية
- [ ] slug تلقائي من الاسم
- [ ] التحقق من uniqueness
- [ ] حماية الحذف (إذا كان المكون مستخدماً في وصفات)

---

## TASK-COD-006: إدارة الوصفات (Admin) — المهمة الأكبر

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M2 — Admin Core CRUD |
| **التقدير** | 90 min |
| **التبعية** | 004, 005 |

### الوصف
إنشاء CRUD كامل للوصفات مع ربط المكونات وإضافة خطوات التحضير. هذه المهمة الأكثر تعقيداً.

### الملفات المستهدفة
- `src/app/api/admin/recipes/route.ts` (GET, POST)
- `src/app/api/admin/recipes/[id]/route.ts` (GET, PUT, DELETE)
- `src/app/admin/recipes/page.tsx` (قائمة)
- `src/app/admin/recipes/new/page.tsx` (إنشاء — مع RecipeForm)
- `src/app/admin/recipes/[id]/edit/page.tsx` (تعديل)
- `src/components/admin/RecipeForm.tsx`
- `src/components/admin/IngredientPicker.tsx`
- `src/components/admin/StepEditor.tsx`

### معايير القبول
- [ ] إنشاء وصفة مع: عنوان، تصنيف، مكونات (كمية + وحدة)، خطوات، رابط فيديو، وقت (1B)
- [ ] تعديل وصفة (مع إضافة/إزالة مكونات وخطوات)
- [ ] حذف وصفة مع Cascade
- [ ] حالة النشر (مسودة/منشور)
- [ ] التحقق من صحة البيانات (مكون واحد على الأقل، خطوة واحدة على الأقل)

---

## TASK-COD-007: رفع وإدارة الصور

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M3 — Media & Upload |
| **التقدير** | 45 min |
| **التبعية** | 006 |

### الوصف
إنشاء نظام رفع الصور: API route للرفع، مكون ImageUploader، معرض صور في Admin.

### الملفات المستهدفة
- `src/app/api/admin/recipes/[id]/images/route.ts` (POST, DELETE)
- `src/components/admin/ImageUploader.tsx`
- تحديث `RecipeForm.tsx` (إضافة ImageUploader)
- تحديث `RecipeImage` model (إذا لزم الأمر)

### معايير القبول
- [ ] رفع صورة (JPG, PNG, WebP)
- [ ] التحقق من نوع الملف وحجمه (max 5MB)
- [ ] اختيار الصورة الرئيسية
- [ ] حذف صورة
- [ ] تخزين آمن (إعادة تسمية UUID)

---

## TASK-COD-008: الواجهة العامة — الرئيسية والتصنيفات

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M4 — Public Views |
| **التقدير** | 45 min |
| **التبعية** | 006, 007 |

### الوصف
إنشاء الصفحة الرئيسية للتطبيق وصفحات التصنيفات للزوار.

### الملفات المستهدفة
- `src/app/page.tsx` — الصفحة الرئيسية
- `src/app/categories/page.tsx` — قائمة التصنيفات
- `src/app/categories/[slug]/page.tsx` — وصفات حسب التصنيف
- `src/components/public/RecipeCard.tsx`
- `src/components/public/CategoryCard.tsx`

### معايير القبول
- [ ] عرض أحدث الوصفات في الصفحة الرئيسية
- [ ] عرض التصنيفات كبطاقات
- [ ] النقر على تصنيف → عرض وصفاته
- [ ] تصميم متجاوب (جوال + سطح مكتب)

---

## TASK-COD-009: صفحة وصفة مفصلة

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M4 — Public Views |
| **التقدير** | 45 min |
| **التبعية** | 008 |

### الوصف
صفحة عرض وصفة مفصلة للزوار: مكونات، خطوات، صور، فيديو.

### الملفات المستهدفة
- `src/app/recipes/[slug]/page.tsx`
- `src/components/public/IngredientList.tsx`
- `src/components/public/StepList.tsx`
- `src/components/public/ImageGallery.tsx`
- `src/app/api/recipes/[slug]/route.ts` (GET)

### معايير القبول
- [ ] عرض عنوان، صورة رئيسية، معرض صور
- [ ] عرض المكونات مع الكميات والوحدات
- [ ] عرض خطوات التحضير مرقمة
- [ ] تضمين فيديو YouTube (إن وجد)
- [ ] عرض وقت التحضير والطهي (إن وجد — 1B)
- [ ] زر المشاركة

---

## TASK-COD-010: Admin Dashboard

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M5 — Dashboard & Auth |
| **التقدير** | 30 min |
| **التبعية** | 004, 005, 006 |

### الوصف
إنشاء Dashboard رئيسي مع إحصائيات وجداول.

### الملفات المستهدفة
- `src/app/admin/page.tsx` — Dashboard
- `src/app/api/admin/dashboard/route.ts`
- `src/components/admin/DashboardCards.tsx`

### معايير القبول
- [ ] عرض إجمالي الوصفات والتصنيفات والمكونات
- [ ] جدول أحدث الوصفات
- [ ] تصميم نظيف مع بطاقات إحصائية

---

## TASK-COD-011: Auth + حماية Admin

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M5 — Dashboard & Auth |
| **التقدير** | 45 min |
| **التبعية** | 003 |

### الوصف
إنشاء نظام مصادقة للوحة التحكم: تسجيل دخول، JWT، Middleware، حماية API Routes.

### الملفات المستهدفة
- `src/app/admin/login/page.tsx` — صفحة تسجيل الدخول
- `src/app/api/admin/auth/login/route.ts`
- `src/app/api/admin/auth/me/route.ts`
- `src/app/api/admin/auth/logout/route.ts`
- `src/lib/auth.ts`
- `src/middleware.ts` — حماية /admin/*

### معايير القبول
- [ ] تسجيل دخول يعمل (username/password من env)
- [ ] كلمة المرور محمية (bcrypt)
- [ ] JWT مخزن في HTTP-Only Cookie
- [ ] Middleware يعيد توجيه غير المصرح لهم
- [ ] API Routes محمية

---

## TASK-COD-012: بحث

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M6 — Search |
| **التقدير** | 30 min |
| **التبعية** | 008 |

### الوصف
إنشاء نظام بحث بسيط في عناوين الوصفات.

### الملفات المستهدفة
- `src/app/search/page.tsx`
- `src/components/public/SearchBar.tsx`
- تحديث `src/app/api/recipes/route.ts` (إضافة معامل `q`)

### معايير القبول
- [ ] البحث في عناوين الوصفات
- [ ] نتائج فورية (أو مع submit)
- [ ] عرض رسالة "لا توجد نتائج" عند عدم وجود نتائج

---

## TASK-COD-013: نظام التعليقات (1B)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M7 — Interactions (1B) |
| **التقدير** | 45 min |
| **التبعية** | 009 |

### الوصف
إنشاء نظام تعليقات عامة مع موافقة مسبقة من Admin.

### الملفات المستهدفة
- `src/app/api/recipes/[slug]/comments/route.ts` (GET + POST)
- `src/app/api/admin/comments/route.ts` (GET, PUT)
- `src/app/admin/comments/page.tsx`
- `src/components/public/CommentSection.tsx`

### معايير القبول
- [ ] إضافة تعليق باسم ونص
- [ ] التعليق لا يظهر فوراً (قيد المراجعة)
- [ ] Admin يمكنه الموافقة أو الحذف
- [ ] عرض التعليقات المعتمدة فقط للزوار
- [ ] حماية ضد HTML و SPAM مبدأي

---

## TASK-COD-014: تحميل PDF (1B)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M7 — Interactions (1B) |
| **التقدير** | 30 min |
| **التبعية** | 009 |

### الوصف
إضافة زر تحميل PDF لكل وصفة.

### الملفات المستهدفة
- `src/app/api/recipes/[slug]/pdf/route.ts`
- `src/lib/pdf.ts`
- `src/components/public/PDFDownloadButton.tsx`
- تحديث صفحة الوصفة

### معايير القبول
- [ ] PDF يحتوي: عنوان، صورة، مكونات، خطوات، وقت
- [ ] PDF يدعم العربية (RTL)
- [ ] زر التحميل يعمل

---

## TASK-COD-015: مقياس ديناميكي 📐 (1B)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M7 — Interactions (1B) |
| **التقدير** | 30 min |
| **التبعية** | 009 |

### الوصف
إضافة أداة لتكبير/تصغير كميات الوصفة حسب عدد الأشخاص.

### الملفات المستهدفة
- `src/components/public/ScalerControl.tsx`
- تحديث صفحة الوصفة

### معايير القبول
- [ ] إدخال عدد الأشخاص يغير الكميات
- [ ] الحساب: الكمية الجديدة = الأصلية × (جديد ÷ أصلي)
- [ ] زر إعادة تعيين
- [ ] لا تغيير في قاعدة البيانات

---

## TASK-COD-016: وقت التحضير والطهي ⏱️ (1B)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M7 — Interactions (1B) |
| **التقدير** | 30 min |
| **التبعية** | 006, 008 |

### الوصف
إضافة وقت التحضير والطهي للوصفات مع إمكانية البحث والفلترة بالوقت.

### الملفات المستهدفة
- تحديث `RecipeForm.tsx` (إضافة prepTime, cookTime, servings)
- تحديث `RecipeCard.tsx` (عرض الوقت)
- تحديث `src/app/api/recipes/route.ts` (فلترة بالوقت)
- تحديث واجهة البحث

### معايير القبول
- [ ] Admin يمكنه إضافة وقت التحضير والطهي
- [ ] الوقت يظهر في بطاقة الوصفة وتفاصيلها
- [ ] بحث/فلترة بالوقت (<= 15، 30، 60، > 60 دقيقة)

---

## TASK-COD-017: قائمة مشتريات ذكية 🛒 (1B)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M7 — Interactions (1B) |
| **التقدير** | 45 min |
| **التبعية** | 009 |

### الوصف
إنشاء قائمة مشتريات تجمع مكونات وصفة/وصفات.

### الملفات المستهدفة
- `src/app/shopping-list/page.tsx`
- `src/components/public/ShoppingListPanel.tsx`
- إضافة زر "أضف إلى المشتريات" في صفحة الوصفة

### معايير القبول
- [ ] إضافة مكونات وصفة إلى القائمة
- [ ] دمج المكونات المتكررة (تجميع الكميات)
- [ ] تخزين محلي (LocalStorage)
- [ ] طباعة القائمة
- [ ] إزالة مكونات من القائمة

---

## TASK-COD-018: مفضلة حفظ وصفات ❤️ (1B)

| الحقل | القيمة |
|-------|--------|
| **المرحلة** | M7 — Interactions (1B) |
| **التقدير** | 30 min |
| **التبعية** | 008 |

### الوصف
إضافة نظام مفضلة للزوار (تخزين محلي).

### الملفات المستهدفة
- `src/app/favorites/page.tsx` — صفحة المفضلة
- `src/components/public/FavoriteButton.tsx`
- إضافة أيقونة القلب في بطاقة الوصفة وصفحة التفاصيل

### معايير القبول
- [ ] الضغط على القلب → حفظ/إزالة من المفضلة
- [ ] المفضلة مخزنة في LocalStorage
- [ ] صفحة المفضلة تعرض الوصفات المحفوظة
- [ ] أيقونة القلب تعكس الحالة (مفضل/غير مفضل)

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| v1 | 2026-06-30 | Tera | خطة تنفيذ تفصيلية — 18 TASK-ID مع الملفات المستهدفة ومعايير القبول |
