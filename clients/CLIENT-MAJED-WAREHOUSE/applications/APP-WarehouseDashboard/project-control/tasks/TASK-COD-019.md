# TASK-COD-019 — IIS Setup + Environment Configuration

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-019 |
| **المجموعة** | B7 — Deployment |
| **المرحلة** | Phase F — Deployment |
| **الوكيل** | engineering-agent |
| **التقدير** | 6–8h |
| **التبعية** | TASK-COD-003 ✅ |
| **الأولوية** | High |
| **الحالة** | 🟡 Assigned |

---

## 1. الهدف

التحقق من جاهزية المشروع للنشر على IIS المحلي وتوثيق خطوات النشر الكاملة.

## 2. ما تم بالفعل ✅

- `web.config` موجود في كلا المشروعين (Web + Api) — ASP.NET Core ModuleV2, in-process hosting
- `ConnectionStringHelper` يدعم env vars + hardcoded passwords
- `appsettings.json` يحتوي connection strings جاهزة

## 3. ما يحتاج إنجازه

### 3.1 التحقق من web.config
- [ ] التأكد من `stdoutLogEnabled="true"` للـ production debugging (ثم إعادته false)
- [ ] التأكد من `hostingModel="inprocess"` (أداء أفضل على IIS)
- [ ] التأكد من `processPath="dotnet"` (يتطلب تثبيت ASP.NET Core Hosting Bundle)

### 3.2 إنشاء `appsettings.Production.json` لكل مشروع
- **Web:** overrides للـ SyncApi BaseUrl (استخدام HTTPS على IIS)
- **Api:** overrides للـ CORS origins (إضافة hostname الفعلي)

### 3.3 تحديث CORS في Api/Program.cs
- إضافة hostname الفعلي للـ CORS policy (ليس فقط localhost)
- أو استخدام configurable CORS من appsettings

### 3.4 إنشاء دليل النشر (Deployment Guide)
ملف `docs/DEPLOYMENT_GUIDE.md` يحتوي:
1. المتطلبات المسبقة (Prerequisites)
   - .NET 8 Hosting Bundle
   - SQL Server
   - Oracle Client / ODP.NET
   - IIS Configuration
2. خطوات النشر
   - نشر الملفات (dotnet publish)
   - إنشاء Application Pool (No Managed Code)
   - إنشاء IIS Sites/Applications
   - تكوين SSL Certificate
   - تكوين Environment Variables
3. التحقق من التشغيل
   - اختبار الاتصال بـ Oracle
   - اختبار الاتصال بـ SQL Server
   - اختبار Dashboard
   - اختبار Admin Panel
   - اختبار Sync

### 3.5 تحديث IIS CORS + Environment
- تأكد من أن Api يقبل requests من hostname الفعلي
- تأكد من أن Web يتصل بالApi عبر hostname الفعلي (وليس localhost)

## 4. Allowed Write Targets

```
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Api/Infrastructure/
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Api/Program.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Program.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Api/appsettings.Production.json
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/appsettings.Production.json
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/docs/
```

## 5. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `appsettings.Production.json` موجود لكلا المشروعين | ⬜ |
| AC-2 | CORS يدعم hostname الفعلي + localhost | ⬜ |
| AC-3 | `DEPLOYMENT_GUIDE.md` مكتمل وواضح | ⬜ |
| AC-4 | `dotnet build -c Release` = 0 errors | ⬜ |
| AC-5 | لا secrets hardcoded في ملفات الإنتاج | ⬜ |

## 6. ملاحظات

- المستخدم يعمل على IIS local على نفس السيرفر
- السيرفر: 10.10.1.1 (نفس Oracle + SQL Server)
- لا حاجة لـ Docker أو CI/CD في هذه المرحلة
- الأوراكل والمعلومات موجودة في `appsettings.json` بالفعل
