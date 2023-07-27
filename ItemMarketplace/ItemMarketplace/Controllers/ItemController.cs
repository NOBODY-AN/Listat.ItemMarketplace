using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.Controllers;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(string? name = null, string? description = null)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
            {
                return BadRequest();
            }

            IEnumerable<Item> response = await _itemRepository.SearchAsync(name, description);

            return Ok(response);
        }

        [HttpGet("~/api/v2/item/search")]
        public async Task<IActionResult> SearchAsync(string searchValue, int page)
        {
            IEnumerable<Item> response = await _itemRepository.SearchAsync(searchValue, page);

            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateItemRequest request)
        {
            await _itemRepository.CreateItemAsync(new Item(
                name: request.Name, 
                description: request.Description, 
                metadata: request.Metadata));

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateItemRequest request)
        {
            await _itemRepository.UpdateItemAsync(new Item(
                id: request.Id,
                name: request.Name, 
                description: request.Description, 
                metadata: request.Metadata));

            return Ok();
        }
    }
}
