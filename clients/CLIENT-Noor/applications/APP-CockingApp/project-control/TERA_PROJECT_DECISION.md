# TERA_PROJECT_DECISION.md — CockingApp

> **Status:** _Approved_
> **Date:** 2026-06-30
> **Decision Type:** Project Decision Formation (Phase 2)

---

## 1. Application Identity

| Field | Value |
|---|---|
| **Application Name** | CockingApp |
| **Client / Owner** | نور |
| **Application Workspace** | `clients/CLIENT-Noor/applications/APP-CockingApp/` |
| **Project Size** | Medium (Web App + Admin Dashboard) |
| **Current Phase** | Phase 2 — Project Decision Formation |

---

## 2. Project Summary

تطبيق ويب متكامل لإدارة وعرض وصفات الطبخ مع:
- لوحة تحكم Admin قوية
- نظام مكونات منفصل (Ingredient Management)
- دعم الصور والفيديو (YouTube) والتعليقات
- تحميل PDF للوصفات
- واجهة عرض عامة أنيقة

تم اعتماد مقترح التطبيق (Application Proposal) من العميل نور بتاريخ 30-06-2026.

---

## 3. Technology Decision

| Item | Decision |
|---|---|
| **Technology Profile** | `nextjs-prisma` |
| **Framework** | Next.js (App Router) |
| **Language** | TypeScript |
| **Database** | PostgreSQL |
| **ORM** | Prisma |
| **UI Framework** | Tailwind CSS |
| **Design System** | Claude Design System (getdesign.md) — Cream + Coral + Dark Navy |
| **Auth** | Admin only (basic auth / NextAuth.js) |
| **Deployment** | On-premise |
| **Language** | Arabic (RTL) |

---

## 4. Scope Confirmation

تم اعتماد النطاق التالي من قبل العميل:

| المرحلة | المحتوى |
|---------|---------|
| **Phase 1A — Core MVP** | 10 عناصر أساسية (لوحة تحكم، تصنيفات، مكونات، وصفات، صور، فيديو، خطوات، عرض عام، تفاصيل، إدارة تعليقات) |
| **Phase 1B — Extended MVP** | 6 عناصر (PDF، تعليقات عامة، قائمة مشتريات، مقياس وصفات، وقت تحضير، مفضلة) |
| **Phase 2** | 7 عناصر (معلومات غذائية، خطة وجبات، بحث بالمكونات، تقييم، تقليل هدر، استيراد، أوفلاين) |
| **Phase 3+** | متقدم — AI، بدائل، ميزانية، جوال، مشاركة، صوتي |
| **Out of Scope** | 6 عناصر (أجهزة ذكية، متجر، اشتراكات، TikTok، أدوار متعددة) |

---

## 5. Approved Suggestions from Market Research

تم اعتماد 8 اقتراحات تحسينية مبنية على أبحاث السوق:

| الاقتراح | المصدر | المرحلة |
|---------|--------|---------|
| 🛒 قائمة مشتريات ذكية | Samsung Food, Cookpad, سمسم | Phase 1B |
| 📐 مقياس الوصفات الديناميكي | Paprika, Mealime | Phase 1B |
| ⏱️ وقت التحضير + بحث | إجماع السوق | Phase 1B |
| ❤️ مفضلة للحفظ | Paprika, Cookpad | Phase 1B |
| 📊 معلومات غذائية | Samsung Food, EatThisMuch | Phase 2 |
| 📅 خطة وجبات أسبوعية | Mealime, سمسم | Phase 2 |
| 🔍 بحث بالمكونات المتوفرة | SuperCook, Yummly | Phase 2 |
| ⭐ نظام تقييم ونجوم | Cookpad | Phase 2 |

---

## 6. Active Technology Profile

Technology Profile: **nextjs-prisma**

The profile `tera-system/profiles/nextjs-prisma.md` will be activated before Phase 5 (Execution Planning).

---

## 7. Next Phase

| Next Phase | Action |
|---|---|
| **Phase 3 — Preparation Planning** | Create `PREPARATION_PLAN.md` — planning only, no file creation |

---

## 8. Decision History

| Date | Decision | Authorized By |
|---|---|---|
| 2026-06-30 | اعتماد Application Proposal بالمحتوى الكامل | العميل نور |
| 2026-06-30 | اعتماد تقنية Next.js + Prisma | ماجد |
| 2026-06-30 | اعتماد الاقتراحات التحسينية | العميل نور (عبر ماجد) |
