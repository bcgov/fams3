using BcGov.Fams3.Utils.Url;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.SearchAgency.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency.Webhook
{
    public interface IAgencyNotificationWebhook<T>
    {
        Task SendNotificationAsync(T notification, CancellationToken cancellationToken);
    }

    public class AgencyNotificationWebhook : IAgencyNotificationWebhook<SearchRequestNotification>
    {

        private readonly HttpClient _httpClient;
        private readonly AgencyNotificationOptions _agencyNotificationOptions;
        private readonly ILogger<AgencyNotificationWebhook> _logger;

        public AgencyNotificationWebhook(HttpClient httpClient, IOptions<AgencyNotificationOptions> agencyNotificationOptions, ILogger<AgencyNotificationWebhook> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _agencyNotificationOptions = agencyNotificationOptions.Value;
        }

        public async Task SendNotificationAsync(SearchRequestNotification srNotification,
           CancellationToken cancellationToken)
        {
            var webHookName = "AgencyNotification";

            if (srNotification == null) throw new ArgumentNullException(nameof(srNotification));

            foreach (var webHook in _agencyNotificationOptions.WebHooks)
            {
                _logger.LogDebug(
                   $"The webHook {webHookName} notification is attempting to send notification to Agency");

                if (!URLHelper.TryCreateUri(webHook.Uri, "", "", out var endpoint))
                {
                    _logger.LogWarning(
                        $"The webHook {webHookName} notification uri is not established or is not an absolute Uri for {webHook.Name}. Set the WebHook.Uri value on SearchApi.WebHooks settings.");

                    throw new Exception($"The webHook { webHookName } notification uri is not established");
                }

                using var request = new HttpRequestMessage();

                string temp = JsonConvert.SerializeObject(srNotification);
                StringContent content = new StringContent(JsonConvert.SerializeObject(srNotification));

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
                        $"The webHook {webHookName} notification has sent notification successfully for {webHook.Name} webHook. The error code is {response.StatusCode.GetHashCode()}.");
                    throw new Exception("Send notification to request.Api failed.");
                }

                _logger.LogInformation("send notification to request.Api successfully from webhook.");

            }
        }
    }
}
