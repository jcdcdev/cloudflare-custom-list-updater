using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

/// <summary>
/// Represents an async Cloudflare bulk operation.
/// </summary>
public sealed class Operation
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	[JsonPropertyName("status")]
	public string Status { get; set; } = string.Empty;

	[JsonPropertyName("error")]
	public string? Error { get; set; }

	[JsonPropertyName("created_date")]
	public DateTime? CreatedDate { get; set; }

	[JsonPropertyName("completed_date")]
	public DateTime? CompletedDate { get; set; }
}

