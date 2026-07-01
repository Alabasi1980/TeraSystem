# TASK-COD-002: Prisma Schema كامل

| Metadata | |
|----------|-|
| **TASK-ID** | TASK-COD-002 |
| **Milestone** | M1 — Foundation |
| **Batch** | B2 |
| **Target Version** | v1.0 |
| **Release Type** | Initial |
| **Estimated Time** | 45 min |
| **Dependencies** | TASK-COD-001 ✅ |
| **Sub-Agent** | `cockingapp-data-prisma` |
| **Reviewer** | Tera (physical review) |
| **Status** | Draft |

---

## 1. Objective

إضافة جميع Prisma models إلى `schema.prisma` استناداً إلى `19_DATABASE_DESIGN.md`.

هذه المهمة تكمل الأساس الذي بدأ في TASK-COD-001 بإضافة **7 models + Unit Enum** إلى ملف `prisma/schema.prisma` الموجود حالياً (الذي يحتوي فقط على generator + datasource).

---

## 2. Context

- **Technology Profile**: `nextjs-prisma` — يُسمح بإضافة Prisma models الآن (لم يكن مسموحاً في TASK-COD-001).
- **Active Technology Profile path**: `tera-system/profiles/nextjs-prisma.md`
- **Source Document**: `project-preparation/19_DATABASE_DESIGN.md`

### الملفات المستهدفة

| File | Action |
|------|--------|
| `cocking-app/prisma/schema.prisma` | **WRITE** — إضافة جميع الـ models والـ Enum إلى الملف الموجود |

### الممنوع في هذه المهمة

- ❌ `db push` / migrations
- ❌ UI components
- ❌ API routes
- ❌ Auth
- ❌ تغيير generator أو datasource
- ❌ إضافة أي ملفات أخرى
- ❌ Seed data
- ❌ تغيير package.json أو إضافة تبعيات

---

## 3. الـ Models المطلوبة

يحتوي `19_DATABASE_DESIGN.md` على الـ Pr schema التالية:

| # | Model / Enum | Description |
|---|-------------|-------------|
| 1 | `Unit` (Enum) | وحدات القياس: CUP, TBSP, TSP, KG, GRAM, ML, L, PIECE |
| 2 | `Category` | تصنيفات الوصفات |
| 3 | `Ingredient` | المكونات مع وحدة قياس افتراضية |
| 4 | `Recipe` | الوصفات مع كافة الحقول |
| 5 | `RecipeIngredient` | جدول وسيط (مكونات كل وصفة) |
| 6 | `RecipeImage` | صور الوصفة |
| 7 | `RecipeStep` | خطوات التحضير |
| 8 | `Comment` | التعليقات (1B) |

> **ملاحظة:** التعليقات (Comment) مصنفة ضمن 1B (Extended MVP) لكن وجودها في schema لا يؤثر على التنفيذ. يتم تضمينها كاملة الآن لتجنب تغيير schema لاحقاً. الـ `prepTime`, `cookTime`, `servings` في Recipe هي أيضاً 1B وتُضمَّن في schema.

---

## 4. Model Capability Assessment

```text
Model Capability Assessment
Current Model: deepseek-v4-flash-free (current runtime model)
Task Complexity: Low — ملف واحد، models محددة مسبقاً في 19_DATABASE_DESIGN.md، لا قرارات تصميمية
Risk Level: Low — لا تغييرات في قاعدة البيانات (لا migrations الآن)، لا بيانات حقيقية
Required Reasoning: Low — ترجمة مباشرة من تصميم موجود
Context Size: Low — ملف واحد، وثيقة واحدة مرجعية
Verification Difficulty: Low — `npx prisma generate` يؤكد الصحة النحوية
Historical Fit: Good — Prisma schema writing is a well-established pattern
Recommended Model Tier: Light
Minimum Acceptable Model Tier: Light
Cost-Saving Option: Use current model — no stronger model needed
User-Facing Recommendation Required: No
Decision: sufficient
Reason: مهمة بسيطة، مباشرة، ملف واحد، verification سهل
Required Safeguards: التزام تام بـ 19_DATABASE_DESIGN.md دون إضافة أو إزالة
User Approval Needed: No
```

---

## 5. Pre-Execution Gate

### Pre-Execution Gate Checklist

