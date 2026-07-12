## إضافة SESSION_MINIMUM_RUNTIME — الحد الأدنى للجلسة في أعلى الملف

Title:
Add A.0 SESSION_MINIMUM_RUNTIME compact section at the very top of TCEA

Request Type:
Owner improvement request based on external audit finding

Problem:
The TCEA file starts directly with identity and then jumps to A.1–A.8. Weaker models must read 200+ lines of detailed protocols before reaching the first operating context. There is no single "pocket reference" of the absolute minimum the model must remember. This causes:
1. Models to forget critical boundaries mid-session
2. Models to proceed without confirming their current operating mode
3. Increased cognitive load — no quick return point when confused

Evidence:
External audit finding #5 and #1 — "ترتيب التحميل الحالي جيد نظرياً لكنه ثقيل عملياً" and "تقليل الحمل التشغيلي على النموذج". The file has grown to 900+ lines; a compact entry point is needed.

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
Add a new section **A.0 SESSION_MINIMUM_RUNTIME** between the CONDUCT GATE separator and `# A — Runtime Core`, containing a single table with:
1. **Identity** — who you are in 1 line
2. **Current mode** — pick A/B/C/D/E
3. **Never do** — 4 critical prohibitions
4. **When to stop** — 3 triggers referencing MR2/MR3/MR4
5. **When to ask Majed confirmation** — 4 key decision points
6. **When pricing is allowed** — 3 preconditions
7. **When handoff is allowed** — 4 preconditions

Why This Is Necessary:
Gives weaker models a one-stop reference they can re-read when confused, reducing the risk of silent deviations. Strengthens the A.6.0 Master Rules by providing a quick-access entry point to the same concepts.

Rejected Alternatives:
- Merge into existing A.1 identity section: A.1 is text-heavy; defeats the purpose of a compact reference.
- Create a separate standalone file (`SESSION_RUNTIME.md`): Anti-Bloat — unnecessary new file.
- Rely on existing Load Order (C.4): C.4 is a reference appendix, not an immediate quick-reference.

Anti-Bloat Check:
- Problem: 900+ line file with no compact entry point for critical session rules.
- Why not edit existing file? We are editing the existing file — no new file created.
- Why not use existing agent? No new agent needed.
- Does this reduce complexity? Yes — provides a single return point when confused.
- Token impact? ~20 lines added; negligible increase vs. the 900-line file.
- Smaller way? This is already the minimally viable compact table (7 rows).

Risk:
Low — section addition only; no change to any rule, gate, permission, or authority.

Rollback Plan:
1. Revert `.opencode/agents/tera-client-engagement.md`
2. Revert `project-control/SYSTEM_EVOLUTION_LOG.md`
3. Delete this proposal file

Approval Required:
Approved by Majed in-session on 2026-07-06
