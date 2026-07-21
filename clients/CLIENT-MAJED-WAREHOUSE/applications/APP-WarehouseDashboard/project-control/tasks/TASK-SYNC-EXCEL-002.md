# TASK-SYNC-EXCEL-002 — Export Excel Button in Sync Dashboard (UI)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-EXCEL-002 |
| **المجموعة** | Sync Enhancement — Export Excel |
| **النوع** | Frontend (UI — Razor Page inline JS) |
| **الوكيل** | ui-designer |
| **الأولوية** | High |
| **الحالة** | 🟡 Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-21 |
| **التبعية** | TASK-SYNC-EXCEL-001 ✅ يجب أن يكون مكتملاً أولاً |

---

## 1. المشكلة

شاشة المزامنة (Sync Dashboard) عندها جدول تعيينات مع زر ▶ لكل صف لتشغيل المزامنة، لكن لا يوجد زر لتنزيل Excel. الـ API الجديد (`GET /api/sync/{id}/export-excel`) جاهز ويحتاج واجهة.

---

## 2. الهدف

إضافة زر تنزيل Excel (📥) في كل صف من جدول التعيينات، بجانب زر المزامنة (▶)، مع تجربة مستخدم سلسة:
- أيقونة واضحة (📥 أو SVG)
- يفتح التحميل المباشر في المتصفح
- Toast تنبيه عند نجاح/فشل التحميل
- يدعم الجوال (Responsive)

---

## 3. النطاق — المطلوب تنفيذه

### 3.1 تعديل جدول HTML

**الملف:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Sync/Index.cshtml`

في قسم `wd-table`، أضف عمود جديد **"تنزيل"** في `<th>` بعد عمود الإجراءات (أو قبله):

```html
<th>تنزيل</th>
```

### 3.2 تعديل JavaScript render للصفوف

في دالة `loadDashboard()`، في حلقة `for (var i = 0; i < mappings.length; i++)` (حوالي السطر 534)، أضف زر تنزيل:

```javascript
html += '<td class="wd-actions">' +
    '<button class="wd-btn wd-btn--ghost wd-btn--sm" onclick="syncSingle(' + m.id + ')" title="مزامنة هذا التعيين">▶</button>' +
    '<button class="wd-btn wd-btn--ghost wd-btn--sm" onclick="downloadExcel(' + m.id + ')" title="تنزيل Excel">📥</button>' +
'</td>';
```

### 3.3 إضافة دالة JavaScript `downloadExcel`

في نفس الملف، أضف الدالة التالية:

```javascript
window.downloadExcel = async function (mappingId) {
    try {
        var resp = await fetch(apiBase + '/api/sync/' + mappingId + '/export-excel');
        if (!resp.ok) {
            var err = await resp.json().catch(function() { return { message: 'HTTP ' + resp.status }; });
            showToast(err.message || 'فشل التحميل', 'error');
            return;
        }

        // Get filename from Content-Disposition header, or use default
        var disposition = resp.headers.get('Content-Disposition');
        var filename = 'export.xlsx';
        if (disposition) {
            var match = disposition.match(/filename\*=UTF-8''(.+?)($|;)/);
            if (match) filename = decodeURIComponent(match[1]);
            else {
                match = disposition.match(/filename="?(.+?)"?($|;)/);
                if (match) filename = match[1];
            }
        }

        var blob = await resp.blob();
        var url = URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);

        showToast('✅ تم تحميل الملف: ' + filename, 'success');
    } catch (err) {
        showToast('❌ فشل التحميل: ' + (err.message || ''), 'error');
    }
};
```

### 3.4 إضافة CSS للزر الجديد (اختياري)

إذا احتجت تنسيق إضافي للزر، استخدم الأنماط الموجودة (`.wd-btn wd-btn--ghost wd-btn--sm`) — نفس نمط زر المزامنة.

### 3.5 Responsive Design

تأكد أن الأزرار في الشاشات الصغيرة (Mobile) لا تتداخل:
- الأزرار صغيرة بما يكفي (`wd-btn--sm`)
- الـ `wd-actions` يستخدم `flex` مع `gap: 6px` (موجود بالفعل)

---

## 4. الملفات المسموح كتابتها (Allowed Write Targets)

| الملف | الإجراء |
|---|---|
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Sync/Index.cshtml` | تعديل (إضافة عمود + زر + دالة JS) |

**ممنوع:**
- لا تعديل أي `.cs` (Page Model)
- لا تعديل API endpoint
- لا إضافة مكتبات JS خارجية
- لا تغيير هيكل الصفحة الأساسي

---

## 5. معايير القبول (Acceptance Criteria)

