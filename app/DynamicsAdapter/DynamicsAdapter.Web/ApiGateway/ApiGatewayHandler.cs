using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using Microsoft.Extensions.Options;

namespace DynamicsAdapter.Web.ApiGateway
{
    public class ApiGatewayHandler : DelegatingHandler
    {

        public readonly ApiGatewayOptions _apiGatewayOptions;
        public readonly OAuthOptions _oAuthOptions;

        public ApiGatewayHandler(
            IOptions<ApiGatewayOptions> apiGatewayOptions,
            IOptions<OAuthOptions> oAuthOptions)
        {
            _apiGatewayOptions = apiGatewayOptions.Value;
            _oAuthOptions = oAuthOptions.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string requestUri = request.RequestUri.AbsoluteUri;
            request.RequestUri = new Uri(requestUri.Replace(_oAuthOptions.ResourceUrl, _apiGatewayOptions.BasePath));
            return await base.SendAsync(request, cancellationToken);
        }
    }
}