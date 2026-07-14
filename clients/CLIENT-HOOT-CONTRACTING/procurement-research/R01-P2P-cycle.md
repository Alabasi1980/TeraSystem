# تقرير بحث المجال — Domain Research Report R01
## دورة المشتريات الكاملة (Procure-to-Pay / P2P Cycle)

**تاريخ التقرير:** يوليو 2026  
**النسخة:** 1.0  
**الأنظمة المدروسة:** SAP MM, Oracle Procurement Cloud, Microsoft Dynamics 365 Supply Chain Management

---

## فهرس المحتويات

1. الملخص التنفيذي
2. دورة P2P في SAP MM
3. دورة P2P في Oracle Procurement Cloud
4. دورة P2P في Microsoft Dynamics 365 Supply Chain Management
5. Three-Way Matching بالتفصيل
6. أفضل الممارسات العالمية في دورة P2P
7. خصوصية دورة المشتريات في شركات المقاولات
8. جدول مقارنة بين الأنظمة الثلاثة
9. المصطلحات الفنية الأساسية
10. التوصيات لقطاع المقاولات

---

## 1. الملخص التنفيذي

دورة **Procure-to-Pay (P2P)** — أو دورة المشتريات الكاملة — هي العملية المتكاملة التي تبدأ من تحديد الحاجة لشراء سلعة أو خدمة وتنتهي بدفع الفاتورة للمورد. تدمج هذه الدورة بين إدارتي المشتريات (Procurement) والحسابات الدائنة (Accounts Payable)، وتُعتبر العمود الفقري لأي نظام ERP حديث.

تغطي هذه الدورة ستة أنشطة رئيسية وفقاً لتعريف Wikipedia و IBM:
1. Supply Management (إدارة التوريد)
2. Cart / Requisition (طلب الشراء)
3. Purchase Order (أمر الشراء)
4. Receiving (الاستلام)
5. Invoice Reconciliation (مطابقة الفاتورة)
6. Accounts Payable (الحسابات الدائنة / الدفع)

> **ملاحظة:** تختلف P2P عن Source-to-Pay (S2P) بأن الأخيرة تشمل أنشطة التوريد الاستراتيجي (Sourcing) والتعاقد (Contracting) قبل مرحلة أمر الشراء.

---

## 2. دورة P2P في SAP MM

### 2.1 الخطوات الكاملة

تعمل دورة P2P في SAP MM (Materials Management) عبر الوحدات التالية: MM (المشتريات والمخزون)، FI (المالية)، وWM (إدارة المستودعات).

#### الخطوة 1: Purchase Requisition (طلب الشراء) — TCode: ME51N
- يقوم المستخدم (طالب الشراء) بإنشاء طلب شراء عبر TCode **ME51N**
- يحتوي الطلب على: المادة (Material)، الكمية، تاريخ التسليم المطلوب، مركز التكلفة (Cost Center)
- يمكن أن يكون الطلب لعنصر مخزون (Stock Item) أو استهلاك مباشر (Consumable)

#### الخطوة 2: Release Strategy (استراتيجية الموافقة)
- تمر طلبات الشراء عبر عملية موافقات متعددة المستويات تعتمد على:
  - قيمة الطلب (Value-based)
  - نوع المادة (Material Group)
  - مركز التكلفة (Cost Center)
- تُكوّن استراتيجية الموافقة (Release Strategy) عبر TCode **ME28** للموافقة الجماعية
- TCode **ME29N** للموافقة الفردية
- لكل مستوى موافقة، يتم تعيين حد أقصى (Release Limit) بحسب صلاحية المفوض

#### الخطوة 3: Source Determination (تحديد المصدر)
- SAP يحدد المصادر تلقائياً عبر:
  - Quota Arrangement (توزيع الحصص) — TCode: MEQ1
  - Source List (قائمة المصادر) — TCode: ME01
  - Outline Agreement (اتفاقيات الإطار)
  - Info Record (سجل المعلومات) — TCode: ME11

#### الخطوة 4: Purchase Order (أمر الشراء) — TCode: ME21N
- تحويل طلب الشراء إلى أمر شراء أو إنشاء مباشر
- تحديث المخزون الافتراضي (ATP — Available to Promise)
- إرسال الأمر للمورد عبر EDI أو SAP Ariba Network

**أنواع أوامر الشراء في SAP MM:**
| النوع | الوصف | TCode |
|-------|-------|-------|
| Standard PO (NB) | أمر شراء قياسي لمرة واحدة | ME21N |
| Contract (MK) | اتفاقية إطارية بكميات/قيم محددة دون تواريخ تسليم | ME31K |
| Scheduling Agreement (LP) | اتفاقية جدولة بتواريخ تسليم محددة ومحدثة دورياً | ME31L |
| Outline Agreement | مظلة تعاقدية تشمل Contract وScheduling Agreement |

**الفرق بين الأنواع:**
- **Standard PO:** يستخدم لمرة واحدة، يُغلق بعد الاستلام والدفع
- **Contract:** اتفاقية طويلة الأجل بقيمة إجمالية، يتم تحرير Release Orders (TCode: ME2O) عند الحاجة
- **Scheduling Agreement:** يستخدم في التصنيع المتكرر (JIT)، يُرسل جداول تسليم محدثة دورياً (Forecast Delivery Schedule / JIT Delivery Schedule)

#### الخطوة 5: Goods Receipt (استلام البضائع) — TCode: MIGO
- إدخال استلام البضائع في النظام
- زيادة المخزون مادياً ومحاسبياً (Inventory Valuation)
- TCode **MIGO** للاستلام (مع خيارات: GR, GI, Transfer)
- TCode **MB03** لعرض سجل الحركة
- يؤدي إلى ترحيل محاسبي: Dr. Inventory / Cr. GR/IR Clearing Account

