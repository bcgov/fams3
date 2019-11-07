using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Auth
{
    public class OAuthHandler : DelegatingHandler
    {

        private readonly ITokenService _tokenService;

        public OAuthHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetToken(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}