using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public class ListItemCreateResponse
{
	[JsonPropertyName("errors")] public List<ApiMessage>? Errors { get; set; }

	[JsonPropertyName("messages")] public List<ApiMessage>? Messages { get; set; }
}

