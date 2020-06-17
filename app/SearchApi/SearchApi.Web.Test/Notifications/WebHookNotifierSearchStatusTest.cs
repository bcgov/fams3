using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BcGov.Fams3.Redis;
using BcGov.Fams3.Redis.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SearchApi.Core.Test.Fake;
using SearchApi.Web.Configuration;
using SearchApi.Web.Notifications;
using SearchApi.Web.Test.Utils;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SearchApi.Web.Test.Notifications
{ 

    public class WebHookNotifierSearchStatusTest
        {
            private WebHookNotifierSearchEventStatus _sut;

            private Mock<IOptions<SearchApiOptions>> _searchApiOptionsMock;

            private Mock<ILogger<WebHookNotifierSearchEventStatus>> _loggerMock;

            private Mock<ICacheService> _cacheServiceMock;

            private Mock<IMapper> _mapper;

        private SearchRequest _allcompleted;
        private SearchRequest _notAllComplete;

        FakePersonSearchAccepted fakePersonSearchStatus;

            [SetUp]
            public void SetUp()
            {
                _loggerMock = LoggerUtils.LoggerMock<WebHookNotifierSearchEventStatus>();
                _searchApiOptionsMock = new Mock<IOptions<SearchApiOptions>>();
            _cacheServiceMock = new Mock<ICacheService>();

            _allcompleted = new SearchRequest
            {
                SearchRequestId = Guid.NewGuid(),
                DataPartners = new List<DataPartner>() {
                new DataPartner { Name = "ICBC", Completed = true },
                new DataPartner { Name = "BCHydro", Completed = true }
                }
            };
            
            _notAllComplete =
                new SearchRequest
                {
                    SearchRequestId = Guid.NewGuid(),
                    DataPartners = new List<DataPartner>() {
                new DataPartner { Name = "ICBC", Completed = true },
                new DataPartner { Name = "BCHydro", Completed = false }
                }
                };
                _mapper = new Mock<IMapper>();
                fakePersonSearchStatus = new FakePersonSearchAccepted()
                {
                    SearchRequestKey = "SearchRequestKey",
                    SearchRequestId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                };

            }

            [Test]
            public async Task it_should_send_notification_to_one_subscribers()
            {

                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Accepted",CancellationToken.None);

                var expectedUri = new Uri($"http://test:1234/Accepted/{fakePersonSearchStatus.SearchRequestKey}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri.AbsoluteUri == expectedUri.AbsoluteUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status Accepted successfully for test webHook.", "failed");

            }


        


        [Test]
        public async Task finalized_it_should_send_zero_notification_to_one_subscribers_when_not_all_dp_completed()
        {

            _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234"));

            _cacheServiceMock.Setup(x => x.GetRequest(It.IsAny<string>())).Returns(Task.FromResult(_notAllComplete));

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
            _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

            await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Finalized", CancellationToken.None);

            var expectedUri = new Uri($"http://test:1234/Finalized/{fakePersonSearchStatus.SearchRequestId}");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri.AbsoluteUri == expectedUri.AbsoluteUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

           
        }

        [Test]
        public async Task finalized_it_should_send_notification_to_one_subscribers_when_all_dp_completed()
        {
            _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234"));

            _cacheServiceMock.Setup(x => x.GetRequest(It.IsAny<string>())).Returns( Task.FromResult(_allcompleted));

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
            _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

            await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Finalized", CancellationToken.None);

            var expectedUri = new Uri($"http://test:1234/Finalized/{fakePersonSearchStatus.SearchRequestKey}");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri.AbsoluteUri == expectedUri.AbsoluteUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status Finalized successfully for test webHook.", "failed");

        }

        [Test]
            public async Task it_should_send_notification_to_one_subscribers_with_uri_having_path()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234/Event"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Accepted",CancellationToken.None);

                var expectedUri = new Uri($"http://test:1234/Event/Accepted/{fakePersonSearchStatus.SearchRequestKey}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status Accepted successfully for test webHook.", "failed");

            }


            [Test]
            public async Task it_should_send_notification_to_all_subscribers()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions()
                    .AddWebHook("test", "http://test:1234")
                    .AddWebHook("test2", "http://test:5678"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Accepted", CancellationToken.None);
            //
                var expectedUri = new Uri($"http://test:1234/Accepted/{fakePersonSearchStatus.SearchRequestKey}");
                var expectedUri2 = new Uri($"http://test:5678/Accepted/{fakePersonSearchStatus.SearchRequestKey}");

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

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status Accepted successfully for test webHook.", "failed");

                _loggerMock.VerifyLog(LogLevel.Information, $"The webHook PersonSearch notification has executed status Accepted successfully for test2 webHook.", "failed");

            }


            [Test]
            public async Task it_should_log_error_when_not_uri()
            {

                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions()
                    .AddWebHook("test", "not_uri"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Accepted",CancellationToken.None);

                _loggerMock.VerifyLog(LogLevel.Warning, $"The webHook PersonSearch notification uri is not established or is not an absolute Uri for test. Set the WebHook.Uri value on SearchApi.WebHooks settings.", "log warning failed");

            }

            [Test]
            public async Task when_subscriber_return_bad_request_it_should_log_an_error()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Accepted", CancellationToken.None);
                var expectedUri = new Uri($"http://test:1234/Accepted/{fakePersonSearchStatus.SearchRequestKey}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Error, $"The webHook PersonSearch notification has not executed status Accepted successfully for test webHook. The error code is 400.", "failed log error");

            }


            [Test]
            public async Task when_subscriber_return_unhautorized_it_should_log_an_error()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, fakePersonSearchStatus, "Accepted",CancellationToken.None);
                var expectedUri = new Uri($"http://test:1234/Accepted/{fakePersonSearchStatus.SearchRequestKey}");

                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post
                            && req.RequestUri == expectedUri // to this uri
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );

                _loggerMock.VerifyLog(LogLevel.Error, $"The webHook PersonSearch notification has not executed status Accepted successfully for test webHook. The error code is 401.", "failed log error");

            }


            [Test]
            public async Task when_httpClient_throw_exception_it_should_log_an_error()
            {


                _searchApiOptionsMock.Setup(x => x.Value).Returns(new SearchApiOptions().AddWebHook("test", "http://test:1234"));

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
                _sut = new WebHookNotifierSearchEventStatus(httpClient, _searchApiOptionsMock.Object, _loggerMock.Object, _cacheServiceMock.Object);

                await _sut.NotifyEventAsync(fakePersonSearchStatus.SearchRequestKey, new FakePersonSearchAdapterEvent(), "Accepted",CancellationToken.None);

                _loggerMock.VerifyLog(LogLevel.Error, "The webHook PersonSearch notification failed for status Accepted for test webHook. [unknown error]", "failed log error");

            }


        }
    
}