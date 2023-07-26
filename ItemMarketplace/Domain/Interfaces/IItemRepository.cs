using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IItemRepository : IDisposable
    {
        Task<Item?> GetItemAsync(int id);
    }
}
