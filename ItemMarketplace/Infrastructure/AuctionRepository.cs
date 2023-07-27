using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure
{
    public class AuctionRepository : IAuctionRepository
    {
        private bool disposed = false;
        private readonly MarketplaceContext _context;
        private readonly IMemoryCache _cache;

        public AuctionRepository(MarketplaceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }


        public Task<Auction?> GetAsync(int id) => _cache.GetOrCreateAsync(CacheHelper.IdBuilder<Auction>(id), (e) => _context.Auction.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id));

        public IEnumerable<Auction> Get(int limit = 100)
        {
            return _context.Auction.AsNoTracking().Take(limit);
        }

        public async Task<(IEnumerable<Auction>, int totalPages)> GetAsync(string name, MarketStatus status, SortOrder sortOrder, AuctionSortKey sortKey, int limit, int page)
        {
            Item? item = await _context.Item.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
            if (item == null)
            {
                return (Enumerable.Empty<Auction>(), 0);
            }

            IQueryable<Auction> query = _context.Auction;
            query = query.AsNoTracking()
                .Where(x => x.ItemId == item.Id && x.Status == status);

            int countElements = await ApplySorting(query, sortOrder, sortKey).CountAsync();
            int totalPages = PageHelper.CalculateTotalPagesCount(countElements, limit);

            query = ApplySorting(query, sortOrder, sortKey)
                .Skip((page > totalPages ? totalPages - 1 : page - 1) * limit)
                .Take(limit);

            return (await query.ToListAsync(), totalPages);
        }

        private static IQueryable<Auction> ApplySorting(IQueryable<Auction> query, SortOrder sortOrder, AuctionSortKey sortKey) => sortOrder switch
        {
            SortOrder.ASC => sortKey switch
            {
                AuctionSortKey.CreatedDt => query.OrderBy(x => x.CreatedDt),
                AuctionSortKey.FinishedDt => query.OrderBy(x => x.FinishedDt),
                AuctionSortKey.Price => query.OrderBy(x => x.Price),
                AuctionSortKey.Status => query.OrderBy(x => x.Status),
                AuctionSortKey.Seller => query.OrderBy(x => x.Seller),
                AuctionSortKey.Buyer => query.OrderBy(x => x.Buyer),
                _ => throw new NotImplementedException()
            },
            SortOrder.DESC => sortKey switch
            {
                AuctionSortKey.CreatedDt => query.OrderByDescending(x => x.CreatedDt),
                AuctionSortKey.FinishedDt => query.OrderByDescending(x => x.FinishedDt),
                AuctionSortKey.Price => query.OrderByDescending(x => x.Price),
                AuctionSortKey.Status => query.OrderByDescending(x => x.Status),
                AuctionSortKey.Seller => query.OrderByDescending(x => x.Seller),
                AuctionSortKey.Buyer => query.OrderByDescending(x => x.Buyer),
                _ => throw new NotImplementedException()
            },
            _ => throw new NotImplementedException(),
        };




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
