# 02_TECHNICAL_CONTEXT.md

# Technical Context — نظام إدارة طلبات المواد (MRMS)

## 1. Programming Language
- **C#** (.NET ecosystem)

## 2. Framework
- المقترح: **ASP.NET Core Blazor** (Server أو WebAssembly)
  - مناسب جداً للتطبيقات الداخلية
  - SignalR مدمج للإشعارات اللحظية
  - يدعم PWA بسهولة عبر Blazor WebAssembly
  - لغة واحدة (C#) للكل (Backend + Frontend)
- البديل: ASP.NET Core MVC + Razor Pages (كلاسيكي، بسيط)
- القرار النهائي: سيعرض Tera التوصية في مرحلة التحضير

## 3. Application Type
- تطبيق ويب متجاوب (Responsive Web)
- PWA (Progressive Web App) لدعم الإشعارات على الجوال بدون تطبيق منفصل

## 4. Database
- **SQL Server** (بما يتوافق مع بيئة Microsoft)
- الإصدار: Standard أو Express (حسب حجم الشركة)

## 5. ORM / Data Access
- **Entity Framework Core** — متوافق مع SQL Server وسهل الصيانة

## 6. Package Manager / CLI
- `dotnet CLI` + NuGet

## 7. UI / Design System
- مكتبة UI متوافقة مع Blazor (مقترحات):
  - **MudBlazor** — مجاني، دعم RTL ممتاز، مناسب لتطبيقات الأعمال
  - **Radzen Blazor** — مجاني، مكونات غنية
  - أو Bootstrap 5 (مع دعم RTL) إذا اخترنا MVC
- دعم ثنائي اللغة (Arabic RTL + English LTR)

## 8. Required External Libraries
- Entity Framework Core + SQL Server Provider
- ASP.NET Core Identity (لإدارة المستخدمين والصلاحيات)
- SendGrid / MailKit (للبريد الإلكتروني)
- Chart.js أو Blazor-charts (للرسوم البيانية)
- ClosedXML أو EPPlus (تصدير Excel)
- QuestPDF أو DinkToPdf (تصدير PDF)

## 9. Forbidden Libraries or Technologies
- لا توجد محظورات حالياً — سيتم توثيقها في PROJECT_RULES.md عند الحاجة

## 10. Runtime Environment
- **خادم (Server)**: Windows Server مع IIS، أو Linux مع Nginx/Kestrel
- محلياً: .NET SDK + SQL Server محلي أو Docker

## 11. Deployment / Hosting
- غير محدد بعد — خيارات:
  - Windows Server داخلي في الشركة
  - Azure App Service (سحابي)
  - Docker على خادم Linux

## 12. Technical or Security Constraints
- نظام صلاحيات صارم (Role-based Access Control)
- منع الموظف من رؤية طلبات الموظفين الآخرين
- المدير يرى فقط طلبات فريقهم
- الإدارة العليا ترى كل شيء للتقارير
- Admin لديه صلاحية كاملة
- لا يمكن حذف الطلبات بعد التقديم (يمكن إلغاؤها فقط)
- سجل تدقيق (Audit Log) لجميع العمليات الحساسة

## 13. Technology Profile Candidate
- **Profile ID**: `dotnet-blazor-ef` (موجود وجاهز في النظام)
- ينطبق إذا تم اعتماد Blazor + EF Core + SQL Server

## 14. Open Questions
- Blazor Server أم Blazor WebAssembly أم MVC؟ — سيقدم Tera توصية.
- هل نستخدم MudBlazor أم مكتبة UI أخرى؟
- هل البيئة الداخلية تدعم HTTPS؟
- هل يوجد Active Directory / Entra ID للتكامل مع حسابات الشركة الحالية؟
- هل يجب دعم خاصية "توقيع إلكتروني" على الموافقات؟
