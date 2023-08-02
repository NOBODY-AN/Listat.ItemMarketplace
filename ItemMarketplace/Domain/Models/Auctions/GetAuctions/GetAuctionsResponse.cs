using Domain.Entities;
using Domain.Models.Controllers.Query;

namespace Domain.Models.Auctions.GetAuctions
{
    public class AdvancedSearchResponse
    {
        public string ItemName { get; set; }
        public int Id { get; set; }

        public int ItemId { get; set; }

        public DateTime CreatedDt { get; set; }
        public DateTime FinishedDt { get; set; }
        public decimal Price { get; set; }
        public MarketStatus Status { get; set; }
        public string Seller { get; set; }
        public string Buyer { get; set; }

        public AdvancedSearchResponse(string itemName, Auction auction)
        {
            ItemName = itemName;
            Id = auction.Id;
            ItemId = auction.ItemId;
            CreatedDt = auction.CreatedDt;
            FinishedDt = auction.FinishedDt;
            Price = auction.Price;
            Status = auction.Status;
            Seller = auction.Seller;
            Buyer = auction.Buyer;
        }

        public AdvancedSearchResponse(string itemName, int id, int itemId, DateTime createdDt, DateTime finishedDt, decimal price, MarketStatus status, string seller, string buyer)
        {
            ItemName = itemName;
            Id = id;
            ItemId = itemId;
            CreatedDt = createdDt;
            FinishedDt = finishedDt;
            Price = price;
            Status = status;
            Seller = seller;
            Buyer = buyer;
        }
    }
}
