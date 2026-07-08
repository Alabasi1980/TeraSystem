## SCP-070 — إضافة Confidence Threshold Table (مقياس الثقة الصريح)

Title:
Add explicit Confidence Threshold framework — self-assessment scale (Low/Medium/High) with decision matrix

Request Type:
Owner improvement request based on external audit finding #9

Problem:
The TCEA file has a "الخطورة: Low/Medium/High" field in A.6.1 Self-Check, but this is about impact severity if wrong, NOT about the model's own confidence. The model has no explicit framework to self-assess "how sure am I?" before proceeding. This causes:
1. The model proceeds with low-confidence information that isn't yet High-severity, creating subtle errors
2. No graded response — either full stop (Hard Block) or continue (Soft Uncertainty), missing a middle ground
3. No explicit mapping between confidence level and required action

Evidence:
External audit finding #9: "أضف مقياس ثقة صريح (Low/Medium/High) للنموذج ليقيم نفسه قبل أن يبدأ أو يستمر — ليس فقط عند الخطأ."

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
**Add A.6.7 Confidence Threshold — مقياس الثقة الذاتية** (after A.6.6 Block Classification)

Content:

1. **Confidence Self-Assessment Table:**

| المستوى | المعنى | متى تستخدمه | السلوك المطلوب |
|:-------:|--------|-------------|----------------|
| **High** 🟢 | المعلومة أكيدة — مصدرها Majed صراحة أو وثيقة معتمدة | `[Confirmed by Majed]` فقط | تابع طبيعي — لا حاجة للتوقف |
| **Medium** 🟡 | المعلومة مرجحة لكن غير مؤكدة — أو مصدرها Research مع تأكيد جزئي | `[Research Hint]`, `[Assumption]` مع Medium/Low خطورة | تابع لكن سجّل UNCERTAINTY_NOTICE وارفعها لـ Majed في أول فرصة مناسبة |
| **Low** 🔴 | المعلومة غير مؤكدة — Inference فقط، أو `[Unresolved]`, أو High خطورة مع `[Assumption]` | `[Unresolved]`, `[Assumption]` مع High خطورة | ⛔ Hard Block إذا كان عالي الخطورة — ⚠️ Soft Uncertainty إذا كان منخفض الخطورة |

2. **Decision Matrix: Confidence × Source Tag → Required Action**

| Source Tag \ Confidence | 🟢 High | 🟡 Medium | 🔴 Low |
|:-----------------------:|:-------:|:---------:|:------:|
| `[Confirmed by Majed]` | ✅ تابع مباشرة | — | — |
| `[Research Hint]` | — | ⚠️ سجّل Notice وتابع (إذا Low-Medium خطورة) | ⛔ توقف (إذا High خطورة) |
| `[Assumption]` | — | ⚠️ سجّل Notice وتابع (إذا Low-Medium خطورة) | ⛔ Hard Block (إذا High خطورة) |
| `[Unresolved]` | — | ⚠️ سجّل Notice وتابع (إذا Low-Medium خطورة) | ⛔ Hard Block (إذا High خطورة) |

3. **متى تعيد تقييم الثقة:**
   - بعد كل رد من Majed — قد ترتفع الثقة (Confirmed) أو تنخفض (تعديل المعلومة)
   - بعد Websearch — Medium فقط، لا يصبح High دون تأكيد Majed
   - بعد تغير النطاق — أعد تقييم ثقة جميع العناصر المتأثرة

Why This Is Necessary:
Gives the model a proactive self-assessment tool (not just reactive stopping). Bridges the gap between the existing Source Tags (A.6.4) and the Block Classification (A.6.6). Prevents subtle errors from low-confidence non-High-severity information.

Rejected Alternatives:
- Merge into A.6.1 Self-Check: Would overload the Self-Check which is about Domain completion, not confidence self-assessment
- Merge into A.6.6 Block Classification: Block Classification is about what type of stop; Confidence is about when to use which type
- Create separate section in C (Operations): Confidence is a runtime core concept, belongs in A

Anti-Bloat Check:
- Problem: Model lacks explicit self-confidence scale — proceeds with low-confidence info
- Why not edit existing file? We are editing existing file — adding A.6.7
- Why not use existing agent? No new agent needed
- Does this reduce complexity? Yes — gives clear decision rules instead of implicit judgment
- Token impact: ~40 lines (one table + decision matrix + re-evaluation rules)
- Smaller way: This IS minimal — one table + one matrix + one re-evaluation rule

Risk:
Low — additive only; does not change existing rules, permissions, or gates. Complements A.6.1 and A.6.2.

Rollback Plan:
1. git checkout -- .opencode/agents/tera-client-engagement.md
2. git checkout -- project-control/SYSTEM_EVOLUTION_LOG.md
3. Remove-Item -LiteralPath "project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-06-070.md"

Approval Required:
Yes — Majed approval before implementation
