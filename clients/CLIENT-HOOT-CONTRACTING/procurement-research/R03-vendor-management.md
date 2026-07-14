# Research R03 — إدارة الموردين وتقييمهم (Vendor/Supplier Management)

**ERP Domain:** Vendor Management  
**Scope:** SAP, Oracle, Microsoft Dynamics 365  
**Date:** 2026-07-07  
**Status:** Complete  

---

## فهرس المحتويات

1. [SAP Vendor Master & Vendor Evaluation](#1-sap-vendor-master--vendor-evaluation)
2. [Oracle Supplier Management](#2-oracle-supplier-management)
3. [Microsoft Dynamics 365 Vendor Management](#3-microsoft-dynamics-365-vendor-management)
4. [دورة حياة المورد (Supplier Lifecycle)](#4-دورة-حياة-المورد-supplier-lifecycle)
5. [إدارة الموردين في المقاولات](#5-إدارة-الموردين-في-المقاولات)
6. [أفضل الممارسات (Best Practices)](#6-أفضل-الممارسات-best-practices)

---

## 1. SAP Vendor Master & Vendor Evaluation

### 1.1 Vendor Master Record — البنية الثلاثية

سجل المورد الرئيسي في SAP ينقسم إلى ثلاث مستويات بيانات:

| المستوى | البيانات | المثال |
|---------|----------|--------|
| **General Data** (على مستوى الـ Client) | الاسم، العنوان، رقم الضريبة، رقم الهاتف، طرق الدفع، اللغة | "مؤسسة النور للمقاولات، الرياض، VAT: 123456789" |
| **Company Code Data** | الحسابات الدائنة (Accounts Payable)، شروط الدفع، البنك، التجميع (Reconciliation Account) | شركة Code 1000: Terms 30 يوم، Account 300000 |
| **Purchasing Data** (على مستوى الـ Plant) | المادة المسموح بها، الحد الأدنى للكمية، مهلة التوريد (Lead Time)، سعر الصرف | Plant 2000: Lead Time 14 days, Min Order 100 |

**Transaction Codes الهامة:**
- `XK01` / `BP` — إنشاء Vendor Master
- `XK02` / `BP` — تعديل
- `XK03` / `BP` — عرض
- `FK05` — حظر المورد
- `XK05` — وضع علامة حذف

### 1.2 Business Partner (BP) Approach في S/4HANA

في **SAP S/4HANA**، تم إلغاء المعاملات القديمة (XK0X) وتم الانتقال كلياً إلى مفهوم **Business Partner (BP)**:

```
Business Partner ← Vendor / Customer / Employee
     ↓
  Internal BP (BP with own org)
  External BP (Suppliers, Customers)
```

- كل **Vendor** هو **Business Partner** له Role واحد على الأقل:
  - `FLVN00` — Vendor (FI)
  - `FLVN01` — Vendor (FI + MM)

- **Benefits:**
  - توحيد البيانات عبر الكيانات (لو كان الشخص مورداً وعميلاً في نفس الوقت)
  - إدارة مركزية للعناوين ووسائل الاتصال
  - دعم تام للـ Data Governance

### 1.3 نظام التقييم التلقائي للموردين (Automatic Vendor Evaluation)

SAP يوفر نظام تقييم تلقائي عبر **Transaction Code: ME2N / ME2M / ME2L / ME80FN** مع **Purchase Order History** و**Automatic Vendor Evaluation**.

#### آلية التقييم:

| المعيار (Criterion) | الوزن الافتراضي | المصدر |
|--------------------|----------------|--------|
| **السعر (Price)** | 30% | سجل سعر الشراء (PO History) |
| **الجودة (Quality)** | 30% | عدد الرفض في Inspection Lot (QM) |
| **الالتزام بالتسليم (Delivery)** | 25% | فرق بين تاريخ التسليم المتفق عليه والفعلي |
| **الخدمة (Service)** | 15% | يدوي — تقييم المشرف |

**نظام النقاط:** Score من **0 إلى 100**:
- 80-100: ممتاز
- 60-79: جيد
- 40-59: مقبول
- أقل من 40: ضعيف — يستدعي المراجعة

**Transaction Code للتقييم:**
- `ME61` — إدخال تقييم يدوي
- `ME62` — قائمة التقييمات
- `ME63` — تقرير التقييم الإجمالي
- `ME64` — إصدار التقييم

#### مثال عملي — Vendor Score:

```
المورد: شركة التموين العالمي
PO: 4500012345
الكمية: 1000 طن
تاريخ التسليم المطلوب: 15/06/2026
تاريخ التسليم الفعلي: 18/06/2026
التأخير: 3 أيام ← خصم 10 نقاط من Delivery Score
الجودة: 5% مواد معيبة ← خصم 15 نقطة من Quality Score
السعر: 5% فوق السعر المتفق عليه ← خصم 5 نقاط من Price Score

النتيجة النهائية: 78/100 — جيد
```

### 1.4 Vendor Segmentation (تصنيف الموردين)

| النوع | المعيار | الإجراء |
|-------|---------|---------|
| **Strategic Partner** | حجم تعاقد كبير، مورد وحيد، مواد حرجة | تعاقد طويل الأجل، شروط دفع مرنة، دعم فني |
| **Regular Supplier** | متوسط الحجم، مواد متوفرة بعدة بدائل | تقييم ربع سنوي، عقود قياسية |
| **Reserve (Backup)** | حجم صغير، استعداد لتغطية العجز | أوامر شراء محدودة، تقييم سنوي |

### 1.5 Automatic Payment Program (APP)

SAP APP (Transaction `F110`) يدير دورة الدفع للموردين:

```
مراحل APP:
1. تحديد الموردين المستحقين (Based on Due Date + Payment Terms)
2. خصم الخصم النقدي (Cash Discount — Skonto)
3. إصدار المقترح (Payment Proposal)
4. المراجعة والاعتماد
5. تنفيذ الدفع (طريق البنك / شيك)
6. تسوية الحساب (Clearing)
```

- **Payment Terms (ZTERM):**
  - `0001`: دفع فوري
  - `0002`: صافي 30 يوم
  - `0003`: 2/10 صافي 30 (خصم 2% خلال 10 أيام)
  - مخصص: Z001 — 30% عند الطلب، 70% عند التسليم

---

## 2. Oracle Supplier Management

### 2.1 Supplier Portal (iSupplier)

**Oracle iSupplier Portal** هو منصة ويب تمكن الموردين من:

- عرض أوامر الشراء (PO) الصادرة لهم
- تأكيد أوامر الشراء (Acknowledgement)
- تحديث جدول التسليم (ASN — Advanced Shipment Notification)
- تقديم الفواتير إلكترونياً (E-Invoicing)
- عرض حالة الدفع

### 2.2 Supplier Registration وإدارة الطلبات

**عملية التسجيل:**
```
1. تقديم الطلب (Supplier Request)
2. إدخال البيانات الأساسية (VAT، IBAN، شهادات)
3. مراجعة من المشتري (Buyer Review)
4. الموافقة ← إنشاء سجل المورد (Supplier Creation)
5. إرسال بيانات الدخول إلى البوابة
```

### 2.3 Supplier Performance Management — Scorecards

**Oracle Supplier Scorecards** توفر:

| Dimensions | Weight | Source |
|-----------|--------|--------|
| Quality | 30% | Receiving Inspection (RTV Rate) |
| On-Time Delivery | 25% | PO Receipt — Requested Date |
| Price Competitiveness | 20% | PO Price vs Market/Estimate |
| Responsiveness | 15% | RFQ Response Time, PO Confirmations |
| Compliance | 10% | Contract Compliance, Regulatory |

**Scorecard Output:**
- Green (85-100): Preferred Supplier
- Yellow (70-84): Monitor
- Red (<70): Improvement Plan / Disqualify

### 2.4 Supplier Qualification & Segmentation

Oracle يدعم **Qualification Management** عبر إجراء تقييم أولي قبل إدراج المورد:

```
Pre-Qualification → Self-Survey → Site Audit → Qualified
```

بعد التأهيل، يتم التصنيف وفق:
- **Commodity Type**: MRO vs Raw Material vs Service
- **Spend Volume**: High/Med/Low
- **Risk Profile**: High/Med/Low (نظام إدارة المخاطر)

---

## 3. Microsoft Dynamics 365 Vendor Management

### 3.1 Vendor Collaboration Portal

**Dynamics 365 Vendor Collaboration** هي بوابة مدمجة مع **Power Platform** تمكن الموردين من:

- الرد على طلبات العروض (RFQ)
- تأكيد أوامر الشراء
- إرسال ASN (إشعار الشحن المسبق)
- تقديم الفواتير
- عرض تقييم أدائهم

### 3.2 Vendor Evaluation عبر Power BI

**Power BI Vendor Performance Dashboard** يدمج بيانات الـ Purchase Orders، الـ Receiving، الـ Invoicing، و**الـ Quality Control** لإنتاج تقارير حية (Real-Time):

| المقياس | الصيغة | الهدف (KPI) |
|---------|--------|-------------|
| **On-Time Delivery %** | عدد الشحنات في الوقت ÷ إجمالي الشحنات × 100 | ≥ 95% |
| **Quality Reject Rate %** | عدد المواد المرفوضة ÷ إجمالي المستلم × 100 | ≤ 2% |
| **Price Variance %** | (السعر الفعلي − السعر المتفق عليه) ÷ المتفق عليه × 100 | ≤ 3% |
| **Invoice Accuracy %** | عدد الفواتير الصحيحة ÷ إجمالي الفواتير × 100 | ≥ 98% |
| **Lead Time Compliance** | الفرق بين Lead Time الفعلي والقياسي | ضمن ± 3 أيام |

### 3.3 Vendor Bank Accounts & Tax Information

- دعم **IBAN** و**SWIFT/BIC**
- سجلات متعددة للبنوك لكل Vendor
- التحقق التلقائي من **VAT ID** عبر خدمة حكومية (إن وجدت)
- ربط مع **Tax Authority Integration** للإبلاغ الضريبي

### 3.4 Vendor Performance Analytics (مثال Power BI)

```
Dashboard: "مؤشر أداء الموردين — قطاع المقاولات"

[On-Time Delivery]     [Quality Reject]      [Price Variance]
       94%                   1.8%                  2.1%

  [Top 5 Vendors by Score]
  1. شركة الحديد السعودي — 96
  2. مؤسسة الخرسانة — 92
  3. شركة النقل السريع — 87
  4. مصنع البلوك — 82
  5. شركة الأسمنت — 78

  [Vendors at Risk — Below 70]
  - مؤسسة النظافة العامة — 63 ⚠️
```

---

## 4. دورة حياة المورد (Supplier Lifecycle)

### 4.1 المراحل الكاملة

```
التسجيل → التأهيل → التصنيف → التعاقد → التقييم → التطوير أو الاستبعاد
   ↑                                                          |
   └──────────────────────── إعادة التسجيل ──────────────────┘
```

| المرحلة | الأنشطة | الأنظمة |
|---------|---------|---------|
| **1. Registration** | تقديم الطلب، إرفاق الشهادات، التحقق من السجل التجاري | Portal / Supplier Self-Service |
| **2. Qualification** | تقييم أولي، Audit ميداني، شهادات ISO/BCR | SRM / Qualification Module |
| **3. Classification** | تحديد الفئة (مادة/خدمة)، التصنيف (استراتيجي/عادي/احتياطي) | Vendor Master |
| **4. Contracting** | التفاوض، توقيع العقد، Frame Agreement | Contracts Module |
| **5. Evaluation** | تقييم دوري (شهري/ربع سنوي/سنوي)، Vendor Scorecard | Evaluation Engine |
| **6. Development / Exclusion** | برنامج تطوير الموردين، أو استبعاد (Blacklist) | Workflow |

### 4.2 إدارة المخاطر لكل مورد

| نوع المخاطرة | المؤشرات | الإجراء |
|-------------|----------|---------|
| **Financial Risk** | تأخر في السداد، إفلاس معلن | خفض الحد الائتماني |
| **Operational Risk** | تأخير تسليم متكرر، جودة متدنية | خطة تحسين إلزامية |
| **Compliance Risk** | مخالفات ضريبية/قانونية | تعليق التعامل |
| **Geopolitical Risk** | عدم استقرار بلد المنشأ | تنويع الموردين |

### 4.3 Blacklist / Whitelist

- **Whitelist:** موردون معتمدون مسبقاً لسرعة الطرح
- **Blacklist:** موردون ممنوعون لأسباب (تزوير، تأخير متكرر، عدم امتثال)
- يتم إدارة القوائم عبر:
  - SAP: `Blocking Indicator` في Vendor Master
  - Oracle: `Supplier Hold Status`
  - Dynamics 365: `Vendor Hold`

---

## 5. إدارة الموردين في المقاولات

### 5.1 خصوصية قطاع المقاولات

| التحدي | الوصف | الحل في ERP |
|--------|-------|-------------|
| **تعدد الموردين لكل مادة** | يمكن أن يكون للحديد 5 موردين في 3 مدن | Vendor per Plant + Source List |
| **تقييم في المشاريع** | أداء المورد يختلف من مشروع لآخر | Evaluation per Project + PO |
| **موردين محليين ودوليين** | فئات مختلفة من حيث العملة، الجمارك، زمن التوريد | Multi-Currency, Incoterms, Lead Time Groups |
| **دفعات مرحلية** | دفع حسب إنجاز المشروع (مثال: 30% عند الطلب + 70% عند التركيب) | Payment Terms مخصصة + Milestone Billing |
| **مقاولي الباطن (Subcontractors)** | يتم إدارتهم كموردي خدمات | Vendor Type = Subcontractor |

### 5.2 Source List — قائمة المصادر

في SAP، **Source List** (`ME01`) تحدد الموردين المسموح لهم لكل مادة في كل Plant:

```
مادة: حديد تسليح قطر 16mm
  Plant 2000 (الرياض):  مورد A, B
  Plant 3000 (جدة):     مورد A, C
  Plant 4000 (الدمام):  مورد C, D, E
```

- يساعد في توزيع الطلبات وتجنب الاحتكار
- يدعم **Quota Arrangement** لتوزيع الكميات حسب النسبة

### 5.3 Quota Arrangement — توزيع الحصص

| المورد | النسبة (%) | الكمية التراكمية (MT) |
|--------|-----------|----------------------|
| المورد A | 40% | 4000 |
| المورد B | 35% | 3500 |
| المورد C | 25% | 2500 |
| **الإجمالي** | **100%** | **10,000** |

يتم التوزيع تلقائياً عند إنشاء PO.

---

## 6. أفضل الممارسات (Best Practices)

### 6.1 Kraljic Matrix لتصنيف الموردين

مصفوفة **Kraljic** هي الأداة الأكثر استخداماً لتصنيف الموردين حسب **أثر الربح (Profit Impact)** و**مخاطر التوريد (Supply Risk)**:

```
                    Supply Risk (Low → High)
                    ───────────────────────
                    Low              High
         ┌───────────────────────────────────┐
    High  │  Leverage Items    │  Strategic   │
          │  (Multiple Suppliers│  Items       │
          │   — تسعير تنافسي)   │  (Partnership)│
          ├──────────────────────┼──────────────┤
    Low   │  Non-Critical Items │  Bottleneck  │
          │  (Standard, MRO)    │  (Scarce/    │
          │                     │   Monopoly)   │
          └───────────────────────────────────┘
  Profit
  Impact
  (Low → High)
```

| الربع | الموردون | الاستراتيجية |
|-------|----------|-------------|
| **Strategic** (استراتيجي) | قليلون، مواد حرجة | شراكة طويلة الأجل، عقود متعددة السنوات |
| **Leverage** (قوة تفاوضية) | متعددون، مواد عالية القيمة | مناقصات تنافسية، تحسين السعر |
| **Bottleneck** (عنق زجاجة) | وحدانية أو ندرة | تأمين مخزون احتياطي، تطوير بدائل |
| **Non-Critical** (غير حرج) | متوفر بكثرة | أتمتة الشراء، تقليل تكاليف الإدارة |

### 6.2 Supplier Relationship Management (SRM)

**أهداف SRM:**
1. تقليل تكاليف الشراء بنسبة 5-15%
2. تحسين جودة المواد بنسبة 10-20%
3. تقليل زمن التوريد بنسبة 20-30%
4. زيادة الابتكار من الموردين الاستراتيجيين

**مستويات العلاقة:**
```
Transactional (Basic)
       ↓
Collaborative (مشاركة معلومات)
       ↓
Strategic Alliance (تطوير مشترك)
       ↓
Co-creation (ابتكار مشترك — نادر)
```

### 6.3 Vendor Scorecard — الأوزان الموصى بها (للمقاولات)

| المعيار | الوزن الموصى به | مصدر البيانات | تكرار القياس |
|---------|----------------|---------------|-------------|
| **التسليم في الوقت (OTD)** | 25% | PO vs Receiving | شهرياً |
| **جودة المواد** | 25% | فحص الجودة (QC) | لكل شحنة |
| **السعر والتنافسية** | 20% | PO Price vs Estimated Cost | ربع سنوي |
| **الالتزام التعاقدي** | 15% | تنفيذ الشروط والعقد | شهرياً |
| **الاستجابة والمرونة** | 10% | سرعة الرد، التعديلات | شهرياً |
| **السلامة والمخاطر** | 5% | HSE Reports, Incidents | شهرياً |
| **الإجمالي** | **100%** | | |

### 6.4 Supplier Audits

| نوع المراجعة | التكرار | الغرض |
|-------------|---------|-------|
| **On-Site Audit** | سنوي للموردين الاستراتيجيين | تقييم المصنع، الجودة، السلامة |
| **Desk Audit** | سنوي للموردين العاديين | مراجعة الشهادات والتقارير |
| **Financial Audit** | حسب الحاجة | تقييم الملاءة المالية |
| **Compliance Audit** | مع كل عقد | التزام باللوائح والعقود |

**نموذج تقييم Audit:**

```
المورد: ____________     التاريخ: ____________
الموقع: ____________     المقيّم: ____________

البند                          الدرجة (1-5)   الملاحظات
نظام إدارة الجودة ISO 9001       4
السلامة والصحة المهنية           3
الالتزام بالمواصفات              5
وقت التوريد                      4
الاستقرار المالي                 3
----------------------------------------------
المجموع:                       19/25 = 76%
التوصية: ✅ معتمد /  ⏸️ مراقبة /  ❌ استبعاد
```

### 6.5 مؤشرات الأداء الرئيسية (KPIs) الموصى بها

| KPI | المعادلة | الهدف | القياس |
|-----|----------|-------|--------|
| **Vendor Delivery Performance** | شحنات في الوقت ÷ إجمالي الشحنات | ≥ 95% | شهري |
| **Quality Acceptance Rate** | مقبول ÷ إجمالي المستلم | ≥ 98% | أسبوعي |
| **Cost Variance** | (سعر فعلي − سعر متوقع) ÷ متوقع | ≤ ±5% | شهري |
| **PO Cycle Time** | أيام من أمر الشراء إلى الاستلام | ≤ 21 يوم | شهري |
| **Supplier Fill Rate** | كمية مستلمة ÷ كمية مطلوبة | ≥ 95% | شهري |
| **Vendor Incident Rate** | عدد الحوادث ÷ عدد الشحنات | ≤ 1% | ربع سنوي |

---

## الملخص التنفيذي

1. **SAP** هو الأقوى في إدارة Vendor Master بـ BP Approach ونظام تقييم تلقائي متكامل.
2. **Oracle** يتفوق في Supplier Portal (iSupplier) و Scorecards مع تكامل قوي مع Procurement.
3. **Dynamics 365** يقدم أفضل تقارير أداء عبر Power BI مع مرونة عالية.
4. **للمقاولات**، يجب التركيز على Source List + Quota Arrangement + تقييم لكل مشروع.
5. **Kraljic Matrix** و **Vendor Scorecard** هما الأداتان الأساسيتان لتصنيف وتقييم الموردين.

---

*تم إعداد هذا التقرير بواسطة DomainResearchAgent — R03: إدارة الموردين وتقييمهم*
