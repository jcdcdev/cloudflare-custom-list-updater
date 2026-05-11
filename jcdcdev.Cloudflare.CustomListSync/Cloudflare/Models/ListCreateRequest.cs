
using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public class ListCreateRequest
{
	[JsonPropertyName("name")] public required string Name { get; set; }

	[JsonPropertyName("kind")] public required string Kind { get; set; }

	[JsonPropertyName("description")] public required string Description { get; set; }
}

