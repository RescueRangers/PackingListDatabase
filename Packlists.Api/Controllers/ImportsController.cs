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
    public class ImportsController : ControllerBase
    {
        private readonly IImportsRepository _repository;
        private readonly ILogger<ImportsController> _logger;

        public ImportsController(IImportsRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Imports
        [HttpGet]
        public async Task<IEnumerable<ImportTransport>> Get()
        {
            return await _repository.Get().ConfigureAwait(false);
        }

        // GET: api/Imports/5
        [HttpGet("{id}")]
        public async Task<ImportTransport> Get(int id)
        {
            return await _repository.GetById(id).ConfigureAwait(false);
        }

        // POST: api/Imports
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ImportTransport import)
        {
            var result = await _repository.Insert(import).ConfigureAwait(false);
            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // PUT: api/Imports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ImportTransport import)
        {
            var result = await _repository.Update(id, import).ConfigureAwait(false);
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
