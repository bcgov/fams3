﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Web.Configuration;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using SearchApi.Web.DeepSearch;
using BcGov.Fams3.SearchApi.Contracts.Person;
using System.Collections.Generic;

namespace SearchApi.Web.Notifications
{

    public class WebHookNotifierSearchEventStatus :  ISearchApiNotifier<PersonSearchAdapterEvent>
    {

        private readonly HttpClient _httpClient;
        private readonly SearchApiOptions _searchApiOptions;
        private readonly IDeepSearchService _deepSearchService;
        private readonly ILogger<WebHookNotifierSearchEventStatus> _logger;
     

        public WebHookNotifierSearchEventStatus(HttpClient httpClient, IOptions<SearchApiOptions> searchApiOptions,
            ILogger<WebHookNotifierSearchEventStatus> logger, IDeepSearchService deepSearchService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _searchApiOptions = searchApiOptions.Value;     
            _deepSearchService = deepSearchService;
            _httpClient.Timeout = TimeSpan.FromMinutes(_searchApiOptions.Timeout);
        }

        public async Task NotifyEventAsync(string searchRequestKey, PersonSearchAdapterEvent eventStatus, string eventName,
           CancellationToken cancellationToken)
        {
            try
            {
                var webHookName = "PersonSearch";
                foreach (var webHook in _searchApiOptions.WebHooks)
                {
                    _logger.LogDebug(
                       $"The webHook {webHookName} notification is attempting to send status {eventName} event for {webHook.Name} webhook.");

                    if (!URLHelper.TryCreateUri(webHook.Uri, eventName, $"{searchRequestKey}", out var endpoint))
                    {
                        _logger.LogWarning(
                            $"The webHook {webHookName} notification uri is not established or is not an absolute Uri for {webHook.Name}. Set the WebHook.Uri value on SearchApi.WebHooks settings.");
                        throw new Exception("uri is not established.");
                    }

                    using var request = new HttpRequestMessage();

                    try
                    {
                        StringContent content;
                        if (eventName == EventName.Finalized)
                        {
                            _logger.LogInformation($"Initiating {eventName} event for {eventStatus.SearchRequestKey} for {eventStatus.ProviderProfile.Name}");
                            PersonSearchEvent finalizedSearch = new PersonSearchFinalizedEvent()
                            {
                                SearchRequestKey = eventStatus.SearchRequestKey,
                                Message = "Search Request Finalized",
                                SearchRequestId = eventStatus.SearchRequestId,
                                TimeStamp = DateTime.Now
                            };
                            _logger.LogDebug(JsonConvert.SerializeObject(finalizedSearch));
                            content = new StringContent(JsonConvert.SerializeObject(finalizedSearch));
                        }
                        else
                        {
                            _logger.LogDebug(JsonConvert.SerializeObject(eventStatus));
                            content = new StringContent(JsonConvert.SerializeObject(eventStatus));
                        }

                        content.Headers.ContentType =
                            System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                        request.Content = content;
                        request.Method = HttpMethod.Post;
                        request.Headers.Accept.Add(
                            System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                        request.Headers.Add("X-ApiKey", _searchApiOptions.ApiKeyForDynadaptor);
                        request.RequestUri = endpoint;
                        var response = await _httpClient.SendAsync(request, cancellationToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError(
                                $"The webHook {webHookName} notification has not executed status {eventName} successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                            throw new Exception($"The webhook has not executed status {eventName} successfully, the error code is {response.StatusCode.GetHashCode()}");
                        }

                        _logger.LogInformation(
                            $"The webHook {webHookName} notification has executed status {eventName} successfully for {webHook.Name} webHook.");
                    }

                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "The webHook {webHookName} notification failed for status {eventName}.", webHookName, eventName);
                        throw;

                    }
                }
            }
            catch (Exception)
            {
                _logger.LogError("The webhook {eventName} is failed and is put into error queue.", eventName);
                throw;
            }
            finally
            {
                if (EventName.Finalized.Equals(eventName))
                    await _deepSearchService.DeleteFromCache(searchRequestKey);
                else
                    await ProcessEvents(searchRequestKey, eventStatus, eventName);
            }

        }

        private async Task ProcessEvents(string searchRequestKey, PersonSearchAdapterEvent eventStatus, string eventName)
        {
            if (eventStatus.ProviderProfile.SearchSpeedType == SearchSpeedType.Slow) //JCA is different
            {
                if (eventName.Equals(EventName.Rejected))
                {
                    await _deepSearchService.UpdateDataPartner(searchRequestKey, eventStatus.ProviderProfile.Name, eventName);
                }
                else if (eventName.Equals(EventName.Completed))
                {
                    PersonSearchCompletedJCA completed = (PersonSearchCompletedJCA)eventStatus;
                    if(completed.Message != null && completed.Message.Contains("All traces received."))
                    {
                        await _deepSearchService.UpdateDataPartner(searchRequestKey, eventStatus.ProviderProfile.Name, eventName);
                    }
                }
            }
            else
            {
                await _deepSearchService.UpdateDataPartner(searchRequestKey, eventStatus.ProviderProfile.Name, eventName);
            }

            if (EventName.Completed.Equals(eventName) && eventStatus.ProviderProfile.SearchSpeedType == SearchSpeedType.Fast)
                await _deepSearchService.UpdateParameters(eventName, (PersonSearchCompleted)eventStatus, searchRequestKey);

            await ProcessWaveSearch(searchRequestKey, eventName, eventStatus.ProviderProfile.Name);

           
               
        }

        private async Task ProcessWaveSearch(string searchRequestKey, string eventName,string dataPartner )
        {
            if (!EventName.Finalized.Equals(eventName))
            {
                if (EventName.Completed.Equals(eventName)|| EventName.Rejected.Equals(eventName))
                {
                    if (await _deepSearchService.IsWaveSearchReadyToFinalize(searchRequestKey))
                    {
                        PersonSearchAdapterEvent finalizedSearch = new PersonSearchFinalizedEvent()
                        {
                            SearchRequestKey = searchRequestKey,
                            Message = "Search Request Finalized",
                            SearchRequestId = Guid.NewGuid(),
                            TimeStamp = DateTime.Now,
                            ProviderProfile = new ProviderProfileDetails { Name = dataPartner }
                        };
                        await NotifyEventAsync(searchRequestKey, (PersonSearchFinalized)finalizedSearch, EventName.Finalized, new CancellationToken());



                    }
                }
            }
        }

        private void LogException(Exception ex)
        {
            var httpRequestException = ex as HttpRequestException;
            if (httpRequestException != null)
            {
                var properties = new Dictionary<string, object>();
                properties.Add("InnerExceptionMsg", httpRequestException.InnerException?.Message);
                using (_logger.BeginScope(properties))
                {
                    _logger.LogError(ex, ex.Message);
                };

            }
            else
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
