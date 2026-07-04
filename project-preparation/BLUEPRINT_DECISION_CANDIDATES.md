---
document_type: blueprint_decision_candidates
client_name: "مؤسسة الفارس للصيانة"
application_name: "نظام إدارة طلبات الصيانة الداخلي"
version: "1.0"
date: "2026-07-04"
language: "ar"
direction: "rtl"
prepared_by: "ApplicationBlueprintAgent"
---

# BLUEPRINT_DECISION_CANDIDATES — مرشحات القرارات التقنية

> ⚠️ **تنبيه مهم:** هذا الملف يحتوي على **مرشحات وتوصيات فقط** — وليس قرارات نهائية.
>
> جميع العناصر هنا تُكتب كـ `candidates / recommendations / tradeoff options`.
>
> **القرارات التقنية النهائية تُتخذ في مرحلة التحضير الرسمي بواسطة TeraAgent و SolutionArchitectureAgent وباعتماد Majed.**

---

## 1. Backend — لغة البرمجة والإطار

| المعيار | الأفضلية |
|---------|:--------:|
| سرعة التطوير للمشاريع الصغيرة | **Node.js + TypeScript** |
| الأمان والقوة للمشاريع الداخلية على Windows | **C# + ASP.NET Core** |
| البساطة للنماذج الأولية | **Python + FastAPI** |

### 🏆 التوصية المبدئية: Node.js + TypeScript مع Express أو NestJS

**المبررات:**
- مناسب لحجم التطبيق (7 شاشات، 4 وحدات)
- مجتمع كبير ودعم ممتاز
- أداء جيد لتطبيق بـ ~30 مستخدم
- TypeScript يمنح أماناً إضافياً بالأنواع

**البديل المنطقي:** C# + ASP.NET Core — إذا كانت بيئة العميل تعتمد على Windows وتفضّل توحيد المنصة.

---

## 2. Frontend — إطار الواجهة

