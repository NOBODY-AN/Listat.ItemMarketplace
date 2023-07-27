using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IItemRepository : IDisposable
    {
        Task<Item?> GetAsync(int id);
        Task<(IEnumerable<Item> result, int totalPages)> SearchAsync(string? name, string? description, int pageNumber);
        Task<(IEnumerable<Item> result, int totalPages)> SearchAsync(string searchValue, int pageNumber);
        Task<int?> CreateItemAsync(Item item);
        Task<bool> UpdateItemAsync(Item item);
    }
}
