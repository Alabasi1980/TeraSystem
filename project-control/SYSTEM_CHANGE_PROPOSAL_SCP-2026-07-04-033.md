# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-04-033

## Title
**تطوير DesignReviewer (ناقد) — قاعدة معايير، بروتوتايب، مساعدة بصرية، وتوسيع فحص التوكينز**

## Request Type
Agent Capability Improvement / Knowledge Base Creation / Process Expansion

## Source
تقرير DesignReviewer (ناقد) الذاتي — تحليل وتوصية من TeraSystemEvolutionAgent (حارس)

---

## Problem

يمتلك ناقد الآن (بعد SCP-032) متصفحاً للمعاينة البصرية (Playwright MCP) وفحص توكينز أساسي و Source of Truth مستقل. لكن تبقى 4 فجوات:

1. **لا قاعدة معايير تدقيق موحدة** — ناقد يعتمد على اجتهاده الشخصي في كل مراجعة، دون قوائم تفتيش منهجية تغطي: حالات الحافة، الاتساق الوظيفي، النظافة، UX، الاستجابة، RTL المتقدمة.
2. **لا بروتوكول لطلب المساعدة البصرية** — عندما يحتاج ناقد تأكيداً بصرياً لا يستطيع الحصول عليه (لون دقيق، تخطيط معقد)، لا توجد آلية تواصل واضحة مع Majed.
3. **لا قدرة على بناء بروتوتايب** — للمراجعة قبل التنفيذ، ناقد لا يستطيع بناء نموذج HTML/CSS سريع لتأكيد التصميم قبل الالتزام بالتنفيذ.
4. **فحص التوكينز محدود** — لا يشمل الـ 3-layer architecture (Primitive → Semantic → Component).

---

## Evidence

- `tera-system/TeraDesignReviewer.md`: 12 أقسام — لا يحتوي على معايير تدقيق ولا بروتوتايب ولا بروتوكول مساعدة بصرية.
- `.opencode/agents/design-reviewer.md`: `write: deny` — لا يستطيع كتابة حتى بروتوتايب مؤقت.
- `tera-system/design-system/`: يوجد لكن لا يحتوي على `DESIGN_REVIEW_STANDARDS.md`.
- `ENGINEERING_AGENT_RESPONSIBILITIES.md §11`: لا يذكر البروتوتايب كأداة مراجعة.

---

## Affected Files

### الملفات المعدلة (5 files)
1. `tera-system/TeraDesignReviewer.md` (Source of Truth — إضافة بروتوتايب + مساعدة بصرية + توكينز مطوّر)
2. `.opencode/agents/design-reviewer.md` (Runtime — `write: ask` + بروتوكولات جديدة + Output format)
3. `tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md` (تحديث §11)
4. `project-control/AGENT_GAPS_LOG.md` (إضافة GAP-008)
5. `project-control/SYSTEM_EVOLUTION_LOG.md` (تسجيل التغيير)

### الملفات الجديدة (1 file)
1. `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md` (قاعدة المعرفة — 9 أقسام)

### مقارنة مع تقرير ناقد

| البند في تقرير ناقد | مقبول؟ | ملاحظة |
|---------|---------|--------|
| 1. رفع `write: deny` → `write: ask` | ✅ | OpenCode لا يدوم تقييد المسارات — الصلاحية عامة، لكن الانضباط الذاتي في وصف agent يحدد الاستخدام |
| 2. إنشاء `DESIGN_REVIEW_STANDARDS.md` | ✅ | في `tera-system/design-system/` — المجلد موجود |
| 3. إضافة قسم البروتوتايب لـ Source of Truth | ✅ | في `TeraDesignReviewer.md` |
| 4. قائمة تفتيش RTL مفصّلة | ✅ | ضمن `DESIGN_REVIEW_STANDARDS.md` القسم 7 |
| 5. بروتوكول المساعدة البصرية | ✅ | Visual Assistance Protocol |
| 6. توسيع فحص التوكينز (3-layer) | ✅ | تحديث § في الملفين |
| تحديث Output format | ✅ | تنسيق جديد شامل |
| تحديث AGENT_GAPS_LOG.md | ✅ | GAP-008 |
| تحديث ENGINEERING_AGENT_RESPONSIBILITIES.md §11 | ✅ | إضافة ذكر البروتوتايب |

