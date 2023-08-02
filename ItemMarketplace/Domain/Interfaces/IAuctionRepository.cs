using Domain.Entities;
using Domain.Models.Auctions.GetAuctions;

namespace Domain.Interfaces
{
    public interface IAuctionRepository : IDisposable
    {
        Task<Auction?> GetAsync(int id);
        IEnumerable<Auction> Get(int limit = 100);
        Task<PageResult> SearchByFirstNameAsync(SearchByAllNamesPageQuery searchQuery);
        Task<PageResult> SearchByAllNamesAsync(SearchByAllNamesPageQuery searchQuery);
        Task<CursorResult> SearchByAllNamesAsync(SearchByAllNamesCursorQuery searchQuery);
        Task<PageResult<AdvancedSearchResponse>> AdvancedSearchAsync(AdvancedSearchPageQuery searchQuery);
    }
}
