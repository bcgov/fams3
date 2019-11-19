using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Fams3Adapter.Dynamics.Test.OptionSets
{


    public class StatusReasonServiceTest
    {

        private OptionSetService _sut;
        private readonly Mock<ILogger<OptionSetService>> _loggerMock = new Mock<ILogger<OptionSetService>>();

        [Test]
        public  async Task should_return_a_list_of_status_reason()
        { 

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var expectedUri = new Uri($"http://test.com/");

            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(GetFakeHttpResponseMessage())
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = expectedUri;
            _sut = new OptionSetService(httpClient, _loggerMock.Object);

            var result = await _sut.GetAllStatusCode("testEntity", CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.All(x => !string.IsNullOrEmpty(x.Name) && x.Value > 0));

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

        public static HttpResponseMessage GetFakeHttpResponseMessage()
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = GetFakeStatusReasonContent()
            };

        }

        public static HttpContent GetFakeStatusReasonContent()
        {
            return new StringContent(JsonConvert.SerializeObject(GetFakeValidReason()), Encoding.UTF8, "application/json");
        }

        public static StatusReason GetFakeValidReason()
        {
            return new StatusReason()
            {
                OptionSet = new OptionSet()
                {
                    Options = new List<Option>()
                    {
                        new Option() { Value  = 1, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Ready For Search" }}},
                        new Option() { Value  = 867670000, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="In Progress" }}},
                        new Option() { Value  = 867670001, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Complete" }}},
                        new Option() { Value  = 2, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Other" }}}
                    }
                }
            };
        }

    }
}
