using Domain.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Auction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ItemId { get; set; }
        
        [JsonIgnore]
        public Item? Item { get; set; }

        public DateTime CreatedDt { get; set; }
        public DateTime FinishedDt { get; set; }
        public decimal Price { get; set; }
        public MarketStatus Status { get; set; }
        public string Seller { get; set; }
        public string Buyer { get; set; }

        public Auction(int id, int itemId, DateTime createdDt, DateTime finishedDt, decimal price, MarketStatus status, string seller, string buyer)
        {
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
