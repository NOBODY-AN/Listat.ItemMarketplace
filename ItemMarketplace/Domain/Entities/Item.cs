using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Metadata { get; set; }

        [JsonIgnore]
        public ICollection<Auction> Auctions { get; set; }

        public Item(int id, string name, string description, string metadata)
        {
            Id = id;
            Name = name;
            Description = description;
            Metadata = metadata;
        }
    }
}
