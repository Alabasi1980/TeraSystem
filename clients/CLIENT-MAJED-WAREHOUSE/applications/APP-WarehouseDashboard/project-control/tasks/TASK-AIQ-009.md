# TASK-AIQ-009 — Vitality & Polish — اللمسات النهائية

**المرحلة:** Phase 2 — واجهة المستخدم (UI)
**الحالة:** Draft
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** تحسين تجربة المستخدم مع اللمسات البصرية والتفاعلية النهائية

---

## 1. الوصف

إضافة التحسينات التالية على واجهة AI Assistant في QueryTester:
1. ✅ **Skeleton loading** — للشات أثناء انتظار الرد (موجود مسبقاً — تحقق)
2. **Toast notifications** — نجاح/فشل العمليات
3. **Connection status indicator** — للـ AI (متصل/غير متصل/خطأ)
4. **Empty states** — رسالة ترحيبية أجمل
5. **Micro-animations** — تحسينات بصرية
6. **Responsive** — تحسينات للموبايل
7. **Chat typing indicator** — مؤشر "الـ AI يكتب..."

---

## 2. المخرجات المطلوبة

### 2.1 تحسينات CSS — تُضاف في نهاية `<style>`

**Chat typing indicator:**
```css
.wd-chat-typing {
    align-self: flex-start; display: flex; align-items: center; gap: 4px;
    padding: 10px 14px; background: var(--c-surface); border: 1px solid var(--c-border);
    border-radius: var(--radius-md); border-end-start-radius: 4px; animation: wdFadeUp var(--dur-norm) var(--ease);
}
.wd-chat-typing__dot {
    width: 8px; height: 8px; border-radius: 50%; background: var(--c-text-muted);
    animation: wdTypingBounce 1.2s ease-in-out infinite;
}
.wd-chat-typing__dot:nth-child(2) { animation-delay: 0.2s; }
.wd-chat-typing__dot:nth-child(3) { animation-delay: 0.4s; }
@keyframes wdTypingBounce {
    0%, 60%, 100% { transform: translateY(0); }
    30% { transform: translateY(-6px); }
}
```

**Connection indicator:**
```css
.wd-chat-conn {
    display: inline-flex; align-items: center; gap: 4px; font-size: 11px;
    padding: 2px 8px; border-radius: var(--radius-full);
}
.wd-chat-conn--online { color: var(--c-success); }
.wd-chat-conn--offline { color: var(--c-error); }
.wd-chat-conn--checking { color: var(--c-text-muted); }
.wd-chat-conn__dot {
    width: 6px; height: 6px; border-radius: 50%; display: inline-block;
}
.wd-chat-conn--online .wd-chat-conn__dot { background: var(--c-success); }
.wd-chat-conn--offline .wd-chat-conn__dot { background: var(--c-error); }
.wd-chat-conn--checking .wd-chat-conn__dot { background: var(--c-text-muted); animation: wdPulse 1s ease-in-out infinite; }
@keyframes wdPulse {
    0%, 100% { opacity: 1; } 50% { opacity: 0.3; }
}
```

**Scroll to bottom button:**
```css
.wd-chat-scroll-btn {
    position: absolute; bottom: 100px; left: 50%; transform: translateX(-50%);
    background: var(--c-surface); border: 1px solid var(--c-border); border-radius: var(--radius-full);
    box-shadow: var(--shadow-md); padding: 6px 12px; cursor: pointer; font-size: 12px;
    color: var(--c-primary); transition: all var(--dur-fast); z-index: 10;
    opacity: 0; pointer-events: none;
}
.wd-chat-scroll-btn.visible { opacity: 1; pointer-events: auto; }
.wd-chat-scroll-btn:hover { background: var(--c-surface-muted); }
```

**Better welcome animation:**
```css
.wd-chat-welcome { animation: wdFadeUp 0.4s var(--ease); }
.wd-chat-welcome p { animation: wdFadeUp 0.4s var(--ease) both; }
.wd-chat-welcome p:nth-child(2) { animation-delay: 0.1s; }
.wd-chat-welcome p:nth-child(3) { animation-delay: 0.2s; }
```

### 2.2 تحسينات HTML — تُضاف/تعدّل في الـ HTML الموجود

**تعديل الفوتر ليشمل حالة الاتصال:**
استبدال الفوتر الموجود بـ:
```html
<div class="wd-chat-footer">
    <span class="wd-chat-conn wd-chat-conn--checking" id="chatConnIndicator">
        <span class="wd-chat-conn__dot"></span>
        <span id="chatConnText">جارٍ الفحص...</span>
    </span>
    <span>⚡ DeepSeek V4 Flash</span>
    <span id="chatTokens">—</span>
</div>
```