| المعيار | الأفضلية |
|---------|:--------:|
| دعم RTL واللغة العربية | **React / Next.js** |
| البساطة وسرعة التعلم | **Vue.js** |
| التوحيد مع Backend | **Blazor (مع C#)** |

### 🏆 التوصية المبدئية: React + Next.js (App Router)

**المبررات:**
- دعم ممتاز للـ RTL مع Tailwind CSS
- App Router يوفر تنظيم ملفات جيد
- يمكن أن يعمل كـ Full-stack (إذا استخدمت Next.js API routes)
- shadcn/ui متوفر لمكونات جاهزة عربية

**البديل المنطقي:** Vue.js + Nuxt — إذا فضّل الفريق البساطة.

---

## 3. قاعدة البيانات

| المعيار | الأفضلية |
|---------|:--------:|
| توازن بين القوة والبساطة | **PostgreSQL** |
| أبسط نشر (بدون خادم) | **SQLite** |
| التكامل مع Windows/.NET | **SQL Server Express** |

### 🏆 التوصية المبدئية: PostgreSQL

**المبررات:**
- مناسب لـ 4 كيانات مع علاقات
- أداء جيد مع 25-30 مستخدم
- مجاني ومفتوح المصدر
- يدعم JSON إذا احتيج للتوسع لاحقاً

**اعتبارات:**
- يحتاج خادم قاعدة بيانات (سيرفر داخلي أو VPS)
- إذا كانت الاستضافة محلية على جهاز واحد فقط، فقد يكون SQLite كافياً

---

## 4. الاستضافة

| المعيار | الأفضلية |
|---------|:--------:|
| تحكم كامل + لا تكاليف شهرية | **On-Premise (محلية)** |
| سهولة الإدارة + توفر عال | **VPS سحابي (Hetzner / DigitalOcean)** |

### 🏆 التوصية المبدئية: On-Premise (محلية داخل شبكة العميل)

**المبررات:**
- متوافقة مع افتراضات حزمة التسليم
- لا تكاليف استضافة شهرية
- مناسبة لعدد محدود من المستخدمين

**البديل المنطقي:** VPS صغير (~5-15$/شهر) إذا لم تتوفر بيئة داخلية مناسبة.

---

## 5. نمط المصادقة

| المرشح | المزايا | العيوب |
|--------|---------|--------|
| **Session-based Auth** | أبسط، أكثر أماناً للتطبيقات الداخلية | غير مناسب لواجهات API العامة |
| **JWT-based Auth** | مرن، عديم الحالة (Stateless) | إدارة التوكنز والتجديد أكثر تعقيداً |

### 🏆 التوصية المبدئية: Session-based Auth + bcrypt لكلمات المرور

**المبررات:**
- تطبيق داخلي بمستخدمين محدودين — لا حاجة لـ JWT
- إدارة جلسات أبسط
- أمان كافٍ لبيئة داخلية

---

## 6. UI Framework / مكتبة التصميم

| المرشح | المزايا |
|--------|---------|
| **Tailwind CSS** | تحكم كامل، خفيف، دعم RTL ممتاز |
| **shadcn/ui (مع Tailwind)** | مكونات جاهزة قابلة للتخصيص |
| **Bootstrap 5 + RTL** | مكونات كثيرة، دعم RTL مدمج |

### 🏆 التوصية المبدئية: Tailwind CSS + shadcn/ui

**المبررات:**
- مكونات جاهزة عربية مع دعم RTL
- مرونة عالية في التصميم
- يمكن استخدام الهوية البصرية للعميل

---

## 7. ORM / الوصول إلى البيانات

| المرشح | المزايا |
|--------|---------|
| **Prisma (TypeScript)** | • Type-safe<br>• Migrations مدمجة<br>• سهل الاستخدام |
| **Drizzle ORM** | • أداء أعلى<br>• أقرب إلى SQL النقي |
| **Entity Framework Core (C#)** | • قوي ومتكامل مع .NET<br>• مناسب للمشاريع المتوسطة |

### 🏆 التوصية المبدئية (مع Node.js): Prisma

---

## 8. ملخص توصيات Stack الكاملة

### Profile المقترح: `nextjs-prisma`

| الطبقة | المرشح | البديل |
|--------|--------|--------|
| Frontend | Next.js (App Router) | Vue.js + Nuxt |
| Backend | Next.js API Routes أو Express | ASP.NET Core |
| لغة البرمجة | TypeScript | C# |
| قاعدة البيانات | PostgreSQL | SQLite |
| ORM | Prisma | Drizzle ORM |
| UI Framework | Tailwind CSS + shadcn/ui | Bootstrap 5 |
| المصادقة | Session-based + bcrypt | JWT |
| الاستضافة | On-Premise (مبدئياً) | VPS سحابي |

> ⚠️ **ملاحظة:** هذا الـ Stack المقترح هو **توصية فقط** من ApplicationBlueprintAgent. القرار النهائي يُتخذ في مرحلة التحضير الرسمي (Project Preparation) بواسطة TeraAgent و SolutionArchitectureAgent، وباعتماد Majed.

---

## 9. جدول ملخص القرارات المطلوبة

| القرار | المرشح الموصى به | يحتاج موافقة |
|--------|:-----------------:|:-----------:|
| Backend Language | TypeScript / Node.js | TeraAgent + Majed |
| Frontend Framework | Next.js (React) | TeraAgent + Majed |
| Database | PostgreSQL | TeraAgent + Majed |
| Hosting | On-Premise | العميل + Majed |
| Auth Method | Session-based | TeraAgent |
| UI Library | Tailwind + shadcn/ui | TeraAgent |
| ORM | Prisma | TeraAgent |

---

*هذا الملف هو Blueprint Decision Candidates فقط. جميع العناصر فيه مرشحات وتوصيات، وليست قرارات نهائية.*
