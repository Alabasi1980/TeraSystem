# 11_DELIVERY_AND_HANDOVER.md

## مستند التسليم — نظام إدارة الشيكات (Checks Management MVP)

---

## 1. ملخص التسليم

| البند | القيمة |
|---|---|
| اسم المشروع | نظام إدارة الشيكات (Checks Management) |
| الإصدار | MVP v1.0 |
| تاريخ التسليم | 2026-06-28 |
| حالة البناء | `npm run build` — PASS ✅ |
| عدد الشاشات | 5 شاشات كاملة |
| عدد الجداول | 4 (users, banks, parties, checks) |
| الأمان | مراجعة SecurityAgent — نظيف ✅ |
| المهام المنجزة | 16 مهمة (TASK-0000 إلى TASK-0015) |
| القضايا المفتوحة | 0 — جميع ISSUE-0000 إلى ISSUE-0008 مغلقة أو محلولة |

---

## 2. ما هو مشمول في التسليم

### 2.1 الشاشات

| الشاشة | المسار | الصلاحية المطلوبة | الوظيفة |
|---|---|---|---|
| تسجيل الدخول | `/login` | عام | تسجيل الدخول باسم المستخدم وكلمة المرور |
| الصفحة الرئيسية | `/` | مصادقة | لوحة تنقل بين جميع أقسام النظام |
| إدارة الشيكات | `/checks` | مصادقة (أي دور) | عرض، إضافة، تعديل، تغيير حالة، حذف (Admin)، طباعة |
| إدارة البنوك | `/banks` | ADMIN فقط | إضافة، تعديل، حذف البنوك المرجعية |
| إدارة الجهات | `/parties` | ADMIN فقط | إضافة، تعديل، حذف الجهات (أطراف الشيك) |
| إدارة المستخدمين | `/users` | ADMIN فقط | إضافة، تعديل، تفعيل/تعطيل المستخدمين |

### 2.2 الميزات الأساسية

- نظام مصادقة كامل (JWT cookies — 24 ساعة)
- دوران: ADMIN و USER
- دورة حياة كاملة للشيكات (5 حالات)
- شريط فلاتر متكامل (نوع، حالة، بنك، جهة، تاريخ، بحث نصي)
- بطاقات ملخص (عدد الشيكات، إجمالي المبالغ، مستحق قريبًا، مرتجعة)
- طباعة كشف الشيكات (Browser Print)
- حماية ADMIN في Middleware + Server Actions (Defense in Depth)
- منع الحذف للمرتبط بشيكات (Banks / Parties)
- منع حذف الشيكات في الحالات النهائية (مصرف / ملغي)
- منع تعطيل الحساب الخاص (Self-Protection)
- واجهة RTL كاملة بالعربية

### 2.3 الملفات المسلّمة

```
checks-management/
├── app/
│   ├── actions.ts              # getCurrentUser()
│   ├── layout.tsx              # RTL root layout
│   ├── page.tsx                # Navigation hub (4 cards)
│   ├── globals.css             # Base styles
│   ├── login/
│   │   ├── actions.ts          # login Server Action
│   │   └── page.tsx            # Login UI
│   ├── logout/
│   │   └── route.ts            # Logout route handler
│   ├── checks/
│   │   ├── actions.ts          # 9 Server Actions
│   │   └── page.tsx            # Full checks UI
│   ├── banks/
│   │   ├── actions.ts          # CRUD Server Actions
│   │   └── page.tsx            # Banks UI
│   ├── parties/
│   │   ├── actions.ts          # CRUD Server Actions
│   │   └── page.tsx            # Parties UI
│   └── users/
│       ├── actions.ts          # 5 Server Actions
│       └── page.tsx            # Users UI
├── lib/
│   ├── auth.ts                 # Auth helpers (hashPassword, verifyPassword, createToken, verifyToken, getSession, requireAdmin)
│   └── prisma.ts              # Prisma client singleton
├── prisma/
│   └── schema.prisma           # 4 models + enums
├── middleware.ts               # Route protection
├── .env.example                # Environment template
├── package.json                # Dependencies + scripts
├── tsconfig.json               # TypeScript config
├── next.config.ts              # Next.js config
└── ...config files
```

---

## 3. متطلبات النظام

### 3.1 المتطلبات الأساسية

| المكون | المتطلب |
|---|---|
| نظام التشغيل | Windows / macOS / Linux |
| Node.js | الإصدار 18.17 أو أحدث (موصى به 20+) |
| npm | الإصدار 9+ |
| PostgreSQL | الإصدار 14+ |
| المنفذ | 5600 (افتراضي — قابل للتغيير) |

