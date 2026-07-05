# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-04-035

## Title
**إنشاء TeraMonitor.md — Source of Truth + ميثاق التدقيق لـ Monitor (رقيب)**

## Request Type
Agent Capability Improvement / Source of Truth Creation / Process Formalization

## Source
تقرير Monitor (رقيب) الذاتي — تحليل وتوصية من TeraSystemEvolutionAgent (حارس)

---

## Problem

Monitor (رقيب) هو أحد عملاء الحوكمة الأساسيين في منظومة Tera، لكنه **لا يملك Source of Truth نظامياً** يحدد:

- **دستور عمله** (قواعد ثابتة، وليس اجتهاداً شخصياً)
- **المراجع المعتمدة وتدرجها الهرمي** (أي ملف يعلو أي ملف)
- **صلاحية رفض خطة معيبة** (بدونها يوافق على انحرافات)
- **آلية التدقيق التراكمي** (لا يعيد من الصفر)
- **العلاقة مع بقية العملاء** في سياق التدقيق

الوضع الحالي: رقيب يعتمد على قائمة مهام (What you do) في `monitor.md` فقط — **بدون دستور مكتوب**. هذا يجعله عرضة للاجتهاد الشخصي، وهو خطر على دور حساس مسؤوليته كشف الانحرافات قبل أن تصل إلى الإنتاج.

**المقارنة:** ناقد (DesignReviewer) حصل على Source of Truth (`TeraDesignReviewer.md`) في SCP-032. رقيب يحتاج نفس المستوى من التوثيق النظامي.

---

## Evidence

- `.opencode/agents/monitor.md`: 92 سطراً — قائمة مهام تشغيلية فقط، بدون دستور أو قواعد ثابتة.
- `tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md` §6: 7 أسطر فقط — تعريف سطحي.
- `tera-system/TeraPolicyMap.md`: لا يوجد إدخال لـ Monitor كـ Source of Truth.
- `tera-system/TeraArchitectureMap.md`: لا يذكر Monitor كطبقة أو دور.
- بالمقابل: `TeraDesignReviewer.md` موجود (169 سطراً، 13 قسماً).

---

## Affected Files

### الملفات الجديدة (1 file)
1. `tera-system/TeraMonitor.md` (Source of Truth + ميثاق التدقيق — يجمع 8 أقسام)

### الملفات المعدلة (5 files)
2. `.opencode/agents/monitor.md` (Runtime — إضافة System Reference + ربط بـ TeraMonitor.md)
3. `tera-system/engineering-governance/ENGINEERING_AGENT_RESPONSIBILITIES.md` (تحديث §6)
4. `tera-system/TeraPolicyMap.md` (إضافة إدخال Monitor)
5. `project-control/AGENT_GAPS_LOG.md` (إضافة GAP-010)
6. `project-control/SYSTEM_EVOLUTION_LOG.md` (تسجيل SCP-035)

---

## Proposed Change

### 1. إنشاء `tera-system/TeraMonitor.md` (Source of Truth)

ملف واحد يجمع 8 أقسام:

| § | القسم | المحتوى |
|---|-------|---------|
| 1 | **الهوية** | الاسم، اللقب (رقيب)، النوع، العلاقة، الصلاحية الافتراضية، التفعيل |
| 2 | **الموقع في المنظومة** | الشجرة التنظيمية + تدفق العمل |
| 3 | **الغرض** | ماذا يفعل رقيب بالضبط |
| 4 | **المراجع المعتمدة (Reference Hierarchy)** | 8 مستويات من الدستور إلى سجل النشاط — أيها يعلو أيها |
| 5 | **قواعد التدقيق السبعة الثابتة (The 7 Immutable Audit Rules)** | مطابقة الخطة، الترتيب والتبعيات، بوابة الهندسة، سجل الامتثال، Handback vs Git Diff، زحف النطاق، الانحراف المعماري |
| 6 | **صلاحية رفض الخطة (Plan Rejection Authority)** | 4 شروط واضحة تمنح رقيب حق الرفض أو طلب التدقيق |
| 7 | **العلاقة مع بقية العملاء** | مع TeraAgent، Auditor، ناقد — لا يتواصل مباشرة |
| 8 | **مرجع التحسين المستمر** | رابط لـ TERA_CONTINUOUS_IMPROVEMENT_POLICY.md |

