using System;
using System.IO;
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

        public ApiGatewayHandler(
            IOptions<ApiGatewayOptions> apiGatewayOptions)
        {
            _apiGatewayOptions = apiGatewayOptions.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(_apiGatewayOptions.BasePath)) return await base.SendAsync(request, cancellationToken);

            if (Uri.TryCreate(CombineUrls(_apiGatewayOptions.BasePath, request.RequestUri.AbsolutePath),  UriKind.Absolute, out var path))
            {
                request.RequestUri = path;
            }
            
            return await base.SendAsync(request, cancellationToken);
        }


        public static string CombineUrls(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(relativeUrl))
                return baseUrl;

            baseUrl = baseUrl.TrimEnd('/');
            relativeUrl = relativeUrl.TrimStart('/');

            return $"{baseUrl}/{relativeUrl}";
        }
    }
}