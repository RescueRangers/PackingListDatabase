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
    public class PackingListsController : ControllerBase
    {
        private readonly IPackingListsRepository _repository;
        private readonly ILogger<PackingListsController> _logger;

        public PackingListsController(IPackingListsRepository repository, ILogger<PackingListsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/PackingLists
        [HttpGet]
        public async Task<IEnumerable<Packliste>> Get()
        {
            return await _repository.Get().ConfigureAwait(false);
        }

        // GET: api/PackingLists/5
        [HttpGet("{id}")]
        public async Task<Packliste> Get(int id)
        {
            return await _repository.GetById(id).ConfigureAwait(false);
        }

        // GET: api/PackingLists/ByDate/2019-09-09
        [HttpGet("ByDate/{date}")]
        public async Task<IEnumerable<Packliste>> GetByMonth(DateTime date)
        {
            return await _repository.GetByMonth(date).ConfigureAwait(false);
        }

        // GET: api/PackingLists/PacklisteData/5
        [HttpGet("PacklisteData/{packListeId}")]
        public async Task<IEnumerable<PacklisteData>> GetPacklisteData(int packListeId)
        {
            return await _repository.GetPacklisteData(packListeId).ConfigureAwait(false);
        }

        // POST: api/PackingLists
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Packliste packingList)
        {
            var result = await _repository.Insert(packingList).ConfigureAwait(false);
            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // POST: api/PackingLists/PacklisteData
        [HttpPost("PacklisteData/{packListeId}")]
        public async Task Post(int packListeId, [FromBody] List<PacklisteData> data)
        {
            await _repository.InsertPacklisteData(packListeId, data).ConfigureAwait(false);
        }

        // PUT: api/PackingLists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Packliste packingList)
        {
            var result = await _repository.Update(id, packingList).ConfigureAwait(false);
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
