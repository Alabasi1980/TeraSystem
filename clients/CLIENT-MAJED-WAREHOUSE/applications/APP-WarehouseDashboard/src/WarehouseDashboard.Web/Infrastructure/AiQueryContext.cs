using System.Text;
using Microsoft.Data.SqlClient;
using WarehouseDashboard.Web.Models.Dto;

namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Prepares AI context by gathering database schema info, formatting conversation
/// history, building system prompts, and executing explorer queries for the AI assistant.
/// </summary>
public class AiQueryContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiQueryContext> _logger;

    public AiQueryContext(IConfiguration configuration, ILogger<AiQueryContext> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // ======================================================================
    // 1. GetSchemaSummaryAsync
    // ======================================================================

    /// <summary>
    /// Fetches database schema summary: tables, columns, and foreign keys.
    /// SQL Server only. Returns empty string on failure (does not throw).
    /// Limited to 30 tables max with a 10-second timeout.
    /// </summary>
    public async Task<string> GetSchemaSummaryAsync(string? source, CancellationToken ct)
    {
        var resolved = string.IsNullOrWhiteSpace(source) ? "SqlServer" : source;
        if (resolved != "SqlServer")
            return string.Empty;

        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("GetSchemaSummaryAsync: No SQL connection string available.");
            return string.Empty;
        }

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(ct);

            var sb = new StringBuilder();
            sb.AppendLine("Tables:");

            // Fetch tables (max 30)
            const string tablesSql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";
            await using var tablesCommand = new SqlCommand(tablesSql, connection);
            tablesCommand.CommandTimeout = 10;

            var tableNames = new List<string>();
            var tableSchemaMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            await using (var reader = await tablesCommand.ExecuteReaderAsync(ct))
            {
                int count = 0;
                while (await reader.ReadAsync(ct) && count < 30)
                {
                    var schema = reader.GetString(0);
                    var name = reader.GetString(1);
                    var qualified = $"[{schema}].[{name}]";
                    tableNames.Add(name);
                    tableSchemaMap[name] = schema;
                    count++;
                }
            }

            if (tableNames.Count == 0)
            {
                sb.AppendLine("  (no tables found)");
            }

            // Fetch columns for each table
            foreach (var tableName in tableNames)
            {
                var schema = tableSchemaMap[tableName];
                var columnSb = new StringBuilder();
                columnSb.Append($"  {tableName} (");

                const string columnsSql = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND TABLE_SCHEMA = @schema ORDER BY ORDINAL_POSITION";
                await using var columnsCommand = new SqlCommand(columnsSql, connection);
                columnsCommand.Parameters.AddWithValue("@table", tableName);
                columnsCommand.Parameters.AddWithValue("@schema", schema);
                columnsCommand.CommandTimeout = 10;

                var colParts = new List<string>();
                await using (var colReader = await columnsCommand.ExecuteReaderAsync(ct))
                {
                    while (await colReader.ReadAsync(ct))
                    {
                        colParts.Add($"{colReader.GetString(0)}: {colReader.GetString(1)}");
                    }
                }

                columnSb.Append(string.Join(", ", colParts));
                columnSb.AppendLine(")");
                sb.Append(columnSb.ToString());
            }

            // Check if there are more tables (beyond 30)
            await using var countCommand = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
            countCommand.CommandTimeout = 10;
            var totalTables = (int)await countCommand.ExecuteScalarAsync(ct);
            if (totalTables > 30)
            {
                sb.AppendLine($"  (... and {totalTables - 30} more tables)");
            }

            // Fetch foreign keys
            sb.AppendLine();
            sb.AppendLine("Foreign Keys:");

            const string fkSql = @"
SELECT 
    CU.TABLE_NAME AS SourceTable,
    CU.COLUMN_NAME AS SourceColumn,
    CU2.TABLE_NAME AS ReferencedTable,
    CU2.COLUMN_NAME AS ReferencedColumn
FROM 
    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU 
        ON RC.CONSTRAINT_CATALOG = CU.CONSTRAINT_CATALOG 
        AND RC.CONSTRAINT_SCHEMA = CU.CONSTRAINT_SCHEMA 
        AND RC.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU2 
        ON RC.UNIQUE_CONSTRAINT_CATALOG = CU2.CONSTRAINT_CATALOG 
        AND RC.UNIQUE_CONSTRAINT_SCHEMA = CU2.CONSTRAINT_SCHEMA 
        AND RC.UNIQUE_CONSTRAINT_NAME = CU2.CONSTRAINT_NAME
ORDER BY CU.TABLE_NAME, CU.COLUMN_NAME";

            await using var fkCommand = new SqlCommand(fkSql, connection);
            fkCommand.CommandTimeout = 10;

            var fkCount = 0;
            await using (var fkReader = await fkCommand.ExecuteReaderAsync(ct))
            {
                while (await fkReader.ReadAsync(ct))
                {
                    var sourceTable = fkReader.GetString(0);
                    var sourceColumn = fkReader.GetString(1);
                    var refTable = fkReader.GetString(2);
                    var refColumn = fkReader.GetString(3);
                    sb.AppendLine($"  {sourceTable}.{sourceColumn} → {refTable}.{refColumn}");
                    fkCount++;
                }
            }

            if (fkCount == 0)
            {
                sb.AppendLine("  (no foreign keys found)");
            }

            return sb.ToString();
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException or TaskCanceledException)
        {
            _logger.LogError(ex, "GetSchemaSummaryAsync: Failed to retrieve schema summary.");
            return string.Empty;
        }
    }

    // ======================================================================
    // 2. GetTableDetailsAsync
    // ======================================================================

    /// <summary>
    /// Fetches detailed info about a specific table: columns, foreign keys,
    /// and first 3 sample rows. Returns formatted text. 10-second timeout.
    /// </summary>
    public async Task<string> GetTableDetailsAsync(string tableName, string? source, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return string.Empty;

        var resolved = string.IsNullOrWhiteSpace(source) ? "SqlServer" : source;
        if (resolved != "SqlServer")
            return string.Empty;

        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("GetTableDetailsAsync: No SQL connection string available.");
            return string.Empty;
        }

        // Sanitize table name for bracket-quoted identifier safety
        var safeTableName = tableName.Replace("]", "]]", StringComparison.Ordinal);

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(ct);
            var sb = new StringBuilder();

            sb.AppendLine($"Table: {tableName}");
            sb.AppendLine(new string('-', 60));

            // Fetch columns
            sb.AppendLine("Columns:");
            const string columnsSql = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table ORDER BY ORDINAL_POSITION";
            await using var columnsCommand = new SqlCommand(columnsSql, connection);
            columnsCommand.Parameters.AddWithValue("@table", tableName);
            columnsCommand.CommandTimeout = 10;

            await using (var colReader = await columnsCommand.ExecuteReaderAsync(ct))
            {
                var colCount = 0;
                while (await colReader.ReadAsync(ct))
                {
                    var colName = colReader.GetString(0);
                    var dataType = colReader.GetString(1);
                    var nullable = colReader.GetString(2);
                    sb.AppendLine($"  {colName} ({dataType}, {(nullable == "YES" ? "nullable" : "not null")})");
                    colCount++;
                }
                if (colCount == 0)
                    sb.AppendLine("  (no columns found)");
            }

            // Fetch foreign keys (both directions)
            sb.AppendLine();
            sb.AppendLine("Foreign Keys:");

            // Outgoing FKs (this table references others)
            const string fkOutSql = @"
