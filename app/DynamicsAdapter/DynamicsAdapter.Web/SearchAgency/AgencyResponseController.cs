using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.SearchAgency.Models;
using DynamicsAdapter.Web.SearchAgency.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using Serilog.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    [Route("[controller]")]
    [ApiController]
    public class AgencyResponseController : ControllerBase
    {
        private readonly ILogger<AgencyResponseController> _logger;
        private readonly IAgencyResponseService _agencyResponseService;
        private readonly IAgencyNotificationWebhook<SearchRequestNotification> _agencyNotifier;

        public AgencyResponseController(
                 ILogger<AgencyResponseController> logger,
                 IAgencyResponseService agencyResponseService,
                 IAgencyNotificationWebhook<SearchRequestNotification> agencyNotifier
                 )
        {
            _logger = logger;
            _agencyResponseService = agencyResponseService;
            _agencyNotifier = agencyNotifier;
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


                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    Person p = await _agencyResponseService.GetSearchRequestResponse(searchResponseReady);
                    await _agencyNotifier.SendNotificationAsync(
                        BuildSearchRequestNotification(searchResponseReady, p),
                        (new CancellationTokenSource()).Token
                        );
                    return Ok();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest();
                }
            }
        }

        private SearchRequestNotification BuildSearchRequestNotification(SearchResponseReady searchResponseReady, Person person)
        {
            return
                new SearchRequestNotification()
                {
                    Action = RequestAction.NOTIFY,
                    RequestId = searchResponseReady.AgencyFileId,
                    SearchRequestKey = searchResponseReady.FileId,
                    SearchRequestId = Guid.Empty,
                    TimeStamp = DateTime.Now,
                    NotificationType = NotificationType.ResponseReady,
                    EstimatedCompletion = null,
                    QueuePosition = null,
                    Message = null,
                    Person = person,
                    ProviderProfile = new ProviderProfile
                    {
                        Name = searchResponseReady.Agency
                    },
                    FSOName = searchResponseReady.FSOName
                };
        }

    }
}