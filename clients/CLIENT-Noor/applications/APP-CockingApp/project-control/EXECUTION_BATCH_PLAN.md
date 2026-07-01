# EXECUTION_BATCH_PLAN.md
## CockingApp — خطة دفعات التنفيذ

| Metadata | |
|----------|-|
| **Phase** | 5 — Execution Planning |
| **Status** | Draft v1 |
| **Source** | `PROJECT_MASTER_PLAN.md` + `PROJECT_DETAILED_EXECUTION_PLAN.md` |
| **Batch Size** | 1–3 TASK-IDs (قابلة للمراجعة) |
| **Gate** | Post-Execution Review بعد كل دفعة |
| **Date** | 2026-06-30 |

---

## 1. هيكل الدفعات

```
قبل كل دفعة → Pre-Execution Gate ✅
بعد كل دفعة → Post-Execution Review Gate ✅
بعد كل 3 دفعات → Self-Diagnosis Checkpoint ✅
```

| الدفعة | TASK-IDs | المحتوى | الوقت | التراكمي |
|--------|----------|---------|-------|----------|
| **B1** | 001 | Scaffold Next.js + Prisma + .env.example | 30 min | 30 min | ✅ |
| **B2** | 002 | Prisma Schema كامل (7 models + Unit Enum) | 45 min | 1.25 h |
| **B3** | 003 | هيكل المجلدات + UI Components أساسية + Tailwind/Colors | 30 min | 1.75 h |
| **B4** | 004, 005 | Categories CRUD + Ingredients CRUD (Admin) | 90 min | 3.25 h |
| **B5** | 006 | Recipes CRUD + RecipeForm + IngredientPicker + StepEditor (Admin) | 90 min | 4.75 h |
| **B6** | 007 | Image Upload + Gallery | 45 min | 5.5 h |
| **B7** | 008 | Public pages: الرئيسية + التصنيفات | 45 min | 6.25 h |
| **B8** | 009 | صفحة وصفة مفصلة | 45 min | 7 h |
| **B9** | 010, 011 | Admin Dashboard + Auth (Middleware) | 75 min | 8.25 h |
| **B10** | 012 | بحث | 30 min | 8.75 h |
| **🔄 Self-Diagnosis Checkpoint** | — | مراجعة شاملة بعد B1–B10 | — | — |
| **B11** | 013 | Comments (1B) | 45 min | 9.5 h |
| **B12** | 014, 015 | PDF + Scaler (1B) | 60 min | 10.5 h |
| **B13** | 016 | Time + Search/Filter (1B) | 30 min | 11 h |
| **B14** | 017, 018 | Shopping List + Favorites (1B) | 75 min | 12.25 h |
| **B15** | — | اختبار يدوي + تحسينات نهائية | 45 min | ~13 h |

---

## 2. تفاصيل الدفعات

### B1 — Scaffold Foundation

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 001 | Next.js + Prisma + .env.example | `package.json`, `prisma/schema.prisma` (basic), `src/app/layout.tsx`, `src/lib/prisma.ts` | 30 min |

**Status**: ✅ **Completed** — Post-Execution Review PASS (2026-07-01)

**Pre-Execution Gate**: ✅
- [x] Scaffold flags clean (--no-tailwind, --typescript, --app, --src-dir)
- [x] Prisma basic schema فقط (لا models)
- [x] لا `.env` بقيم حقيقية
- [x] لا `db push`
- [x] لا UI/API/Auth

---

### B2 — Prisma Schema

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 002 | إضافة جميع Prisma models + Enum | `prisma/schema.prisma` (النموذج الكامل من `19_DATABASE_DESIGN.md`) | 45 min |

**Status**: 🔜 **Ready for delegation** — Pre-Execution Gate PASS

**Gate**: `npx prisma generate` يعمل ✅
**Agent**: `cockingapp-data-prisma` (awaiting activation + delegation approval)

---

### B3 — Folder Structure + UI Kit

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 003 | هيكل المجلدات + UI components + Tailwind + Claude colors + RTL | `tailwind.config.ts`, `src/components/ui/*.tsx`, `src/lib/utils.ts` | 30 min |

**Gate**: جميع UI components تعمل ✅

---

### B4 — Admin Core CRUD I

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 004 | Categories CRUD | API + Admin pages | 45 min |
| 005 | Ingredients CRUD | API + Admin pages | 45 min |

**Gate**: CRUD كامل للتصنيفات والمكونات ✅

---

