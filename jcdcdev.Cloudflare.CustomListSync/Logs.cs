namespace jcdcdev.Cloudflare.CustomListSync;

public static partial class Logs
{
    [LoggerMessage(LogLevel.Information, "SyncWorker started — interval: {intervalSeconds}s, lists: {listCount}")]
    internal static partial void LogSyncWorkerStarted(this ILogger<SyncWorker> logger, int intervalSeconds, int listCount);

    [LoggerMessage(LogLevel.Information, "Sync cycle started at {startTime:O}")]
    internal static partial void LogSyncCycleStarted(this ILogger<SyncWorker> logger, DateTimeOffset startTime);

    [LoggerMessage(LogLevel.Error, "Sync cycle failed at {failureTime:O}")]
    internal static partial void LogSyncCycleFailed(this ILogger<SyncWorker> logger, Exception ex, DateTimeOffset failureTime);

    [LoggerMessage(LogLevel.Information, "Sync cycle completed in {elapsedMs}ms at {completedTime:O}")]
    internal static partial void LogSyncCycleCompleted(this ILogger<SyncWorker> logger, double elapsedMs, DateTimeOffset completedTime);

    [LoggerMessage(LogLevel.Debug, "Next sync cycle in {delaySeconds}s")]
    internal static partial void LogNextSyncCycleIn(this ILogger<SyncWorker> logger, double delaySeconds);

    [LoggerMessage(LogLevel.Information, "SyncWorker stopped")]
    internal static partial void LogSyncWorkerStopped(this ILogger<SyncWorker> logger);

    [LoggerMessage(LogLevel.Information, "No desired entries configured — skipping sync cycle")]
    internal static partial void LogNoDesiredEntriesConfigured(this ILogger<SyncWorker> logger);

    [LoggerMessage(LogLevel.Information, "Resolved {entryCount} desired entries")]
    internal static partial void LogResolvedDesiredEntries(this ILogger<SyncWorker> logger, int entryCount);

    [LoggerMessage(LogLevel.Warning, "No entries resolved with valid IPs — skipping sync cycle")]
    internal static partial void LogNoEntriesWithValidIps(this ILogger<SyncWorker> logger);

    [LoggerMessage(LogLevel.Information, "Resolved {comment} {ipVersion}: {ip}")]
    internal static partial void LogResolvedEntryIp(this ILogger<SyncWorker> logger, string comment, string ipVersion, string ip);

    [LoggerMessage(LogLevel.Warning, "Failed to detect {ipVersion} for entry '{comment}'")]
    internal static partial void LogFailedToDetectIpVersion(this ILogger<SyncWorker> logger, string ipVersion, string comment);
}