#### الخطوة 6: Invoice Receipt (استلام الفاتورة) — TCode: MIRO
- إدخال فاتورة المورد
- TCode **MIRO** (Logistics Invoice Verification)
- يتم مطابقة الفاتورة مع أمر الشراء وإذن الاستلام
- يؤدي إلى ترحيل: Dr. GR/IR Clearing Account / Cr. Vendor Account

#### الخطوة 7: Three-Way Matching (المطابقة الثلاثية)
- تتم المطابقة بين:
  1. Purchase Order (الكمية والسعر)
  2. Goods Receipt (الكمية المستلمة)
  3. Invoice Receipt (الكمية والمبلغ في الفاتورة)
- يتم تحديد التوليرانس (Tolerance Limits) عبر TCode **OMR6**
- إذا تجاوزت الفروقات الحدود المسموحة → يتم تعليق الفاتورة (Invoice Block) → TCode **MRBR** لإزالة التعليق

#### الخطوة 8: Payment (الدفع) — TCode: F110
- بعد اعتماد الفاتورة، تتم جدولة الدفع
- TCode **F110** (Automatic Payment Program)
- يتم إنشاء دفتر الشيكات/التحويل البنكي
- ترحيل: Dr. Vendor Account / Cr. Bank

### 2.2 أهم Transaction Codes في SAP MM للمشتريات

| الرمز | الوظيفة |
|-------|---------|
| **ME51N** | إنشاء Purchase Requisition |
| **ME52N** | تعديل Purchase Requisition |
| **ME53N** | عرض Purchase Requisition |
| **ME54N** | الموافقة على Purchase Requisition |
| **ME21N** | إنشاء Purchase Order |
| **ME22N** | تعديل Purchase Order |
| **ME23N** | عرض Purchase Order |
| **ME28** | موافقة جماعية على Purchase Orders |
| **ME29N** | موافقة فردية على Purchase Order |
| **ME31K** | إنشاء Contract |
| **ME31L** | إنشاء Scheduling Agreement |
| **ME01** | صيانة Source List |
| **ME11** | إنشاء Info Record |
| **MEQ1** | إدارة Quota Arrangement |
| **MIGO** | استلام البضائع (Goods Receipt) |
| **MIRO** | إدخال فاتورة المورد |
| **MRBR** | إصدار فاتورة معلقة |
| **MIR4** | عرض سجل الفاتورة |
| **F110** | الدفع التلقائي |
| **ME2L** | تقرير أوامر الشراء حسب المورد |
| **ME2M** | تقرير أوامر الشراء حسب المادة |
| **ME2N** | تقرير أوامر الشراء حسب رقم المستند |
| **MB03** | عرض سجل حركة المخزون |
| **OMR6** | تعريف حدود المطابقة (Tolerance Limits) |

---

## 3. دورة P2P في Oracle Procurement Cloud

### 3.1 الخطوات الكاملة

يعمل Oracle Procurement Cloud كجزء من Oracle Fusion Cloud ERP. الدورة أوسع من SAP MM لأنها تتكامل مع Self-Service Procurement وSourcing وContracts.

#### الخطوة 1: Self-Service Procurement (المشتريات الذاتية)
- واجهة **Oracle Self-Service Procurement** (واجهة Redwood الحديثة)
- يقوم الموظف بإنشاء طلب شراء (Requisition) من كتالوج إلكتروني (Catalog) أو طلب مخصص (Non-Catalog Request)
- يدعم **SmartSearch** المدعوم بالذكاء الاصطناعي للبحث عن المنتجات
- يمكن إرفاق مبررات الشراء (Justification) ووثائق الدعم

#### الخطوة 2: Approval Workflow (سير عمل الموافقة)
- **Oracle Approval Management Engine (AME)** — محرك الموافقات المدمج
- دعم:
  - الموافقات المتسلسلة (Sequential)
  - الموافقات المتوازية (Parallel)
  - الموافقات الشرطية (Conditional based on amount, category, cost center)
  - الموافقات عبر الجوال (Mobile Approvals)

#### الخطوة 3: Sourcing (التوريد)
- إذا كان الطلب يتطلب sourcing → يتم تحويله إلى **Oracle Sourcing**
- إنشاء منافسة (Sourcing Event): RFI, RFP, RFQ, Auction
- تقييم العروض إلكترونياً
- ترسية المنافسة (Award) تنشئ Contract أو PO تلقائياً

#### الخطوة 4: Contract Management (إدارة العقود)
- **Oracle Procurement Contracts** ينشئ اتفاقية شراء (Purchase Agreement)
- أنواع العقود: Contract Purchase Agreement, Blanket Purchase Agreement
- إدارة الشروط والأحكام والملحقات

#### الخطوة 5: Purchase Order (أمر الشراء)
- إنشاء PO يدوياً أو تلقائياً من Requisition أو من Contract
- أنواع أوامر الشراء:
  - **Standard PO** — أمر شراء قياسي
  - **Blanket PO** — أمر إطاري بقيمة إجمالية
  - **Contract PO** — مرتبط بعقد خدمات
  - **Planned PO** — أمر مخطط (يُحوَّل لاحقاً إلى Standard PO)
- إرسال PO إلكترونياً للمورد عبر **Oracle Supplier Portal**

#### الخطوة 6: Receiving (الاستلام)
- **Oracle Receiving** — استلام البضائع والخدمات
- دعم:
  - الاستلام الكامل (Full Receipt)
  - الاستلام الجزئي (Partial Receipt)
  - استلام الخدمات (Service Receipt) مع اعتماد إنجاز العمل
  - استلام متقدم (ASN — Advanced Shipment Notification)
