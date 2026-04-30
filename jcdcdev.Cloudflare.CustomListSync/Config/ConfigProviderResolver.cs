using jcdcdev.Cloudflare.CustomListSync.Configuration;
using Microsoft.Extensions.Options;

namespace jcdcdev.Cloudflare.CustomListSync.Config;

/// <summary>
/// Resolves to appropriate IListSyncEntryProvider based on which environment variables are set.
/// </summary>
public sealed class ConfigProviderResolver(IOptions<AppOptions> options, ILoggerFactory loggerFactory)
{
    /// <summary>
    /// Returns to appropriate list sync entry provider based on configuration.
    /// Priority: CsvConfigProvider > KvpEnvConfigProvider.
    /// </summary>
    public IListSyncEntryProvider Resolve()
    {
        if (!string.IsNullOrWhiteSpace(options.Value.FilePath))
        {
            return new CsvConfigProvider(options, loggerFactory.CreateLogger<CsvConfigProvider>());
        }

        return new KvpEnvConfigProvider(options);
    }
}
