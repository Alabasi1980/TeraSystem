# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-12-092

**Title:** قاعدة مسارات الكتابة للمشاريع — التفريق بين المجلدات الجذرية (قوالب النظام) ومجلدات تطبيقات العملاء
**Request Type:** AIS Processing (AIS-0008) + Gap Resolution (GAP-013)
**Problem:** AIS-0008 و GAP-013 يوثقان مشكلة حقيقية: TeraAgent يكتب ملفات المشروع في المجلدات الجذرية `project-preparation/` و `project-control/` بدلاً من مجلد تطبيق العميل. هذا يدمّر القوالب الأصلية، ويخلط بين ملفات النظام وملفات المشروع، ويجعل من المستحيل تمييز مشاريع مختلفة.

**Evidence:**
1. ملفات المشروع كُتبت بالفعل في الجذر (GAP-013) وتم نقلها يدوياً لاحقاً
2. `clients/README.md` يحدد بنية المجلدات الصحيحة (كل تطبيق عميل له مجلداته الفرعية)
3. `tera.md` §7 (Project Output Location) يوجّه إلى مجلدات الجذر فقط بدون تمييز
4. الملفات الجذرية في `project-preparation/` و `project-control/` هي **قوالب فارغة** (PROJECT_STATE.md, TERA_ACTIVE_CONTEXT.md, DECISIONS_LOG.md) — يجب أن تبقى قوالب وليس فيها بيانات مشروع
5. `TeraArchitectureMap.md` §3 يحدد `project-preparation/` الجذر كـ "Internal Tera preparation outputs" — لكنه لا يذكر أن المشاريع الخارجية لها مجلداتها الخاصة تحت `clients/`

**Affected Files:**
1. `.opencode/agents/tera.md` — §7 (Project Output Location): غير دقيق للتمييز بين مشاريع العملاء والمشاريع الداخلية
2. `tera-system/runtime/TERA_RUNTIME_PROTOCOLS.md` — يحتاج قاعدة Write Location Decision
3. `tera-system/runtime/TERA_RUNTIME_CHECKLISTS.md` — يحتاج تحديث First Action checklist لمراعاة مسار العميل
4. `project-control/AGENT_GAPS_LOG.md` — GAP-013 بحاجة تحديث الحالة
5. `project-control/AGENT_IMPROVEMENT_SUGGESTIONS.md` — AIS-0008 بحاجة تحديث الحالة

**Proposed Change:**

### 1. تحديث `tera.md` §7 (Project Output Location)
إعادة هيكلة القسم ليصبح:

**Before (حالياً):**
```
project-preparation/  ← لكل مخرجات التحضير
project-control/     ← لكل سجلات التحكم
clients/             ← لسجلات العملاء الخارجيين
```

**After (مقترح):**
```
Two tiers of write locations:

TIER 1 — Root level (system templates only)
  project-preparation/  ← قوالب وتحضير النظام (يُنسخ إلى مجلد العميل عند الحاجة)
  project-control/      ← سجلات التحكم للنظام نفسه (تظل قوالب فارغة)

TIER 2 — Application level (project-specific data)  
  clients/CLIENT-XXXXX/applications/APP-XXXXX/project-preparation/  ← مخرجات تحضير المشروع
  clients/CLIENT-XXXXX/applications/APP-XXXXX/project-control/      ← سجلات تحكم المشروع

Write Location Decision Rule:
- For EXTERNAL CLIENT PROJECTS: Write to clients/.../applications/APP-xxx/project-*/
- For INTERNAL Tera PROJECTS (no client folder): Write to root project-*/
- Check before every write: "Is this for a client application?" → if YES, use client sub-path
```

### 2. تحديث `TERA_RUNTIME_PROTOCOLS.md`
إضافة قسم قصير (مثلاً §Write Location Protocol) يوضح قاعدة اتخاذ القرار عند الكتابة.

### 3. تحديث `TERA_RUNTIME_CHECKLISTS.md`
إضافة بند في First Action Checklist (Phase 1):
- [ ] تحديد نوع المشروع: Client Project أم Internal Project — لتحديد مسار الكتابة الصحيح

### 4. تحديث حالة GAP-013 و AIS-0008

**Why This Is Necessary:**
- يمنع تكرار مشكلة تلويث القوالب الجذرية
- يحافظ على فصل واضح بين مشاريع العملاء المختلفة
- يجعل المنظومة قابلة للتوسع مع زيادة عدد العملاء
- يُصحح مصدر الخلط: عدم وضوح مسارات الكتابة في `tera.md`

**Rejected Alternatives:**
1. إنشاء ملف منفصل `TERA_WRITE_LOCATION_PROTOCOL.md` — مرفوض (Anti-Bloat): المحتوى قليل ويكفي تحديث الأقسام الموجودة
2. إضافة قاعدة فقط في `TERA_RUNTIME_PROTOCOLS.md` دون تحديث `tera.md` — مرفوض لأن `tera.md` هو مصدر الحقيقة
3. إنشاء عميل جديد لمراقبة مسارات الكتابة — مرفوض (تضخم)

**Anti-Bloat Check:**
| السؤال | الإجابة |
|--------|---------|
| ما المشكلة التي تحلها؟ | TeraAgent يكتب في المكان الخطأ |
| لماذا لا يكفي تعديل ملف موجود؟ | يكفي — وسيتم تعديل ملفات موجودة فقط |
| لماذا لا يكفي عميل موجود؟ | يكفي — التعديل في `tera.md` نفسه |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تقلل — تزيل الغموض عن مسارات الكتابة |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | لا — إضافات قليلة جداً (عشرات السطور) |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | نعم — مجرد توضيح في الملفات الموجودة، وهذا ما نفعله |

**Risk:**
- منخفض — تغيير توضيحي فقط، لا يكسر شيئاً
- يحتاج متابعة أن `clients/README.md` يبقى متزامناً

**Rollback Plan:**
- إعادة `tera.md` §7 إلى نصه الأصلي
- إزالة القسم المضاف من `TERA_RUNTIME_PROTOCOLS.md`
- إزالة البند المضاف من `TERA_RUNTIME_CHECKLISTS.md`

**Approval Required:** Majed
