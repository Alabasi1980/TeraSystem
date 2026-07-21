# TASK-AI-D02 — Assistant Side Panel Partial View + CSS

> **Status:** Draft → Approved  
> **Batch:** D-1 (Parallel with D01)  
> **Depends On:** None (pure UI, uses known API pattern)  
> **Phase:** AI Assistant — Phase D  

---

## 1. Objective

Create the Side Panel partial view and its CSS for the AI assistant. The panel slides in from the right side (RTL-friendly), contains mode buttons, answer area, and status indicators.

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources

- `src\WarehouseDashboard.Web\Pages\Index.cshtml` — card rendering pattern
- `src\WarehouseDashboard.Web\wwwroot\css\blue-theme.css` — existing styles
- `src\WarehouseDashboard.Web\Pages\_Layout.cshtml` — layout structure

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Pages\Shared\_AssistantSidePanel.cshtml` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\wwwroot\css\assistant-panel.css` — **NEW**
3. `ClientAppPath\src\WarehouseDashboard.Web\Pages\_Layout.cshtml` — add CSS link (1 line)

---

## 5. Technical Spec

### Partial View: `_AssistantSidePanel.cshtml`

Structure:
```html
<div id="ai-assistant-panel" class="assistant-panel" aria-hidden="true">
    <div class="assistant-panel-header">
        <h3>💡 مساعد البطاقة</h3>
        <span id="assistant-card-title"></span>
        <button class="assistant-close" aria-label="إغلاق">&times;</button>
    </div>
    <div class="assistant-panel-body">
        <div class="assistant-mode-buttons">
            <button id="btn-explain" class="assistant-btn">📌 شرح البطاقة</button>
            <button id="btn-deep" class="assistant-btn">🔍 شرح عميق</button>
        </div>
        <div id="assistant-scope" class="assistant-scope"></div>
        <div id="assistant-answer" class="assistant-answer"></div>
        <div id="assistant-deepen-area" class="assistant-deepen" style="display:none;">
            <button id="btn-deepen" class="assistant-btn">تعمق أكثر</button>
        </div>
        <div id="assistant-status" class="assistant-status"></div>
    </div>
    <div class="assistant-panel-footer">
        <button class="assistant-close-btn">إغلاق</button>
    </div>
</div>
```

### CSS: `assistant-panel.css`

Requirements:
- Fixed position, right side, full height, width ~380px
- RTL-friendly: right-to-left, text-align: right
- Slide-in animation from right (translateX)
- Blue theme: follow existing color palette (#2563EB primary, #F8FAFC bg, #FFFFFF cards)
- Z-index: high enough to overlay cards but below modals
- Responsive: on mobile (< 768px), full width
- Scrollable body if content is long
- Transition: 0.3s ease for open/close

---

## 6. Forbidden

- No JavaScript in this task (JS comes in D03)
- No API calls
- No Razor PageModel — pure partial view
- No modification of card rendering

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | Partial view has all required sections (header, body, footer) |
| AC-2 | CSS: right-side panel, 380px, RTL, slide animation |
| AC-3 | CSS: blue theme colors match existing dashboard |
| AC-4 | CSS: responsive (full-width on mobile) |
| AC-5 | `_Layout.cshtml` includes assisant-panel.css |
| AC-6 | Build succeeds (no compilation errors from .cshtml) |

---

## 8. Handback

1. Files created/modified
2. Full CSS content
3. Screenshot/description of panel appearance
4. Build result
