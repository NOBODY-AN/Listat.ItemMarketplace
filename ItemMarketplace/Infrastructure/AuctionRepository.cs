using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

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



        public Task<Auction?> GetAsync(int id) => _context.Auction.FirstOrDefaultAsync(x => x.Id == id);

        public IEnumerable<Auction> Get(int limit = 100)
        {
            return _context.Auction.AsNoTracking().Take(limit);
        }

        public async Task<IEnumerable<Auction>> GetAsync(string name, MarketStatus status, SortOrder sortOrder, AuctionSortKey sortKey, int limit)
        {
            Item? item = await _context.Item.FirstOrDefaultAsync(x => x.Name == name);
            if (item == null)
            {
                return Enumerable.Empty<Auction>();
            }

            IQueryable<Auction> query = _context.Auction;
            query = query.AsNoTracking();

            query = query.Where(x => x.ItemId == item.Id && x.Status == status);
            query = query.Take(limit);

            query = ApplySorting(query, sortOrder, sortKey);

            return query;
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
