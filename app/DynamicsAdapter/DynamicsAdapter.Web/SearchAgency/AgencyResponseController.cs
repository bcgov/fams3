using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    [Route("[controller]")]
    [ApiController]
    public class AgencyResponseController : ControllerBase
    {
        private readonly ILogger<AgencyRequestController> _logger;
        private readonly IAgencyResponseService _agencyResponseService;
        public AgencyResponseController(
                 ILogger<AgencyRequestController> logger,
                 IAgencyResponseService agencyResponseService
                 )
        {
            _logger = logger;
            _agencyResponseService = agencyResponseService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("ResponseReady")]
        [OpenApiTag("Agency Search Result Response API")]
        public async Task<IActionResult> ResponseReady([FromBody]SearchResponseReady searchResponseReady)
        {
            using (LogContext.PushProperty("RequestRef", $"{searchResponseReady?.AgencyFileId}"))
            using (LogContext.PushProperty("AgencyCode", $"{searchResponseReady?.Agency}"))
            {
                _logger.LogInformation("Get searchResponseReady");
                _logger.LogDebug(JsonConvert.SerializeObject(searchResponseReady));

                Person p = await _agencyResponseService.GetSearchRequestResponse(searchResponseReady);
                searchResponseReady.Person = p;
                return Ok();
            }
        }
    }
}