SELECT 
    CU.COLUMN_NAME AS SourceColumn,
    CU2.TABLE_NAME AS ReferencedTable,
    CU2.COLUMN_NAME AS ReferencedColumn
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU 
    ON RC.CONSTRAINT_CATALOG = CU.CONSTRAINT_CATALOG 
    AND RC.CONSTRAINT_SCHEMA = CU.CONSTRAINT_SCHEMA 
    AND RC.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU2 
    ON RC.UNIQUE_CONSTRAINT_CATALOG = CU2.CONSTRAINT_CATALOG 
    AND RC.UNIQUE_CONSTRAINT_SCHEMA = CU2.CONSTRAINT_SCHEMA 
    AND RC.UNIQUE_CONSTRAINT_NAME = CU2.CONSTRAINT_NAME
WHERE CU.TABLE_NAME = @table";

            await using var fkOutCommand = new SqlCommand(fkOutSql, connection);
            fkOutCommand.Parameters.AddWithValue("@table", tableName);
            fkOutCommand.CommandTimeout = 10;

            var fkOutCount = 0;
            await using (var fkReader = await fkOutCommand.ExecuteReaderAsync(ct))
            {
                while (await fkReader.ReadAsync(ct))
                {
                    var srcCol = fkReader.GetString(0);
                    var refTable = fkReader.GetString(1);
                    var refCol = fkReader.GetString(2);
                    sb.AppendLine($"  {tableName}.{srcCol} → {refTable}.{refCol}");
                    fkOutCount++;
                }
            }

            // Incoming FKs (other tables reference this table)
            const string fkInSql = @"
