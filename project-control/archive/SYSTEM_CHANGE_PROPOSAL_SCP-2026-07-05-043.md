# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-05-043

## تنظيف ملف TCEA — إزالة التكرار والتعارض

---

## 1. Request Type
System Maintenance / Anti-Bloat / Policy Conflict Resolution

## 2. Problems (3 Issues)

### المشكلة 1 — 🔴 تكرار كبير: §1 (Core Functional Roles) + §2 (مسؤولياتك الأساسية)

| الـ 8 أدوار English في §1 | الـ 10 مسؤوليات عربي في §2 |
|---|---|
| Client Discovery Consultant | Client Qualification + Client Discovery |
| Scope Analyst | Scope Packaging |
| Pricing Estimator | Pricing Management |
| Client Documentation Manager | Client Documents |
| Change Request Analyst | Change Request Management ✅ |
| Handoff Package Manager | Delivery & Handover |
| Workspace Creator | Workspace Creation ✅ |
| Maintenance & Support Advisor | Maintenance & Support ✅ |
| — | Project Classification (فريد) |

**المشكلة:** §1 و §2 يصفان نفس المهام بطريقتين مختلفتين بلغتين وتقسيمين. يقرأ المستشار 8 أدوار ثم 10 مسؤوليات — غير واضح أيها المعتمد، وأيهما يسبق.

### المشكلة 2 — 🟠 تعارض/نقص: §3 workflow لا يذكر Consultation Response Protocol

§3 workflow (الأسطر 122-123) يذكر Self-Check و Uncertainty Protocols فقط:
```
← لكل Domain: طبق Self-Check Protocol قبل إعلان Complete
← إذا كان هناك عدم يقين: طبق Uncertainty Protocol
```

لكن §5.3 أضيف بعد SCP-042 ولا ينعكس في التدفق العام.

### المشكلة 3 — 🟡 تعارض بسيط: أداة التسعير في §6

§6 (مسموح) السطر 252:
```
إنتاج مسودة عرض سعر (Level 2) باستخدام TeraPricingPolicy.md
```
لكن §10 يقول الحاسبة (Excel) هي الأداة الوحيدة المعتمدة. هذا يخلق ارتباكاً: هل يستخدم الـ Policy مباشرة أم الحاسبة؟

## 3. Affected Files

| الملف | نوع التغيير |
|-------|------------|
| `.opencode/agents/tera-client-engagement.md` | **UPDATE** — 3 تعديلات |

ملف واحد فقط — لا ملفات جديدة.

## 4. Proposed Changes

### 4.1 — دمج §1 + §2 في جدول واحد ثنائي اللغة

**الإجراء:** حذف §1 (Core Functional Roles) و §2 (مسؤولياتك الأساسية) واستبدالهما بقسم واحد موحد:

```
## 1. الأدوار والمسؤوليات

### جدول الأدوار ثنائي اللغة

| # | English Role | المسؤولية بالعربي | الوصف المختصر |
|---|---|---|---|
| 1 | **Client Discovery & Qualification** | اكتشاف العميل وتأهيله | فهم من هو الزبون، ماذا يحتاج، لماذا، من يقرر، ومدى جدية الفرصة |
| 2 | **Scope Analyst** | تحليل النطاق | تحويل احتياجات الزبون إلى نطاق مبدئي: داخل النطاق، خارج النطاق، مؤجل، غير واضح، افتراضات، قيود |
| 3 | **Pricing Estimator** | تقدير التسعير | تحويل النطاق المبدئي إلى خيارات تسعير منظمة (Level 1 → Level 2) — مسودات فقط، Majed يعتمد |
| 4 | **Client Documentation Manager** | إدارة وثائق العميل | توثيق كل المعلومات بقراراتها وإصداراتها في مسار نظيف يمكن تتبعه |
| 5 | **Change Request Analyst** | تحليل طلبات التغيير | تقييم أثر كل طلب جديد على النطاق والتكلفة والوقت والمخاطر — Majed يقرر |
| 6 | **Handoff & Delivery Manager** | إدارة التسليم والهاندوف | تجميع كل ما اكتشف وحُدّد في حزمة هاندوف نظيفة للتسليم لـ Tera |
| 7 | **Workspace Creator** | إنشاء مساحة العمل | إنشاء هيكل مجلدات العميل بعد اعتماد الهاندوف |
| 8 | **Maintenance & Support Advisor** | استشارات الصيانة والدعم | تحديد رؤية الدعم ما بعد التسليم مبكراً: الضمان، الصيانة، حدود الدعم، التدريب |
| 9 | **Project Classifier** | تصنيف المشروع | تصنيف المشروع (صغير/متوسط/معقد/غامض) لتحديد مسار التسعير والتحليل والعمق |
```

**مبرر الدمج:**
- إزالة 15 سطراً من التكرار
- توحيد المرجع (لا تساؤل: أيهما المعتمد؟)
- إضافة عمود "وصف مختصر" يلخص الدور مباشرة بدلاً من فقرتين منفصلتين

### 4.2 — إضافة Consultation Response إلى §3 workflow

**الإجراء:** إضافة سطر بعد "حوار استكشافي" في التدفق:
```
← بعد كل دفعة معلومات من Majed: طبّق Consultation Response Protocol (§5.3) — حلّل، اقترح، حدّد مخاطر، اسأل، قسّم مرحلياً
```

### 4.3 — تصحيح أداة التسعير في §6

**الإجراء:** تعديل السطر من:
```
إنتاج مسودة عرض سعر (Level 2) باستخدام TeraPricingPolicy.md
```
إلى:
```
إنتاج مسودة عرض سعر (Level 2) باستخدام TeraPricingCalculator.xlsx (حسب TeraPricingPolicy.md)
```

## 5. Why This Is Necessary

1. **تقليل التضخم** — إزالة تكرار 15 سطراً في ملف الرنتايم
2. **إزالة الارتباك** — المستشار لن يتساءل "هل أتبع §1 أم §2؟"
3. **اتساق التدفق** — §3 سيذكر Consultation Response مثلما يذكر Self-Check و Uncertainty
4. **دقة الإحالة** — §6 سيحيل للأداة الصحيحة (الحاسبة) بدلاً من السياسة

## 6. Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| إبقاء §1 و §2 منفصلين مع cross-reference | لا يحل مشكلة التكرار — يبقيها فقط |
| حذف §1 وحده | §1 يحتوي تفاصيل بالإنجليزية مفيدة للتوجيه |
| حذف §2 وحده | §2 يحتوي مسؤوليات (Qualification, Classification) غير موجودة في §1 |
| إنشاء ملف منفصل للأدوار | Anti-Bloat — ملف واحد يكفي |

## 7. Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | تكرار + تعارضين في ملف مستخدم واحد |
| لماذا لا يكفي تعديل ملف موجود؟ | التعديل داخل الملف نفسه — لا ملفات جديدة |
| لماذا لا يكفي عميل موجود؟ | الخلل في تعريف المستشار نفسه |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلله** — إزالة 15 سطراً مكرراً، توحيد المرجع |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | **إيجابي** — الملف يصبح أقصر |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — 3 تعديلات ضرورية في ملف واحد |

## 8. Risk

- **منخفضة جداً** — دمج جدول لا يغير أي صلاحية أو Gate أو سلوك
- التعديلات الثلاثة شكلية/تنظيمية فقط
- لا تأثير على runtime behavior

## 9. Rollback Plan

1. استعادة `.opencode/agents/tera-client-engagement.md` من git:
   `git checkout -- .opencode/agents/tera-client-engagement.md`

## 10. Approval Required

- [ ] **Majed** — موافقة صريحة
