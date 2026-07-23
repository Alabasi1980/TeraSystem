# TASK-AIQ-008 — قائمة الكويريز المحفوظة (Saved Queries Modal + Load)

**المرحلة:** Phase 2 — واجهة المستخدم (UI)
**الحالة:** Draft
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** إضافة Modal لقائمة الكويريز المحفوظة مع فتح وتحميل وحذف

---

## 1. الوصف

إضافة Modal في صفحة QueryTester (`Index.cshtml`) تعرض قائمة بكل الكويريز المحفوظة في قاعدة البيانات. تتيح:
- عرض القائمة مع بحث
- فتح كويري ← تحميل SQL في CodeMirror + تحميل محادثته في الشات
- حذف كويري مع تأكيد
- تعديل اسم الكويري (تحديث)

---

## 2. المخرجات المطلوبة

### 2.1 CSS — يُضاف في نهاية `<style>`

```css
/* ============================================================
   Saved Queries Modal (TASK-AIQ-008)
   ============================================================ */
.wd-modal-overlay { /* موجود مسبقاً في QueryTester — نستخدمه */ }
.wd-modal-saved {
    background: var(--c-surface); border-radius: var(--radius-lg);
    box-shadow: var(--shadow-xl); padding: var(--sp-6);
    min-width: 500px; max-width: 600px; max-height: 70vh;
    display: flex; flex-direction: column;
    animation: wdFadeUp 0.2s var(--ease);
}
.wd-modal-saved__header {
    display: flex; align-items: center; justify-content: space-between;
    margin-bottom: var(--sp-4); flex-shrink: 0;
}
.wd-modal-saved__title { font-size: 18px; font-weight: 700; color: var(--c-text); margin: 0; display: flex; align-items: center; gap: var(--sp-2); }
.wd-modal-saved__close { background: none; border: none; font-size: 20px; cursor: pointer; color: var(--c-text-muted); padding: 4px; border-radius: var(--radius-sm); }
.wd-modal-saved__close:hover { background: var(--c-surface-muted); }
.wd-modal-saved__search {
    margin-bottom: var(--sp-3); flex-shrink: 0;
    position: relative;
}
.wd-modal-saved__search input {
    width: 100%; padding: 10px 14px; border: 1px solid var(--c-border); border-radius: var(--radius-md);
    font-family: var(--font-ar); font-size: 14px; color: var(--c-text); background: var(--c-surface);
    outline: none; transition: border-color var(--dur-fast);
}
.wd-modal-saved__search input:focus { border-color: var(--c-primary); box-shadow: 0 0 0 3px rgba(31,78,121,0.12); }
.wd-modal-saved__search-icon { position: absolute; inset-inline-start: 12px; top: 50%; transform: translateY(-50%); color: var(--c-text-muted); }
.wd-modal-saved__search input { padding-inline-start: 36px; }
.wd-modal-saved__list {
    flex: 1; overflow-y: auto; display: flex; flex-direction: column; gap: var(--sp-2);
    min-height: 200px;
}
.wd-modal-saved__empty {
    text-align: center; padding: 40px 20px; color: var(--c-text-muted);
    display: flex; flex-direction: column; align-items: center; gap: var(--sp-2);
}
.wd-modal-saved__item {
    display: flex; align-items: center; gap: var(--sp-3); padding: 12px 14px;
    background: var(--c-surface); border: 1px solid var(--c-border); border-radius: var(--radius-md);
    transition: background var(--dur-fast), border-color var(--dur-fast); cursor: pointer;
    animation: wdFadeUp var(--dur-norm) var(--ease) both;
}
.wd-modal-saved__item:hover { background: var(--c-surface-muted); border-color: var(--c-primary); }
.wd-modal-saved__item-icon { font-size: 20px; flex-shrink: 0; }
.wd-modal-saved__item-info { flex: 1; min-width: 0; }
.wd-modal-saved__item-name { font-size: 14px; font-weight: 600; color: var(--c-text); }
.wd-modal-saved__item-preview { font-size: 12px; color: var(--c-text-muted); direction: ltr; text-align: start; font-family: 'Courier New', monospace; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; margin-top: 2px; }
.wd-modal-saved__item-meta { font-size: 11px; color: var(--c-text-muted); display: flex; gap: var(--sp-3); margin-top: 4px; }
.wd-modal-saved__item-actions { display: flex; gap: 4px; flex-shrink: 0; }
.wd-modal-saved__btn {
    background: none; border: 1px solid transparent; cursor: pointer; padding: 4px 8px; border-radius: var(--radius-sm);
    font-family: var(--font-ar); font-size: 12px; color: var(--c-primary); transition: all var(--dur-fast);
}
.wd-modal-saved__btn:hover { background: rgba(31,78,121,0.08); border-color: var(--c-border); }
.wd-modal-saved__btn--danger { color: var(--c-error); }
.wd-modal-saved__btn--danger:hover { background: rgba(214,69,69,0.08); border-color: rgba(214,69,69,0.2); }
.wd-modal-saved__loading { text-align: center; padding: 40px; color: var(--c-text-muted); }
.wd-modal-saved__footer { margin-top: var(--sp-3); padding-top: var(--sp-3); border-top: 1px solid var(--c-border); text-align: center; font-size: 12px; color: var(--c-text-muted); }
/* Inline edit */
.wd-modal-saved__edit-input {
    border: 1px solid var(--c-primary); border-radius: var(--radius-sm); padding: 2px 8px;
    font-family: var(--font-ar); font-size: 14px; font-weight: 600; color: var(--c-text); background: var(--c-surface);
    width: 100%; outline: none;
}
/* Delete confirmation */
.wd-modal-confirm { text-align: center; padding: 20px; }
.wd-modal-confirm p { margin: 0 0 var(--sp-4); color: var(--c-text); font-size: 14px; }
.wd-modal-confirm-actions { display: flex; justify-content: center; gap: var(--sp-3); }
/* Loading spinner for items */
.wd-modal-saved__spinner { display: inline-block; width: 14px; height: 14px; border: 2px solid var(--c-border); border-top-color: var(--c-primary); border-radius: 50%; animation: qtSpin 0.6s linear infinite; }
```

