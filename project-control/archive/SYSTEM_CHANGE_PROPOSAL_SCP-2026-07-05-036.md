# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-05-036

## Title: إنشاء Source of Truth لـ Auditor (مدقق) — TeraAuditor.md + تحديثات منظومية

## Request Type: Agent Source of Truth Creation / Policy Update / Gap Closure

## Problem:
Auditor (مدقق) هو عميل حوكمة مستقل بدون Source of Truth. كل عميل حوكمة آخر لديه:
- Monitor → `TeraMonitor.md` ✅
- DesignReviewer → `TeraDesignReviewer.md` ✅
- TCEA → `TeraClientEngagement.md` ✅
- **Auditor → ❌ لا يوجد**

المشاكل الناتجة:
1. ليس لدى مدقق دستور عمل مكتوب يحدد خطواته ومنهجيته وأولوياته بدقة.
2. يبدأ كل تدقيق من الصفر لأنه لا يملك بروتوكولاً تراكمياً.
3. لا يوجد عنده بروتوكول عدم يقين — قد يخمن أو يتجاوز.
4. لا يوجد تدرج هرمي للمراجع — يعتمد على اجتهاده الشخصي عند التعارض.
5. ENGINEERING_AGENT_RESPONSIBILITIES.md §5 تعريفه الحالي مختصر جداً (13 سطراً فقط).

## Evidence:
1. كل ملفات `.opencode/agents/auditor.md` لا تشير إلى أي Source of Truth مخصص.
2. ENGINEERING_AGENT_RESPONSIBILITIES.md §5 يصف دوره في 13 سطراً فقط — لا منهجية، لا مصنف نتائج، لا بروتوكولات.
3. Auditor نفسه قدّم تقريراً بفجواته مباشرة لـ Majed.

## Affected Files:
### 🆕 إنشاء
1. `tera-system/TeraAuditor.md` — Source of Truth (10 أقسام)

### 🔄 تحديث
2. `.opencode/agents/auditor.md` — إضافة System Reference + بروتوكول التراكم
3. `tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md §5` — توسيع التعريف (منهجية + مراجع + بروتوكولات)
4. `tera-system/TeraPolicyMap.md` — إضافة إدخال Auditor
5. `project-control/AGENT_GAPS_LOG.md` — إضافة GAP-011
6. `project-control/SYSTEM_EVOLUTION_LOG.md` — تسجيل SCP-036

## Proposed Change:
### 1. TeraAuditor.md
Source of Truth لـ Auditor (مدقق) — 10 أقسام:
- §1: الهوية
- §2: الموقع في المنظومة
- §3: الغرض
- §4: المراجع المعتمدة (Reference Hierarchy — 7 مستويات)
- §5: منهجية التدقيق المتدرجة (6 مراحل: استيعاب ← توثيق ← تدقيق هندسي ← مطابقة ← تقرير ← توصية)
- §6: جدول تصنيف النتائج (PASS / NEEDS_FIX / BLOCKED / DEFERRED مع معايير كل حالة)
- §7: بروتوكول عدم اليقين والبحث (متى أتوقف، متى أطلب بحثاً — WebSearch/WebFetch)
- §8: بروتوكول التراكم (بناءً على آخر تدقيق)
- §9: العلاقة مع بقية العملاء
- §10: مرجع التحسين المستمر

### 2. auditor.md (Runtime)
- إضافة System Reference بعد CONDUCT GATE: `Your source of truth is: tera-system/TeraAuditor.md`
- تحديث Output Format بإضافة حالة DEFERRED
- إضافة بروتوكول التراكم كـ "one-liner" في What you do

### 3. ENGINEERING_AGENT_RESPONSIBILITIES.md §5
توسيع §5 من 13 سطراً إلى تعريف كامل يغطي:
- المرجع الأعلى (TeraAuditor.md)
- منهجية التدقيق (باختصار — تفصيلها في TeraAuditor.md)
- بروتوكول التراكم
- بروتوكول عدم اليقين
- روابط مع Engineer §4 و Monitor §6

### 4. TeraPolicyMap.md
إضافة إدخال: `Auditor audit framework` → Source of Truth = `tera-system/TeraAuditor.md`

## Why This Is Necessary:
Auditor يقوم بدور مراجعة الجودة والامتثال والتوثيق. بدون Source of Truth:
- قراراته تصبح اجتهادية دون معايير واضحة
- لا يستطيع التراكم على تدقيقات سابقة
- لا توجد منهجية واضحة لاكتشاف فجوات التوثيق

هذه هي آخر فجوة من نوعها — كل عملاء الحوكمة أصبح لديهم مصادر حقيقة.

## Rejected Alternatives:
1. **دمج المحتوى في ENGINEERING_AGENT_RESPONSIBILITIES.md** ❌ — هذا الملف هو خريطة مسؤوليات مهندسية فقط، ليس Source of Truth. Pattern السائد (TeraMonitor.md, TeraDesignReviewer.md) يضع Source of Truth في `tera-system/` .
2. **AUDIT_TRAIL.md** ❌ — Anti-Bloat. التدقيق التراكمي يسجل في `PROJECT_ACTIVITY_LOG.md` الموجود.
3. **الاكتفاء بتحديث auditor.md فقط** ❌ — ملف .opencode هو Runtime موجز، ليس Source of Truth كاملاً.

## Anti-Bloat Check:
| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | Auditor بدون Source of Truth — يقف وحده بين عملاء الحوكمة بدون دستور |
| لماذا لا يكفي تعديل ملف موجود؟ | ENGINEERING_AGENT_RESPONSIBILITIES.md ليس مصدر حقيقة كافياً — Pattern السائد يضع Sources of Truth في `tera-system/` |
| لماذا لا يكفي عميل موجود؟ | Auditor هو العميل نفسه — هو من يحتاج Source of Truth |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلل التعقيد** — Source of Truth يقلل الاجتهاد الشخصي ويوحد المنهجية |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | لا — ملف واحد read-only في tera-system/، يقرأه Auditor فقط عند التفعيل |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — نظام الحوكمة يحتاج لكل عميل Source of Truth مخصص |

## Risk:
منخفضة — نمط متبع مع Monitor و DesignReviewer. ملف واحد جديد + تحديثات طفيفة للـ runtime.

## Rollback Plan:
1. حذف `tera-system/TeraAuditor.md`
2. إزالة System Reference من `.opencode/agents/auditor.md`
3. إعادة ENGINEERING_AGENT_RESPONSIBILITIES.md §5 للصيغة السابقة (13 سطراً)
4. إزالة إدخال Auditor من `TeraPolicyMap.md`
5. إزالة GAP-011 من `AGENT_GAPS_LOG.md`
6. إزالة إدخال SCP-036 من `SYSTEM_EVOLUTION_LOG.md`

## Approval Required:
✅ Majed