- يدعم **Mobile Receiving** عبر تطبيق الجوال

#### الخطوة 7: Matching (المطابقة)
- **Oracle Payables Matching** — مطابقة ثلاثية:
  - PO Match (مطابقة مع أمر الشراء)
  - Receipt Match (مطابقة مع إذن الاستلام)
  - الحالة الخاصة: خدمات بدون استلام مادي يتم مطابقتها مع PO + قبول الخدمة
- يتم عبر **Invoice Workbench** أو **AP Invoice Automation**

#### الخطوة 8: Smart Tax (المعالجة الضريبية الذكية)
- **Oracle Tax Cloud** — معالجة ضريبية آلية
- تحديد ضريبة القيمة المضافة (VAT) تلقائياً حسب:
  - موقع المورد والمستلم
  - نوع المادة/الخدمة
  - قوانين الضرائب المحلية
  - اتفاقيات التجارة الدولية
- يتم حساب الضريبة على الفاتورة تلقائياً مع تحديث القوانين الضريبية بشكل دوري

#### الخطوة 9: Payment (الدفع)
- **Oracle Payments** — جدولة الدفع
- دعم قنوات دفع متعددة: تحويل بنكي (EFT)، بطاقة ائتمان، شيك
- **Payment Manager** — إدارة شروط الدفع وخصومات الدفع المبكر (Early Payment Discount)
- **Supplier Portal** يتيح للمورد عرض حالة الدفع

### 3.2 Oracle Supplier Portal (بوابة الموردين)
- بوابة تفاعلية تتيح للموردين:
  - عرض أوامر الشراء (POs)
  - تأكيد أوامر الشراء (PO Acknowledgment)
  - تحديث حالة الشحن (ASN)
  - إرسال الفواتير إلكترونياً (E-Invoicing)
  - عرض حالة الدفع
  - التواصل مع فريق المشتريات

---

## 4. دورة P2P في Microsoft Dynamics 365 Supply Chain Management

### 4.1 الخطوات الكاملة

Dynamics 365 Supply Chain Management يتكامل مع **Finance & Operations** ويوفر دورة P2P متكاملة مع قدرات ذكاء اصطناعي متقدمة.

#### الخطوة 1: Purchase Requisition (طلب الشراء)
- يتم إنشاء طلب شراء عبر **Procurement and Sourcing** > **Purchase Requisitions**
- أنواع الطلبات:
  - طلب من كتالوج (From Catalog)
  - طلب غير كتالوج (Non-Catalog)
  - طلب استهلاك مباشر (Direct Consumption)
  - طلب مخزون (Replenishment)
- دعم **Requisition Purpose**: Consumption (استهلاك مباشر) / Replenishment (تزويد مخزون)

#### الخطوة 2: Approval Workflow (سير عمل الموافقة)
- **Dynamics 365 Workflow System** — محرك موافقات متقدم
- أنواع الموافقات:
  - الموافقة التسلسلية (Sequential)
  - الموافقة المتوازية (Parallel)
  - الموافقة حسب الخطوات (Stage-based)
  - الموافقة الشرطية (Conditional: amount, category, legal entity)
- دعم الموافقة عبر **Teams** و **Power Apps Mobile**

#### الخطوة 3: Purchase Order (أمر الشراء) — **Procurement and Sourcing** > **Purchase Orders**
- يتم إنشاء PO يدوياً أو تلقائياً من Requisition أو من **Master Planning** (التخطيط الرئيسي)
- دعم **Purchase Order Types**:
  - **Purchase Order** (أمر شراء قياسي)
  - **Purchase Agreement** (اتفاقية شراء إطارية)
  - **Requisition** (يُستخدم للتحويل)
- دعم **Charges** (المصاريف الإضافية: شحن، تأمين، جمارك) عبر **Charges Code**
- إرسال PO للمورد عبر **Vendor Collaboration** أو **EDI**

#### الخطوة 4: Vendor Confirmation (تأكيد المورد)
- عبر **Vendor Collaboration Portal** (بوابة تعاون الموردين)
- يستطيع المورد:
  - عرض PO
  - قبول أو رفض PO
  - اقتراح تاريخ تسليم بديل
  - إرسال تأكيد
- تكامل مع **Power BI** لتقارير الموردين

#### الخطوة 5: Product Receipt (استلام المنتج)
- **Product Receipt** — استلام البضائع مع تحديث المخزون
- دعم:
  - استلام كامل / جزئي
  - **Registration** (تسجيل الوارد) قبل التحديث الكامل
  - **Quality Management** — فحص الجودة مع تمرير/رفض
  - **Warehouse Management (WMS)** — استلام عبر الماسح الضوئي (RF Scanner)

#### الخطوة 6: Invoice and Matching (الفاتورة والمطابقة)
- **Accounts Payable** > **Invoice Journal** — إدخال الفاتورة
- **Invoice Matching** — ثلاثة أنواع:
  - **Two-Way Matching** (PO + Invoice)
  - **Three-Way Matching** (PO + Product Receipt + Invoice)
  - **Charges Matching** (مطابقة المصاريف الإضافية)
- **Invoice Matching Policy** تُحدد عند مستوى الشركة أو المورد
- **Invoice Automation** — إدخال الفواتير تلقائياً عبر OCR و AI

#### الخطوة 7: Payment (الدفع)
- **Payment Journal** (يومية الدفع)
- دعم:
  - **Payment Proposal** (اقتراح الدفع التلقائي)
  - **Payment Schedule** (جدولة الدفع حسب شروط السداد)
  - **Early Payment Discount** (خصم الدفع المبكر)
  - **Vendor Settlement** (تسوية حسابات المورد)
