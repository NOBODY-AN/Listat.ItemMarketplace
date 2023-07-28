using Domain.Entities;
using Domain.Models;
using Domain.Models.Auctions.GetAuctions;

namespace Domain.Interfaces
{
    public interface IAuctionRepository : IDisposable
    {
        Task<Auction?> GetAsync(int id);
        IEnumerable<Auction> Get(int limit = 100);
        Task<PageResponse> SearchByFirstNameAsync(SearchByAllNamesPageQuery searchQuery);
        Task<PageResponse> SearchByAllNamesAsync(SearchByAllNamesPageQuery searchQuery);
        Task<CursorResponse> SearchByAllNamesAsync(SearchByAllNamesCursorQuery searchQuery);
    }
}
