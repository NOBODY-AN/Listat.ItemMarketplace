using Newtonsoft.Json;

namespace Domain.Models.Controllers.Response
{
    public class PageResponse<T> : ErrorResponse<T>
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("totalPagesCount")]
        public int TotalPagesCount { get; set; }

        public PageResponse(int currentPage, int totalPagesCount, T result) : base(result)
        {
            CurrentPage = currentPage;
            TotalPagesCount = totalPagesCount;
        }
    }
}
