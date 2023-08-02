using Domain.Entities;

namespace Domain.Models.Auctions.GetAuctions
{
    public class CursorResult
    {
        public int NextCursor { get; set; }
        public IEnumerable<Auction> Result { get; set; }

        public CursorResult(int nextCursor, IEnumerable<Auction> result)
        {
            NextCursor = nextCursor;
            Result = result;
        }
    }
}
