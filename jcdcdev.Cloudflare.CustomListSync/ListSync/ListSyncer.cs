using jcdcdev.Cloudflare.CustomListSync.Cloudflare;
using jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;
using jcdcdev.Cloudflare.CustomListSync.Configuration;
using Microsoft.Extensions.Options;

namespace jcdcdev.Cloudflare.CustomListSync.ListSync;

public sealed class ListSyncer(
    ICloudflareApiClient apiClient,
    ILogger<ListSyncer> logger,
    IOptions<AppOptions> options)
    : IListSyncer
{
    public async Task<ListSyncResult> ProcessAsync(string name, IReadOnlyList<ListEntry> desiredEntries, CancellationToken ct = default)
    {
        var listId = await apiClient.GetOrCreateListAsync(name, cancellationToken: ct);
        logger.LogInformation("Syncing list {ListId} with {DesiredCount} desired entries", listId, desiredEntries.Count);

        // Fetch actual state
        var actualItems = (await apiClient.GetListItemsAsync(listId, ct)).Result ?? [];
        logger.LogInformation("List {ListId} has {ActualCount} actual items", listId, actualItems.Count);

        // Build lookup of actual items by comment
        var actualByComment = actualItems
            .Where(i => !string.IsNullOrWhiteSpace(i.Comment))
            .ToDictionary(i => i.Comment, i => i, StringComparer.OrdinalIgnoreCase);

        // Build lookup of desired entries by comment
        var desiredByComment = desiredEntries
            .Where(e => !string.IsNullOrWhiteSpace(e.Comment))
            .ToDictionary(e => e.Comment, e => e, StringComparer.OrdinalIgnoreCase);

        // Compute diff
        var toAdd = desiredEntries.ToList();
        var toRemove = actualByComment.Values
            .Where(a => !desiredByComment.ContainsKey(a.Comment))
            .ToList();

        var unchanged = actualByComment.Values
            .Count(a => desiredByComment.TryGetValue(a.Comment, out var desired) && desired.Ip == a.Ip);

        logger.LogInformation("ListSync diff for list {ListId}: {ToAdd} to add, {ToRemove} to remove, {Unchanged} unchanged", listId, toAdd.Count, toRemove.Count, unchanged);

        if (options.Value.KeepOrphans)
        {
            logger.LogInformation("Keeping {OrphanCount} orphan entries in list {ListId} (CF_KEEP_ORPHANS=true)", toRemove.Count, listId);
            toAdd = toAdd.Concat(desiredEntries).ToList();
        }

        var createRequests = new List<ListItemCreateRequest>();
        foreach (var entry in toAdd)
        {
            ct.ThrowIfCancellationRequested();
            createRequests.Add(
                new ListItemCreateRequest
                {
                    Ip = entry.Ip,
                    Comment = entry.Comment
                });
        }

        var operation = await apiClient.UpdateAllListItems(listId, createRequests, ct);

        var result = new ListSyncResult
        {
            ListId = listId,
            Added = toAdd.Count,
            Removed = options.Value.KeepOrphans ? 0 : toRemove.Count,
            Unchanged = unchanged,
        };

        logger.LogInformation("ListSync complete for list {ListId}: {Added} added, {Removed} removed", listId, result.Added, result.Removed);
        foreach (var entry in toAdd)
        {
            logger.LogInformation("Added to list {ListId}: {Comment} ({Ip})", listId, entry.Comment, entry.Ip);
        }

        return result;
    }
}