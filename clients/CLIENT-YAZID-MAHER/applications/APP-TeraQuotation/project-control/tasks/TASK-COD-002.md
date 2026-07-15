# TASK-COD-002: Services Layer — Interfaces + Implementations

> **Batch 1 — Foundation | اليوم 1**
> **الحالة:** ✅ Approved (جاهز للتفويض)

---

## 1. الوصف

إنشاء طبقة الخدمات (Services) للتطبيق — واجهات (Interfaces) وتنفيذات (Implementations) لكل خدمة مطلوبة، وتسجيلها في Dependency Injection.

## 2. المخرجات المطلوبة

- [ ] 7 Service Interfaces في `Services/`
- [ ] 7 Service Implementations في `Services/`
- [ ] تسجيل جميع الخدمات في DI (App.xaml.cs)

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Services\
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\App.xaml.cs
```

## 4. Acceptance Criteria

- ✅ `dotnet build` يمر بدون أخطاء
- ✅ جميع الخدمات الـ 7 لها Interface + Implementation
- ✅ جميع الخدمات مسجلة في DI (ServiceCollection في App.xaml.cs)
- ✅ لا يوجد كود غير مطلوب

## 5. تفاصيل الخدمات

### 5.1 IQuotationService / QuotationService
```csharp
// مسؤول عن: إنشاء، تعديل، حفظ، حذف، استعراض العروض
Task<List<Quotation>> GetAllQuotationsAsync();
Task<Quotation?> GetQuotationByIdAsync(int id);
Task<Quotation?> GetQuotationByNumberAsync(string number);
Task<string> GenerateNextQuoteNumberAsync();
Task<Quotation> CreateQuotationAsync(Quotation quotation);
Task UpdateQuotationAsync(Quotation quotation);
Task DeleteQuotationAsync(int id);
Task<List<Quotation>> SearchQuotationsAsync(string? search, string? status);
Task AddItemToQuotationAsync(int quotationId, int itemId, string? s1Type, decimal? s1Price, string? s2Type, decimal? s2Price, string? s3Type, decimal? s3Price);
```

### 5.2 ISettingsService / SettingsService
```csharp
// مسؤول عن: إدارة الموردين، القطع، التوقيعات، إعدادات الترويسة
// Suppliers
Task<List<Supplier>> GetAllSuppliersAsync();
Task<Supplier> AddSupplierAsync(Supplier supplier);
Task UpdateSupplierAsync(Supplier supplier);
Task DeleteSupplierAsync(int id);

// Items
Task<List<Item>> GetAllItemsAsync();
Task<List<Item>> SearchItemsAsync(string search);
Task<Item> AddItemAsync(Item item);
Task UpdateItemAsync(Item item);
Task DeleteItemAsync(int id);

// Signatures
Task<List<Signature>> GetAllSignaturesAsync();
Task<Signature> AddSignatureAsync(Signature signature);
Task DeleteSignatureAsync(int id);

// Settings (Letterhead, etc.)
Task<string?> GetSettingAsync(string key);
Task SetSettingAsync(string key, string value);
```

### 5.3 IReportService / ReportService
```csharp
// مسؤول عن: التقارير الأربعة — جلب البيانات المُجمّعة
Task<List<SupplierComparisonDto>> GetSupplierComparisonAsync(int quotationId);
Task<List<TopItemDto>> GetTopItemsAsync(int topN, DateTime? from, DateTime? to);
Task<List<QuotationHistoryDto>> GetQuotationHistoryAsync(DateTime? from, DateTime? to, string? status);
Task<MonthlyTotalDto> GetMonthlyTotalAsync(int year);
```

### 5.4 IPdfService / PdfService
```csharp
// مسؤول عن: توليد PDF للعروض والتقارير باستخدام QuestPDF
Task<byte[]> GenerateQuotationPdfAsync(int quotationId);
Task<byte[]> GenerateReportPdfAsync(string reportType, object data);
```

### 5.5 IOutlookService / OutlookService
```csharp
// مسؤول عن: إرسال عرض عبر Outlook (اختياري — Fallback PDF)
Task<bool> SendViaOutlookAsync(int quotationId, string recipient);
bool IsOutlookInstalled { get; }
```

### 5.6 IBackupService / BackupService
```csharp
// مسؤول عن: النسخ الاحتياطي التلقائي واليدوي
Task BackupAsync();
Task<List<string>> GetBackupFilesAsync();
Task<bool> RestoreAsync(string backupFilePath);
```

### 5.7 INavigationService / NavigationService
```csharp
// مسؤول عن: التنقل بين الشاشات عبر MainWindow Frame
void NavigateTo<TView>() where TView : FrameworkElement;
void GoBack();
```

### 5.8 DTOs للتحديثات (في Models/ أو Services/)
```csharp
public class SupplierComparisonDto { /* QuotationId, ItemName, Supplier1Name, Supplier1Price, Supplier2Name, Supplier2Price, Supplier3Name, Supplier3Price, BestPrice, BestSupplier */ }
public class TopItemDto { /* ItemName, RequestCount, AveragePrice */ }
public class QuotationHistoryDto { /* QuoteNumber, Date, Description, Status, TotalPrice */ }
public class MonthlyTotalDto { /* Year, MonthlyBreakdown[], TotalQuotations, GrandTotal */ }
public class MonthlyBreakdown { /* Month, Count, Total */ }
```

## 6. تحديث DI (App.xaml.cs)

إضافة جميع الخدمات إلى `ServiceCollection`:

```csharp
services.AddSingleton<IQuotationService, QuotationService>();
services.AddSingleton<ISettingsService, SettingsService>();
services.AddSingleton<IReportService, ReportService>();
services.AddSingleton<IPdfService, PdfService>();
services.AddSingleton<IOutlookService, OutlookService>();
services.AddSingleton<IBackupService, BackupService>();
services.AddSingleton<INavigationService, NavigationService>();
```

## 7. Pre-Execution Gate Result

| # | السؤال | النتيجة |
|:-:|:-------|:-------:|
| 1 | مرتبطة بخطة معتمدة؟ | ✅ PROJECT_MASTER_PLAN.md — TASK-COD-002 |
| 2 | أصغر وحدة تنفيذية؟ | ✅ Services Layer فقط |
| 3 | هدف واحد؟ | ✅ |
| 4-22 | جميع البنود | ✅ PASS |
| **النتيجة النهائية** | | **🟢 PASS** |

## 8. Task Log

| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved — جاهز للتفويض إلى EngineeringAgent |
| 2026-07-13 | ✅ Delegated to EngineeringAgent |
| 2026-07-13 | ✅ Handback — 18 ملف، 0 Build Errors |
| 2026-07-13 | 🟢 **Post-Execution Review: PASS** |
