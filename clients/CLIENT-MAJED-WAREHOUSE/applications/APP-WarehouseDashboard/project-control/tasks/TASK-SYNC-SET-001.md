# TASK-SYNC-SET-001 — Backend: ربط SyncSettings بـ Sync API

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-SET-001 |
| **المجموعة** | BACKEND-ENHANCEMENT |
| **النوع** | C# Backend |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

توسيع `SyncSettingsModel.cs` (في `Pages/admin-secure-panel/SyncSettings/`) لجلب بيانات实时 من Sync API، مع الحفاظ على وظائف الحفظ الحالية.

---

## 2. التغييرات المطلوبة

### 2.1 إضافة كلاسات البيانات

أضف داخل ملف `Index.cshtml.cs` (بعد الكلاس الرئيسي):

```csharp
public class SyncInfo
{
    public bool IsRunning { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public string? LastStatus { get; set; }
    public int LastRecordCount { get; set; }
}

public class SyncConfigInfo
{
    public int IntervalMinutes { get; set; } = 30;
    public bool IsAutoSyncEnabled { get; set; }
    public DateTime? LastSyncTimestamp { get; set; }
}

public class MappingItem
{
    public int Id { get; set; }
    public string OracleSource { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SqlTargetTable { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
```

### 2.2 إضافة خصائص جديدة لـ SyncSettingsModel

```csharp
// API data
public SyncInfo? SyncStatus { get; set; }
public SyncConfigInfo? SyncConfig { get; set; }
public List<MappingItem> Mappings { get; set; } = new();
public string? LoadError { get; set; }

// Computed properties for the view
public int ActiveMappingsCount => Mappings.Count(m => m.IsActive);
public int TotalMappingsCount => Mappings.Count;
public bool HasSyncError => SyncStatus?.LastStatus == "error";
public string? LastSyncStatusText => SyncStatus?.LastStatus switch
{
    "success" => "نجاح",
    "error" => "فشل",
    _ => null
};
```

### 2.3 إضافة HttpClient و FetchAsync

```csharp
private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };
```

وأضف دالة `FetchAsync<T>` (مطابقة للـ Sync/Index.cshtml.cs).

### 2.4 تحديث OnGetAsync

```csharp
public async Task OnGetAsync()
{
    // 1. Load from DB (existing code)
    var setting = await GetOrCreateSettingAsync();
    IntervalMinutes = setting.IntervalMinutes;
    IsAutoSyncEnabled = setting.IsAutoSyncEnabled;
    LastSyncTimestamp = setting.LastSyncTimestamp;

    // 2. Fetch from Sync API
    var apiBase = _configuration.GetValue<string>("SyncApiBaseUrl") ?? "http://localhost:5001";
    var client = _httpClient;

    var statusTask = FetchAsync<SyncInfo>(client, $"{apiBase}/api/sync/status");
    var configTask = FetchAsync<SyncConfigInfo>(client, $"{apiBase}/api/sync/config");
    var mappingsTask = FetchAsync<List<MappingItem>>(client, $"{apiBase}/api/sync/mappings");

    await Task.WhenAll(statusTask, configTask, mappingsTask);

    SyncStatus = statusTask.Result;
    SyncConfig = configTask.Result;
    if (mappingsTask.Result is { } mappings)
        Mappings = mappings;

    if (statusTask.Exception != null || configTask.Exception != null || mappingsTask.Exception != null)
    {
        LoadError = "تعذر الاتصال بخدمة المزامنة.";
    }
}
```

### 2.5 حقن IConfiguration

أضف `IConfiguration` إلى الـ Constructor:

```csharp
private readonly IConfiguration _configuration;

public SyncSettingsModel(WarehouseDashboardDbContext db, ILogger<SyncSettingsModel> logger, IConfiguration configuration)
{
    _db = db;
    _logger = logger;
    _configuration = configuration;
}
```

### 2.6 Fix Arabic text in OnPostAsync

السطران الحاليان يحتويان على encoding مشوّه:
- `ToastMessage = "�?م �?�?ظ �?�?�?عد�?د�?�? �?�?�?�?�?.";` → `ToastMessage = "تم حفظ الإعدادات بنجاح.";`
- `ToastMessage = "�?عذر �?�?ظ �?�?�?عد�?د�?�?. ير�?ى �?�?م�?�?و�?�? �?�?�?ق�?ً.";` → `ToastMessage = "تعذر حفظ الإعدادات. يرجى المحاولة لاحقاً.";`

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\SyncSettings\Index.cshtml.cs
```

**فقط هذا الملف. لا تلمس أي ملف آخر.**

---

## 4. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | `SyncInfo`, `SyncConfigInfo`, `MappingItem` كلاسات موجودة |
| AC-2 | `FetchAsync<T>` موجودة وتعمل |
| AC-3 | `OnGetAsync` تجلب من API بالتوازي (Parallel) |
| AC-4 | `ActiveMappingsCount` و `TotalMappingsCount` محسوبتان |
| AC-5 | `HasSyncError` و `LastSyncStatusText` محسوبتان |
| AC-6 | `IConfiguration` محقونة في الـ Constructor |
| AC-7 | النصوص العربية في `OnPostAsync` صحيحة (لا mojibake) |
| AC-8 | `dotnet build` ناجح بدون أخطاء |
| AC-9 | الحفظ (OnPostAsync) لا يزال يعمل بدون تغيير |

---

## 5. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ ملف واحد — Backend فقط |
| لا تغيير واجهة | ✅ (الواجهة ستتغير في المهمة الثانية) |
| لا تغيير Auth | ✅ |
| API آمن | ✅ FetchAsync تتعامل مع الأخطاء بهدوء |
| Build | ✅ متوقع 0 errors |

**Gate Status:** ✅ PASS
