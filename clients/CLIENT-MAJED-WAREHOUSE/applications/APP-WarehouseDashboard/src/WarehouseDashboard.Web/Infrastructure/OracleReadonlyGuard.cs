namespace WarehouseDashboard.Web.Infrastructure;

/// <summary>
/// Server-side guard that ensures an Oracle SQL batch is read-only before it is
/// executed by the Admin Oracle Query Lab. Defence in depth: the UI also prevents
/// writes, but the executing endpoint must never trust the client.
///
/// Rules enforced:
/// - Every statement in the batch must be a <c>SELECT</c>, a <c>WITH</c> (CTE)
///   that leads to a <c>SELECT</c>, or a parenthesised subquery <c>(SELECT ...)</c>.
/// - Leading comments (<c>--</c> line comments and <c>/* */</c> block comments)
///   are stripped before analysis.
/// - Data-modifying and DDL keywords are rejected with a clear Arabic message.
/// </summary>
public static class OracleReadonlyGuard
{
    /// <summary>
    /// First-word keywords that are never allowed in the read-only Oracle tester.
    /// </summary>
    private static readonly HashSet<string> BlockedKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "INSERT", "UPDATE", "DELETE", "MERGE", "DROP", "CREATE", "ALTER", "TRUNCATE",
        "EXEC", "EXECUTE", "GRANT", "REVOKE", "DENY", "BACKUP", "RESTORE", "BULK",
        "USE", "SHUTDOWN", "KILL", "COMMIT", "ROLLBACK", "SAVEPOINT", "LOCK", "EXPLAIN"
    };

    /// <summary>
    /// Returns <c>true</c> when the Oracle batch is read-only; otherwise
    /// <paramref name="reason"/> contains a localized, user-safe explanation.
    /// </summary>
    public static bool IsReadOnly(string sql, out string reason)
    {
        reason = string.Empty;

        if (string.IsNullOrWhiteSpace(sql))
        {
            reason = "الرجاء إدخال استعلام Oracle.";
            return false;
        }

        // Remove comments and blank out string literal contents so their
        // contents (which may contain keywords or semicolons) cannot spoof
        // the analysis.
        var normalized = Normalize(sql);

        // Split into individual statements on ';' (literals already blanked).
        var statements = normalized
            .Split(';')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();

        if (statements.Count == 0)
        {
            reason = "الرجاء إدخال استعلام Oracle صالح.";
            return false;
        }

        foreach (var statement in statements)
        {
            var firstWord = LeadingKeyword(statement);
            if (string.IsNullOrEmpty(firstWord))
            {
                reason = "تعذر تحليل الاستعلام. تأكد من صياغته بشكل صحيح.";
                return false;
            }

            if (firstWord.Equals("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                // fall through to the blocked-keyword scan below
            }
            else if (firstWord.Equals("WITH", StringComparison.OrdinalIgnoreCase))
            {
                // A WITH (CTE) statement must lead to a SELECT somewhere.
                if (!ContainsSelect(statement))
                {
                    reason = "استعلام WITH يجب أن يؤدي إلى SELECT (للقراءة فقط).";
                    return false;
                }
            }
            else if (firstWord.StartsWith("(", StringComparison.Ordinal))
            {
                // Parenthesised subquery — allowed if it contains SELECT.
                if (!ContainsSelect(statement))
                {
                    reason = "استعلام نائب يجب أن يحتوي على SELECT (للقراءة فقط).";
                    return false;
                }
            }
            else
            {
                reason = $"الاستعلام غير مسموح: '{firstWord}' ليس استعلام قراءة فقط (SELECT / WITH...SELECT).";
                return false;
            }

            // Any read-only statement must not contain a forbidden keyword
            // (catches e.g. "WITH cte AS (SELECT 1) INSERT INTO ...").
            var forbidden = BlockedKeywordFound(statement);
            if (forbidden is not null)
            {
                reason = $"الاستعلام غير مسموح: تم رصد الكلمة '{forbidden}' التي تعدّل البيانات أو المخطط.";
                return false;
            }
        }

        return true;
    }

    private static string? BlockedKeywordFound(string statement)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            statement,
            @"\b(" + string.Join("|", BlockedKeywords) + @")\b",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return match.Success ? match.Value : null;
    }

    private static bool ContainsSelect(string statement)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(
            statement, @"\bSELECT\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Returns the leading alphabetic keyword of a statement, ignoring any
    /// leading parentheses (e.g. "(SELECT ...)").
    /// </summary>
    private static string LeadingKeyword(string statement)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            statement, @"^\s*\(*\s*([A-Za-z]+)");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    /// <summary>
    /// Strips comments and replaces the contents of string literals (<c>'...'</c>)
    /// with spaces, preserving structure so keyword and statement-boundary
    /// detection is reliable. Oracle does not use bracket identifiers like
    /// SQL Server, so only comments and string literals are handled.
    /// </summary>
    private static string Normalize(string sql)
    {
        var sb = new System.Text.StringBuilder(sql.Length);
        bool inLineComment = false;
        bool inBlockComment = false;
        bool inString = false;

        for (int i = 0; i < sql.Length; i++)
        {
            char c = sql[i];
            char next = i + 1 < sql.Length ? sql[i + 1] : '\0';

            if (inLineComment)
            {
                if (c == '\n')
                {
                    inLineComment = false;
                    sb.Append(c);
                }
                continue;
            }

            if (inBlockComment)
            {
                if (c == '*' && next == '/')
                {
                    inBlockComment = false;
                    i++;
                }
                continue;
            }

            if (inString)
            {
                if (c == '\'')
                {
                    if (next == '\'') // escaped quote ''
                    {
                        i++;
                        continue;
                    }
                    inString = false;
                }
                continue; // consume literal content
            }

            // Not in any special state.
            if (c == '-' && next == '-')
            {
                inLineComment = true;
                i++;
                continue;
            }

            if (c == '/' && next == '*')
            {
                inBlockComment = true;
                i++;
                continue;
            }

            if (c == '\'')
            {
                inString = true;
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}
