using Domain.Entities;

namespace Domain.Models.Auctions.GetAuctions
{
    public class PageResponse
    {
        public int TotalPages { get; set; }
        public IEnumerable<Auction> Auctions { get; set; }

        public PageResponse(int totalPages, IEnumerable<Auction> auctions)
        {
            TotalPages = totalPages;
            Auctions = auctions;
        }
    }
}
