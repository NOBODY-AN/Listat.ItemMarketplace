using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Domain.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketStatus
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "canceled")]
        Canceled,
        [EnumMember(Value = "finished")]
        Finished,
        [EnumMember(Value = "active")]
        Active
    }
}
