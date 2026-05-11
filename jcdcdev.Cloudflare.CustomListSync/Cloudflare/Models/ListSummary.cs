namespace jcdcdev.Cloudflare.CustomListSync.Cloudflare.Models;

public class ListSummary
{
	public string Id { get; set; } = string.Empty;
	public DateTimeOffset CreatedOn { get; set; }
	public string Kind { get; set; } = string.Empty;
	public DateTimeOffset ModifiedOn { get; set; }
	public string Name { get; set; } = string.Empty;
	public int NumItems { get; set; }
	public int NumReferencingFilters { get; set; }
	public string? Description { get; set; }
}

