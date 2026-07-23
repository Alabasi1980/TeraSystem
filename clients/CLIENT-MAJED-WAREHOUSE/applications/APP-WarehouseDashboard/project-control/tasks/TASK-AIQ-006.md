# TASK-AIQ-006 — Bottom Drawer Chat Panel UI

**المرحلة:** Phase 2 — واجهة المستخدم (UI)
**الحالة:** Draft
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** إضافة Bottom Drawer Chat Panel إلى صفحة QueryTester للتفاعل مع AI Assistant

---

## 1. الوصف

إضافة لوحة دردشة (Chat Panel) في أسفل صفحة QueryTester (`Pages/admin-secure-panel/QueryTester/Index.cshtml`) تسمح للمستخدم بالدردشة مع AI Assistant لإنشاء وتحسين استعلامات SQL.

**هام:** يتم تعديل ملف `Index.cshtml` فقط — لا يتم إنشاء ملفات منفصلة.

---

## 2. التصميم

```
┌──────────────────────────────────────────────────────────────┐
│  QueryTester Content (as is — unchanged)                     │
│  Schema Browser | Editor | Results                           │
│                                                              │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│  🤖 مساعد الاستعلامات        [🧹 جديد] [📁 حفظ] [📂 قائمة]   │  ← شريط العنوان
│  ────────────────────────────────────────────────            │
│                                                              │
│  أنت: أريد كويري يعرض حركة الأصناف مع الموردين              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ 🤖: فهمت طلبك... هذا هو الكويري المقترح:             │   │
│  │                                                      │   │
│  │ SELECT i.ItemName, m.Quantity, m.Price               │   │
│  │ FROM Items i                                         │   │
│  │ JOIN Movements m ON i.ItemID = m.ItemID              │   │
│  │ WHERE m.MovementDate >= '2026-01-01'                 │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                              │
│  ┌─────────────────────────────────────┐ [▶ إرسال] [🔄 SQL] │
│  │ اكتب رسالتك...                      │                    │
│  └─────────────────────────────────────┘                    │
├──────────────────────────────────────────────────────────────┤
│  [📂 الكويريز المحفوظة (3)]  [📥 تصدير]                      │  ← Footer
└──────────────────────────────────────────────────────────────┘
```

| الحجم | الارتفاع |
|-------|---------|
| مطوي (Collapsed) | 48px (شريط العنوان فقط) |
| مفتوح (Expanded) | 45% من ارتفاع الشاشة |
| على الموبايل | 60% من ارتفاع الشاشة |

---

## 3. المخرجات المطلوبة

### 3.1 CSS (يُضاف في نهاية الـ `<style>` الموجود)

متغيرات وأسلوب الـ Bottom Drawer — تستخدم متغيرات `blue-theme.css` الموجودة (`var(--c-surface)`, `var(--c-border)`, `var(--c-primary)`, إلخ).

**المكونات المطلوبة:**
- `.wd-chat-drawer` — الحاوية الأساسية (أسفل الصفحة)
- `.wd-chat-drawer.collapsed` — حالة الطي
- `.wd-chat-header` — شريط العنوان (قابل للنقر للطي/الفتح)
- `.wd-chat-messages` — منطقة الرسائل (يظهر فيها طلب/رد)
- `.wd-chat-input-area` — منطقة الإدخال (textbox + أزرار)
- `.wd-chat-footer` — الفوتر (إحصائيات، أزرار إضافية)
- `.wd-msg-user` — رسالة المستخدم (محاذاة لليمين)
- `.wd-msg-assistant` — رسالة الـ AI (محاذاة لليسار)
- `.wd-msg-system` — رسالة نظام (استعلام استكشاف، أخطاء)

**الأنماط:**
- أنيميشن fadeUp للرسائل الجديدة
- Scroll تلقائي لآخر رسالة
- شريط تمرير مخصص (دائماً في الأسفل)
- تمايز لوني بين رسالة المستخدم والـ AI
- Code blocks داخل رسائل AI (```sql ... ```)
- Skeleton loader أثناء انتظار رد AI

### 3.2 HTML (يُضاف في نهاية `<body>` قبل الـ `<script>`)

```html
<!-- ===== AI Chat Drawer ===== -->
<div id="aiChatDrawer" class="wd-chat-drawer">
  <!-- Header -->
  <div class="wd-chat-header" onclick="toggleChatDrawer()">
    <span class="wd-chat-header__title">🤖 مساعد الاستعلامات</span>
    <div class="wd-chat-header__actions">
      <button onclick="newChat()" title="محادثة جديدة">🧹</button>
      <button onclick="saveQuery()" title="حفظ الكويري" id="chatSaveBtn">📁</button>
      <button onclick="openQueryList()" title="الكويريز المحفوظة" id="chatListBtn">📂</button>
      <span class="wd-chat-header__toggle">▼</span>
    </div>
  </div>

  <!-- Messages Area -->
  <div class="wd-chat-messages" id="chatMessages">
    <div class="wd-chat-welcome">
      <p>👋 مرحباً! أنا مساعد الاستعلامات الذكي.</p>
      <p>اسألني عن أي كويري SQL تحتاجه، وسأساعدك في بنائه.</p>
      <p class="wd-chat-hint">مثال: "أريد كويري يعرض حركة الأصناف مع الموردين"</p>
    </div>
  </div>

  <!-- Input Area -->
  <div class="wd-chat-input-area">
    <textarea class="wd-chat-input" id="chatInput" rows="1" 
      placeholder="اكتب رسالتك..." 
      onkeydown="handleChatKeydown(event)"></textarea>
    <button class="wd-btn wd-btn--primary wd-chat-send" onclick="sendChatMessage()">▶ إرسال</button>
  </div>
</div>
```