- دفع عبر EFT, Checks, Credit Card

### 4.2 Procurement Agent (AI) — وكيل المشتريات الذكي
ميزة جديدة في Dynamics 365 تستخدم **AI Copilot** و **Azure OpenAI**:
- **Proactive suggestions** — اقتراحات استباقية لإنشاء أوامر الشراء
- **Automated matching** — مطابقة ذكية للفواتير مع اكتشاف الحالات الشاذة (Anomaly Detection)
- **Natural language queries** — استعلامات باللغة الطبيعية عن حالة الطلبات
- **Supplier recommendations** — توصية بموردين بديلين بناءً على الأداء السابق
- **Contract compliance checks** — التحقق من الالتزام بالعقود تلقائياً

### 4.3 Vendor Collaboration Portal (بوابة تعاون الموردين)
- مساحة عمل آمنة للموردين
- الميزات:
  - عرض جميع POs
  - إرسال تأكيدات (Acknowledgments)
  - تحديث ASN (Advanced Shipment Notices)
  - فتح ونشر الطلبات (Purchase Orders)
  - متابعة حالة الدفع
  - إرسال فواتير إلكترونية
  - إدارة بيانات المورد الأساسية

---

## 5. Three-Way Matching بالتفصيل

### 5.1 المفهوم الأساسي

**Three-Way Matching** هي عملية محاسبية وأساسية في أي نظام ERP للمشتريات. تقوم بمطابقة ثلاث وثائق للتأكد من صحة الفاتورة قبل دفعها:

```
                     ┌─────────────────────────┐
                     │   Purchase Order (PO)   │
                     │   أمر الشراء             │
                     │   (الكمية المتعاقد عليها) │
                     └────────────┬────────────┘
                                  │
           ┌──────────────────────┼──────────────────────┐
           │                      │                      │
           ▼                      ▼                      ▼
┌────────────────────┐  ┌────────────────────┐  ┌────────────────────┐
│  Goods Receipt      │  │  Three-Way Match   │  │  Invoice Receipt   │
│  إذن الاستلام       │──▶  المطابقة الثلاثية   ◀──│  فاتورة المورد     │
│  (الكمية المستلمة)  │  │                     │  │  (المبلغ المطلوب)  │
└────────────────────┘  └────────────────────┘  └────────────────────┘
```

**المقارنة بين العناصر الثلاثة:**
| العنصر | المقارنة | مثال خطأ |
|--------|----------|----------|
| الكمية (Quantity) | PO Qty = GR Qty = Invoice Qty | تم استلام 100 وحدة وفوّرت 120 |
| السعر (Price) | PO Price = Invoice Price | تم الاتفاق على 10$ وفوّرت 12$ |
| القيمة (Total Amount) | PO Total = Invoice Total | اختلاف في إجمالي المبلغ |
| الضريبة (Tax) | حسب القوانين الضريبية | خطأ في حساب VAT |

### 5.2 كيف يعمل في كل نظام؟

| الخطوة | SAP MM | Oracle Procurement | Dynamics 365 |
|--------|--------|--------------------|--------------|
| إعداد التوليرانس | TCode OMR6 | Matching Rules | Invoice Matching Policy |
| تنفيذ المطابقة | تلقائي عند MIRO | Invoice Workbench | تلقائي عند Posting |
| تعليق الفاتورة | Invoice Block (MRBR) | Holds & Approvals | Invoice Hold |
| إصدار التعليق | MRBR | Approve Hold | Release Hold |

### 5.3 ماذا يحدث عند عدم التطابق (Discrepancy)?

عند وجود فرق بين أي من الوثائق الثلاث، يقوم النظام بالآتي:

**في SAP MM:**
- يتم تعليق الفاتورة (Invoice Block) مع إشارة إلى سبب التعليق
- عدم التطابق في الكمية: يتم تعليق الفرق فقط (Quantity Block)
- عدم التطابق في السعر: يتم تعليق الفاتورة إذا تجاوزت الحد المسموح (Tolerance Limit)
- يمكن تكوين النظام لقبول نسبة خطأ معينة (مثل: ±5% في السعر)
- تصدر تقارير مثل **MRBR** لمراجعة الفواتير المعلقة

**في Oracle Procurement:**
- يضع الفاتورة في حالة **Held** pending investigation
- استخدام **Approval Rules** لتحديد من يمكنه الموافقة على الفروقات
- **Matches** > **Approve** أو **Reject**
- تسجيل ملاحظات التسوية (Dispute Notes)

**في Dynamics 365:**
- الفاتورة تدخل في **Invoice Hold** state
- استخدام **Invoice Matching Details** لرؤية الفروقات
- يمكن للمستخدم تجاوز التعليق (Override hold) إذا كان الفرق مبرراً
- إعداد **Tolerance percentage** في **Accounts payable parameters**

### 5.4 الفرق بين 2-Way و 3-Way Matching

| المعيار | 2-Way Matching | 3-Way Matching |
|---------|---------------|----------------|
| الوثائق المطابقة | PO + Invoice | PO + GR + Invoice |
| مستوى الرقابة | أقل — لا يتحقق من الاستلام الفعلي | أعلى — يتأكد من استلام البضاعة |
| مناسَب لـ | الخدمات والمصاريف غير المخزنية | المواد الخام، البضائع المخزنية |
| مخاطر الدفع الزائد | موجودة (قد يُدفع لمواد لم تستلم) | أقل بكثير |
| وقت المعالجة | أسرع | أبطأ (يتطلب استلام قبل المطابقة) |
| التطبيق في SAP | يمكن تخطي GR إذا المادة غير مخزنية | إلزامي للمواد المخزنية |
| التطبيق في Oracle | يُستخدم مع الخدمات فقط | يُستخدم مع المواد |
| التطبيق في D365 | يُحدد في Policy لكل مورد/فئة | يُحدد في Policy |

