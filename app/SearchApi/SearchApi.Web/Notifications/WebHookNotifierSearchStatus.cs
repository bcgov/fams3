using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Core.Adapters.Models;
using SearchApi.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Web.Notifications
{

    public class WebHookNotifierSearchStatus :  ISearchApiNotifier<ProviderSearchEventStatus>
    {

        private readonly HttpClient _httpClient;
        private readonly SearchApiOptions _searchApiOptions;
        private readonly ILogger<WebHookNotifierSearchStatus> _logger;


        public WebHookNotifierSearchStatus(HttpClient httpClient, IOptions<SearchApiOptions> searchApiOptions,
            ILogger<WebHookNotifierSearchStatus> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _searchApiOptions = searchApiOptions.Value;
            
        }

        public async Task NotifyEventAsync(Guid searchRequestId, ProviderSearchEventStatus eventStatus,
           CancellationToken cancellationToken)
        {
            var webHookName = "PersonSearch";
            foreach (var webHook in _searchApiOptions.WebHooks.FindAll(x => x.EventName.Contains(webHookName,StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogDebug(
                   $"The webHook {webHookName} notification is attempting to send status {eventStatus.EventType} event for {webHook.Name} webhook.");

                if (!URLHelper.TryCreateUri(webHook.Uri, $"{searchRequestId}", out var endpoint))
                {
                    _logger.LogWarning(
                        $"The webHook {webHookName} notification uri is not established or is not an absolute Uri for {webHook.Name}. Set the WebHook.Uri value on SearchApi.WebHooks settings.");
                    return;
                }

                using var request = new HttpRequestMessage();

                try
                {
               ;
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
                            $"The webHook {webHookName} notification has not executed status {eventStatus.EventType} successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                        return;
                    }

                    _logger.LogInformation(
                        $"The webHook {webHookName} notification has executed status {eventStatus.EventType} successfully for {webHook.Name} webHook.");

                }
                catch (Exception exception)
                {
                    _logger.LogError($"The failure notification for {webHook.Name} has not executed successfully.",
                        exception);
                }
            }



        }

    }
}
