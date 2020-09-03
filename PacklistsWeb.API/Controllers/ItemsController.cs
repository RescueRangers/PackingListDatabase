using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packilists.Shared.Data;
using PacklistsWeb.API.Repositories.Interfaces;

namespace PacklistsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<IEnumerable<Item>> Get()
        {
            return await _repository.Get();
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _repository.GetById(id).ConfigureAwait(false);
            return item == null ? NotFound() : (IActionResult)Ok(item);
        }

        // POST: api/Items
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Item item)
        {
            var result = await _repository.Insert(item).ConfigureAwait(false);
            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Item item)
        {
            var result = await _repository.Update(id, item).ConfigureAwait(false);
            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repository.Delete(id).ConfigureAwait(false);
            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }
    }
}