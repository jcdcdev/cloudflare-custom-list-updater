using System.Text.Json;
using jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;
using jcdcdev.Cloudflare.CustomListSync.Configuration;
using Microsoft.Extensions.Options;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare;

public sealed class CloudflareApiClient(HttpClient httpClient, ILogger<CloudflareApiClient> logger, IOptions<AppOptions> options) : ICloudflareApiClient
{
    private readonly string _accountId = options.Value.AccountId;

    public async Task<ListItemResponse> GetListItemsAsync(string listId, CancellationToken ct = default)
    {
        var path = $"accounts/{_accountId}/rules/lists/{listId}/items";
        var response = await SendRequestAsync<ListItemResponse>(HttpMethod.Get, path, ct: ct);
        return response;
    }

    public async Task<ListCreateResponse> CreateListAsync(string name, CancellationToken ct = default)
    {
        var path = $"accounts/{_accountId}/rules/lists";
        var body = new ListCreateRequest
        {
            Name = name,
            Kind = "ip",
            Description = "Auto managed ip list",
        };

        var response = await SendRequestAsync<ListCreateResponse>(HttpMethod.Post, path, body, ct);
        return response;
    }

    public async Task<ListItemCreateResponse> UpdateAllListItems(string listId, IEnumerable<ListItemCreateRequest> items, CancellationToken ct = default)
    {
        var path = $"accounts/{_accountId}/rules/lists/{listId}/items";
        var body = items.ToList();
        var response = await SendRequestAsync<ListItemCreateResponse>(HttpMethod.Put, path, body, ct);
        return response;
    }

    public async Task<string> GetOrCreateListAsync(string name, CancellationToken cancellationToken)
    {
        var path = $"accounts/{_accountId}/rules/lists";
        var response = await SendRequestAsync<ListsResponse>(HttpMethod.Get, path, ct: cancellationToken);
        var existing = response.Result?.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing.Id;
        }

        var newList = await CreateListAsync(name, cancellationToken);
        var id = newList.Result?.Id;
        if (id is null)
        {
            throw new CloudflareApiException("Failed to create new list.", [], 0);
        }

        return id;
    }


    private async Task<T> SendRequestAsync<T>(HttpMethod method, string path, object? body = null, CancellationToken ct = default)
    {
        logger.LogDebug("CF API {Method} /{Path}", method, path);

        var startedAt = DateTimeOffset.UtcNow;
        using var request = new HttpRequestMessage(method, path);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        var elapsed = (DateTimeOffset.UtcNow - startedAt).TotalMilliseconds;

        logger.LogInformation("CF API {Method} /{Path} → {StatusCode} in {ElapsedMs:F0}ms", method, path, (int)response.StatusCode, elapsed);

        var json = await response.Content.ReadAsStringAsync(ct);
        var apiResponse = JsonSerializer.Deserialize<T>(json, JsonOptions);

        if (apiResponse is null)
        {
            throw new CloudflareApiException("Failed to deserialize Cloudflare API response.", [], (int)response.StatusCode);
        }

        return apiResponse;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false,
    };
}