---

## 6. أفضل الممارسات العالمية في دورة P2P

### 6.1 من CIPS (Chartered Institute of Procurement & Supply)

**مبادئ CIPS الخمسة:**
1. **Right Quality** — الجودة المناسبة: شراء ما يناسب الغرض دون إسراف
2. **Right Quantity** — الكمية المناسبة: تجنب التخزين الزائد أو النقص
3. **Right Place** — المكان المناسب: تسليم للموقع الصحيح
4. **Right Time** — الوقت المناسب: تجنب التأخير أو التسليم المبكر
5. **Right Price** — السعر المناسب: أفضل قيمة إجمالية (Total Cost of Ownership)

**CIPS تضيف "المصدر المناسب" (Right Source)** كحق سادس في إصدار 2018 من Contract Administration.

### 6.2 من ISM (Institute for Supply Management)

**ISM Principles:**
- **Supply Chain Integration** — دمج سلسلة التوريد مع استراتيجية المؤسسة
- **Risk Management** — إدارة مخاطر الموردين والتعاقدات
- **Data-Driven Decisions** — قرارات مبنية على البيانات والتحليلات
- **Supplier Relationship Management (SRM)** — إدارة علاقات الموردين كشركاء استراتيجيين

### 6.3 ممارسات متخصصة

**Deloitte — تحديات P2P:**
- عدم تكامل الأنظمة (Multiple/unintegrated systems)
- ضعف جودة البيانات (Vendor Master Data)
- عدم كفاية التدريب على ERP
- التخصيص المفرط للنظام (Customization)
- نقص التحليلات المالية لدعم القرارات

**Aberdeen Group — فوائد الأتمتة:**
- تقليل وقت دورة P2P بنسبة 50-70%
- تقليل تكلفة معالجة الفاتورة بنسبة 80%
- تحسين نسبة الخصم المبكر (Early Payment Discount)
- زيادة نسبة الالتزام بالعقود (Contract Compliance) إلى 95%+

### 6.4 مؤشرات الأداء لدورة P2P (KPIs)

| المؤشر (KPI) | المعادلة | الهدف المثالي |
|-------------|----------|--------------|
| **Procurement Cycle Time** | وقت إنشاء PO ÷ عدد POs | < 24 ساعة |
| **PO Accuracy Rate** | POs بدون أخطاء ÷ إجمالي POs | > 98% |
| **Invoice Error Rate** | فواتير بأخطاء ÷ إجمالي الفواتير | < 2% |
| **Three-Way Match Rate** | فواتير مطابقة تلقائياً ÷ الإجمالي | > 90% |
| **Touchless Processing Rate** | فواتير بدون تدخل يدوي ÷ الإجمالي | > 70% |
| **Days Payable Outstanding (DPO)** | (المدفوعات التجارية ÷ تكلفة المشتريات) × 365 | 45-60 يوم |
| **Cost per Invoice** | تكلفة معالجة الفاتورة | < 10$ |
| **Contract Compliance** | مشتريات ضمن العقود ÷ إجمالي المشتريات | > 90% |
| **Supplier On-Time Delivery** | شحنات في الوقت المحدد ÷ إجمالي الشحنات | > 95% |
| **Purchase Requisition to PO Time** | وقت التحويل | < 4 ساعات |
| **Spend Under Management** | المشتريات المدارة ÷ إجمالي المشتريات | > 80% |
| **ROSM (Return on Supply Management Assets)** | الأرباح ÷ تكلفة إدارة التوريد | متغير |

---

## 7. خصوصية دورة المشتريات في شركات المقاولات

### 7.1 كيف تختلف عن التصنيع؟

تختلف دورة P2P في **Construction** (المقاولات) جوهرياً عن التصنيع (Manufacturing) بعدة جوانب:

| المعيار | التصنيع (Manufacturing) | المقاولات (Construction) |
|---------|------------------------|--------------------------|
| طبيعة المواد | BOM محدد، مواد خام متكررة | متعددة المشاريع، مواد خاصة بكل مشروع |
| الشراء حسب | Production Plan / MRP | **BoQ** و Project Schedule |
| التسليم | إلى مستودع مركزي | إلى موقع المشروع (Project Site) |
| الاستلام | في مستودع مركزي | في موقع المشروع بموافقة مهندس الموقع |
| Service Entry | نادر (خدمات قليلة) | شائع جداً (مقاولي الباطن، استشارات، تأجير معدات) |
| توزيع التكلفة | Cost Center | **WBS Element** + Activity |
| المطابقة | 3-Way قياسي | 3-Way + Service Entry Sheet (SES) |
| التخزين | مخزون دائم | نقل مباشر لموقع المشروع (Project Stock) |
| Schedule | حسب MRP | حسب **Project Schedule** و **Milestones** |

### 7.2 الربط مع Project Systems

#### SAP PS (Project System)
- الربط بين **SAP MM** و **SAP PS** يتم عبر:
  - **WBS Element** (Work Breakdown Structure) في كل طلب شراء وأمر شراء
  - يتم توزيع تكلفة المشتريات على WBS Element تلقائياً عند GR
  - **Network & Activities** لربط المشتريات بجدول المشروع
  - **Project Stock** (مخزون المشروع) — مواد مخصصة لمشروع معين لا تظهر في المخزون العام
