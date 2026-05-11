using System.Text.Json.Serialization;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public class ApiMessage
{
    [JsonPropertyName("code")] public int Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("documentation_url")]
    public string? DocumentationUrl { get; set; }

    [JsonPropertyName("source")] public CloudflareApiSource? Source { get; set; }

    public class CloudflareApiSource
    {
        [JsonPropertyName("pointer")] public string? Pointer { get; set; }
    }
}