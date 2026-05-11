using jcdcdev.Cloudflare.CustomListSync.Config;
using jcdcdev.Cloudflare.CustomListSync.Configuration;
using jcdcdev.Cloudflare.CustomListSync.IpDetection;
using jcdcdev.Cloudflare.CustomListSync.ListSync;
using Microsoft.Extensions.Options;
using ListEntry = jcdcdev.Cloudflare.CustomListSync.ListSync.ListEntry;

namespace jcdcdev.Cloudflare.CustomListSync;

public sealed class SyncWorker(
    ILogger<SyncWorker> logger,
    IOptions<AppOptions> options,
    ConfigProviderResolver configResolver,
    IIpDetector ipDetector,
    IListSyncer listSyncer)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogSyncWorkerStarted(options.Value.SyncIntervalSeconds, options.Value.Entries.Length);

        while (!stoppingToken.IsCancellationRequested)
        {
            var startedAt = DateTimeOffset.UtcNow;
            logger.LogSyncCycleStarted(startedAt);

            try
            {
                await ExecuteSyncCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogSyncCycleFailed(ex, DateTimeOffset.UtcNow);
            }

            var completedAt = DateTimeOffset.UtcNow;
            var elapsed = completedAt - startedAt;
            logger.LogSyncCycleCompleted(elapsed.TotalMilliseconds, completedAt);

            var delay = TimeSpan.FromSeconds(options.Value.SyncIntervalSeconds) - elapsed;
            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            logger.LogNextSyncCycleIn(delay.TotalSeconds);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        logger.LogSyncWorkerStopped();
    }

    private async Task ExecuteSyncCycleAsync(CancellationToken ct)
    {
        var configProvider = configResolver.Resolve();
        var entries = await configProvider.GetListSyncEntriesAsync(ct);

        if (entries.Count == 0)
        {
            logger.LogNoDesiredEntriesConfigured();
            return;
        }

        logger.LogResolvedDesiredEntries(entries.Count);

        var resolvedEntries = await ResolveEntriesAsync(entries, ct);
        if (resolvedEntries.Count == 0)
        {
            logger.LogNoEntriesWithValidIps();
            return;
        }

        foreach (var name in options.Value.ParsedListIds)
        {
            ct.ThrowIfCancellationRequested();
            await listSyncer.ProcessAsync(name, resolvedEntries, ct);
        }
    }
    
    private async Task<List<ListEntry>> ResolveEntriesAsync(IReadOnlyList<ListSyncEntry> entries, CancellationToken ct)
    {
        var resolved = new List<ListEntry>();

        foreach (var entry in entries)
        {
            if (entry.Address is "ipv4" or "ipv6")
            {
                var ipVersion = entry.Address switch
                {
                    "ipv4" => IpVersion.V4,
                    "ipv6" => IpVersion.V6,
                    _ => throw new InvalidOperationException("Unreachable")
                };

                var ip = await ipDetector.GetPublicIpAsync(ipVersion, ct);
                if (ip is not null)
                {
                    resolved.Add(new ListEntry { Comment = entry.Label, Ip = ip });
                    logger.LogResolvedEntryIp(entry.Label, entry.Address, ip);
                }
                else
                {
                    logger.LogFailedToDetectIpVersion(entry.Address, entry.Label);
                }

                continue;
            }

            resolved.Add(new ListEntry
            {
                Comment = entry.Label,
                Ip = entry.Address
            });
        }

        return resolved;
    }
}