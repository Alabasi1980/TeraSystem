# AGENT_GAPS_LOG.md — WarehouseDashboard

## GAP-2026-07-20-UI-REFERENCES-TARGET

- **Date:** 2026-07-20
- **Reported By:** ui-designer via TeraAgent
- **Related Task:** TASK-CARD-KPI-SMALL-COMPOSE-001
- **Severity:** Low
- **Observation:** UI tasks can require recording design/research references, but focused UI implementation delegations may intentionally restrict Allowed Write Targets to only the code file.
- **Evidence:** ui-designer reviewed dashboard reference sources but could not write `design-source/REFERENCES.md` because the task allowed only `Index.cshtml`.
- **Impact:** Minor governance friction; agents may either skip reference documentation or violate Allowed Write Targets unless Tera explicitly includes a documentation target.
- **Recommended Action:** For future design-sensitive UI tasks, Tera should either include an explicit documentation write target for references or state that references are to be reported in handback only.
- **Status:** Open

## GAP-2026-07-20-OUT-OF-TARGET-REFERENCE

- **Date:** 2026-07-20
- **Reported By:** TeraAgent
- **Related Task:** TASK-CARD-KPI-S-TOTALS-2ROWS-001
- **Severity:** Medium
- **Observation:** ui-designer wrote `design-source/REFERENCES.md` despite the delegation limiting Allowed Write Targets to `Index.cshtml` only.
- **Evidence:** Handback listed `design-source/REFERENCES.md` under changed files; Tera verified the file exists.
- **Impact:** Governance/traceability risk. Runtime code fix is valid, but out-of-target documentation artifacts can create uncontrolled project changes.
- **Recommended Action:** Reinforce in UI delegations that research references must be returned in handback only unless explicitly listed as an Allowed Write Target. Ask Majed whether to keep or remove the out-of-target file.
- **Status:** Open — awaiting Majed decision on artifact retention/removal.
