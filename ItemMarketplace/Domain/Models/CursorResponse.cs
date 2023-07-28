using Newtonsoft.Json;

namespace Domain.Models
{
    public class CursorResponse<T> : ErrorResponse<T>
    {
        [JsonProperty("nextCursor")]
        public int NextCursor { get; set; }

        public CursorResponse(int nextCursor, T result) : base(result)
        {
            NextCursor = nextCursor;
        }
    }
}
