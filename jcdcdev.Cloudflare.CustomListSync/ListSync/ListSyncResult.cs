namespace jcdcdev.Cloudflare.CustomListSync.ListSync;

public sealed class ListSyncResult
{
	public int Added { get; init; }
	public int Removed { get; init; }
	public int Unchanged { get; init; }
	public string ListId { get; init; } = string.Empty;

	public bool HasChanges => Added > 0 || Removed > 0;
}

