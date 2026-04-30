using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public class ListsResponse
{
	[JsonPropertyName("errors")] public List<ApiMessage>? Errors { get; set; }

	[JsonPropertyName("messages")] public List<ApiMessage>? Messages { get; set; }

	[JsonPropertyName("success")] private bool Success { get; set; }

	[JsonPropertyName("result")] public List<ListSummary>? Result { get; set; }
}

