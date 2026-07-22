// ======================================================================
// ReportService has been refactored into specialized services.
// See TASK-FIX-REFACTOR-001.
//
// - ReportViewService:      View discovery, schema introspection
// - ReportCrudService:      CRUD operations (via EF Core)
// - ReportExecutionService: Dynamic query execution, filter/parameter options
// - ReportLayoutService:    Layout management
//
// DTOs moved to WarehouseDashboard.Web.Models.Dto.ReportDtos
// ======================================================================
