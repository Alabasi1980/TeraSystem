# TASK-AI-D03 — Assistant Icon in Card + Open/Close Panel Logic

> **Status:** Draft → Approved  
> **Batch:** D-2 (Sequential after D02)  
> **Depends On:** TASK-AI-D02 (panel partial view exists)  
> **Phase:** AI Assistant — Phase D  

---

## 1. Objective

Add the assistant icon (💡) to each enabled card in the dashboard, and create the JavaScript file that opens/closes the Side Panel and tracks the currently selected card.

---

## 2. ClientAppPath

`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\`

---

## 3. Allowed Sources

- `src\WarehouseDashboard.Web\Pages\Index.cshtml` — card rendering loop
- `src\WarehouseDashboard.Web\Pages\Shared\_AssistantSidePanel.cshtml` — panel structure
- `src\WarehouseDashboard.Web\wwwroot\css\assistant-panel.css` — panel classes

---

## 4. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\wwwroot\js\assistant-panel.js` — **NEW**
2. `ClientAppPath\src\WarehouseDashboard.Web\Pages\Index.cshtml` — add icon HTML + JS reference
3. `ClientAppPath\src\WarehouseDashboard.Web\Pages\_Layout.cshtml` — add JS script tag

---

## 5. Technical Spec

### JavaScript: `assistant-panel.js`

```javascript
// Global state
let currentCardId = null;
let currentChartType = null;

function openAssistantPanel(cardId, cardTitle, chartType) {
    currentCardId = cardId;
    currentChartType = chartType;
    document.getElementById('assistant-card-title').textContent = cardTitle;
    document.getElementById('ai-assistant-panel').classList.add('open');
    document.getElementById('assistant-overlay').style.display = 'block';
    document.getElementById('ai-assistant-panel').setAttribute('aria-hidden', 'false');
    // Reset panel content
    document.getElementById('assistant-answer').innerHTML = '<p class="assistant-placeholder">اختر أحد خياري التحليل.</p>';
    document.getElementById('assistant-scope').textContent = '';
    document.getElementById('assistant-deepen-area').style.display = 'none';
    document.getElementById('assistant-status').textContent = '';
}

function closeAssistantPanel() {
    document.getElementById('ai-assistant-panel').classList.remove('open');
    document.getElementById('assistant-overlay').style.display = 'none';
    document.getElementById('ai-assistant-panel').setAttribute('aria-hidden', 'true');
    currentCardId = null;
}
```

### Card HTML (in Index.cshtml)

In the card header area, add an icon button AFTER the card title:

```html
<button class="card-assistant-btn" 
        onclick="openAssistantPanel(@card.Id, '@card.Title', '@card.ChartType')"
        title="تحليل البطاقة"
        aria-label="فتح مساعد تحليل البطاقة">
    💡
</button>
```

Only show the button if `card.AssistantEnabled`.

Add minimal CSS for the icon (inline in the card or in blue-theme.css):
```css
.card-assistant-btn {
    background: none;
    border: none;
    cursor: pointer;
    font-size: 16px;
    padding: 2px 6px;
    border-radius: 4px;
    opacity: 0.6;
    transition: opacity 0.2s;
}
.card-assistant-btn:hover { opacity: 1; background: rgba(37,99,235,0.08); }
```

### Layout: `_Layout.cshtml`

Add before closing `</body>`:
```html
<script src="~/js/assistant-panel.js"></script>
```

---

## 6. Forbidden

- No AJAX/API calls in this task (wiring comes in D04)
- No modification to card data rendering
- Icon must only appear on enabled cards

---

## 7. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | Assistant icon appears on cards with AssistantEnabled=true |
| AC-2 | Icon hidden on cards with AssistantEnabled=false |
| AC-3 | Clicking icon opens the panel with card title shown |
| AC-4 | Clicking close/overlay closes the panel |
| AC-5 | Panel content resets when opening for a new card |
| AC-6 | Build succeeds |

---

## 8. Handback

1. Files created/modified
2. Full JS file content
3. Icon integration location in Index.cshtml
4. Build result
