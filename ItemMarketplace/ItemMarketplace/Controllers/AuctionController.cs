using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Auctions.GetAuctions;
using Microsoft.AspNetCore.Mvc;

namespace ItemMarketplace.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/auction")]
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
        public async Task<IActionResult> SearchAsync(string name, MarketStatus status, [FromQuery(Name = "sort_order")] SortOrder sortOrder, [FromQuery(Name = "sort_key")] AuctionSortKey sortKey, int limit, int pageNumber = 1)
        {
            if (limit < 1 || limit > 50)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }
            if (pageNumber < 1)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }

            PageResponse response = await _auctionRepository.SearchByFirstNameAsync(new SearchByAllNamesPageQuery(name, status, sortOrder, sortKey, limit, pageNumber));

            return Ok(new PageResponse<IEnumerable<Auction>>(pageNumber > response.TotalPages ? response.TotalPages : pageNumber, response.TotalPages, response.Auctions));
        }

        [ApiVersion("2")]
        [HttpGet]
        public async Task<IActionResult> PageSearchV2Async(string name, MarketStatus status, [FromQuery(Name = "sort_order")] SortOrder sortOrder, [FromQuery(Name = "sort_key")] AuctionSortKey sortKey, int limit, int pageNumber = 1)
        {
            if (limit < 1 || limit > 50)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }
            if (pageNumber < 1)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }

            PageResponse response = await _auctionRepository.SearchByAllNamesAsync(new SearchByAllNamesPageQuery(name, status, sortOrder, sortKey, limit, pageNumber));

            return Ok(new PageResponse<IEnumerable<Auction>>(pageNumber > response.TotalPages ? response.TotalPages : pageNumber, response.TotalPages, response.Auctions));
        }

        [ApiVersion("3")]
        [HttpGet]
        public async Task<IActionResult> CursorSearchV3Async(string name, MarketStatus status, [FromQuery(Name = "sort_order")] SortOrder sortOrder, [FromQuery(Name = "sort_key")] AuctionSortKey sortKey, int limit, int cursor)
        {
            if (limit < 1 || limit > 50)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }
            if (cursor < 1)
            {
                return Ok(new ErrorResponse(Errors.AUCTION_ERROR));
            }

            CursorResponse response = await _auctionRepository.SearchByAllNamesAsync(new SearchByAllNamesCursorQuery(name, status, sortOrder, sortKey, limit, cursor));

            return Ok(new CursorResponse<IEnumerable<Auction>>(response.NextCursor, response.Result));
        }
    }
}
