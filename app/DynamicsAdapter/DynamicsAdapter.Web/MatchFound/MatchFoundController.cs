using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.SearchRequest;
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
        private ISearchRequestService _service;
        public MatchFoundController(ILogger<MatchFoundController> logger, ISearchRequestService service)
        {
            _logger = logger;
            _service = service;
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
            var cts = new CancellationTokenSource();
            await _service.UploadIdentifier(Guid.Parse(id), null, cts.Token);
            return Ok();
        }
    }
}