---

## Proposed Change

### 1. ترقية صلاحية الكتابة
**الملف:** `.opencode/agents/design-reviewer.md`
```
-  write: deny
+  write: ask
```
مع إضافة انضباط ذاتي في الوصف: الكتابة محصورة في بروتوتايب HTML/CSS داخل `project-control/prototypes/` فقط.

### 2. إنشاء DESIGN_REVIEW_STANDARDS.md
**الملف الجديد:** `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md`

9 أقسام (قوائم تفتيش قابلة للتنفيذ):
1. **Functional & Edge Case Completeness** — حالات فارغ/خطأ/تحميل + حافة
2. **Visual Consistency** — ألوان، خطوط، مسافات، أزرار، جداول، مودالات
3. **Visual Quality Principles** — تدرج هرمي، مسافات بيضاء، تكتلات، تباين
4. **Design Hygiene** — لا Debug، لا نصوص وهمية، لا Broken elements
5. **UX Principles** — نماذج، تأكيدات، تغذية راجعة، أهداف لمسية
6. **Responsive** — 4 أحجام شاشة كحد أدنى
7. **RTL & Arabic Review** — CSS Logical Properties، Icon Mirroring، Arabic Typography، Form Fields، Animations + Common Mistakes (10 بنود)
8. **Design Token Verification** — 3-layer hierarchy
9. **Functional Consistency** — Tooltips، Toasts، Modals، Validation trigger

### 3. تحديث TeraDesignReviewer.md (Source of Truth)
إضافة:
- **§X. إنشاء البروتوتايب التجريبي** — متى، كيف، القواعد، مثال بنية، الممنوعات
- **§Y. طلب المساعدة البصرية** — في نهاية §8 (Browser Preview)
- **توسيع §9 (التحقق من التوكينز)** — إضافة 3-layer check

### 4. تحديث design-reviewer.md (Runtime)
إضافة:
- **Visual Assistance Protocol** — طلب صور من Majed + تحليلها
- **Prototype Protocol** — بناء HTML/CSS بناءً على طلب Majed
- **توسيع Design Token Verification** — 3-layer hierarchy
- **تحديث Output format** — تنسيق شامل مع Pre/Post Implementation Checklists
- رفع `write: deny` → `write: ask`

### 5. تحديث ENGINEERING_AGENT_RESPONSIBILITIES.md §11
إضافة: `may build static HTML/CSS prototypes from design sources for visual review when Majed requests (prototypes are for review only, not production code).`

### 6. تسجيل GAP-008 في AGENT_GAPS_LOG.md

---

## Why This Is Necessary

1. **قاعدة المعايير** تمنح ناقد مرجعاً موضوعياً بدلاً من الاجتهاد الشخصي — يقرأ الملف قبل كل مراجعة ويطبق القوائم.
2. **البروتوتايب** يسد فجوة "المراجعة قبل التنفيذ" — ناقد يبني نموذج HTML/CSS سريع لتأكيد التصميم قبل الالتزام بالكود.
3. **المساعدة البصرية** تحل مشكلة "لا أستطيع رؤية اللون الدقيق" — بدلاً من التخمين، يطلب صورة من Majed.
4. **3-layer token** يمنع انحراف المكونات عن التوكينز الدلالية (Semantic Tokens).

---

## Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| دمج `DESIGN_REVIEW_STANDARDS.md` في `TeraDesignReviewer.md` | المصدر سيصبح 21 قسماً — يخلط بين الهوية (identity) والمرجع (reference) |
| عدم إنشاء قاعدة معايير والاكتفاء بـ "ناقد يعرف" | هذا الوضع الحالي — أثبت التقرير أن الاجتهاد غير كافٍ |
| `write: ask` مع تقييد مسارات (غير مدعوم) | OpenCode لا يدعمه — الانضباط الذاتي + موافقة Majed كافيان |
| إنشاء MCP أداة تصوير إضافية | غير مبرر — يوجد متصفح (Playwright) + طلب صور من Majed |
| إضافة قواعد RTL كملف منفصل | دُمجت في `DESIGN_REVIEW_STANDARDS.md` القسم 7 (Anti-Bloat) |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | ناقد بلا معايير موحدة، بلا بروتوتايب، بلا مساعدة بصرية، بلا توكينز معمّق |
| لماذا لا يكفي تعديل ملف موجود؟ | `TeraDesignReviewer.md` هو Source of Truth للهوية والصلاحيات — إضافة 9 قوائم تفتيش يخلط الغرض |
| لماذا لا يكفي عميل موجود؟ | لا يوجد عميل آخر يقوم بمراجعة التصميم — هذا هو دور ناقد الوحيد |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلّله** — قوائم التفتيش الموحدة تمنع التكرار والاجتهاد العشوائي |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | طفيف — `DESIGN_REVIEW_STANDARDS.md` يُقرأ قبل المراجعة فقط (~100 سطر) |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — ملف واحد جديد + تحديثات محدودة للموجود. 9 أقسام في ملف واحد أفضل من 9 ملفات |

---

## Risk

| المخاطرة | مستواها | التخفيف |
|-----------|---------|---------|
| `write: ask` يتطلب موافقة لكل كتابة حتى لو كانت غير مقصودة | 🟢 منخفض | Majed يوافق/يرفض كل عملية — لا قدرة على كتابة غير مراقبة |
| تضخم في `DESIGN_REVIEW_STANDARDS.md` مع الوقت | 🟡 متوسط | سياسة: أي إضافة تتطلب مبرر Anti-Bloat. المرجع لا ينمو بدون سبب |
| البروتوتايب يُستخدم ككود إنتاج | 🟢 منخفض | ممنوع صراحة في البروتوكول + البروتوتايب مؤقت ويُحذف بعد الاعتماد |
| طلب صور متكرر من Majed يُربك سير العمل | 🟢 منخفض | البروتوكول ينص على أن الطلب فقط عند انسداد التحليل — ليس لكل فحص |

---

## Rollback Plan

1. `.opencode/agents/design-reviewer.md`: إعادة `write: ask` → `write: deny`، حذف الأقسام المضافة، إعادة Output format القديم.
2. `tera-system/design-system/DESIGN_REVIEW_STANDARDS.md`: حذف الملف.
3. `tera-system/TeraDesignReviewer.md`: حذف أقسام البروتوتايب والمساعدة البصرية، إعادة §9 القديم.
4. `tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md`: إعادة §11 لصيغته السابقة.
5. `project-control/AGENT_GAPS_LOG.md`: حذف GAP-008.
6. `project-control/SYSTEM_EVOLUTION_LOG.md`: حذف إدخال SCP-033.

---

## Approval Required

Majed — موافقة على SCP-033 بكامله:
- [ ] Approval: تنفيذ الـ 6 بنود دفعة واحدة
- [ ] Approval: `write: deny` → `write: ask`
- [ ] Approval: إنشاء `DESIGN_REVIEW_STANDARDS.md`
- [ ] Approval: تحديث `TeraDesignReviewer.md` (بروتوتايب + مساعدة بصرية + توكينز)
- [ ] Approval: تحديث `design-reviewer.md` (بروتوكولات + Output format)
- [ ] Approval: تحديث `ENGINEERING_AGENT_RESPONSIBILITIES.md §11`
