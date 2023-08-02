namespace Domain.Models.Items.GetItems
{
    public sealed record SearchItemsPageV1Query(string? Name, string? Description, int PageNumber);
    public sealed record SearchItemsPageV2Query(string SearchValue, int PageNumber);
    public sealed record UpdateItemQuery(int Id, string? Name, string? Description, string? Metadata);
}
