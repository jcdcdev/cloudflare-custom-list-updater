using System.ComponentModel.DataAnnotations;

namespace jcdcdev.Cloudflare.CustomListSync.Configuration;

/// <summary>
/// Configuration options bound from CF_-prefixed environment variables.
/// </summary>
public sealed class AppOptions
{
    /// <summary>
    /// Cloudflare API token with permissions to manage account-level rules lists.
    /// Env: CF_API_TOKEN
    /// </summary>
    [Required(ErrorMessage = "Cloudflare API token is required. Set CF_API_TOKEN environment variable.")]
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>
    /// Cloudflare account ID that owns the target rules lists.
    /// Env: CF_ACCOUNT_ID
    /// </summary>
    [Required(ErrorMessage = "Cloudflare account ID is required. Set CF_ACCOUNT_ID environment variable.")]
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of Cloudflare custom list IDs to sync.
    /// Env: CF_LIST_IDS
    /// </summary>
    [Required(ErrorMessage = "At least one Cloudflare list ID is required. Set CF_LIST_IDS environment variable.")]
    public string ListIds { get; set; } = string.Empty;

    /// <summary>
    /// Interval between sync runs. Defaults to 5 minutes.
    /// Env: CF_SYNC_INTERVAL_SECONDS (parsed as seconds)
    /// </summary>
    public int SyncIntervalSeconds { get; set; } = 300;
    
    /// <summary>
    /// When true, orphan entries (present in Cloudflare but not in source) are kept.
    /// Defaults to false — orphans are removed for declarative sync.
    /// Env: CF_KEEP_ORPHANS
    /// </summary>
    public bool KeepOrphans { get; set; }

    /// <summary>
    /// Semicolon-separated list of name=value pairs (KVP mode).
    /// Example: "home=ipv4;office=10.0.0.1;vpn=172.16.0.1"
    /// Value "ipv4"/"ipv6" auto-detects that IP version;
    /// any other value is used as-is.
    /// Env: CF_ENTRIES
    /// </summary>
    public string Entries { get; set; } = string.Empty;

    /// <summary>
    /// Path to a CSV file with columns: comment, value.
    /// Takes priority over CF_ENTRIES when set.
    /// Env: CF_FILE_PATH
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    public IEnumerable<string> ParsedListIds => ListIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => id.Trim()).Where(x=>!string.IsNullOrEmpty(x));
}