namespace jcdcdev.Cloudflare.CustomListSync.Config;

/// <summary>
/// Provides Cloudflare list entries for synchronization.
/// </summary>
public interface IListSyncEntryProvider
{
    /// <summary>
    /// Get the list of entries to synchronize.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Read-only list of entries</returns>
    Task<IReadOnlyList<ListSyncEntry>> GetListSyncEntriesAsync(CancellationToken ct = default);
}

