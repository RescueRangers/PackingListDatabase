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
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialsRepository _repository;
        private readonly ILogger<MaterialsController> _logger;

        public MaterialsController(IMaterialsRepository repository, ILogger<MaterialsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<IEnumerable<Material>> Get()
        {
            return await _repository.Get().ConfigureAwait(false);
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var material = await _repository.GetById(id).ConfigureAwait(false);

            return material == null ? NotFound() : (IActionResult)Ok(material);
        }

        // POST: api/Materials
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Material material)
        {
            var result = await _repository.Insert(material).ConfigureAwait(false);

            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // POST: api/Materials/amounts
        [HttpPost("amounts")]
        public async Task<IActionResult> PostMaterialAmount([FromBody] MaterialAmount material)
        {
            var result = await _repository.InsertMaterialAmount(material).ConfigureAwait(false);

            return result ? new StatusCodeResult(200) : new StatusCodeResult(422);
        }

        // PUT: api/Materials/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Material material)
        {
            var result = await _repository.Update(id, material).ConfigureAwait(false);

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