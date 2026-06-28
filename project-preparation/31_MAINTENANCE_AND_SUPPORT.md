# 31_MAINTENANCE_AND_SUPPORT.md

## دليل الصيانة والدعم — نظام إدارة الشيكات (Checks Management MVP)

---

## جدول المحتويات

1. [مقدمة](#1-مقدمة)
2. [كيفية تحديث التطبيق](#2-كيفية-تحديث-التطبيق)
3. [النسخ الاحتياطي لقاعدة البيانات (Backup)](#3-النسخ-الاحتياطي-لقاعدة-البيانات-backup)
4. [المشاكل الشائعة والحلول](#4-المشاكل-الشائعة-والحلول)
5. [كيفية إضافة مستخدم جديد](#5-كيفية-إضافة-مستخدم-جديد)
6. [المراقبة والسجلات (Monitoring & Logs)](#6-المراقبة-والسجلات-monitoring--logs)
7. [مسارات الترقية المستقبلية](#7-مسارات-الترقية-المستقبلية)
8. [معلومات الاتصال والدعم](#8-معلومات-الاتصال-والدعم)

---

## 1. مقدمة

هذا المستند موجه لمسؤولي النظام (System Administrators) والمطورين المسؤولين عن صيانة ودعم تطبيق **نظام إدارة الشيكات** بعد التسليم.

### الجمهور المستهدف

- مطور (Developer) — للصيانة التقنية والتحديثات
- مسؤول نظام (SysAdmin) — لإدارة قواعد البيانات والنسخ الاحتياطي
- مدير النظام (Admin User) — لإدارة المستخدمين والإعدادات اليومية

---

## 2. كيفية تحديث التطبيق

### 2.1 تحديث الكود (Code Update)

عند استلام تحديث كود جديد، اتبع الخطوات التالية:

```bash
# 1. الانتقال إلى مجلد المشروع
cd checks-management

# 2. سحب آخر التغييرات (إذا كان المشروع مرتبط بمستودع)
git pull origin main

# 3. تثبيت أي اعتماديات جديدة
npm install

# 4. تطبيق أي تغييرات على قاعدة البيانات
npx prisma migrate dev

# 5. بناء المشروع (للتحقق من عدم وجود أخطاء)
npm run build

# 6. إعادة تشغيل التطبيق
npm run dev    # للتطوير
# أو
npm start      # للإنتاج
```

### 2.2 تحديث قاعدة البيانات (Database Migration)

عند إضافة أو تعديل جداول قاعدة البيانات:

```bash
# إنشاء migration جديدة
npx prisma migrate dev --name description-of-change

# لتطبيق migration في الإنتاج
npx prisma migrate deploy
```

### 2.3 تحديث البيانات الأولية (Seed)

إذا تم تعديل ملف `prisma/seed.ts`:

```bash
npx prisma db seed
```

> ⚠️ الـ Seed يستخدم `upsert` — لن يقوم بإنشاء مستخدم مكرر، لكنه سيحدّث كلمة المرور إذا تغيرت.

---

## 3. النسخ الاحتياطي لقاعدة البيانات (Backup)

### 3.1 أهمية النسخ الاحتياطي

النسخ الاحتياطي المنتظم يحمي بياناتك من:
- أعطال الأجهزة
- أخطاء المستخدمين (حذف غير مقصود)
- تحديثات خاطئة
- هجمات برمجية

### 3.2 طريقة عمل نسخة احتياطية

**يدويًا:**

```bash
# النسخ الاحتياطي الكامل لقاعدة البيانات checks_management
pg_dump -U postgres checks_management > backup_YYYY-MM-DD.sql

# استرجاع من نسخة احتياطية
psql -U postgres -d checks_management < backup_YYYY-MM-DD.sql
```

### 3.3 خطة النسخ الاحتياطي المقترحة

| التكرار | النوع | الاحتفاظ |
|---|---|---|
| يوميًا | نسخة كاملة (Full Backup) | 7 أيام |
| أسبوعيًا | نسخة كاملة | 4 أسابيع |
| شهريًا | نسخة كاملة | 12 شهرًا |

### 3.4 نصائح للنسخ الاحتياطي

- احفظ النسخ في مكان مختلف عن السيرفر (مثل قرص خارجي، تخزين سحابي).
- اختبر استرجاع النسخة مرة واحدة على الأقل شهريًا للتأكد من سلامتها.
- أتمتة العملية باستخدام Windows Task Scheduler أو cron (Linux/macOS).
- لا تنسَ نسخ ملف `.env` — يحتوي على `JWT_SECRET` وإعدادات الاتصال.

### 3.5 أتمتة النسخ الاحتياطي (Windows — Scheduled Task)

```powershell
# مثال لسكريبت PowerShell للنسخ الاحتياطي
$date = Get-Date -Format "yyyy-MM-dd"
$backupDir = "D:\Backups\ChecksManagement"
$backupFile = "$backupDir\backup_$date.sql"

# تأكد من وجود المجلد
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir
}

# تنفيذ النسخ الاحتياطي
& "C:\Program Files\PostgreSQL\17\bin\pg_dump.exe" -U postgres checks_management > $backupFile

# حذف النسخ الأقدم من 7 أيام
Get-ChildItem $backupDir -Filter *.sql | Where-Object {
    $_.LastWriteTime -lt (Get-Date).AddDays(-7)
} | Remove-Item
```

---

## 4. المشاكل الشائعة والحلول

### 4.1 المشكلة: خطأ عند تشغيل `npm run dev`

| الخطأ | السبب المحتمل | الحل |
|---|---|---|
| `'next' is not recognized` | لم يتم تثبيت الاعتماديات | نفّذ `npm install` |
| `Error: Cannot find module 'next'` | مجلد `node_modules` تالف | احذف `node_modules` ثم `npm install` |
| `port 5600 is already in use` | المنفذ مشغول بتطبيق آخر | أنهِ العملية الأخرى أو غيّر المنفذ في `package.json` |

### 4.2 المشكلة: خطأ في الاتصال بقاعدة البيانات

| الخطأ | السبب المحتمل | الحل |
|---|---|---|
| `Can't reach database server` | PostgreSQL غير شغال | ابدأ خدمة PostgreSQL عبر `net start postgresql-17` أو من Services |
| `role "postgres" does not exist` | المستخدم غير موجود | أنشئ المستخدم: `CREATE ROLE postgres LOGIN SUPERUSER;` |
| `database "checks_management" does not exist` | قاعدة البيانات غير موجودة | أنشئها: `CREATE DATABASE checks_management;` |
| `password authentication failed` | كلمة مرور خاطئة | تحقق من `DATABASE_URL` في ملف `.env` |

### 4.3 المشكلة: خطأ في Prisma Migration

| الخطأ | السبب المحتمل | الحل |
|---|---|---|
| `The migration was not applied` | قاعدة البيانات غير محدثة | نفّذ `npx prisma migrate deploy` |
| `Error: P1001: Can't reach database` | قاعدة البيانات غير متصلة | تحقق من أن PostgreSQL يعمل |
| `Error: P2002: Unique constraint failed` | بيانات مكررة في الجدول | تحقق من البيانات الموجودة وأزل المكرر يدويًا |

### 4.4 المشكلة: مشاكل في تسجيل الدخول

| المشكلة | السبب المحتمل | الحل |
|---|---|---|
| "اسم المستخدم أو كلمة المرور غير صحيحة" | بيانات غير صحيحة أو الحساب مُعطّل | تحقق من بيانات المستخدم في قاعدة البيانات |
| خطأ `JWT_SECRET is not set` | متغير البيئة مفقود | تأكد من وجود `JWT_SECRET` في `.env` |
| يتم إعادة التوجيه إلى `/login` بعد الدخول | انتهت صلاحية الـ Cookie | سجل دخول مرة أخرى. الجلسة تنتهي بعد 24 ساعة. |

للتحقق من حالة المستخدم في قاعدة البيانات:

```bash
npx prisma studio
```

ثم اذهب إلى جدول `users` وتحقق من أن `is_active` هو `true`.

### 4.5 المشكلة: Build فاشل

```bash
npm run build
```

إذا فشل البناء، ابحث عن الخطأ المحدد في المخرجات. الأسباب الشائعة:

- خطأ TypeScript في أحد الملفات — أصلح نوع المتغير.
- مكتبة مفقودة — نفّذ `npm install`.
- مشكلة في `next.config.ts` — تحقق من الإعدادات.

### 4.6 المشكلة: الشاشة لا تظهر أو تظهر فارغة

- **تحقق من Console في المتصفح** (F12 → Console) — ابحث عن أخطاء JavaScript.
- **تحقق من Network tab** (F12 → Network) — تأكد من أن الطلبات تعود بنجاح.
- **حاول مسح Cache** (Ctrl+Shift+R) أو فتح التطبيق في نافذة تصفح خاصة (Incognito).

---

## 5. كيفية إضافة مستخدم جديد

### الطريقة 1: عبر واجهة المستخدم (الأسهل)

1. سجل الدخول كمستخدم ADMIN.
2. اذهب إلى شاشة **المستخدمين** من الصفحة الرئيسية.
3. اضغط على **"إضافة مستخدم جديد"**.
4. أدخل البيانات المطلوبة واضغط حفظ.

### الطريقة 2: عبر Prisma Studio

```bash
npx prisma studio
```

- اذهب إلى جدول `users`.
- اضغط على **Add Record**.
- أدخل `username` (فريد)، `password_hash` (اخرج من bcrypt)، `display_name`، `role` (ADMIN أو USER).
- اضغط **Save Changes**.

### الطريقة 3: عبر سكريبت مباشر

```bash
# الاتصال بقاعدة البيانات
psql -U postgres -d checks_management

# إضافة مستخدم جديد (كلمة المرور: 123456 — قم بتغييرها فورًا)
INSERT INTO users (username, password_hash, display_name, role, is_active, created_at, updated_at)
VALUES ('newuser', '$2a$10$...[hash]...', 'مستخدم جديد', 'USER', true, NOW(), NOW());
```

لتوليد `hash` لكلمة المرور، يمكنك استخدام:

```bash
node -e "const bcrypt = require('bcryptjs'); bcrypt.hash('yourpassword', 10).then(console.log)"
```

> ⚠️ **تأكد من تغيير كلمة المرور الافتراضية فورًا بعد إنشاء الحساب.**

---

## 6. المراقبة والسجلات (Monitoring & Logs)

### 6.1 سجلات التطبيق

التطبيق الحالي لا يحتوي على سجل مركزي (Audit Log). العمليات المتاحة للمراقبة:

| المصدر | ما يمكن رؤيته |
|---|---|
| Console التطبيق (Terminal) | أخطاء Prisma، أخطاء الاتصال، طلبات HTTP |
| متصفح المستخدم (F12 Console) | أخطاء JavaScript، فشل Server Actions |
| سجلات PostgreSQL | استعلامات قاعدة البيانات (يمكن تفعيلها) |

### 6.2 تفعيل سجلات Prisma

أضف في ملف `.env`:

```
# تفعيل سجلات الاستعلامات
# يمكن إضافته في lib/prisma.ts: log: ['query', 'info', 'warn', 'error']
```

لتعديل `lib/prisma.ts`:

```typescript
const adapter = new PrismaPg(process.env.DATABASE_URL!)
export const prisma = globalForPrisma.prisma ?? new PrismaClient({
  adapter,
  log: process.env.NODE_ENV === 'development' ? ['query', 'info', 'warn', 'error'] : ['error'],
})
```

### 6.3 مراقبة صحة التطبيق

- **تحقق من أن التطبيق يستجيب:** افتح `http://localhost:5600/login` في المتصفح.
- **تحقق من قاعدة البيانات:** استخدم `npx prisma studio` أو `psql`.
- **تحقق من الذاكرة:** استخدم Task Manager (Windows) أو `htop` (Linux).

### 6.4 سجلات الأعطال

سجلات Next.js تظهر في الـ Terminal الذي يعمل فيه التطبيق. في بيئة الإنتاج، يُنصح باستخدام:

- **PM2** (لإدارة عملية Node.js والإبقاء عليها قيد التشغيل)
- **يمكن إعادة توجيه السجلات** إلى ملف:

```bash
npm start > app.log 2>&1
```

---

## 7. مسارات الترقية المستقبلية

### 7.1 تحسينات مقترحة للمرحلة القادمة

| الأولوية | التحسين | التأثير | الجهد التقريبي |
|---|---|---|---|
| عالية | سجل تاريخ الحالات (Status History) | تدقيق ومتابعة دقيقة | متوسط |
| عالية | سجل النشاطات (Audit Log) | أمان ومسؤولية | متوسط |
| متوسطة | تصدير Excel | تقارير خارجية | متوسط |
| متوسطة | ترحيل إلى Tailwind CSS | صيانة أسهل للواجهة | كبير |
| متوسطة | إشعارات (Email) | تذكير بالاستحقاقات | كبير |
| منخفضة | OAuth / SSO | سهولة الدخول | كبير |
| منخفضة | API خارجي (REST) | تكامل مع أنظمة أخرى | كبير |
| منخفضة | تطبيق موبايل | وصول من الجوال | كبير جدًا |

### 7.2 كيف تضيف Status History

1. أضف موديل جديد في `prisma/schema.prisma`:

```prisma
model CheckStatusLog {
  id        Int         @id @default(autoincrement())
  checkId   Int         @map("check_id")
  fromStatus CheckStatus? @map("from_status")
  toStatus  CheckStatus @map("to_status")
  changedBy Int         @map("changed_by")
  note      String?
  createdAt DateTime    @default(now()) @map("created_at")

  check     Check       @relation(fields: [checkId], references: [id])
  changedByUser User   @relation(fields: [changedBy], references: [id])

  @@map("check_status_logs")
}
```

2. نفّذ migration:

```bash
npx prisma migrate dev --name add_status_history
```

3. عدّل `updateCheckStatus` في `app/checks/actions.ts` لكتابة سجل عند تغيير الحالة.

### 7.3 كيف تضيف تصدير Excel

استخدم مكتبة مثل `exceljs` أو `xlsx`:

```bash
npm install exceljs
```

أضف Server Action جديدة في `app/checks/actions.ts` لتصدير الشيكات إلى Excel وإرجاع الملف كـ `Blob`.

### 7.4 كيف تضيف إشعارات Email

1. استخدم `nodemailer` (مجاني، محلي) أو خدمة مثل SendGrid / AWS SES.
2. أضف جدول `notifications` لتتبع الإشعارات المرسلة.
3. أضف وظيفة مجدولة (Cron Job) لفحص الشيكات المستحقة قريبًا وإرسال تنبيهات.

---

## 8. معلومات الاتصال والدعم

### 8.1 قنوات الدعم

| النوع | الطريقة |
|---|---|
| الإبلاغ عن مشكلة | توثيق المشكلة مع خطوات إعادة الإنتاج |
| طلب تحسين | توثيق الطلب مع وصف الفائدة المتوقعة |
| استفسار تقني | التواصل مع المطور المسؤول |

### 8.2 المعلومات المطلوبة عند الإبلاغ عن مشكلة

1. وصف المشكلة (ماذا يحدث؟ ماذا تتوقع أن يحدث؟)
2. خطوات إعادة إنتاج المشكلة
3. لقطة شاشة (إن أمكن)
4. رسالة الخطأ من Console المتصفح (F12 → Console)
5. محتوى ملف `app.log` (إن وجد)
6. إصدار Node.js (`node --version`)
7. إصدار PostgreSQL (`psql --version`)

### 8.3 قائمة المراجع التقنية

| المورد | الرابط / الوصف |
|---|---|
| Next.js Documentation | https://nextjs.org/docs |
| Prisma Documentation | https://www.prisma.io/docs |
| PostgreSQL Documentation | https://www.postgresql.org/docs/ |
| bcryptjs | https://www.npmjs.com/package/bcryptjs |
| jose (JWT) | https://www.npmjs.com/package/jose |

---

## الملحق أ: الأوامر السريعة

| الأمر | الوصف |
|---|---|
| `npm run dev` | تشغيل التطوير (منفذ 5600) |
| `npm run build` | بناء الإنتاج |
| `npm start` | تشغيل الإنتاج |
| `npx prisma migrate dev` | تطبيق migrations |
| `npx prisma migrate deploy` | تطبيق migrations (إنتاج) |
| `npx prisma db seed` | تشغيل seed |
| `npx prisma studio` | فتح واجهة Prisma |
| `npx prisma validate` | التحقق من صحة schema |

## الملحق ب: متغيرات البيئة المطلوبة

```env
# يجب أن تكون جميعها موجودة في ملف .env
DATABASE_URL="postgresql://user:password@localhost:5432/checks_management"
JWT_SECRET="[REDACTED]"
ADMIN_USERNAME="admin"
ADMIN_SEED_PASSWORD="[REDACTED]"
```

---

*تم إعداد هذا المستند بواسطة DocumentationHandoverAgent — 2026-06-28*
