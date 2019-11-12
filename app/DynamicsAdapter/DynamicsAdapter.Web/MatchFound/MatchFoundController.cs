using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamicsAdapter.Web.MatchFound
{

    [Route("[controller]")]
    [ApiController]
    public class MatchFoundController : ControllerBase
    {
        private readonly ILogger<MatchFoundController> _logger;
        public MatchFoundController(ILogger<MatchFoundController> logger)
        {
            _logger = logger;
        }

        //POST: MatchFound/id
        [HttpPost("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MatchFound(string id, [FromBody]Object payload)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            _logger.LogInformation("Received MatchFound response with SearchRequestId is " + id);
            _logger.LogInformation("SearchResult is " + payload);
            return Ok();
        }
    }
}