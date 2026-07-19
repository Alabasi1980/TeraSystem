# خطة تطوير آلية Drill Down (التنقّل العميق)

> **النسخة:** 3.0 — Parameter & Display Contract Added  
> **التاريخ:** 2026-07-19  
> **الجهة المعدة:** TeraAgent  
> **الحالة:** Draft جاهزة للاعتماد قبل تحويلها إلى TASK-IDs تنفيذية  
> **مهم:** هذه الوثيقة خطة تنفيذية فقط، وليست كوداً.

---

## 1. الهدف

تحويل آلية Drill Down من مسودة تقنية إلى آلية تحليل احترافية:

- واضحة للمستخدم من خلال زر **تفاصيل** على البطاقة.
- تعمل داخل **مودال واحد متدرّج** لا يفتح مودالات متعددة.
- تدعم مستويات متعددة حسب إعدادات `CardDrillDownLevels`.
- تسمح للـ Admin بالتحكم فيما يظهر داخل كل مستوى: جدول، بطاقة KPI، Gauge، أو رسم بياني.
- تحدد بوضوح كيف تنتقل قيمة من مستوى إلى الذي يليه كـ parameter.
- تدعم تصدير البيانات عندما يكون العرض جدولياً أو عندما تكون بيانات المستوى قابلة للتصدير.
- آمنة عند اختبار وتنفيذ SQL.

الهدف النهائي:

> كل بطاقة لديها إعدادات Drill Down تعرض زر **تفاصيل**. عند الضغط عليه يُفتح مودال المستوى الأول، ثم يتنقل المستخدم داخل نفس المودال بين المستويات حسب الإعدادات، مع Breadcrumb، وتحكم واضح بالباراميتر، وإمكانية تصدير للبيانات المناسبة.

---

## 2. قرارات Majed المعتمدة

| القرار | الصياغة المعتمدة |
|---|---|
| مسار العرض | **المودال هو المسار الأساسي** للمستخدم النهائي. صفحة `/Dashboard/Drill` تصبح Legacy/Fallback ولا تُطوّر الآن إلا بقرار منفصل. |
| عدد المستويات | عدد المستويات يأتي من إعدادات Drill Down في `CardDrillDownLevels`. إذا احتجنا سقف سلامة افتراضي فليكن **2 مستوى بدون احتساب البطاقة نفسها**. |
| طريقة فتح Drill | لا نفتح Drill بالنقر المباشر على عنصر الرسم. نضيف زر/إجراء واضح باسم **تفاصيل** يفتح مودال المستوى الأول. |
| الانتقال بين المستويات | لا نفتح مودالاً جديداً لكل مستوى. **نفس المودال** يتغير محتواه من Level إلى آخر، مع Breadcrumb للرجوع. |
| التحكم بالمحتوى | كل مستوى يجب أن يحدد نوع العرض: Table / Bar / Line / Pie / KPI / Gauge. |
| اختبار الاستعلام | مطلوب زر **اختبار الاستعلام** في صفحة تكوين Drill Down قبل الاعتماد. |
| التصدير | مطلوب دعم التصدير خصوصاً إذا كان المستوى يعرض جدولاً. |

---

## 3. الوضع الحالي المختصر

| المكون | الحالة الحالية |
|---|---|
| `CardDrillDownLevel` model | موجود: ParentCardId, Level, DisplayName, DrillDownQuery, TargetChartType |
| `CardDrillDownLevels` table | موجود مع Unique Index على `(ParentCardId, Level)` |
| Admin DrillDown config page | CRUD يعمل، لكنه تقني ولا يختبر الاستعلام ولا يحدد parameter column |
| Drill API | ينفّذ SQL بـ `@p0` فقط، ويعيد JSON للعرض |
| Dashboard modal | موجود لكنه غير مكتمل لإدارة levels والـ breadcrumb والـ parameter contract |
| `/Dashboard/Drill` page | مسار منفصل قديم؛ ليس المسار المعتمد حالياً |
| Cards admin page | لا تعرض Drill metadata ولا زر تكوين Drill |

---

## 4. الفجوات الأساسية

