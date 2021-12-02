using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DynamicsAdapter.Web.Auth
{
    public interface IOAuthApiClient
    {
        Task<Token> GetRefreshToken(CancellationToken cancellationToken);
    }

    /// <summary>
    /// The OAuthApiClient interact with OAuth endpoing to manage refresh tokens.
    /// </summary>
    public class OAuthApiClient : IOAuthApiClient
    {
        private readonly HttpClient _httpClient;

        private readonly OAuthOptions _oAuthOptions;

        public OAuthApiClient(HttpClient httpClient, IOptionsMonitor<OAuthOptions> oAuthOptions)
        {
            this._httpClient = httpClient;
            this._oAuthOptions = oAuthOptions.CurrentValue;
        }

        public async Task<Token> GetRefreshToken(CancellationToken cancellationToken)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("client-request-id")){
                _httpClient.DefaultRequestHeaders.Remove("client-request-id");
            }
            _httpClient.DefaultRequestHeaders.Add("client-request-id", Guid.NewGuid().ToString());
            if (_httpClient.DefaultRequestHeaders.Contains("return-client-request-id")){
                _httpClient.DefaultRequestHeaders.Remove("return-client-request-id");
            }
            _httpClient.DefaultRequestHeaders.Add("return-client-request-id", "true");
            if (_httpClient.DefaultRequestHeaders.Contains("Accept")){
                _httpClient.DefaultRequestHeaders.Remove("Accept");
            }
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var data = new Dictionary<string, string>
            {
                {"resource", _oAuthOptions.ResourceUrl},
                {"client_id", _oAuthOptions.ClientId},
                {"client_secret", _oAuthOptions.Secret},
                {"username", _oAuthOptions.Username},
                {"password", _oAuthOptions.Password},
                {"scope", "openid"},
                {"response_mode", "form_post"},
                {"grant_type", "password"}
            };

            var content = new FormUrlEncodedContent(data);

            using (var request = new HttpRequestMessage(HttpMethod.Post, _oAuthOptions.OAuthUrl) {Content = content})
            {
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var responseData = response.Content == null
                        ? null
                        : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new OAuthApiException(
                        "The HTTP status code of the response was not expected (" + (int) response.StatusCode + ").",
                        (int) response.StatusCode, responseData,
                        response.Headers.ToDictionary(x => x.Key, x => x.Value), null);
                }


                var stream = await response.Content.ReadAsStreamAsync();

                using (StreamReader sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<Token>(reader);
                }
            }
        }

    }
}