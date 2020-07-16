using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Notifier
{
    public interface ISearchRequestNotifier<T>
    {
        Task NotifySearchRequestEventAsync(string searchRequestKey, T notificationStatus, string eventName, CancellationToken cancellationToken);
    }

    public class WebHookSearchRequestNotifier : ISearchRequestNotifier<SearchRequestEvent>
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<WebHookSearchRequestNotifier> _logger;


        public WebHookSearchRequestNotifier(HttpClient httpClient, ILogger<WebHookSearchRequestNotifier> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task NotifySearchRequestEventAsync(string searchRequestKey, SearchRequestEvent searchRequestEvent, string eventName,
           CancellationToken cancellationToken)
        {
            _logger.LogInformation("got searchRequestEvent");

        }
    }
}