### 3.2 المتطلبات الاختيارية

- `tsx` — لتشغيل seed script (مثبت كـ devDependency)
- متصفح حديث (Chrome, Firefox, Edge)

---

## 4. تعليمات الإعداد والتشغيل (Setup Instructions)

### 4.1 الخطوة 1 — التأكد من وجود PostgreSQL

تأكد من أن PostgreSQL مثبت وقيد التشغيل على جهازك المحلي:

```bash
# التحقق من PostgreSQL
psql --version
```

إذا لم يكن PostgreSQL مثبتًا، قم بتثبيته أولاً.

### 4.2 الخطوة 2 — إنشاء قاعدة البيانات

```bash
# اتصال بـ PostgreSQL وإنشاء قاعدة البيانات
psql -U postgres
CREATE DATABASE checks_management;
\q
```

### 4.3 الخطوة 3 — إعداد متغيرات البيئة

انسخ ملف `.env.example` إلى `.env`:

```bash
cd checks-management
copy .env.example .env
```

عدّل ملف `.env` بقيمتك المحلية:

```
DATABASE_URL="postgresql://postgres:[REDACTED]@localhost:5432/checks_management"
JWT_SECRET="[REDACTED]"
ADMIN_USERNAME="admin"
ADMIN_SEED_PASSWORD="[REDACTED]"
```

> **ملاحظة أمان:** استبدل `[REDACTED]` بالقيم الفعلية. احفظ `JWT_SECRET` في مكان آمن. لا تستخدم القيم الافتراضية في الإنتاج.

### 4.4 الخطوة 4 — تثبيت الاعتماديات

```bash
npm install
```

### 4.5 الخطوة 5 — تشغيل Prisma Migration

```bash
npx prisma migrate dev
```

هذا الأمر سينشئ الجداول الأربعة في قاعدة البيانات.

### 4.6 الخطوة 6 — تشغيل Seed (إنشاء المستخدم الإداري الأول)

```bash
npx prisma db seed
```
أو مباشرة:
```bash
npx tsx prisma/seed.ts
```

### 4.7 الخطوة 7 — بناء المشروع (اختياري للتحقق)

```bash
npm run build
```

### 4.8 الخطوة 8 — تشغيل التطبيق

```bash
# وضع التطوير
npm run dev          # يعمل على http://localhost:5600

# أو وضع الإنتاج
npm run build
npm start            # يعمل على http://localhost:3000 (افتراضي)
```

---

## 5. بيانات الدخول الافتراضية

| الحقل | القيمة الافتراضية |
|---|---|
| اسم المستخدم | القيمة من `ADMIN_USERNAME` في `.env` (افتراضيًا: `admin`) |
| كلمة المرور | القيمة من `ADMIN_SEED_PASSWORD` في `.env` |

> ⚠️ **يجب تغيير كلمة المرور الافتراضية فورًا بعد أول تسجيل دخول.**

---

## 6. متغيرات البيئة (Environment Variables)

| المتغير | إلزامي؟ | الوصف |
|---|---|---|
| `DATABASE_URL` | نعم | رابط اتصال PostgreSQL (مثال: `postgresql://user:password@localhost:5432/checks_management`) |
| `JWT_SECRET` | نعم | مفتاح توقيع JWT (نص آمن عشوائي) |
| `ADMIN_USERNAME` | نعم (للـ Seed) | اسم المستخدم الإداري الأول |
| `ADMIN_SEED_PASSWORD` | نعم (للـ Seed) | كلمة مرور المستخدم الإداري الأول |

---

## 7. أوامر التشغيل السريعة

