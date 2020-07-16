using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Notifier
{
    public interface ISearchRequestNotifier<T>
    {
        Task NotifyEventAsync(string searchRequestKey, T notificationStatus, string eventName, CancellationToken cancellationToken);
    }

    public class WebHookNotifierSearchRequest : ISearchRequestNotifier<SearchRequestEvent>
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<WebHookNotifierSearchRequest> _logger;


        public WebHookNotifierSearchRequest(HttpClient httpClient, ILogger<WebHookNotifierSearchRequest> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task NotifyEventAsync(string searchRequestKey, SearchRequestEvent searchRequestEvent, string eventName,
           CancellationToken cancellationToken)
        {
            //todo: following code is just to remove warning. When real code is in, need to remove it.
            await Task.Delay(1000);
        }
    }
}