| الرمز | الفجوة | المطلوب |
|---|---|---|
| G1 | لا يوجد زر **تفاصيل** واضح على البطاقات | إضافة زر يظهر فقط للبطاقات التي لديها Drill configured |
| G2 | المودال لا يدير مستويات متعددة بشكل احترافي | بناء Modal State Machine: level, context chain, breadcrumb, loading, empty, error, retry |
| G3 | لا يوجد تحديد صريح للباراميتر بين المستويات | إضافة Parameter Contract لكل مستوى: من أين تأتي قيمة المستوى التالي؟ |
| G4 | الاعتماد على أول عمود كـ parameter غير احترافي | إضافة `ParameterColumn` أو equivalent config |
| G5 | لا يوجد اختبار استعلام | إضافة Test Query آمن في Admin DrillDown |
| G6 | لا يوجد تصدير | إضافة Export CSV للنتائج الجدولية وبيانات المستوى |
| G7 | لا يوجد تحكم كافٍ بمحتوى كل مستوى | توثيق Display Contract لكل مستوى |
| G8 | صفحة Cards لا تعرض Drill metadata | عرض عدد المستويات + زر تكوين Drill |
| G9 | صفحة `/Dashboard/Drill` مسار موازٍ | لا تُطوّر الآن؛ تبقى Legacy/Fallback مؤجل |

---

## 5. تجربة المستخدم المستهدفة

### 5.1 فتح المستوى الأول

```
بطاقة Dashboard
└── زر تفاصيل
    └── يفتح Drill Modal
        └── يعرض Level 1 حسب إعدادات Drill Down
```

قواعد زر **تفاصيل**:

- يظهر فقط للبطاقات التي لديها `CardDrillDownLevels.Count > 0`.
- لا يعتمد على النقر على الرسم نفسه.
- لا يتعارض مع Refresh / Resize / Drag.
- يفتح نفس المودال دائماً عند `level = 1`.
- إذا لا يوجد مستوى 1 أو فشل تحميله، يظهر Error State واضح داخل المودال.

### 5.2 الانتقال إلى مستوى لاحق

لا نفتح مودالاً جديداً. لا نغلق المودال ثم نفتح مودالاً آخر.  
القاعدة المعتمدة:

```text
Same Modal, New State
```

أي:

```text
Modal Level 1
  المستخدم يختار عنصر/صف/قيمة
    ↓
نفس المودال يتحول إلى Level 2
  Breadcrumb يتحدث
  زر رجوع/مسار يسمح بالعودة إلى Level 1
```

### 5.3 Breadcrumb داخل المودال

مثال:

```text
المخزون › المستودعات › المستودع الشمالي › الأصناف
```

كل عنصر سابق في Breadcrumb قابل للنقر للعودة إلى ذلك المستوى بدون إغلاق المودال.

---

## 6. عقد الباراميترات Parameter Contract

هذا القسم إلزامي قبل التنفيذ. بدونه ستبقى آلية Drill Down غير احترافية.

### 6.1 المفاهيم

| المفهوم | المعنى |
|---|---|
| `Parent Value` | القيمة التي يتم تمريرها من المستوى السابق للمستوى الحالي كـ `@p0` |
| `Parameter Column` | العمود من نتيجة المستوى الحالي الذي سيُستخدم كقيمة للمستوى التالي |
| `Label Column` | العمود الذي يظهر في Breadcrumb كنص مفهوم للمستخدم |
| `Requires Parent Value` | هل المستوى يحتاج قيمة من المستوى السابق؟ |
| `Root Parameter` | قيمة اختيارية يستخدمها المستوى الأول إذا احتاج `@p0` |

### 6.2 المستوى الأول Level 1

الأفضل في النسخة الأولى:

```text
Level 1 يعمل بدون parameter افتراضياً
```

مثال:

```sql
SELECT CategoryName, SUM(Quantity) AS TotalQty
FROM Items
GROUP BY CategoryName
```

إذا احتاج Level 1 إلى `@p0`، يجب تحديد **Root Parameter Source** في الإعدادات أو إدخال قيمة اختبارية/ثابتة. لا يجوز أن يعتمد على قيمة غامضة من البطاقة بدون تعريف.

### 6.3 المستوى الثاني وما بعده

كل مستوى لاحق يأخذ قيمة من اختيار المستخدم في المستوى السابق:

```text
Level N receives @p0 = selected value from Level N-1
```

مثال:

Level 1 يعرض:

