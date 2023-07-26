using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IAuctionRepository : IDisposable
    {
        Task<Auction?> GetAsync(int id);
        IEnumerable<Auction> Get(int limit = 100);
        Task<IEnumerable<Auction>> GetAsync(string name, MarketStatus status, SortOrder sortOrder, AuctionSortKey sortKey, int limit);
    }
}
