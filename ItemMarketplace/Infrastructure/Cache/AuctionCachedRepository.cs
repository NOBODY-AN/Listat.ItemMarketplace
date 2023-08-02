using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Models.Auctions.GetAuctions;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Cache
{
    public class AuctionCachedRepository : IAuctionRepository
    {
        private readonly AuctionRepository _repository;
        private readonly IMemoryCache _cache;

        public AuctionCachedRepository(AuctionRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<Auction> Get(int limit = 100) => _repository.Get(limit);

        public Task<Auction?> GetAsync(int id) => 
            _cache.GetOrCreateAsync(CacheHelper.IdBuilder<Auction>(id), (e) =>
            {
                e.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return _repository.GetAsync(id);
            });

        public Task<PageResult> SearchByAllNamesAsync(SearchByAllNamesPageQuery searchQuery) => _repository.SearchByAllNamesAsync(searchQuery);

        public Task<CursorResult> SearchByAllNamesAsync(SearchByAllNamesCursorQuery searchQuery) => _repository.SearchByAllNamesAsync(searchQuery);

        public Task<PageResult> SearchByFirstNameAsync(SearchByAllNamesPageQuery searchQuery) => _repository.SearchByFirstNameAsync(searchQuery);

        public Task<PageResult<AdvancedSearchResponse>> AdvancedSearchAsync(AdvancedSearchPageQuery searchQuery) => _repository.AdvancedSearchAsync(searchQuery);

        public void Dispose() => _repository.Dispose();
    }
}
