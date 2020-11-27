using DynamicsAdapter.Web.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchApi
{
    public class SearchApiKeyHandler : DelegatingHandler
    {
        public readonly SearchApiConfiguration _searchApiConfig;

        public SearchApiKeyHandler(IOptions<SearchApiConfiguration> apiConfig)
        {
            _searchApiConfig = apiConfig.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-ApiKey", _searchApiConfig.ApiKey);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
