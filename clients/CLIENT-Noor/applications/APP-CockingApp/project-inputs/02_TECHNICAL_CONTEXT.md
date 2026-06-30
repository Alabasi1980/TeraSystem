# Technical Context — CockingApp

> **Intake Status:** _Partial_
> **Last Updated:** 2026-06-30

---

## 1. Technology Stack

| Item | Decision / Notes |
|---|---|
| Programming Language | TypeScript |
| Framework | Next.js (App Router) |
| Application Type | Web App (SSR + SPA hybrid) |
| Database | PostgreSQL (Prisma ORM) |
| ORM / Data Access | Prisma |
| Package Manager | npm |
| UI Framework / Library | Tailwind CSS + Claude design system |

---

## 2. External Integrations

- تحميل PDF — `react-pdf` أو `pdf-lib` أو `@react-pdf/renderer`
- فيديو — رفع واستضافة فيديو (YouTube embed أو رفع محلي)
- صور — رفع صور (حل تخزين محلي أو CDN)

---

## 3. Forbidden Libraries or Technologies

_لم يحدد بعد._

---

## 4. Deployment & Hosting

- Runtime environment: On-premise (على سيرفر العميل)
- Hosting notes: لم يحدد نوع السيرفر بعد
- Docker required: _Not decided_

---

## 5. Technical & Security Constraints

- Auth: لوحة تحكم Admin فقط (حماية بكلمة مرور)
- البيانات: وصفات، مكونات، صور، فيديو — غير حساسة
- لا يحتاج SSL معقد (on-premise)
- التعليقات عامة بدون حساب (قد تحتاج فلترة ضد SPAM)

---

## 6. Technology Profile Candidate

- ✅ **Next.js + Prisma** — approved by Majed
- Ter akan mengaktifkan profile `nextjs-prisma` dari `tera-system/profiles/`

---

## 7. Notes

- RTL (Arabic) هو اللغة الافتراضية
- التصميم: Claude design system (cream + coral + dark navy)
