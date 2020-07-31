using DynamicsAdapter.Web.PersonSearch.Models;
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
        [OpenApiTag("Agency Search Request API")]
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
        [OpenApiTag("Agency Search Request API")]
        public async Task<IActionResult> UpdateSearchRequest(string key, [FromBody]SearchRequestOrdered searchRequestOrdered)
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
        [Route("CancelSearchRequest/{requestId}")]
        [OpenApiTag("Agency Search Request API")]
        public async Task<IActionResult> CancelSearchRequest(string requestId, [FromBody]SearchRequestOrdered searchRequestOrdered)
        {
            if (string.IsNullOrEmpty(requestId)) 
                return BadRequest(new { Message = "requestId cannot be empty." });

            if (searchRequestOrdered.Action != RequestAction.CANCEL) 
                return BadRequest(new { Message = "CancelSearchRequest should only get Cancel request." });

            if (String.IsNullOrEmpty(searchRequestOrdered.SearchRequestKey)) 
                return BadRequest(new { Message = "FileId cannot be empty for cancelling request." });

            try
            {
                SSG_SearchRequest cancelledSearchRequest = await _agencyRequestService.ProcessCancelSearchRequest(searchRequestOrdered);
                return Ok(BuildSearchRequestSubmitted(cancelledSearchRequest, searchRequestOrdered));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        private SearchRequestSubmitted BuildSearchRequestSubmitted(SSG_SearchRequest ssgSearchRequest, SearchRequestOrdered requestOrdered)
        {
            SearchRequestSubmitted submitted = 
                new SearchRequestSubmitted()
                    {
                        Action = requestOrdered.Action,
                        RequestId = requestOrdered.RequestId,
                        SearchRequestKey = ssgSearchRequest.FileId,
                        SearchRequestId = ssgSearchRequest.SearchRequestId,
                        TimeStamp = DateTime.Now,
                        ProviderProfile = new ProviderProfile() {
                            Name= requestOrdered?.Person?.Agency?.Code
                        }
                    };
            if(requestOrdered.Action == RequestAction.NEW)
            {
                submitted.EstimatedCompletion = DateTime.Now.AddDays(60); //todo: need to implement when design is ready.
                submitted.QueuePosition = 4;//todo: need to implement when design is ready.
                submitted.Message = $"The new Search Request reference: {requestOrdered.RequestId} has been submitted successfully.";
            }
            else if(requestOrdered.Action == RequestAction.CANCEL)
            {
                if(ssgSearchRequest.StatusCode == SearchRequestStatusCode.AgencyCancelled.Value)
                    submitted.Message = $"The Search Request {ssgSearchRequest.FileId} has been cancelled successfully upon the request {requestOrdered.RequestId}.";
                else
                    submitted.Message = $"The Search Request {ssgSearchRequest.FileId} cannot be cancelled upon the request {requestOrdered.RequestId}.";
            }
            return submitted;
        }

    }
}