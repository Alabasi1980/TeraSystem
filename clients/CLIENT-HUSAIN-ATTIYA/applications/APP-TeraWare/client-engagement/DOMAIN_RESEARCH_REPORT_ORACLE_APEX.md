# Domain Research Report: Oracle APEX & WebView2

## Metadata

| Field | Value |
|-------|-------|
| **Topic** | Oracle APEX & WebView2 Integration for TeraWare |
| **Domain** | Oracle APEX Web Application Platform |
| **Mode** | consulting |
| **Date** | 2026-07-09 |
| **Requested by** | TCEA (مستشار) |
| **Project** | TeraWare — تطبيق مساعد لـ NatejSoft ERP |

## Research Objective

فهم كيفية بناء تطبيق سطح مكتب (WPF مع WebView2) يتفاعل مع تطبيق ويب Oracle APEX الخاص بـ NatejSoft ERP. يشمل ذلك: بنية URL، نمط DOM، حقن JavaScript عبر WebView2، واستخدام APEX JavaScript API.

---

> ⚠️ **تنبيه:** جميع المعلومات في هذا التقرير هي [Research Hint] — استرشادية فقط. لا تدخل النطاق المعتمد دون تأكيد Majed.

---

## 1. APEX URL Structure — بنية رابط APEX

### 1.1 تنسيق f?p الأساسي

رابط Oracle APEX التقليدي (Legacy f?p Syntax) يتبع التنسيق التالي:

```
f?p=App:Page:Session:Request:Debug:ClearCache:itemNames:itemValues:PrinterFriendly
```

