using Domain.Entities;

namespace Domain.Models.Auctions.GetAuctions
{
    public static class OrderSortingQuery
    {
        public static IQueryable<Auction> ApplyAuctionOrderSortingQuery(this IQueryable<Auction> query, SortOrder sortOrder, AuctionSortKey sortKey) => sortOrder switch
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

        public static IQueryable<TItem> BuildSearchTextQuery<TItem>(this IQueryable<TItem> query, Func<IQueryable<TItem>, string, IQueryable<TItem>> expression, string searchString)
        {
            string[] searchTerms = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in searchTerms)
            {
                query = expression(query, item);
            }

            return query;
        }
    }
}
