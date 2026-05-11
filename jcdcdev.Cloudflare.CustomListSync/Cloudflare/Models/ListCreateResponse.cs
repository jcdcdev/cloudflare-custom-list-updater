using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public class ListCreateResponse
{
	[JsonPropertyName("errors")] public List<ApiMessage>? Errors { get; set; }

	[JsonPropertyName("messages")] public List<ApiMessage>? Messages { get; set; }

	[JsonPropertyName("result")] public ListCreateResult? Result { get; set; }

	[JsonPropertyName("success")] public bool Success { get; set; }

	public class ListCreateResult
	{
		[JsonPropertyName("id")] public string? Id { get; set; }

		[JsonPropertyName("created_on")] public DateTimeOffset CreatedOn { get; set; }

		[JsonPropertyName("kind")] public string? Kind { get; set; }

		[JsonPropertyName("modified_on")] public DateTimeOffset ModifiedOn { get; set; }

		[JsonPropertyName("name")] public string? Name { get; set; }

		[JsonPropertyName("num_items")] public int NumItems { get; set; }

		[JsonPropertyName("num_referencing_filters")]
		public int NumReferencingFilters { get; set; }

		[JsonPropertyName("description")] public string? Description { get; set; }
	}
}