### 2.2 HTML — يُضاف في نهاية `<body>`

```html
<!-- ============================================================
     Saved Queries Modal (TASK-AIQ-008)
     ============================================================ -->
<div id="savedQueriesModal" class="wd-modal-overlay" hidden onclick="if(event.target===this)closeSavedQueriesModal()">
    <div class="wd-modal-saved">
        <!-- Header -->
        <div class="wd-modal-saved__header">
            <h3 class="wd-modal-saved__title">📁 الكويريز المحفوظة</h3>
            <button class="wd-modal-saved__close" onclick="closeSavedQueriesModal()">✕</button>
        </div>

        <!-- Search -->
        <div class="wd-modal-saved__search">
            <span class="wd-modal-saved__search-icon">🔍</span>
            <input type="text" id="sqSearchInput" placeholder="ابحث بالاسم..." oninput="searchSavedQueries()" />
        </div>

        <!-- List -->
        <div class="wd-modal-saved__list" id="sqList">
            <div class="wd-modal-saved__loading" id="sqLoading"><span class="wd-modal-saved__spinner"></span> جارٍ التحميل...</div>
        </div>

        <!-- Footer -->
        <div class="wd-modal-saved__footer" id="sqFooter" hidden>
            <span id="sqCount">0</span> كويري محفوظ
        </div>
    </div>
</div>
```

### 2.3 JavaScript — يُضاف في نهاية آخر `<script>` block

استبدال دالة `openQueryList()` الموجودة في AIQ-006 بهذه الدوال:

