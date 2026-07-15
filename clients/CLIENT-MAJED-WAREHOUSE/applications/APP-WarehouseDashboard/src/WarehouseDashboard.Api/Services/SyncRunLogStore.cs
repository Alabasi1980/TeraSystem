using System.Collections.Generic;
using System.Linq;
using WarehouseDashboard.Api.Models;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// TEMPORARY thread-safe, in-memory ring buffer of recent sync runs (max 100 entries).
///
/// <para>
/// This is a stopgap until the structured <c>SyncLogs</c> / <c>ErrorLogs</c> DB tables are created
/// (deferred task). It is registered as a singleton so the <see cref="Controllers.SyncController"/>
/// and the running <see cref="SyncEngineService"/> share the same buffer. Data is lost on process
/// restart and must NOT be used for audit/compliance — only for live monitoring.
/// </para>
/// </summary>
public class SyncRunLogStore
{
    private readonly object _lock = new();
    private readonly List<SyncRunRecord> _records = new();
    private const int MaxRecords = 100;

    /// <summary>
    /// Opens a new run record (status <c>"Running"</c>) and appends it to the buffer.
    /// The returned reference is later passed to <see cref="CompleteRun"/> to fill in the result.
    /// </summary>
    public SyncRunRecord BeginRun(string triggerType)
    {
        var record = new SyncRunRecord
        {
            StartTime = DateTime.UtcNow,
            Status = "Running",
            TriggerType = triggerType
        };

        lock (_lock)
        {
            _records.Add(record);

            // Ring buffer: keep only the newest MaxRecords entries.
            if (_records.Count > MaxRecords)
            {
                _records.RemoveRange(0, _records.Count - MaxRecords);
            }
        }

        return record;
    }

    /// <summary>
    /// Marks a previously opened run as finished, recording its final status, total row count,
    /// end time, and any error message.
    /// </summary>
    public void CompleteRun(SyncRunRecord? record, string status, int recordCount, string? errorMessage)
    {
        if (record is null)
            return;

        record.EndTime = DateTime.UtcNow;
        record.Status = status;
        record.RecordCount = recordCount;
        record.ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Returns the buffered runs, newest first (up to 100).
    /// </summary>
    public IReadOnlyList<SyncRunRecord> GetRecent()
    {
        lock (_lock)
        {
            return _records
                .OrderByDescending(r => r.StartTime)
                .ToList();
        }
    }
}
