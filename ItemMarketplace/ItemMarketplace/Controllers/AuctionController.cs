using Azure;
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
            if (id < 0)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }

            Auction? response = await _auctionRepository.GetAsync(id);

            if (response == null)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }

            return Ok(new ErrorResponse<Auction>(response));
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string name, MarketStatus status, [FromQuery(Name = "sort_order")] SortOrder sortOrder, [FromQuery(Name = "sort_key")] AuctionSortKey sortKey, int limit, int page)
        {
            if (limit < 1 || limit > 50)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }
            if (page < 1)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }

            IEnumerable<Auction> response = await _auctionRepository.GetAsync(name, status, sortOrder, sortKey, limit, page);

            return Ok(new ErrorResponse<IEnumerable<Auction>>(response));
        }
    }
}