| Check | Result | Notes |
|-------|--------|-------|
| **Implementation Agent Strategy** exists and is approved | ✅ PASS | Option B approved — DataPrismaAgent listed for TASK-COD-002 |
| **Technology Profile** loaded | ✅ PASS | `nextjs-prisma` — models allowed now |
| **Model Capability Gate** applied | ✅ PASS | Light model sufficient |
| **Security Sensitivity** determined | Low | No Auth/API/Secrets — Prisma model definitions only |
| **Allowed Sources** identified | ✅ PASS | `19_DATABASE_DESIGN.md` + existing `schema.prisma` |
| **Allowed Write Targets** defined | ✅ PASS | `cocking-app/prisma/schema.prisma` only |
| **Forbidden actions** defined | ✅ PASS | No migrations, no db push, no UI/API/Auth, no extra files |
| **Dependencies satisfied** | ✅ PASS | TASK-COD-001 ✅ |
| **Project-control records reviewed** | ✅ PASS | State reflects completed TASK-COD-001 |
| **Previous issues reviewed** | ✅ PASS | IS-001, IS-002, IS-003, IS-004 checked — none block B2 |
| **Design Source Decision** | N/A | No UI task |
| **UI Acceptance Gate** | N/A | No UI task |
| **Engineering Governance Gate applied** | ✅ PASS | Explicit reference: `tera-system/engineering-governance/ENGINEERING_GOVERNANCE_GATE.md` — 12 pre-checks verified |
| **No silent maintainability violation** | ✅ PASS | Single file, single responsibility (Prisma schema), no UI/Logic mixing, no shared/utils misuse |
| **Target Version / Release Type** set | ✅ PASS | v1.0 / Initial |
| **Security Agent required?** | No | Low sensitivity — no Auth/API/Secrets |
| **QA Agent required?** | No | Tera will review schema directly |

### Pre-Execution Gate Result

```text
Pre-Execution Gate Result: PASS
Date: 2026-07-01
Reviewer: Tera Agent
Notes: Ready for delegation to DataPrismaAgent
```

---

## 6. Delegation Package

عند التفويض إلى `cockingapp-data-prisma`، يُرسل ما يلي:

```text
Task ID: TASK-COD-002
Requested Agent: CockingApp DataPrismaAgent
Stage: Implementation (Phase 6)
Objective: Complete the Prisma schema with all 7 models + Unit Enum in prisma/schema.prisma
Context Type: Task Context
Reference Files:
  - project-control/tasks/TASK-COD-002.md
  - cocking-app/prisma/schema.prisma
  - project-preparation/19_DATABASE_DESIGN.md
Required Sections:
  - All 7 models + Unit Enum from 19_DATABASE_DESIGN.md
  - Generator + datasource blocks preserved from existing file
  - All @@index, @@map, @relation, @unique directives as specified
  - All field types and modifiers as specified
  - No extra fields or models
Allowed Write Targets:
  - cocking-app/prisma/schema.prisma (modify existing file)
Forbidden Files / Actions:
  - No files outside cocking-app/prisma/schema.prisma
  - No migrations, db push, studio, seed
  - No UI, API, Auth
  - No package.json changes
  - No real secrets
Token Budget: Light
Model Tier Recommendation: Light
Minimum Acceptable Model Tier: Light
Current Model Assessment: sufficient
Cost Note: No user approval needed
Acceptance Criteria:
  1. All 7 models + Unit Enum defined exactly as per 19_DATABASE_DESIGN.md
  2. Relationships correct (1:M, M:M via RecipeIngredient)
  3. @@index on all specified fields
  4. @@map for table names to snake_case
  5. npx prisma generate runs without errors
  6. Existing generator + datasource preserved unmodified
Return Status Required: Done / Blocked / Needs Clarification
```

---

## 7. معايير القبول (Acceptance Criteria)

- [ ] **AC1:** All 7 models + Unit Enum موجودة ومعرّفة بالكامل
- [ ] **AC2:** العلاقات صحيحة (1:M، M:M عبر RecipeIngredient مع `@@unique([recipeId, ingredientId])`)
- [ ] **AC3:** `@@index` موجودة على جميع الحقول المطلوبة (categoryId, slug, isPublished+publishedAt, recipeId, ingredientId, stepNumber, isApproved)
- [ ] **AC4:** `@@map` موجودة لتحويل أسماء الجداول إلى snake_case
- [ ] **AC5:** `npx prisma generate` يعمل بدون أخطاء
- [ ] **AC6:** الأنواع صحيحة: String, Int, Decimal, DateTime, Boolean, Unit?, String?
- [ ] **AC7:** `@relation` مع `onDelete: Cascade` مضبوطة على RecipeIngredient, RecipeImage, RecipeStep, Comment
- [ ] **AC8:** `@default` للقيم الافتراضية صحيحة (autoincrement, now, false, 0, GRAM)
- [ ] **AC9:** الملفات الأخرى لم تتغير (package.json, tsconfig.json, إلخ)
- [ ] **AC10:** Generator + datasource blocks لم يتغيرا

---

## 8. Execution Plan

| Step | Action | Command |
|------|--------|---------|
| 1 | Read current `schema.prisma` | حفظ المحتوى الحالي |
| 2 | إضافة Unit Enum | قبل الـ models |
| 3 | إضافة Category model | بعد Enum |
| 4 | إضافة Ingredient model | |
| 5 | إضافة Recipe model | مع العلاقات |
| 6 | إضافة RecipeIngredient model | junction table |
| 7 | إضافة RecipeImage model | |
| 8 | إضافة RecipeStep model | |
| 9 | إضافة Comment model | |
| 10 | تشغيل `npx prisma generate` للتحقق | من دليل `cocking-app/` |
| 11 | رفع النتيجة إلى Tera | |

---

## 9. Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| v1 | 2026-07-01 | Tera | إنشاء المهمة مع Pre-Execution Gate PASS |
