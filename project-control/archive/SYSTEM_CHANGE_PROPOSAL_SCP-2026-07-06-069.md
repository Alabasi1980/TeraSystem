## SCP-069 — Runtime Load Order تبسيط (Required Now / Required If Triggered / Reference Only)

Title:
Simplify C.4 Runtime Load Order into 3 categories: Required Now → Required If Triggered → Reference Only

Request Type:
Owner improvement request based on external audit finding #5

Problem:
C.4 currently has 6 separate trigger-based tables (Session start, Discovery start, First Pricing, DRAFT_QUOTATION, DISCOVERY_COVERAGE_SUMMARY, TERA_HANDOFF_PACKAGE). This structure causes:
1. The model must scan 6 tables to know what to load — cognitive overhead
2. Some files appear in multiple tables (e.g., letterhead template appears in Pricing table AND Tools table)
3. No explicit category tells the model "this is critical path, load immediately vs load only if needed"
4. The distinction between "must load every session" vs "load once in a lifetime" vs "load on demand" is implicit

Evidence:
External audit finding #5: "الملفات كثيرة. رتبها حسب الحاجة: Required Now (اقرأها فوراً), Required If Triggered (عند شرط معين), Reference Only (لا تقرأها افتراضياً استدعها عند الحاجة)."

Affected Files:
- `.opencode/agents/tera-client-engagement.md` (C.4 Runtime Load Order, A.0 SESSION_MINIMUM_RUNTIME)
- `project-control/SYSTEM_EVOLUTION_LOG.md`

Proposed Change:
Three-part change:

**Part 1 — Add category labels to A.0 table:**
Add a second column to the A.0 table showing the load category (🟢 Required Now / 🟡 Required If Triggered / 🔵 Reference Only) for each item in the Session Minimum Runtime.

**Part 2 — Simplify C.4 into 3 categories:**

```
🟢 Required Now — اقرأها فوراً عند دخول السياق (كل Session)
  - Session Start: TERA_AGENT_CONDUCT.md + هذا الملف
  - Discovery Start: TeraApplicationQuestionBank.md + TeraClientPolicy.md
  - Pricing Start: TeraPricingPolicy.md + PRICING_SCORECARD_APPLICATION_MAWTHOOQ.md

🟡 Required If Triggered — اقرأها فقط عند حصول المسبب
  - TRAINING_GUIDE_TCEA.md (إذا أول Session تسعيرية عمرياً)
  - TERA_RUNTIME_TEMPLATES.md §35 (عند إنتاج DISCOVERY_COVERAGE_SUMMARY.md)
  - TeraPricingCalculator.xlsx (عند Level 2)
  - letterhead-master-fixed-print.html (عند مراسلة رسمية)
  - handover/ templates (عند TERA_HANDOFF_PACKAGE.md)

🔵 Reference Only — لا تقرأها افتراضياً، استدعها عند الحاجة
  - TeraPricingPolicy.md (إذا عدّل النطاق بعد التسعير)
  - TRAINING_GUIDE_TCEA.md (عند تحذير Proportion Check بعد أول Session)
  - letterhead-master-fixed-print.html (إذا احتجت تأكيد التنسيق)
```

**Part 3 — Add runtime decision rule:**
```text
قاعدة التحميل:
1. Required Now → اقرأها قبل البدء في السياق
2. Required If Triggered → اقرأها عند حصول الشرط فقط
3. Reference Only → لا تقرأها. استدعها فقط إذا احتجت تأكيد معلومة محددة
```

Why This Is Necessary:
Reduces cognitive load on the model from scanning 6 tables to scanning 3 categories. Makes it explicit which files are critical path vs optional. Direct token savings from not loading Reference Only files by default. Audit finding #5 explicitly requested this.

Rejected Alternatives:
- Keep 6 tables + add category labels: Adds complexity without simplification
- Merge all into one flat table: Loses trigger context
- Remove C.4 entirely: Would lose the load order, making model behavior non-deterministic

Anti-Bloat Check:
- Problem: 6 tables with 81 file references cause cognitive overhead
- Why not edit existing file? We ARE editing existing file — simplifying C.4
- Why not use existing agent? No new agent needed
- Does this reduce complexity? Yes — 3 tables instead of 6
- Token impact: Slightly reduces overall lines (~10 fewer)
- Smaller way: Cannot — this IS the minimal simplification

Risk:
Low — restructure of existing content, no new rules, no permission changes.

Rollback Plan:
1. git checkout -- .opencode/agents/tera-client-engagement.md
2. git checkout -- project-control/SYSTEM_EVOLUTION_LOG.md
3. Remove-Item -LiteralPath "project-control/SYSTEM_CHANGE_PROPOSAL_SCP-2026-07-06-069.md"

Approval Required:
Yes — Majed approval before implementation