| الأمر | الوصف |
|---|---|
| `npm run dev` | تشغيل في وضع التطوير (http://localhost:5600) |
| `npm run build` | بناء المشروع للإنتاج |
| `npm start` | تشغيل في وضع الإنتاج |
| `npx prisma migrate dev` | تطبيق التغييرات على قاعدة البيانات |
| `npx prisma db seed` | إدخال بيانات البداية (Admin user) |
| `npx prisma studio` | فتح واجهة Prisma Studio لإدارة البيانات |
| `npm run lint` | فحص الكود (ESLint) |

---

## 8. بنية المشروع (Project Structure)

```
checks-management/
├── app/                        # App Router Pages + Server Actions
│   ├── actions.ts              # مشترك: getCurrentUser
│   ├── layout.tsx              # Root layout مع RTL
│   ├── page.tsx                # الصفحة الرئيسية (Navigation Hub)
│   ├── globals.css             # أنماط أساسية
│   ├── login/                  # شاشة تسجيل الدخول
│   ├── logout/                 # معالجة تسجيل الخروج
│   ├── checks/                 # شاشة إدارة الشيكات
│   ├── banks/                  # شاشة إدارة البنوك
│   ├── parties/                # شاشة إدارة الجهات
│   └── users/                  # شاشة إدارة المستخدمين
├── lib/                        # مكتبات مساعدة
│   ├── auth.ts                 # دوال المصادقة
│   └── prisma.ts               # اتصال Prisma
├── prisma/
│   └── schema.prisma           # نموذج البيانات
├── middleware.ts               # حماية المسارات
├── .env.example                # قالب متغيرات البيئة
├── package.json
├── tsconfig.json
└── next.config.ts
```

---

## 9. القيود المعروفة / العناصر المؤجلة (Known Limitations / Deferred Items)

### 9.1 غير موجود في MVP (مؤجل لمرحلة لاحقة)

| العنصر | السبب |
|---|---|
| سجل تاريخ الحالات (Status History / Audit Log) | تحسين مستقبلي — MVP يكتفي بالحالة الحالية وتاريخ آخر تغيير |
| إشعارات (Email / SMS / WhatsApp) | غير مطلوبة في النطاق الحالي |
| لوحة قيادة (Dashboard) متقدمة | بطاقات الملخص الحالية كافية لـ MVP |
| تصدير Excel | غير مطلوب حاليًا |
| استيراد بيانات | غير مطلوب حاليًا |
| API خارجي / تكاملات | خارج نطاق MVP |
| صلاحيات متقدمة | دوران ADMIN و USER كافيان حاليًا |
| إدارة فروع / شركات متعددة | لا حاجة حاليًا |
| تسجيل مستخدم ذاتي | فقط Admin هو من يضيف المستخدمين |
| OAuth / 2FA | خارج نطاق MVP |
| Docker / CI/CD | لم تبدأ مرحلة النشر بعد |
| Status History | Anti-Bloat قرار من المعمارية — يمكن إضافته لاحقًا (جدول `check_status_logs`) |

### 9.2 ملاحظات تشغيلية

- **الطباعة:** تستخدم `window.print()` — تظهر بيانات الشيكات في جدول طباعة مخصص مع إخفاء الأزرار.
- **حذف البنوك والجهات:** ممنوع إذا كان هناك شيكات مرتبطة بها.
- **حذف الشيكات:** ممنوع للحالات النهائية (CASHED / CANCELLED). متاح فقط لـ ADMIN.
- **تعديل الشيك:** ممنوع للحالات النهائية.
- **تعطيل المستخدم:** ممنوع تعطيل الحساب الخاص. يمكن لـ Admin تفعيل/تعطيل أي مستخدم آخر.
- **تغيير صلاحية ADMIN إلى USER:** ممنوع على حسابك الخاص.

---

## 10. ملاحظات الدعم

- التطبيق لا يتطلب اتصال إنترنت للتشغيل المحلي (باستثناء تثبيت الحزم).
- جميع الأنماط (styles) داخلية (Inline Styles) — لا حاجة لـ Tailwind أو CSS Libraries.
- لا توجد واجهة API منفصلة — كل العمليات عبر Server Actions.
- للترقية إلى Status History: يمكن إنشاء جدول `check_status_logs` مع FK إلى `checks` وربطه في `updateCheckStatus`.
- للنشر على الإنتاج: قم بتعيين `JWT_SECRET` لقيمة آمنة قوية، وتأكد من استخدام `secure: true` للكوكيز عبر HTTPS.

---

## 11. قائمة الفحوصات النهائية (Go-Live Checklist)

- [ ] `DATABASE_URL` محدث بقيم حقيقية
- [ ] `JWT_SECRET` قوي وعشوائي
- [ ] `ADMIN_SEED_PASSWORD` تم تغييره
- [ ] `npm run build` يمر بنجاح
- [ ] `npx prisma migrate dev` يعمل بشكل صحيح
- [ ] `npx prisma db seed` أضاف المستخدم الإداري
- [ ] يمكن تسجيل الدخول باسم المستخدم الإداري
- [ ] جميع الشاشات الخمس تعمل
- [ ] الـ Middleware يحمي المسارات الإدارية
- [ ] النسخة الاحتياطية من قاعدة البيانات موجودة

---

*تم إعداد هذا المستند بواسطة DocumentationHandoverAgent — 2026-06-28*
