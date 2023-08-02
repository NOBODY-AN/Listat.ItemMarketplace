using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Auctions.GetAuctions;
using Domain.Extentions;
using Domain.Models.Controllers.Query;

namespace Infrastructure
{
    public class AuctionRepository : IAuctionRepository
    {
        private bool disposed = false;
        private readonly MarketplaceContext _context;

        public AuctionRepository(MarketplaceContext context)
        {
            _context = context;
        }


        public Task<Auction?> GetAsync(int id) => _context.Auction.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public IEnumerable<Auction> Get(int limit = 100)
        {
            return _context.Auction.AsNoTracking().Take(limit);
        }

        public async Task<PageResult> SearchByFirstNameAsync(SearchByAllNamesPageQuery searchQuery)
        {
            Item? item = await _context.Item.AsNoTracking().FirstOrDefaultAsync(x => x.Name == searchQuery.Name);
            if (item == null)
            {
                return new (0, Enumerable.Empty<Auction>());
            }

            IQueryable<Auction> query = _context.Auction;
            (query, int totalCount) = await query.AsNoTracking()
                .Where(x => x.ItemId == item.Id && x.Status == searchQuery.Status)
                .ApplyAuctionOrderSortingQuery(searchQuery.SortOrder, searchQuery.SortKey)
                .BuildPageAsync(searchQuery.Page, searchQuery.Limit);

            return new PageResult(totalCount, await query.ToListAsync());
        }

        public async Task<PageResult> SearchByAllNamesAsync(SearchByAllNamesPageQuery searchQuery)
        {
            IQueryable<Item> result = _context.Item
                .AsNoTracking()
                .BuildSearchTextQuery((q, s) => q.Where(x => x.Name.Contains(s)), searchQuery.Name);

            IQueryable<Auction> query = _context.Auction.AsNoTracking();
            (query, int totalCount) = await result.Join(query,
                item => item.Id,
                auction => auction.ItemId,
                (item, auction) => auction)
                .Where(x => x.Status == searchQuery.Status)
                .ApplyAuctionOrderSortingQuery(searchQuery.SortOrder, searchQuery.SortKey)
                .BuildPageAsync(searchQuery.Page, searchQuery.Limit);

            return new PageResult(totalCount, await query.ToListAsync());
        }

        public async Task<CursorResult> SearchByAllNamesAsync(SearchByAllNamesCursorQuery searchQuery)
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
                Domain.Models.Controllers.Query.SortOrder.ASC => query.Where(x => x.Id >= searchQuery.Cursor),
                Domain.Models.Controllers.Query.SortOrder.DESC => query.Where(x => x.Id <= searchQuery.Cursor),
                _ => throw new NotImplementedException(),
            };
            IReadOnlyList<Auction> auctions = await query.Take(searchQuery.Limit + 1).ToListAsync();
            if (auctions.Count < 1)
            {
                return new CursorResult(0, Enumerable.Empty<Auction>());
            }
            int lastId = auctions[auctions.Count - 1].Id;

            IEnumerable<Auction> auctionsResponse = auctions.Take(searchQuery.Limit);
            return new CursorResult(lastId, auctions);
        }

        public async Task<PageResult<AdvancedSearchResponse>> AdvancedSearchAsync(AdvancedSearchPageQuery searchQuery)
        {
            IQueryable<Item> itemQuery = _context.Item
                .AsNoTracking();
            if (!string.IsNullOrEmpty(searchQuery.Name))
            {
                itemQuery = itemQuery.BuildSearchTextQuery((q, s) => q.Where(x => x.Name.Contains(s)), searchQuery.Name);
            }

            IQueryable<Auction> auctionQuery = _context.Auction.AsNoTracking();
            if (searchQuery.Status.HasValue)
            {
                auctionQuery = auctionQuery.Where(x => x.Status == searchQuery.Status);
            }
            if (!string.IsNullOrEmpty(searchQuery.SellerName))
            {
                auctionQuery = auctionQuery.Where(x => x.Seller == searchQuery.SellerName);
            }

            var itemActionJoin = itemQuery.Join(auctionQuery,
                item => item.Id,
                auction => auction.ItemId,
                (item, auction) => new
                {
                    item.Name,
                    auction.Id,
                    auction.ItemId,
                    auction.CreatedDt,
                    auction.FinishedDt,
                    auction.Price,
                    auction.Status,
                    auction.Seller,
                    auction.Buyer
                });

            itemActionJoin = searchQuery.SortKey switch
            {
                AuctionSortKey.CreatedDt => itemActionJoin.ApplyOrderSortingQuery(searchQuery.SortOrder, x => x.CreatedDt),
                AuctionSortKey.Price => itemActionJoin.ApplyOrderSortingQuery(searchQuery.SortOrder, x => x.Price),
                _ => throw new NotImplementedException()
            };
            (itemActionJoin, int totalCount) = await itemActionJoin.BuildPageAsync(searchQuery.Page, searchQuery.Limit);


            return new PageResult<AdvancedSearchResponse>(totalCount, await itemActionJoin
                .Select(x => new AdvancedSearchResponse(x.Name, x.Id, x.ItemId, x.CreatedDt, x.FinishedDt, x.Price, x.Status, x.Seller, x.Buyer)).ToListAsync());
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
