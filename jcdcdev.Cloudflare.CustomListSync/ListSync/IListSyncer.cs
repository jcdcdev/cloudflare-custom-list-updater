namespace jcdcdev.Cloudflare.CustomListSync.ListSync;

public interface IListSyncer
{
	Task<ListSyncResult> ProcessAsync(string listId, IReadOnlyList<ListEntry> desiredEntries, CancellationToken ct = default);
}

