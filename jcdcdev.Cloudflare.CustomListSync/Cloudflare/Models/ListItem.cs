using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

/// <summary>
/// Represents an item in a Cloudflare custom list.
/// </summary>
public sealed class ListItem
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	[JsonPropertyName("ip")]
	public string Ip { get; set; } = string.Empty;

	[JsonPropertyName("comment")]
	public string Comment { get; set; } = string.Empty;

	[JsonPropertyName("created_on")]
	public DateTime? CreatedOn { get; set; }

	[JsonPropertyName("modified_on")]
	public DateTime? ModifiedOn { get; set; }
}

