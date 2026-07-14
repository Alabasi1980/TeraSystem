# Research R02 — هيكل الموافقات والصلاحيات في المشتريات
## Approval & Authorization Limits in Procurement (ERP Systems)

> **Domain**: ERP Procurement Approvals — SAP MM, Oracle Procurement Cloud, Microsoft Dynamics 365  
> **تاريخ البحث**: 07 يوليو 2026  
> **الهدف**: دراسة متعمقة لأنظمة الموافقات في المشتريات في أنظمة ERP الثلاثة الكبرى  

---

## فهرس المحتويات

1. [SAP MM Release Strategy](#1-sap-mm-release-strategy)
2. [Oracle Procurement Cloud — Approvals Management (AME)](#2-oracle-procurement-cloud--approvals-management-ame)
3. [Microsoft Dynamics 365 — Approval Workflows](#3-microsoft-dynamics-365--approval-workflows)
4. [مبدأ الفصل بين الصلاحيات (Segregation of Duties - SoD)](#4-مبدأ-الفصل-بين-الصلاحيات-segregation-of-duties---sod)
5. [هيكل الصلاحيات النموذجي في المقاولات](#5-هيكل-الصلاحيات-النموذجي-في-المقاولات)
6. [أفضل الممارسات العالمية (Best Practices)](#6-أفضل-الممارسات-العالمية-best-practices)
7. [الخاتمة والتوصيات](#7-الخاتمة-والتوصيات)

---

## 1. SAP MM Release Strategy

### 1.1 المفهوم العام

**Release Strategy** (إستراتيجية التحرير/الموافقة) في SAP MM هي الآلية التي تتحكم في تفعيل أمر الشراء (Purchase Order - PO). لا يمكن إرسال أمر الشراء للمورّد (Vendor) حتى يكتمل مسار الموافقات المحدد مسبقاً.

تعمل SAP بنظام **التصنيف** (Classification) لربط أوامر الشراء بإستراتيجية الموافقة المناسبة، وذلك عبر خصائص محددة مثل قيمة الأمر، نوع المادة، مجموعة المواد، الشركة، إلخ.

### 1.2 المكونات الأساسية

| المصطلح (Term) | المعنى (Arabic) | الوصف |
|---|---|---|
| **Release Strategy** | إستراتيجية التحرير | مجموعة الموافقات المطلوبة لتفعيل PO |
| **Release Group** | مجموعة التحرير | تصنيف عام يضم إستراتيجيات متعددة |
| **Release Code** | كود التحرير | رمز يمثل مستوى موافقة معين (شخص/منصب) |
| **Release Condition** | شرط التحرير | الشرط الذي يحدد أي إستراتيجية تُطبق |
| **Release Indicator** | مؤشر التحرير | حالة الموافقة (معلق/موافق/مرفوض) |
| **Release Status** | حالة التحرير | يوضح كم مستوى تمت الموافقة عليه |

### 1.3 معمارية Release Strategy (الهيكل العام)

```
Release Group (مجموعة)
    ├── Release Strategy 1 (إستراتيجية)
    │       ├── Release Code 01 → مبلغ < 10,000
    │       ├── Release Code 02 → مبلغ 10,000 - 50,000
    │       └── Release Code 03 → مبلغ > 50,000
    └── Release Strategy 2
            ├── Release Code 01 → مشتريات خدمات
            └── Release Code 02 → مشتريات مواد خام
```

### 1.4 خطوات التكوين (Configuration Steps)

#### 1.4.1 تعريف خصائص التصنيف (Characteristics)
- **TCode: CT04** — تعريف الخصائص التي ستحدد الإستراتيجية
- أمثلة على خصائص: قيمة الشراء (Total Value)، نوع الشراء، نوع المادة

#### 1.4.2 تعريف الفئة (Class)
- **TCode: CL02** — تعريف الفئة التي تجمع الخصائص معاً
- فئة التحرير القياسية: `CLASS_TYPE = 032`

#### 1.4.3 تعريف Release Group
- **TCode: OMCB** أو **Path**: IMG → Materials Management → Purchasing → Release Procedure
- يتم هنا تعريف Release Group (مثل: ZG1 للمشتريات المحلية، ZG2 للمشتريات الخارجية)

#### 1.4.4 تعريف Release Codes
- **TCode: OMCB** — إنشاء "أكواد التحرير" (01, 02, 03...)
- كل Release Code يمثل شخصاً أو وظيفة (مثل: مدير مشتريات، مدير مالي)

#### 1.4.5 تعريف Release Indicators
- يحدد حالة كل كود (عامودياً وأفقياً)
- **SAP Standard Indicators**: 1 (SAP Release), 2, 3, 4, 5...

#### 1.4.6 تعريف Release Strategy
- **TCode: OMD2** — ربط Release Codes بترتيبها (Sequence)
- تحديد عدد الموافقات المطلوبة (Release Prerequisite)
- ربط الفئة (Class) بالإستراتيجية

#### 1.4.7 تعيين الصلاحيات للأفراد
- **TCode: SU01** — User maintenance
- **TCode: OMCB** → Assign Release Codes to users
- كل مستخدم يحصل على Release Code(s) في ملف تعريفه (Profile)

### 1.5 أنواع الشروط (Release Conditions)

يمكن بناء الشروط بناءً على:

| الشرط (Condition) | الوصف |
|---|---|
| قيمة أمر الشراء (PO Value) | $0-5K, $5K-50K, $50K+ |
| نوع المادة (Material Type) | ROH (مواد خام), FERT (منتج نهائي), HAWA (بضاعة) |
| مجموعة المشتريات (Purchasing Group) | E01, E02 |
| المنظمة (Purchasing Organization) | P001, P002 |
| الشركة (Company Code) | 1000, 2000 |
| مركز التكلفة (Cost Center) | CC-001 |
| المورّد (Vendor) | V001, V002 |

### 1.6 TCodes الرئيسية

| TCode | الوظيفة |
|---|---|
| **CT04** | تعريف Characteristics |
| **CL02** | تعريف Class (تصنيف) |
| **CL22N** | تعيين Classification للمواد |
| **OMCB** | تعريف Release Groups & Release Codes |
| **OMD2** | تعريف Release Strategies |
| **OMD3** | تعريف Release Indicators Matrix |
| **ME28** | الموافقة الجماعية على أوامر الشراء (Mass Release) |
| **ME29N** | الموافقة الفردية على أمر شراء (Individual Release) |
| **ME2N** | عرض أوامر الشراء |
| **SU01** | إدارة المستخدمين وتعيين Release Codes |
| **M_EINK_FRG** | كائن التفويض (Authorization Object) للموافقات |

> **ملاحظة**: كائن التفويض الأساسي هو `M_EINK_FRG` الذي يتحكم في من يمكنه الموافقة على أي Release Code.

### 1.7 سير عمل الموافقة (Approval Flow)

```
منشئ أمر الشراء (Buyer)
    ↓  إنشاء PO → PO في حالة "معلق" (Blocked)
    ↓  
الموافقة الأولى (Supervisor) → TCode ME29N
    ↓  إذا تجاوزت قيمة PO الحد المسموح للمستوى الأول
الموافقة الثانية (Department Manager)
    ↓  إذا تجاوزت قيمة PO الحد المسموح للمستوى الثاني
الموافقة الثالثة (Finance Manager)
    ↓  إذا تجاوزت قيمة PO الحد المسموح للمستوى الثالث
الموافقة الرابعة (CEO)
    ↓
تم التحرير → PO متاح للإرسال للمورّد
```

### 1.8 مزايا وعيوب Release Strategy في SAP

**المزايا:**
- تكامل تام مع Classification (المرونة في تحديد الشروط)
- دعم متعدد المستويات والتسلسل الهرمي
- إمكانية الموافقة الجماعية (ME28) والفردية (ME29N)
- سجل تدقيق (Audit Trail) كامل لكل موافقة

**العيوب:**
- تعقيد في التكوين الأولي
- الحاجة لفهم Classification جيداً
- صعوبة التعديل بعد التشغيل
- عدم وجود Dashboard رسومي مدمج للموافقات

---

## 2. Oracle Procurement Cloud — Approvals Management (AME)

### 2.1 المفهوم العام

**AME (Approvals Management Engine)** هو محرك الموافقات المدمج في Oracle Fusion Procurement Cloud. يوفر نظاماً مرناً لتحديد قواعد الموافقات (Approval Rules) بناءً على عدة عوامل مثل المبلغ، نوع المستند، القسم، إلخ.

### 2.2 أنواع الموافقات في Oracle

| النوع | الوصف |
|---|---|
| **Chain of Authority** | موافقة تسلسلية حسب التسلسل الإداري |
| **Position Hierarchy** | موافقة مبنية على التسلسل الوظيفي (Position-based) |
| **Supervisory Hierarchy** | موافقة مبنية على التسلسل الإشرافي |
| **Job Level Hierarchy** | موافقة بناءً على المستوى الوظيفي |
| **Dynamic Approval Groups** | مجموعات موافقة ديناميكية تُحدد وقت التشغيل |
| **List-Based Approvals** | قائمة محددة مسبقاً من الموافقين |

### 2.3 Hierarchy-based vs Position-based Approvals

**Hierarchy-based:**
- الموافقة تمر عبر التسلسل الإداري مباشرة
- الرئيس المباشر → رئيس القسم → المدير العام
- يتم تحديد حدود مالية لكل مستوى في الـ Hierarchy

**Position-based:**
- الموافقة مرتبطة بالمنصب (Position) وليس بالشخص
- مثال: "مدير المشتريات" موافق على مبالغ حتى 100K
- عند تغيير الشخص، يظل المنصب كما هو

### 2.4 مكونات AME

| المكون | الوصف |
|---|---|
| **Transaction Type** | نوع المعاملة (PO, Requisition, Invoice...) |
| **Approval Rule** | قاعدة الموافقة (شرط + إجراء = موافقة) |
| **Approval Group** | مجموعة موافقين (تسلسلية أو متوازية) |
| **Condition** | الشرط الذي يحدد تفعيل القاعدة (مبلغ، قسم، إلخ) |
| **Action** | الإجراء: إرسال للموافقة، تخطي، رفض |
| **Chain of Authority** | سلسلة الصلاحية الممتدة |

### 2.5 Dynamic Approval Groups

مجموعات الموافقة الديناميكية تسمح بتجميع الموافقين أثناء وقت التشغيل (Runtime) بناءً على:

- بيانات المستند (Document Data): قيمة PO، نوع المادة
- بيانات المستخدم (User Data): القسم، الموقع
- بيانات خارجية (External Data): من تطبيقات أخرى عبر Web Service

### 2.6 Smart Approvals (الموافقات الذكية)

Oracle تقدم **Smart Approvals** التي تشمل:

| الخاصية | الوصف |
|---|---|
| **Auto-Forwarding** | إعادة التوجيه التلقائي عند غياب الموافق الأول |
| **Approval Delegation** | تفويض الموافقة لشخص آخر مؤقتاً |
| **Parallel Approvals** | موافقات متوازية لعدة أقسام في وقت واحد |
| **Sequential Approvals** | موافقات تسلسلية |
| **Single Action Approvals** | موافقة / رفض بضغطة واحدة |
| **History & Audit Trail** | سجل كامل لكل موافقة |
| **Mobile Approvals** | موافقات عبر الجوال |
| **Push Notifications** | إشعارات فورية |

### 2.7 تكوين AME

#### خطوات التكوين الأساسية:

1. **تحديد Transaction Type** → Purchase Order/Requisition
2. **إنشاء Approval Rules** → Define conditions & approvers
3. **ربط Supervisory Hierarchy** → Link to HR hierarchy
4. **تعيين Limit Ranges** → لكل مستوى موافق
5. **تفعيل Dynamic Groups** (اختياري)
6. **اختبار** → استخدام Simulation mode

### 2.8 مثال على قاعدة موافقة Oracle

```
IF PO.TotalAmount BETWEEN 0 AND 5000
    THEN APPROVER = Employee.Supervisor (Chain of Authority Level 1)

IF PO.TotalAmount BETWEEN 5000 AND 50000
    THEN APPROVER = Employee.Supervisor.Supervisor (Level 2)

IF PO.TotalAmount > 50000
    THEN APPROVER = "Procurement Director" (Position)
```

### 2.9 الفروقات عن SAP MM Release Strategy

| الجانب | SAP | Oracle AME |
|---|---|---|
| آلية الربط | Classification (CT04/CL02) | Approval Rules |
| التسلسل | Release Codes + Release Indicators | Chain of Authority |
| Overflow | يدوي عبر Indicators | تلقائي مع Overflow Rules |
| القابلية للتوسع | معقدة نسبياً | مرنة مع Dynamic Groups |
| الموبايل | SAP Fiori فقط | مدمج في Cloud |
| الإشعارات | Workflow | Push Notifications + Email |

---

## 3. Microsoft Dynamics 365 — Approval Workflows

### 3.1 المفهوم العام

Dynamics 365 for Finance and Operations يستخدم نظام **Workflows** المدمج في النظام لإدارة الموافقات على المشتريات. هذا النظام يعتمد على مزيج من **Purchase Policies** (سياسات الشراء) و **Purchase Agreement Classifications** (تصنيفات اتفاقيات الشراء).

### 3.2 أنواع الـ Workflows في المشتريات

| نوع الـ Workflow | المستند المستهدف |
|---|---|
| **PO Approval Workflow** | أوامر الشراء (Purchase Orders) |
| **Requisition Approval Workflow** | طلبات الشراء (Purchase Requisitions) |
| **Vendor Approval Workflow** | إضافة/تعديل المورّدين |
| **Invoice Approval Workflow** | فواتير المشتريات |
| **Purchase Agreement Workflow** | اتفاقيات الشراء |
| **Change Management Workflow** | تغييرات أوامر الشراء |

### 3.3 Purchase Policies (سياسات الشراء)

سياسات الشراء هي القواعد التي تحكم سلوك المشتريات في Dynamics 365:

| عنصر السياسة | الوصف |
|---|---|
| **Purchase Policy** | مجموعة قواعد تنطبق على كيان قانوني (Legal Entity) |
| **Policy Rule Type** | أنواع القواعد: Policy Rule, Category Policy Rule |
| **Policy Organization Hierarchy** | هيكل المؤسسة لتطبيق السياسات |
| **Purchase Agreement Classification** | تصنيف اتفاقيات الشراء (فئات مختلفة لكل نوع) |
| **Catalog Policy** | سياسة كتالوج المواد المسموح بها |

### 3.4 مكونات Workflow

```
Workflow Element
    ├── Approval Step 1 → Sequential
    │       ├── Condition: Amount < $10,000
    │       └── Assigned to: Purchasing Manager
    ├── Parallel Activity
    │       ├── Approval Step 2a → Finance
    │       └── Approval Step 2b → Legal
    └── Approval Step 3 → Escalation
            ├── Condition: Amount > $100,000
            └── Assigned to: VP Procurement
```

### 3.5 Sequential vs Parallel Workflows

**Sequential Approval (تسلسلي):**
- الموافقة تمر عبر مستويات متتالية
- لا تنتقل للمستوى التالي إلا بعد اكتمال السابق
- مناسب للحدود المالية المتدرجة

**Parallel Approval (متوازي):**
- عدة موافقين في وقت واحد
- جميعهم يحتاجون للموافقة (All Approve) أو واحد يكفي (Any Approve)
- مناسب للموافقات المشتركة (مالي + فني + قانوني)

### 3.6 Threshold-based Approvals + Escalation Rules

| المبلغ | المسار | Escalation |
|---|---|---|
| $0 - $5,000 | موافقة المشرف المباشر | — |
| $5,001 - $50,000 | موافقة مدير المشتريات | إذا لم يوافق خلال 3 أيام → ترقية لمدير الإدارة |
| $50,001 - $200,000 | موافقة المدير المالي + مدير المشتريات | إذا لم يوافق خلال 5 أيام → ترقية لـ CEO |
| $200,000+ | موافقة CEO + مجلس الإدارة | إذا لم يوافق خلال 7 أيام → اجتماع طارئ |

### 3.7 Power Automate Integration

من أقوى ميزات Dynamics 365 هو التكامل مع **Power Automate** (مايكروسوفت Flow):

| التكامل | الوصف |
|---|---|
| **Custom Approval Flows** | إنشاء مسارات موافقة مخصصة خارج الـ Workflow القياسي |
| **Email Integration** | إرسال إشعارات موافقة عبر Outlook مع أزرار تفاعلية |
| **Teams Integration** | موافقة مباشرة من Microsoft Teams |
| **Mobile Approvals** | تطبيق Power Apps Mobile للموافقات |
| **AI Builder Integration** | استخدام AI لاقتراح مسار الموافقة الأمثل |
| **Approval Center** | مركز موافقات موحد لكل المستندات |
| **SLA Tracking** | تتبع وقت الاستجابة لكل موافقة |

### 3.8 خطوات تكوين Workflow في Dynamics 365

1. **إنشاء Workflow**: المسار → Procurement → Setup → Procurement workflows
2. **تحديد المشغّل (Trigger)**: إنشاء PO, تعديل PO, إلخ
3. **إضافة Approval Steps**: تعريف كل خطوة موافقة
4. **تحديد الشروط (Conditions)**: لكل خطوة شرط تفعيل
5. **تحديد الموافق (Assignment)**: مستخدم، مجموعة، منصب، فريق
6. **إعداد Escalation Rules**: ماذا يحدث عند التأخير
7. **تفعيل الـ Workflow**: نشر (Publish) لبدء الاستخدام

### 3.9 إدارة التغيير (Change Management)

في Dynamics 365، يمكن تفعيل **Change Management** على أوامر الشراء:
- أي تعديل على PO بعد الموافقة يتطلب إعادة موافقة
- يتم تسجيل كل تغيير في الـ Audit Log
- يمكن تحديد أي الحقول تتطلب إعادة موافقة

---

## 4. مبدأ الفصل بين الصلاحيات (Segregation of Duties - SoD)

### 4.1 المفهوم العام

**Segregation of Duties (SoD)** أو **الفصل بين الصلاحيات** هو مبدأ رقابي أساسي يمنع تضارب المصالح من خلال توزيع المهام الحساسة على أكثر من شخص.

**الفكرة الأساسية**: لا يمكن لشخص واحد أن يقوم بخطوتين متضاربتين في نفس العملية.

### 4.2 مبدأ من يطلب ≠ من يوافق ≠ من يستلم ≠ من يدفع

```
الدورة الكاملة للمشتريات (Procurement Cycle):

يطلب (Requestor) → يوافق (Approver) ← يستلم (Receiver) ← يدفع (Payer)
    ↑                                                          ↓
    └─────────────────── الفصل إلزامي ──────────────────────────┘
```

| الدور | الوظيفة | SoD المطلوب |
|---|---|---|
| **Requestor** (الطالب) | يقدم طلب الشراء | لا يمكنه الموافقة على طلبه |
| **Approver** (الموافق) | يوافق على الطلب | لا يمكنه أن يكون الطالب |
| **Buyer** (المشتري) | ينفذ الشراء | لا يمكنه الموافقة |
| **Goods Receiver** (مستلم البضاعة) | يستلم المواد | لا يمكنه الشراء |
| **Invoice Verifier** (مدقق الفاتورة) | يطابق الفاتورة | لا يمكنه الدفع |
| **Payment Processor** (الدافع) | يصدر الدفع | لا يمكنه استلام البضاعة |

### 4.3 تطبيق SoD في الأنظمة الثلاثة

#### SAP — SoD عبر Authorization Objects

SAP تستخدم **Authorization Objects** (كائنات التفويض) لتطبيق SoD:

| كائن التفويض | description | الدور |
|---|---|---|
| `M_BEST_BSA` | إنشاء PO | المشتري |
| `M_EINK_FRG` | الموافقة على PO | الموافق |
| `M_BEST_EKO` | عرض PO | للقراءة فقط |
| `MEPO` | المستندات في ME2x | صلاحيات متنوعة |
| `M_MSEG_WMB` | استلام البضائع (Goods Receipt) | المستلم |
| `F_IVDO_FKE` | إصدار الدفعات | الدافع |

**أمثلة على تعارضات SoD في SAP:**
- إنشاء PO + الموافقة عليه (Create + Release)
- إنشاء PO + استلام البضائع (Create + GR)
- الموافقة على PO + تغيير بيانات المورّد
- استلام البضائع + إصدار الدفعات

#### Oracle — SoD عبر Duty Roles

Oracle Fusion يستخدم **Duty Roles** (أدوار الواجب):

| Duty Role | المسموح به |
|---|---|
| **Procurement Agent** | إنشاء PO |
| **Procurement Approver** | الموافقة على PO |
| **Receiving Agent** | استلام البضائع |
| **Accounts Payable Manager** | معالجة الدفعات |

Oracle يفرض SoD عبر **Separation of Duties Framework** المدمج:
- يمنع تعيين أدوار متعارضة لنفس المستخدم
- يوفر تقارير تحليل تعارضات
- يدعم Periodic Review (مراجعة دورية)

#### Dynamics 365 — SoD عبر Security Roles

Dynamics 365 يستخدم **Security Roles** و **Duties**:

| Security Role | الإجراء |
|---|---|
| **Purchasing Agent** | إنشاء وتعديل POs |
| **Purchasing Manager** | الموافقة على POs |
| **Receiving Clerk** | استلام البضائع |
| **Vendor Invoice Clerk** | إدخال الفواتير |
| **Accounts Payable Clerk** | الدفع |

**يمكن كشف تعارضات SoD في Dynamics 365 باستخدام**:
- **Segregation of Duties Report** (تقارير مدمجة)
- **Compliance Center** في Power Platform
- **Audit Logs** في Lifecycle Services (LCS)

### 4.4 الامتثال لمعايير التدقيق (Audit Compliance)

| المعيار | المتطلبات المتعلقة بـ SoD |
|---|---|
| **SOX (Sarbanes-Oxley)** | يتطلب إثبات الفصل بين الصلاحيات في التقارير المالية |
| **ISO 9001:2015** | يتطلب تحديد أدوار ومسؤوليات واضحة |
| **ISO 27001** | يتطلب التحكم في الوصول و SoD لأنظمة المعلومات |
| **COSO 2013** | يتضمن Control Activities التي تغطي SoD |
| **Saudi NCA-ECC** | يتطلب ضوابط وصول صارمة للمنشآت الحيوية |
| **ZATCA (الزكاة والضريبة)** | يتطلب مسار تدقيق إلكتروني لكل المعاملات |

---

## 5. هيكل الصلاحيات النموذجي في المقاولات

### 5.1 المستويات الإدارية الموصى بها

في قطاع المقاولات (Contracting/Construction)، أوصي بالهيكل التالي:

| المستوى | المسمى | الحد المالي | نوع الموافقة |
|---|---|---|---|
| **Level 0** | طالب الشراء / مهندس موقع | — | تقديم طلب فقط |
| **Level 1** | رئيس المهندسين / مشرف المشتريات | $0 - $5,000 | موافقة أولية |
| **Level 2** | مدير المشروع (Project Manager) | $5,000 - $50,000 | موافقة متوسطة |
| **Level 3** | مدير المشتريات / مدير مالي | $50,000 - $200,000 | موافقة مالية + فنية |
| **Level 4** | المدير التنفيذي (CEO / GM) | $200,000 - $1,000,000 | موافقة عليا |
| **Level 5** | مجلس الإدارة | $1,000,000+ | موافقة نهائية |

### 5.2 الموافقة الفنية + الموافقة المالية

في المقاولات، ينقسم مسار الموافقة عادة إلى مسارين متوازيين:

```
طلب الشراء (PR)
    ├── الموافقة الفنية (Technical Approval)
    │       └── يشرف عليها: مدير المشروع أو كبير المهندسين
    │       └── تتحقق من: المواصفات، الكميات، الجدول الزمني
    │
    └── الموافقة المالية (Financial Approval)
            └── يشرف عليها: مدير المشتريات أو المدير المالي
            └── تتحقق من: السعر، الميزانية، شروط الدفع
```

**مثال تطبيقي:**
- أمر شراء لمقاول باطن بقيمة $150,000
- الموافقة الفنية: مدير المشروع (يتأكد من أن الخدمة مطلوبة)
- الموافقة المالية: مدير المشتريات (يتأكد من أن السعر مناسب)
- الموافقة النهائية: المدير التنفيذي (لقيمة تتجاوز $100K)

### 5.3 حدود الصلاحيات حسب الموقع

في المقاولات الكبيرة (مشاريع متعددة الفروع):

| الموقع | الحد المالي للمدير المباشر | الحد المالي للمدير التنفيذي |
|---|---|---|
| المقر الرئيسي | $50,000 | $500,000 |
| فرع الرياض | $30,000 | $200,000 |
| فرع جدة | $25,000 | $150,000 |
| موقع المشروع | $10,000 | $100,000 |

### 5.4 استثناءات الطوارئ (Emergency Exceptions)

في حالات الطوارئ (أعطال، كوارث، توقف إنتاج)، قد تكون هناك حاجة لتجاوز إجراءات الموافقة:

| السيناريو | المبلغ | الإجراء |
|---|---|---|
| عطل في معدات حيوية | حتى $20,000 | موافقة شفهية + توثيق خلال 24 ساعة |
| كارثة طبيعية | حتى $100,000 | تفويض مسبق لمدير المشروع |
| نقص مواد حرج | حتى $10,000 | موافقة منسق المشتريات + مدير الموقع |
| استثناء بعد الدوام | أي مبلغ | مدير الطوارئ فقط + توثيق فوري |

> **قاعدة صارمة**: جميع استثناءات الطوارئ يجب توثيقها إلكترونياً ومراجعتها من قبل لجنة التدقيق الداخلي.

### 5.5 الجمع بين الموافقات (Combined Approvals)

في بعض الحالات قد يمر أمر الشراء بمسار تجميعي:

```
PO بقيمة $500,000
    ├── موافقة تسلسلية فنية:
    │       مهندس موقع → مدير مشروع → كبير المهندسين
    │
    └── موافقة تسلسلية مالية (بالتوازي):
            مدير مالي → نائب المدير التنفيذي → CEO
```

### 5.6 الجدول التجميعي: مستويات الموافقة حسب نوع الشراء

| نوع الشراء | $0-5K | $5K-50K | $50K-200K | $200K-1M | $1M+ |
|---|---|---|---|---|---|
| مواد خام | L1 | L2 | L2+L3 | L3+L4 | L4+L5 |
| مقاولي باطن | L1+L2 | L2 | L2+L3 | L3+L4 | L4+L5 |
| خدمات | L1 | L2 | L2+L3 | L3+L4 | L4+L5 |
| معدات/أصول | L2 | L2+L3 | L3+L4 | L3+L4 | L4+L5 |
| طوارئ | L1 | L2 | L2+توثيق | L4+توثيق | L4+L5+توثيق |

---

## 6. أفضل الممارسات العالمية (Best Practices)

### 6.1 COSO Framework للرقابة الداخلية

**COSO (Committee of Sponsoring Organizations of the Treadway Commission)** هو إطار الرقابة الداخلية الأكثر قبولاً عالمياً.

#### المكونات الخمسة لـ COSO (COSO Cube):

```
┌─────────────────────────────────────────────────────┐
│                 1. بيئة الرقابة                      │
│        (Control Environment) - Tone at the Top       │
├─────────────────────────────────────────────────────┤
│              2. تقييم المخاطر                        │
│           (Risk Assessment)                          │
├─────────────────────────────────────────────────────┤
│           3. أنشطة الرقابة                           │
│         (Control Activities) ← تتضمن الموافقات      │
├─────────────────────────────────────────────────────┤
│          4. المعلومات والاتصال                       │
│      (Information & Communication)                   │
├─────────────────────────────────────────────────────┤
│              5. المراقبة                             │
│            (Monitoring Activities)                   │
└─────────────────────────────────────────────────────┘
```

#### تطبيق COSO في المشتريات:

| مكون COSO | التطبيق في المشتريات |
|---|---|
| **Control Environment** | سياسات الشراء، هيكل الصلاحيات، مدونة قواعد السلوك |
| **Risk Assessment** | تحليل مخاطر المورّدين، مخاطر الاحتيال، مخاطر الأسعار |
| **Control Activities** | Release Strategy في SAP، AME في Oracle، Workflows في D365 |
| **Information & Communication** | إشعارات الموافقات، Audit Trail، تقارير الأداء |
| **Monitoring** | مراجعة الموافقات المتأخرة، تحليل SoD، تدقيق دوري |

### 6.2 توزيع المهام (Segregation of Duties)

**أفضل الممارسات العالمية لـ SoD في المشتريات:**

1. **مبدأ الأربع عيون (Four-Eyes Principle)**: كل معاملة تحتاج نظر شخصين على الأقل
2. **مبدأ الحد الأدنى من الصلاحية (Least Privilege)**: كل شخص يحصل على أقل صلاحية يحتاجها
3. **التدوير الوظيفي (Job Rotation)**: تغيير مسؤوليات المشتريات بشكل دوري
4. **مراجعة دورية للصلاحيات (Periodic Access Review)**: كل 3-6 أشهر
5. **كشف التعارضات تلقائياً (Automated Conflict Detection)**: الأنظمة تكتشف تعارض الصلاحيات
6. **لا يمكن تعديل الحدود المالية من قبل المستخدم نفسه**: أي تغيير في الحد المالي يحتاج موافقة أعلى

### 6.3 التوثيق الإلكتروني لمسار الموافقة (Electronic Audit Trail)

**عناصر مسار الموافقة الإلكتروني الكامل:**

| العنصر | الوصف | مثال |
|---|---|---|
| **Timestamp** | ختم زمني لكل إجراء | 2026-07-07 14:30:22 |
| **User ID** | معرف المستخدم | M.ALI |
| **Action** | الإجراء المتخذ | Approved / Rejected / Returned |
| **Decision** | القرار مع التعليق | "موافق مع مراجعة السعر" |
| **System** | النظام الذي تمت فيه الموافقة | SAP / Oracle / D365 |
| **IP Address** | عنوان IP (للأمان) | 192.168.1.100 |
| **Previous Status** | الحالة قبل الإجراء | Pending Level 2 |
| **New Status** | الحالة بعد الإجراء | Released |

**فوائد التوثيق الإلكتروني:**
- إثبات الامتثال للمعايير (SOX, ISO, ZATCA)
- تسهيل مراجعة المدققين (Auditors)
- كشف محاولات التلاعب
- تحليل أداء الموافقات (مدة كل موافقة)
- دعم قانوني في حال النزاعات

### 6.4 توصيات إضافية (Additional Recommendations)

1. **الموافقة الهرمية التصاعدية (Hierarchical Escalation)**
   - L1: 100% من الموافقات
   - L2: إذا تجاوز المبلغ الحد → L2
   - L3: إذا تجاوز L2 أو لم يوافق خلال زمن محدد

2. **فصل أوامر الشراء حسب نوع المصدر:**
   - مشتريات محلية (Local): حدود أقل
   - مشتريات دولية (International): حدود أعلى + موافقات إضافية

3. **استخدام تقارير Performance Dashboard:**
   - Average Approval Time (متوسط زمن الموافقة)
   - Bottleneck Analysis (تحليل الاختناقات)
   - Approval Compliance Rate (نسبة الامتثال)

4. **التكامل مع العقود (Contract Integration):**
   - الموافقة على PO المرتبط بعقد: مسار مختصر
   - الموافقة على PO بدون عقد: مسار كامل مع تدقيق قانوني

5. **التدقيق الدوري (Periodic Audit):**
   - مراجعة 10% من POs كل شهر
   - مراجعة 100% من POs ذات القيمة العالية
   - كشف حالات تجاوز الصلاحيات

---

## 7. الخاتمة والتوصيات

### 7.1 الخلاصة

يُعد نظام الموافقات في المشتريات (Procurement Approval System) العمود الفقري للرقابة الداخلية في أي مؤسسة. كل نظام ERP يوفر أدوات قوية لتحقيق ذلك:

| النظام | نقاط القوة | نقاط الضعف |
|---|---|---|
| **SAP MM** | تكامل مع Classification، سجل تدقيق قوي، مرونة في الشروط | تعقيد التكوين، تكلفة عالية، واجهة قديمة |
| **Oracle AME** | موافقات ديناميكية، تكامل مع Hierarchy، واجهة حديثة، موبايل | يعتمد على هيكل Perosonnel قوي، تكلفة ترخيص |
| **Dynamics 365** | تكامل Power Automate، سهولة الاستخدام، مرونة Workflows، Teams | يحتاج تخصيص أكثر، بعض الميزات تحتاج Add-ons |

### 7.2 توصيات للتنفيذ

1. **للمؤسسات الصغيرة (< 50 مستخدم):**
   - Dynamics 365 Business Central (تكلفة أقل، سهولة في التطبيق)
   - هيكل صلاحيات: 3 مستويات كافية (مشرف، مدير، تنفيذي)

2. **للمؤسسات المتوسطة (50-500 مستخدم):**
   - SAP S/4HANA Public Cloud أو Oracle Cloud
   - هيكل: 4 مستويات + موافقات متوازية (فني + مالي)

3. **للمؤسسات الكبيرة (500+ مستخدم):**
   - SAP S/4HANA Private Cloud أو Oracle Fusion
   - هيكل: 5 مستويات + Classification معقد + Dynamic Groups
   - تكامل مع SoD Automation و Identity Management

4. **لقطاع المقاولات تحديداً:**
   - استخدام مسارين متوازيين (فني + مالي)
   - تحديد حدود مالية لكل موقع ولكل مشروع
   - تطبيق استثناءات الطوارئ مع توثيق إلزامي
   - فصل أوامر الشراء للمقاولين (Subcontractors) عن المشتريات المباشرة

### 7.3 المصادر والمراجع

- SAP Help Portal — Release Procedure for Purchase Orders
- Oracle Cloud Help Center — Approvals Management (AME)
- Microsoft Learn — Purchase Order Workflows in Dynamics 365
- COSO 2013 — Internal Control — Integrated Framework
- ISACA — Segregation of Duties in Procurement
- SAP TCode documentation: ME28, ME29N, CT04, CL02, OMCB, OMD2

---

> **تم إعداد هذا التقرير بواسطة**: DomainResearchAgent  
> **لمشروع**: TeraSystem — نظام إدارة المشاريع والمقاولات  
> **الصيغة**: Markdown  
> **جاهز للتخزين**: بنك المعلومات — Knowledge Base
