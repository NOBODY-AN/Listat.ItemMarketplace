using Domain.Interfaces;
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
            var response = await _itemRepository.GetItemAsync(id);

            return Ok(response);
        }
    }
}