**ملاحظة Anti-Bloat:** التدقيق التراكمي (آخر نقطة تدقيق) يُسجل في `PROJECT_ACTIVITY_LOG.md` الموجود — لا ملف جديد.

### 2. تحديث `.opencode/agents/monitor.md` (Runtime)

- إضافة: `System Reference: \`tera-system/TeraMonitor.md\``
- إضافة: `Last Synced: 2026-07-04`
- إضافة `DESIGN_REVIEW_STANDARDS.md` إلى قائمة القراءة عند الحاجة
- اختصار الـ "What you do" بالإشارة إلى TeraMonitor.md للقواعد التفصيلية

### 3. تحديث `ENGINEERING_AGENT_RESPONSIBILITIES.md` §6

توسيع §6 من 7 أسطر إلى تعريف يشير إلى TeraMonitor.md كمصدر الحقيقة + القواعد السبعة + صلاحية الرفض.

### 4. تحديث `TeraPolicyMap.md`

إضافة إدخال جديد:

```
| Monitor audit framework | `tera-system/TeraMonitor.md` | `.opencode/agents/monitor.md` | Governs the 7 immutable audit rules, reference hierarchy, plan rejection authority, and cumulative audit. |
```

---

## Rejected Alternatives

| البديل | سبب الرفض |
|--------|-----------|
| `MONITOR_CHARTER.md` في `project-control/` | موقع خطأ — `project-control/` للمشاريع، وليس لتعريفات العملاء النظاميين |
| `MONITOR_AUDIT_TRAIL.md` كملف منفصل | تضخم — آخر نقطة تدقيق تُسجل في `PROJECT_ACTIVITY_LOG.md` الموجود |
| دمج الميثاق في `ENGINEERING_AGENT_RESPONSIBILITIES.md` | هذا ملف مسؤوليات هندسية، وليس Source of Truth لعميل حوكمة |
| إنشاء `TeraMonitor.md` في `tera-system/engineering-governance/` | غير مناسب — هذا ملف تعريف عميل نظامي، وليس حوكمة هندسية |
| عدم إنشاء أي شيء والاكتفاء بـ "رقيب يعرف" | الوضع الحالي — أثبت تقرير رقيب أن الاجتهاد غير كافٍ |

---

## Anti-Bloat Check

| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | رقيب بلا دستور — يعمل باجتهاد شخصي في دور حساس |
| لماذا لا يكفي تعديل ملف موجود؟ | لا يوجد ملف موجود يجمع الدستور + القواعد + التدرج — الملفات الحالية تشغيلية فقط |
| لماذا لا يكفي عميل موجود؟ | رقيب هو العميل المسؤول — يحتاج Source of Truth خاص به |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | **تقلّله** — القواعد الثابتة تقضي على الاجتهاد العشوائي ويقلل الأخطاء |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | طفيف — يُقرأ عند تفعيل رقيب فقط |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | لا — ملف واحد لـ 8 أقسام هو الأصغر |

---

## Risk

| المخاطرة | مستواها | التخفيف |
|-----------|---------|---------|
| تضارب بين TeraMonitor.md و monitor.md | 🟡 متوسط | monitor.md يشير إلى TeraMonitor.md كـ source — القواعد في المصدر، التشغيل في الرنتايم |
| صلاحية الرفض قد توقف العمل | 🟢 منخفض | الرفض محدود بـ 4 شروط واضحة + القرار النهائي لك |
| الميثاق أكبر من اللازم | 🟢 منخفض | 8 أقسام فقط — مقارنة بـ 13 قسماً في TeraDesignReviewer.md |

---

## Rollback Plan

1. حذف `tera-system/TeraMonitor.md`.
2. `.opencode/agents/monitor.md`: إزالة System Reference وإعادة "What you do" القديم.
3. `ENGINEERING_AGENT_RESPONSIBILITIES.md`: إعادة §6 للصيغة السابقة.
4. `TeraPolicyMap.md`: حذف إدخال Monitor.
5. `AGENT_GAPS_LOG.md`: حذف GAP-010.

---

## Approval Required

Majed — موافقة على SCP-035:
- [ ] Approval: إنشاء `tera-system/TeraMonitor.md` (8 أقسام)
- [ ] Approval: تحديث `monitor.md` (ربط بـ TeraMonitor.md)
- [ ] Approval: تحديث `ENGINEERING_AGENT_RESPONSIBILITIES.md §6`
- [ ] Approval: تحديث `TeraPolicyMap.md`
- [ ] Approval: تسجيل GAP-010
