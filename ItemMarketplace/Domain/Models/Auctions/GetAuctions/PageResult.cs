using Domain.Entities;

namespace Domain.Models.Auctions.GetAuctions
{
    public class PageResult
    {
        public int TotalPages { get; set; }
        public IEnumerable<Auction> Auctions { get; set; }

        public PageResult(int totalPages, IEnumerable<Auction> auctions)
        {
            TotalPages = totalPages;
            Auctions = auctions;
        }
    }

    public class PageResult<T>
    {
        public int TotalPages { get; set; }
        public IEnumerable<T> Auctions { get; set; }

        public PageResult(int totalPages, IEnumerable<T> auctions)
        {
            TotalPages = totalPages;
            Auctions = auctions;
        }
    }
}
