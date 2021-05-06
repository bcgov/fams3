using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchRequest.Adaptor.Notifier.Models;
using SearchRequest.Adaptor.Publisher.Models;
using SearchRequestAdaptor.Publisher;
using Serilog;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Notifier
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly ISearchRequestEventPublisher _publisher;

        public NotificationController(ISearchRequestEventPublisher searchRequestEventPublisher, ILogger<NotificationController> logger)
        {
            _publisher = searchRequestEventPublisher;
            _logger = logger;
        }

        /// <summary>
        /// Receives a <see cref="PersonSearchRequest"/> and start the process of searching additional information for a person
        /// by submitting an ExecuteSearch Command to appropriate queue.
        /// </summary>
        /// <param name="personSearchRequest">A request to search a person</param>
        /// <returns><see cref="PersonSearchResponse"/></returns>
        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Notify([FromBody] Notification notification)
        {
            using (LogContext.PushProperty("RequestRef", $"{notification?.AgencyFileId}"))
            using (LogContext.PushProperty("AgencyCode", $"{notification?.Agency}"))
            using (LogContext.PushProperty("SearchRequestKey", $"{notification?.FileId}"))
            {
                _logger.Log(LogLevel.Information, $"receive {notification.Acvitity} notification");
                if (notification == null)
                {
                    _logger.Log(LogLevel.Error, $"Notification request cannot be null");
                    return BadRequest("Notification request cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.Log(LogLevel.Error, $"Notification request is invalid");
                    return BadRequest(ModelState);
                }
                _logger.Log(LogLevel.Debug, $"Receving notification from FAMS3 - Dynamics Platform for Search Request - File Id: {notification.FileId} (Agency SR Id {notification.AgencyFileId})");

                var notifyEvent = new SearchRequestNotificationEvent
                {
                    ProviderProfile = new ProviderProfile { Name = notification.Agency },
                    NotificationType = (NotificationType)Enum.Parse(typeof(NotificationType), notification.Acvitity, true),
                    RequestId = notification.AgencyFileId,
                    SearchRequestKey = notification.FileId,
                    QueuePosition = notification.PositionInQueue,
                    Message = $"Activity {notification.Acvitity} occured. FSO : {notification.FSOName}",
                    TimeStamp = notification.ActivityDate,
                    EstimatedCompletion = notification.EstimatedCompletionDate,
                    FSOName = notification.FSOName,
                    Person = notification.Person
                };

                await _publisher.PublishSearchRequestNotification(notifyEvent);

                return Ok();
            }
        }
    }
}