| CategoryCode | CategoryName | TotalQty |
|---|---|---|
| SPARE | قطع غيار | 1250 |

إذا `ParameterColumn = CategoryCode`، فعند اختيار الصف:

```text
@p0 = SPARE
```

Level 2 query:

```sql
SELECT ItemCode, ItemName, Quantity
FROM Items
WHERE CategoryCode = @p0
```

### 6.4 حسب نوع العرض

#### إذا المستوى يعرض Table

| المطلوب | القرار |
|---|---|
| اختيار الصف | الصف كله قابل للاختيار إذا يوجد مستوى تالٍ |
| parameter للمستوى التالي | يؤخذ من `ParameterColumn` المحدد |
| label في Breadcrumb | يؤخذ من `LabelColumn` إن وجد، وإلا من `ParameterColumn` |
| إذا العمود غير موجود | يظهر Error: “عمود الباراميتر غير موجود في نتيجة الاستعلام” |

#### إذا المستوى يعرض Chart (Bar / Pie / Line)

رغم أننا لا نفتح Drill من الرسم الرئيسي مباشرة، داخل مودال Drill يمكن التحكم بطريقة الاختيار بإحدى طريقتين:

| الطريقة | الوصف |
|---|---|
| قائمة/جدول مصاحب للرسم | الأفضل: أسفل الرسم تظهر قائمة مختصرة/جدول، والاختيار يتم منها |
| اختيار عنصر من الرسم داخل المودال | مسموح داخل المودال فقط إذا كان واضحاً، لكن ليس من الرسم الرئيسي في الداشبورد |

القيمة الممررة للمستوى التالي تأتي أيضاً من `ParameterColumn` ضمن بيانات المستوى.

#### إذا المستوى يعرض KPI / Gauge

KPI/Gauge غالباً يجب أن تكون **نقطة نهاية** وليست مستوى وسيطاً.

إذا أصررنا أن KPI/Gauge تقود لمستوى تالٍ، يجب تحديد:

| الإعداد | المعنى |
|---|---|
| `NextParameterSource = KpiValue` | تمرير قيمة KPI نفسها |
| `NextParameterSource = FixedValue` | تمرير قيمة ثابتة يحددها Admin |
| `NextParameterSource = None` | لا يوجد مستوى تالٍ |

التوصية في النسخة الأولى:

```text
KPI/Gauge = terminal level by default
```

---

## 7. Display Contract — التحكم بما يظهر داخل كل مستوى

كل مستوى Drill يجب أن يحدد طريقة العرض من خلال `TargetChartType`:

| TargetChartType | ماذا يظهر داخل المودال؟ | ملاحظات |
|---|---|---|
| `Table` | جدول بيانات | يدعم export و row selection |
| `Bar` | رسم أعمدة + قائمة/جدول مصاحب للاختيار | مناسب للتجميعات |
| `Line` | رسم خطي + بيانات خام قابلة للتصدير | مناسب للزمن |
| `Pie` | رسم دائري + قائمة عناصر | مناسب للتوزيع |
| `KPI` | بطاقة قيمة رئيسية | يفضل كنهاية |
| `Gauge` | مؤشر دائري | يفضل كنهاية |

### 7.1 معلومات يجب عرضها في كل مستوى

- اسم المستوى `DisplayName`.
- نوع العرض.
- عدد الصفوف المعروضة.
- إذا يوجد مستوى تالٍ: تعليمات “اختر صفاً/عنصراً للانتقال للمستوى التالي”.
- إذا لا يوجد مستوى تالٍ: Badge “آخر مستوى”.

### 7.2 التحكم بالأعمدة

في النسخة الأولى، الأعمدة تأتي من نتيجة SQL كما هي.  
لاحقاً يمكن إضافة إعدادات:

- VisibleColumns
- HiddenColumns
- ColumnLabels
- Number/date formatting

لكن لا نبدأ بها حتى لا يتضخم التنفيذ.

---

## 8. Export Contract — التصدير

### 8.1 متى يظهر زر Export؟

| الحالة | زر Export |
|---|---|
| المستوى يعرض Table | يظهر دائماً إذا توجد rows |
| المستوى يعرض Chart | يظهر كـ “تصدير بيانات الرسم CSV” إذا توجد rows |
| KPI/Gauge | لا يظهر افتراضياً |
| Empty/Error | لا يظهر |

