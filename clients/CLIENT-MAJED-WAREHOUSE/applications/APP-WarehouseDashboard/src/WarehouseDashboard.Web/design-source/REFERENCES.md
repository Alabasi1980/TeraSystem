# REFERENCES — BATCH-3A: Keyboard HUD + Sync Status Badge

## Research Sources

### 1. Linear App — Keyboard Shortcuts Modal
- **Source:** Linear's `?` keyboard shortcut overlay
- **Inspiration:** Clean overlay with backdrop blur, kbd elements styled as mini-chips, grouped shortcut categories, dismiss on Esc
- **What I took:** The overlay + blur approach, the kbd styling (monospace-like chip on light bg), the Esc to dismiss
- **What I avoided:** Multi-column layout (ours is a simple single-column list for readability in RTL)

### 2. GitHub — Keyboard Shortcuts Reference
- **Source:** GitHub's `?` shortcut modal (`Shift+/`)
- **Inspiration:** Modal with header/title, close button, table-like rows of shortcuts, slide-up animation
- **What I took:** The header + close button structure, the row pattern (kbd + description), slide-up entrance
- **What I avoided:** Table markup (used flex rows for better RTL adaptation)

### 3. Dribbble — Sync Status Indicators Collection
- **Source:** Dribbble search for "sync status" + "connection indicator dashboard"
- **Keywords searched:** sync status badge, live indicator, connection dot
- **Inspiration:** Colored dot + label pattern, pulsing animation for active states, pill-shaped badges
- **What I took:** The dot + text pattern, the pulse animation for running state, the pill badge shape
- **What I avoided:** Overly complex multi-color indicators (kept it to 3 states: idle/running/error)

### 4. Notion — Sync Status Indicator
- **Source:** Notion's toolbar sync indicator ("All changes saved" / "Saving..." / "Offline")
- **Inspiration:** Minimal dot + status text, auto-dismiss when idle, color-coded states
- **What I took:** The minimal footprint (fits in a topbar), the three-state model, the polling approach
- **What I avoided:** Toast-based approach (kept it as a persistent badge for at-a-glance visibility)

### 5. Vercel Dashboard — Status Badge Pattern
- **Source:** Vercel's deployment status badges
- **Inspiration:** Small pills with colored dots, consistent with dark-on-light header, semantic colors
- **What I took:** The pill shape with dot, the use of green/red/gray semantics, the typographic scale
- **What I avoided:** Badge-only without text (ours includes text for accessibility in RTL)

## Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Keyboard HUD trigger | `?` key | Universal convention (GitHub, Linear, Gmail, etc.) |
| HUD animation | fadeIn overlay + slideUp panel | Feels natural, draws eye to panel |
| HUD overlay | backdrop-filter blur | Keeps context visible, modern feel |
| HUD kbd styling | Mini chip with border + shadow | Clearly distinguishable from description text |
| Navigation shortcuts | Single-letter keys (C, S, L, Q) | Quick muscle-memory for power users |
| Sync status polling | 15-second interval | Balance between freshness and network requests |
| Sync status states | Idle / Running / Error | Covers all meaningful engine states |
| Sync status placement | Inside `.wd-topbar__actions` | Consistent with connection indicator, always visible |
| Sync status colors | Matches existing success/error/muted tokens | Reuses established semantic palette |
| Badge style | Semi-transparent white bg (for dark topbar) | Matches the connection indicator in the same topbar |
