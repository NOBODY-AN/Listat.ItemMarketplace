using Newtonsoft.Json;

namespace Domain.Models.Controllers
{
    public class UpdateItemRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("metadata")]
        public string? Metadata { get; set; }

        [JsonConstructor]
        public UpdateItemRequest(int id)
        {
            Id = id;
        }
    }
}
