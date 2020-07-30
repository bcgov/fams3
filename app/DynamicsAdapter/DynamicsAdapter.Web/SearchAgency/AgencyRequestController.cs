using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    [Route("[controller]")]
    [ApiController]
    public class AgencyRequestController : ControllerBase
    {
        private readonly ILogger<AgencyRequestController> _logger;
        private readonly IAgencyRequestService _agencyRequestService;

        public AgencyRequestController(               
                ILogger<AgencyRequestController> logger,
                IAgencyRequestService agencyRequestService
                )
        {
            _logger = logger;
            _agencyRequestService = agencyRequestService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("CreateSearchRequest/{requestId}")]
        [OpenApiTag("Agency Search Reqeust API")]
        public async Task<IActionResult> CreateSearchRequest(string requestId, [FromBody]SearchRequestOrdered searchRequestOrdered)
        {
            if (string.IsNullOrEmpty(requestId)) return BadRequest();
            if (searchRequestOrdered.Action != RequestAction.NEW) return BadRequest();
            try
            {
                SSG_SearchRequest createdSearchRequest = await _agencyRequestService.ProcessSearchRequestOrdered(searchRequestOrdered);
                 
                return Ok(BuildSearchRequestSubmitted(createdSearchRequest, searchRequestOrdered));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("UpdateSearchRequest/{key}")]
        [OpenApiTag("Agency Search Reqeust API")]
        public async Task<IActionResult> UpdateSearchRequest(string key, [FromBody]SearchRequestOrdered personCompletedEvent)
        {
            //todo: Not implemented yet.
            await Task.Delay(1);
            return Ok();
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("CancelSearchRequest/{key}")]
        [OpenApiTag("Agency Search Reqeust API")]
        public async Task<IActionResult> CancelSearchRequest(string key, [FromBody]SearchRequestOrdered personCompletedEvent)
        {
            //todo: Not implemented yet.
            await Task.Delay(1);
            return Ok();
        }

        private SearchRequestSubmitted BuildSearchRequestSubmitted(SSG_SearchRequest createdSearchRequest, SearchRequestOrdered requestOrdered)
        {
            return new SearchRequestSubmitted()
            {
                Action = requestOrdered.Action,
                RequestId = requestOrdered.RequestId,
                SearchRequestKey = createdSearchRequest.FileId,
                SearchRequestId = createdSearchRequest.SearchRequestId,
                TimeStamp = DateTime.Now,
                EstimatedCompletion = DateTime.Now.AddDays(60), //todo: need to implement when design is ready.
                QueuePosition = 4,
                Message = $"The new Search Request reference: {requestOrdered.RequestId} has been submitted successfully."
            };
        }
    }
}