### 8.2 صيغة التصدير في النسخة الأولى

```text
CSV UTF-8 BOM
```

قواعد التصدير:

- يصدّر النتائج المعروضة حالياً في المودال.
- لا يعيد تنفيذ الاستعلام في النسخة الأولى.
- يحافظ على أسماء الأعمدة كما وصلت من API.
- اسم الملف:

```text
drill-{cardId}-level-{level}-{yyyyMMdd-HHmm}.csv
```

### 8.3 Excel

تصدير Excel مؤجل. CSV يكفي للنسخة الأولى.

---

## 9. تغييرات البيانات المطلوبة

الهيكل الحالي كافٍ لتشغيل Drill بسيط، لكنه غير كافٍ لآلية احترافية في تحديد parameter columns.

### 9.1 حقول مقترحة إضافية إلى `CardDrillDownLevels`

| الحقل | النوع المقترح | الغرض | أولوية |
|---|---|---|---|
| `ParameterColumn` | nvarchar(100), nullable | العمود الذي يؤخذ منه `@p0` للمستوى التالي | P0 |
| `LabelColumn` | nvarchar(100), nullable | النص المعروض في Breadcrumb | P0 |
| `RequiresParentValue` | bit, default false | هل هذا المستوى يحتاج قيمة من المستوى السابق؟ | P0 |
| `EnableExport` | bit, default true | هل يسمح بتصدير بيانات المستوى؟ | P1 |
| `MaxRows` | int, default 500 | حد الصفوف لهذا المستوى | P1 |

### 9.2 لماذا هذه الحقول مهمة؟

بدون `ParameterColumn` سيضطر النظام لاستخدام أول عمود دائماً، وهذا غير احترافي وخطير وظيفياً.  
مثال: الاستعلام قد يرجع:

| Name | Code | Qty |
|---|---|---|
| قطع غيار | SPARE | 1250 |

إذا استخدم النظام أول عمود (`Name`) بينما المستوى التالي يحتاج `Code`، ستفشل النتائج أو تكون غير دقيقة. لذلك يجب تحديد `ParameterColumn = Code`.

### 9.3 إن أردنا تجنب Migration في البداية

يمكن تنفيذ نسخة أولى بدون migration، لكن ستكون أقل احترافية:

```text
Fallback ParameterColumn = first column
Fallback LabelColumn = first column
```

لكن التوصية المهنية: **نضيف الحقول الضرورية الآن** لأن الهدف آلية قوية وليست ترقيعاً.

---

## 10. ضوابط اختبار الاستعلام في Admin

زر **اختبار الاستعلام** يجب أن يلتزم بهذه القواعد:

| القاعدة | الوصف |
|---|---|
| Read-only intent | الاستعلام يبدأ بـ `SELECT` أو `WITH` فقط في النسخة الأولى |
| Parameters | إذا كان الاستعلام يحتوي `@p0` يجب طلب قيمة اختبارية |
| SqlParameter | قيمة الاختبار تمر دائماً عبر `SqlParameter` |
| Row limit | عرض أول 100 صف كحد أقصى |
| Timeout | 30 ثانية أو أقل |
| Error sanitization | لا تُعرض connection strings أو كلمات مرور أو تفاصيل حساسة |
| No save required | يمكن اختبار الاستعلام قبل الحفظ |
| Result preview | عرض columns + rows + عدد الصفوف + PASS/FAIL |
| Parameter validation | إذا تم تحديد `ParameterColumn` يجب التحقق أنه موجود في نتيجة الاختبار |
| Label validation | إذا تم تحديد `LabelColumn` يجب التحقق أنه موجود في نتيجة الاختبار |

---

## 11. API Contract المقترح

### 11.1 Execute level

```http
GET /api/dashboard/drill/{cardId}/{level}?parentValue={value}
```

يرجع:

```json
{
  "cardId": 3,
  "cardTitle": "المخزون",
  "level": 1,
  "displayName": "حسب الفئة",
  "chartType": "Table",
  "hasNextLevel": true,
  "parameterColumn": "CategoryCode",
  "labelColumn": "CategoryName",
  "enableExport": true,
  "status": "success",
  "columns": ["CategoryCode", "CategoryName", "TotalQty"],
  "rows": [],
  "kpiValue": null,
  "errorMessage": null
}
```

