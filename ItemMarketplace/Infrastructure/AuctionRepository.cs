using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Domain.Models.Auctions.GetAuctions;

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

        public async Task<PageResponse> SearchByFirstNameAsync(SearchByAllNamesPageQuery searchQuery)
        {
            Item? item = await _context.Item.AsNoTracking().FirstOrDefaultAsync(x => x.Name == searchQuery.Name);
            if (item == null)
            {
                return new (0, Enumerable.Empty<Auction>());
            }

            IQueryable<Auction> query = _context.Auction;
            query = query.AsNoTracking()
                .Where(x => x.ItemId == item.Id && x.Status == searchQuery.Status);

            int countElements = await query.CountAsync();
            int totalPages = PageHelper.CalculateTotalPagesCount(countElements, searchQuery.Limit);

            query = query.ApplyAuctionOrderSortingQuery(searchQuery.SortOrder, searchQuery.SortKey)
                .Skip((searchQuery.Page > totalPages ? totalPages - 1 : searchQuery.Page - 1) * searchQuery.Limit)
                .Take(searchQuery.Limit);

            return new PageResponse(totalPages, await query.ToListAsync());
        }

        public async Task<PageResponse> SearchByAllNamesAsync(SearchByAllNamesPageQuery searchQuery)
        {
            IQueryable<Item> result = _context.Item
                .AsNoTracking()
                .BuildSearchTextQuery((q, s) => q.Where(x => x.Name.Contains(s)), searchQuery.Name);

            IQueryable<Auction> query = result.Join(_context.Auction,
                item => item.Id,
                auction => auction.ItemId,
                (item, auction) => auction).Where(x => x.Status == searchQuery.Status);

            int countElements = await query.CountAsync();
            int totalPages = PageHelper.CalculateTotalPagesCount(countElements, searchQuery.Limit);

            query = query.ApplyAuctionOrderSortingQuery(searchQuery.SortOrder, searchQuery.SortKey)
                .Skip((searchQuery.Page > totalPages ? totalPages - 1 : searchQuery.Page - 1) * searchQuery.Limit)
                .Take(searchQuery.Limit);

            return new PageResponse(totalPages, await query.ToListAsync());
        }

        public async Task<CursorResponse> SearchByAllNamesAsync(SearchByAllNamesCursorQuery searchQuery)
        {
            IQueryable<Item> result = _context.Item
                .AsNoTracking()
                .BuildSearchTextQuery((q, s) => q.Where(x => x.Name.Contains(s)), searchQuery.Name);

            var query = result.Join(_context.Auction,
                item => item.Id,
                auction => auction.ItemId,
                (item, auction) => auction)
                .Where(x => x.Status == searchQuery.Status)
                .ApplyAuctionOrderSortingQuery(searchQuery.SortOrder, searchQuery.SortKey);

            query = searchQuery.SortOrder switch
            {
                SortOrder.ASC => query.Where(x => x.Id >= searchQuery.Cursor),
                SortOrder.DESC => query.Where(x => x.Id <= searchQuery.Cursor),
                _ => throw new NotImplementedException(),
            };
            IReadOnlyList<Auction> auctions = await query.Take(searchQuery.Limit + 1).ToListAsync();
            if (auctions.Count < 1)
            {
                return new CursorResponse(0, Enumerable.Empty<Auction>());
            }
            int lastId = auctions[auctions.Count - 1].Id;

            IEnumerable<Auction> auctionsResponse = auctions.Take(searchQuery.Limit);
            return new CursorResponse(lastId, auctions);
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
