# خطة استبدال Syncfusion بـ Radzen Blazor

> **المشروع:** WarehouseDashboard — .NET 8 Razor Pages
> **الحالة:** Syncfusion → Radzen Blazor
> **الهدف:** إزالة كل آثر لـ Syncfusion بالكامل
> **التاريخ:** 2026-07-17

---

## ملخص النطاق

| البند | العدد |
|-------|-------|
| ملفات متأثرة | **20 ملف** |
| مكونات Grid | **7 مواقع** |
| مكونات Chart | **3 مواقع** |
| مكونات CircularGauge | **2 موقع** |
| مكونات Forms | **موقع واحد** |
| **المجموع** | **13 موقع** عبر 20 ملف |

---

## المراحل

| المرحلة | المحتوى | الملفات | الحالة |
|---------|---------|---------|--------|
| **0** | البنية التحتية — إعداد Radzen + إزالة Syncfusion | 4 ملفات | ⏳ |
| **1** | استبدال DataGrid (7 مواقع) | ~7 ملفات | ⏳ |
| **2** | استبدال Charts (3 مواقع) | ~3 ملفات | ⏳ |
| **3** | استبدال CircularGauge (2 موقع) | ~2 ملفات | ⏳ |
| **4** | استبدال Forms + مكونات أصغر | ~2 ملفات | ⏳ |
| **5** | التنظيف النهائي + التحقق من عدم وجود أي آثر | كل الملفات | ⏳ |

---

## المرحلة 0: البنية التحتية

### الهدف
إعداد Radzen Blazor في المشروع وإزالة حزم Syncfusion النواة.

### ما ي change

| الملف | التعديل |
|-------|---------|
| `*.Web.csproj` | إزالة حزم Syncfusion NuGet — إضافة `Radzen.Blazor` |
| `Program.cs` | إضافة `builder.Services.AddRadzenComponents()` + `app.MapBlazorHub()` |
| `Pages/_Layout.cshtml` | إضافة `<script src="_framework/blazor.web.js"></script>` |
| `Pages/_ViewImports.cshtml` | إضافة `@using Radzen` + `@using Radzen.Blazor` |

### شروط القبول
- [ ] `dotnet build` — 0 Errors
- [ ] `dotnet run` — التطبيق يعمل بدون أخطاء
- [ ] لا توجد مرجع لـ Syncfusion في أي `.csproj`

### المخاطر
- تفعيل Blazor Server قد يؤثر على أداء الصفحة — يجب تقييم ذلك

---

## المرحلة 1: استبدال DataGrid (7 مواقع)

### الهدف
استبدال جميع جداول Syncfusion بـ `RadzenDataGrid`.

### المكون المستبدل
```html
<!-- القديم (Syncfusion) -->
<ejs-grid dataSource="@data" allowPaging="true" allowSorting="true" allowFiltering="true">
    <e-grid-columns>
        <e-grid-column field="Name" headerText="الاسم" />
    </e-g rid-columns>
</ejs-grid>

<!-- الجديد (Radzen) -->
<RadzenDataGrid Data="@data" AllowPaging="true" AllowSorting="true" AllowFiltering="true">
    <Columns>
        <RadzenDataGridColumn Property="Name" Title="الاسم" />
    </Columns>
</RadzenDataGrid>
```

### المواقع المتأثرة
| # | الصفحة | الوصف |
|---|--------|-------|
| 1 | TableMappings/Index | جدول تعيارات الجداول |
| 2 | Cards/Index | جدول البطاقات |
| 3 | Cards/Builder | جدول بناء البطاقة |
| 4 | Sync/Index | جدول سجلات المزامنة |
| 5 | QueryTester/Index | نتائج الاستعلام |
| 6 | DrillDown/Index | جدول التفصيل |
| 7 | SyncDashboard/Index | لوحة المزامنة |

### ما ي change في كل موقع
- استبدال `ejs-grid` بـ `RadzenDataGrid`
- تحويل `e-grid-columns` إلى `Columns` + `RadzenDataGridColumn`
- ربط البيانات同样的 (`Data=` بدلاً من `dataSource=`)
- التأكد من أن الفلترة والترتيب والتصدير تعمل

### شروط القبول
- [ ] كل 7 الجداول تعرض البيانات بشكل صحيح
- [ ] الفلترة تعمل (نصي، رقمي، تاريخي)
- [ ] الترتيب يعمل (تصاعدي/تنازلي)
- [ ] Pagination يعمل
- [ ] التصدير يعمل (PDF/Excel/CSV) إن وُجد
- [ ] RTL يعمل بشكل صحيح
- [ ] `dotnet build` — 0 Errors

---

## المرحلة 2: استبدال Charts (3 مواقع)

### الهدف
استبدال رسومات Syncfusion بـ `RadzenChart`.

### المكون المستبدل
```html
<!-- القديم (Syncfusion) -->
<ejs-chart>
    <e-series dataSource="@chartData" xName="Month" yName="Value" type="Bar" />
</ejs-chart>

<!-- الجديد (Radzen) -->
<RadzenChart>
    <RadzenBarSeries Data="@chartData" CategoryProperty="Month" ValueProperty="Value" />
</RadzenChart>
```

### المواقع المتأثرة
| # | الصفحة | الوصف |
|---|--------|-------|
| 1 | Dashboard/Index | رسومات لوحة التحكم |
| 2 | DrillDown/Index | رسومات التفصيل |
| 3 | Cards/Builder | رسومات بناء البطاقة |

### أنواع الرسومات المطلوبة
- Bar Chart
- Line Chart
- Pie/Donut Chart
- Area Chart
- any other types currently used

