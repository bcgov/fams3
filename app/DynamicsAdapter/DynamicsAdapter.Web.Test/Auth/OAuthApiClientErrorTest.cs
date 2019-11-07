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
    public class OAuthApiClientErrorTest
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
                    StatusCode = HttpStatusCode.BadRequest,
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
        public void When_error_response_it_should_throw_an_exception()
        {
            Assert.ThrowsAsync<OAuthApiException>( async () =>
            {
                var token = await _sut.GetRefreshToken(CancellationToken.None);
            });

        }
    }
}