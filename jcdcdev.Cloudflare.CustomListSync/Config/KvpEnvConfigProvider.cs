using jcdcdev.Cloudflare.CustomListSync.Configuration;
using Microsoft.Extensions.Options;

namespace jcdcdev.Cloudflare.CustomListSync.Config;

/// <summary>
/// KVP mode: reads CF_ENTRIES with semicolon-separated name=value pairs.
/// Example: "home=ipv4;office=10.0.0.1;server=192.168.1.1"
/// Value "ipv4"/"ipv6" auto-detects that IP version;
/// any other value is used as-is.
/// </summary>
public sealed class KvpEnvConfigProvider(IOptions<AppOptions> options) : IListSyncEntryProvider
{
    public Task<IReadOnlyList<ListSyncEntry>> GetListSyncEntriesAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(options.Value.Entries))
        {
            return Task.FromResult<IReadOnlyList<ListSyncEntry>>([]);
        }

        var entries = new List<ListSyncEntry>();
        var pairs = options.Value.Entries.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var pair in pairs)
        {
            var eqIndex = pair.IndexOf('=');
            if (eqIndex < 0) continue;

            var label = pair[..eqIndex].Trim();
            var address = pair[(eqIndex + 1)..].Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(label)) continue;

            entries.Add(new ListSyncEntry
            {
                Label = label,
                Address = address
            });
        }

        return Task.FromResult<IReadOnlyList<ListSyncEntry>>(entries);
    }
}