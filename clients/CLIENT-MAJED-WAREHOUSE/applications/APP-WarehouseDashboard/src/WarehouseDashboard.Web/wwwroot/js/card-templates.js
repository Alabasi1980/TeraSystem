/*
 * ==========================================================================
 *  card-templates.js
 *  Global data for the Card Builder wizard.
 *  Plain (non-module) script — exposes window.CardBuilderTemplates and
 *  window.CardBuilderPalettes so card-builder.js can consume them.
 *
 *  The 6 templates below EXACTLY mirror Services/CardBuilderService.cs
 *  (_templates). The 7 palettes EXACTLY mirror Builder.cshtml.cs ColorPalettes.
 * ==========================================================================
 */
(function (global) {
  'use strict';

  /* ---------- 6 predefined templates (mirror of CardBuilderService._templates) ---------- */
  global.CardBuilderTemplates = [
    {
      id: 'total-stock',
      name: 'إجمالي المخزون',
      description: 'بطاقة KPI تعرض إجمالي كمية المخزون',
      chartType: 'KPI',
      dataSourceType: 'SQL Query',
      sqlQueryTemplate: 'SELECT COUNT(*) AS TotalItems FROM [{TableName}]',
      defaultGridWidth: 3,
      defaultGridHeight: 2,
      defaultRefreshInterval: 300,
      requiredOracleSource: 'WarehouseStock'
    },
    {
      id: 'sales-trend',
      name: 'اتجاه المبيعات',
      description: 'رسم بياني عمودي للمبيعات الشهرية',
      chartType: 'Bar',
      dataSourceType: 'SQL Query',
      sqlQueryTemplate: "SELECT FORMAT(SaleDate, 'yyyy-MM') AS Month, SUM(Amount) AS TotalSales FROM [{TableName}] GROUP BY FORMAT(SaleDate, 'yyyy-MM') ORDER BY Month",
      defaultGridWidth: 6,
      defaultGridHeight: 4,
      defaultRefreshInterval: 3600,
      requiredOracleSource: 'Sales'
    },
    {
      id: 'items-distribution',
      name: 'توزيع الأصناف',
      description: 'رسم بياني دائري لتوزيع الأصناف حسب الفئة',
      chartType: 'Pie',
      dataSourceType: 'SQL Query',
      sqlQueryTemplate: 'SELECT Category, COUNT(*) AS ItemCount FROM [{TableName}] GROUP BY Category ORDER BY ItemCount DESC',
      defaultGridWidth: 4,
      defaultGridHeight: 4,
      defaultRefreshInterval: 3600,
      requiredOracleSource: 'Items'
    },
    {
      id: 'stock-movement',
      name: 'حركة المخزون',
      description: 'جدول بأحدث حركات المخزون',
      chartType: 'Table',
      dataSourceType: 'SQL Query',
      sqlQueryTemplate: 'SELECT TOP 50 MovementDate, ItemCode, MovementType, Quantity, ReferenceNo FROM [{TableName}] ORDER BY MovementDate DESC',
      defaultGridWidth: 12,
      defaultGridHeight: 6,
      defaultRefreshInterval: 60,
      requiredOracleSource: 'StockMovement'
    },
    {
      id: 'low-stock-alert',
      name: 'تنبيه مخزون منخفض',
      description: 'بطاقة KPI لعدد الأصناف التي وصلت للحد الأدنى',
      chartType: 'KPI',
      dataSourceType: 'SQL Query',
      sqlQueryTemplate: 'SELECT COUNT(*) AS LowStockCount FROM [{TableName}] WHERE Quantity <= MinQuantity AND MinQuantity > 0',
      defaultGridWidth: 3,
      defaultGridHeight: 2,
      defaultRefreshInterval: 300,
      requiredOracleSource: 'WarehouseStock'
    },
    {
      id: 'top-selling-items',
      name: 'الأكثر مبيعاً',
      description: 'رسم بياني عمودي لأعلى 10 أصناف مبيعاً',
      chartType: 'Bar',
      dataSourceType: 'SQL Query',
      sqlQueryTemplate: 'SELECT TOP 10 ItemCode, SUM(Quantity) AS TotalQty FROM [{TableName}] GROUP BY ItemCode ORDER BY TotalQty DESC',
      defaultGridWidth: 6,
      defaultGridHeight: 4,
      defaultRefreshInterval: 3600,
      requiredOracleSource: 'Sales'
    }
  ];

  /* ---------- 7 color palettes (mirror of Builder.cshtml.cs ColorPalettes) ---------- */
  global.CardBuilderPalettes = [
    { id: 'primary',  name: 'الرئيسي',   colors: ['#1F4E79', '#2E6DA4', '#8FBCDE'] },
    { id: 'secondary', name: 'الثانوي',  colors: ['#2E6DA4', '#1F4E79', '#8FBCDE'] },
    { id: 'accent',   name: 'التمييز',   colors: ['#0A2540', '#1F4E79', '#2E6DA4'] },
    { id: 'success',  name: 'النجاح',    colors: ['#1E9E6A', '#28A745', '#4CD97B'] },
    { id: 'warning',  name: 'التحذير',   colors: ['#E0A106', '#FFC107', '#FFD54F'] },
    { id: 'info',     name: 'المعلومات', colors: ['#2E6DA4', '#17A2B8', '#4FC3F7'] },
    { id: 'custom',   name: 'مخصص',      colors: ['#1F4E79', '#E0A106', '#1E9E6A', '#D64545', '#2E6DA4', '#8FBCDE'] }
  ];

})(window);
