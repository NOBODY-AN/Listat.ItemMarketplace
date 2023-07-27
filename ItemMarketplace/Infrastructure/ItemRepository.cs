using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure
{
    public class ItemRepository : IItemRepository
    {
        private bool disposed = false;
        protected int maxItemsPerPage = 10;
        private readonly MarketplaceContext _context;
        private readonly IMemoryCache _cache;

        public ItemRepository(MarketplaceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public Task<Item?> GetAsync(int id) => _cache.GetOrCreateAsync(CacheHelper.IdBuilder<Item>(id), (e) => _context.Item.FirstOrDefaultAsync(x => x.Id == id));

        public async Task<int?> CreateItemAsync(Item item)
        {
            EntityEntry<Item> entity = await _context.Item.AddAsync(item);

            await _context.SaveChangesAsync();

            return entity.IsKeySet ? entity.Entity.Id : null;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            _context.Item.Attach(item);

            if (!string.IsNullOrEmpty(item.Name))
                _context.Entry(item).Property(p => p.Name).IsModified = true;
            if (!string.IsNullOrEmpty(item.Description))
                _context.Entry(item).Property(p => p.Description).IsModified = true;
            if (!string.IsNullOrEmpty(item.Metadata))
                _context.Entry(item).Property(p => p.Metadata).IsModified = true;

            int numberWritenEntities = await _context.SaveChangesAsync();
            if (numberWritenEntities > 0)
            {
                _cache.Remove(CacheHelper.IdBuilder<Item>(item.Id));
            }

            return numberWritenEntities > 0;
        }

        public async Task<IEnumerable<Item>> SearchAsync(string? name, string? description)
        {
            IQueryable<Item> query = _context.Item;
            query = query.AsNoTracking();
            if (!string.IsNullOrEmpty(name))
            {
                query = BuildSearchQuery(query, (q, s) => q.Where(x => x.Name.Contains(s)), name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                query = BuildSearchQuery(query, (q, s) => q.Where(x => x.Description.Contains(s)), description);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Item>> SearchAsync(string searchValue, int pageNumber)
        {
            IQueryable<Item> query = _context.Item;
            query = query.AsNoTracking();
            query = BuildSearchQuery(query,
                (q, s) => q.Where(
                    x => x.Name.Contains(s)
                        || x.Description.Contains(s)
                        || x.Metadata.Contains(s)), searchValue);
            query = query.Skip((pageNumber - 1) * maxItemsPerPage).Take(maxItemsPerPage);

            return await query.ToListAsync();
        }

        private static IQueryable<TItem> BuildSearchQuery<TItem>(IQueryable<TItem> query, Func<IQueryable<TItem>, string, IQueryable<TItem>> expression, string searchString)
        {
            string[] searchTerms = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); 
            foreach (string item in searchTerms)
            {
                query = expression(query, item);
            }

            return query;
        }


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
