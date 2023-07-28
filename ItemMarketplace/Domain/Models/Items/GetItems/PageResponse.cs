using Domain.Entities;

namespace Domain.Models.Items.GetItems
{
    public record PageResponse(IEnumerable<Item> Result, int TotalPages);
}
