# TASK-COD-KPI-001 — Rename OracleTable → SqlTable in Card Builder

## Status: DONE

## 8. Handback

### Status
**DONE** ✅

### Files Changed

| File | Lines | Changes |
|------|-------|---------|
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml` | 483 | 5 edits: option value/label, panel data-source/id, label for/text, select id/aria-describedby, hint id/text |
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs` | 532 | 8 edits: comment, property name, template category Id/Name/Description, 2× method call rename, method name, 2× property assignment |
| `src/WarehouseDashboard.Web/wwwroot/js/comment-builder.js` | 961 | 7 bulk replaceAll edits: `'OracleTable'`→`'SqlTable'`, `wb-oracle-table-hint`→`wb-sql-table-hint`, `wb-oracle-table`→`wb-sql-table`, `populateOracleTableSelect`→`populateSqlTableSelect`, `applyOracleTable`→`applySqlTable`, 2× comment |

**Total:** 3 files modified, ~25 individual replacements

### Build Result
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:02.97
```

### Issues / Gaps
- None observed. All replacements are consistent across HTML (cshtml), C# (cshtml.cs), and JS.
- Arabic text updated where Oracle was mentioned (labels, descriptions, hints).
- Arabic text preserved where no Oracle mention existed (task instructions items 8-9).
- `OracleTables` property in `Builder.cshtml.cs` renamed to `SqlTables` — JS references to the DOM element `wb-oracle-table` also updated to `wb-sql-table`.
- The `.cshtml` comment `<!-- Oracle Tables -->` on line 134 was intentionally left as-is (not in the task's replacement list).

### Secrets Check
No secrets, credentials, or sensitive data were written. All changes are text/UI renames only.