SELECT 
    CU.TABLE_NAME AS SourceTable,
    CU.COLUMN_NAME AS SourceColumn,
    CU2.COLUMN_NAME AS ReferencedColumn
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU 
    ON RC.CONSTRAINT_CATALOG = CU.CONSTRAINT_CATALOG 
    AND RC.CONSTRAINT_SCHEMA = CU.CONSTRAINT_SCHEMA 
    AND RC.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU2 
    ON RC.UNIQUE_CONSTRAINT_CATALOG = CU2.CONSTRAINT_CATALOG 
    AND RC.UNIQUE_CONSTRAINT_SCHEMA = CU2.CONSTRAINT_SCHEMA 
    AND RC.UNIQUE_CONSTRAINT_NAME = CU2.CONSTRAINT_NAME
WHERE CU2.TABLE_NAME = @table";

            await using var fkInCommand = new SqlCommand(fkInSql, connection);
            fkInCommand.Parameters.AddWithValue("@table", tableName);
            fkInCommand.CommandTimeout = 10;

            var fkInCount = 0;
            await using (var fkReader = await fkInCommand.ExecuteReaderAsync(ct))
            {
                while (await fkReader.ReadAsync(ct))
                {
                    var srcTable = fkReader.GetString(0);
                    var srcCol = fkReader.GetString(1);
                    var refCol = fkReader.GetString(2);
                    sb.AppendLine($"  {srcTable}.{srcCol} → {tableName}.{refCol}");
                    fkInCount++;
                }
            }

            if (fkOutCount == 0 && fkInCount == 0)
                sb.AppendLine("  (no foreign keys)");

            // Fetch first 3 sample rows
            sb.AppendLine();
            sb.AppendLine("Sample Data (first 3 rows):");
            var sampleSql = $"SELECT TOP 3 * FROM [{safeTableName}]";

            await using var sampleCommand = new SqlCommand(sampleSql, connection);
            sampleCommand.CommandTimeout = 10;

            await using (var sampleReader = await sampleCommand.ExecuteReaderAsync(ct))
            {
                var fieldCount = sampleReader.FieldCount;
                var fieldNames = new string[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                    fieldNames[i] = sampleReader.GetName(i);

                sb.AppendLine("  Columns: " + string.Join(", ", fieldNames));

                int rowNum = 0;
                while (await sampleReader.ReadAsync(ct) && rowNum < 3)
                {
                    var values = new List<string>();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        var val = sampleReader.IsDBNull(i) ? "NULL" : sampleReader.GetValue(i)?.ToString() ?? "NULL";
                        if (val.Length > 100)
                            val = val[..100] + "...";
                        values.Add(val);
                    }
                    sb.AppendLine($"  Row {rowNum + 1}: {string.Join(", ", values)}");
                    rowNum++;
                }

                if (rowNum == 0)
                    sb.AppendLine("  (no data)");
            }

            return sb.ToString();
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException or TaskCanceledException)
        {
            _logger.LogError(ex, "GetTableDetailsAsync: Failed to get details for table {TableName}.", tableName);
            return string.Empty;
        }
    }

    // ======================================================================
    // 3. BuildSystemPromptAsync
    // ======================================================================

    /// <summary>
    /// Builds a comprehensive system prompt including the current database schema
    /// and the optional current SQL query in the editor.
    /// </summary>
    public async Task<string> BuildSystemPromptAsync(string? currentSql, string? source, CancellationToken ct)
    {
        var schemaSummary = await GetSchemaSummaryAsync(source, ct);

        var sb = new StringBuilder();
        sb.AppendLine("أنت مساعد متخصص في SQL و Views Databases.");
        sb.AppendLine("أنت جزء من تطبيق WarehouseDashboard — لوحة تحكم لإدارة المستودعات.");
        sb.AppendLine();
        sb.AppendLine("[هويتك]");
        sb.AppendLine("- اسمك: 🤖 مساعد الاستعلامات");
        sb.AppendLine("- تخصصك: إنشاء وتحسين وتعديل استعلامات SQL للـ Views");
        sb.AppendLine("- تتحدث العربية الفصحى البسيطة");
        sb.AppendLine();
        sb.AppendLine("[قاعدة البيانات الحالية]");

        if (string.IsNullOrEmpty(schemaSummary))
        {
            sb.AppendLine("(تعذر الاتصال بقاعدة البيانات أو لا توجد جداول)");
        }
        else
        {
            sb.Append(schemaSummary);
            if (!schemaSummary.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                sb.AppendLine();
        }

        sb.AppendLine();
        sb.AppendLine("[قواعد عملك]");
        sb.AppendLine("1. ادرس Schema أولاً قبل اقتراح أي كويري");
        sb.AppendLine("2. ضع الكويري الكامل في ```sql ... ```");
        sb.AppendLine("3. اشرح باختصار ماذا يفعل كل جزء");
        sb.AppendLine("4. عند تعديل كويري موجود، اقرأه أولاً ولا تعيد كتابة الكل إلا لزم");
        sb.AppendLine("5. صلاحياتك: SELECT فقط (حد 100 صف)، INFORMATION_SCHEMA، تعديل SQL في المحرر");
        sb.AppendLine("6. ممنوع: DELETE, UPDATE, INSERT, DROP, ALTER, CREATE");
        sb.AppendLine("7. أنت مساعد — المستخدم هو القائد");

        if (!string.IsNullOrEmpty(currentSql))
        {
            sb.AppendLine();
            sb.AppendLine("[الاستعلام الحالي في المحرر]");
            sb.AppendLine("```sql");
            sb.AppendLine(currentSql);
            sb.AppendLine("```");
        }

        return sb.ToString();
    }

    // ======================================================================
    // 4. FormatConversationHistoryAsync
    // ======================================================================

    /// <summary>
    /// Converts saved conversation messages to OpenAI-format message objects.
    /// Truncates message content to 2000 characters max with "..." suffix.
    /// </summary>
    public Task<List<object>> FormatConversationHistoryAsync(List<ConversationMessage> messages, CancellationToken ct)
    {
        var result = new List<object>();

        if (messages is null || messages.Count == 0)
            return Task.FromResult(result);

        foreach (var msg in messages)
        {
            var content = msg.Message ?? string.Empty;
            if (content.Length > 2000)
                content = content[..2000] + "...";

            result.Add(new
            {
                role = msg.Role,
                content
            });
        }

        return Task.FromResult(result);
    }

    // ======================================================================
    // 5. ExecuteExplorerQueryAsync
    // ======================================================================

    /// <summary>
    /// Executes a read-only SQL query for AI exploration purposes.
    /// Validates via <see cref="SqlReadonlyGuard"/>, limits to 100 rows,
    /// uses 15-second timeout. Returns results as a formatted string
    /// wrapped in <see cref="AIAssistantResponse"/>.
    /// Does NOT log results to any database.
    /// </summary>
    public async Task<AIAssistantResponse> ExecuteExplorerQueryAsync(string sql, string? source, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            return new AIAssistantResponse
            {
                Success = false,
                Content = string.Empty,
                ErrorMessage = "الرجاء إدخال استعلام SQL."
            };
        }

        // Validate read-only via SqlReadonlyGuard
        if (!SqlReadonlyGuard.IsReadOnly(sql, out var guardReason))
        {
            return new AIAssistantResponse
            {
                Success = false,
                Content = string.Empty,
                ErrorMessage = guardReason
            };
        }

        var resolved = string.IsNullOrWhiteSpace(source) ? "SqlServer" : source;
        if (resolved != "SqlServer")
        {
            return new AIAssistantResponse
            {
                Success = false,
                Content = string.Empty,
                ErrorMessage = $"مصدر البيانات غير معروف: '{resolved}'. يدعم SqlServer فقط حالياً."
            };
        }

        var connectionString = ConnectionStringHelper.ResolveSql(_configuration);
        if (string.IsNullOrEmpty(connectionString))
        {
            return new AIAssistantResponse
            {
                Success = false,
                Content = string.Empty,
                ErrorMessage = "إعدادات الاتصال بقاعدة البيانات غير متوفرة حالياً."
            };
        }

        const int maxRows = 100;

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(ct);

            await using var command = new SqlCommand(sql, connection);
            command.CommandTimeout = 15;

            await using var reader = await command.ExecuteReaderAsync(ct);

            var fieldCount = reader.FieldCount;
            var fieldNames = new string[fieldCount];
            for (int i = 0; i < fieldCount; i++)
                fieldNames[i] = reader.GetName(i);

            var sb = new StringBuilder();
            sb.AppendLine("[");
            int rowCount = 0;

            while (await reader.ReadAsync(ct) && rowCount < maxRows)
            {
                if (rowCount > 0)
                    sb.AppendLine(",");

                sb.Append("  { ");
                var vals = new List<string>();
                for (int i = 0; i < fieldCount; i++)
                {
                    var val = reader.IsDBNull(i) ? "null" : FormatValueForJson(reader.GetValue(i));
                    vals.Add($"\"{EscapeJson(fieldNames[i])}\": {val}");
                }
                sb.Append(string.Join(", ", vals));
                sb.Append(" }");

                rowCount++;
            }

            sb.AppendLine();
            sb.AppendLine("]");

            if (rowCount == 0)
            {
                sb = new StringBuilder("[]");
            }

            var content = sb.ToString();

            return new AIAssistantResponse
            {
                Success = true,
                Content = content,
                ErrorMessage = null
            };
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "ExecuteExplorerQueryAsync: SQL error executing explorer query.");
            return new AIAssistantResponse
            {
                Success = false,
                Content = string.Empty,
                ErrorMessage = "تعذر تنفيذ الاستعلام. تأكد من صحة الصياغة وأن الجداول والحقول موجودة."
            };
        }
        catch (Exception ex) when (ex is InvalidOperationException or TaskCanceledException)
        {
            _logger.LogError(ex, "ExecuteExplorerQueryAsync: Error executing explorer query.");
            return new AIAssistantResponse
            {
                Success = false,
                Content = string.Empty,
                ErrorMessage = "حدث خطأ أثناء تنفيذ الاستعلام."
            };
        }
    }

    // ======================================================================
    // Helpers
    // ======================================================================

    /// <summary>Formats a value for JSON-like output in explorer results.</summary>
    private static string FormatValueForJson(object value)
    {
        if (value is DBNull)
            return "null";

        if (value is string str)
            return $"\"{EscapeJson(str)}\"";

        if (value is DateTime dt)
            return $"\"{dt:yyyy-MM-dd HH:mm:ss}\"";

        if (value is DateTimeOffset dto)
            return $"\"{dto:yyyy-MM-dd HH:mm:ss}\"";

        if (value is byte[])
            return "\"<binary>\"";

        if (value is bool b)
            return b ? "true" : "false";

        if (value is sbyte or byte or short or ushort or int or uint or long or ulong
            or float or double or decimal)
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "null";

        return $"\"{EscapeJson(value.ToString() ?? string.Empty)}\"";
    }

    /// <summary>Escapes a string for embedding in a JSON string value.</summary>
    private static string EscapeJson(string text)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }
}
