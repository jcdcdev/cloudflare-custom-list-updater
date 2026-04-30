using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

/// <summary>
/// Pagination info for list responses.
/// </summary>
public sealed class ResultInfo
{
	[JsonPropertyName("page")]
	public int Page { get; set; }

	[JsonPropertyName("per_page")]
	public int PerPage { get; set; }

	[JsonPropertyName("total_pages")]
	public int TotalPages { get; set; }

	[JsonPropertyName("count")]
	public int Count { get; set; }

	[JsonPropertyName("total_count")]
	public int TotalCount { get; set; }
}

