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
            using (LogContext.PushProperty("RequestRef", requestId ?? "(null)"))
            using (LogContext.PushProperty("AgencyCode", searchRequestOrdered?.Person?.Agency?.Code))
            {
                _logger.LogInformation("➡️ Start CreateSearchRequest for RequestId: {RequestId}", requestId);

                if (string.IsNullOrEmpty(requestId))
                {
                    _logger.LogWarning("❌ requestId is missing.");
                    return BadRequest(new { ReasonCode = "error", Message = "requestId cannot be empty." });
                }

                if (searchRequestOrdered == null)
                {
                    _logger.LogWarning("❌ SearchRequestOrdered payload is missing.");
                    return BadRequest(new { ReasonCode = "error", Message = "SearchRequestOrdered cannot be empty." });
                }

                if (searchRequestOrdered.Action != RequestAction.NEW)
                {
                    _logger.LogWarning(
                        "❌ CreateSearchRequest should only receive NEW action, received: {Action}",
                        searchRequestOrdered.Action);

                    return BadRequest(new
                    {
                        ReasonCode = "error",
                        Message = "CreateSearchRequest should only get NEW request."
                    });
                }

                SSG_SearchRequest createdSearchRequest = null;
                try
                {
                    _logger.LogDebug("Processing SearchRequestOrdered via AgencyRequestService...");
                    createdSearchRequest = await _agencyRequestService.ProcessSearchRequestOrdered(searchRequestOrdered);

                    if (createdSearchRequest == null)
                    {
                        _logger.LogError("❌ _agencyRequestService returned NULL for ProcessSearchRequestOrdered");
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                    _logger.LogInformation(
                        "SearchRequest created successfully. SearchRequestId: {SearchRequestId}",
                        createdSearchRequest.SearchRequestId);
                }
                catch (AgencyRequestException ex)
                {
                    _logger.LogError(ex,
                        "❌ AgencyRequestException while creating SearchRequest. Reason: {Reason}",
                        ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Unexpected error while creating SearchRequest.");

                    SSG_SearchRequest createdSR = _agencyRequestService.GetSSGSearchRequest();
                    if (createdSR != null)
                    {
                        _logger.LogWarning("Rolling back: cancelling partially created SSG_SearchRequest {SearchRequestId}", createdSR.SearchRequestId);
                        await _agencyRequestService.SystemCancelSSGSearchRequest(createdSR);
                    }

                    if( ex is Simple.OData.Client.WebRequestException webEx)
                    {
                        _logger.LogError("OData RequestUri: {Uri}", webEx.RequestUri?.AbsoluteUri);
                        _logger.LogError("OData Response: {Response}", webEx.Response);
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }

                //try to submit to queue and then get EstimatedDate and positionInQueue
                try
                {
                    _logger.LogDebug(
                        "Submitting SearchRequest {SearchRequestId} to queue...",
                        createdSearchRequest.SearchRequestId);
                    
                    await _agencyRequestService.SubmitSearchRequestToQueue(createdSearchRequest.SearchRequestId);
                    createdSearchRequest = await _agencyRequestService.RefreshSearchRequest(createdSearchRequest.SearchRequestId);

                    _logger.LogInformation(
                        "SearchRequest submitted to queue successfully. SearchRequestId: {SearchRequestId}",
                        createdSearchRequest.SearchRequestId);
                }catch(Exception ex)
                {
                    _logger.LogError(ex,
                        "❌ Failed to submit SearchRequest {SearchRequestId} to queue.",
                        createdSearchRequest.SearchRequestId);
                    //default value, in case there is error, we still can return accept event.
                    createdSearchRequest.EstimatedCompletionDate = DateTime.UtcNow.AddMonths(3);
                    createdSearchRequest.QueuePosition = 9999;
                }

                _logger.LogInformation(
                    "Returning SearchRequestSaved event for RequestId {RequestId}",
                    requestId);
                _logger.LogDebug(
                    "🏁 End CreateSearchRequest for RequestId: {RequestId}",
                    requestId);
                    
                return Ok(BuildSearchRequestSaved_Create(createdSearchRequest, searchRequestOrdered));
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
                    return BadRequest(new { ReasonCode = "error", Message = "requestId cannot be empty." });

                if (searchRequestOrdered == null)
                    return BadRequest(new { ReasonCode = "error", Message = "SearchRequestOrdered cannot be empty." });

                if (searchRequestOrdered.Action != RequestAction.UPDATE)
                    return BadRequest(new { ReasonCode = "error", Message = "UpdateSearchRequest should only get Update request." });

                if (String.IsNullOrEmpty(searchRequestOrdered.SearchRequestKey))
                    return BadRequest(new { ReasonCode = "error", Message = "FileId cannot be empty for updating request." });

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
                    _logger.LogInformation(ex.Message);
                    return BadRequest(new { ReasonCode = ex.Message, Message = $"FileId ( {searchRequestOrdered.SearchRequestKey} ) is invalid. {ex.Message}" });

                }
                catch (Exception ex)
                {
                    if (ex is Simple.OData.Client.WebRequestException)
                    {
                        _logger.LogError(((Simple.OData.Client.WebRequestException)ex).Response);
                    }
                    _logger.LogError(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = "error", Message = ex.Message });
                }
                _logger.LogInformation("UpdateSearchRequest successfully");

                //try to get EstimatedDate and positionInQueue
                try
                {
                    updatedSearchRequest = await _agencyRequestService.RefreshSearchRequest(updatedSearchRequest.SearchRequestId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "get current search request failed.");
                    //default value, in case there is error, we still can return accept event.
                    updatedSearchRequest.EstimatedCompletionDate = DateTime.UtcNow.AddMonths(3);
                    updatedSearchRequest.QueuePosition = 9999;
                }
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
                    return BadRequest(new { ReasonCode = "error", Message = "SearchRequestOrdered cannot be empty." });

                if (searchRequestOrdered.Action != RequestAction.CANCEL)
                    return BadRequest(new { ReasonCode = "error", Message = "CancelSearchRequest should only get Cancel request." });

                if (String.IsNullOrEmpty(searchRequestOrdered.SearchRequestKey))
                    return BadRequest(new { ReasonCode = "error", Message = "FileId cannot be empty for cancelling request." });

                SSG_SearchRequest cancelledSearchRequest;
                try
                {
                    cancelledSearchRequest = await _agencyRequestService.ProcessCancelSearchRequest(searchRequestOrdered);
                }
                catch (AgencyRequestException ex)
                {
                    _logger.LogInformation(ex.Message);
                    return BadRequest(new { ReasonCode = ex.Message, Message = $"FileId ( {searchRequestOrdered.SearchRequestKey} ) is invalid. {ex.Message}" });
                }
                catch (Exception ex)
                {
                    if (ex is Simple.OData.Client.WebRequestException)
                    {
                        _logger.LogError(((Simple.OData.Client.WebRequestException)ex).Response);
                    }
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
                    _logger.LogInformation(ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = ex.Message, Message = ex.InnerException?.Message });
                }
                catch (Exception ex)
                {
                    if (ex is Simple.OData.Client.WebRequestException)
                    {
                        _logger.LogError(((Simple.OData.Client.WebRequestException)ex).Response);
                    }
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
                    EstimatedCompletion = ssgSearchRequest?.EstimatedCompletionDate,
                    QueuePosition = ssgSearchRequest?.QueuePosition,
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
                    EstimatedCompletion = ssgSearchRequest?.EstimatedCompletionDate,
                    QueuePosition = ssgSearchRequest?.QueuePosition,
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