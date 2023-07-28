using Domain.Entities;

namespace Domain.Models.Auctions.GetAuctions
{
    public class CursorResponse
    {
        public int NextCursor { get; set; }
        public IEnumerable<Auction> Result { get; set; }

        public CursorResponse(int nextCursor, IEnumerable<Auction> result)
        {
            NextCursor = nextCursor;
            Result = result;
        }
    }
}
