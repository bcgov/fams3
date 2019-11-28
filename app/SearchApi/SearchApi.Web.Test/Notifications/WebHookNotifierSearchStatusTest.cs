using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using SearchApi.Core.OpenTracing;
using SearchApi.Core.Test.Fake;
using SearchApi.Web.Configuration;
using SearchApi.Web.Notifications;
using SearchApi.Web.Test.Utils;

namespace SearchApi.Web.Test.Notifications
{

   
        public class WebHookNotifierSearchStatusTest
        {
            private WebHookNotifierSearchEventStatus _sut;

            private Mock<IOptions<SearchApiOptions>> _searchApiOptionsMock;

            private Mock<ILogger<WebHookNotifierSearchEventStatus>> _loggerMock;

            private Mock<IMapper> _mapper;

            FakePersonSearchAdapterEvent fakePersonSearchStatus;

            [SetUp]
            public void SetUp()
            {
                _loggerMock = LoggerUtils.LoggerMock<WebHookNotifierSearchEventStatus>();
                _searchApiOptionsMock = new Mock<IOptions<SearchApiOptions>>();
                _mapper = new Mock<IMapper>();
                fakePersonSearchStatus = new FakePersonSearchAdapterEvent

                {

                    SearchRequestId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                };

                //_mapper.Setup(m => m.Map<ProviderSearchEventStatus>(It.IsAny<PersonSearchAccepted>()))
                //                    .Returns(fakePersonSearchStatus);
            }

            [Test]
            public async Task it_should_send_notification_to_one_subscribers()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234", "PersonSearch"));

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
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("worked!"),
                    })
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, fakePersonSearchStatus, CancellationToken.None);

                var expectedUri = new Uri($"http://test:1234/{fakePersonSearchStatus.SearchRequestId}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status PersonSearchAccepted successfully for test webHook.", "failed");

            }

            [Test]
            public async Task it_should_send_notification_to_one_subscribers_with_uri_having_path()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234/Event", "PersonSearch"));

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
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("worked!"),
                    })
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, fakePersonSearchStatus, CancellationToken.None);

                var expectedUri = new Uri($"http://test:1234/Event/{fakePersonSearchStatus.SearchRequestId}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status PersonSearchAccepted successfully for test webHook.", "failed");

            }


            [Test]
            public async Task it_should_send_notification_to_all_subscribers()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions()
                    .AddWebHook("test", "http://test:1234", "PersonSearch")
                    .AddWebHook("test2", "http://test:5678", "PersonSearch"));

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
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("worked!"),
                    })
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, fakePersonSearchStatus, CancellationToken.None);

                var expectedUri = new Uri($"http://test:1234/{fakePersonSearchStatus.SearchRequestId}");
                var expectedUri2 = new Uri($"http://test:5678/{fakePersonSearchStatus.SearchRequestId}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri2 // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status PersonSearchAccepted successfully for test webHook.", "failed");

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status PersonSearchAccepted successfully for test2 webHook.", "failed");

            }


            [Test]
            public async Task it_should_log_error_when_not_uri()
            {
                var fakeMatchFound = new FakePersonFound();

                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions()
                    .AddWebHook("test", "not_uri", "PersonSearch"));

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
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("worked!"),
                    })
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, fakePersonSearchStatus, CancellationToken.None);

                _loggerMock.VerifyLog(LogLevel.Warning, $"The webHook PersonSearch notification uri is not established or is not an absolute Uri for test. Set the WebHook.Uri value on SearchApi.WebHooks settings.", "log warning failed");

            }

            [Test]
            public async Task when_subscriber_return_bad_request_it_should_log_an_error()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234", "PersonSearch"));

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
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("worked!"),
                    })
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, fakePersonSearchStatus, CancellationToken.None);
                var expectedUri = new Uri($"http://test:1234/{fakePersonSearchStatus.SearchRequestId}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Error, $"The webHook PersonSearch notification has not executed status PersonSearchAccepted successfully for test webHook. The error code is 400.", "failed log error");

            }


            [Test]
            public async Task when_subscriber_return_unhautorized_it_should_log_an_error()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234", "PersonSearch"));

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
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StringContent("worked!"),
                    })
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, fakePersonSearchStatus, CancellationToken.None);
                var expectedUri = new Uri($"http://test:1234/{fakePersonSearchStatus.SearchRequestId}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Error, $"The webHook PersonSearch notification has not executed status PersonSearchAccepted successfully for test webHook. The error code is 401.", "failed log error");

            }


            [Test]
            public async Task when_httpClient_throw_exception_it_should_log_an_error()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234", "PersonSearch"));

                var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                handlerMock
                    .Protected()
                    // Setup the PROTECTED method to mock
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .Throws(new Exception("unknown error"))
                    .Verifiable();

                var httpClient = new HttpClient(handlerMock.Object);
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestId, new FakePersonSearchAdapterEvent(), CancellationToken.None);

                _loggerMock.VerifyLog(LogLevel.Error, $"The failure notification for test has not executed successfully.", "failed log error");

            }


        }
    
}