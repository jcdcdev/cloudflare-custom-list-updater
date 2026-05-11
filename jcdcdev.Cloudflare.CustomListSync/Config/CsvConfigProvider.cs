using jcdcdev.Cloudflare.CustomListSync.Configuration;
using Microsoft.Extensions.Options;

namespace jcdcdev.Cloudflare.CustomListSync.Config;

/// <summary>
/// CSV mode: reads file at CF_FILE_PATH with columns: label, address.
/// Header row is expected. Address "ipv4"/"ipv6" means auto-detect;
/// any other value is used as-is.
/// </summary>
public sealed class CsvConfigProvider(IOptions<AppOptions> options, ILogger<CsvConfigProvider> logger) : IListSyncEntryProvider
{
    public async Task<IReadOnlyList<ListSyncEntry>> GetListSyncEntriesAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(options.Value.FilePath))
        {
            return [];
        }

        if (!File.Exists(options.Value.FilePath))
        {
            logger.LogWarning("CSV file not found: {FilePath}", options.Value.FilePath);
            return [];
        }

        var lines = await File.ReadAllLinesAsync(options.Value.FilePath, ct);
        if (lines.Length == 0)
        {
            return [];
        }

        var entries = new List<ListSyncEntry>();

        // Skip header row
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 1) continue;

            var label = parts[0];
            var address = parts.Length >= 2 ? parts[1].ToLowerInvariant() : string.Empty;

            if (string.IsNullOrEmpty(label)) continue;

            entries.Add(new ListSyncEntry
            {
                Label = label,
                Address = address
            });
        }

        logger.LogInformation("Parsed {EntryCount} entries from CSV file {FilePath}", entries.Count, options.Value.FilePath);
        return entries;
    }
}
