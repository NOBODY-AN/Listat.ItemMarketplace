using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Models.Items.GetItems;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Cache
{
    public class ItemCachedRepository : IItemRepository
    {
        private readonly ItemRepository _repository;
        private readonly IMemoryCache _cache;

        public ItemCachedRepository(ItemRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public Task<int?> CreateItemAsync(Item item) => _repository.CreateItemAsync(item);

        public Task<Item?> GetAsync(int id) => 
            _cache.GetOrCreateAsync(CacheHelper.IdBuilder<Item>(id), (e) =>
            {
                e.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return _repository.GetAsync(id);
            });

        public Task<PageResponse> SearchAsync(SearchItemsPageV1Query searchQuery) => _repository.SearchAsync(searchQuery);

        public Task<PageResponse> SearchAsync(SearchItemsPageV2Query searchQuery) => _repository.SearchAsync(searchQuery);

        public async Task<bool> UpdateItemAsync(UpdateItemQuery query)
        {
            bool isSuccess = await _repository.UpdateItemAsync(query);

            if (isSuccess)
            {
                _cache.Remove(CacheHelper.IdBuilder<Item>(query.Id));
            }

            return isSuccess;
        }

        public void Dispose() => _repository.Dispose();
    }
}
