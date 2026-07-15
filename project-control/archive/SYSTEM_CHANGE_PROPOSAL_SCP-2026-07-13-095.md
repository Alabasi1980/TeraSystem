# SYSTEM_CHANGE_PROPOSAL — SCP-2026-07-13-095

**Title:** إضافة قالب TERA_HANDOFF_PACKAGE مع قسم التوافق مع SCP-038
**Request Type:** AIS Processing (AIS-0007)
**Problem:** AIS-0007: SCP-038 compliance section was being added manually to handoff packages. No official template exists in TERA_RUNTIME_TEMPLATES.md for TERA_HANDOFF_PACKAGE.md. Future projects risk missing SCP-038 compliance, and Auditor loses unified reference.

**Evidence:**
1. AIS-0007 documented in `AGENT_IMPROVEMENT_SUGGESTIONS.md`
2. Two handoff packages (MAWTHOOQ, Omran) both contain SCP-038 compliance sections but with inconsistent structures
3. Existing handoff template in `tera-workshop/client-templates/handover/HANDOVER_REPORT_TEMPLATE.md` is a post-delivery template, not the pre-execution handoff
4. TERA_RUNTIME_TEMPLATES.md had no section for TERA_HANDOFF_PACKAGE

**Affected Files:**
1. `tera-system/runtime/TERA_RUNTIME_TEMPLATES.md` — add §36 TERA_HANDOFF_PACKAGE.md template

**Proposed Change:**
Add a comprehensive reusable template for TERA_HANDOFF_PACKAGE.md as §36 in TERA_RUNTIME_TEMPLATES.md, including:
- Front matter metadata
- Table of contents
- §1 SCP-038 Compliance section (mandatory — do NOT remove)
- §§2-11 Standard handoff sections (Executive Summary, Client Info, Scope, Features, Out of Scope, Pricing, Decisions, Open Points, File Inventory, Readiness Decision)
- All sections include placeholders and usage notes

**Why This Is Necessary:**
- Ensures every future handoff includes SCP-038 compliance
- Standardizes structure across all handoff packages
- Auditor can reference a single source of truth
- Reduces TCEA's manual work

**Rejected Alternatives:**
1. Creating a separate file `tera-system/runtime/TERA_HANDOFF_PACKAGE_TEMPLATE.md` — rejected for now (file size issue should be handled holistically via M-1)
2. Adding only the SCP-038 section (not the full template) — rejected: the full template provides structure and context

**Anti-Bloat Check:**
| Question | Answer |
|----------|--------|
| ما المشكلة التي تحلها؟ | لا يوجد قالب موحد لـ TERA_HANDOFF_PACKAGE — كل مشروع يعيد ابتكار هيكله |
| لماذا لا يكفي تعديل ملف موجود؟ | لا يوجد ملف موجود — يجب إضافته |
| لماذا لا يكفي عميل موجود؟ | القوالب وظيفة TCEA، لكن القالب نفسه أداة نظامية في TERA_RUNTIME_TEMPLATES.md |
| هل الإضافة ستقلل التعقيد أم تزيده؟ | تقلل — تمنع إعادة الاختراع لكل مشروع |
| هل يوجد أثر سلبي على استهلاك التوكنز؟ | يزيد الملف ~145 سطراً (موجود في ملف كبير أصلاً) — يُعالَج مع M-1 |
| هل توجد طريقة أصغر لتحقيق نفس الهدف؟ | إضافة القالب الكامل أفضل من إضافة القسم فقط لأنه يوفر السياق الكامل |

**Risk:**
- Low — template only, no active system behaviour change
- File size of TERA_RUNTIME_TEMPLATES.md increases (~1,606 → ~1,751 lines) — flagged for splitting (M-1)

**Rollback Plan:**
- Remove §36 (TERA_HANDOFF_PACKAGE.md) from TERA_RUNTIME_TEMPLATES.md
- Revert AGENT_IMPROVEMENT_SUGGESTIONS.md AIS-0007 status

**Approval Required:** Owner Directive — Majed authorised "you decide and fix" for Round 2
