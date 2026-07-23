# 🤖 AI Query Assistant — خطة التنفيذ الكاملة

**تاريخ:** 2026-07-22
**الحالة:** 📋 خطة معتمدة للتنفيذ
**الهدف:** دمج مساعد AI ذكي في صفحة QueryTester الحالية لإنشاء وتعديل وتحسين كويريز Views

---

## 📑 فهرس المحتويات

1. [الرؤية الكاملة](#1-الرؤية-الكاملة)
2. [هندسة الحل](#2-هندسة-الحل)
3. [تصميم قاعدة البيانات](#3-تصميم-قاعدة-البيانات)
4. [System Prompt — هوية AI](#4-system-prompt--هوية-ai)
5. [API Endpoints](#5-api-endpoints)
6. [مكونات UI — Bottom Drawer + Saved Queries](#6-مكونات-ui--bottom-drawer--saved-queries)
7. [تدفق العمل الكامل](#7-تدفق-العمل-الكامل)
8. [قائمة المهام (TASK-IDs)](#8-قائمة-المهام-task-ids)
9. [الاعتماديات والترتيب](#9-الاعتماديات-والترتيب)
10. [اعتبارات الأمان والأداء](#10-اعتبارات-الأمان-والأداء)

---

## 1. الرؤية الكاملة

### 🎯 المفهوم

مساعد AI مدمج في صفحة **QueryTester** ـ يجلس مع المستخدم في **Bottom Drawer**، يرى الكويري الحالي، يفهم قاعدة البيانات، وينشئ/يحسن/يطوّر كويريز Views.

### 🔄 التدفق الرئيسي

```
① المستخدم يفتح QueryTester
② يرى المحرر (CodeMirror) + شات AI في الأسفل
③ يختار "محادثة جديدة" أو يفتح كويري/فيو محفوظ
④ يطلب من AI: "أريد كويري يجيب حركة الأصناف مع الموردين"
⑤ AI يستكشف Schema (ينفذ SELECTs صغيرة) → يفهم الجداول
⑥ AI يعدل SQL في CodeMirror مباشرة
⑦ المستخدم يشغّل الكويري → يشوف النتيجة
⑧ يطلب تعديل: "زود عمود السعر" → AI يعدّل
⑨ يوصل للنتيجة المطلوبة → يحفظها
```

### ✨ المبادئ الأساسية

| المبدأ | الشرح |
|--------|-------|
| **AI مساعد وليس منفّذ** | يقدم اقتراحات، يعدل SQL، ينفذ استكشاف — لكن المستخدم هو القائد |
| **المحادثة تبع الكويري** | كل كويري محفوظ له محادثة مستقلة خاصة به |
| **AI يرى كل شيء** | يقرأ Schema Database، يقرأ الكويري الحالي، يقرأ نتائج الاستعلامات |
| **الكويري النهائي لك** | AI يعدل في المحرر الذي أنت تتحكم فيه — لا شيء خفي |
| **الاستمرارية** | تفتح كويري بعد شهر → AI يتذكر كل النقاش السابق |

---

## 2. هندسة الحل

### 2.1 المكونات الجديدة

```
WarehouseDashboard.Web/
│
├── Pages/admin-secure-panel/QueryTester/
│   ├── Index.cshtml           ← (معدّل) + Bottom Drawer + Chat
│   └── Index.cshtml.cs        ← (معدّل) + Endpoints جديدة
│
├── Infrastructure/
│   ├── IAIProvider.cs         ← (موجود) — لا تغيير
│   ├── OpenCodeGoAdapter.cs   ← (موجود) — لا تغيير
│   ├── AIAssistantOptions.cs  ← (موجود) — لا تغيير
│   ├── AIAssistantRequest.cs  ← (موجود) — لا تغيير
│   │
│   ├── AiQueryService.cs      ← 🆕 خدمة الذكاء الاصطناعي للاستعلامات
│   ├── AiQueryContext.cs      ← 🆕 إدارة سياق المحادثة + Schema
│   └── SavedQueryService.cs   ← 🆕 CRUD للكويريز المحفوظة
│
├── Data/
│   ├── WarehouseDashboardDbContext.cs  ← (معدّل) + DbSets جديدة
│   └── Migrations/                     ← (جديد) هجرة قاعدة البيانات
│
├── Models/Entities/
│   ├── SavedQuery.cs          ← 🆕 كيان الكويري المحفوظ
│   └── AiConversation.cs     ← 🆕 كيان المحادثة
│
└── wwwroot/js/
    └── queryTester.js         ← (معدّل) + Chat logic + Editor integration
```

### 2.2 Diagram — تدفق البيانات

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           AI Query Assistant                                 │
│                                                                             │
│  ┌──────────────┐    ┌──────────────┐    ┌────────────────────────┐        │
│  │  المستخدم    │◄──▶│  Chat UI     │◄──▶│  AiQueryService        │        │
│  │  (يكتب)     │    │  (Bottom     │    │  ┌──────────────────┐  │        │
│  └──────────────┘    │   Drawer)    │    │  │ IAIProvider      │  │        │
│                      └──────┬───────┘    │  │ (DeepSeek)       │  │        │
│                             │            │  └────────┬─────────┘  │        │
│                             ▼            │           │            │        │
│                      ┌──────────────┐    │           ▼            │        │
│                      │  CodeMirror  │    │  ┌──────────────────┐  │        │
│                      │  Editor      │◄───│  │ Schema Explorer  │  │        │
│                      │  (SQL)       │    │  │ (SELECT limit 100│  │        │
│                      └──────────────┘    │  │  لاستكشاف)       │  │        │
│                             │            │  └──────────────────┘  │        │
│                             ▼            └────────────────────────┘        │
│                      ┌──────────────┐                                      │
│                      │  SQL Server  │                                      │
│                      │  (Config +   │                                      │
│                      │  Data +      │                                      │
│                      │  SavedQrys)  │                                      │
│                      └──────────────┘                                      │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. تصميم قاعدة البيانات

### 3.1 جدول `SavedQueries` — الكويريز المحفوظة

```sql
CREATE TABLE dbo.SavedQueries (
    Id              INT            IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(200)  NOT NULL,        -- اسم الكويري (مثال: "حركة الأصناف مع الموردين")
    Description     NVARCHAR(500)  NULL,             -- وصف اختياري
    SqlQuery        NVARCHAR(MAX)  NOT NULL,         -- نص الاستعلام
    DataSourceType  NVARCHAR(50)   NOT NULL DEFAULT 'SqlServer',  -- SqlServer / Oracle
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT CK_SavedQueries_DataSourceType 
        CHECK (DataSourceType IN ('SqlServer', 'Oracle'))
);

CREATE INDEX IX_SavedQueries_Name ON dbo.SavedQueries(Name);
CREATE INDEX IX_SavedQueries_UpdatedAt ON dbo.SavedQueries(UpdatedAt DESC);
```

### 3.2 جدول `AiConversations` — سجل المحادثات

```sql
CREATE TABLE dbo.AiConversations (
    Id              BIGINT         IDENTITY(1,1) PRIMARY KEY,
    SavedQueryId    INT            NULL,             -- NULL = محادثة لكويري غير محفوظ بعد
    Role            NVARCHAR(20)   NOT NULL,          -- 'user', 'assistant', 'system'
    Message         NVARCHAR(MAX)  NOT NULL,          -- نص الرسالة
    SqlSnapshot     NVARCHAR(MAX)  NULL,              -- صورة SQL وقت إرسال الرسالة
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_AiConversations_SavedQueries 
        FOREIGN KEY (SavedQueryId) 
        REFERENCES dbo.SavedQueries(Id) 
        ON DELETE CASCADE,
    CONSTRAINT CK_AiConversations_Role 
        CHECK (Role IN ('user', 'assistant', 'system'))
);

CREATE INDEX IX_AiConversations_SavedQueryId 
    ON dbo.AiConversations(SavedQueryId, CreatedAt);
```

### 3.3 العلاقة

```
SavedQueries (1) ──── (N) AiConversations
     │                          │
     │                          │ ON DELETE CASCADE
     │                          │ (حذف الكويري = حذف المحادثة تلقائياً)
     │
     └── SqlQuery = الكويري النهائي
     └── Name = الاسم الذي تختاره للحفظ
```

### 3.4 EF Core Entities

```csharp
public class SavedQuery
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SqlQuery { get; set; } = string.Empty;
    public string DataSourceType { get; set; } = "SqlServer";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<AiConversation> Conversations { get; set; } = new List<AiConversation>();
}

public class AiConversation
{
    public long Id { get; set; }
    public int? SavedQueryId { get; set; }
    public string Role { get; set; } = string.Empty;  // user / assistant / system
    public string Message { get; set; } = string.Empty;
    public string? SqlSnapshot { get; set; }  // SQL في لحظة إرسال الرسالة
    public DateTime CreatedAt { get; set; }
    
    public SavedQuery? SavedQuery { get; set; }
}
```

---

## 4. System Prompt — هوية AI

```text
أنت مساعد متخصص في SQL و Views Databases.
أنت جزء من تطبيق WarehouseDashboard — لوحة تحكم لإدارة المستودعات.

هويتك:
- اسمك: 🤖 مساعد الاستعلامات
- تخصصك الوحيد: إنشاء وتحسين وتعديل استعلامات SQL — خاصة Views
- لا تعمل أي شيء خارج SQL
- تتحدث العربية الفصحى البسيطة

قواعد عملك:
1. عندما يطلب منك المستخدم استعلاماً، ادرس Schema أولاً:
   - نفّذ SELECT TOP 5 * FROM [جدول] لتفهم هيكل الجدول
   - ابحث في INFORMATION_SCHEMA عن العلاقات (Foreign Keys)
   - حلّل البيانات لتفهم المعنى

2. عند اقتراح SQL:
   - ضع الكويري الكامل في منصة كود ```sql ... ```
   - اشرح باختصار ماذا يفعل كل جزء
   - حذّر من أي pitfalls محتملة

3. عند تعديل كويري موجود:
   - اقرأ الكويري الحالي من المحرر أولاً
   - افهم ما يفعله قبل أن تقترح تغييراً
   - احتفظ بكل الأجزاء الصحيحة — لا تعيد كتابة الكل إلا إذا لزم

4. صلاحياتك:
   - ✅ تنفيذ SELECT (حد 100 صف) — للاستكشاف فقط
   - ✅ استخدام INFORMATION_SCHEMA
   - ✅ تعديل الكويري الحالي في المحرر
   - ✅ اقتراح تحسينات أداء
   - ❌ ممنوع DELETE, UPDATE, INSERT, DROP, ALTER, CREATE
   - ❌ ممنوع تعديل أي شيء خارج SQL Editor

5. أسلوبك:
   - واضح ومباشر — اشرح المنطق
   - احترم أن المستخدم هو القائد — أنت تقدم اقتراحات فقط
   - إذا لم تفهم المطلوب، اسأل قبل أن تنفذ
   - إذا كان الطلب معقداً، اقترح خطة أولاً

6. حفظ الكويريز:
   - الكويريز النهائية تُحفظ في SavedQueries
   - لكل كويري محفوظ محادثة مستقلة
   - عندما يفتح المستخدم كويرياً محفوظاً، اقرأ المحادثة السابقة لتكمل من حيث توقفتم
```

---

## 5. API Endpoints

### 5.1 Chat with AI

| Method | Path | الوظيفة |
|--------|------|---------|
| `POST` | `/admin-secure-panel/QueryTester?handler=Chat` | إرسال رسالة + استقبال رد AI |

**Request:**
```json
{
  "message": "أريد كويري يعرض حركة الأصناف",
  "currentSql": "SELECT * FROM Items",
  "savedQueryId": null,
  "source": "SqlServer"
}
```

**Response:**
```json
{
  "success": true,
  "reply": "فهمت طلبك... هذا هو الكويري المقترح:\n\n```sql\nSELECT ...\n```",
  "suggestedSql": "SELECT i.ItemName, SUM(m.Qty) AS Total...",
  "updateEditor": true,
  "conversationId": 42
}
```

### 5.2 Execute AI Explorer Query

| Method | Path | الوظيفة |
|--------|------|---------|
| `POST` | `/admin-secure-panel/QueryTester?handler=AiExecute` | AI ينفذ SELECT لاستكشاف |

**Request:**
```json
{
  "sql": "SELECT TOP 5 * FROM Items",
  "source": "SqlServer",
  "conversationId": 42
}
```

**Response:**
```json
{
  "success": true,
  "columns": [...],
  "rows": [...],
  "rowCount": 5,
  "elapsed": 45,
  "maxRows": 100
}
```

**الأمان:** نفس `SqlReadonlyGuard` + حد 100 صف فقط + مهلة 15 ثانية + سجل كل استعلام.

### 5.3 Saved Queries CRUD

| Method | Path | الوظيفة |
|--------|------|---------|
| `GET` | `/admin-secure-panel/QueryTester?handler=ListQueries` | عرض كل الكويريز المحفوظة |
| `POST` | `/admin-secure-panel/QueryTester?handler=SaveQuery` | حفظ كويري جديد |
| `GET` | `/admin-secure-panel/QueryTester?handler=LoadQuery` | تحميل كويري + محادثته |
| `POST` | `/admin-secure-panel/QueryTester?handler=UpdateQuery` | تحديث كويري موجود |
| `POST` | `/admin-secure-panel/QueryTester?handler=DeleteQuery` | حذف كويري + محادثته |
| `POST` | `/admin-secure-panel/QueryTester?handler=ClearConversation` | مسح محادثة لكويري موجود (بدون حذف الكويري) |

### 5.4 Schema Info for AI

| Method | Path | الوظيفة |
|--------|------|---------|
| `GET` | `/admin-secure-panel/QueryTester?handler=SchemaInfo` | معلومات Schema كاملة (Tables + Columns + FKs) لبناء context |

**Response:**
```json
{
  "success": true,
  "schema": {
    "tables": [
      {
        "name": "Items",
        "columns": [
          { "name": "ItemId", "type": "int", "nullable": false },
          { "name": "ItemName", "type": "nvarchar", "nullable": false }
        ],
        "foreignKeys": [
          { "column": "SupplierId", "referencesTable": "Suppliers", "referencesColumn": "SupplierId" }
        ]
      }
    ]
  }
}
```

---

## 6. مكونات UI — Bottom Drawer + Saved Queries

### 6.1 Bottom Drawer — واجهة الدردشة

```
┌─────────────────────────────────────────────────────────────┐
│  الصفحة الحالية (QueryTester) — كل شيء كما هو               │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  CodeMirror Editor (SQL)                              │  │
│  │                                                       │  │
│  │  SELECT ...                                           │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  النتائج                                              │  │
│  │  ...                                                  │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│══════════════════════════════════════════════════════════════║│
│  🤖 مساعد الاستعلامات          [🧹 محادثة جديدة] [📁 حفظ]   ││
│  ─────────────────────────────────────────────               ││
│                                                              ││
│  أنا: أريد كويري يعرض حركة الأصناف مع الموردين               ││
│                                                              ││
│  🤖:理解了 طلبك... سأستكشف الجداول أولاً                   ││
│  > نفّذت SELECT TOP 5 FROM Items (3 rows)                    ││
│  > نفّذت SELECT TOP 5 FROM Movements (10 rows)              ││
│  > وجدت العلاقة: Items.ItemID ← Movements.ItemID           ││
│                                                              ││
│  هذا هو الكويري المقترح:                                    ││
│  ┌──────────────────────────────────────────────────────┐  ││
│  │ SELECT i.ItemName, m.Quantity, m.MovementDate        │  ││
│  │ FROM Items i                                         │  ││
│  │ JOIN Movements m ON i.ItemID = m.ItemID              │  ││
│  └──────────────────────────────────────────────────────┘  ││
│                                                              ││
│  ┌──────────────────────────────────┐ [▶ تحديث] [🔄 أعد]    ││
│  │ اكتب رسالتك هنا...                │                      ││
│  └──────────────────────────────────┘                       ││
══════════════════════════════════════════════════════════════║│
└─────────────────────────────────────────────────────────────┘
```

### 6.2 عناصر Bottom Drawer

| العنصر | الوصف |
|--------|-------|
| **شريط العنوان** | 🤖 مساعد الاستعلامات + أزرار: 🧹 محادثة جديدة, 📁 حفظ, 📂 القائمة |
| **منطقة المحادثة** | تظهر الرسائل بشكل متدرج، مع تمايز لوني بين المستخدم والـ AI |
| **مدخل النص** | TextBox مع زر إرسال ودعم Shift+Enter |
| **أزرار سريعة** | ▶ تحديث الكويري (AI يحدث SQL في المحرر), 🔄 أعد المحاولة |
| **إنديكر استكشاف** | عندما ينفذ AI استعلامات استكشافية، تظهر كرسائل صغيرة بلون مختلف |
| **مؤشر تحميل** | أثناء انتظار رد AI |

### 6.3 قائمة الكويريز المحفوظة — Modal

عند الضغط على 📂 القائمة، تظهر نافذة:

```
┌────────────────────────────────────────────────┐
│  📁 الكويريز المحفوظة                           │
│                                                │
│  🔍 [ابحث........................]             │
│                                                │
│  ┌──────────────────────────────────────────┐  │
│  │ 📄 حركة الأصناف مع الموردين   2026-07-22 │  │
│  │   SELECT i.ItemName, ...                 │  │
│  │   [فتح] [تعديل الاسم] [🗑 حذف]           │  │
│  ├──────────────────────────────────────────┤  │
│  │ 📄 مخزون ناقص                  2026-07-21 │  │
│  │   SELECT ItemName, SUM(Qty)...           │  │
│  │   [فتح] [تعديل الاسم] [🗑 حذف]           │  │
│  └──────────────────────────────────────────┘  │
│                                                │
│  [➕ كويري جديد]               [إغلاق]         │
└────────────────────────────────────────────────┘
```

### 6.4 Flow — حفظ كويري + فتحه

```
📁 حفظ
   │
   ├── ① المستخدم يضغط "حفظ"
   ├── ② Modal باسم الكويري
   │    └── "حركة الأصناف مع الموردين" + وصف (اختياري)
   ├── ③ SaveQuery API → يُنشئ SavedQuery + يربط AiConversations
   └── ④ Toast: ✅ تم الحفظ

📂 فتح كويري محفوظ
   │
   ├── ① المستخدم يختار كويرياً من القائمة
   ├── ② LoadQuery API → يعيد Sql + AiConversations
   ├── ③ CodeMirror ← SqlQuery
   ├── ④ Bottom Drawer ← آخر 50 رسالة محادثة
   └── ⑤ AI يستقبل: "عدنا لكويري 'حركة الأصناف مع الموردين' — أكمل من حيث توقفت"
```

### 6.5 Vitality & Polish Checklist للـ UI

- [ ] Skeleton loading أثناء تحميل المحادثة
- [ ] Toast عند حفظ الكويري/حذفه
- [ ] Micro-animations عند ظهور رسائل AI (stagger)
- [ ] Empty state لـ "لا توجد رسائل سابقة"
- [ ] Connection status للـ AI (متصل/غير متصل)
- [ ] Scroll التلقائي لآخر رسالة في الشات
- [ ] Responsive: Bottom drawer يتحول لـ full-screen على الموبايل

---

## 7. تدفق العمل الكامل

### 7.1 أول مرة — محادثة جديدة بدون كويري

```
User                                   AI                              System
  │                                      │                                │
  ├─ يفتح QueryTester                   │                                │
  │                                      │                                │
  ├─ يكتب: "أريد كويري لحركة الأصناف"   │                                │
  │  ──────────────────────────────────▶│                                │
  │                                      │                                │
  │                                      ├── يقرأ الكويري الحالي (فارغ)  │
  │                                      ├── يطلب SchemaInfo              │
  │                                      ├── ينفذ AiExecute(SELECT Items) │
  │                                      ├── ينفذ AiExecute(SELECT Movmts)│
  │                                      │                                │
  │                                      ├── يكوّن SQL المقترح            │
  │  ◀───────────────────────────────────┤                                │
  │  الـ AI يرد مع الكويري المقترح       │                                │
  │  "suggestedSql": "SELECT ..."       │                                │
  │                                      │                                │
  ├─ يشوف الاقتراح                      │                                │
  ├─ يضغط "تحديث المحرر" ← CodeMirror ← suggestedSql                      │
  ├─ يضغط "تشغيل" ← يشوف النتائج        │                                │
  │                                      │                                │
  ├─ يكتب: "زود عمود السعر"             │                                │
  │  ──────────────────────────────────▶│                                │
  │                                      ├── يقرأ الكويري الحالي          │
  │                                      ├── يقترح تعديل                  │
  │  ◀───────────────────────────────────┤                                │
  │                                      │                                │
  ├─ يضغط تحديث → يجرب → يعجبه          │                                │
  ├─ يضغط "حفظ" → يدخل اسم              │                                │
  │  ──────────────────────────────────▶│ ← SaveQuery API                │
  │  ✅ "تم حفظ الكويري"                │                                │
  └                                      └                                ┘
```

### 7.2 العودة لكويري محفوظ

```
User                                   AI                              System
  │                                      │                                │
  ├─ يفتح قائمة الكويريز                 │                                │
  ├─ يختار "حركة الأصناف مع الموردين"    │                                │
  │  ──────────────────────────────────▶│ ← LoadQuery API                │
  │                                      │                                │
  │  ◀── SqlQuery + AiConversations ────┤                                │
  │                                      │                                │
  ├─ CodeMirror ← SqlQuery              │                                │
  ├─ Bottom Drawer ← آخر 50 رسالة       │                                │
  │                                      │                                │
  ├─ يقرأ المحادثة القديمة               │                                │
  ├─ يكتب: "زود تصفية على المستودع 2"   │                                │
  │  ──────────────────────────────────▶│                                │
  │                                      ├── يقرأ السياق الكامل          │
  │                                      ├── يعدل SQL                     │
  │  ◀───────────────────────────────────┤                                │
  │                                      │                                │
  └                                      └                                ┘
```

### 7.3 تعدد الكويريز — كل واحد بمحادثته

```
SavedQueries Table                       AiConversations Table
┌──────────────────────────┐            ┌──────────────────────────────────────┐
│ Id=1: "حركة الأصناف"     │            │ SavedQueryId=1 → "أريد كويري..."     │
│ Sql=SELECT i....         │            │ SavedQueryId=1 → "هذا الكويري..."    │
│                          │            │ SavedQueryId=1 → "زود السعر..."      │
│ Id=2: "مخزون ناقص"      │            │ SavedQueryId=1 → "تم التعديل..."     │
│ Sql=SELECT I...          │            ├──────────────────────────────────────┤
│                          │            │ SavedQueryId=2 → "أريد كويري..."     │
│ Id=3: "مبيعات الشهر"     │            │ SavedQueryId=2 → "اقتراح..."         │
│ Sql=SELECT M...          │            ├──────────────────────────────────────┤
│                          │            │ SavedQueryId=3 → "طلب..."            │
│                          │            │ SavedQueryId=3 → "رد..."            │
└──────────────────────────┘            └──────────────────────────────────────┘

كل كويري = محادثة مستقلة تماماً 👌
```

---

## 8. قائمة المهام (TASK-IDs)

### 🏗️ Phase 1 — البنية التحتية (Backend)

| TASK-ID | الوصف | لمن | الحجم |
|---------|-------|-----|-------|
| **TASK-AIQ-001** | إنشاء جدولي `SavedQueries` و `AiConversations` + EF Core Entities + Migration | engineering-agent-dotnet | صغير |
| **TASK-AIQ-002** | إنشاء `SavedQueryService` — CRUD + List + Search للكويريز المحفوظة | engineering-agent-dotnet | صغير-متوسط |
| **TASK-AIQ-003** | إنشاء `AiQueryContext` — إدارة سياق المحادثة + Schema info + حدود الـ 50 رسالة | engineering-agent-dotnet | متوسط |
| **TASK-AIQ-004** | إنشاء `AiQueryService` — منطق AI للاستعلامات (System Prompt، Schema، استكشاف، تحديث SQL) | engineering-agent-dotnet | كبير |
| **TASK-AIQ-005** | API Endpoints: Chat, AiExecute, SavedQueries CRUD, SchemaInfo, ClearConversation | engineering-agent-dotnet | كبير |

### 🎨 Phase 2 — واجهة المستخدم (UI)

| TASK-ID | الوصف | لمن | الحجم |
|---------|-------|-----|-------|
| **TASK-AIQ-006** | تصميم Bottom Drawer Chat Panel + دمجها في صفحة QueryTester | ui-designer | متوسط |
| **TASK-AIQ-007** | ربط الـ Chat مع CodeMirror Editor (JS: تحديث SQL، قراءة SQL الحالي، Apply) | ui-designer | كبير |
| **TASK-AIQ-008** | قائمة الكويريز المحفوظة (Modal) + زر الحفظ + تحميل الكويري مع المحادثة | ui-designer | متوسط |
| **TASK-AIQ-009** | Vitality & Polish: Skeleton, Toasts, Empty States, Micro-animations, Responsive | ui-designer | صغير |

### 🧪 Phase 3 — اختبار وضبط

| TASK-ID | الوصف | لمن | الحجم |
|---------|-------|-----|-------|
| **TASK-AIQ-010** | اختبار Chat Flow كامل: محادثة جديدة ← استكشاف ← تعديل SQL ← حفظ ← فتح ← متابعة | qa-agent | صغير |
| **TASK-AIQ-011** | اختبار الأمان: رفض AI لـ DELETE/UPDATE/DROP، حد 100 صف، مهلة 30 ثانية | qa-agent | صغير |
| **TASK-AIQ-012** | ضبط System Prompt بناءً على نتائج الاختبار (تعديل السلوك إن لزم) | TeraAgent (أنا) | صغير |

---

## 9. الاعتماديات والترتيب

```
TASK-AIQ-001 (جداول) ────▶ TASK-AIQ-002 (SavedQueryService) ────▶ TASK-AIQ-005 (APIs)
                                   │                                      │
                                   └──▶ TASK-AIQ-003 (AiQueryContext) ────┘
                                                   │
                                                   ▼
                                            TASK-AIQ-004 (AiQueryService)
                                                   │
                                                   ▼
                                            TASK-AIQ-005 (API Endpoints)
                                                   │
                            ┌──────────────────────┼──────────────────────┐
                            ▼                      ▼                      ▼
                     TASK-AIQ-006           TASK-AIQ-007           TASK-AIQ-008
                     (Bottom Drawer)        (Editor Integration)   (Saved Queries UI)
                            │                      │                      │
                            └──────────────────────┼──────────────────────┘
                                                   ▼
                                            TASK-AIQ-009
                                         (Vitality & Polish)
                                                   │
                                                   ▼
                                            TASK-AIQ-010
                                            TASK-AIQ-011   (اختبارات)
                                            TASK-AIQ-012
```

### 🎯 تسلسل التنفيذ الموصى به:

```
الأسبوع 1: TASK-AIQ-001 → 002 → 003 → 004 (Backend كامل)
الأسبوع 2: TASK-AIQ-005 → 006 → 007 (APIs + Chat UI + Editor)
الأسبوع 3: TASK-AIQ-008 → 009 (حفظ/فتح + Vitality)
الأسبوع 4: TASK-AIQ-010 → 011 → 012 (اختبار + ضبط)
```

---

## 10. اعتبارات الأمان والأداء

### 10.1 الأمان

| الإجراء | التفاصيل |
|---------|----------|
| **AI لا ينفذ DELETE/UPDATE/DROP** | نفس `SqlReadonlyGuard` الموجود — يضاف له endpoint خاص (AiExecute) مع تشديد أعلى |
| **حد 100 صف للاستكشاف** | AiExecute يسمح بـ 100 صف كحد أقصى — يمنع استنزاف SQL Server |
| **مهلة 15 ثانية** | AiExecute مهلة 15 ثانية — أسرع من QueryTester العادي |
| **كل استعلام يُسجّل** | تسجيل استعلامات الـ AI في AiConversations مع `role=system` للتدقيق |
| **تطهير النصوص** | HTML Encoding لجميع النصوص قبل عرضها في الشات |
| **API Key آمن** | ممنوع إرسال API Key للـ UI — يبقى في Server Side فقط |

### 10.2 الأداء

| الإجراء | التفاصيل |
|---------|----------|
| **حد 50 رسالة** | آخر 50 رسالة فقط تُحفظ وتحمل — القديم يتقشر |
| **Cache Schema** | SchemaInfo يُخزّن مؤقتاً لمدة 5 دقائق — يمنع ضرب قاعدة البيانات كل سؤال |
| **Streaming قيد المراجعة** | حالياً `thinking: disabled` — يمكن إضافة streaming لاحقاً لتحسين تجربة الانتظار |
| **MemoryCache للنتائج** | نتائج استكشاف الـ AI تخزّن مؤقتاً لمدة دقيقتين لمنع التكرار |

### 10.3 صلاحيات AI — جدول واضح

| الإجراء | مسموح؟ | الحدود |
|---------|:------:|--------|
| تنفيذ SELECT (استكشاف) | ✅ | 100 صف كحد أقصى |
| استخدام INFORMATION_SCHEMA | ✅ | بدون حد |
| تعديل SQL في CodeMirror | ✅ | فقط باقتراح وموافقة المستخدم |
| اقتراح تحسينات SQL | ✅ | بدون حدود |
| تنفيذ DELETE / UPDATE / INSERT | ❌ | ممنوع |
| تنفيذ DDL (CREATE, ALTER, DROP) | ❌ | ممنوع |
| الوصول لبيانات خارج SQL Server | ❌ | ممنوع |
| تعديل أي شيء خارج SQL Editor | ❌ | ممنوع |

---

## ✅ التوقيع

| البند | الحالة |
|-------|--------|
| الرؤية متفق عليها مع Majed | ✅ |
| التصميم متوافق مع Architecture الحالية | ✅ |
| قاعدة البيانات لا تتعارض مع الجداول الموجودة | ✅ (جداول جديدة فقط) |
| الـ AI Infrastructure جاهز (IAIProvider موجود) | ✅ |
| UI متوافق مع Design System (blue-theme.css) | ✅ |
| **جاهز للتنفيذ** | **✅** |

---

> **إعداد:** Tera Agent (منسّق المشروع)
> **تاريخ:** 2026-07-22
> **الموقع:** `enhancements/AI_QUERY_ASSISTANT_EXECUTION_PLAN.md`
> **المراجعة:** Majed ✅
