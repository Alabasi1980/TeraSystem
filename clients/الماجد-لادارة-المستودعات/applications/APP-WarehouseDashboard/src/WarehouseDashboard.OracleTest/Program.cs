using System.Text.Json;
using Oracle.ManagedDataAccess.Client;

const string AppSettingsFile = "appsettings.json";
const string ConnectionStringKey = "OracleConnection";
const string TestQuery = "SELECT SYSDATE FROM DUAL";

try
{
    // ---------------------------------------------------------------
    // 1. Load configuration
    // ---------------------------------------------------------------
    if (!File.Exists(AppSettingsFile))
    {
        WriteError($"Configuration file not found: {AppSettingsFile}");
        WriteInfo($"Ensure '{AppSettingsFile}' exists in the output directory.");
        return;
    }

    var jsonText = await File.ReadAllTextAsync(AppSettingsFile);
    var config = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonText);

    if (config is null || !config.TryGetValue(ConnectionStringKey, out var connectionString))
    {
        WriteError($"'{ConnectionStringKey}' not found or empty in '{AppSettingsFile}'.");
        WriteInfo("Expected format:");
        WriteInfo("  { \"OracleConnection\": \"User Id=...;Password=...;Data Source=...;\" }");
        return;
    }

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        WriteError("Connection string is empty. Provide valid Oracle credentials.");
        return;
    }

    if (connectionString.Contains("YOUR_USER") || connectionString.Contains("YOUR_PASSWORD"))
    {
        WriteWarning("Connection string still contains placeholder values.");
        WriteInfo("Replace YOUR_USER, YOUR_PASSWORD, and Data Source with real Oracle credentials.");
        WriteInfo("Proceeding anyway to test connection parsing...");
    }

    // ---------------------------------------------------------------
    // 2. Attempt connection + query
    // ---------------------------------------------------------------
    WriteHeader("WarehouseDashboard — Oracle Connectivity Test");
    Console.WriteLine($"Target  : Oracle Database (ODP.NET Managed Driver)");
    Console.WriteLine($"Query   : {TestQuery}");
    Console.WriteLine($"Time    : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine();

    await using (var connection = new OracleConnection(connectionString))
    {
        WriteInfo("Opening connection to Oracle...");
        await connection.OpenAsync();
        WriteSuccess("Connection established successfully.");
        Console.WriteLine($"  Server Version : {connection.ServerVersion}");
        Console.WriteLine($"  Database       : {connection.Database}");
        Console.WriteLine($"  Connection ID  : {connection.ConnectionId}");
        Console.WriteLine();

        // -----------------------------------------------------------
        // 3. Execute SYSDATE query
        // -----------------------------------------------------------
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = TestQuery;
            command.CommandType = System.Data.CommandType.Text;

            WriteInfo("Executing query...");
            var result = await command.ExecuteScalarAsync();

            if (result is not null && result != DBNull.Value)
            {
                WriteSuccess($"Query executed successfully.");
                Console.WriteLine($"  SYSDATE value : {result}");
                Console.WriteLine($"  Type          : {result.GetType().Name}");
            }
            else
            {
                WriteWarning("Query returned null or empty result.");
            }
        }
    }

    Console.WriteLine();
    WriteSuccess("Oracle connectivity test completed successfully.");
}
catch (OracleException ex)
{
    WriteError("Oracle Database error occurred.");
    Console.WriteLine($"  Error Code : {ex.Number}");
    Console.WriteLine($"  Message    : {ex.Message}");
    Console.WriteLine($"  Source     : {ex.Source}");

    // Categorize the Oracle error for clarity
    switch (ex.Number)
    {
        case 1017:
            WriteError("  → Cause: Invalid username/password. Check credentials.");
            break;
        case 1034:
        case 12541:
            WriteError("  → Cause: Cannot connect to Oracle (TNS listener down or unreachable).");
            break;
        case 12154:
            WriteError("  → Cause: TNS alias could not be resolved. Check tnsnames.ora or Data Source format.");
            break;
        case 28000:
        case 28001:
        case 28002:
            WriteError("  → Cause: Account is locked or password expired. Contact DBA.");
            break;
        default:
            WriteError($"  → Cause: Oracle error #{ex.Number}. See message above.");
            break;
    }
}
catch (JsonException ex)
{
    WriteError("Failed to parse configuration file.");
    Console.WriteLine($"  Message : {ex.Message}");
    WriteInfo($"Ensure '{AppSettingsFile}' contains valid JSON.");
}
catch (InvalidOperationException ex)
{
    WriteError("Configuration error.");
    Console.WriteLine($"  Message : {ex.Message}");
}
catch (Exception ex)
{
    WriteError("An unexpected error occurred.");
    Console.WriteLine($"  Type    : {ex.GetType().Name}");
    Console.WriteLine($"  Message : {ex.Message}");
}
finally
{
    Console.WriteLine();
    WriteInfo("Press any key to exit...");
    if (!Console.IsOutputRedirected)
    {
        Console.ReadKey(intercept: true);
    }
}

// =================================================================
// Helper methods
// =================================================================

static void WriteHeader(string text)
{
    var line = new string('=', text.Length + 4);
    Console.WriteLine(line);
    Console.WriteLine($"  {text}");
    Console.WriteLine(line);
    Console.WriteLine();
}

static void WriteInfo(string message)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[INFO] {message}");
    Console.ResetColor();
}

static void WriteSuccess(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[OK]   {message}");
    Console.ResetColor();
}

static void WriteWarning(string message)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[WARN] {message}");
    Console.ResetColor();
}

static void WriteError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[FAIL] {message}");
    Console.ResetColor();
}
