namespace jcdcdev.Cloudflare.CustomListSync.ListSync;

/// <summary>
/// Represents an entry in the Cloudflare custom list for syncing.
/// </summary>
public sealed class ListEntry
{
	/// <summary>
	/// Unique identifier for this entry — matches the comment field on Cloudflare list items.
	/// </summary>
	public string Comment { get; set; } = string.Empty;

	/// <summary>
	/// The IP address for this entry.
	/// </summary>
	public string Ip { get; set; } = string.Empty;
}

