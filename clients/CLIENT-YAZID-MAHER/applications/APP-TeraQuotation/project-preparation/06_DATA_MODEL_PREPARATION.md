# 06_DATA_MODEL_PREPARATION.md — TeraQuotation

> **Data Model Preparation Document**
> **المشروع:** TeraQuotation — نظام إدارة عروض أسعار قطع السيارات
> **التقنية:** SQLite + EF Core (.NET 8) — WPF Desktop
> **الحجم:** Small (8 كيانات، علاقات محدودة)
> **تاريخ الإصدار:** 2026-07-13
> **المصادر:** APPLICATION_BLUEPRINT.md ✅ + 08_TECHNICAL_ARCHITECTURE.md ✅ + TERA_PROJECT_DECISION.md ✅ + `dotnet-wpf-sqlite` Profile ✅

---

## Lifecycle Header

| الحقل | القيمة |
|:------|:-------|
| **Document State** | **Module Baseline Approved** |
| **Baseline Module** | TeraQuotation (Full Application) |
| **Current State** | Module Baseline Approved |
| **Owner** | Software Designer Agent (مُصمم) |
| **Last Review** | 2026-07-13 |
| **Expiry** | End of Project Delivery |

---

## قائمة المحتويات

1. [نظرة عامة](#1-نظرة-عامة)
2. [Entity Relationship Diagram (نصي)](#2-entity-relationship-diagram-نصي)
3. [تفصيل الكيانات (Entities)](#3-تفصيل-الكيانات-entities)
   - 3.1 [User](#31-user)
   - 3.2 [Supplier](#32-supplier)
   - 3.3 [Item](#33-item)
   - 3.4 [Quotation](#34-quotation)
   - 3.5 [QuotationItem](#35-quotationitem)
   - 3.6 [Signature](#36-signature)
   - 3.7 [Setting](#37-setting)
   - 3.8 [AuditLog (اختياري)](#38-auditlog-اختياري)
4. [ملخص Data Annotations و Fluent API](#4-ملخص-data-annotations-و-fluent-api)
5. [تفصيل العلاقات (Relationships)](#5-تفصيل-العلاقات-relationships)
6. [قرار تخزين أسماء الموردين كنصوص](#6-قرار-تخزين-أسماء-الموردين-كنصوص)
7. [أنواع الحقول في SQLite ونظيرها في C#](#7-أنواع-الحقول-في-sqlite-ونظيرها-في-c)
8. [استراتيجية Migrations](#8-استراتيجية-migrations)
9. [سيناريوهات استعلام شائعة (LINQ)](#9-سيناريوهات-استعلام-شائعة-linq)
10. [سيناريوهات الأداء والحجم](#10-سيناريوهات-الأداء-والحجم)
11. [Gaps and Recommendations](#11-gaps-and-recommendations)

---

## 1. نظرة عامة

### 1.1 الغرض من هذا الملف

توفير **تصميم كامل لقاعدة البيانات** لمشروع TeraQuotation، بحيث يتمكن **EngineeringAgent** من:
- إنشاء Entity Classes (C#) في مجلد `Models/`
- إنشاء `AppDbContext` مع DbSet Properties
- تكوين العلاقات (Fluent API + Data Annotations)
- كتابة أول Migration وتطبيقها
- كتابة استعلامات LINQ دقيقة في طبقة Services

### 1.2 قواعد أساسية

| القاعدة | الشرح |
|:--------|:------|
| **لا Stored Procedures** | SQLite لا يدعمها |
| **لا Views** | كل الاستعلامات عبر LINQ مباشر |
| **لا Triggers** | المنطق في طبقة Services (C#) وليس في قاعدة البيانات |
| **EF Core فقط** | لا ADO.NET خام — الاستفادة من Migrations والعلاقات |
| **Fluent API + Data Annotations** | الاثنان معاً للتحكم الكامل بالـ Schema |
| **Single Project** | Entities داخل `Models/`، DbContext داخل `Data/` |
| **Pluralization** | أسماء الجداول بصيغة الجمع (EF Core Convention) |

### 1.3 الكيانات

| # | الكيان | جدول | نوع | إلزامي؟ |
|:-:|:-------|:-----|:---|:-------:|
| 1 | **User** | `Users` | أساسي | ✅ نعم |
| 2 | **Supplier** | `Suppliers` | أساسي | ✅ نعم |
| 3 | **Item** | `Items` | أساسي | ✅ نعم |
| 4 | **Quotation** | `Quotations` | أساسي | ✅ نعم |
| 5 | **QuotationItem** | `QuotationItems` | أساسي | ✅ نعم |
| 6 | **Signature** | `Signatures` | أساسي | ✅ نعم |
| 7 | **Setting** | `Settings` | أساسي | ✅ نعم |
| 8 | **AuditLog** | `AuditLogs` | اختياري | ❌ يُقرر أثناء التنفيذ |

---

## 2. Entity Relationship Diagram (نصي)

```
┌───────────────────┐          ┌────────────────────────────┐
│       User        │          │        Quotation           │
│───────────────────│          │────────────────────────────│
│ PK  Id (INT)      │          │ PK  Id (INT)               │
│     Username      │          │ UQ  QuoteNumber (TEXT)     │
│     PasswordHash  │          │     Date (TEXT)            │
│     CreatedAt     │          │     Description (TEXT)     │
└───────────────────┘          │     Status (TEXT)          │
                               │     CreatedAt (TEXT)       │
┌───────────────────┐          │     UpdatedAt (TEXT)       │
│     Supplier      │          └───────────┬────────────────┘
│───────────────────│                      │ 1
│ PK  Id (INT)      │                      │
│     Name (TEXT)   │                      │ N (Cascade Delete)
│     ContactInfo   │              ┌────────┴────────────────┐
│     Notes         │              │     QuotationItem       │
│     CreatedAt     │              │─────────────────────────│
└───────────────────┘              │ PK  Id (INT)            │
                                   │ FK  QuotationId (INT)   │──┘
┌───────────────────┐              │ FK  ItemId (INT)        │──┐
│       Item        │              │     Supplier1Type (TEXT)│  │
│───────────────────│              │     Supplier1Price (DEC)│  │
│ PK  Id (INT)      │              │     Supplier2Type (TEXT)│  │
│     Name (TEXT)   │              │     Supplier2Price (DEC)│  │
│     Description   │              │     Supplier3Type (TEXT)│  │
│     Notes         │              │     Supplier3Price (DEC)│  │
│     CreatedAt     │              │     SortOrder (INT)     │  │
│     UpdatedAt     │              └─────────────────────────┘  │
└───────────────────┘                                           │
                                                                │
                    ┌──────────────────┐                        │
                    │    Signature     │                        │
                    │──────────────────│                        │
                    │ PK  Id (INT)     │                        │
                    │     Name (TEXT)  │                        │
                    │     OrderIndex   │                        │
                    │     CreatedAt    │                        │
                    └──────────────────┘                        │
                                                                │
                    ┌──────────────────┐                        │
                    │     Setting      │    ┌────────────────┐  │
                    │──────────────────│    │   AuditLog     │  │
                    │ PK  Key (TEXT)   │    │ (اختياري)      │  │
                    │     Value (TEXT) │    │────────────────│  │
                    └──────────────────┘    │ PK  Id (INT)   │  │
                                            │     Action     │  │
                 Item ←── QuotationItem ──→ Quotation        │  │
                 (N:1)         (N:1)              (1:N)      │  │
                                                             │  │
 ملاحظة: لا توجد علاقة مباشرة بين QuotationItem و Supplier  │  │
 لأن أسماء الموردين تُخزّن كنصوص (أنظر Section 6).          │  │
 └───────────────────────────────────────────────────────────┘  │
                                                                │
 مفتاح العلاقة بين Item و QuotationItem: ItemId (FK)            │
 Item: 1 ──→ N : QuotationItem                                  │
 (القطعة الواحدة قد تظهر في عدة عروض)                            │
 └────────────────────────────────────────────────────────────────┘
```

---

## 3. تفصيل الكيانات (Entities)

### 3.1 User

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد (Constraint) |
|:--------|:----------|:-----------|:-------:|:--------:|:-------------------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` — Auto-increment |
| `Username` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(50)]`, `HasMaxLength(50)` |
| `PasswordHash` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(500)]` |
| `CreatedAt` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |

**فهرس:** لا حاجة — جدول صغير (صف واحد).

**مثال الكود:**
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

**Fluent API:**
```csharp
entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
entity.Property(e => e.CreatedAt).IsRequired();
```

---

### 3.2 Supplier

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` |
| `Name` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(200)]` |
| `ContactInfo` | `string?` | `TEXT` | — | ❌ | `[MaxLength(500)]` |
| `Notes` | `string?` | `TEXT` | — | ❌ | `[MaxLength(1000)]` |
| `CreatedAt` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |

**فهرس:** لا حاجة — كمية الموردين صغيرة (< 100).

**ملاحظة:** لا يوجد `UpdatedAt` لأن معلومات المورد لا تتغير عادة بعد الإضافة. إذا احتجنا لاحقاً، نضيف الحقل في تحديث Schema.

**مثال الكود:**
```csharp
public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

### 3.3 Item

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` |
| `Name` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(200)]` |
| `Description` | `string?` | `TEXT` | — | ❌ | `[MaxLength(1000)]` |
| `Notes` | `string?` | `TEXT` | — | ❌ | `[MaxLength(1000)]` |
| `CreatedAt` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |
| `UpdatedAt` | `DateTime?` | `TEXT` | — | ❌ | — |

**فهرس:** 
- `IX_Items_Name` — فهرس على `Name` لتسريع البحث في كتالوج القطع (اختياري، يقرر أثناء التنفيذ إذا كان الأداء بطيئاً).

**مثال الكود:**
```csharp
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

### 3.4 Quotation

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` |
| `QuoteNumber` | `string` | `TEXT` | **UQ** | ✅ | `[Required]`, `[MaxLength(20)]`, `HasIndex(IsUnique=true)` |
| `Date` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |
| `Description` | `string?` | `TEXT` | — | ❌ | `[MaxLength(500)]` |
| `Status` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(20)]` |
| `CreatedAt` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |
| `UpdatedAt` | `DateTime?` | `TEXT` | — | ❌ | — |
| `PrintedAt` | `DateTime?` | `TEXT` | — | ❌ | — |

**فهرس فريد (Unique Index):**
```csharp
entity.HasIndex(e => e.QuoteNumber).IsUnique();
```

**قيم Status المسموحة:**

| القيمة | المعنى |
|:-------|:--------|
| `Draft` | مسودة — لا تزال قيد الإنشاء |
| `UpdatedWithPrices` | تم إدخال الأسعار وجاهزة للطباعة النهائية |
| `Printed` | تمت الطباعة (نهائية) |
| `PDFExported` | تم تصدير PDF |
| `SentViaOutlook` | تم الإرسال عبر Outlook |

**ملاحظة تحويلية:** في التصميم الأولي للـ Task، ذكر `Status (Draft/Complete)` ولكن الـ Architecture وسير العمل الفعلي يتطلب 5 حالات. تم اعتماد الحالات التفصيلية من Architecture Document لأنها تعكس workflow التطبيق بدقة.

**مثال الكود:**
```csharp
public class Quotation
{
    public int Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PrintedAt { get; set; }

    // Navigation Property
    public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}
```

---

### 3.5 QuotationItem

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` |
| `QuotationId` | `int` | `INTEGER` | **FK** | ✅ | `[Required]`, `ForeignKey` → `Quotation.Id` |
| `ItemId` | `int` | `INTEGER` | **FK** | ✅ | `[Required]`, `ForeignKey` → `Item.Id` |
| `Supplier1Type` | `string?` | `TEXT` | — | ❌ | `[MaxLength(100)]` |
| `Supplier1Price` | `decimal?` | `TEXT` | — | ❌ | `HasColumnType("decimal(18,2)")` |
| `Supplier2Type` | `string?` | `TEXT` | — | ❌ | `[MaxLength(100)]` |
| `Supplier2Price` | `decimal?` | `TEXT` | — | ❌ | `HasColumnType("decimal(18,2)")` |
| `Supplier3Type` | `string?` | `TEXT` | — | ❌ | `[MaxLength(100)]` |
| `Supplier3Price` | `decimal?` | `TEXT` | — | ❌ | `HasColumnType("decimal(18,2)")` |
| `SortOrder` | `int` | `INTEGER` | — | ✅ | `[Required]`, Default: `0` |

**فهرس مركب (تسريع الاستعلامات):**
```csharp
entity.HasIndex(e => new { e.QuotationId, e.SortOrder });
```

**العلاقات:**
- `QuotationId` → FK إلى `Quotation.Id` مع **Cascade Delete** (حذف العرض يحذف بنوده)
- `ItemId` → FK إلى `Item.Id` مع **Restrict** (يمنع حذف قطعة مستخدمة في عروض)

**مثال الكود:**
```csharp
public class QuotationItem
{
    public int Id { get; set; }
    public int QuotationId { get; set; }
    public int ItemId { get; set; }

    public string? Supplier1Type { get; set; }
    public decimal? Supplier1Price { get; set; }
    public string? Supplier2Type { get; set; }
    public decimal? Supplier2Price { get; set; }
    public string? Supplier3Type { get; set; }
    public decimal? Supplier3Price { get; set; }

    public int SortOrder { get; set; }

    // Navigation Properties
    public Quotation Quotation { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
```

**أنواع الأسعار (SupplierXPrice):** تستخدم `decimal?` (Nullable) لأنه في مرحلة الطباعة الأولى (WithoutPrices) تكون الأسعار فارغة، وتُملأ لاحقاً.

---

### 3.6 Signature

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` |
| `Name` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(200)]` |
| `OrderIndex` | `int` | `INTEGER` | — | ✅ | `[Required]` |
| `CreatedAt` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |

**فهرس:** `IX_Signatures_OrderIndex` — لترتيب التوقيعات عند الطباعة.

**مثال الكود:**
```csharp
public class Signature
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

### 3.7 Setting

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Key` | `string` | `TEXT` | **PK** | ✅ | `[Required]`, `[MaxLength(100)]` |
| `Value` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(2000)]` |

**ملاحظة:** هذا جدول Key-Value بسيط. **لا Auto-increment** — المفتاح هو النص نفسه.

**المفاتيح المتوقعة (Planned Keys):**

| Key | Example Value | الغرض |
|:----|:--------------|:------|
| `LetterheadLogoPath` | `C:\Users\...\logo.png` | مسار صورة الشعار |
| `CompanyName` | `مؤسسة يزيد ماهر` | اسم الشركة في الترويسة |
| `CompanyAddress` | `الرياض، المملكة العربية السعودية` | عنوان الشركة |
| `CompanyPhone` | `+966 5X XXX XXXX` | هاتف الشركة |
| `CompanyEmail` | `yazid@example.com` | البريد الإلكتروني |
| `FirstTimeSetupDone` | `true` | هل تم الإعداد الأولي؟ |
| `LastBackupDate` | `2026-07-13 15:00:00` | تاريخ آخر نسخة احتياطية |
| `LastActiveScreen` | `QuotationForm` | آخر شاشة مفتوحة (لاستعادة عند الفتح) |
| `PasswordLastChanged` | `2026-07-13` | تاريخ آخر تغيير لكلمة المرور |

**مثال الكود:**
```csharp
[PrimaryKey(nameof(Key))]
public class Setting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
```

> **تنبيه:** استخدام `[PrimaryKey]` في EF Core الحديث، أو Fluent API: `entity.HasKey(e => e.Key)`.

---

### 3.8 AuditLog (اختياري)

| الخاصية | النوع (C#) | نوع SQLite | المفتاح | Required | القيد |
|:--------|:----------|:-----------|:-------:|:--------:|:------|
| `Id` | `int` | `INTEGER` | **PK** | ✅ | `IDENTITY (1,1)` |
| `Action` | `string` | `TEXT` | — | ✅ | `[Required]`, `[MaxLength(100)]` |
| `Description` | `string?` | `TEXT` | — | ❌ | `[MaxLength(2000)]` |
| `Timestamp` | `DateTime` | `TEXT` | — | ✅ | `[Required]` |

**فهرس:** `IX_AuditLog_Timestamp` — لترتيب السجلات زمنياً.

**أمثلة على قيم Action:**
- `LOGIN_SUCCESS`، `LOGIN_FAILED`
- `QUOTATION_CREATED`، `QUOTATION_UPDATED`، `QUOTATION_PRINTED`
- `QUOTATION_EXPORTED_PDF`، `QUOTATION_SENT_OUTLOOK`
- `SETTINGS_CHANGED`، `PASSWORD_CHANGED`
- `ITEM_ADDED`، `ITEM_DELETED`، `SUPPLIER_ADDED`
- `BACKUP_CREATED`، `DATABASE_RESTORED`

**مثال الكود:**
```csharp
public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
}
```

> **توصية:** يتم تنفيذ `AuditLog` إذا أظهر EngineeringAgent أن الجهد ضئيل (< 30 دقيقة) وإلا يُؤجل للإصدار الثاني.

---

## 4. ملخص Data Annotations و Fluent API

### 4.1 Data Annotations (على الكلاس)

```csharp
[Required]        // not-null
[MaxLength(n)]    // TEXT Maximum Length (EF Core يترجمها إلى طول نصي)
```

### 4.2 Fluent API في OnModelCreating

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // === User ===
    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
        entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
    });

    // === Supplier ===
    modelBuilder.Entity<Supplier>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.ContactInfo).HasMaxLength(500);
        entity.Property(e => e.Notes).HasMaxLength(1000);
        entity.Property(e => e.CreatedAt).IsRequired();
    });

    // === Item ===
    modelBuilder.Entity<Item>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.Notes).HasMaxLength(1000);
        entity.Property(e => e.CreatedAt).IsRequired();

        entity.HasIndex(e => e.Name).HasDatabaseName("IX_Items_Name");
    });

    // === Quotation ===
    modelBuilder.Entity<Quotation>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.QuoteNumber).IsRequired().HasMaxLength(20);
        entity.Property(e => e.Date).IsRequired();
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.Status).IsRequired().HasMaxLength(20)
               .HasDefaultValue("Draft");
        entity.Property(e => e.CreatedAt).IsRequired();

        entity.HasIndex(e => e.QuoteNumber).IsUnique()
              .HasDatabaseName("IX_Quotations_QuoteNumber");
    });

    // === QuotationItem ===
    modelBuilder.Entity<QuotationItem>(entity =>
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Supplier1Type).HasMaxLength(100);
        entity.Property(e => e.Supplier1Price).HasColumnType("decimal(18,2)");
        entity.Property(e => e.Supplier2Type).HasMaxLength(100);
        entity.Property(e => e.Supplier2Price).HasColumnType("decimal(18,2)");
        entity.Property(e => e.Supplier3Type).HasMaxLength(100);
        entity.Property(e => e.Supplier3Price).HasColumnType("decimal(18,2)");

        entity.Property(e => e.SortOrder).IsRequired().HasDefaultValue(0);

        // العلاقة مع Quotation (Cascade Delete)
        entity.HasOne(e => e.Quotation)
              .WithMany(q => q.QuotationItems)
              .HasForeignKey(e => e.QuotationId)
              .OnDelete(DeleteBehavior.Cascade);

        // العلاقة مع Item (Restrict — لا تحذف قطعة مستخدمة)
        entity.HasOne(e => e.Item)
              .WithMany()
              .HasForeignKey(e => e.ItemId)
              .OnDelete(DeleteBehavior.Restrict);

        // فهرس مركب
        entity.HasIndex(e => new { e.QuotationId, e.SortOrder })
              .HasDatabaseName("IX_QuotationItems_QuotationId_SortOrder");
    });

    // === Signature ===
    modelBuilder.Entity<Signature>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.OrderIndex).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();

        entity.HasIndex(e => e.OrderIndex)
              .HasDatabaseName("IX_Signatures_OrderIndex");
    });

    // === Setting ===
    modelBuilder.Entity<Setting>(entity =>
    {
        entity.HasKey(e => e.Key);
        entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Value).IsRequired().HasMaxLength(2000);
    });

    // === AuditLog (اختياري) ===
    // modelBuilder.Entity<AuditLog>(entity =>
    // {
    //     entity.HasKey(e => e.Id);
    //     entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
    //     entity.Property(e => e.Description).HasMaxLength(2000);
    //     entity.Property(e => e.Timestamp).IsRequired();
    //
    //     entity.HasIndex(e => e.Timestamp)
    //           .HasDatabaseName("IX_AuditLog_Timestamp");
    // });
}
```

### 4.3 جدول المقارنة: Data Annotation vs Fluent API

| العنصر | Data Annotation | Fluent API |
|:-------|:---------------|:-----------|
| المفاتيح الأساسية | `[Key]` / `[PrimaryKey]` | `HasKey()` |
| المفاتيح الخارجية | `[ForeignKey(name)]` | `HasOne().WithMany().HasForeignKey()` |
| الفهارس | ❌ غير مدعوم | `HasIndex()` ✅ |
| Default Values | ❌ غير مدعوم | `HasDefaultValue()` ✅ |
| Column Type | ❌ غير مدعوم | `HasColumnType()` ✅ |
| العلاقات | `[ForeignKey]` | `HasOne()/WithMany()` ✅ |

> **توصية:** استخدم Fluent API للعلاقات والفهارس، و Data Annotations للقيود البسيطة (Required, MaxLength).

---

## 5. تفصيل العلاقات (Relationships)

### 5.1 Quotation → QuotationItem (1 : N)

| الجانب | الكيان | شرح |
|:-------|:-------|:-----|
| **الأب (Parent)** | `Quotation` | عرض سعر واحد |
| **الابن (Child)** | `QuotationItem` | بنود متعددة داخل العرض |
| **المفتاح الخارجي** | `QuotationItem.QuotationId` → `Quotation.Id` |
| **Cascade Delete** | ✅ نعم — حذف العرض يحذف جميع بنوده تلقائياً |

**الكود:**
```csharp
// في Quotation
public ICollection<QuotationItem> QuotationItems { get; set; }

// في Fluent API
entity.HasOne(e => e.Quotation)
      .WithMany(q => q.QuotationItems)
      .HasForeignKey(e => e.QuotationId)
      .OnDelete(DeleteBehavior.Cascade);
```

### 5.2 Item → QuotationItem (1 : N)

| الجانب | الكيان | شرح |
|:-------|:-------|:-----|
| **الأب (Parent)** | `Item` | قطعة واحدة في الكتالوج |
| **الابن (Child)** | `QuotationItem` | قد تظهر في عدة عروض |
| **المفتاح الخارجي** | `QuotationItem.ItemId` → `Item.Id` |
| **Cascade Delete** | ❌ **Restrict** — يمنع حذف قطعة مستخدمة في أي عرض |

**الكود:**
```csharp
entity.HasOne(e => e.Item)
      .WithMany()  // لا حاجة لـ Navigation Property في Item
      .HasForeignKey(e => e.ItemId)
      .OnDelete(DeleteBehavior.Restrict);
```

> **تأثير Restrict:** إذا حاول المستخدم حذف قطعة مستخدمة في عرض سابق أو حالي، سيرمي EF Core `DbUpdateException` ويجب على Service Layer ترجمة هذا إلى رسالة مفهومة: "لا يمكن حذف هذه القطعة لأنها مستخدمة في عرض سعر موجود."

### 5.3 Supplier — مستقل (لا علاقة مباشرة)

| الجانب | الكيان | شرح |
|:-------|:-------|:-----|
| **جدول Suppliers** | `Supplier` | قائمة الموردين (للإدارة والاختيار فقط) |
| **QuotationItem** | `QuotationItem` | يخزن اسم المورد كنص (ليس FK) |

**لا توجد علاقة FK بين QuotationItem و Supplier.** لمزيد من التفاصيل، راجع Section 6.

### 5.4 بقية الكيانات — مستقلة

| الكيان | علاقات |
|:-------|:-------|
| **User** | مستقل — لا FK لأي جدول |
| **Signature** | مستقل — يستخدم فقط للطباعة |
| **Setting** | Key-Value — لا علاقات |
| **AuditLog** | مستقل — سجل أحداث فقط |

---

## 6. قرار تخزين أسماء الموردين كنصوص

### 6.1 القرار (من ADR-005)

```
تخزين أسماء الموردين في QuotationItem كنصوص (string)
بدلاً من Foreign Keys إلى جدول Suppliers.
```

### 6.2 لماذا؟ — حماية البيانات التاريخية

عند إنشاء عرض سعر في 2026-07-13، قد يكون المورد اسمه **"مؤسسة أحمد للتجارة"** وبعد سنة يغيّر اسمه إلى **"مؤسسة أحمد الدولية"**. إذا استخدمنا FK، فإن العرض القديم سيظهر الاسم الجديد تلقائياً، مما يحرف المعلومة التاريخية.

**مع النصوص:** يبقى اسم المورد كما كان وقت إنشاء العرض.

### 6.3 كيف يعمل في واجهة المستخدم؟

```
1. المستخدم يفتح QuotationForm
2. يضيف قطعة ← يختار "المورد الأول" من ComboBox
   → الـ ComboBox مصدره جدول Suppliers (Name)
3. عند اختيار المورد، يُنسخ الاسم (النص) إلى Supplier1Type
4. المستخدم يدخل السعر في حقل Supplier1Price
5. يتم حفظ QuotationItem مع النص والسعر

→ في المستقبل، حتى لو حُذف المورد من جدول Suppliers،
  يبقى اسمه محفوظاً في العرض.
```

### 6.4 التأثير على الاستعلامات

| السيناريو | مع النصوص | مع FK |
|:----------|:----------|:------|
| عرض عرض قديم | يظهر الاسم الأصلي | يظهر الاسم الحالي (قد يكون مختلفاً) |
| تقرير مقارنة الموردين | {X} | `JOIN Suppliers` + `GROUP BY` |
| حذف مورد | آمن — لا تأثير على العروض القديمة | يحتاج Soft Delete أو `SET NULL` |

### 6.5 ماذا عن تقرير مقارنة أسعار الموردين؟ (تقرير D1)

بما أن الأسماء نصوص وليست FK، فإن تجميع الأسعار حسب المورد يتم بـ `GROUP BY` على أعمدة `Supplier1Type`, `Supplier2Type`, `Supplier3Type`. هذا يعني:

- إذا ظهر الاسم نفسه في أكثر من عرض كـ `Supplier1Type`، سيتم تجميعه بشكل صحيح.
- إذا كان الاسم مكتوباً بحرف مختلف في عرض آخر، سيُعامَل كمورد مختلف.

> **توصية:** يجب توحيد كتابة أسماء الموردين في شاشة Settings لضمان دقة التقارير.

---

## 7. أنواع الحقول في SQLite ونظيرها في C#

### 7.1 جدول أنواع SQLite ↔ C# (EF Core Mapping)

| نوع C# | نوع SQLite | ملاحظات |
|:-------|:-----------|:--------|
| `int` | `INTEGER` | Auto-increment للـ PK |
| `long` | `INTEGER` | — |
| `bool` | `INTEGER` (0/1) | — |
| `string` | `TEXT` | EF Core يخزن `string` كـ `TEXT` |
| `char` | `TEXT` (حرف واحد) | — |
| `DateTime` | `TEXT` | بصيغة ISO 8601: `2026-07-13 14:30:00` |
| `DateTime?` | `TEXT` (NULLABLE) | نفس الشيء مع السماح بـ NULL |
| `decimal` | `TEXT` | **❗ هام:** SQLite لا يدعم DECIMAL أصلاً. EF Core يخزنه كـ `TEXT`. استخدام `HasColumnType("decimal(18,2)")` للتوثيق فقط — في SQLite يُخزّن كنص. |
| `decimal?` | `TEXT` (NULLABLE) | — |
| `double` / `float` | `REAL` | 8-byte IEEE floating point |
| `byte[]` | `BLOB` | للصور أو الملفات المشفرة |
| `Guid` | `TEXT` | — |
| `Enum` | `INTEGER` | يُخزّن كقيمة رقمية |

### 7.2 قرارات خاصة بالأسعار

```csharp
// في Fluent API لكل حقل سعر:
entity.Property(e => e.Supplier1Price)
      .HasColumnType("decimal(18,2)");
// SQLite سيخزنها كـ TEXT، لكن EF Core سيتعامل معها كـ decimal
// عند القراءة والكتابة. في C#، نوعها decimal? (Nullable).
```

### 7.3 قرارات خاصة بالتواريخ

```csharp
// EF Core + SQLite يتعاملان مع DateTime كـ TEXT بصيغة:
// "2026-07-13 14:30:00.0000000"
// لا حاجة لتحديدHasColumnType — EF Core يفعلها تلقائياً.

entity.Property(e => e.CreatedAt).IsRequired();
// ينتج: CreatedAt TEXT NOT NULL
```

### 7.4 قرارات خاصة بالـ Boolean

لا توجد `bool` في هذا المشروع حالياً. لو احتجنا لاحقاً:

```csharp
// bool في C# → INTEGER في SQLite (0 = false, 1 = true)
entity.Property(e => e.IsActive).HasDefaultValue(true);
```

---

## 8. استراتيجية Migrations

### 8.1 لماذا EF Core Migrations؟

- **تتبع التغييرات** في Schema مع مرور الوقت
- **تطبيق تلقائي** للجداول والفهارس والعلاقات
- **إلغاء (Rollback)** إذا احتجنا العودة لإصدار سابق
- مناسبة لحجم المشروع الصغير

### 8.2 سير العمل

```
┌──────────────────────────────────────────────────────────────┐
│                   سير عمل Migrations                         │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  [1] إنشاء Entities + DbContext                               │
│       │                                                        │
│  [2] dotnet ef migrations add InitialCreate                   │
│       │                                                        │
│  [3] dotnet ef database update                                │
│       │                                                        │
│  [4] ✅ SQLite.db يُنشأ بالجداول المطلوبة                      │
│       │                                                        │
│  ──── لاحقاً: ────                                             │
│       │                                                        │
│  [5] إضافة/تعديل Entity                                       │
│       │                                                        │
│  [6] dotnet ef migrations add <اسم التغيير>                   │
│       │                                                        │
│  [7] dotnet ef database update                                │
│                                                               │
└──────────────────────────────────────────────────────────────┘
```

### 8.3 قواعد الـ Migrations

| القاعدة | الشرح |
|:--------|:------|
| **لا تعديل يدوي** | لا تعدل ملفات `.cs` في `Migrations/` يدوياً |
| **لا حذف** | لا تحذف Migration موجودة — استخدم `Remove-Migration` إذا لزم الأمر قبل `Update-Database` |
| **تسمية واضحة** | `InitialCreate`, `AddPrintedAtToQuotations`, `AddAuditLogTable` |
| **Migration واحدة لكل تغيير** | لا تدمج تغييرات غير متعلقة في Migration واحدة |
| **SQLite قيود** | SQLite لا يدعم `ALTER COLUMN` أو `DROP COLUMN` بسهولة. لتغيير عمود موجود، قد تحتاج لإعادة إنشاء الجدول. هذا وارد في SQLite فقط — يُفضل تصميم Schema نهائي من البداية. |

### 8.4 الأوامر المطلوبة

```powershell
# إنشاء أول Migration
dotnet ef migrations add InitialCreate
    --project .\src\TeraQuotation\TeraQuotation.csproj

# تطبيق التحديث على قاعدة البيانات
dotnet ef database update
    --project .\src\TeraQuotation\TeraQuotation.csproj

# إلغاء آخر Migration
dotnet ef migrations remove
    --project .\src\TeraQuotation\TeraQuotation.csproj

# إنشاء Script SQL (اختياري)
dotnet ef migrations script
    --project .\src\TeraQuotation\TeraQuotation.csproj
```

### 8.5 ترتيب أولوية الإنشاء في التنفيذ

| الخطوة | المهمة | الوصف |
|:------:|:-------|:------|
| 1 | إنشاء Entity Classes | جميع الـ 7 كيانات أساسية في `Models/` |
| 2 | إنشاء AppDbContext | مع DbSet Properties + Fluent API |
| 3 | `Add-Migration InitialCreate` | إنشاء أول Migration |
| 4 | `Update-Database` | تطبيق الجداول |
| 5 | التحقق | تشغيل التطبيق والتحقق من وجود الجداول في SQLite |

### 8.6 التعامل مع Migration في SQLite (قيود خاصة)

```text
⚠️ SQLite لا يدعم:
  - ALTER COLUMN (تغيير نوع عمود)
  - DROP COLUMN (حذف عمود) — قبل EF Core 6.1 دعم محدود
  - Rename Column (إعادة تسمية عمود)

🔧 الحل:
  - صمم Schema بدقة من البداية
  - إذا احتجت تغيير لاحق، استخدم إحدى الطريقتين:
    1. Rename Table → Create New Table → Copy Data → Drop Old Table
    2. تجاهل Migration المعقدة واحذف الـ DB يدوياً (فقط في التطوير)
  - للإنتاج: لا يوجد إنتاج فعلي — تطبيق محلي لمستخدم واحد.
    في حالة التحديث، ننشئ Backup قبل الـ Migration.
```

---

## 9. سيناريوهات استعلام شائعة (LINQ)

### 9.1 إنشاء عرض سعر جديد

```csharp
// QuotationService.CreateNewQuotationAsync
public async Task<Quotation> CreateNewQuotationAsync(string? description)
{
    var lastQuote = await _context.Quotations
        .OrderByDescending(q => q.Id)
        .FirstOrDefaultAsync();

    int nextNumber = (lastQuote != null)
        ? int.Parse(lastQuote.QuoteNumber.Replace("Q-", "")) + 1
        : 1;

    var quotation = new Quotation
    {
        QuoteNumber = $"Q-{nextNumber:D3}",  // Q-001, Q-002, ...
        Date = DateTime.Today,
        Description = description,
        Status = "Draft",
        CreatedAt = DateTime.Now
    };

    _context.Quotations.Add(quotation);
    await _context.SaveChangesAsync();
    return quotation;
}
```

### 9.2 جلب عرض سعر مع جميع البنود (Include)

```csharp
// QuotationService.GetQuotationByIdAsync
public async Task<Quotation?> GetQuotationByIdAsync(int id)
{
    return await _context.Quotations
        .Include(q => q.QuotationItems)
            .ThenInclude(qi => qi.Item)  // جلب اسم القطعة
        .OrderBy(q => q.Id)
        .FirstOrDefaultAsync(q => q.Id == id);
}
```

### 9.3 البحث في قائمة العروض

```csharp
// QuotationService.SearchQuotationsAsync
public async Task<List<Quotation>> SearchQuotationsAsync(string? keyword,
    string? statusFilter, DateTime? fromDate, DateTime? toDate)
{
    var query = _context.Quotations.AsQueryable();

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        query = query.Where(q =>
            q.QuoteNumber.Contains(keyword) ||
            (q.Description != null && q.Description.Contains(keyword)));
    }

    if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
    {
        query = query.Where(q => q.Status == statusFilter);
    }

    if (fromDate.HasValue)
        query = query.Where(q => q.Date >= fromDate.Value);

    if (toDate.HasValue)
        query = query.Where(q => q.Date <= toDate.Value);

    return await query
        .OrderByDescending(q => q.Date)
        .ThenByDescending(q => q.Id)
        .ToListAsync();
}
```

### 9.4 إضافة بند إلى عرض سعر

```csharp
// QuotationService.AddQuotationItemAsync
public async Task<bool> AddQuotationItemAsync(QuotationItem item)
{
    // تحديد SortOrder تلقائي
    var maxOrder = await _context.QuotationItems
        .Where(qi => qi.QuotationId == item.QuotationId)
        .MaxAsync(qi => (int?)qi.SortOrder) ?? -1;

    item.SortOrder = maxOrder + 1;

    _context.QuotationItems.Add(item);
    return await _context.SaveChangesAsync() > 0;
}
```

### 9.5 حذف بند من عرض سعر

```csharp
// QuotationService.RemoveQuotationItemAsync
public async Task<bool> RemoveQuotationItemAsync(int itemId)
{
    var item = await _context.QuotationItems.FindAsync(itemId);
    if (item == null) return false;

    _context.QuotationItems.Remove(item);
    return await _context.SaveChangesAsync() > 0;
}
```

### 9.6 البحث في كتالوج القطع

```csharp
// SettingsService.GetAllItemsAsync
public async Task<List<Item>> GetAllItemsAsync(string? search)
{
    var query = _context.Items.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(i =>
            i.Name.Contains(search) ||
            (i.Description != null && i.Description.Contains(search)));
    }

    return await query
        .OrderBy(i => i.Name)
        .ToListAsync();
}
```

### 9.7 تقرير مقارنة أسعار الموردين (D1)

```csharp
// ReportService.GetSupplierPriceComparisonAsync
// بما أن أسماء الموردين نصوص، نستخدم UNION ALL لتجميعها
public async Task<List<ComparisonRow>> GetSupplierPriceComparisonAsync()
{
    var supplier1Data = await _context.QuotationItems
        .Where(qi => qi.Supplier1Type != null && qi.Supplier1Price.HasValue)
        .GroupBy(qi => qi.Supplier1Type!)
        .Select(g => new ComparisonRow
        {
            SupplierName = g.Key,
            TotalAmount = g.Sum(qi => qi.Supplier1Price!.Value),
            AveragePrice = g.Average(qi => qi.Supplier1Price!.Value),
            ItemCount = g.Count()
        })
        .ToListAsync();

    var supplier2Data = await _context.QuotationItems
        .Where(qi => qi.Supplier2Type != null && qi.Supplier2Price.HasValue)
        .GroupBy(qi => qi.Supplier2Type!)
        .Select(g => new ComparisonRow
        {
            SupplierName = g.Key,
            TotalAmount = g.Sum(qi => qi.Supplier2Price!.Value),
            AveragePrice = g.Average(qi => qi.Supplier2Price!.Value),
            ItemCount = g.Count()
        })
        .ToListAsync();

    var supplier3Data = await _context.QuotationItems
        .Where(qi => qi.Supplier3Type != null && qi.Supplier3Price.HasValue)
        .GroupBy(qi => qi.Supplier3Type!)
        .Select(g => new ComparisonRow
        {
            SupplierName = g.Key,
            TotalAmount = g.Sum(qi => qi.Supplier3Price!.Value),
            AveragePrice = g.Average(qi => qi.Supplier3Price!.Value),
            ItemCount = g.Count()
        })
        .ToListAsync();

    // دمج كل البيانات وتجميعها
    return supplier1Data
        .Concat(supplier2Data)
        .Concat(supplier3Data)
        .GroupBy(r => r.SupplierName)
        .Select(g => new ComparisonRow
        {
            SupplierName = g.Key,
            TotalAmount = g.Sum(r => r.TotalAmount),
            AveragePrice = g.Average(r => r.AveragePrice),
            ItemCount = g.Sum(r => r.ItemCount)
        })
        .OrderByDescending(r => r.TotalAmount)
        .ToList();
}

public class ComparisonRow
{
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal AveragePrice { get; set; }
    public int ItemCount { get; set; }
}
```

### 9.8 أكثر القطع طلباً (D2)

```csharp
// ReportService.GetMostRequestedItemsAsync
public async Task<List<ItemRequestCount>> GetMostRequestedItemsAsync(int topN = 10)
{
    return await _context.QuotationItems
        .GroupBy(qi => new { qi.ItemId, qi.Item.Name })
        .Select(g => new ItemRequestCount
        {
            ItemName = g.Key.Name,
            RequestCount = g.Count()
        })
        .OrderByDescending(r => r.RequestCount)
        .Take(topN)
        .ToListAsync();
}

public class ItemRequestCount
{
    public string ItemName { get; set; } = string.Empty;
    public int RequestCount { get; set; }
}
```

### 9.9 إجمالي الشهر (D4)

```csharp
// ReportService.GetMonthlyTotalAsync
public async Task<MonthlyTotalDto> GetMonthlyTotalAsync(int year, int month)
{
    var items = await _context.QuotationItems
        .Include(qi => qi.Quotation)
        .Where(qi => qi.Quotation.Date.Year == year
                  && qi.Quotation.Date.Month == month
                  && qi.Quotation.Status != "Draft")  // المسودات لا تحتسب
        .ToListAsync();

    return new MonthlyTotalDto
    {
        Year = year,
        Month = month,
        TotalFromSupplier1 = items.Sum(i => i.Supplier1Price ?? 0),
        TotalFromSupplier2 = items.Sum(i => i.Supplier2Price ?? 0),
        TotalFromSupplier3 = items.Sum(i => i.Supplier3Price ?? 0),
        GrandTotal = items.Sum(i =>
            (i.Supplier1Price ?? 0) +
            (i.Supplier2Price ?? 0) +
            (i.Supplier3Price ?? 0)),
        QuotationCount = items.Select(i => i.QuotationId).Distinct().Count()
    };
}

public class MonthlyTotalDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalFromSupplier1 { get; set; }
    public decimal TotalFromSupplier2 { get; set; }
    public decimal TotalFromSupplier3 { get; set; }
    public decimal GrandTotal { get; set; }
    public int QuotationCount { get; set; }
}
```

### 9.10 التحقق من استخدام قطعة (قبل الحذف)

```csharp
// SettingsService — التحقق قبل حذف Item
public async Task<bool> IsItemUsedInAnyQuotationAsync(int itemId)
{
    return await _context.QuotationItems
        .AnyAsync(qi => qi.ItemId == itemId);
}

// في دالة DeleteItemAsync:
if (await IsItemUsedInAnyQuotationAsync(itemId))
{
    throw new InvalidOperationException(
        "لا يمكن حذف هذه القطعة لأنها مستخدمة في عروض سابقة.");
}
```

---

## 10. سيناريوهات الأداء والحجم

### 10.1 الحجم المتوقع

| الجدول | عدد السجلات المتوقع |
|:-------|:-------------------:|
| `Users` | 1 |
| `Suppliers` | < 100 |
| `Items` | < 5,000 |
| `Quotations` | < 10,000 |
| `QuotationItems` | < 100,000 (10 × عدد العروض) |
| `Signatures` | < 10 |
| `Settings` | < 30 |
| `AuditLog` | < 50,000 (اختياري) |

### 10.2 معالجة الكميات الكبيرة

في السيناريو غير المتوقع (أكثر من 50,000 عرض)، نوصي بـ:

| التحسين | الوصف |
|:--------|:------|
| **صفحنة (Pagination)** | استخدم `.Skip()` / `.Take()` بدلاً من `ToList()` للجداول الكبيرة |
| **فهارس إضافية** | أضف `HasIndex` على `Quotation.Date` و `Quotation.Status` |
| **تقليل الـ Includes** | لا تجلب `QuotationItems` إلا عند الحاجة (وليس في قائمة العروض) |
| **إلغاء Tracking** | استخدم `.AsNoTracking()` للقراءة فقط (`queries`) |

### 10.3 مثال صفحنة

```csharp
// لقائمة العروض (S4) مع صفحنة
public async Task<List<Quotation>> GetQuotationsPagedAsync(
    int page = 1, int pageSize = 20)
{
    return await _context.Quotations
        .OrderByDescending(q => q.Date)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()  // تحسين أداء — قراءة فقط
        .ToListAsync();
}
```

---

## 11. Gaps and Recommendations

### 11.1 Design Gaps

| # | الفجوة | النوع | التأثير | التوصية |
|:-:|:-------|:-----:|:--------|:--------|
| 1 | **`12_BUSINESS_RULES.md` غير موجود** | 📄 مفقود | قد توجد قواعد عمل غير موثقة تؤثر على الـ Data Model (مثلاً: هل يمكن أن يكون سعر المورد صفر؟ هل الوصف إلزامي لحالات معينة؟) | إنشاء Business Rules قبل التنفيذ — أو على الأقل توثيق القواعد المعروفة في Task Engineering Review |
| 2 | **`05_BUSINESS_WORKFLOWS.md` غير موجود** | 📄 مفقود | قد تؤثر تفاصيل Workflow على حالة Status الإضافية أو على علاقات إضافية | يُستخدم الـ Workflow من Architecture (Section 3) كمرجع لحين إنشاء الملف |
| 3 | **Status: Draft/Complete vs 5 حالات** | 🔄 تعارض بسيط | الـ Task الأصلي ذكر `Status (Draft/Complete)` والـ Architecture يحدد 5 حالات | تم اعتماد 5 حالات من Architecture لأنها تفصيلية وتتوافق مع Workflow. EngineeringAgent يقرر في Task Review ما إذا كان يبقي 5 حالات أو يدمج بعضها |
| 4 | **AuditLog: تنفيذ أو لا؟** | 🤔 قرار معلق | يضيف 30-60 دقيقة جهد إضافي | EngineeringAgent سيقرر أثناء التنفيذ بناءً على عبء العمل الإجمالي |

### 11.2 خلافات مع الـ Architecture

| النقطة | Architecture | Data Model | الإجراء |
|:-------|:------------|:-----------|:--------|
| `PrintedAt` في Quotation | يوجد `DateTime? PrintedAt` | تم تضمينه ✅ | متوافق |
| `SortOrder` في QuotationItem | يوجد `int SortOrder` | تم تضمينه ✅ | متوافق |
| `Status` التفصيلي | 5 حالات | 5 حالات ✅ | متوافق مع Architecture (خلاف مع Task Requirements) |
| Supplier ليس FK | ADR-005 | تم التأكيد ✅ | متوافق |
| `bool IsLocked` في User | موجود في Class Diagram كاختياري | لم يُضمّن ❌ | لا حاجة لمستخدم واحد. يُضاف لاحقاً إذا تطلب الأمر |

### 11.3 توصيات للملف التالي (07_SCREENS_AND_UI_STRUCTURE.md)

بناءً على تحليل Data Model، إليك توصيات لتصميم الشاشات:

1. **S3: QuotationForm** — ستحتاج إلى DataGrid بعنوانات:
   - `#` (رقم البند)
   - `اسم القطعة` (ComboBox من Items مع بحث)
   - `المورد الأول` (ComboBox من Suppliers → يُملأ النص)
   - `نوع/سعر المورد الأول`
   - `المورد الثاني` (ComboBox من Suppliers)
   - `نوع/سعر المورد الثاني`
   - `المورد الثالث` (ComboBox من Suppliers)
   - `نوع/سعر المورد الثالث`
   - `إجراء` (حذف)

2. **S4: QuotationList** — أعمدة الجدول:
   - `رقم العرض` (QuoteNumber)
   - `التاريخ` (Date)
   - `الوصف` (Description)
   - `الحالة` (Status — مع لون لكل حالة)
   - `إجراء` (فتح/طباعة/PDF/Outlook)

3. **S2: Settings** — 4 Tabs:
   - Tab 1: **Suppliers** — DataGrid (Name, ContactInfo, Notes) + إضافة/تعديل/حذف
   - Tab 2: **Items** — DataGrid (Name, Description) + بحث + إضافة/تعديل/حذف
   - Tab 3: **Signatures** — DataGrid (Name, OrderIndex) + إضافة/حذف
   - Tab 4: **Letterhead** — Form (شعار, اسم شركة, عنوان, هاتف, بريد)

4. **S5: Reports** — 4 أزرار تقارير مع منطقة عرض نتائج.

5. **S1: Login** — حقل كلمة مرور فقط + زر دخول (لا حقل اسم مستخدم لأن المستخدم واحد).

6. **مكونات UI إضافية مطلوبة:**
   - `StatusBadge` — عنصر عرض الحالة بلون مختلف لكل حالة (Draft=رمادي، UpdatedWithPrices=أزرق، Printed=أخضر، PDFExported=برتقالي، SentViaOutlook=بنفسجي)
   - `CurrencyTextBox` — حقل إدخال رقمي للأسعار بتنسيق العملة
   - `SupplierSelector` — ComboBox لاختيار مورد مع نقل الاسم كنص

---

## ختم الملف

| البند | الحالة |
|:------|:-------|
| **إعداد** | Software Designer Agent (مُصمم) |
| **تاريخ الإصدار** | 2026-07-13 |
| **الحالة** | ✅ Module Baseline Approved |
| **الاعتماد** | ⏳ بانتظار مراجعة TeraAgent و Majed |
| **المراجعة التالية** | بعد إنشاء 07_SCREENS_AND_UI_STRUCTURE.md — للتحقق من توافق UI مع Data Model |

---

**روابط ذات صلة:**
- [08_TECHNICAL_ARCHITECTURE.md](./08_TECHNICAL_ARCHITECTURE.md) — القرارات المعمارية والـ ADRs
- [APPLICATION_BLUEPRINT.md](./APPLICATION_BLUEPRINT.md) — المخطط العام للتطبيق
- [TERA_PROJECT_DECISION.md](../project-control/TERA_PROJECT_DECISION.md) — قرارات المشروع
- [dotnet-wpf-sqlite.md](../../tera-system/profiles/dotnet-wpf-sqlite.md) — Technology Profile المعتمد
