# PROJECT_MASTER_PLAN.md
## CockingApp — خطة التنفيذ الرئيسية

| Metadata | |
|----------|-|
| **Phase** | 5 — Execution Planning |
| **Status** | Draft v1 |
| **Profile** | `nextjs-prisma` |
| **Developer Mode** | Tera Agent |
| **Date** | 2026-06-30 |

---

## 1. الرؤية التنفيذية

| البند | القيمة |
|-------|--------|
| **الهدف** | بناء تطبيق CockingApp — وصفات طبخ مع لوحة تحكم Admin |
| **النطاق** | Core 1A (12 مهمة) + Extended 1B (6 مهام) |
| **إجمالي TASK-IDs** | 18 |
| **المدة المقدرة** | ~24 ساعة تنفيذ |
| **طريقة التنفيذ** | مباشر بواسطة Tera (بدون Sub-Agents) |
| **حجم الدفعة** | 1-3 TASK-IDs لكل دفعة (قابلة للمراجعة) |

---

## 2. المراحل (Milestones)

| Milestone | TASK-IDs | المحتوى | الحالة |
|-----------|----------|---------|--------|
| **M1 — Foundation** | 001–003 | Scaffold + Prisma schema + Folder structure | ⬜ |
| **M2 — Admin Core CRUD** | 004–006 | Categories, Ingredients, Recipes CRUD | ⬜ |
| **M3 — Media & Upload** | 007 | Image upload + gallery | ⬜ |
| **M4 — Public Views** | 008–009 | Home, categories, recipe detail page | ⬜ |
| **M5 — Dashboard & Auth** | 010–011 | Admin dashboard + authentication | ⬜ |
| **M6 — Search** | 012 | Search functionality | ⬜ |
| **M7 — Interactions (1B)** | 013–018 | Comments, PDF, Scaling, Time filter, Shopping list, Favorites | ⬜ |
| **M8 — Testing & Refinement** | — | Manual testing, bug fixes, polish | ⬜ |

---

## 3. التبعيات بين المراحل

```
M1 (Foundation) ← لا تبعيات
    │
    ├──→ M2 (Admin CRUD) ← يحتاج M1
    │       │
    │       ├──→ M3 (Media) ← يحتاج M2
    │       │
    │       └──→ M5 (Dashboard & Auth) ← يحتاج M2
    │
    ├──→ M4 (Public Views) ← يحتاج M1, M2, M3
    │
    ├──→ M6 (Search) ← يحتاج M4
    │
    └──→ M7 (1B Features) ← يحتاج M4
            │
            └──→ M8 (Testing) ← يحتاج M1–M7
```

---

## 4. استراتيجية التنفيذ

### 4.1 أول TASK-ID (حسب Technology Profile)

```
TASK-COD-001: Scaffold Next.js + TypeScript + Prisma + basic schema + .env.example
- لا تضاف Prisma models في أول مهمة
- لا `db push` أو migrations في أول مهمة
- لا UI, API, Auth في أول مهمة
```

### 4.2 مبدأ الدفعات الصغيرة

- كل دفعة: **1-3 TASK-IDs** كحد أقصى
- بعد كل دفعة: **Post-Execution Review Gate** إلزامي
- بعد كل 3 مهام: **Self-Diagnosis Checkpoint**
- لا توجد دفعة تحتوي على أكثر من نوع واحد من العمل (مثل DB + UI + API معاً)

### 4.3 Pre-Execution Gate

قبل كل TASK-ID:
- التحقق من `TeraPreExecutionGate.md`
- التحقق من `nextjs-prisma` profile additions
- التحقق من استعداد الملفات المستهدفة
- التحقق من Acceptance Criteria

---

## 5. إجمالي المهام (18 TASK-ID)

| المعرف | المهمة | المرحلة | التقدير |
|--------|--------|---------|---------|
| 001 | Scaffold Next.js + Prisma + .env.example | M1 | 30 min |
| 002 | Prisma schema كامل + models | M1 | 45 min |
| 003 | هيكل المجلدات + المكونات الأساسية + Tailwind + RTL | M1 | 30 min |
| 004 | إدارة التصنيفات (CRUD + API + UI) Admin | M2 | 45 min |
| 005 | إدارة المكونات (CRUD + API + UI) Admin | M2 | 45 min |
| 006 | إدارة الوصفات (CRUD + API + RecipeForm + مكونات + خطوات) Admin | M2 | 90 min |
| 007 | رفع وإدارة الصور (API + ImageUploader + Gallery) | M3 | 45 min |
| 008 | الواجهة العامة — الرئيسية والتصنيفات (Public pages) | M4 | 45 min |
| 009 | صفحة وصفة مفصلة (مكونات، خطوات، صور، فيديو) | M4 | 45 min |
| 010 | Admin Dashboard (إحصائيات + جداول) | M5 | 30 min |
| 011 | Auth (تسجيل دخول + Middleware + حماية API) | M5 | 45 min |
| 012 | بحث (API + UI) | M6 | 30 min |
| 013 | نظام التعليقات (1B) (Comment model + API + UI + موافقة) | M7 | 45 min |
| 014 | تحميل PDF (1B) | M7 | 30 min |
| 015 | مقياس ديناميكي 📐 (1B) | M7 | 30 min |
| 016 | وقت التحضير والطهي + بحث/فلترة ⏱️ (1B) | M7 | 30 min |
| 017 | قائمة مشتريات ذكية 🛒 (1B) | M7 | 45 min |
| 018 | مفضلة حفظ وصفات ❤️ (1B) | M7 | 30 min |
| | **الإجمالي** | | **~12.75 ساعة** |

> **ملاحظة**: التقديرات للتنفيذ المباشر بواسطة Tera. قد تختلف حسب التعقيدات غير المتوقعة.

---

## 6. Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| v1 | 2026-06-30 | Tera | خطة التنفيذ الرئيسية — 18 TASK-ID عبر 8 Milestones |
