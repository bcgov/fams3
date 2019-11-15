using System;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.SearchRequest.Models;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Test.FakeMessages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DynamicsAdapter.Web.Test.SearchRequest
{


    public class StatusReasonServiceTest
    {

        private StatusReasonService _sut;
        private Mock<IOptions<OAuthOptions>> _oauthOptionsMock;
        private readonly Mock<ILogger<StatusReasonService>> _loggerMock = new Mock<ILogger<StatusReasonService>>();


        [SetUp]
        public void SetUp()
        {

            _oauthOptionsMock = new Mock<IOptions<OAuthOptions>>();
        }

        [Test]
        public  async Task should_return_a_list_of_status_reason()
        {

            var options = new OAuthOptions()
            {
                ResourceUrl =  "http://localhost/getentityset/"
            };
            _oauthOptionsMock.Setup(x => x.Value).Returns(options);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(FakeHttpMessageResponse.GetList())
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            _sut = new StatusReasonService(httpClient, _oauthOptionsMock.Object, _loggerMock.Object);

            await _sut.GetListAsync( CancellationToken.None);

            var expectedUri = new Uri($"http://localhost/getentityset/");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(0),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get
                        && req.RequestUri == expectedUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

       
    
}
}
