using jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare;

/// <summary>
/// Exception thrown when the Cloudflare API returns an error response.
/// </summary>
public sealed class CloudflareApiException : Exception
{
    /// <summary>
    /// The API errors returned by Cloudflare.
    /// </summary>
    public IReadOnlyList<ApiError> ApiErrors { get; }

    /// <summary>
    /// The HTTP status code of the failed response, if available.
    /// </summary>
    public int? StatusCode { get; }

    public CloudflareApiException(string message, IReadOnlyList<ApiError> errors, int? statusCode = null)
        : base(message)
    {
        ApiErrors = errors;
        StatusCode = statusCode;
    }

    public CloudflareApiException(string message, IReadOnlyList<ApiError> errors, int statusCode, Exception inner)
        : base(message, inner)
    {
        ApiErrors = errors;
        StatusCode = statusCode;
    }
}
