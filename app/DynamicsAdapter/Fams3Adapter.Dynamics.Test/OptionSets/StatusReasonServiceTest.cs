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

        [Test]
        public void GetAllOptions_with_null_string_it_should_throw_ArgumentNullException()
        {
            _sut = new OptionSetService(new HttpClient(), _loggerMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _sut.GetAllOptions(null, CancellationToken.None));
        }

        [Test]
        public void GetStatusCode_with_null_string_it_should_throw_ArgumentNullException()
        {
            _sut = new OptionSetService(new HttpClient(), _loggerMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _sut.GetAllStatusCode(null, CancellationToken.None));
        }

        [Test]
        public void GetAllOptions_with_empty_string_it_should_throw_ArgumentNullException()
        {
            _sut = new OptionSetService(new HttpClient(), _loggerMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _sut.GetAllOptions(string.Empty, CancellationToken.None));
        }

        [Test]
        public void GetStatusCode_with_empty_string_it_should_throw_ArgumentNullException()
        {
            _sut = new OptionSetService(new HttpClient(), _loggerMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _sut.GetAllStatusCode(string.Empty, CancellationToken.None));
        }

        [Test]
        public async Task GetAllBankAccountTypes_should_return_listof_optionset()
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
                .ReturnsAsync(GetFakeHttpResponseMessageBank())
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = expectedUri;
            _sut = new OptionSetService(httpClient, _loggerMock.Object);
            var result = await _sut.GetAllBankAccountTypes(CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.All(x => !string.IsNullOrEmpty(x.Name) && x.Value > 0));
        }

        public static HttpResponseMessage GetFakeHttpResponseMessage()
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = GetFakeStatusReasonContent()
            };

        }

        public static HttpResponseMessage GetFakeHttpResponseMessageBank()
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = GetFakeBankAccountTypesContent()
            };

        }

        public static HttpContent GetFakeStatusReasonContent()
        {
            return new StringContent(JsonConvert.SerializeObject(GetFakeValidReason()), Encoding.UTF8, "application/json");
        }

        public static HttpContent GetFakeBankAccountTypesContent()
        {
            return new StringContent(JsonConvert.SerializeObject(GetFakeBankAccountTypes()), Encoding.UTF8, "application/json");
        }

        public static OptionSetMetadata GetFakeValidReason()
        {
            return new OptionSetMetadata()
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

        public static BankAccountTypeOptionSet GetFakeBankAccountTypes()
        {
            return new BankAccountTypeOptionSet()
            {
                Value = new List<OptionSetMetadata>
                {
                    new OptionSetMetadata
                    {
                        OptionSet = new OptionSet()
                        {
                            Options = new List<Option>()
                            {
                                new Option() { Value = 1, Label = new Label { UserLocalizedLabel = new UserLocalizedLabel { Label = "type1" } } },
                                new Option() { Value = 867670000, Label = new Label { UserLocalizedLabel = new UserLocalizedLabel { Label = "type2" } } },
                                new Option() { Value = 867670001, Label = new Label { UserLocalizedLabel = new UserLocalizedLabel { Label = "type3" } } },
                                new Option() { Value = 2, Label = new Label { UserLocalizedLabel = new UserLocalizedLabel { Label = "type4" } } }
                            }
                        }
                    }
                }
            };
        }

    }
}
