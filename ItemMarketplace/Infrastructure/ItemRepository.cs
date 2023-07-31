using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Models.Items.GetItems;
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

        public ItemRepository(MarketplaceContext context)
        {
            _context = context;
        }

        public Task<Item?> GetAsync(int id) => _context.Item.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<int?> CreateItemAsync(Item item)
        {
            EntityEntry<Item> entity = await _context.Item.AddAsync(item);

            await _context.SaveChangesAsync();

            return entity.IsKeySet ? entity.Entity.Id : null;
        }

        public async Task<bool> UpdateItemAsync(UpdateItemQuery query)
        {
            EntityEntry<Item> item = _context.Item.Attach(new Item(query.Id));

            if (!string.IsNullOrEmpty(query.Name))
                item.Entity.Name = query.Name;
            if (!string.IsNullOrEmpty(query.Description))
                item.Entity.Description = query.Description;
            if (!string.IsNullOrEmpty(query.Metadata))
                item.Entity.Metadata = query.Metadata;

            int numberWritenEntities = await _context.SaveChangesAsync();

            return numberWritenEntities > 0;
        }

        public async Task<PageResponse> SearchAsync(SearchItemsPageV1Query searchQuery)
        {
            IQueryable<Item> query = _context.Item;
            query = query.AsNoTracking();
            if (!string.IsNullOrEmpty(searchQuery.Name))
            {
                query = BuildSearchQuery(query, (q, s) => q.Where(x => x.Name.Contains(s)), searchQuery.Name);
            }
            if (!string.IsNullOrEmpty(searchQuery.Description))
            {
                query = BuildSearchQuery(query, (q, s) => q.Where(x => x.Description.Contains(s)), searchQuery.Description);
            }

            int totalPages = PageHelper.CalculateTotalPagesCount(await query.CountAsync(), maxItemsPerPage);

            query = query.Skip((searchQuery.PageNumber > totalPages ? totalPages - 1 : searchQuery.PageNumber - 1) * maxItemsPerPage)
                .Take(maxItemsPerPage);

            return new(await query.ToListAsync(), totalPages);
        }

        public async Task<PageResponse> SearchAsync(SearchItemsPageV2Query searchQuery)
        {
            IQueryable<Item> query = _context.Item;
            query = query.AsNoTracking();

            IQueryable<Item> Expression(IQueryable<Item> query) => BuildSearchQuery(query,
                (q, s) => q.Where(
                    x => x.Name.Contains(s)
                        || x.Description.Contains(s)
                        || x.Metadata.Contains(s)), searchQuery.SearchValue);

            int countElements = await Expression(query).CountAsync();
            if (countElements < 1)
            {
                return new(Enumerable.Empty<Item>(), 0);
            }

            int totalPages = PageHelper.CalculateTotalPagesCount(countElements, maxItemsPerPage);


            query = Expression(query)
                .OrderBy(x => x.Id)
                .Skip((searchQuery.PageNumber > totalPages ? totalPages - 1 : searchQuery.PageNumber - 1) * maxItemsPerPage)
                .Take(maxItemsPerPage);

            return new(await query.ToListAsync(), totalPages);
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