```javascript
// ================================================================
// Saved Queries Modal (TASK-AIQ-008)
// ================================================================

let savedQueriesCache = [];  // Cache for search filtering

// Open the saved queries modal
async function openQueryList() {
    var modal = document.getElementById('savedQueriesModal');
    modal.hidden = false;
    document.getElementById('sqFooter').hidden = true;

    // Reset search
    document.getElementById('sqSearchInput').value = '';

    // Show loading
    document.getElementById('sqList').innerHTML = '<div class="wd-modal-saved__loading" id="sqLoading"><span class="wd-modal-saved__spinner"></span> جارٍ التحميل...</div>';

    try {
        var response = await fetch('/admin-secure-panel/QueryTester?handler=ListQueries');
        var result = await response.json();

        if (result.success && result.queries) {
            savedQueriesCache = result.queries;
            renderSavedQueries(result.queries);
        } else {
            document.getElementById('sqList').innerHTML = '<div class="wd-modal-saved__empty">⚠️ تعذر تحميل القائمة.</div>';
        }
    } catch (err) {
        document.getElementById('sqList').innerHTML = '<div class="wd-modal-saved__empty">⚠️ تعذر الاتصال بالخادم.</div>';
    }
}

// Render saved queries list
function renderSavedQueries(queries) {
    var container = document.getElementById('sqList');

    if (!queries || queries.length === 0) {
        container.innerHTML = '<div class="wd-modal-saved__empty"><span style="font-size:40px">📂</span> لا توجد كويريز محفوظة.</div>';
        document.getElementById('sqFooter').hidden = true;
        return;
    }

    var html = '';
    queries.forEach(function(q) {
        var preview = q.sqlPreview || '';
        var convCount = q.conversationCount || 0;
        var date = q.updatedAt ? new Date(q.updatedAt).toLocaleDateString('ar-SA') : '';
        html += '<div class="wd-modal-saved__item" data-id="' + q.id + '">' +
            '<span class="wd-modal-saved__item-icon">📄</span>' +
            '<div class="wd-modal-saved__item-info">' +
                '<div class="wd-modal-saved__item-name">' + escapeHtml(q.name) + '</div>' +
                '<div class="wd-modal-saved__item-preview">' + escapeHtml(preview) + '</div>' +
                '<div class="wd-modal-saved__item-meta">' +
                    '<span>💬 ' + convCount + ' رسالة</span>' +
                    '<span>📅 ' + date + '</span>' +
                '</div>' +
            '</div>' +
            '<div class="wd-modal-saved__item-actions">' +
                '<button class="wd-modal-saved__btn" onclick="event.stopPropagation(); loadSavedQuery(' + q.id + ')">📂 فتح</button>' +
                '<button class="wd-modal-saved__btn wd-modal-saved__btn--danger" onclick="event.stopPropagation(); deleteSavedQuery(' + q.id + ', \'' + escapeHtml(q.name).replace(/'/g, "\\'") + '\')">🗑</button>' +
            '</div>' +
        '</div>';
    });

    container.innerHTML = html;
    document.getElementById('sqFooter').hidden = false;
    document.getElementById('sqCount').textContent = queries.length;
}

// Search/filter saved queries
function searchSavedQueries() {
    var term = document.getElementById('sqSearchInput').value.trim().toLowerCase();
    if (!term) {
        renderSavedQueries(savedQueriesCache);
        return;
    }
    var filtered = savedQueriesCache.filter(function(q) {
        return q.name.toLowerCase().includes(term);
    });
    renderSavedQueries(filtered);
}

// Close modal
function closeSavedQueriesModal() {
    document.getElementById('savedQueriesModal').hidden = true;
}

// Load a saved query: SQL → CodeMirror, conversations → Chat
async function loadSavedQuery(id) {
    try {
        showChatToast('📂 جارٍ تحميل الكويري...', 'info');

        var response = await fetch('/admin-secure-panel/QueryTester?handler=LoadQuery&id=' + id);
        var result = await response.json();

        if (!result.success || !result.query) {
            showChatToast('⚠️ فشل تحميل الكويري', 'error');
            return;
        }

        var query = result.query;

        // Set SQL in CodeMirror
        if (typeof editor !== 'undefined' && query.sqlQuery) {
            editor.setValue(query.sqlQuery);
        }

        // Set savedQueryId for conversation continuity
        chatState.savedQueryId = query.id;

        // Load conversations into chat
        var container = document.getElementById('chatMessages');
        container.innerHTML = '';

        if (query.conversations && query.conversations.length > 0) {
            query.conversations.forEach(function(msg) {
                addMessage(msg.role, msg.message, msg.sqlSnapshot);
            });
        } else {
            // No conversations — show welcome
            container.innerHTML = '<div class="wd-chat-welcome" id="chatWelcome">' +
                '<p>📄 <strong>' + escapeHtml(query.name) + '</strong></p>' +
                '<p class="wd-chat-hint">اكتب رسالتك لمواصلة التعديل على هذا الكويري.</p>' +
                '</div>';
        }

        // Close modal
        closeSavedQueriesModal();
        showChatToast('✅ تم تحميل "' + query.name + '"', 'success');

        // Expand chat drawer if collapsed
        var drawer = document.getElementById('aiChatDrawer');
        if (drawer.classList.contains('collapsed')) {
            toggleChatDrawer();
        }
    } catch (err) {
        showChatToast('⚠️ تعذر تحميل الكويري', 'error');
    }
}

// Delete saved query with confirmation
async function deleteSavedQuery(id, name) {
    if (!confirm('🗑 هل أنت متأكد من حذف "' + name + '"؟\nسيتم حذف الكويري وجميع محادثاته.')) {
        return;
    }

    try {
        var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
        var response = await fetch('/admin-secure-panel/QueryTester?handler=DeleteQuery', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({ id: id })
        });
        var result = await response.json();

        if (result.success) {
            showChatToast('🗑 تم حذف "' + name + '"', 'success');
            // Remove from cache and re-render
            savedQueriesCache = savedQueriesCache.filter(function(q) { return q.id !== id; });
            renderSavedQueries(savedQueriesCache);

            // If this was the current conversation, reset
            if (chatState.savedQueryId === id) {
                chatState.savedQueryId = null;
            }
        } else {
            showChatToast('⚠️ ' + (result.errorMessage || 'فشل الحذف'), 'error');
        }
    } catch (err) {
        showChatToast('⚠️ تعذر حذف الكويري', 'error');
    }
}

// Update saveQuery function (AIQ-008 enhancement): after save, refresh cache
// We override the existing saveQuery from AIQ-006
var originalSaveQuery = window.saveQuery;
window.saveQuery = async function() {
    await originalSaveQuery();
    // Refresh the cache after save
    try {
        var response = await fetch('/admin-secure-panel/QueryTester?handler=ListQueries');
        var result = await response.json();
        if (result.success && result.queries) {
            savedQueriesCache = result.queries;
        }
    } catch(e) {}
};
```

**هام:** استبدل دالة `openQueryList()` الموجودة (التي كانت مجرد toast placeholder) بهذا الكود الكامل.

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml  (edit)
```

---

## 4. معايير القبول

| # | المعيار | التحقق |
|---|---------|--------|
| 1 | الضغط على 📂 يفتح Modal مع القائمة | معاينة |
| 2 | القائمة تظهر كل الكويريز مع اسم + معاينة SQL + عدد الرسائل + تاريخ | معاينة |
| 3 | البحث يصفي القائمة حسب الاسم | معاينة |
| 4 | **فتح كويري** ← يحمّل SQL في CodeMirror + محادثته في الشات | معاينة |
| 5 | **حذف كويري** ← تأكيد ← حذف ← تحديث القائمة | معاينة |
| 6 | بعد الحذف، إذا كان الكويري مفتوحاً حالياً، `savedQueryId` يتصفّر | مراجعة |
| 7 | `dotnet build` PASS — 0 errors | أمر الـ build |

---

## 5. Pre-Execution Gate: ✅ PASS
