using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Domain.Models.Controllers.Query
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuctionSortKey
    {
        [EnumMember(Value = "createdDt")]
        CreatedDt,
        [EnumMember(Value = "finishedDt")]
        FinishedDt,
        [EnumMember(Value = "price")]
        Price,
        [EnumMember(Value = "status")]
        Status,
        [EnumMember(Value = "seller")]
        Seller,
        [EnumMember(Value = "buyer")]
        Buyer
    }
}
