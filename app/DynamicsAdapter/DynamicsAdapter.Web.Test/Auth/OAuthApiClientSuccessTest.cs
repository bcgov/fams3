using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Auth
{
    public class OAuthApiClientSuccessTest
    {
        private OAuthApiClient _sut;
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private Mock<IOptionsMonitor<OAuthOptions>> _optionsMock = new Mock<IOptionsMonitor<OAuthOptions>>();

        [SetUp]
        public void SetUp()
        {

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{\"access_token\": \"token\",\"token_type\": \"bearer\",\"expires_in\": 3600,\"resource\": \"https://resource\",\"refresh_token\": \"refresh_token\",\"refresh_token_expires_in\": 28799,\"scope\": \"openid\",\"id_token\": \"token_id\"}"),
                })
                .Verifiable();

            _optionsMock.Setup(x => x.CurrentValue).Returns(new OAuthOptions()
            {
                Secret = "secret",
                ResourceUrl = "resourceUrl",
                Password = "password",
                ClientId = "clientId",
                Username = "username",
                OAuthUrl = "oauthurl"
            });

            // use real http client with mocked handler here
            _httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            _sut = new OAuthApiClient(_httpClient, _optionsMock.Object);
        }

        [Test]
        public async Task When_success_response_it_should_return_a_token()
        {
            var token = await _sut.GetRefreshToken(CancellationToken.None);
            Assert.AreEqual("token", token.AccessToken);
            Assert.AreEqual("bearer", token.TokenType);
            Assert.AreEqual(3600, token.ExpiresIn);
            Assert.AreEqual("https://resource", token.Resource);
            Assert.AreEqual(28799, token.RefreshTokenExpiresIn);
            Assert.AreEqual("openid", token.Scope);
            Assert.AreEqual("token_id", token.IdToken);
        }
    }
}