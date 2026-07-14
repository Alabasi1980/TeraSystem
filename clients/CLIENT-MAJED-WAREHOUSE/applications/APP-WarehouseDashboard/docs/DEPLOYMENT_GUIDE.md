# دليل نشر WarehouseDashboard على IIS

## المتطلبات المسبقة

1. **.NET 8 ASP.NET Core Hosting Bundle** — يُثبّت AspNetCoreModuleV2 (الرابط: https://dotnet.microsoft.com/download/dotnet/8.0)
2. **SQL Server** على `10.10.1.1`
3. **Oracle Client / ODP.NET** على السيرفر (لتواصل الـ API مع Oracle)
4. **IIS** مع تفعيل ASP.NET Core Module

---

## خطوات النشر

### 1. نشر الملفات

```bash
# نشر الـ Web Dashboard
dotnet publish -c Release -o C:\inetpub\WarehouseDashboard.Web

# نشر الـ API
dotnet publish -c Release -o C:\inetpub\WarehouseDashboard.Api
```

### 2. إنشاء Application Pool

1. افتح **IIS Manager**
2. اذهب إلى **Application Pools** → **Add Pool**
3. الإعدادات:
   - **Name:** `WarehouseDashboardPool`
   - **.NET CLR Version:** `No Managed Code`
   - **Managed Pipeline Mode:** `Integrated`
   - **Identity:** `ApplicationPoolIdentity`

### 3. إنشاء IIS Site — Web Dashboard

1. اذهب إلى **Sites** → **Add Website**
2. الإعدادات:
   - **Site name:** `WarehouseDashboard`
   - **Application pool:** `WarehouseDashboardPool`
   - **Physical path:** `C:\inetpub\WarehouseDashboard.Web`
   - **Binding:** HTTPS port 443 (أو منفذ مخصص)
   - **SSL Certificate:** شهادة HTTPS متاحة على السيرفر

### 4. إنشاء IIS Application — API

1. داخل WarehouseDashboard Site → **Add Application**
2. الإعدادات:
   - **Alias:** `api`
   - **Application pool:** `WarehouseDashboardPool`
   - **Physical path:** `C:\inetpub\WarehouseDashboard.Api`
   - **Binding:** نفس الموقع (الرابط الناتج: `/api`)

### 5. تكوين Environment Variables

**الطرق متعددة:**

**الطريقة الأولى — عبر IIS Manager:**
1. IIS Manager → Application Pool → `WarehouseDashboardPool` → **Advanced Settings**
2. أضف Environment Variables:
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ASPNETCORE_URLS` = `https://+:5001` (للـ API)

**الطريقة الثانية — عبر web.config:**
```xml
<environmentVariables>
  <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
</environmentVariables>
```

> ملاحظة: تم تحديث ملفات web.config تلقائياً لتضمين `ASPNETCORE_ENVIRONMENT=Production`

### 6. تكوين SSL

1. تأكد من وجود شهادة SSL مثبتة على السيرفر
2. اربط الشهادة بـ IIS Binding
3. الإعداد: **Require SSL** (اختياري لكن مُوصى به)

### 7. تأكد من صلاحيات المجلدات

- `IIS_IUSRS` يحتاج صلاحيات **READ** على مجلدات النشر
- مجلد `logs` يحتاج صلاحيات **WRITE** (للـ stdout logging)

```powershell
# إنشاء مجلد logs وإعطاء صلاحيات
New-Item -ItemType Directory -Path "C:\inetpub\WarehouseDashboard.Web\logs"
New-Item -ItemType Directory -Path "C:\inetpub\WarehouseDashboard.Api\logs"
icacls "C:\inetpub\WarehouseDashboard.Web\logs" /grant "IIS_IUSRS:(OI)(CI)W"
icacls "C:\inetpub\WarehouseDashboard.Api\logs" /grant "IIS_IUSRS:(OI)(CI)W"
```

---

## التحقق من التشغيل

1. افتح المتصفح على `https://10.10.1.1`
2. تأكد من ظهور **Dashboard**
3. افتح `https://10.10.1.1/admin-secure-panel`
4. سجّل الدخول بالباسورد المُعرّف
5. اختر **"اختبار الاتصال بـ Oracle"** → تأكد من النجاح
6. اختر **"تشغيل المزامنة"** → انتظر الاكتمال
7. عُد للـ **Dashboard** → تأكد من ظهور البيانات

---

## استكشاف الأخطاء

### stdout Logs
```text
C:\inetpub\WarehouseDashboard.Web\logs\stdout*.log
C:\inetpub\WarehouseDashboard.Api\logs\stdout*.log
```
> تم تفعيل stdout logging في web.config (`stdoutLogEnabled="true"`)

### Event Viewer
- **Windows Logs** → **Application** — تحقق من أخطاء ASP.NET Core Module

### IIS Failed Request Tracing
- من IIS Manager → اختر الموقع → **Failed Request Tracing** → فعّله

### تسجيل التغييرات (CORS)
- تم تعديل CORS policy في Api/Program.cs ليقرأ الأ Origins من `appsettings.json`
- الإعداد: `CORS:Origins` (مصفوفة من الروابط المسموحة)
- القيمة الافتراضية: `https://localhost:5000`, `http://localhost:5000`, `https://10.10.1.1`, `http://10.10.1.1`

---

## ملفات الإعدادات حسب البيئة

| الملف | البيئة | الوصف |
|-------|--------|-------|
| `appsettings.json` | Development | الإعدادات الافتراضية للتطوير |
| `appsettings.Production.json` | Production | الإعدادات للنشر على IIS |

> ASP.NET Core يحمّل `appsettings.Production.json` تلقائياً عندما `ASPNETCORE_ENVIRONMENT=Production`

---

## ملاحظات مهمة

- **لا تُشغّل `dotnet run` على IIS** — يستخدم `dotnet publish` فقط
- **تأكد من تثبيت ASP.NET Core Hosting Bundle** قبل أي شيء
- إذا ظهرت أخطاء **500**: تحقق من stdout logs
- إذا ظهرت أخطاء **502**: تأكد من أن الـ application pool يشتغل
- إذا ظهرت أخطاء **404**: تحقق من `physical path` و صلاحيات المجلدات
- **CORS:** إذا لم تُعرّف `CORS:Origins` في appsettings، ستُستخدم القيم الافتراضية

---

## هيكل المشروع النهائي

```
C:\inetpub\
├── WarehouseDashboard.Web\          ← الـ Dashboard (port 443)
│   ├── appsettings.json
│   ├── appsettings.Production.json  ← NEW
│   ├── web.config                   ← MODIFIED (stdout + env vars)
│   ├── wwwroot\
│   └── WarehouseDashboard.Web.dll
│
├── WarehouseDashboard.Api\          ← الـ API (port 5001)
│   ├── appsettings.json
│   ├── appsettings.Production.json  ← NEW
│   ├── web.config                   ← MODIFIED (stdout + env vars)
│   └── WarehouseDashboard.Api.dll
```

---

## التغييرات في هذه المهمة (TASK-COD-019)

| الملف | الحالة | الوصف |
|-------|--------|-------|
| `Web/appsettings.Production.json` | **NEW** | إعدادات Production للـ Web |
| `Api/appsettings.Production.json` | **NEW** | إعدادات Production للـ API |
| `Api/Program.cs` | **MODIFIED** | CORS policy يقرأ Origins من Configuration |
| `Web/web.config` | **MODIFIED** | stdout logging + ASPNETCORE_ENVIRONMENT |
| `Api/web.config` | **MODIFIED** | stdout logging + ASPNETCORE_ENVIRONMENT |
