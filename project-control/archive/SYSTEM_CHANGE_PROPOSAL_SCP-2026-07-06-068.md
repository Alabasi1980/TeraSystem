## إضافة تصنيف Hard Block ⛔ vs Soft Uncertainty ⚠️

Title:
Add Block Classification — Hard Block vs Soft Uncertainty — across TCEA protocols and gates

Request Type:
Owner improvement request based on external audit finding

Problem:
The TCEA file has multiple stop/block conditions (in A.6.1, A.6.2, B.1–B.7) but treats all stops as uniform "توقف إجباري". This causes two problems:
1. Weaker models stop completely even when they could safely continue discovery elsewhere
2. Models lack a framework to distinguish "must stop everything" vs "stop only pricing/handoff"

Evidence:
External audit finding #6 — "العميل مضبوط جداً على التوقف أكثر من الاستمرار المنضبط. أضف تمييزًا صريحًا بين Hard Block و Soft Uncertainty."

Affected Files:
- `.opencode/agents/tera-client-engagement.md`
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
1. **Add A.6.6 Block Classification** — a new subsection defining Hard Block ⛔ vs Soft Uncertainty ⚠️ with a reference table and handling guidelines
2. **Update A.0 "متى تتوقف فوراً" row** — to mention both types
3. **Tag A.6.1 Self-Check critical rule** as Hard Block ⛔
4. **Tag A.6.2 Uncertainty Protocol 3 cases** as Soft Uncertainty ⚠️
5. **Tag gate blocking conditions** in B.1, B.3, B.4, B.7 as Hard Block ⛔ (the gates already prevent progression)

Why This Is Necessary:
Gives models a clear operational distinction: "I can keep exploring some things (Soft) but I must stop entirely for others (Hard)." Reduces unnecessary full stops during discovery while maintaining strict control over pricing/handoff.

Rejected Alternatives:
- Merge into Master Rules MR4: MR4 is about stopping for uncertainty — classification is about what kind of stop
- Create separate file: Anti-Bloat — no new file needed
- Only document in A.6 without tagging gates: tagging gates is essential for model compliance

Anti-Bloat Check:
- Problem: All stops treated equally; model over-stops or under-stops
- Why not edit existing file? We are editing the existing file
- Why not use existing agent? No new agent needed
- Does this reduce complexity? Yes — clearer operational distinction
- Token impact? ~30 lines added
- Smaller way? This is the minimum viable: classification + key tags

Risk:
Low — addition only; no change to existing rules, permissions, or gates.

Rollback Plan:
1. Revert `.opencode/agents/tera-client-engagement.md`
2. Revert `project-control/SYSTEM_EVOLUTION_LOG.md`
3. Delete this proposal file

Approval Required:
Approved by Majed in-session on 2026-07-06
