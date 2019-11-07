using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packlists.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CocsController : ControllerBase
    {
        private readonly ICocRepository _repository;
        private readonly ILogger<CocsController> _logger;

        public CocsController(ICocRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Imports
        [HttpGet]
        public async Task<IEnumerable<COC>> Get()
        {
            return await _repository.Get().ConfigureAwait(false);
        }

        // GET: api/Imports/5
        [HttpGet("{id}")]
        public async Task<COC> Get(int id)
        {
            return await _repository.GetById(id).ConfigureAwait(false);
        }

        // POST: api/Imports
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] COC coc)
        {
            var result = await _repository.Insert(coc).ConfigureAwait(false);
            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // PUT: api/Imports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] COC coc)
        {
            var result = await _repository.Update(id, coc).ConfigureAwait(false);
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