### 11.2 Test query

```http
POST /admin-secure-panel/DrillDown?handler=TestQuery
```

Body:

```text
drillDownQuery
targetChartType
testParameterValue
parameterColumn
labelColumn
```

يرجع:

```json
{
  "success": true,
  "columns": [],
  "rows": [],
  "rowCount": 25,
  "warnings": []
}
```

---

## 12. خطة التنفيذ المقترحة — Revised v3

### Phase A — Schema & Contract Foundation

| TASK مقترح | الهدف | الملفات | ملاحظات |
|---|---|---|---|
| `TASK-DRILL-SCHEMA-001` | إضافة حقول Parameter/Display/Export إلى `CardDrillDownLevels` | Model + DbContext + Migration | ضروري لاحترافية الربط |
| `TASK-DRILL-API-001` | تحديث Drill API لإرجاع parameter/display/export metadata | `Api/Dashboard/Drill.cshtml.cs`, `DrillDataResult.cs` | بدون تغيير UI كبير |

### Phase B — Admin Query Test

| TASK مقترح | الهدف | الملفات | ملاحظات |
|---|---|---|---|
| `TASK-DRILL-ADMIN-001` | Backend handler لاختبار Drill query بأمان | `admin-secure-panel/DrillDown/Index.cshtml.cs` | SELECT/WITH فقط، `@p0`, max 100 rows |
| `TASK-DRILL-ADMIN-002` | UI اختبار الاستعلام + ParameterColumn/LabelColumn | `admin-secure-panel/DrillDown/Index.cshtml` | Preview + validation |

### Phase C — Modal State & Navigation

| TASK مقترح | الهدف | الملفات | ملاحظات |
|---|---|---|---|
| `TASK-DRILL-MODAL-001` | بناء State Model للمودال | `Pages/Index.cshtml` | currentLevel, chain, selected labels, max levels |
| `TASK-DRILL-MODAL-002` | Breadcrumb داخل المودال | `Pages/Index.cshtml` | رجوع لمستوى سابق داخل نفس المودال |
| `TASK-DRILL-MODAL-003` | حالات العرض | `Pages/Index.cshtml` | loading/empty/error/retry/no-next-level |

### Phase D — Details Entry on Dashboard Cards

| TASK مقترح | الهدف | الملفات | ملاحظات |
|---|---|---|---|
| `TASK-DRILL-ENTRY-001` | إضافة زر **تفاصيل** للبطاقات التي لديها Drill | `Pages/Index.cshtml`, `Index.cshtml.cs` | لا فتح من الرسم مباشرة |
| `TASK-DRILL-ENTRY-002` | منع تعارض زر تفاصيل مع refresh/resize/drag | `Pages/Index.cshtml` | stopPropagation |

### Phase E — Level Renderers & Export

| TASK مقترح | الهدف | الملفات | ملاحظات |
|---|---|---|---|
| `TASK-DRILL-RENDER-001` | Renderer للـ Table + row selection بالـ ParameterColumn | `Pages/Index.cshtml` | أساس الانتقال للمستوى التالي |
| `TASK-DRILL-RENDER-002` | Renderer للـ Chart + selection list داخل المودال | `Pages/Index.cshtml` | لا يعتمد على chart main click |
| `TASK-DRILL-EXPORT-001` | Export CSV للنتائج الحالية | `Pages/Index.cshtml` | UTF-8 BOM |

### Phase F — Admin Cards Integration

| TASK مقترح | الهدف | الملفات | ملاحظات |
|---|---|---|---|
| `TASK-DRILL-CARDS-001` | عرض عدد مستويات Drill في صفحة Cards | `admin/Cards/Index.cshtml.cs`, `admin/Cards/Index.cshtml` | “2 مستويات Drill” |
| `TASK-DRILL-CARDS-002` | زر “تكوين Drill” من صفحة Cards | نفس الملفات | يفتح صفحة DrillDown مع البطاقة |

### Phase G — Legacy/Fallback decision

| TASK مقترح | الهدف | الحالة |
|---|---|---|
| `TASK-DRILL-LEGACY-001` | تقرير قرار بخصوص `/Dashboard/Drill` | مؤجل |

---

## 13. معايير القبول العامة

### للمودال

