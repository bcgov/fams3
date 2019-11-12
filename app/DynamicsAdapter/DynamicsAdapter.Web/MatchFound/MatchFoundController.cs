using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace DynamicsAdapter.Web.MatchFound
{
    public class MatchFoundResponse
    { 
        public string SearchRequestId { get; set; }
        public string SearchResult { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class MatchFoundController : ControllerBase
    {
        private readonly ILogger<MatchFoundController> _logger;
        public MatchFoundController(ILogger<MatchFoundController> logger)
        {
            _logger = logger;
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> MatchFound(MatchFoundResponse response)
        { 
            if(String.IsNullOrEmpty(response.SearchRequestId))
            {
                return BadRequest();
            }
            _logger.LogInformation("Received MatchFound response with SearchRequestId is " + response.SearchRequestId);
            _logger.LogInformation("SearchResult is " + response.SearchResult);
            return Ok();
        }
    }
}