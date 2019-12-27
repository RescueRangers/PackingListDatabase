using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Packlists.Api.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packlists.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly ITestingRepository _repository;
        private readonly ILogger<CocsController> _logger;

        public TestingController(ITestingRepository repository, ILogger<CocsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/testing/multiquery/2019-09-01
        [HttpGet("multiquery/{date}")]
        public async Task<TimeSpan> GetMultiQuery(DateTime date)
        {
            return await _repository.BenchMultipleQuerries(date).ConfigureAwait(false);
        }

        // GET: api/testing/mapping/2019-09-01
        [HttpGet("mapping/{date}")]
        public async Task<TimeSpan> GetMapping(DateTime date)
        {
            return await _repository.BenchMultiMapping(date).ConfigureAwait(false);
        }

        // GET: api/testing/mapping/2019-09-01
        [HttpGet("itemmapping/{id}")]
        public async Task<TimeSpan> GetItemMapping(int id)
        {
            return await _repository.BenchItemMultiMapping(id).ConfigureAwait(false);
        }

        // GET: api/testing/mapping/2019-09-01
        [HttpGet("itemmultiquery/{id}")]
        public async Task<TimeSpan> GetItemMultiquery(int id)
        {
            return await _repository.BenchItemMultiQuery(id).ConfigureAwait(false);
        }
    }
}
