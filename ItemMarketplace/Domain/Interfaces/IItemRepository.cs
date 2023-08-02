using Domain.Entities;
using Domain.Models.Items.GetItems;

namespace Domain.Interfaces
{
    public interface IItemRepository : IDisposable
    {
        Task<Item?> GetAsync(int id);
        Task<PageResponse> SearchAsync(SearchItemsPageV1Query searchQuery);
        Task<PageResponse> SearchAsync(SearchItemsPageV2Query searchQuery);
        Task<int?> CreateItemAsync(Item item);
        Task<bool> UpdateItemAsync(UpdateItemQuery query);
    }
}