- [ ] **AC1:** كل صف في جدول التعيينات يعرض زر 📥 (أو أيقونة تنزيل)
- [ ] **AC2:** عند الضغط على 📥، يبدأ تحميل ملف Excel في المتصفح
- [ ] **AC3:** اسم الملف صحيح: `{TableName}_{تاريخ}.xlsx`
- [ ] **AC4:** يظهر Toast ✅ عند نجاح التحميل
- [ ] **AC5:** يظهر Toast ❌ عند فشل التحميل مع رسالة الخطأ
- [ ] **AC6:** إذا كان API يعيد 404 → Toast "التعيين غير موجود"
- [ ] **AC7:** Build يمر بـ 0 Errors / 0 Warnings
- [ ] **AC8:** الصفحة لا تظهر أخطاء JS في الـ Console
- [ ] **AC9:** متوافق مع الجوال (الأزرار لا تتداخل)

---

## 6. Pre-Execution Gate

| البند | الحالة |
|---|---|
| التكنولوجي بروفايل | `dotnet-razorpages-adonet` ✅ |
| الملفات المرجعية مقروءة | ✅ |
| النطاق محدد وواضح | ✅ |
| المسموح والممنوع محدد | ✅ |
| الـ AC كامل | ✅ |
| التبعية: TASK-SYNC-EXCEL-001 | ⏳ يجب أن يكون Accepted أولاً |
| الـ Auditor: NOT_REQUIRED (UI only) | ✅ |

**PASS ✅ — جاهز للتفويض بعد إكمال TASK-SYNC-EXCEL-001**

---

## 7. ملاحظات إضافية

- **قبل الكتابة:** اقرأ الملف الحالي من القرص (Fresh File Read Rule) لتتأكد من عدم وجود تغييرات أخرى
- استخدم نمط الأزرار الموجود (`wd-btn wd-btn--ghost wd-btn--sm`) — لا تخترع أنماط جديدة
- احترم الـ `apiBase` المتغير (قد يكون `http://localhost:5001`)
- لا تنسَ `asp-append-version="true"` إذا أضفت أي ملف JS خارجي (غير مطلوب هنا)
- اختبر يدويًا: افتح الصفحة، اضغط 📥، تحقق من تحميل الملف

---

## Handback — 2026-07-21

**الحالة:** ✅ مكتمل
**الملف المغير:** `Pages/admin-secure-panel/Sync/Index.cshtml`
**ملاحظات:**

### التعديلات المنفذة:

1. **إضافة عمود "تنزيل" في `<thead>`** — أضيف `<th>تنزيل</th>` بعد `<th>الإجراءات</th>` (السطر 401).
2. **إضافة زر تنزيل 📥 في كل صف** — أضيف `<button>` مستقل في `<td>` منفصل بعد `wd-actions` في حلقة الـ JS render (الأسطر 546-548)، باستخدام نفس أنماط الأزرار الموجودة `wd-btn wd-btn--ghost wd-btn--sm`.
3. **إضافة دالة `window.downloadExcel`** — أضيفت في نهاية المقطع الـ JS داخل IIFE (الأسطر 812-849)، تستخدم `apiBase` و `showToast` الموجودين مسبقاً.
   - تستخرج اسم الملف من `Content-Disposition` header
   - تنشئ Blob وتفتح التحميل المباشر في المتصفح
   - تظهر Toast ✅ عند النجاح و ❌ عند الفشل

### Vitality Self-Check:

| البند | الحالة | التعليل |
|-------|--------|---------|
| Skeleton Loading / Shimmer | ✅ | موجود مسبقاً (wd-skeleton-wrap) |
| Toast Notifications | ✅ | showToast موجود ويُستخدم في الدالة الجديدة |
| Connection Status | N/A | Admin Dashboard داخلي، الأخطاء تظهر Toast |
| Search | N/A | القائمة محدودة، لا حاجة لبحث |
| Micro-animations | ✅ | wdFadeUp, wdShimmer موجودة مسبقاً |
| Empty States | ✅ | موجود مسبقاً مع SVG مخصص |
| Realistic Data | N/A | البيانات من API حقيقي |
| يبدو "حية" | ✅ | auto-refresh، تفاعلات، Toast، Progress |

**ملاحظات إضافية:**
- حُفظت المحاذاة الصحيحة للأعمدة (7 أعمدة في `<th>` تقابل 7 أعمدة في `<td>`).
- استخدمت نفس أنماط الأزرار الموجودة `wd-btn wd-btn--ghost wd-btn--sm`.
- الدالة الجديدة `downloadExcel` تملك حق الوصول إلى `apiBase` و `showToast` (نفس نطاق IIFE).
- التوثيق: راجع `design-source/REFERENCES.md` (إن وجد).