- TCode **CJ20N** لعرض هيكل المشروع
- TCode **CJ30** لتوزيع الميزانية على WBS Elements
- **Commitment Management** — حجز التكاليف عند تحرير PO

#### Oracle Project Costing
- ربط المشتريات بـ **Oracle Project Costing** عبر:
  - **Project + Task + Expenditure Type** في PO Lines
  - **Capitalization** — رسملة التكاليف على المشروع
  - **Billing** — إمكانية فوترة العميل مباشرة من المشتريات (إذا كانت قابلة للفوترة)
  - **Budget Control** — التحكم في الميزانية حسب WBS

#### Dynamics 365 Project Operations
- التكامل بين **Dynamics 365 Finance** و **Project Operations**:
  - **Project-based POs** — أمر شراء مرتبط بـ Project و Activity
  - **Project Budget Control** — التحكم في ميزانية المشروع
  - **Resource Management** — إدارة موارد المشروع (مقاولي باطن، معدات)
  - **Time & Material vs Fixed Price** — أنواع العقود مع العملاء تؤثر على طبيعة المشتريات

### 7.3 تأثير BoQ (Bill of Quantities) على المشتريات

**BoQ** هو الوثيقة الأساسية في المقاولات التي تحدد كميات المواد والأعمال لكل بند في المشروع.

**كيف يؤثر على P2P:**
1. **تقسيم المشتريات حسب بنود BoQ:**
   - كل بند في BoQ يتوافق مع WBS Element
   - يتم الشراء بناءً على كميات BoQ وليس بناءً على MRP

2. **شراء المواد حسب مراحل المشروع:**
   - BoQ يحدد الجدول الزمني لشراء كل مادة
   - يتم إصدار POs وفقاً لـ **Project Schedule** (جدول المشروع)

3. **مطابقة الكميات المستلمة مع BoQ:**
   - عند استلام المواد، يجب أن تتطابق مع البند المحدد في BoQ
   - أي زيادة عن BoQ تحتاج إلى **Change Order / Variation Order**

4. **فواتير المقاولين من الباطن (Subcontractor Invoices):**
   - تعتمد على **Measured Quantities** (الكميات المُنجزة فعلياً) وليس على الفاتورة التقليدية
   - الربط مع **Service Entry Sheets (SES)** في SAP

5. **التحديث الدوري لـ BoQ:**
   - كل Variation Order (VO) يغير في BoQ
   - يؤدي إلى تغيير في المشتريات المخططة

6. **تحديات خاصة:**
   - **Price Escalation** — تغير أسعار المواد (حديد، أسمنت) أثناء المشروع
   - **Bulk Materials** — مواد سائبة يصعب قياسها بدقة (خرسانة، رمل)
   - **Site Theft & Damage** — سرقة أو تلف المواد في موقع العمل

---

## 8. جدول مقارنة بين الأنظمة الثلاثة

| الخطوة / الميزة | SAP MM | Oracle Procurement Cloud | Dynamics 365 SCM |
|----------------|--------|------------------------|------------------|
| **Purchase Requisition** | ME51N / ME51 | Self-Service Procurement | Procurement and Sourcing |
| **Approval Engine** | Release Strategy (ME28/ME29N) | AME (Approval Management Engine) | Power Automate / Workflows |
| **Purchase Order** | ME21N — Standard/Contract/Sched. | Standard/Blanket/Contract/Planned | PO / Purchase Agreement |
| **3-Way Matching** | MIRO + OMR6 for tolerances | Invoice Workbench + Matching Rules | Invoice Matching Policy |
| **Goods Receipt** | MIGO | Oracle Receiving | Product Receipt / Registration |
| **Invoice Entry** | MIRO | Invoice Workbench | Invoice Journal / Automation |
| **Payment** | F110 (Auto Payment Program) | Oracle Payments | Payment Journal / Proposal |
| **Supplier Portal** | SAP Ariba Network / Supplier Self-Service | Oracle Supplier Portal | Vendor Collaboration Portal |
| **AI/ML Capabilities** | SAP AI Core / Ariba AI | Oracle AI (Smart Search, Smart Tax) | Copilot / Procurement Agent |
| **Cloud/On-Prem** | On-Prem / Cloud (S4/HANA Cloud) | Cloud Only (SaaS) | Cloud Only (SaaS) |
| **Integration with Project** | SAP PS (CJ20N, WBS) | Oracle Project Costing | Dynamics Project Operations |
| **Tax Handling** | Fiori / SAP Tax | Oracle Smart Tax (Tax Cloud) | Dynamics Tax Engine |
| **Mobile Support** | SAP Fiori Mobile | Oracle Mobile | Power Apps / Teams |
| **E-Invoicing** | SAP Document Compliance | Oracle E-Invoicing (P2P) | E-Invoice Integration |
| **Sourcing** | SAP Ariba (منتج منفصل) | Oracle Sourcing (مدمج) | Dynamics Sourcing (مدمج حديثاً) |
| **Contract Management** | SAP Ariba Contracts | Oracle Procurement Contracts | Purchase Agreements |
| **Vendor Master** | XK01/XK02/XK03 | Supplier Management | Vendors module |
| **Quality Management** | QM Module (TCode QM01) | Oracle Quality | Quality Management |
| **Multisite/Project Stock** | Project Stock + WBS | Project + Task + Expenditure | Project inventory |
| **Budget Control** | Availability Control (CJ30) | Budget Control (Project Costing) | Project Budget Control |

---

## 9. المصطلحات الفنية الأساسية مع تعريفاتها

