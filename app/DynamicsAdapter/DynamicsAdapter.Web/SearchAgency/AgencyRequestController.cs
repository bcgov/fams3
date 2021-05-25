using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.Register;
using DynamicsAdapter.Web.SearchAgency.Exceptions;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchRequest;
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
    public class AgencyRequestController : ControllerBase
    {
        private readonly ILogger<AgencyRequestController> _logger;
        private readonly IAgencyRequestService _agencyRequestService;
        private readonly ISearchRequestRegister _register;

        public AgencyRequestController(
                ILogger<AgencyRequestController> logger,
                IAgencyRequestService agencyRequestService,
                ISearchRequestRegister register
                )
        {
            _logger = logger;
            _agencyRequestService = agencyRequestService;
            _register = register;
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
            using (LogContext.PushProperty("RequestRef", $"{requestId}"))
            using (LogContext.PushProperty("AgencyCode", $"{searchRequestOrdered?.Person?.Agency?.Code}"))
            {
                _logger.LogInformation("Get CreateSearchRequest");
                _logger.LogDebug(JsonConvert.SerializeObject(searchRequestOrdered));
                if (string.IsNullOrEmpty(requestId)) return BadRequest(new { Message = "requestId cannot be empty." });
                if (searchRequestOrdered == null) return BadRequest(new { Message = "SearchRequestOrdered cannot be empty." });
                if (searchRequestOrdered.Action != RequestAction.NEW) return BadRequest(new { Message = "CreateSearchRequest should only get NEW request." });

                try
                {
                    SSG_SearchRequest createdSearchRequest = await _agencyRequestService.ProcessSearchRequestOrdered(searchRequestOrdered);
                    if (createdSearchRequest == null)
                        return StatusCode(StatusCodes.Status500InternalServerError);

                    _logger.LogInformation("SearchRequest is created successfully.");
                    return Ok(BuildSearchRequestSaved_Create(createdSearchRequest, searchRequestOrdered));
                }
                catch (AgencyRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }
                catch (Exception ex)
                {
                    SSG_SearchRequest createdSR = _agencyRequestService.GetSSGSearchRequest();
                    if (createdSR != null)
                    {
                        await _agencyRequestService.SystemCancelSSGSearchRequest(createdSR);
                    }
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status504GatewayTimeout, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("UpdateSearchRequest/{requestId}")]
        [OpenApiTag("Agency Search Request API")]
        public async Task<IActionResult> UpdateSearchRequest(string requestId, [FromBody]SearchRequestOrdered searchRequestOrdered)
        {
            using (LogContext.PushProperty("RequestRef", $"{requestId}"))
            using (LogContext.PushProperty("AgencyCode", $"{searchRequestOrdered?.Person?.Agency?.Code}"))
            using (LogContext.PushProperty("SearchRequestKey", $"{searchRequestOrdered?.SearchRequestKey}"))
            {
                _logger.LogInformation("Get UpdateSearchRequest");
                if (string.IsNullOrEmpty(requestId))
                    return BadRequest(new { Message = "requestId cannot be empty." });

                if (searchRequestOrdered == null)
                    return BadRequest(new { Message = "SearchRequestOrdered cannot be empty." });

                if (searchRequestOrdered.Action != RequestAction.UPDATE)
                    return BadRequest(new { Message = "UpdateSearchRequest should only get Update request." });

                if (String.IsNullOrEmpty(searchRequestOrdered.SearchRequestKey))
                    return BadRequest(new { Message = "FileId cannot be empty for updating request." });

                SSG_SearchRequest updatedSearchRequest = null;
                try
                {
                    updatedSearchRequest = await _agencyRequestService.ProcessUpdateSearchRequest(searchRequestOrdered);
                    if (updatedSearchRequest == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = "error", Message ="error" });
                    }                
                }
                catch (AgencyRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest(new { Message = $"FileId ( {searchRequestOrdered.SearchRequestKey} ) is invalid. {ex.Message}" });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = "error", Message = ex.Message });
                }
                _logger.LogInformation("UpdateSearchRequest successfully");
                return Ok(BuildSearchRequestSaved_Update(updatedSearchRequest, searchRequestOrdered));
            }
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
            using (LogContext.PushProperty("AgencyCode", $"{searchRequestOrdered?.Person?.Agency?.Code}"))
            using (LogContext.PushProperty("SearchRequestKey", $"{searchRequestOrdered?.SearchRequestKey}"))
            {
                _logger.LogInformation("Get CancelSearchRequest");

                if (searchRequestOrdered == null)
                    return BadRequest(new { Message = "SearchRequestOrdered cannot be empty." });

                if (searchRequestOrdered.Action != RequestAction.CANCEL)
                    return BadRequest(new { Message = "CancelSearchRequest should only get Cancel request." });

                if (String.IsNullOrEmpty(searchRequestOrdered.SearchRequestKey))
                    return BadRequest(new { Message = "FileId cannot be empty for cancelling request." });

                SSG_SearchRequest cancelledSearchRequest;
                try
                {
                    cancelledSearchRequest = await _agencyRequestService.ProcessCancelSearchRequest(searchRequestOrdered);
                    if (cancelledSearchRequest == null)
                        return BadRequest(new { Message = $"FileId ( {searchRequestOrdered.SearchRequestKey} ) is invalid." });
                }
                catch (AgencyRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = "error", Message = "error" });
                }
                _logger.LogInformation("CancelSearchRequest successfully");
                return Ok(BuildSearchRequestSaved_Cancel(cancelledSearchRequest, searchRequestOrdered));
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("NotificationAcknowledged/{requestId}")]
        [OpenApiTag("Agency Search Request API")]
        public async Task<IActionResult> NotificationAcknowledged(string requestId, [FromBody]Acknowledgement ack)
        {
            using (LogContext.PushProperty("AgencyCode", $"{ack?.ProviderProfile.Name}"))
            using (LogContext.PushProperty("SearchRequestKey", $"{ack?.SearchRequestKey}"))
            {
                if ((string.IsNullOrEmpty(ack?.RequestId) || string.IsNullOrEmpty(ack?.SearchRequestKey)) 
                    && ack.NotificationType==BcGov.Fams3.SearchApi.Contracts.SearchRequest.NotificationType.RequestClosed)
                    return StatusCode(StatusCodes.Status400BadRequest);

                _logger.LogInformation("Get NotificationAcknowledged");
               
                try
                {
                    SearchResponseReady ready = await _register.GetSearchResponseReady(ack.SearchRequestKey, ack.RequestId);

                    if (ready == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                    bool isAmendment = false;
                    if (ack.SearchRequestKey.Contains("-")) isAmendment = true;
                    await _agencyRequestService.ProcessNotificationAcknowledgement(ack, ready.ApiCallGuid, isAmendment);
                    await _register.DeleteSearchResponseReady(ack.SearchRequestKey, ack.RequestId);
                }
                catch (AgencyRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status504GatewayTimeout, new { ReasonCode = "error", Message = "error" });
                }
                _logger.LogInformation("NotificationAcknowledged successfully");
                return Ok();
            }
        }
        private SearchRequestSaved BuildSearchRequestSaved_Create(SSG_SearchRequest ssgSearchRequest, SearchRequestOrdered requestOrdered)
        {
            SearchRequestSaved saved =
                new SearchRequestSaved()
                {
                    Action = requestOrdered.Action,
                    RequestId = requestOrdered.RequestId,
                    SearchRequestKey = ssgSearchRequest.FileId,
                    SearchRequestId = ssgSearchRequest.SearchRequestId,
                    TimeStamp = DateTime.Now,
                    EstimatedCompletion = null,
                    QueuePosition = null,
                    Message = $"The new Search Request reference: {requestOrdered.RequestId} has been submitted successfully.",
                    ProviderProfile = new ProviderProfile()
                    {
                        Name = requestOrdered?.Person?.Agency?.Code
                    }
                };
            return saved;
        }

        private SearchRequestSaved BuildSearchRequestSaved_Cancel(SSG_SearchRequest ssgSearchRequest, SearchRequestOrdered requestOrdered)
        {
            SearchRequestSaved saved =
                new SearchRequestSaved()
                {
                    Action = requestOrdered.Action,
                    RequestId = requestOrdered.RequestId,
                    SearchRequestKey = requestOrdered.SearchRequestKey,
                    SearchRequestId = ssgSearchRequest == null ? Guid.Empty : ssgSearchRequest.SearchRequestId,
                    TimeStamp = DateTime.Now,
                    ProviderProfile = new ProviderProfile()
                    {
                        Name = requestOrdered?.Person?.Agency?.Code
                    },
                    Message = $"The Search Request {requestOrdered.SearchRequestKey} has been cancelled successfully upon the request {requestOrdered.RequestId}."
                };

            return saved;
        }

        private SearchRequestSaved BuildSearchRequestSaved_Update(SSG_SearchRequest ssgSearchRequest, SearchRequestOrdered requestOrdered)
        {
            SearchRequestSaved saved =
                new SearchRequestSaved()
                {
                    Action = requestOrdered.Action,
                    RequestId = requestOrdered.RequestId,
                    SearchRequestKey = requestOrdered.SearchRequestKey,
                    SearchRequestId = ssgSearchRequest == null ? Guid.Empty : ssgSearchRequest.SearchRequestId,
                    TimeStamp = DateTime.Now,
                    ProviderProfile = new ProviderProfile()
                    {
                        Name = requestOrdered?.Person?.Agency?.Code
                    }
                };

            if (ssgSearchRequest != null)
                saved.Message = $"The Search Request {requestOrdered.SearchRequestKey} has been updated successfully upon the request {requestOrdered.RequestId}.";

            return saved;
        }
    }
}