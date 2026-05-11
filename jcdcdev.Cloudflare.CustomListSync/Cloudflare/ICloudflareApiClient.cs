using jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare;

public interface ICloudflareApiClient
{

    Task<ListItemResponse> GetListItemsAsync(string listId, CancellationToken ct = default);
    Task<ListCreateResponse> CreateListAsync(string name, CancellationToken ct = default);
    
    Task<ListItemCreateResponse> UpdateAllListItems(string listId, IEnumerable<ListItemCreateRequest> items, CancellationToken ct = default);
    Task<string> GetOrCreateListAsync(string name, CancellationToken cancellationToken);
}