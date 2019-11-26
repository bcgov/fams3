using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Web.Notifications
{

    public class WebHookNotifierPersonSearchAccepted : BaseApiNotifier, ISearchApiNotifier<PersonSearchAccepted>
    {

        private readonly HttpClient _httpClient;
        private readonly SearchApiOptions _searchApiOptions;
        private readonly ILogger<WebHookNotifierPersonSearchAccepted> _logger;
        private readonly IMapper _mapper;

        public WebHookNotifierPersonSearchAccepted(HttpClient httpClient, IOptions<SearchApiOptions> searchApiOptions,
            ILogger<WebHookNotifierPersonSearchAccepted> logger, Mapper mapper)
        {
            _httpClient = httpClient;
            _logger = logger;
            _searchApiOptions = searchApiOptions.Value;
            _mapper = mapper;
        }

        public async Task NotifyEventAsync(Guid searchRequestId, PersonSearchAccepted personSearchAccepted,
           CancellationToken cancellationToken)
        {
            var webHookName = "PersonSearch";
            foreach (var webHook in _searchApiOptions.WebHooks.FindAll(x => x.EventName.Contains(webHookName)))
            {
                _logger.LogDebug(
                   $"The webHook {webHookName}_Accepted notification is attempting to send event for {webHook.Name} webhook.");

                if (!TryCreateUri(webHook.Uri, $"{searchRequestId}", out var endpoint))
                {
                    _logger.LogWarning(
                        $"The webHook {webHookName}_Accepted notification uri is not established or is not an absolute Uri for {webHook.Name}. Set the WebHook.Uri value on SearchApi.WebHooks settings.");
                    return;
                }

                using var request = new HttpRequestMessage();

                try
                {
                    var providerSearchEvent = _mapper.Map<ProviderSearchEvent>(personSearchAccepted);
                    var content = new StringContent(JsonConvert.SerializeObject(providerSearchEvent));
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
                            $"The webHook {webHookName}_Accepted notification has not executed successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                        return;
                    }

                    _logger.LogInformation(
                        $"The webHook {webHookName}_Accepted notification has executed successfully for {webHook.Name} webHook.");

                }
                catch (Exception exception)
                {
                    _logger.LogError($"The failure notification for {webHook.Name}_Accepted has not executed successfully.",
                        exception);
                }
            }



        }

    }
}
