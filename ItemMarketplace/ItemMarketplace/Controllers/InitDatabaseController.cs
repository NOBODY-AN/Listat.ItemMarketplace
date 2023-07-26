using Domain.Entities;
using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ItemMarketplace.Controllers
{
    [Route("api/v1/init")]
    [ApiController]
    public class InitDatabaseController : ControllerBase, IDisposable
    {
        private readonly MarketplaceContext _marketplaceContext;

        public InitDatabaseController(MarketplaceContext marketplaceContext)
        {
            _marketplaceContext = marketplaceContext;
        }

        [HttpPost]
        public async Task InitAsync()
        {
            int auctionsCount = 10_000;
            Auction[] auctions = new Auction[auctionsCount];


            Random random = new();


            for (int i = 0, j = 0; i < 100; i++)
            {
                await _marketplaceContext.Item.AddAsync(BuildItem(i));

                _ = await _marketplaceContext.SaveChangesAsync();


                for (int a = 0; a < auctionsCount; a++, j++)
                {
                    auctions[a] = BuildAuction(
                        id: j, 
                        itemId: i,
                        seconds: random.Next(1658629448/*Sun Jul 24 2022 02:24:08 GMT+0000*/, 1690165448/*Mon Jul 24 2023 02:24:08 GMT+0000*/),
                        price: random.Next(1, 777),
                        status: random.Next(0, 3));
                }

                _marketplaceContext.Auction.BulkInsert(auctions);
            }
        }

        private static Item BuildItem(int id)
        {
            StringBuilder sb = new();

            return new Item(
                id: id,
                name: sb.Append("Some item").Append(' ').Append(id).ToString(),
                description: sb.Append(' ').Append("small description for it").ToString(),
                metadata: Guid.NewGuid().ToString("n"));
        }

        private static Auction BuildAuction(int id, int itemId, long seconds, int price, int status)
        {
            return new Auction(
                id: id,
                itemId: itemId,
                createdDt: new DateTime(DateTimeOffset.FromUnixTimeSeconds(seconds).Ticks),
                finishedDt: new DateTime(DateTimeOffset.FromUnixTimeSeconds(seconds).AddMinutes(777).Ticks),
                price: price,
                status: (MarketStatus)status,
                seller: Guid.NewGuid().ToString("n"),
                buyer: Guid.NewGuid().ToString("n"));
        }

        public void Dispose()
        {
            _marketplaceContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