### 3.3 JavaScript (يُضاف في نهاية الـ `<script>` الموجود)

**الدوال المطلوبة:**

1. **`toggleChatDrawer()`** — طي/فتح الـ Drawer
2. **`sendChatMessage()`** — الدالة الرئيسية:
   - تقرأ رسالة المستخدم من `chatInput`
   - تقرأ SQL الحالي من CodeMirror: `editor.getValue()`
   - ترسل `POST /QueryTester?handler=Chat` مع JSON body
   - تعرض رسالة المستخدم في الشات
   - تعرض رد AI (مع تحويل ```sql إلى code blocks)
   - إذا في `suggestedSql` → تعرض زر "تطبيق SQL"
   - تحفظ `savedQueryId` في متغير عام لاستمرار المحادثة
3. **`addMessage(role, content, sqlSnapshot?)`** — إضافة رسالة إلى الشات
4. **`applySuggestedSql(sql)`** — تطبيق SQL المقترح في CodeMirror
5. **`newChat()`** — بدء محادثة جديدة (تصفير savedQueryId + مسح الشات)
6. **`saveQuery()`** — حفظ الكويري الحالي عبر `POST /QueryTester?handler=SaveQuery`
7. **`openQueryList()`** — فتح قائمة الكويريز المحفوظة (لتُنفَّذ في AIQ-008، لكن زرها موجود الآن)
8. **`handleChatKeydown(e)`** — Ctrl+Enter للإرسال، Enter للسطر الجديد
9. **`showChatToast(msg, type)`** — إشعارات داخل الشات
10. **`autoResizeTextarea(el)`** — تكبير الـ textarea تلقائياً عند الكتابة

**API Endpoints المتاحة:**
- `POST /admin-secure-panel/QueryTester?handler=Chat` — body: `{ message, currentSql, savedQueryId, source }` → returns `AiChatResult`
- `POST /admin-secure-panel/QueryTester?handler=SaveQuery` — body: `{ name, description, sqlQuery, dataSourceType }`
- `GET /admin-secure-panel/QueryTester?handler=ListQueries` — قائمة الكويريز المحفوظة
- `GET /admin-secure-panel/QueryTester?handler=SchemaInfo` — معلومات Schema

**مثال تنسيق كود SQL في الرد:**
```javascript
function formatAiResponse(text) {
  // Convert ```sql ... ``` to highlighted code blocks
  return text.replace(/```sql\n?([\s\S]*?)```/g, function(match, sql) {
    return '<div class="wd-chat-code-block"><code>' + escapeHtml(sql.trim()) + '</code></div>';
  });
}
```

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml  (edit)
```

**ملاحظة:** كل الإضافات تكون في ملف واحد — CSS في نهاية `<style>`، HTML في نهاية `<body>`، JS في نهاية `<script>`.

---

## 5. Forbidden Actions

- ❌ لا تعدل أي ملف خارج Allowed Write Targets
- ❌ لا تغير الـ HTML/JS الموجود — فقط أضف الجديد في نهاية كل قسم
- ❌ لا تعدل CodeMirror setup الموجود (editor ثابت)
- ❌ لا تضيف مكتبات خارجية

---

## 6. معايير القبول

| # | المعيار | التحقق |
|---|---------|--------|
| 1 | Bottom Drawer يظهر في أسفل الصفحة | معاينة |
| 2 | زر طي/فتح يعمل (toggle) | معاينة |
| 3 | إرسال رسالة ترسل request إلى OnPostChatAsync | مراجعة JS |
| 4 | رد AI يظهر في منطقة الرسائل | معاينة |
| 5 | كود SQL في رد AI يظهر كـ code block | معاينة |
| 6 | زر "تطبيق SQL" يظهر إذا في suggestedSql | معاينة |
| 7 | **Ctrl+Enter** يرسل الرسالة، **Enter** سطر جديد | مراجعة |
| 8 | `savedQueryId` يُحفظ للمتابعة في نفس المحادثة | مراجعة |
| 9 | محادثة جديدة تمسح الشات وتصفّر savedQueryId | معاينة |
| 10 | Skeleton loader أثناء انتظار الرد | معاينة |
| 11 | متوافق مع `blue-theme.css` (متغيرات CSS) | مراجعة |
| 12 | `dotnet build` PASS — 0 errors | أمر الـ build |

---

## 7. Vitality & Polish Checklist

- [ ] Skeleton loading أثناء انتظار رد AI
- [ ] Toast عند نجاح/فشل الإرسال
- [ ] Connection status للـ AI (متصل/غير متصل)
- [ ] Micro-animations عند ظهور رسائل جديدة (stagger)
- [ ] Empty state (رسالة ترحيبية)
- [ ] Auto-scroll لآخر رسالة
- [ ] Responsive: Bottom drawer يكبر على الموبايل
- [ ] Auto-resize textarea
- [ ] Enter = سطر جديد، Ctrl+Enter = إرسال

---

## 8. Sub-Agent Handback

### Changes Applied

| القسم | الوصف |
|-------|-------|
| **CSS** (أسطر 462–561) | تنسيق كامل للـ Bottom Drawer: header, messages, input, code blocks, skeleton, responsive |
| **HTML** (أسطر 745–776) | هيكل الـ Drawer: header مع أزرار، منطقة رسائل مع ترحيب، input، footer |
| **JS** (أسطر 2480–2615) | 12 دالة: toggleChatDrawer, sendChatMessage, addMessage, formatAiResponse, addSuggestedSqlButton, showChatSkeleton, newChat, saveQuery, openQueryList, handleChatKeydown, autoResizeTextarea, showChatToast |

**Build:** 0 errors, 0 warnings ✅
**Safety:** [CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓

### Post-Execution Review: ✅ PASS → **Accepted**
