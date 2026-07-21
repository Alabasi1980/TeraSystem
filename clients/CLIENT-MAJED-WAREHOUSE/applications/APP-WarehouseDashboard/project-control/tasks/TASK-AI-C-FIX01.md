# TASK-AI-C-FIX01 — Register Missing Summary Builders in DI

> **Status:** Draft → Approved  
> **Batch:** AI-C-FIX (Quick fix)  
> **Depends On:** TASK-AI-C02, C03, C04, C05  
> **Phase:** AI Assistant — Phase C (Fix)  

---

## 1. Objective

Add the missing DI registrations for `TableSummaryBuilder` and `GenericSummaryBuilder` in `Program.cs`. These builders were created by C04/C05 but not registered because the task specs said "No Program.cs changes." The `CardSummaryBuilderFactory` needs all 4 builders registered to resolve by `ChartType`.

---

## 2. Allowed Write Targets

1. `ClientAppPath\src\WarehouseDashboard.Web\Program.cs` — add 2 lines only

---

## 3. Technical Spec

After line 64 (`builder.Services.AddScoped<ICardSummaryBuilder, ChartSummaryBuilder>();`), add:

```csharp
builder.Services.AddScoped<ICardSummaryBuilder, TableSummaryBuilder>();
builder.Services.AddScoped<ICardSummaryBuilder, GenericSummaryBuilder>();
```

Lines 63-66 should become:
```csharp
builder.Services.AddScoped<ICardSummaryBuilder, KpiSummaryBuilder>();
builder.Services.AddScoped<ICardSummaryBuilder, ChartSummaryBuilder>();
builder.Services.AddScoped<ICardSummaryBuilder, TableSummaryBuilder>();
builder.Services.AddScoped<ICardSummaryBuilder, GenericSummaryBuilder>();
builder.Services.AddScoped<CardSummaryBuilderFactory>();
```

---

## 4. Forbidden

- No other changes to Program.cs
- No changes to any builder file

---

## 5. Acceptance

- `dotnet build` passes with 0 errors and 0 warnings
- All 4 builders registered before the Factory

---

## 6. Pre-Execution Gate

```
Gate: PASS | Model: Light (2 lines DI fix) | Security: Low
```
