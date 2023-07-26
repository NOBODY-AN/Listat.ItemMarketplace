using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure
{
    public class ItemRepository : IItemRepository
    {
        private bool disposed = false;
        private readonly MarketplaceContext _context;
        private readonly IMemoryCache _cache;

        public ItemRepository(MarketplaceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public Task<Item?> GetItemAsync(int id) => _cache.GetOrCreateAsync(CacheHelper.IdBuilder<Item>(id), (e) => _context.Item.FirstOrDefaultAsync(x => x.Id == id));



        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
