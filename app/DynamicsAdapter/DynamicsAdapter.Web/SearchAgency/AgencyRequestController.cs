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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("CreateSearchRequest/{requestId}")]
        [OpenApiTag("Agency Search Request API")]
        public async Task<IActionResult> CreateSearchRequest(string requestId, [FromBody]SearchRequestOrdered searchRequestOrdered)
        {
            if (string.IsNullOrEmpty(requestId)) return BadRequest(new { Message = "requestId cannot be empty." });
            if (searchRequestOrdered.Action != RequestAction.NEW) return BadRequest(new { Message = "CreateSearchRequest should only get NEW request." });
            try
            {
                SSG_SearchRequest createdSearchRequest = await _agencyRequestService.ProcessSearchRequestOrdered(searchRequestOrdered);
                if(createdSearchRequest == null ) 
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return Ok(BuildSearchRequestSubmitted_Create(createdSearchRequest, searchRequestOrdered));
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

            SSG_SearchRequest cancelledSearchRequest = null;
            try
            {
                cancelledSearchRequest = await _agencyRequestService.ProcessCancelSearchRequest(searchRequestOrdered);
                if(cancelledSearchRequest == null)
                    return BadRequest(new { Message = $"FileId ( {searchRequestOrdered.SearchRequestKey} ) is invalid." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return Ok(BuildSearchRequestSubmitted_Cancel(cancelledSearchRequest, searchRequestOrdered));
        }

        private SearchRequestSubmitted BuildSearchRequestSubmitted_Create(SSG_SearchRequest ssgSearchRequest, SearchRequestOrdered requestOrdered)
        {
            SearchRequestSubmitted submitted = 
                new SearchRequestSubmitted()
                    {
                        Action = requestOrdered.Action,
                        RequestId = requestOrdered.RequestId,
                        SearchRequestKey = ssgSearchRequest.FileId,
                        SearchRequestId = ssgSearchRequest.SearchRequestId,
                        TimeStamp = DateTime.Now,
                        EstimatedCompletion = DateTime.Now.AddDays(60),
                        QueuePosition = 4,
                        Message = $"The new Search Request reference: {requestOrdered.RequestId} has been submitted successfully.",
                        ProviderProfile = new ProviderProfile() {
                            Name= requestOrdered?.Person?.Agency?.Code
                        }
                    };
            return submitted;          
        }

        private SearchRequestSubmitted BuildSearchRequestSubmitted_Cancel(SSG_SearchRequest ssgSearchRequest, SearchRequestOrdered requestOrdered)
        {
            SearchRequestSubmitted submitted =
                new SearchRequestSubmitted()
                {
                    Action = requestOrdered.Action,
                    RequestId = requestOrdered.RequestId,
                    SearchRequestKey = requestOrdered.SearchRequestKey,
                    SearchRequestId = ssgSearchRequest==null? Guid.Empty: ssgSearchRequest.SearchRequestId,
                    TimeStamp = DateTime.Now,
                    ProviderProfile = new ProviderProfile()
                    {
                        Name = requestOrdered?.Person?.Agency?.Code
                    }
                };

                if (ssgSearchRequest != null && ssgSearchRequest.StatusCode == SearchRequestStatusCode.AgencyCancelled.Value)
                    submitted.Message = $"The Search Request {requestOrdered.SearchRequestKey} has been cancelled successfully upon the request {requestOrdered.RequestId}.";
                else
                    submitted.Message = $"The Search Request {requestOrdered.SearchRequestKey} cannot be cancelled upon the request {requestOrdered.RequestId}.";

            return submitted;
        }
    }
}