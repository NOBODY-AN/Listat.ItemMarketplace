using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItemMarketplace.Controllers
{
    [Route("api/v1/auction")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;

        public AuctionController(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return Ok(await _auctionRepository.GetAsync(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string name, MarketStatus status, [FromQuery(Name = "sort_order")] SortOrder sortOrder, [FromQuery(Name = "sort_key")] AuctionSortKey sortKey, int limit, int page)
        {
            IEnumerable<Auction> result = await _auctionRepository.GetAsync(name, status, sortOrder, sortKey, limit, page);
            return Ok(result);
        }
    }
}
