## إضافة Next Action إلزامي إلى Consultation Response Protocol

Title:
Add mandatory Next Action field to Consultation Response Protocol (A.6.3)

Request Type:
Owner improvement request based on external audit finding

Problem:
The Consultation Response template has 5 sections (understanding, risks, suggestions, questions, phasing) but ends without an explicit action item. This leaves the model in a "response-only" mode — it may produce a beautiful consultation reply but then hesitate on what to do next, or default to waiting without stating the expected follow-up.

Evidence:
External audit finding #8 — "Consultation Response ممتاز لكنه يحتاج طبقة تنفيذية أوضح. ينقصه سطر حاسم يربط الرد بالفعل التالي."

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
1. Add a 6th section "الخطوة التالية (Next Action)" to the Consultation Response template with 6 predefined options as checkboxes (Await confirmation, Update CLIENT_INTAKE.md, Prepare DISCOVERY_COVERAGE_SUMMARY.md draft, Prepare DRAFT_QUOTATION.md, Prepare TERA_HANDOFF_PACKAGE.md, STOP pending uncertainty)
2. Update the commitment line from "الأقسام الخمسة" to "الأقسام الستة"
3. Add rule #8 to the strict rules: "اختر Next Action واحداً فقط — لا تترك الحقل فارغاً ولا تختار أكثر من خيار"
4. Update the A.4 workflow reference to mention Next Action

Why This Is Necessary:
Reduces model hesitation after producing a response. The model knows exactly what to do next, and Majed knows what to expect. Without it, the model may "reply beautifully but go nowhere."

Rejected Alternatives:
- Free-text Next Action: predefined options are clearer and prevent vague answers
- Merge into existing sections: would dilute the actionable nature
- Add as optional: defeats the purpose — must be mandatory to prevent omission

Anti-Bloat Check:
- Problem: Consultation Response has no explicit next-action linkage
- Why not edit existing file? We are editing the existing file
- Why not use existing agent? No new agent needed
- Does this reduce complexity? Yes — makes the model's expected next step explicit
- Token impact? ~10 lines added
- Smaller way? This is the minimally viable addition

Risk:
Low — addition only; no change to existing rules, permissions, or gates.

Rollback Plan:
1. Revert `.opencode/agents/tera-client-engagement.md`
2. Revert `project-control/SYSTEM_EVOLUTION_LOG.md`
3. Delete this proposal file

Approval Required:
Approved by Majed in-session on 2026-07-06
