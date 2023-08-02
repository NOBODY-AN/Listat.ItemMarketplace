using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.Controllers;
using Domain.Models.Controllers.Response;
using Domain.Models.Items.GetItems;
using Microsoft.AspNetCore.Mvc;

namespace ItemMarketplace.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/item")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync(int id)
        {
            Item? response = await _itemRepository.GetAsync(id);
            if (response == null)
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            return Ok(new ErrorResponse<Item>(response));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(string? name = null, string? description = null, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            PageResponse response = await _itemRepository.SearchAsync(new SearchItemsPageV1Query(name, description, pageNumber));

            return Ok(new PageResponse<IEnumerable<Item>>(pageNumber > response.TotalPages ? response.TotalPages : pageNumber, response.TotalPages, response.Result));
        }

        [ApiVersion("2")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchV2Async(string searchValue, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }
            if (pageNumber < 1)
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            PageResponse response = await _itemRepository.SearchAsync(new SearchItemsPageV2Query(searchValue, pageNumber));

            return Ok(new PageResponse<IEnumerable<Item>>(pageNumber > response.TotalPages ? response.TotalPages : pageNumber, response.TotalPages, response.Result));
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateItemRequest request)
        {
            if (string.IsNullOrEmpty(request.Name)
                || string.IsNullOrEmpty(request.Description)
                || string.IsNullOrEmpty(request.Metadata))
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            int? id =await _itemRepository.CreateItemAsync(new Item(
                name: request.Name, 
                description: request.Description, 
                metadata: request.Metadata));
            if (id == null)
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            return Ok(new ErrorResponse<int>(id.Value));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateItemRequest request)
        {
            if (request.Id < 0)
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }
            if (request.Name == null && request.Description == null && request.Metadata == null)
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            bool isUpdated = await _itemRepository.UpdateItemAsync(new UpdateItemQuery(
                Id: request.Id,
                Name: request.Name, 
                Description: request.Description, 
                Metadata: request.Metadata));

            return Ok(new ErrorResponse<bool>(isUpdated));
        }
    }
}
