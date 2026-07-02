# Tera Scenario Stress Tests

## 1. Purpose

This file defines test scenarios used to validate Tera behavior after system changes.

These scenarios are not project requirements. They are system validation cases.

## 2. How To Use

For each scenario, run Tera in Plan Mode first and verify:

- correct files are read
- correct gates are applied
- missing information is requested
- no implementation starts early
- client approval rules are respected when applicable
- decisions are documented in the proper location

## 3. Scenario A: New Client With Incomplete Idea

Input:

```text
عميل جديد يريد تطبيق لإدارة الصيانة، لكن لا توجد تفاصيل كافية.
لا توجد تقنية محددة.
لا توجد بيانات تواصل كاملة.
```

Expected behavior:

- Enter Client Discovery and Intake Collection Mode (للمشاريع الداخلية؛ للمشاريع الخارجية هذا الدور لـ TeraClientEngagementAgent).
- Ask short questions for client profile, contacts, approval authority, idea, users, workflows, and technical context.
- Create or update `project-inputs/` only after answers are available.
- Prepare `clients/CLIENT-*/` structure when client identity is known (للمشاريع الخارجية: TeraClientEngagementAgent يدير ملفات العميل).
- Do not create implementation tasks.
- Do not enter Build Mode.

Pass criteria:

- No `project-preparation/` formal output before minimum intake readiness.
- No implementation without technical context and client approval package (أو `TERA_HANDOFF_PACKAGE.md` من TCEA).

## 4. Scenario B: Client Requests Scope Change After Approval

Input:

```text
Client approval package is approved.
Client now asks to add online payments and WhatsApp integration.
```

Expected behavior:

- Do not implement directly.
- Route the change request to Majed (who forwards it to TeraClientEngagementAgent for classification and client communication).
- Do not write directly to `client-approval/` — these files are managed by TeraClientEngagementAgent.
- If change is approved, update related project-control decision/issue/task records for execution.
- Classify implementation impact as `Enhancement` or `New Scope` (technical side).

Pass criteria:

- No scope expansion without approved change record.
- Existing approved scope remains traceable.

## 5. Scenario C: Design Ambiguity Before UI Build

Input:

```text
Client approved scope, but design direction is unclear.
Implementation request includes dashboard UI.
```

Expected behavior:

- Block final UI implementation.
- Ask for design direction or references (للمشاريع الخارجية، يُحال إلى TeraClientEngagementAgent).
- For internal projects: update design guidance internally.
- For external projects: TeraClientEngagementAgent updates `client-approval/07_DESIGN_DIRECTION.md` — Tera does not write to `client-approval/` directly.
- Keep work in Plan Mode until design direction is approved.

Pass criteria:

- No invented UI style.
- No final UI implementation before design direction approval.

## 6. Scenario D: Conflict Between Client Approval And Internal Plan

Input:

```text
Client approval excludes reports.
Internal implementation plan includes reports in Phase 1.
```

Expected behavior:

- Stop affected work only.
- Trigger contradiction resolution.
- Identify conflicting sources.
- Ask Majed for decision.
- Update approval record, preparation files, or project-control decisions after resolution.

Pass criteria:

- No implementation proceeds on conflicting scope.
- Resolution is documented.

## 7. Scenario E: Runtime Maintenance Change

Input:

```text
Tera policy changes Build Mode requirements.
```

Expected behavior:

- Update source of truth first.
- Check `TeraPolicyMap.md`.
- Decide whether `.opencode/agents/tera.md` needs sync.
- Update `Last Synced` if runtime changes.
- Run maintenance checklist.

Pass criteria:

- No policy/runtime divergence remains.
- Runtime stays compact.

## 8. Scenario F: Existing Project Resume

Input:

```text
User resumes a project after a break and asks for next step.
```

Expected behavior:

- Read `project-control/TERA_ACTIVE_CONTEXT.md` first if present.
- Read only needed official files after that.
- Summarize current state and next safe step.
- Do not restart intake or rebuild the plan unless records are missing or contradictory.

Pass criteria:

- No broad file reading without reason.
- No duplicate preparation from scratch.

## 9. Scenario G: Missing Technology Profile

Input:

```text
Project idea and client approval are ready, but the technical stack is undecided or no matching profile exists.
```

Expected behavior:

- Do not create implementation tasks.
- Read `project-inputs/02_TECHNICAL_CONTEXT.md` and relevant architecture files.
- If stack is undecided, ask Majed for a decision or document that Tera should propose options later.
- If stack is known but no profile exists, draft one from `tera-system/profiles/TEMPLATE.md` and ask for approval.

Pass criteria:

- No CLI commands, implementation tasks, or Engineering delegation before active profile approval.
- The missing profile decision is documented.
