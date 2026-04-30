namespace jcdcdev.Cloudflare.CustomListSync.Config;

/// <summary>
/// Represents a Cloudflare list entry to be synchronized.
/// The Label field is the unique key used for matching against existing list items.
/// </summary>
public sealed class ListSyncEntry
{
    /// <summary>
    /// Unique label for this entry — matches the comment field on Cloudflare list items.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The IP address or keyword determining how to resolve this entry:
    /// - "ipv4" → auto-detect public IPv4
    /// - "ipv6" → auto-detect public IPv6
    /// - any other value → use as-is (static IP/CIDR)
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Returns true if Address is an auto-detect keyword (ipv4 or ipv6).
    /// </summary>
    public bool IsAutoDetect => Address is "ipv4" or "ipv6";
}