- زر **تفاصيل** يظهر فقط للبطاقات التي لديها Drill.
- زر **تفاصيل** يفتح Level 1 داخل مودال.
- لا يتم فتح مودال جديد لكل مستوى.
- نفس المودال يغيّر محتواه عند الانتقال للمستوى التالي.
- Breadcrumb يسمح بالعودة لمستوى سابق.
- لا يوجد Drill من الرسم الرئيسي مباشرة.
- لا يتجاوز عدد المستويات configured في DB.
- fallback safety cap = 2 levels إذا تعذر تحميل config.

### للباراميترات

- كل انتقال يستخدم `ParameterColumn` إن تم تحديده.
- إذا `ParameterColumn` غير موجود في نتيجة المستوى يظهر Error واضح.
- `LabelColumn` يستخدم للـ Breadcrumb إن وجد.
- KPI/Gauge تعتبر terminal افتراضياً.
- كل قيم SQL تمر عبر `SqlParameter`.

### لاختبار الاستعلام

- Admin يستطيع الاختبار قبل الحفظ.
- `@p0` يمر عبر `SqlParameter`.
- max rows = 100.
- timeout مضبوط.
- errors sanitized.
- الاستعلامات غير SELECT/WITH تُرفض.
- `ParameterColumn` و `LabelColumn` يتم التحقق من وجودهما في نتيجة الاختبار.

### للتصدير

- Export CSV يظهر في Table levels إذا توجد rows.
- Chart levels يمكن تصدير بياناتها الخام.
- لا يظهر Export عند empty/error.
- CSV يستخدم UTF-8 BOM.

---

## 14. مخاطر يجب مراقبتها

| الخطر | المعالجة |
|---|---|
| كسر ApexCharts الحالي | كل تعديل في `Index.cshtml` محدود ومراجع بعد build |
| تضارب أزرار البطاقة | `event.stopPropagation()` لزر تفاصيل |
| SQL مكلف | Row cap + timeout + اختبار قبل الحفظ |
| اختيار parameter خاطئ | `ParameterColumn` إلزامي عند وجود مستوى تالٍ |
| غياب العمود في نتيجة SQL | Error واضح في الاختبار والتنفيذ |
| تضخم التنفيذ | تقسيم المهام كما في Phase A-G |
| Migration risk | مراجعة DbContext/migration بعناية + build + اختبار صفحة Admin |

---

## 15. ترتيب التنفيذ الموصى به

بعد اعتماد Majed:

1. `TASK-DRILL-SCHEMA-001` — إضافة حقول Parameter/Display/Export.
2. `TASK-DRILL-API-001` — تحديث API payload.
3. `TASK-DRILL-ADMIN-001` — Backend اختبار الاستعلام.
4. `TASK-DRILL-ADMIN-002` — UI اختبار الاستعلام.
5. `TASK-DRILL-MODAL-001` — State model للمودال.
6. `TASK-DRILL-MODAL-002` — Breadcrumb.
7. `TASK-DRILL-RENDER-001` — Table renderer + row selection.
8. `TASK-DRILL-ENTRY-001` — زر تفاصيل على البطاقة.
9. `TASK-DRILL-EXPORT-001` — Export CSV.
10. `TASK-DRILL-CARDS-001` — Drill metadata في صفحة Cards.

لا نبدأ بـ `/Dashboard/Drill` ولا بالنقر المباشر على عناصر الرسم.

---

## 16. خلاصة تنفيذية للعميل المنفذ

هذه الخطة لا تطلب “تجميل مودال” فقط. المطلوب بناء آلية Drill Down كاملة:

1. Admin يحدد لكل مستوى:
   - SQL query.
   - نوع العرض.
   - العمود الذي يمرر للمستوى التالي.
   - العمود الذي يظهر في Breadcrumb.
   - هل التصدير مفعّل.
2. المستخدم يفتح Drill بزر **تفاصيل**.
3. نفس المودال يعرض كل المستويات بالتتابع.
4. التنقل بين المستويات مبني على `ParameterColumn` وليس أول عمود عشوائياً.
5. الجداول قابلة للتصدير CSV.
6. كل SQL يتم اختباره وآمن قبل الاعتماد.

---

> **Prepared by:** TeraAgent  
> **Mode:** Plan Mode only  
> **No code written in this document**