**المصدر:** [Oracle Documentation](https://docs.oracle.com/en/database/oracle/apex/24.2/htmdb/understanding-legacy-url-syntax.html) — Tier 1

### 1.2 شرح المعاملات (Parameters)

| المعامل | الوصف | مثال |
|:--------|-------|:----:|
| **App** | رقم التطبيق (Application ID) أو الاسم المستعار (Alias) | `101` |
| **Page** | رقم الصفحة أو الاسم المستعار | `10123` |
| **Session** | رقم الجلسة الفريد (Session ID) | `14334840355373` |
| **Request** | قيمة الطلب (SAVE, DELETE) | `SAVE` |
| **Debug** | DEBUG flag — YES أو LEVELn أو NO | `YES` |
| **ClearCache** | قائمة بأرقام الصفحات لمسح Cache أو APP, SESSION, RP | `10123` |
| **itemNames** | أسماء Page Items مفصولة بفاصلة | `P0_PK,GLOBAL_SCREEN_SEQ` |
| **itemValues** | قيم العناصر بنفس الترتيب | `192,Y` |
| **PrinterFriendly** | وضع الطباعة | `Yes` |

### 1.3 تحليل رابط ERP الفعلي

```
https://erp.hae.com.jo:444/erp/f?p=101:10123:14334840355373:10123::10123:P0_PK,GLOBAL_SCREEN_SEQ,FROM_MENU:,192,Y
```

| الجزء | القيمة | المعنى |
|:------|:-------|:-------|
| https://erp.hae.com.jo:444/erp/ | Base URL | خادم ORDS |
| f?p= | Prefix | بادئة APEX |
| 101 | App ID | معرف التطبيق |
| 10123 | Page ID | رقم الصفحة الحالية |
| 14334840355373 | Session ID | معرف الجلسة |
| 10123 | Request | قيمة الطلب |
| (فارغ) | Debug | — |
| 10123 | ClearCache | مسح Cache |
| P0_PK,GLOBAL_SCREEN_SEQ,FROM_MENU, | Item Names | 3 عناصر + فارغ |
| 192,Y | Item Values | القيم المقابلة |

### 1.4 بناء URL برمجياً — JavaScript

```javascript
function buildApexUrl(pageId, itemNames, itemValues) {
    var baseUrl = \"https://erp.hae.com.jo:444/erp/\";
    var appId = \"101\";
    var sessionId = apex.env.APP_SESSION;
    return baseUrl + \"f?p=\" + appId + \":\" + pageId + \":\" + sessionId 
           + \":::\" + pageId + \":\" + (itemNames || \"\") + \":\" + (itemValues || \"\") + \":\";
}
```

### 1.5 بناء URL برمجياً — C# WebView2

```csharp
string baseUrl = \"https://erp.hae.com.jo:444/erp/\";
string appId = \"101\";
string sessionId = ExtractSessionFromUrl(webView.Source.ToString());
string url = $\"{baseUrl}f?p={appId}:{pageId}:{sessionId}:::{pageId}:{itemNames}:{itemValues}:\";
webView.CoreWebView2.Navigate(url);
```

### 1.6 اكتشاف رقم الصفحة الحالية

```javascript
// الأفضل — يستخدم APEX API
var currentPageId = apex.env.APP_PAGE_ID;
var currentAppId = apex.env.APP_ID;
var currentSession = apex.env.APP_SESSION;

// بديل — من الـ URL
var match = window.location.href.match(/f?p=\\d+:(\\d+):/);
var pageId = match ? match[1] : null;
```

**المصدر:** [Oracle APEX JavaScript API Reference — apex.env](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.html) — Tier 1

### 1.7 التعامل مع Session Timeout

```javascript
function isSessionExpired() {
    return document.body.innerText.indexOf(\"session has timed out\") > -1
           || document.body.innerText.indexOf(\"Session expired\") > -1;
}
```

**المصدر:** [Oracle Documentation — Security Attributes](https://docs.oracle.com/en/database/oracle/apex/26.1/htmdb/configuring-security-attributes.html) — Tier 1

---

## 2. APEX DOM & Element Patterns — نمط DOM في APEX

### 2.1 تسمية العناصر

APEX items لها معرف (id) مساوٍ لاسم العنصر:

```html
<!-- item P1_FIRST_NAME يتحول إلى -->
<input type=\"text\" id=\"P1_FIRST_NAME\" name=\"P1_FIRST_NAME\" />
```

**المصدر:** [Oracle Documentation — Understanding Page Items](https://docs.oracle.com/en/database/oracle/apex/26.1/htmdb/understanding-page-items.html) — Tier 1

### 2.2 CSS Classes الشائعة

| العنصر | الـ CSS Class |
|:-------|:--------------|
| حقل إدخال | `.text_field` أو `input[type=\"text\"]` |
| زر APEX | `.t-Button` |
| منطقة (Region) | `.t-Region` |
| حاوية الصفحة | `.t-Page` |
| جدول تقارير | `.t-Report` |
| نموذج | `.t-Form` |
| حوار (Dialog) | `.t-Dialog` |
| رسالة خطأ | `.t-Form-error` |

### 2.3 العثور على العناصر عبر JavaScript

```javascript
// 1. باستخدام id
$(\"#P10123_ITEM_NAME\");

// 2. باستخدام APEX API (مستحسن)
var item = apex.item(\"P10123_ITEM_NAME\");
var value = item.getValue();

// 3. حسب CSS Class
$(\".t-Button\");
$(\".text_field\");

// 4. أزرار حسب النص
$(\"button:contains('حفظ')\");
```

### 2.4 تعيين وقراءة القيم

```javascript
// تعيين قيمة
apex.item(\"P10123_ITEM_NAME\").setValue(\"القيمة الجديدة\");
// مع إخفاء change event
apex.item(\"P10123_ITEM_NAME\").setValue(\"القيمة\", null, true);
// مع قيمة عرض (لـ Popup LOV)
apex.item(\"P10123_ITEM_ID\").setValue(\"10\", \"اسم العنصر\", false);

// قراءة قيمة
var val = apex.item(\"P10123_ITEM_NAME\").getValue();

// تمكين/تعطيل
apex.item(\"P10123_ITEM_NAME\").disable();
apex.item(\"P10123_ITEM_NAME\").enable();

// إظهار/إخفاء
apex.item(\"P10123_ITEM_NAME\").hide();
apex.item(\"P10123_ITEM_NAME\").show();
```

**المصدر:** [Oracle APEX JS API — item Interface](https://docs.oracle.com/en/database/oracle/apex/26.1/aexjs/item.html) — Tier 1

### 2.5 كشف تحميل الصفحة بالكامل

```javascript
// apexreadyend — بعد تحميل كل عناصر APEX
apex.jQuery(apex.gPageContext$).on(\"apexreadyend\", function(e) {
    console.log(\"APEX page fully loaded\");
    initializeTeraWare(); // بدء التهيئة
});

// بديل — Page Load
$(document).ready(function() {
    // DOM جاهز ولكن APEX قد لا يكون جاهزاً
});
```

### 2.6 التعامل مع الـ Partial Page Refresh (PPR)

PPR يمسح التعديلات السابقة على DOM — يجب إعادة الحقن:

```javascript
// الاستماع لأي PPR
apex.jQuery(\"body\").on(\"apexbeforerefresh\", function() {
    console.log(\"PPR starting...\");
}).on(\"apexafterrefresh\", function() {
    console.log(\"PPR completed — إعادة حقن\");
    reinitializeTeraWareHooks();
});

// إعادة تطبيق hooks
function reinitializeTeraWareHooks() {
    bindCustomButtons();
    modifyLabels();
}
```

**المصدر:** [Oracle Documentation — Dynamic Actions](https://docs.oracle.com/en/database/oracle/apex/24.2/htmdb/managing-dynamic-actions.html) — Tier 1

---

## 3. WebView2 Integration — دمج WebView2 مع APEX

### 3.1 تحضير WebView2 في WPF

```csharp
public partial class MainWindow : Window
{
    private async void InitializeWebView()
    {
        await webView.EnsureCoreWebView2Async();
        
        webView.CoreWebView2.Settings.IsScriptEnabled = true;
        webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
        
        webView.CoreWebView2.NavigationStarting += OnNavigationStarting;
        webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
        webView.CoreWebView2.DOMContentLoaded += OnDOMContentLoaded;
        webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        
        // حقن JS في كل صفحة
        await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(teraWareInitScript);
    }
}
```

**المصدر:** [Microsoft Learn — WebView2 WPF](https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/wpf) — Tier 1

### 3.2 حقن JavaScript بعد تحميل الصفحة

```csharp
// الطريقة 1: حقن ثابت في كل صفحة
await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@\"
    window.teraWareState = { initialized: false };
    function initializeTeraWare() {
        if (window.teraWareState.initialized) return;
        window.teraWareState.initialized = true;
        modifyApexLabels();
        addCustomButtons();
    }
\");

// الطريقة 2: حقن ديناميكي
string script = File.ReadAllText(\"teraWare.js\");
await webView.CoreWebView2.ExecuteScriptAsync(script);

// الطريقة 3: حقن مع انتظار نتيجة
string result = await webView.CoreWebView2.ExecuteScriptAsync(@\"
    (function() {
        var item = apex.item('P10123_ITEM_NAME');
        return JSON.stringify({
            value: item ? item.getValue() : null,
            exists: item !== null
        });
    })()
\");
```

**المصدر:** [Microsoft Learn — JavaScript in WebView2](https://github.com/MicrosoftDocs/edge-developer/blob/main/microsoft-edge/webview2/how-to/javascript.md) — Tier 1

### 3.3 إعادة الحقن بعد PPR

```javascript
// في JavaScript — إعلام C# بعد PPR
apex.jQuery(\"body\").on(\"apexafterrefresh\", function() {
    window.chrome.webview.postMessage(JSON.stringify({
        type: \"PPR_COMPLETED\",
        pageId: apex.env.APP_PAGE_ID
    }));
    reapplyTeraWareHooks();
});

function reapplyTeraWareHooks() {
    modifyApexLabels();
    addCustomButtons();
    bindEvents();
}
```

```csharp
// في C# — استقبال إشعار PPR
private void OnWebMessageReceived(object sender, 
    CoreWebView2WebMessageReceivedEventArgs e)
{
    string json = e.TryGetWebMessageAsString();
    var message = JsonConvert.DeserializeObject<dynamic>(json);
    
    if (message.type == \"PPR_COMPLETED\")
    {
        _ = webView.CoreWebView2.ExecuteScriptAsync(\"reapplyTeraWareHooks();\");
    }
}
```

### 3.4 كشف التنقل داخل WebView2

```csharp
// SourceChanged — يتغير URL
webView.CoreWebView2.SourceChanged += (sender, e) => {
    string url = webView.Source.ToString();
    var pageId = ExtractPageIdFromApexUrl(url);
    UpdatePageContext(pageId);
};

// NavigationCompleted — اكتمل التحميل
webView.CoreWebView2.NavigationCompleted += (sender, e) => {
    if (e.IsSuccess) { /* نجاح */ }
};

// دالة مساعدة
private string ExtractPageIdFromApexUrl(string url) {
    var match = Regex.Match(url, @\"f?p=\\d+:(\\d+):\");
    return match.Success ? match.Groups[1].Value : null;
}
```

**المصدر:** [Microsoft Learn — Navigation Events](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/navigation-events) — Tier 1

### 3.5 التواصل ثنائي الاتجاه (Two-Way Communication)

```csharp
// C# → JavaScript (إرسال)
string json = JsonConvert.SerializeObject(new {
    action = \"setItemValue\",
    itemName = \"P10123_NAME\",
    value = \"قيمة جديدة\"
});
await webView.CoreWebView2.PostWebMessageAsJson(json);

// C# ← JavaScript (استقبال)
private void OnWebMessageReceived(object sender, 
    CoreWebView2WebMessageReceivedEventArgs e)
{
    string json = e.TryGetWebMessageAsString();
    var msg = JsonConvert.DeserializeObject<dynamic>(json);
    
    switch ((string)msg.type) {
        case \"readData\": HandleReadData(msg); break;
        case \"saveToSql\": HandleSaveToSql(msg); break;
        case \"PPR_COMPLETED\": ReapplyScripts(); break;
        case \"sessionExpired\": HandleSessionExpired(); break;
    }
}
```

```javascript
// JavaScript → C#
window.chrome.webview.postMessage(JSON.stringify({
    type: \"readData\",
    itemNames: [\"P10123_NAME\", \"P10123_CODE\"]
}));

// JavaScript ← C#
window.chrome.webview.addEventListener(\"message\", function(event) {
    var msg = JSON.parse(event.data);
    if (msg.type === \"readDataResult\") {
        console.log(\"Data from C#:\", msg.data);
    }
});
```

**المصدر:** [Microsoft Learn — PostWebMessageAsJson](https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.postwebmessageasjson) — Tier 1

### 3.6 التعامل مع APEX Modal Dialogs

APEX Dialogs تُفتح كـ Overlay داخل Iframe:

```javascript
// كشف فتح Dialog
function detectDialogOpen() {
    return $(\".t-Dialog\").length > 0 || $(\".ui-dialog\").length > 0;
}

// كشف إغلاق Dialog
apex.jQuery(apex.gPageContext$).on(\"apexafterclosedialog\", 
    function(event, data) {
        console.log(\"Dialog closed - page:\", data.dialogPageId);
        reapplyTeraWareHooks();
});

// كشف إغلاق (بأي طريقة)
apex.jQuery(apex.gPageContext$).on(\"apexafterclosecanceldialog\", 
    function(event, data) {
        console.log(\"Dialog closed/cancelled\");
});
```

**المصدر:** [Oracle Documentation — Dialog Pages](https://docs.oracle.com/en/database/oracle/apex/26.1/htmdb/creating-dialog-pages.html) — Tier 1

---

## 4. APEX JavaScript API — واجهة APEX الجافاسكريبت

### 4.1 كائن apex الرئيسي

| الخاصية/الدالة | الوصف |
|:---------------|:-------|
| apex.env | APP_ID, APP_PAGE_ID, APP_SESSION, APP_USER |
| apex.jQuery | نسخة jQuery التي يستخدمها APEX |
| apex.gPageContext$ | سياق الصفحة الحالية |
| apex.items | مجموعة item interfaces |
| apex.regions | مجموعة region interfaces |

**المصدر:** [Oracle APEX JS API — apex Namespace](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.html) — Tier 1

### 4.2 apex.item — التعامل مع العناصر

```javascript
var item = apex.item(\"P10123_ITEM_NAME\");
item.value;           // اختصار getValue/setValue
item.getValue();      // الحصول على القيمة
item.setValue(\"val\"); // تعيين القيمة
item.setValue(\"val\", \"display\", true); // بدون change event
item.disable();       // تعطيل
item.enable();        // تمكين
item.hide();          // إخفاء
item.show();          // إظهار
item.refresh();       // تحديث
item.setFocus();      // تركيز
item.isEmpty();       // هل فارغ؟
item.isChanged();     // هل تغير؟
```

**المصدر:** [Oracle APEX JS API — item](https://docs.oracle.com/en/database/oracle/apex/26.1/aexjs/item.html) — Tier 1

### 4.3 apex.server.process — استدعاء عمليات الخادم

```javascript
// استدعاء Ajax Callback
apex.server.process(\"MY_PROCESS\", {
    x01: \"test\",
    pageItems: \"#P10123_ITEM1,#P10123_ITEM2\"
}, {
    success: function(data) { console.log(data); },
    error: function(err) { console.error(err); },
    dataType: \"json\"
});

// باستخدام Promise
apex.server.process(\"MY_PROCESS\", { x01: \"value\" })
    .then(function(data) { })
    .catch(function(error) { });
```

**المصدر:** [Oracle APEX JS API — apex.server](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.server.html) — Tier 1

### 4.4 apex.page.submit — إرسال الصفحة

```javascript
apex.submit(\"SAVE\");
apex.submit({
    request: \"SAVE\",
    set: { \"P10123_ITEM1\": \"value1\" },
    showWait: true,
    validate: true
});

// التحقق من التغييرات
if (apex.page.isChanged()) { }
```

**المصدر:** [Oracle APEX JS API — apex.page](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.page.html) — Tier 1

### 4.5 أحداث APEX المهمة

| الحدث | التوقيت |
|:------|:---------|
| apexreadyend | بعد تحميل كل عناصر APEX بالكامل |
| apexbeforepagesubmit | قبل إرسال الصفحة (قابل للإلغاء) |
| apexpagesubmit | قبل الإرسال الفعلي |
| apexbeforerefresh | قبل تحديث PPR |
| apexafterrefresh | بعد تحديث PPR |
| apexafterclosedialog | بعد إغلاق Dialog |
| apexafterclosecanceldialog | بعد إغلاق أو إلغاء Dialog |

```javascript
apex.jQuery(apex.gPageContext$).on(\"apexreadyend\", function() {
    // كل شيء جاهز
});
```

### 4.6 تعديل واجهة APEX بدون كسرها

```javascript
// ✅ استخدم APEX API
apex.item(\"P10123_ITEM\").setValue(\"value\");

// ❌ لا تستخدم jQuery مباشر (قد لا يشغل أحداث APEX)
// $(\"#P10123_ITEM\").val(\"value\");

// ✅ إضافة أزرار مع تنسيق APEX
function addApexButton(regionId, text, handler) {
    var btn = `<button type=\"button\" 
        class=\"t-Button t-Button--hot t-Button--small\">
        ${text}</button>`;
    $(\"#\" + regionId + \" .t-ButtonRegion-buttons\").append(btn);
    $(\"#\" + regionId + \" .t-Button:last\").on(\"click\", handler);
}

// ✅ تعديل النصوص
function modifyLabel(itemName, newLabel) {
    $(\"label[for='\" + itemName + \"']\").text(newLabel);
}
```

---

## 5. ملخص النتائج الرئيسية

| # | النتيجة | مستوى الثقة |
|:-:|---------|:-----------:|
| 1 | APEX URL: f?p=App:Page:Session:Request:Debug:ClearCache:Items:Values:Print | عالي (Tier 1) |
| 2 | عناصر APEX لها id = اسم العنصر (P_XX_NAME) — يسهل الوصول إليها | عالي (Tier 1) |
| 3 | apex.env.APP_PAGE_ID يعطي رقم الصفحة — الأفضل للكشف | عالي (Tier 1) |
| 4 | WebView2.ExecuteScriptAsync ينفذ JavaScript ويعيد النتيجة | عالي (Tier 1) |
| 5 | PostWebMessageAsJson + WebMessageReceived للتواصل ثنائي الاتجاه | عالي (Tier 1) |
| 6 | PPR يمسح التعديلات — أعد الحقن بعد apexafterrefresh | عالي (Tier 1) |
| 7 | apex.server.process يستدعي عمليات الخادم | عالي (Tier 1) |
| 8 | apex.item().setValue() أفضل طريقة لتعديل القيم | عالي (Tier 1) |
| 9 | APEX Modal Dialogs كـ Overlay — لا تشغل NavigationCompleted | متوسط |
| 10 | Session Timeout يُكشف عبر مراقبة محتوى الصفحة | متوسط |

## 6. المصادر المستخدمة

| # | المصدر | Tier |
|:-:|--------|:----:|
| 1 | [Oracle APEX Docs — f?p URL](https://docs.oracle.com/en/database/oracle/apex/24.2/htmdb/understanding-legacy-url-syntax.html) | 1 |
| 2 | [Oracle APEX JS API — apex Namespace](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.html) | 1 |
| 3 | [Oracle APEX JS API — item Interface](https://docs.oracle.com/en/database/oracle/apex/26.1/aexjs/item.html) | 1 |
| 4 | [Oracle APEX JS API — apex.server](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.server.html) | 1 |
| 5 | [Oracle APEX JS API — apex.page](https://docs.oracle.com/en/database/oracle/apex/24.2/aexjs/apex.page.html) | 1 |
| 6 | [Oracle Docs — Page Items](https://docs.oracle.com/en/database/oracle/apex/26.1/htmdb/understanding-page-items.html) | 1 |
| 7 | [Oracle Docs — Dialog Pages](https://docs.oracle.com/en/database/oracle/apex/26.1/htmdb/creating-dialog-pages.html) | 1 |
| 8 | [Oracle Docs — Dynamic Actions](https://docs.oracle.com/en/database/oracle/apex/24.2/htmdb/managing-dynamic-actions.html) | 1 |
| 9 | [Microsoft Learn — WebView2 WPF](https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/wpf) | 1 |
| 10 | [Microsoft Learn — WebView2 JavaScript](https://github.com/MicrosoftDocs/edge-developer/blob/main/microsoft-edge/webview2/how-to/javascript.md) | 1 |
| 11 | [Microsoft Learn — PostWebMessageAsJson](https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.postwebmessageasjson) | 1 |
| 12 | [Microsoft Learn — Navigation Events](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/navigation-events) | 1 |
| 13 | [Oracle-and-APEX.com — URL Format](https://www.oracle-and-apex.com/apex-url-format/) | 2 |
| 14 | [Rick Strahl — WebView2 Interop](https://weblog.west-wind.com/posts/2021/Jan/26/Chromium-WebView2-Control-and-NET-to-JavaScript-Interop-Part-2) | 3 |

## 7. الفجوات البحثية (Research Gaps)

1. **تنسيق URL خاص بـ NatejSoft** — الرابط يختلف قليلاً عن التنسيق القياسي (يكرر PAGE_ID). يحتاج تأكيد.
2. **أسماء عناصر APEX الدقيقة** — لا نعرف أسماء العناصر الفعلية حتى نصل إلى قاعدة البيانات.
3. **سلوك APEX Dialogs مع Iframe** — قد لا تعمل postMessage داخل Iframe إذا كان Cross-Origin.
4. **إصدار APEX المستخدم** — NatejSoft قد يكون على إصدار قديم يؤثر على توافر APIs.

## 8. مخاطر وتحذيرات

- ⚠️ **Same-Origin Policy**: WebView2 يحترم Same-Origin — لا حقن JS في Iframes من مصادر مختلفة
- ⚠️ **PPR يمسح التعديلات**: يجب إعادة الحقن بعد كل PPR
- ⚠️ **إصدار APEX**: غير معروف — بعض الـ APIs قد تختلف
- ⚠️ **الأداء**: حقن JS في كل PPR قد يؤثر على الأداء

---

> **خلاصة:** هذا التقرير يحتوي على [Research Hint] — معلومات استرشادية فقط.
> لا يُعرّف النطاق النهائي أو التسعير أو الالتزامات.
> جميع التوصيات استشارية حتى تأكيد Majed.