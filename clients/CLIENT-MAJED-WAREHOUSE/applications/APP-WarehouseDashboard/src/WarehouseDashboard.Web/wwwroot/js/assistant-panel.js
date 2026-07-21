// AI Assistant Panel — State & Controls
let assistantCurrentCardId = null;

// Track depth for deep analysis
let assistantCurrentDepth = 1;
let assistantCurrentMode = null;
let assistantIsFullDataReached = false;
let assistantHasDeeperData = false;

function openAssistantPanel(cardId, cardTitle) {
    assistantCurrentCardId = cardId;
    
    // Set card title
    var titleEl = document.getElementById('assistant-card-title');
    if (titleEl) titleEl.textContent = cardTitle;

    var btnExplain = document.getElementById('btn-explain');
    var btnDeep = document.getElementById('btn-deep');
    
    // Show panel
    var panel = document.getElementById('ai-assistant-panel');
    var overlay = document.getElementById('assistant-overlay');
    if (panel) {
        panel.classList.add('open');
        panel.setAttribute('aria-hidden', 'false');
    }
    if (overlay) overlay.style.display = 'block';
    
    // Reset content
    var answer = document.getElementById('assistant-answer');
    var scope = document.getElementById('assistant-scope');
    var deepen = document.getElementById('assistant-deepen-area');
    var status = document.getElementById('assistant-status');
    if (answer) answer.innerHTML = '<p class="assistant-placeholder">اختر أحد خياري التحليل.</p>';
    if (scope) scope.textContent = '';
    if (deepen) deepen.style.display = 'none';
    if (status) status.textContent = '';
    
    // Reset full-data message
    var fullDataMsg = document.getElementById('assistant-full-data-msg');
    if (fullDataMsg) fullDataMsg.style.display = 'none';
    
    // Defensive: disable buttons if card not assistant-enabled
    var cardEl = document.querySelector('[data-card-id="' + cardId + '"]');
    var isEnabled = cardEl ? cardEl.getAttribute('data-assistant-enabled') !== 'false' : true;
    if (!isEnabled) {
        if (btnExplain) { btnExplain.disabled = true; btnExplain.title = 'المساعد غير مفعل لهذه البطاقة'; }
        if (btnDeep)    { btnDeep.disabled = true;    btnDeep.title = 'المساعد غير مفعل لهذه البطاقة'; }
        if (answer) answer.innerHTML = '<p class="assistant-error">عذراً، المساعد الذكي غير متوفر لهذه البطاقة.</p>';
        if (status) status.textContent = '';
        return;
    }
    
    // Enable mode buttons
    if (btnExplain) btnExplain.disabled = false;
    if (btnDeep) btnDeep.disabled = false;
}

function closeAssistantPanel() {
    var panel = document.getElementById('ai-assistant-panel');
    var overlay = document.getElementById('assistant-overlay');
    if (panel) {
        panel.classList.remove('open');
        panel.setAttribute('aria-hidden', 'true');
    }
    if (overlay) overlay.style.display = 'none';
    assistantCurrentCardId = null;
}

// =============================================================================
// TASK-AI-D04 — AJAX wiring: POST /api/card-insights/analyze
// =============================================================================

async function analyzeCard(mode) {
    if (!assistantCurrentCardId) return;

    assistantCurrentMode = mode;
    if (mode === 'deep') assistantCurrentDepth = 1;

    // Show loading state
    var answer = document.getElementById('assistant-answer');
    var status = document.getElementById('assistant-status');
    var deepen = document.getElementById('assistant-deepen-area');
    var scope = document.getElementById('assistant-scope');

    answer.classList.add('assistant-answer--loading');
    answer.innerHTML = '';
    status.textContent = 'جاري التحليل...';
    status.className = 'assistant-status assistant-status--loading';
    deepen.style.display = 'none';

    // Hide full-data message during loading
    var fullDataMsg = document.getElementById('assistant-full-data-msg');
    if (fullDataMsg) fullDataMsg.style.display = 'none';

    var fetchStartTime = Date.now();

    try {
        var response = await fetch('/api/card-insights/analyze', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                cardId: assistantCurrentCardId,
                mode: mode,
                depthLevel: assistantCurrentDepth
            })
        });

        var data = await response.json();

        // Remove loading
        answer.classList.remove('assistant-answer--loading');

        if (data.success) {
            answer.innerHTML = formatAssistantResponse(data.content);
            scope.textContent = 'النطاق: ' + (data.depthLabel || '');

            // Track depth state
            assistantCurrentDepth = data.depthLevel || 1;
            assistantIsFullDataReached = data.isFullDataReached;
            assistantHasDeeperData = data.hasDeeperData;

            // Full-data-reached: show message instead of deepen button
            if (assistantIsFullDataReached) {
                deepen.style.display = 'none';
                if (fullDataMsg) fullDataMsg.style.display = 'block';
            } else if (mode === 'deep' && assistantHasDeeperData) {
                // Show deepen button if more data available
                deepen.style.display = 'block';
            }

            // Cached/speedy response indicator (< 500ms)
            var elapsed = Date.now() - fetchStartTime;
            status.textContent = elapsed < 500 ? '✓ اكتمل التحليل' : '';
            status.className = 'assistant-status';
        } else {
            answer.innerHTML = '<p class="assistant-error">' + (data.errorMessage || 'حدث خطأ') + '</p>';
            status.textContent = '';
            status.className = 'assistant-status';
        }
    } catch (err) {
        answer.classList.remove('assistant-answer--loading');
        answer.innerHTML = '<p class="assistant-error">تعذر الاتصال بخدمة التحليل.</p>';
        status.textContent = '⚠ فشل الاتصال';
        status.className = 'assistant-status assistant-status--error';
        console.error('Assistant panel: fetch failed', err);
    }
}

function formatAssistantResponse(text) {
    // Basic formatting: convert newlines to <br>, bold markdown-like patterns
    if (!text) return '';
    var html = text
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/\n/g, '<br>')
        .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    return html;
}

function deepenAnalysis() {
    if (!assistantCurrentCardId) return;
    if (assistantIsFullDataReached) return;

    assistantCurrentDepth += 1;

    // Re-call analyzeCard with incremented depth
    analyzeCard('deep');
}
