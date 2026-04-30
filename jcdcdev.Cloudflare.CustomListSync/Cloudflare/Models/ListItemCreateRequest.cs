using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public sealed class ListItemCreateRequest
{
	[JsonPropertyName("ip")]
	public string Ip { get; set; } = string.Empty;

	[JsonPropertyName("comment")]
	public string Comment { get; set; } = string.Empty;
}