**إضافة زر "التمرير للأسفل":**
داخل `wd-chat-messages` (قبل الإغلاق):
```html
<button class="wd-chat-scroll-btn" id="chatScrollBtn" onclick="scrollChatToBottom()">↓ أحدث</button>
```

**تعديل "محادثة جديدة" زر ليكون له confirmation:**
```html
<button type="button" onclick="event.stopPropagation(); confirmNewChat()" title="محادثة جديدة">🧹</button>
```

### 2.3 تحسينات JavaScript — تُضاف في نهاية آخر `<script>`

**Connection status check:**
```javascript
// ================================================================
// AI Chat — Vitality & Polish (TASK-AIQ-009)
// ================================================================

// Check AI connection status on load
async function checkAiConnection() {
    var indicator = document.getElementById('chatConnIndicator');
    var text = document.getElementById('chatConnText');
    if (!indicator) return;

    try {
        var response = await fetch('/admin-secure-panel/QueryTester?handler=SchemaInfo&source=SqlServer', {
            signal: AbortSignal.timeout(5000)
        });
        var result = await response.json();
        if (result.success) {
            indicator.className = 'wd-chat-conn wd-chat-conn--online';
            text.textContent = 'متصل';
        } else {
            throw new Error('API error');
        }
    } catch (err) {
        indicator.className = 'wd-chat-conn wd-chat-conn--offline';
        text.textContent = 'غير متصل';
    }
}
```

**Typing indicator:**
```javascript
function showChatTyping() {
    var container = document.getElementById('chatMessages');
    var typing = document.createElement('div');
    typing.className = 'wd-chat-typing';
    typing.id = 'chatTyping';
    typing.innerHTML = '<span class="wd-chat-typing__dot"></span><span class="wd-chat-typing__dot"></span><span class="wd-chat-typing__dot"></span>';
    container.appendChild(typing);
    scrollChatToBottom();
}
function hideChatTyping() {
    var typing = document.getElementById('chatTyping');
    if (typing) typing.remove();
}
```

**Confirm new chat before clearing:**
```javascript
function confirmNewChat() {
    var hasMessages = document.querySelectorAll('#chatMessages .wd-msg').length > 0;
    if (hasMessages && !confirm('🧹 هل تريد بدء محادثة جديدة؟\nسيتم مسح المحادثة الحالية.')) {
        return;
    }
    newChat();
}
```

**Auto-scroll detection (show scroll button when user scrolls up):**
```javascript
function setupChatScrollDetection() {
    var container = document.getElementById('chatMessages');
    var btn = document.getElementById('chatScrollBtn');
    if (!container || !btn) return;

    container.addEventListener('scroll', function() {
        var isNearBottom = container.scrollHeight - container.scrollTop - container.clientHeight < 100;
        btn.classList.toggle('visible', !isNearBottom);
    });
}
```

**Enhanced `sendChatMessage` — replace existing:**
- Replace skeleton with typing indicator (show typing instead of skeleton)
- Call `checkAiConnection()` after each successful response
- Update tokens display

**Enhanced `scrollChatToBottom`:**
```javascript
function scrollChatToBottom() {
    var container = document.getElementById('chatMessages');
    if (!container) return;
    requestAnimationFrame(function() {
        container.scrollTop = container.scrollHeight;
        var btn = document.getElementById('chatScrollBtn');
        if (btn) btn.classList.remove('visible');
    });
}
```

**Initialize on page load:**
```javascript
// Run on page load
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(checkAiConnection, 1000);
    setupChatScrollDetection();
});
```

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml  (edit)
```

---

## 4. معايير القبول

| # | المعيار | التحقق |
|---|---------|--------|
| 1 | Typing indicator يظهر أثناء انتظار رد AI | معاينة |
| 2 | Connection status يظهر في الفوتر (متصل/غير متصل) | معاينة |
| 3 | Confirm قبل مسح المحادثة | معاينة |
| 4 | زر "↓ أحدث" يظهر عند التمرير للأعلى | معاينة |
| 5 | Toasts تظهر للعمليات (حفظ، حذف، إرسال) | معاينة |
| 6 | Responsive للجوال (الشات 60vh، الأزرار قابلة للنقر) | معاينة |
| 7 | `dotnet build` PASS — 0 errors | أمر الـ build |

---

## 5. Pre-Execution Gate: ✅ PASS
