using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Web.Configuration;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.Redis;
using SearchApi.Web.Search;

namespace SearchApi.Web.Notifications
{

    public class WebHookNotifierSearchEventStatus :  ISearchApiNotifier<PersonSearchAdapterEvent>
    {

        private readonly HttpClient _httpClient;
        private readonly SearchApiOptions _searchApiOptions;
        private readonly ILogger<WebHookNotifierSearchEventStatus> _logger;

        private readonly ICacheService _cacheService;

        public WebHookNotifierSearchEventStatus(HttpClient httpClient, IOptions<SearchApiOptions> searchApiOptions,
            ILogger<WebHookNotifierSearchEventStatus> logger, ICacheService cacheService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _searchApiOptions = searchApiOptions.Value;
            _cacheService = cacheService;
            
        }

        public async Task NotifyEventAsync(Guid searchRequestId, PersonSearchAdapterEvent eventStatus, string eventName,
           CancellationToken cancellationToken)
        {
            var webHookName = "PersonSearch";

            await UpdateDataPartner(searchRequestId, eventStatus, eventName);
            if (await IsAllDataPartnerCompletedOrSearchInProgress(searchRequestId, eventName))
            {

                foreach (var webHook in _searchApiOptions.WebHooks)
                {
                    _logger.LogDebug(
                       $"The webHook {webHookName} notification is attempting to send status {eventName} event for {webHook.Name} webhook.");

                    if (!URLHelper.TryCreateUri(webHook.Uri, eventName, $"{searchRequestId}", out var endpoint))
                    {
                        _logger.LogWarning(
                            $"The webHook {webHookName} notification uri is not established or is not an absolute Uri for {webHook.Name}. Set the WebHook.Uri value on SearchApi.WebHooks settings.");
                        return;
                    }

                    using var request = new HttpRequestMessage();

                    try
                    {
                        if (eventName == EventName.Finalized) {
                            eventStatus = new PersonSearchFinalizedCls()
                            {
                                FileId=eventStatus.FileId,
                                Message = "Search Request Finalized",
                                SearchRequestId= eventStatus.SearchRequestId,
                                TimeStamp=DateTime.Now,
                                ProviderProfile=null
                            };
                        }
                        var content = new StringContent(JsonConvert.SerializeObject(eventStatus));
                        content.Headers.ContentType =
                            System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                        request.Content = content;
                        request.Method = HttpMethod.Post;
                        request.Headers.Accept.Add(
                            System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                        request.RequestUri = endpoint;
                        var response = await _httpClient.SendAsync(request, cancellationToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError(
                                $"The webHook {webHookName} notification has not executed status {eventName} successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                            return;
                        }

                        _logger.LogInformation(
                            $"The webHook {webHookName} notification has executed status {eventName} successfully for {webHook.Name} webHook.");

                    }
                    catch (Exception exception)
                    {
                        _logger.LogError($"The webHook {webHookName} notification failed for status {eventName} for {webHook.Name} webHook. [{exception.Message}]");
                    }
                }
                DeleteFromCache(searchRequestId, eventName);
            }

        }

        private async Task<bool> IsAllDataPartnerCompletedOrSearchInProgress(Guid searchRequestId, string eventName)
        {
            try
            {
                if (eventName.Equals(EventName.Finalized))
                {
                    return JsonConvert.SerializeObject(await _cacheService.GetRequest(searchRequestId)).AllPartnerCompleted();
                }
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Check Data Partner Status Failed. [{eventName}] for {searchRequestId}. [{exception.Message}]");
                return false;
            }
        }
        private async void DeleteFromCache(Guid searchRequestId, string eventName)
        {
            try
            {
                if (eventName.Equals(EventName.Finalized))
                {
                    await _cacheService.DeleteRequest(searchRequestId);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Delete search request failed. [{eventName}] for {searchRequestId}. [{exception.Message}]");
                
            }

        }


        private async Task UpdateDataPartner(Guid searchRequestId, PersonSearchAdapterEvent eventStatus, string eventName)
        {
            try
            {
                if (eventName.Equals(EventName.Completed) || eventName.Equals(EventName.Rejected))
                {
                    var searchRequest = JsonConvert.SerializeObject(await _cacheService.GetRequest(searchRequestId)).UpdateDataPartner(eventStatus.ProviderProfile.Name);
                    await _cacheService.SaveRequest(searchRequest);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Update Data Partner Status Failed. [{eventName}] for {searchRequestId}. [{exception.Message}]");
                
            }
        }
    }
}
