using Newtonsoft.Json;

namespace Domain.Models.Controllers
{
    public class CreateItemRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        [JsonConstructor]
        public CreateItemRequest(string name, string description, string metadata)
        {
            Name = name;
            Description = description;
            Metadata = metadata;
        }
    }
}