### B5 — Admin Core CRUD II

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 006 | Recipes CRUD + RecipeForm + IngredientPicker + StepEditor | API + Admin pages + Form components | 90 min |

**Gate**: CRUD كامل للوصفات مع مكونات وخطوات ✅

---

### B6 — Image Upload

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 007 | Image upload + gallery | API + ImageUploader | 45 min |

**Gate**: رفع وعرض وحذف صور ✅

---

### B7 — Public Home + Categories

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 008 | الصفحة الرئيسية + التصنيفات العامة | Public pages + RecipeCard + CategoryCard | 45 min |

**Gate**: الواجهة العامة تعرض البيانات من DB ✅

---

### B8 — Recipe Detail Page

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 009 | صفحة وصفة مفصلة | Recipe detail + IngredientList + StepList + ImageGallery | 45 min |

**Gate**: جميع عناصر الوصفة تظهر ✅

---

### B9 — Dashboard + Auth

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 010 | Admin Dashboard | Dashboard page + API + Stat Cards | 30 min |
| 011 | Auth + Middleware | Login page + JWT + middleware | 45 min |

**Gate**: Admin Dashboard محمي ويعمل ✅

---

### B10 — Search

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 012 | بحث في عناوين الوصفات | Search page + SearchBar + API update | 30 min |

**Gate**: البحث يعمل ✅

---

### 🔄 Self-Diagnosis Checkpoint

```
بعد 10 دفعات — مراجعة شاملة قبل بدء 1B

- عدد الـ TASK-IDs المنفذة: 12/18
- هل لا يزال المسار متوافقاً مع النطاق؟
- هل هناك Issues مفتوحة؟
- هل الـ activity log محدث؟
```

---

### B11 — Comments (1B)

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 013 | Comment system | API + Admin page + CommentSection | 45 min |

**Gate**: التعليقات مع الموافقة تعمل ✅

---

### B12 — PDF + Scaler (1B)

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 014 | تحميل PDF | PDF API + Button | 30 min |
| 015 | مقياس ديناميكي | ScalerControl | 30 min |

**Gate**: PDF + Scaler يعملان ✅

---

### B13 — Time + Filter (1B)

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 016 | وقت التحضير والطهي + فلترة | RecipeForm update + Card update + API | 30 min |

**Gate**: الوقت يظهر ويبحث ✅

---

### B14 — Shopping List + Favorites (1B)

| TASK-ID | الوصف | الملفات الرئيسية | الوقت |
|---------|-------|-----------------|-------|
| 017 | قائمة مشتريات ذكية | ShoppingList page + Panel + LocalStorage | 45 min |
| 018 | مفضلة | Favorites page + FavoriteButton + LocalStorage | 30 min |

**Gate**: كلتاهما تعملان مع LocalStorage ✅

---

### B15 — Final Testing & Polish

| المهمة | الوصف | الوقت |
|--------|-------|-------|
| اختبار يدوي | تجربة المستخدم الكاملة (زائر + Admin) | 30 min |
| تحسينات | Fix أي مشاكل تظهر | 15 min |
| تحديث السجلات | PROJECT_STATE, ACTIVITY_LOG, إلخ | 10 min |

**Gate**: ✅ جاهز للتسليم

---

## 3. إجمالي الدفعات

| الدفعات | العدد | TASK-IDs | الوقت الإجمالي |
|---------|-------|----------|---------------|
| B1–B10 | 10 دفعات | 001–012 (Core 1A) | ~8.75 ساعة |
| Checkpoint | 1 | — | — |
| B11–B14 | 4 دفعات | 013–018 (Extended 1B) | ~4 ساعات |
| B15 | 1 (اختبار) | — | ~45 دقيقة |
| **الإجمالي** | **15 دفعة** | **18 TASK-ID** | **~13 ساعة** |

---

## 4. 3 Tera Self-Diagnosis Checkpoints مقترحة

| # | بعد الدفعة | التوقيت |
|---|-----------|---------|
| 1 | بعد B3 | بعد إعداد الأساسيات (Schema + UI Kit) |
| 2 | بعد B6 | بعد Admin Core + Images |
| 3 | بعد B10 | **قبل البدء في 1B** — الأهم |

---

## 5. Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| v1 | 2026-06-30 | Tera | خطة 15 دفعة تنفيذية — 18 TASK-ID عبر 13 ساعة تقديرية |
| v2 | 2026-07-01 | Tera | B1 completed ✅; B2 ready for delegation with Pre-Execution Gate PASS |
