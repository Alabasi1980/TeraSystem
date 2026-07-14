# TASK-COD-028 — Fix Builder Styles Section Mismatch (HTTP 500)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-028 |
| **المجموعة** | FIX — Card Builder follow-up |
| **المرحلة** | Phase 6 |
| **الوكيل** | engineering-agent |
| **التبعية** | TASK-COD-027 |
| **الأولوية** | Critical |
| **الحالة** | 🟡 Assigned |

---

## 1. السبب
`Builder.cshtml` يعرّف `@section Styles { ... }` لكن `_CardsLayout.cshtml` لا يعرّف `@RenderSectionAsync("Styles")` → Razor يرمي InvalidOperationException عند العرض → HTTP 500.

## 2. الإصلاح
إضافة سطر واحد في `_CardsLayout.cshtml` بعد `</style>` (السطر 307) وقبل `</head>` (السطر 308):
```
@await RenderSectionAsync("Styles", required: false)
```

## 3. Allowed Write Targets
```
D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\_CardsLayout.cshtml
```

## 4. المعايير
- `dotnet build` = 0 errors
- Builder page renders without 500 for authenticated user
