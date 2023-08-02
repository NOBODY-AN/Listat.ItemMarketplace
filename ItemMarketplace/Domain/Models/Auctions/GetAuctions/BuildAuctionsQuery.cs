using Domain.Entities;
using Domain.Models.Controllers.Query;
using System.Linq.Expressions;

namespace Domain.Models.Auctions.GetAuctions
{
    public static class BuildAuctionsQuery
    {
        public static IQueryable<TItem> ApplyOrderSortingQuery<TItem, TKey>(this IQueryable<TItem> query, SortOrder sortOrder, Expression<Func<TItem, TKey>> func) => sortOrder switch
        {
            SortOrder.ASC => query.OrderBy(func),
            SortOrder.DESC => query.OrderByDescending(func),
            _ => throw new NotImplementedException(),
        };

        public static IQueryable<TItem> ApplyAuctionOrderSortingQuery<TItem>(this IQueryable<TItem> query, SortOrder sortOrder, AuctionSortKey sortKey) where TItem : Auction => sortOrder switch
        {
            SortOrder.ASC => sortKey switch
            {
                AuctionSortKey.CreatedDt => query.OrderBy(x => x.CreatedDt),
                AuctionSortKey.Price => query.OrderBy(x => x.Price),
                _ => throw new NotImplementedException()
            },
            SortOrder.DESC => sortKey switch
            {
                AuctionSortKey.CreatedDt => query.OrderByDescending(x => x.CreatedDt),
                AuctionSortKey.Price => query.OrderByDescending(x => x.Price),
                _ => throw new NotImplementedException()
            },
            _ => throw new NotImplementedException(),
        };
    }
}
