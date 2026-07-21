using System.Data;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Infrastructure;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// Generates a formatted Excel (.xlsx) file from any SQL Server table.
/// Used by <see cref="Controllers.SyncController.ExportExcel"/> to let users
/// download sync-table data as a polished spreadsheet.
/// </summary>
public class ExportExcelService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExportExcelService> _logger;

    public ExportExcelService(IConfiguration configuration, ILogger<ExportExcelService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Generates a formatted Excel file for the given SQL Server table.
    /// </summary>
    /// <param name="tableName">SQL Server table name (e.g. stg_ST_ITEM_CARD).</param>
    /// <param name="mappingName">Display name for the mapping (used in footer).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Excel file bytes, or null if table not found.</returns>
    public async Task<byte[]?> GenerateAsync(string tableName, string mappingName, CancellationToken ct)
    {
        var connStr = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrWhiteSpace(connStr))
        {
            _logger.LogWarning("SQL connection string is empty — cannot export table {TableName}.", tableName);
            return null;
        }

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM [{tableName}] ORDER BY 1";

            await using var reader = await cmd.ExecuteReaderAsync(ct);

            // Extract column metadata from the reader's schema table.
            var schemaTable = await reader.GetSchemaTableAsync(ct);
            if (schemaTable is null || schemaTable.Rows.Count == 0)
            {
                _logger.LogWarning("Could not retrieve schema for table {TableName}.", tableName);
                return null;
            }

            var columns = new List<ColumnInfo>();
            foreach (DataRow row in schemaTable.Rows)
            {
                var rawName = row["ColumnName"]?.ToString() ?? "Column";
                var colName = rawName.Length > 50 ? rawName[..50] : rawName;
                var dataType = (Type)row["DataType"];
                columns.Add(new ColumnInfo(colName, dataType));
            }

            // Build the Excel workbook.
            using var workbook = new XLWorkbook();
            var sheetName = tableName.Length > 31 ? tableName[..31] : tableName;
            var ws = workbook.Worksheets.Add(sheetName);

            // RTL support for Arabic content.
            ws.RightToLeft = true;

            // ── Header row (row 1) ──────────────────────────────────────────
            for (int i = 0; i < columns.Count; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = columns[i].Name;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(31, 78, 121);
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 11;
            }

            // ── Data rows ───────────────────────────────────────────────────
            int rowNum = 2;
            long totalRows = 0;
            bool hasRows = false;

            while (await reader.ReadAsync(ct))
            {
                ct.ThrowIfCancellationRequested();
                hasRows = true;
                totalRows++;
                var isEvenRow = rowNum % 2 == 0;

                for (int i = 0; i < columns.Count; i++)
                {
                    var cell = ws.Cell(rowNum, i + 1);
                    SetCellValue(cell, reader, i, columns[i].DataType);

                    // Alternating row colours.
                    cell.Style.Fill.BackgroundColor = isEvenRow
                        ? XLColor.White
                        : XLColor.FromArgb(245, 248, 250);
                }

                rowNum++;
            }

            // ── Empty-table message ─────────────────────────────────────────
            if (!hasRows)
            {
                ws.Cell(rowNum, 1).Value = "لا توجد بيانات";
                ws.Cell(rowNum, 1).Style.Font.Italic = true;
                ws.Cell(rowNum, 1).Style.Font.FontColor = XLColor.Gray;
                rowNum++;
            }

            // ── AutoFilter (header row only) ────────────────────────────────
            if (columns.Count > 0)
            {
                var lastDataRow = hasRows ? rowNum - 1 : 1;
                var range = ws.Range(1, 1, lastDataRow, columns.Count);
                range.SetAutoFilter();
            }

            // ── Freeze first row ────────────────────────────────────────────
            ws.SheetView.Freeze(1, 0);

            // ── Auto-fit column widths (limit to first 100 rows for perf) ──
            if (columns.Count > 0)
            {
                var maxRow = Math.Min(rowNum - 1, 100);
                if (maxRow >= 1)
                {
                    ws.Columns().AdjustToContents(1, maxRow);
                }
            }

            // ── Footer ──────────────────────────────────────────────────────
            int footerRow = rowNum;
            int midCol = Math.Max(columns.Count / 2, 1);

            // Right-side footer (visually left in RTL): المصدر
            if (midCol <= columns.Count)
            {
                var leftRange = ws.Range(footerRow, midCol, footerRow, columns.Count);
                leftRange.Merge();
                var leftCell = leftRange.FirstCell();
                leftCell.Value = $"المصدر: {mappingName}";
                leftCell.Style.Font.Italic = true;
                leftCell.Style.Font.FontColor = XLColor.Gray;
            }

            // Left-side footer (visually right in RTL): تاريخ التصدير
            var rightRange = ws.Range(footerRow, 1, footerRow, midCol);
            rightRange.Merge();
            var rightCell = rightRange.FirstCell();
            rightCell.Value = $"تاريخ التصدير: {DateTime.Now:yyyy-MM-dd HH:mm}";
            rightCell.Style.Font.Italic = true;
            rightCell.Style.Font.FontColor = XLColor.Gray;

            // ── Save to memory stream ───────────────────────────────────────
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            // Object not found — table does not exist.
            _logger.LogWarning(ex, "Table {TableName} does not exist in the database.", tableName);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export table {TableName} to Excel.", tableName);
            return null;
        }
    }

    /// <summary>
    /// Writes a cell value from an active <see cref="SqlDataReader"/>, applying
    /// appropriate number and date formatting based on the column's data type.
    /// </summary>
    private static void SetCellValue(IXLCell cell, SqlDataReader reader, int ordinal, Type dataType)
    {
        if (reader.IsDBNull(ordinal))
        {
            cell.Value = Blank.Value;
            return;
        }

        if (dataType == typeof(DateTime) || dataType == typeof(DateTimeOffset))
        {
            cell.Value = reader.GetDateTime(ordinal);
            cell.Style.NumberFormat.Format = "YYYY-MM-DD HH:MM";
        }
        else if (dataType == typeof(int) || dataType == typeof(long) ||
                 dataType == typeof(short) || dataType == typeof(byte))
        {
            cell.Value = Convert.ToDouble(reader.GetValue(ordinal));
            cell.Style.NumberFormat.Format = "#,##0";
        }
        else if (dataType == typeof(decimal) || dataType == typeof(float) ||
                 dataType == typeof(double))
        {
            cell.Value = Convert.ToDouble(reader.GetValue(ordinal));
            cell.Style.NumberFormat.Format = "#,##0.00";
        }
        else if (dataType == typeof(bool))
        {
            cell.Value = reader.GetBoolean(ordinal);
        }
        else if (dataType == typeof(Guid))
        {
            cell.Value = reader.GetGuid(ordinal).ToString();
        }
        else if (dataType == typeof(string))
        {
            var val = reader.GetString(ordinal);
            cell.Value = val;
        }
        else
        {
            // Fallback: convert to string.
            var val = reader.GetValue(ordinal)?.ToString() ?? string.Empty;
            cell.Value = val;
        }
    }

    /// <summary>
    /// Lightweight holder for a column name and its CLR data type.
    /// </summary>
    private readonly record struct ColumnInfo(string Name, Type DataType);
}
