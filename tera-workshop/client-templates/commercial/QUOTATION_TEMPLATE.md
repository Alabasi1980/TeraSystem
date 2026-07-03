---
document_type: quotation
template_version: v0.1-draft
category: commercial
source_html: tera-workshop/temp/QUOTATION_TEMPLATE.html
used_by: TeraClientEngagementAgent
client_facing: true
approval_required: Majed
client_signature_required: optional
legal_review_required: false
currency: JOD
pricing_policy: tera-system/TeraPricingPolicy.md
related_documents:
  - APPLICATION_PROPOSAL_TEMPLATE
  - SCOPE_OF_WORK_TEMPLATE
  - SOFTWARE_SERVICES_AGREEMENT_TEMPLATE
status: TEMPLATE
---

# Quotation — `[QUOTATION_TITLE]`

> **نوع الوثيقة:** عرض سعر رسمي  
> **الحالة:** مسودة حتى اعتماد Majed  
> **العملة:** الدينار الأردني (JOD)  
> **مرجع التسعير:** `tera-system/TeraPricingPolicy.md`

---

## 1. بيانات العرض

| الحقل | القيمة |
|---|---|
| رقم العرض | `[QID]` |
| اسم العميل | `[CLIENT_NAME]` |
| اسم المشروع | `[PROJECT_NAME]` |
| تاريخ الإصدار | `[Q_DATE]` |
| صلاحية العرض | `[VALIDITY]` |
| طريقة الدفع | `[PAYMENT_METHOD]` |
| الحالة | `[STATUS]` |

---

## 2. ملخص العرض

`[QUOTATION_SUMMARY]`

> **ملاحظة:** هذا العرض مبني على النطاق المعتمد حالياً فقط. أي توسع لاحق يمر عبر `Change Request` مستقل.

---

## 3. البنود والأسعار

| # | البند | الوصف | الكمية | سعر الوحدة (JOD) | الإجمالي (JOD) |
|---|---|---|---:|---:|---:|
| 1 | `[ITEM_1]` | `[ITEM_1_DESC]` | `[QTY_1]` | `[UNIT_PRICE_1]` | `[TOTAL_1]` |
| 2 | `[ITEM_2]` | `[ITEM_2_DESC]` | `[QTY_2]` | `[UNIT_PRICE_2]` | `[TOTAL_2]` |
| 3 | `[ITEM_3]` | `[ITEM_3_DESC]` | `[QTY_3]` | `[UNIT_PRICE_3]` | `[TOTAL_3]` |
| 4 | `[ITEM_4]` | `[ITEM_4_DESC]` | `[QTY_4]` | `[UNIT_PRICE_4]` | `[TOTAL_4]` |

|  |  |  |  | **المجموع الفرعي** | **[SUBTOTAL]** |
|  |  |  |  | **الخصم** | **[DISCOUNT]** |
|  |  |  |  | **الضرائب والرسوم** | **غير شاملة إلا إذا ذُكر صراحة** |
|  |  |  |  | **الإجمالي النهائي** | **[GRAND_TOTAL]** |

---

## 4. شروط الدفع

| البند | التفاصيل |
|---|---|
| الدفعة المقدمة | `[PAYMENT_UPFRONT]` |
| الدفعات المرحلية | `[PAYMENT_MILESTONES]` |
| عند التسليم | `[PAYMENT_DELIVERY]` |
| طريقة التحويل | `[PAYMENT_TRANSFER_METHOD]` |

---

## 5. التسليم والمدة

| البند | التفاصيل |
|---|---|
| مدة التنفيذ المتوقعة | `[ESTIMATED_DURATION]` |
| تاريخ بدء متوقع | `[EXPECTED_START_DATE]` |
| مراحل التسليم | `[DELIVERY_MILESTONES]` |

---

## 6. ما يشمله العرض وما يستثنيه

### يشمل

- `[INCLUDED_1]`
- `[INCLUDED_2]`
- `[INCLUDED_3]`

### لا يشمل

- الضرائب والرسوم الحكومية (إلا إذا ذُكرت صراحة)
- رسوم بوابات الدفع
- الاشتراكات الخارجية (تراخيص، APIs، خدمات طرف ثالث)
- الاستضافة أو الدومين أو SSL (إلا إذا ذُكرت صراحة)

---

## 7. الافتراضات والملاحظات

- `[ASSUMPTION_1]`
- `[ASSUMPTION_2]`
- `[NOTE_1]`

> أي تغيير على النطاق أو الافتراضات قد يتطلب **Change Request** وتعديل السعر/المدة.

---

## 8. اعتماد العرض

### 8.1 اعتماد داخلي — إلزامي

| الدور | الاسم | الحالة | التاريخ |
|---|---|---|---|
| أعدّه | TeraClientEngagementAgent | `[DRAFT / REVIEWED]` | `[DATE]` |
| اعتمده | Majed | `[APPROVED / CHANGES_REQUESTED]` | `[DATE]` |

### 8.2 إقرار العميل — اختياري حسب السياق

| الطرف | الاسم | التوقيع / الإقرار | التاريخ |
|---|---|---|---|
| ممثل العميل | `[CLIENT_SIGNATORY]` | `[OPTIONAL]` | `[DATE]` |
| ممثل Teranoo | `[PROVIDER_SIGNATORY]` | `[OPTIONAL]` | `[DATE]` |

---

## 9. ملاحظات الاستخدام

1. هذا الملف **Template** ويُملأ بواسطة TCEA.
2. لا يُرسل للعميل قبل اعتماد Majed.
3. لا تظهر فيه المصفوفة الداخلية أو طريقة الحساب التفصيلية.
4. إذا تغير النطاق، يُستخدم `Change Request Form` أو تُصدر نسخة Quote جديدة.
5. يجب أن يبقى متوافقاً مع `TeraPricingPolicy.md`.
