# TASK-COD-021 — UAT Documentation + Deployment Testing Checklist

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-021 |
| **المجموعة** | B7 — Deployment |
| **المرحلة** | Phase F — Deployment |
| **الوكيل** | engineering-agent |
| **التقدير** | 6–10h |
| **التبعية** | ALL (B1-B6 + B8 ✅) |
| **الأولوية** | High |
| **الحالة** | 🟡 Assigned |

---

## 1. الهدف

إنشاء دليل UAT شامل يحتوي على كل سيناريوهات الاختبار المطلوبة للتحقق من أن التطبيق يعمل بشكل كامل بعد النشر على IIS.

## 2. المخرج المطلوب

ملف `docs/UAT_TEST_PLAN.md` يحتوي على:

### 2.1 بيئات الاختبار
- IIS Local على السيرفر (10.10.1.1)
- Oracle: SID NATEJSOFT
- SQL Server: Database WarehouseDashboard

### 2.2 سيناريوهات الاختبار

#### A. الاتصالات (Connections)
| # | الاختبار | الخطوات | النتيجة المتوقعة |
|---|---|---|---|
| A1 | Oracle Connection | POST /api/sync/test-oracle | 200 OK + info |
| A2 | SQL Server Connection | POST /api/sync/test-sql | 200 OK + info |
| A3 | Oracle Read Data | GET /api/sync/oracle-data | DataTable JSON |

#### B. المزامنة (Sync)
| # | الاختبار | الخطوات | النتيجة المتوقعة |
|---|---|---|---|
| B1 | Manual Sync Trigger | POST /api/sync/trigger | 200 + "cycle started" |
| B2 | Sync Status Check | GET /api/sync/status | IsRunning + LastStatus |
| B3 | Auto-Sync (enable) | Update SyncSettings via Admin | Auto-sync starts |
| B4 | Sync Logs | GET /api/sync/logs | Array of sync runs |
| B5 | Full Refresh Cycle | Wait for sync completion | Data in SQL Server tables |

#### C. Admin Panel
| # | الاختبار | الخطوات | النتيجة المتوقعة |
|---|---|---|---|
| C1 | Admin Login | POST /admin-secure-panel/Login | Session cookie set |
| C2 | Admin Logout | GET /admin-secure-panel/Logout | Session cleared |
| C3 | Wrong Password | POST login with wrong password | Error message |
| C4 | Protected Routes | GET /admin-secure-panel without login | Redirect to Login |
| C5 | Card CRUD - List | GET /admin-secure-panel/Cards | List of cards |
| C6 | Card CRUD - Create | POST create card | Card created |
| C7 | Card CRUD - Edit | POST edit card | Card updated |
| C8 | Card CRUD - Delete | POST delete card | Card deleted |
| C9 | Query Tester | GET /admin-secure-panel/QueryTester | Page loads |
| C10 | Query Tester - Execute | POST execute query | Results displayed |
| C11 | Drill Down Config | GET /admin-secure-panel/DrillDown | Config page loads |
| C12 | Sync Logs Page | GET /admin-secure-panel/SyncLogs | Logs displayed |
| C13 | Sync Settings | GET /admin-secure-panel/SyncSettings | Settings page loads |
| C14 | Sync Settings - Save | POST save settings | Settings saved |

#### D. Dashboard
| # | الاختبار | الخطوات | النتيجة المتوقعة |
|---|---|---|---|
| D1 | Dashboard Loads | GET / | Dashboard with cards |
| D2 | Card Data | Check card values | Data from SQL Server |
| D3 | Drill Down | Click card → drill page | Drill data displayed |
| D4 | Breadcrumb Navigation | Click breadcrumb | Navigate back |
| D5 | Search | Use search box | Filter cards |
| D6 | Sync Status Bar | Check status indicator | Shows last sync time |
| D7 | Manual Refresh | Click refresh button | Data refreshes |
| D8 | Skeleton Loading | Observe loading state | Shimmer animation |
| D9 | Empty State | View empty section | Empty state message |

#### E. Performance & Security
| # | الاختبار | الخطوات | النتيجة المتوقعة |
|---|---|---|---|
| E1 | Page Load Time | Measure dashboard load | < 3 seconds |
| E2 | Sync Duration | Measure full sync | < 60 seconds |
| E3 | No Hardcoded Secrets | Search codebase | No passwords in source |
| E4 | SQL Injection | Test query inputs | Parameterized queries |
| E5 | Session Security | Check cookie flags | HttpOnly, SameSite |

### 2.3 سيناريوهات الخطأ (Error Scenarios)
| # | الاختبار | الخطوات | النتيجة المتوقعة |
|---|---|---|---|
| F1 | Oracle Down | Stop Oracle | Graceful error in sync |
| F2 | SQL Server Down | Stop SQL Server | Error logged, app survives |
| F3 | Network Interruption | Kill network mid-sync | Sync retries, then skips |
| F4 | Invalid Query | Enter bad SQL in QueryTester | Error message displayed |

### 2.4 تسجيل النتائج
- جدول تسجيل لكل اختبار: PASS / FAIL / BLOCKED
- ملاحظات لكل فشل
- توقيع القبول النهائي

## 3. Allowed Write Targets

```
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/docs/
```

## 4. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `UAT_TEST_PLAN.md` موجود وشامل | ⬜ |
| AC-2 | يحتوي على 5+ فئات اختبار | ⬜ |
| AC-3 | يحتوي على 30+ سيناريو اختبار | ⬜ |
| AC-4 | يحتوي على سيناريوهات الخطأ | ⬜ |
| AC-5 | جدول تسجيل النتائج موجود | ⬜ |
| AC-6 | `dotnet build -c Release` = 0 errors | ⬜ |

## 5. ملاحظات

- هذا دليل اختبار فقط — لا ينفّذ اختبارات فعلية
- الاختبار الفعلي يبدأ بعد النشر على IIS
- المستخدم (الماجد) هو من ينفّذ اختبارات UAT بناءً على هذا الدليل
