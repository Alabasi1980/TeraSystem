# TASK-AI-D02 — Design Research References

## Research Date: 2026-07-21
## Research Scope: AI Assistant Side Panel (Sliding Panel Pattern)

---

## Reference 1: shadcn/ui Sheet Component Pattern
- **Source:** https://ui.shadcn.com/docs/components/sheet
- **What I'm borrowing:**
  - Slide-in from right with backdrop overlay
  - Smooth 0.3s transition with ease curve
  - Fixed positioning, full viewport height
  - Close button in header with X icon
  - Body scrolls independently with overflow-y: auto
- **What I'm avoiding:**
  - The two-panel "sides" variant (we only need right side)
  - The complex keyboard navigation (JS comes in D03)

## Reference 2: Copilot / ChatGPT Side Panel
- **Source:** https://dribbble.com/search/ai-side-panel (general pattern)
- **What I'm borrowing:**
  - Header with gradient/accent color to distinguish from page content
  - Mode toggle buttons (explain vs deep analysis)
  - Placeholder text in answer area
  - Loading state with spinner/pulse animation
  - Footer with close button for clear dismissal path
- **What I'm avoiding:**
  - Chat input at bottom (this is not a chat — it's an analysis panel)
  - Message bubbles / conversation UI
  - Overly complex floating action buttons

## Reference 3: Linear App Issue Detail Panel
- **Source:** https://linear.app (design inspiration)
- **What I'm borrowing:**
  - Clean white card surfaces within panel
  - Thin border separators
  - Subtle hover effects on buttons
  - Compact, information-dense layout
  - Minimal scrollbar styling
- **What I'm avoiding:**
  - Multi-tab complexity
  - Command palette integration

## Reference 4: Existing WarehouseDashboard Modal Pattern
- **Source:** blue-theme.css / Index.cshtml (wd-modal)
- **What I'm borrowing:**
  - Gradient header: `linear-gradient(135deg, var(--c-primary), var(--c-primary-strong))`
  - Decorative ::before/::after circles in header
  - Consistent border-radius (var(--radius-lg))
  - Same shadow tokens (var(--shadow-xl))
  - Same animation easing: cubic-bezier(0.22, 1, 0.36, 1)
  - Same font family: var(--font-ar)
  - Same color tokens from blue-theme.css for consistency
- **What I'm avoiding:**
  - Modal centering (panel is right-anchored, not modal)
  - Breadcrumb bar (not needed for assistant)
  - Heavy modal footer badges

## Reference 5: Awwwards AI Websites — Common Panel Patterns
- **Source:** https://www.awwwards.com/websites/ai-assistant/
- **What I'm borrowing:**
  - RTL-friendly layout with logical properties
  - Responsive full-width on mobile (< 768px)
  - Overlay with subtle blur (backdrop-filter)
  - Fade-in animation for content
- **What I'm avoiding:**
  - Overly branded/animated headers
  - Non-functional decorative elements

---

## Design Decisions Summary

| Decision | Rationale |
|----------|-----------|
| Use CSS custom properties from blue-theme.css | Consistency with existing dashboard theme, supports dark mode |
| Blue gradient header (matching wd-modal) | Visual consistency across the app |
| RTL with logical properties | Arabic-first design, proper user experience |
| Sliding from right (not modal) | Keeps dashboard visible, non-blocking context |
| 380px width | Narrow enough to not dominate, wide enough for readable text |
| 100% width on mobile | Responsive best practice for small viewports |
| Overlay with rgba + backdrop-filter | Focus user on panel while maintaining spatial context |
| Thin custom scrollbar | Polish detail that elevates perceived quality |
