using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using WarehouseDashboard.Api.Models;

namespace WarehouseDashboard.Api.Services;

/// <summary>
/// Thread-safe, in-memory store for live sync-run progress (TASK-ENH-003).
///
/// <para>
/// Each call to <c>POST /api/sync/trigger-selected</c> creates a <see cref="SyncRunProgress"/>
/// entry keyed by a new <see cref="Guid"/>. The caller receives the <c>runId</c> immediately and
/// polls <c>GET /api/sync/progress?runId=...</c> to watch per-mapping status, row counts, and
/// overall completion percent.
/// </para>
///
/// <para>
/// Runs older than 5 minutes are automatically evicted by a background <see cref="Timer"/> that
/// fires every 60 seconds. This is an in-memory store only — progress is lost on restart.
/// </para>
/// </summary>
public class SyncRunProgressStore
{
    private readonly ConcurrentDictionary<Guid, SyncRunProgress> _runs = new();
    private readonly ILogger<SyncRunProgressStore> _logger;
    private static readonly TimeSpan MaxAge = TimeSpan.FromMinutes(5);
    private Timer? _cleanupTimer;

    public SyncRunProgressStore(ILogger<SyncRunProgressStore> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new progress run for the requested mapping IDs. Every mapping starts as
    /// <c>"pending"</c> with zero rows and zero percent. The returned <see cref="SyncRunProgress"/>
    /// is also stored internally and can be retrieved via <see cref="GetRun"/>.
    /// </summary>
    /// <param name="mappingIds">The list of mapping IDs the caller requested (may include IDs that are no longer active).</param>
    /// <param name="mappings">The resolved <see cref="TableMapping"/> instances loaded from the DB. Used for display names only.</param>
    /// <returns>The newly created <see cref="SyncRunProgress"/> (already stored).</returns>
    public SyncRunProgress CreateRun(List<int> mappingIds, List<TableMapping> mappings)
    {
        var run = new SyncRunProgress
        {
            RunId = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            OverallStatus = "running",
            Mappings = mappingIds.Select(id =>
            {
                var mapping = mappings.FirstOrDefault(m => m.Id == id);
                return new MappingProgress
                {
                    MappingId = id,
                    TargetTable = mapping?.SqlTargetTable ?? $"Mapping#{id}",
                    Status = "pending"
                };
            }).ToList()
        };

        _runs[run.RunId] = run;

        _logger.LogDebug(
            "SyncRunProgressStore: created run {RunId} for {Count} mapping(s).",
            run.RunId, mappingIds.Count);

        return run;
    }

    /// <summary>
    /// Retrieves a previously created run by its <paramref name="runId"/>.
    /// Returns <c>null</c> if the run does not exist or has been cleaned up.
    /// </summary>
    public SyncRunProgress? GetRun(Guid runId)
    {
        _runs.TryGetValue(runId, out var run);
        return run;
    }

    /// <summary>
    /// Updates the progress of a single mapping within a run. If the mapping is not found
    /// (e.g. the run was cleaned up between ticks), the update is silently ignored.
    /// After updating the mapping, the run's <see cref="SyncRunProgress.OverallPercent"/>
    /// and <see cref="SyncRunProgress.TotalRowsSoFar"/> are recalculated.
    /// </summary>
    public void UpdateMapping(Guid runId, int mappingId, string status, int rowsSoFar, string? error = null)
    {
        if (!_runs.TryGetValue(runId, out var run))
        {
            _logger.LogDebug(
                "SyncRunProgressStore: run {RunId} not found — skipping UpdateMapping for Mapping#{MappingId}.",
                runId, mappingId);
            return;
        }

        var mappingProgress = run.Mappings.FirstOrDefault(m => m.MappingId == mappingId);
        if (mappingProgress is null)
        {
            _logger.LogDebug(
                "SyncRunProgressStore: Mapping#{MappingId} not found in run {RunId} — skipping update.",
                mappingId, runId);
            return;
        }

        mappingProgress.Status = status;
        mappingProgress.RowsSoFar = rowsSoFar;
        mappingProgress.Error = error;

        mappingProgress.Percent = status switch
        {
            "completed" => 100,
            "failed" => 0,
            "pending" => 0,
            // For "running" we keep the previous percent — or set to 0 if it was never set.
            _ => mappingProgress.Percent
        };

        // Recalculate totals.
        run.TotalRowsSoFar = run.Mappings.Sum(m => m.RowsSoFar);
        var completedCount = run.Mappings.Count(m => m.Status == "completed");
        run.OverallPercent = run.Mappings.Count > 0
            ? (int)Math.Round((double)completedCount / run.Mappings.Count * 100)
            : 0;

        _logger.LogDebug(
            "SyncRunProgressStore: run {RunId} Mapping#{MappingId} → {Status}, {Rows} row(s). Overall={Overall}%.",
            runId, mappingId, status, rowsSoFar, run.OverallPercent);
    }

    /// <summary>
    /// Marks the entire run as finished, sets <see cref="SyncRunProgress.OverallPercent"/> to 100,
    /// and records the elapsed duration.
    /// </summary>
    public void CompleteRun(Guid runId, string overallStatus)
    {
        if (!_runs.TryGetValue(runId, out var run))
        {
            _logger.LogDebug(
                "SyncRunProgressStore: run {RunId} not found — skipping CompleteRun.", runId);
            return;
        }

        run.OverallStatus = overallStatus;
        run.OverallPercent = 100;
        run.ElapsedSeconds = Math.Round((DateTime.UtcNow - run.StartedAt).TotalSeconds, 1);

        _logger.LogInformation(
            "SyncRunProgressStore: run {RunId} completed. Status={Status}, {Rows} row(s), {Elapsed:F1}s.",
            runId, overallStatus, run.TotalRowsSoFar, run.ElapsedSeconds);
    }

    /// <summary>
    /// Removes all runs whose age exceeds <see cref="MaxAge"/> (5 minutes).
    /// Called periodically by the cleanup <see cref="Timer"/>.
    /// </summary>
    private void CleanupOldRuns(object? state)
    {
        var cutoff = DateTime.UtcNow - MaxAge;
        var expired = _runs
            .Where(kvp => kvp.Value.StartedAt < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        if (expired.Count == 0)
            return;

        foreach (var runId in expired)
        {
            if (_runs.TryRemove(runId, out var removed))
            {
                _logger.LogDebug(
                    "SyncRunProgressStore: cleaned up expired run {RunId} (started at {StartedAt:O}).",
                    removed.RunId, removed.StartedAt);
            }
        }

        if (expired.Count > 0)
        {
            _logger.LogInformation(
                "SyncRunProgressStore: cleanup removed {Count} expired run(s).", expired.Count);
        }
    }

    /// <summary>
    /// Starts the background cleanup timer. Must be called once during application startup
    /// (after the service provider is built). The timer fires every 60 seconds and evicts
    /// runs older than <see cref="MaxAge"/>.
    /// </summary>
    public void Initialize()
    {
        // Fire the first cleanup after MaxAge (give runs a chance to complete),
        // then every 60 seconds thereafter.
        _cleanupTimer = new Timer(
            CleanupOldRuns,
            null,
            dueTime: MaxAge,
            period: TimeSpan.FromMinutes(1));

        _logger.LogInformation(
            "SyncRunProgressStore initialized. Cleanup timer started (interval=1m, maxAge=5m).");
    }
}
