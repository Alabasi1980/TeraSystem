# AGENT_GAPS_LOG.md — TeraQuotation

## GAP-UI-001 — Missing project-level UI/UX guidelines file

- **Date:** 2026-07-14
- **Agent:** UI Designer / TeraAgent
- **Context:** APP-TeraQuotation UI/UX rescue review before redesign implementation.
- **Observation:** `project-preparation/28_UI_UX_GUIDELINES.md` is referenced by `07_SCREENS_AND_UI_STRUCTURE.md`, but the file is missing in the client application project-preparation path.
- **Impact:** UI implementation risks continuing with invented styles, hardcoded colors, inconsistent spacing, and weak acceptance criteria.
- **Recommended Action:** Create and approve `28_UI_UX_GUIDELINES.md` before any UI implementation task.
- **Severity:** High
- **Status:** Open

## GAP-UI-002 — Current UI implementation lacks centralized design tokens

- **Date:** 2026-07-14
- **Agent:** UI Designer / TeraAgent
- **Context:** XAML review of APP-TeraQuotation screens.
- **Observation:** Styling appears hardcoded across XAML files instead of governed by a shared ResourceDictionary or project-level UI rules.
- **Impact:** Visual inconsistency, difficult maintenance, and high risk of future UI drift.
- **Recommended Action:** Define Mini Design System and implement WPF design resources before broad screen polishing.
- **Severity:** Medium
- **Status:** Open

## GAP-UI-003 — MainWindow implementation does not match documented shell concept

- **Date:** 2026-07-14
- **Agent:** UI Designer / TeraAgent
- **Context:** Comparison between `07_SCREENS_AND_UI_STRUCTURE.md` and current MainWindow UI.
- **Observation:** The preparation document describes a Sidebar + Content Frame shell, while current MainWindow appears to contain only a Frame with ad-hoc navigation inside screens.
- **Impact:** Navigation feels inconsistent and contributes to weak UX structure.
- **Recommended Action:** Decide whether to implement a real RTL shell/sidebar or revise the documented design direction before engineering.
- **Severity:** Medium
- **Status:** Open
