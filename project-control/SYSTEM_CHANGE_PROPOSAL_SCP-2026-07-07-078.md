# SYSTEM_CHANGE_PROPOSAL

## SCP-2026-07-07-078 — إنشاء عميل "مهندس منفّذ" (EngineeringAgent) لربط التصميم بالكود الخلفي

---

### Request Type
New Agent Creation — Engineering Implementation Agent

### Requested By
TeraAgent — بعد تحليل سير عمل التصميم والتنفيذ

---

### Problem

تم إنشاء **UIDesignerAgent** (SCP-077) لحل مشكلة التنفيذ البصري، لكن تحليل سير العمل كشف عن **فجوة جديدة**:

سير العمل الحالي بعد SCP-077:

| الترتيب | العميل | المخرجات | هل يكفي وحده؟ |
|:-------:|--------|----------|:-------------:|
| 1 | Software Designer | Technical Spec (API, Data, Logic) | ❌ ما يكتب كود |
| 2 | UI Designer | UI Code (React/Tailwind — شكل فقط) | ❌ ما يربط API |
| 3 | DesignReviewer | تقرير مراجعة | ❌ ما ينفذ |
| **❓** | **EngineeringAgent ???** | **❌ مفقود** | |

### الثغرة:

```
Software Designer يكتب:
- API endpoints
- Data models
- Business rules
- Component tree
    ↓
UI Designer ينفّذ:
- شكل الصفحة (UI)
- ألوان وخطوط
- تصميم متجاوب
    ↓
❌ من يكتب:
- API calls (fetch, axios, HttpClient)
- State Management (Redux, Context, Zustand)
- Controllers, Services, Middleware
- Database queries (SQL, LINQ, EF Core)
- Authentication & Authorization logic
- Error handling & Loading states (الفعلي)
- Data binding (API response → UI component)
- Routing, Navigation, Guards
- Form validation & submission logic
- Caching, Pagination, Filtering (الفعلي)
```

### ببساطة:

> **Software Designer يقول "ماذا نبني".**
> **UI Designer يقول "كيف سيبدو".**
> **EngineeringAgent يقول "كيف سيعمل — ويكتب الكود الذي يربط الاثنين".**

---

### Solution

إنشاء عميل **EngineeringAgent (مهندس منفّذ)** — وهو عميل متخصص في:
- كتابة **كود التطبيق الفعلي** (وليس التصميم فقط)
- **ربط UI بالـ Backend** (API Integration)
- **إدارة الحالة** (State Management)
- **كتابة Business Logic** والخوارزميات
- **التعامل مع قواعد البيانات** (ORM, SQL)
- **التعامل مع Authentication, Authorization, Middleware**

---

### الفرق بينه وبين العملاء الموجودين

| Software Designer | UI Designer | EngineeringAgent (الجديد) |
|:-----------------:|:-----------:|:------------------------:|
| يكتب Tech Specs | يكتب شكل الواجهة | **يكتب كود الشغل** |
| يخطط ما يُبنى | يجمّل ما بُني | **يشغّل ما جُمّل** |
| ما يمس الكود | HTML + Tailwind فقط | **API, DB, Logic, State, Auth** |
| يقول "استخدم axios" | لا يعرف axios | **يكتب axios.call** |

---

### سير العمل المتوقع

```
1. Software Designer ← TECHNICAL_SPECIFICATION.md
    ↓
2. UI Designer ← UI Code (React + Tailwind)
    ↓
3. EngineeringAgent:
   - يقرأ Tech Spec ← يفهم API endpoints والبيانات
   - يقرأ UI Code ← يفهم المكونات التي تحتاج ربط
   - يربط API بالـ UI
   - يكتب Business Logic (if/else, validation, formatting)
   - يكتب State Management
   - يكتب Error/Empty/Loading حالات
   - يربط كل شيء في **تطبيق حي شغّال**
    ↓
4. TeraAgent ← موافقة
    ↓
5. DesignReviewer ← مراجعة المخرجات النهائية
```

---

### الأولوية

| البند | القيمة |
|-------|--------|
| **الأولوية** | متوسطة — لكن ضرورية لأي مشروع كامل |
| **التأثير** | يسد فجوة حرجة في سلسلة التطوير |
| **نوع المشاريع التي تحتاجه** | كل مشروع يحتاج كود خلفي (Business Logic, API, DB) |
| **المشاريع التي لا تحتاجه** | مشاريع UI فقط (Prototypes غير متصلة بـ API) |

---

### قرار TeraAgent

بصفتي مدير المبرمجين:

- في **مشروع Dashboard Builder** الحالي (مشروع العميل الجديد):
  - المرحلة الأولى: UI Prototype فقط ← **لا نحتاج EngineeringAgent** حالياً
  - المرحلة الثانية: الربط بقواعد البيانات (SQL, APIs) ← **سنحتاج EngineeringAgent**
- في **مشروع كامل** (.NET, Full Stack, إلخ): **EngineeringAgent أساسي**

> **العميل مهم أن يكون موجوداً — حتى لو لم نستخدمه في كل مشروع.**

---

### ملفات التأثير

1. **جديد:** `.opencode/agents/engineering-agent.md` — الملف التشغيلي
2. **تحديث:** `.opencode/agents/tera.md` §8 — إضافة EngineeringAgent كخيار تفويض

---

### Approval

- [ ] الموافقة على إنشاء عميل **EngineeringAgent (مهندس منفّذ)**
- [ ] إحالة إلى TeraSystemEvolutionAgent (حارس) للتنفيذ

---

**أعدّه:** TeraAgent  
**التاريخ:** 2026-07-07  
**الموجه إلى:** حارس (TeraSystemEvolutionAgent)