| المصطلح | الترجمة | التعريف |
|---------|---------|---------|
| **Procure-to-Pay (P2P)** | دورة المشتريات الكاملة | العملية من تحديد الحاجة حتى دفع الفاتورة |
| **Purchase Requisition (PR)** | طلب شراء | وثيقة داخلية تطلب شراء سلعة أو خدمة |
| **Purchase Order (PO)** | أمر شراء | عقد ملزم قانونياً مع المورد لشراء سلع/خدمات |
| **Goods Receipt (GR)** | استلام بضائع | إثبات استلام المواد في المستودع |
| **Invoice Receipt (IR)** | استلام فاتورة | تسجيل فاتورة المورد في النظام |
| **Three-Way Matching (3-WM)** | المطابقة الثلاثية | مطابقة PO × GR × IR قبل الدفع |
| **Two-Way Matching (2-WM)** | المطابقة الثنائية | مطابقة PO × IR بدون GR |
| **Release Strategy** | استراتيجية الموافقة | قواعد الموافقات متعددة المستويات حسب القيمة والصلاحية |
| **Release Order** | أمر صادر عن عقد | أمر شراء يُحرَّر ضد Contract موجود |
| **Scheduling Agreement** | اتفاقية جدولة | تعاقد طويل الأجل مع جداول تسليم محدثة دورياً |
| **Outline Agreement** | اتفاقية إطارية | مظلة تعاقدية تشمل Contract و Scheduling Agreement |
| **Info Record** | سجل معلومات المادة-المورد | بيانات العلاقة التجارية بين مادة معينة ومورد معين |
| **Source List** | قائمة المصادر | قائمة الموردين المعتمدين لمادة معينة |
| **Quota Arrangement** | توزيع الحصص | توزيع المشتريات بين موردين بنسب محددة |
| **ATP (Available to Promise)** | المتاح للتسليم | كمية المخزون المتوفرة للبيع مع الالتزامات المستقبلية |
| **GR/IR Clearing Account** | حساب تسوية الاستلام/الفاتورة | حساب وسيط بين استلام المخزون وفاتورة المورد |
| **Invoice Block** | تعليق الفاتورة | منع دفع الفاتورة مؤقتاً لوجود اختلاف |
| **Tolerance Limit** | حد التسامح | النسبة/القيمة المسموح بها للاختلاف دون تعليق |
| **BoQ (Bill of Quantities)** | جدول الكميات | وثيقة تحدد كميات المواد والأعمال في المشروع |
| **WBS Element** | عنصر هيكل العمل | وحدة تنظيمية لتجميع التكاليف في المشروع |
| **Project Stock** | مخزون المشروع | مواد مخصصة لمشروع معين خارج المخزون العام |
| **Service Entry Sheet (SES)** | سجل إدخال الخدمات | وثيقة إثبات إنجاز الخدمات من مقاولي الباطن |
| **Variation Order (VO)** | أمر تغيير | تعديل في نطاق العمل/BoQ بعد التعاقد |
| **Commitment** | الالتزام المالي | حجز تكلفة في الميزانية عند إصدار PO |
| **DPO (Days Payable Outstanding)** | أيام الدفع المستحقة | متوسط الأيام بين استلام الفاتورة وسدادها |
| **Contract Compliance** | الالتزام بالعقود | نسبة المشتريات التي تتم ضمن العقود الموقعة |
| **Touchless Processing** | المعالجة بدون لمس | فواتير تمر بدون تدخل يدوي (تلقائياً بالكامل) |
| **ROSMA** | العائد على أصول إدارة التوريد | مقياس أداء إدارة المشتريات من A.T. Kearney |
| **Smart Tax** | الضريبة الذكية | معالجة ضريبية آلية تعتمد على قوانين حقيقية ومتغيرة |
| **Release Strategy** | استراتيجية الموافقة | سلسلة موافقات إلكترونية تمر بها المستندات |
| **SRM (Supplier Relationship Mgmt)** | إدارة علاقات الموردين | إدارة استراتيجية للتفاعل مع الموردين |
| **RFx (RFQ/RFP/RFI)** | طلب عرض/عرض معلومات | وثائق المنافسة لاختيار الموردين |
| **ASN (Advanced Shipment Notice)** | إشعار شحن مسبق | إخطار من المورد بشحنة قادمة مع تفاصيلها |
| **Payment Run / Payment Proposal** | جدولة الدفع | عملية تلقائية لاختيار الفواتير المستحقة للدفع |
| **Early Payment Discount** | خصم الدفع المبكر | خصم يمنحه المورد عند الدفع قبل الاستحقاق |
| **MRP (Material Requirements Planning)** | تخطيط احتياجات المواد | نظام يحدد كميات ومواعيد شراء المواد بناءً على الإنتاج |
| **Cost Center** | مركز تكلفة | وحدة تنظيمية لتجميع التكاليف غير المباشرة |

---

## 10. التوصيات لقطاع المقاولات

بناءً على البحث المتعمق في الأنظمة الثلاثة واحتياجات قطاع المقاولات، إليك التوصيات:

### 10.1 النظام الأكثر ملاءمة

