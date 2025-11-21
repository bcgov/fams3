using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.Utils.Url;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchRequest.Adaptor.Publisher.Models;
using SearchRequestAdaptor.Configuration;
using SearchRequestAdaptor.Publisher;
using SearchRequestAdaptor.Publisher.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Notifier
{
    public interface ISearchRequestNotifier<T>
    {
        Task NotifySearchRequestEventAsync(string requestId, T searchRequestEvent, CancellationToken cancellationToken, int retryTimes=0, int maxRetryTimes=0);
    }

    public class WebHookSearchRequestNotifier : ISearchRequestNotifier<SearchRequestEvent>
    {

        private readonly HttpClient _httpClient;
        private readonly SearchRequestAdaptorOptions _searchRequestOptions;
        private readonly ILogger<WebHookSearchRequestNotifier> _logger;
        private readonly ISearchRequestEventPublisher _searchRequestEventPublisher;


        public WebHookSearchRequestNotifier(HttpClient httpClient, IOptions<SearchRequestAdaptorOptions> searchRequestOptions, ILogger<WebHookSearchRequestNotifier> logger, ISearchRequestEventPublisher searchRequestEventPublisher)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(3);
            _logger = logger;
            _searchRequestOptions = searchRequestOptions.Value;
            _searchRequestEventPublisher = searchRequestEventPublisher;
        }

        public async Task NotifySearchRequestEventAsync(string requestId, SearchRequestEvent searchRequestEvent,
           CancellationToken cancellationToken, int retryTimes=0, int maxRetryTimes=0)
        {
            if(searchRequestEvent is SearchRequestOrdered)
            {
                await NotifySearchRequestOrderedEvent(requestId, (SearchRequestOrdered)searchRequestEvent, cancellationToken, retryTimes, maxRetryTimes);
            }
            else
            {
                await NotifyNotificationAcknowledged(requestId, (NotificationAcknowledged)searchRequestEvent, cancellationToken);
            }

        }

        private async Task NotifySearchRequestOrderedEvent(
            string requestId, 
            SearchRequestOrdered searchRequestOrdered,
            CancellationToken cancellationToken,
            int retryTimes,
            int maxRetryTimes)
        {
            if (searchRequestOrdered == null)
            {
                throw new ArgumentNullException(nameof(searchRequestOrdered));
            }

            const string webHookName = "SearchRequest";
            _logger.LogDebug("➡️ Start NotifySearchRequestOrderedEvent for RequestId: {RequestId} and WebHookName: {webHookName}", requestId, webHookName);

            string eventName = searchRequestOrdered.Action switch
            {
                RequestAction.NEW => "CreateSearchRequest",
                RequestAction.UPDATE => "UpdateSearchRequest",
                RequestAction.CANCEL => "CancelSearchRequest",
                _ => null
            };

            foreach (var webHook in _searchRequestOptions.WebHooks)
            {
                _logger.LogDebug(
                    $"The webHook {webHookName} notification is attempting to send {eventName} for {webHook.Name} webhook.");

                if (!URLHelper.TryCreateUri(webHook.Uri, eventName, $"{requestId}", out var endpoint))
                {
                    _logger.LogError(
                        "❌ Invalid webhook URI. WebHookName: {WebHookName}, ConfigName: {Name}. Please verify SearchRequestAdaptor.WebHooks settings.",
                        webHookName,
                        webHook.Name);
                    throw new Exception($"The webHook {webHookName} notification uri is not established or is not an absolute Uri for {webHook.Name}.");
                }

                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                    var json = JsonConvert.SerializeObject(searchRequestOrdered);

                    request.Content = new StringContent(json);
                    request.Content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request.Headers.Accept.Add(
                        System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    request.Headers.Add("X-ApiKey", _searchRequestOptions.ApiKeyForDynadaptor);

                    var response = await _httpClient.SendAsync(request, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError || response.StatusCode==System.Net.HttpStatusCode.GatewayTimeout)
                        {
                            string reason = await response.Content.ReadAsStringAsync();
                            _logger.LogError(
                                $"The webHook {webHookName} notification has not executed status {eventName} successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.Reason is {reason}.");
                            throw new Exception($"The webHook {webHookName} notification has not executed status {eventName} successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            string reason = await response.Content.ReadAsStringAsync();
                            RejectReason reasonObj = JsonConvert.DeserializeObject<RejectReason>(reason);
                            if (reasonObj.ReasonCode.Equals("error", StringComparison.InvariantCultureIgnoreCase))
                                throw new Exception("should not get here. the request is wrong.");

                            await _searchRequestEventPublisher.PublishSearchRequestRejected(
                                    searchRequestOrdered,
                                    new List<ValidationResult>()
                                    {
                                        new ValidationResultData(){ PropertyName=reasonObj.ReasonCode, ErrorMessage=reasonObj.Message }
                                    });
                            return;
                        }

                        var exContent = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Message Failed {response.StatusCode}, {exContent}");
                    }

                    _logger.LogInformation("get response successfully from webhook.");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var saved = JsonConvert.DeserializeObject<SearchRequestSavedEvent>(responseContent);

                    //for the new action, we changed from Dynamics push the notification to here, openshift publish notification once sr is created.
                    //for the update action, fmep needs notified and notification from dynamics.
                    if(saved.Action == RequestAction.NEW) 
                    {
                        _logger.LogInformation("create sr get success, publish accepted notification");
                        var notifyEvent = new SearchRequestNotificationEvent
                        {
                            ProviderProfile = saved.ProviderProfile,
                            NotificationType = NotificationType.RequestSaved,
                            RequestId = saved.RequestId,
                            SearchRequestKey = saved.SearchRequestKey, 
                            QueuePosition = saved.QueuePosition,
                            Message = $"Activity RequestSaved occured. ",
                            TimeStamp = DateTime.Now,
                            EstimatedCompletion = saved.EstimatedCompletion,
                            FSOName = null,
                            Person = null
                        };
                        await _searchRequestEventPublisher.PublishSearchRequestNotification(notifyEvent);
                    }

                    if (saved.Action == RequestAction.UPDATE)
                    {                       
                        _logger.LogInformation($"publish SearchRequestSaved");
                        await _searchRequestEventPublisher.PublishSearchRequestSaved(saved);
                        _logger.LogInformation("update sr get success, publish accepted notification");
                        var notifyEvent = new SearchRequestNotificationEvent
                        {
                            ProviderProfile = saved.ProviderProfile,
                            NotificationType = NotificationType.RequestSaved,
                            RequestId = saved.RequestId,
                            SearchRequestKey = saved.SearchRequestKey,
                            QueuePosition = saved.QueuePosition,
                            Message = $"Activity RequestSaved occured. ",
                            TimeStamp = DateTime.Now,
                            EstimatedCompletion = saved.EstimatedCompletion,
                            FSOName = null,
                            Person = null
                        };

                        await _searchRequestEventPublisher.PublishSearchRequestNotification(notifyEvent);
                    }

                    if (saved.Action == RequestAction.CANCEL)
                    {
                        _logger.LogInformation(
                            $"publish SearchRequestSaved");
                        await _searchRequestEventPublisher.PublishSearchRequestSaved(saved);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Webhook {webHook.Name} failed: {exception.Message}");
                    throw;
                }
            }
            _logger.LogDebug("🏁 End NotifySearchRequestOrderedEvent for RequestId: {RequestId}", requestId);
        }

        private async Task NotifyNotificationAcknowledged(
            string requestId,
            NotificationAcknowledged notificationAck,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                throw new ArgumentNullException(nameof(requestId));
            }

            if (notificationAck == null)
            {
                throw new ArgumentNullException(nameof(notificationAck));
            }

            const string webHookName = "SearchRequest";
            _logger.LogDebug("➡️ Starting NotifyNotificationAcknowledged for RequestId: {RequestId} and WebHookName: {webHookName}", requestId, webHookName);

            foreach (var webHook in _searchRequestOptions.WebHooks)
            {
                if (!URLHelper.TryCreateUri(webHook.Uri, "NotificationAcknowledged", $"{requestId}", out var endpoint))
                {
                    _logger.LogError(
                    "❌ Invalid webhook URI for RequestId {RequestId}. WebHookName: {WebHookName}, ConfigName: {Name}. Please verify SearchRequestAdaptor.WebHooks settings.",
                    requestId,
                    webHookName,
                    webHook.Name);
                    return;
                }

                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);

                try
                {
                    string payload = JsonConvert.SerializeObject(notificationAck);
                    StringContent content = new StringContent(payload);
                    content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request.Headers.Accept.Add(
                        System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    request.Headers.Add("X-ApiKey", _searchRequestOptions.ApiKeyForDynadaptor);
                    request.Content = content;

                    _logger.LogDebug("Posting notification to webhook: {Endpoint}", endpoint);
                    _logger.LogDebug(
                        "📨 Posting WebHook notification for RequestId {RequestId} to {Endpoint}. PayloadSize={PayloadSize} bytes",
                        requestId,
                        endpoint,
                        payload.Length);
                    HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorBody = await response.Content.ReadAsStringAsync();
                        _logger.LogError(
                            "❌ Webhook call failed for RequestId {RequestId}. WebHookName {WebHookName}, Endpoint {Endpoint}. Status={StatusCode}, Response={Body}",
                            requestId,
                            webHookName,
                            endpoint,
                            response.StatusCode,
                            errorBody);

                        if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError 
                            && response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                        {
                            throw new Exception($"Message Failed {response.StatusCode}, {errorBody}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation(
                            "✅ Webhook acknowledged successfully for RequestId {RequestId}. Endpoint: {Endpoint}",
                            requestId,
                            endpoint);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception,
                        "❌ NotifyNotificationAcknowledged failed for RequestId {RequestId}: {Message}",
                        requestId,
                        exception.Message);
                    throw;
                }
            }
            _logger.LogDebug("🏁 End NotifyNotificationAcknowledged for RequestId: {RequestId}", requestId);
        }
    }

    public class RejectReason
    {
        public string ReasonCode { get; set; }
        public string Message { get; set; }
    }
}
