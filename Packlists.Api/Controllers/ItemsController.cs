using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;

namespace Packlists.Api.Controllers
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
        public async Task Post([FromBody] Item item)
        {
            await _repository.Insert(item);
        }

        // PUT: api/Items/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Item item)
        {
            await _repository.Update(item);
            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
