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

            //todo, replaced with data from payload
            var toBeReplaced = new SSG_Identifier()
            {
                SSG_Identification = "Test from dynamics adapter",
                ssg_identificationeffectivedate = new DateTime(2014, 1, 1),
                SSG_SearchRequest = new SSG_SearchRequest()
                {
                    SearchRequestId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4")
                },
                StateCode = 0,
                StatusCode = 1
            };
            //todo

            try
            {
                SSG_Identifier result = await _service.UploadIdentifier(toBeReplaced, cts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
            return Ok();
        }
    }
}