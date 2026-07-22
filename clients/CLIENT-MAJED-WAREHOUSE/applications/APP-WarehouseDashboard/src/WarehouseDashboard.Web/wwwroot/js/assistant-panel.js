// AI Assistant Panel — State & Controls
let assistantCurrentCardId = null;

// Track depth for deep analysis
let assistantCurrentDepth = 1;
let assistantCurrentMode = null;

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
    
    // Reset drill levels area
    var drillLevelsArea = document.getElementById('assistant-drill-levels');
    if (drillLevelsArea) drillLevelsArea.style.display = 'none';
    
    // Hide depth options on panel open
    var depthOptions = document.getElementById('assistant-depth-options');
    if (depthOptions) depthOptions.style.display = 'none';
    
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

function analyzeCardAtDepth(mode, depth) {
    assistantCurrentDepth = depth;
    analyzeCard(mode);
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
    var scope = document.getElementById('assistant-scope');

    answer.classList.add('assistant-answer--loading');
    answer.innerHTML = '';
    status.textContent = 'جاري التحليل...';
    status.className = 'assistant-status assistant-status--loading';

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
            if (mode === 'deep' && data.isFullDataReached) {
                if (fullDataMsg) fullDataMsg.style.display = 'block';
            }

            // Show/hide depth options based on card's date column
            var depthOptions = document.getElementById('assistant-depth-options');
            if (depthOptions) {
                if (mode === 'deep' && data.hasDateColumn) {
                    depthOptions.style.display = 'block';
                    // Mark active depth button
                    depthOptions.querySelectorAll('.assistant-depth-btn').forEach(function(btn) {
                        btn.classList.remove('active');
                        if (parseInt(btn.getAttribute('data-depth')) === assistantCurrentDepth) {
                            btn.classList.add('active');
                        }
                    });
                } else {
                    depthOptions.style.display = 'none';
                }
            }

            // Show advanced drill levels from DB
            var drillLevelsArea = document.getElementById('assistant-drill-levels');
            if (drillLevelsArea) {
                drillLevelsArea.innerHTML = '';
                if (data.availableDrillLevels && data.availableDrillLevels.length > 0) {
                    var title = document.createElement('div');
                    title.className = 'assistant-drill-title';
                    title.textContent = '🔍 تعمق متقدم';
                    drillLevelsArea.appendChild(title);
                    
                    data.availableDrillLevels.forEach(function(level) {
                        var btn = document.createElement('button');
                        btn.className = 'assistant-drill-btn';
                        btn.textContent = '🔎 ' + level.displayName;
                        btn.onclick = function() {
                            var cardId = assistantCurrentCardId;
                            closeAssistantPanel();
                            setTimeout(function() {
                                if (typeof wdOpenDrill === 'function' && cardId) {
                                    wdOpenDrill(cardId);
                                }
                            }, 300);
                        };
                        drillLevelsArea.appendChild(btn);
                    });
                    drillLevelsArea.style.display = 'block';
                } else {
                    drillLevelsArea.style.display = 'none';
                }
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

// deepenAnalysis() removed — replaced by analyzeCardAtDepth() with direct depth buttons