### ما ي change في كل موقع
- استبدال `ejs-chart` بـ `RadzenChart`
- تحويل `e-series` إلى `RadzenBarSeries` / `RadzenLineSeries` / `RadzenPieSeries` إلخ
- ربط البيانات (`Data=` بدلاً من `dataSource=`)
- التأكد من التفاعلية (hover, tooltips)

### شروط القبول
- [ ] كل الرسومات تعرض البيانات بشكل صحيح
- [ ] التفاعلية تعمل (hover, tooltips, legend)
- [ ] الألوان متناسقة مع تصميم Blue Theme
- [ ] الرسومات responsive
- [ ] RTL يعمل
- [ ] `dotnet build` — 0 Errors

---

## المرحلة 3: استبدال CircularGauge (2 موقع)

### الهدف
استبدال مقياس Syncfusion بـ `RadzenRadialGauge` أو `RadzenGauge`.

### المواقع المتأثرة
| # | الصفحة | الوصف |
|---|--------|-------|
| 1 | Dashboard/Index | مقياس لوحة التحكم |
| 2 | DrillDown/Index | مقياس التفصيل |

### ما ي change
- استبدال `ejs-circulargauge` بـ `RadzenRadialGauge`
- ربط القيم والحدود

### شروط القبول
- [ ] المقياسان يعملان بشكل صحيح
- [ ] القيم تظهر بشكل دقيق
- [ ] التصميم متناسق
- [ ] `dotnet build` — 0 Errors

---

## المرحلة 4: استبدال Forms + مكونات أصغر

### الهدف
استبدال مكونات النماذج والأدوات الصغيرة.

### المواقع المتأثرة
| # | الصفحة | الوصف |
|---|--------|-------|
| 1 | Cards/Builder | حقول النموذج |
| 2 | أي صفحة أخرى فيها مكونات Syncfusion Forms | حقول إدخال، قائمة منسدلة، تاريخ |

### ما ي change
- استبدال `ejs-dropdownlist` بـ `RadzenDropDown`
- استبدال `ejs-datepicker` بـ `RadzenDatePicker`
- استبدال `ejs-textbox` بـ `RadzenTextBox`
- استبدال أي مكون نموذج آخر

### شروط القبول
- [ ] كل حقول النموذج تعمل بشكل صحيح
- [ ] Validation يعمل
- [ ] RTL يعمل
- [ ] `dotnet build` — 0 Errors

---

## المرحلة 5: التنظيف النهائي + التحقق

### الهدف
التأكد من عدم وجود أي آثر لـ Syncfusion في التطبيق بالكامل.

### قائمة التحقق الشاملة

#### 1. NuGet Packages
```bash
# يجب أن لا توجد نتيجة
grep -r "Syncfusion" --include="*.csproj" src/
```

#### 2. CSS References
```bash
# يجب أن لا توجد نتيجة
grep -ri "syncfusion" --include="*.cshtml" --include="*.css" src/
```

#### 3. JavaScript References
```bash
# يجب أن لا توجد نتيجة
grep -ri "syncfusion" --include="*.cshtml" --include="*.js" src/
```

#### 4. Tag Helpers
```bash
# يجب أن لا توجد نتيجة
grep -r "ejs-" --include="*.cshtml" src/
```

#### 5. C# Code
```bash
# يجب أن لا توجد نتيجة
grep -r "Syncfusion" --include="*.cs" src/
```

#### 6. Configuration
```bash
# يجب أن لا توجد نتيجة
grep -ri "syncfusion" --include="*.json" src/
```

#### 7. Static Files
```bash
# يجب أن لا توجد نتيجة
find src/ -name "*syncfusion*" -o -name "*Syncfusion*"
```

### شروط القبول النهائية
- [ ] لا توجد أي نتيجة لأي أمر grep أعلاه
- [ ] `dotnet build` — 0 Errors, 0 Warnings مرتبطة بـ Syncfusion
- [ ] `dotnet run` — التطبيق يعمل بالكامل
- [ ] كل الميزات تعمل: Grid, Charts, Gauges, Forms
- [ ] RTL عربي يعمل في كل الصفحات
- [ ] المظهر متناسق مع Blue Theme
- [ ] لا توجد حزم Syncfusion في `obj/` أو `bin/` (بعد `dotnet clean`)

---

## ملاحظات تقنية مهمة

### Radzen Blazor في Razor Pages
Radzen Blazor مكتبة Blazor، لكنها تعمل في Razor Pages عبر:
```html
<script src="_framework/blazor.web.js"></script>
```
وتسجيل الخدمات في `Program.cs`:
```csharp
builder.Services.AddRadzenComponents();
```

### التصميم الحالي
التطبيق يستخدم تصميم مخصص (`blue-theme.css` مع CSS variables). Radzen يدعم تخصيص CSS بالكامل، لذا يمكن الحفاظ على الهوية البصرية الحالية.

### التخزين المؤقت
بعد الإزالة النهائية، يجب تنفيذ:
```bash
dotnet clean
dotnet build
```
للتأكد من عدم وجود بقايا في `bin/` أو `obj/`.

---

## التسلسل المقترح للتنفيذ

```
المرحلة 0 → [مراجعة + اختبار] → المرحلة 1 → [مراجعة + اختبار]
→ المرحلة 2 → [مراجعة + اختبار] → المرحلة 3 → [مراجعة + اختبار]
→ المرحلة 4 → [مراجعة + اختبار] → المرحلة 5 → [تحقق نهائي]
```

كل مرحلة = **مهمة واحدة** (`TASK-COD-MIG-0X`) مع:
- Handback مسجل
- Build passing
- اختبار يدوي
- مراجعة Post-Execution