| معيار المفاضلة | SAP S/4HANA + PS | Oracle Procurement Cloud | Dynamics 365 |
|----------------|-----------------|------------------------|--------------|
| دعم إدارة المشاريع | ★★★★★ (SAP PS الأقوى) | ★★★★☆ (Oracle Project Costing) | ★★★☆☆ (Project Operations حديث) |
| إدارة BoQ | ★★★★★ (تكامل PS مع MM) | ★★★★☆ (تكامل مع Project) | ★★★☆☆ (محدود) |
| Subcontractor Management | ★★★★★ (Service Entry Sheet) | ★★★★☆ (خدمات + عقود) | ★★★☆☆ (تحت التطوير) |
| Change Orders (Variations) | ★★★★★ (تقارير وتتبع) | ★★★★☆ (Change Management) | ★★★☆☆ (Power Automate) |
| Multi-Site/Project Stock | ★★★★★ (Project Stock + WBS) | ★★★★☆ (Project Inventory) | ★★★☆☆ (محدود) |
| مرونة التخصيص | ★★★★☆ (عالية لكن مكلفة) | ★★★☆☆ (SaaS - محدود) | ★★★☆☆ (SaaS - محدود) |
| تكلفة التملك (TCO) | عالية (On-Prem أو Cloud) | متوسطة (SaaS) | متوسطة (SaaS) |
| سرعة التنفيذ | 12-24 شهراً | 6-12 شهراً | 6-12 شهراً |
| AI/Innovation | ★★★★☆ (SAP AI Core/BTP) | ★★★★☆ (Oracle AI) | ★★★★★ (Copilot + Azure OpenAI) |

### 10.2 التوصيات التفصيلية

#### الخيار الموصى به: SAP S/4HANA مع SAP PS (Project System)
لشركات المقاولات الكبيرة والمتوسطة، SAP هو النظام الأكثر نضجاً لدورة P2P في المشاريع لعدة أسباب:
- **Project Stock** — إدارة مخزون مخصص لكل مشروع
- **WBS Element في كل مستند** — توزيع دقيق للتكاليف
- **Service Entry Sheets (ML81N)** — إدارة مقاولي الباطن
- **Availability Control** — التحكم في الميزانية ومنع التجاوز
- **Integration مع SAP PS** — ربط المشتريات بهيكل المشروع وجدوله الزمني

#### الخيار البديل: Oracle Procurement Cloud + Oracle Project Costing
إذا كانت الشركة تفضل الحوسبة السحابية (SaaS) وتريد:
- **Self-Service Procurement** — سهولة الاستخدام للمستخدمين
- **Smart Tax** — معالجة ضريبية آلية في بلدان متعددة
- **Supplier Portal** — تواصل سلس مع الموردين
- **Oracle P6 Integration** — ربط المشتريات مع جدول المشروع (Primavera P6)

#### الخيار الناشئ: Microsoft Dynamics 365 SCM + Project Operations
مناسب للشركات التي:
- تستخدم بالفعل Microsoft ecosystem (Teams, Office 365, Power Platform)
- تريد **AI/ML** مدمج (Copilot, Procurement Agent)
- تحتاج تكاملاً مع Azure و Power BI
- لكنه أقل نضجاً في إدارة المشاريع مقارنة بـ SAP PS

### 10.3 متطلبات أساسية يجب توفرها لأي نظام لقطاع المقاولات

1. **Project-based purchasing** — كل أمر شراء يجب أن يرتبط بمشروع + WBS + Activity
2. **Budget Control with warnings** — تحذير تلقائي عند تجاوز 80% من ميزانية بند BoQ
3. **Change Order Management** — إدارة Variations (تغييرات) تؤثر على المشتريات
4. **Subcontractor Management** — وتسجيل إنجاز مقاولي الباطن عبر SES
5. **Retention Management** — إدارة محجوزات الضمان (Retention Money)
6. **Material Tracking by Project** — تتبع المواد من الشراء إلى التركيب في المشروع
7. **Site Receiving** — استلام في مواقع متعددة مع توثيق صوري
8. **Advanced Tax Configuration** — معالجة ضريبية معقدة (VAT, WHT, Reverse Charge)
9. **Multi-currency Support** — دعم العملات المتعددة (استيراد مواد من الخارج)
10. **Bulk Material Management** — إدارة المواد السائبة (خرسانة، حديد، رمل)

### 10.4 استراتيجية تنفيذ مقترحة

```
المرحلة 1: الهيكلة الأساسية (الأشهر 1-3)
├── Vendor Master Data — تنظيف وتوحيد بيانات الموردين
├── Material Master — ربط المواد بـ BoQ
├── Project Structure — إنشاء WBS Elements لجميع المشاريع
└── BoQ Integration — ربط بنود الكميات مع النظام

المرحلة 2: P2P الأساسي (الأشهر 4-6)
├── Purchase Requisition workflow
├── Approval Strategy (Release Strategy)
├── Purchase Orders مع Project Coding
├── Goods Receipt في مواقع المشاريع
└── Service Entry Sheets للمقاولين

المرحلة 3: المالية والمطابقة (الأشهر 7-9)
├── Three-Way Matching للمواد
├── Two-Way Matching للخدمات
├── Invoice Automation
├── Supplier Portal
└── Payment Integration

المرحلة 4: التحسين والذكاء (الأشهر 10-12)
├── AI/ML Optimization (استباقية)
├── KPIs Dashboard (Power BI / SAP Analytics)
├── Predictive Analytics للمشتريات
├── Supplier Performance Scorecard
└── Continuous Improvement Program
```

---

## المراجع

1. IBM Think — What is Procure to Pay (P2P)? https://www.ibm.com/think/topics/procure-to-pay
2. Wikipedia — Procure-to-pay en.wikipedia.org/wiki/Procure-to-pay
3. Wikipedia — Procurement en.wikipedia.org/wiki/Procurement
4. CIPS — Procurement & Supply Cycle www.cips.org
5. SAP Help Portal — SAP MM Purchasing help.sap.com
6. Microsoft Learn — Dynamics 365 Supply Chain Management learn.microsoft.com
7. Oracle Help Center — Oracle Procurement docs.oracle.com
8. Deloitte — P2P Risk Analytics Report
9. Aberdeen Group — P2P Process Optimization
10. A.T. Kearney — ROSMA (Return on Supply Management Assets)

---

**نهاية التقرير**
