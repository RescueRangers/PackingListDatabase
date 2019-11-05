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
            return await _repository.Get();
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<Material> Get(int id)
        {
            return await _repository.GetById(id);
        }

        // POST: api/Materials
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Materials/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
