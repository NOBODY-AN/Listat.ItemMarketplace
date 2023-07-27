using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ItemMarketplace.Controllers
{
    [Route("api/v1/item")]
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
        public async Task<IActionResult> SearchAsync(string? name = null, string? description = null)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            IEnumerable<Item> response = await _itemRepository.SearchAsync(name, description);

            return Ok(new ErrorResponse<IEnumerable<Item>>(response));
        }

        [HttpGet("~/api/v2/item/search")]
        public async Task<IActionResult> SearchAsync(string searchValue, int page)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }
            if (page < 1)
            {
                return Ok(new ErrorResponse(Errors.ITEM_ERROR));
            }

            IEnumerable<Item> response = await _itemRepository.SearchAsync(searchValue, page);

            return Ok(new ErrorResponse<IEnumerable<Item>>(response));
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

            bool isUpdated = await _itemRepository.UpdateItemAsync(new Item(
                id: request.Id,
                name: request.Name, 
                description: request.Description, 
                metadata: request.Metadata));

            return Ok(new ErrorResponse<bool>(isUpdated));
        }
    }
}
