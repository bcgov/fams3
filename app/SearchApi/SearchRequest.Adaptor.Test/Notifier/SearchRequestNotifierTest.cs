using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SearchRequestAdaptor.Configuration;
using SearchRequestAdaptor.Notifier;
using SearchRequestAdaptor.Publisher;
using SearchRequestAdaptor.Publisher.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Test.Notifier
{
    public class SearchRequestNotifierTest
    {
        private WebHookSearchRequestNotifier _sut;

        private Mock<IOptions<SearchRequestAdaptorOptions>> _searchRequestOptionsMock;

        private Mock<ILogger<WebHookSearchRequestNotifier>> _loggerMock;

        private Mock<ISearchRequestEventPublisher> _searchRquestEventPublisherMock;

        private Mock<HttpMessageHandler> _httpHandlerMock;

        private HttpClient _httpClient;

        private FakeSearchRequestOrdered _fakeSearchRequestOrdered;


        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<WebHookSearchRequestNotifier>>();
            _searchRequestOptionsMock = new Mock<IOptions<SearchRequestAdaptorOptions>>();
            _searchRquestEventPublisherMock = new Mock<ISearchRequestEventPublisher>();
            _httpHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _httpHandlerMock
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
                    Content = new StringContent(JsonConvert.SerializeObject(
                        new SearchRequestSavedEvent()
                        {
                            SearchRequestId = Guid.NewGuid(),
                            SearchRequestKey = "fileId",
                            RequestId = "requestId",
                            ProviderProfile = new SearchRequest.Adaptor.Publisher.Models.ProviderProfile() { Name = "AgencyName" }
                        })),
                })
                .Verifiable();

            _httpClient = new HttpClient(_httpHandlerMock.Object);
            _fakeSearchRequestOrdered = new FakeSearchRequestOrdered()
            {
                Action = BcGov.Fams3.SearchApi.Contracts.Person.RequestAction.NEW,
                RequestId = "id"
            };

            _searchRequestOptionsMock.Setup(x => x.Value).Returns(new SearchRequestAdaptorOptions().AddWebHook("test", "http://test:1234/"));
        }

        [Test]
        public async Task NotifySearchRequestEventAsync_should_send_httpRequest_to_one_subscribers_and_not_publish_saved()
        {
            _sut = new WebHookSearchRequestNotifier(_httpClient, _searchRequestOptionsMock.Object, _loggerMock.Object, _searchRquestEventPublisherMock.Object);


            await _sut.NotifySearchRequestEventAsync(
                _fakeSearchRequestOrdered.RequestId,
                _fakeSearchRequestOrdered, CancellationToken.None);

            var expectedUri = new Uri("http://test:1234/CreateSearchRequest/id");

            _httpHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri.AbsoluteUri == expectedUri.AbsoluteUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            _searchRquestEventPublisherMock.Verify(
                x => x.PublishSearchRequestSaved(
                It.IsAny<SearchRequestSavedEvent>()), Times.Never);
        }

        [Test]
        public async Task NotifySearchRequestEventAsync_update_should_send_httpRequest_to_one_subscribers_and_publish_saved()
        {
            _sut = new WebHookSearchRequestNotifier(_httpClient, _searchRequestOptionsMock.Object, _loggerMock.Object, _searchRquestEventPublisherMock.Object);
            _fakeSearchRequestOrdered.Action = BcGov.Fams3.SearchApi.Contracts.Person.RequestAction.UPDATE;
            _httpHandlerMock
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
                    Content = new StringContent(JsonConvert.SerializeObject(
                        new SearchRequestSavedEvent()
                        {
                            SearchRequestId = Guid.NewGuid(),
                            Action=BcGov.Fams3.SearchApi.Contracts.Person.RequestAction.UPDATE,
                            SearchRequestKey = "fileId",
                            RequestId = "requestId",
                            ProviderProfile = new SearchRequest.Adaptor.Publisher.Models.ProviderProfile() { Name = "AgencyName" }
                        })),
                })
                .Verifiable();

            await _sut.NotifySearchRequestEventAsync(
                _fakeSearchRequestOrdered.RequestId,
                _fakeSearchRequestOrdered, CancellationToken.None);

            var expectedUri = new Uri("http://test:1234/UpdateSearchRequest/id");

            _httpHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri.AbsoluteUri == expectedUri.AbsoluteUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            _searchRquestEventPublisherMock.Verify(
                x => x.PublishSearchRequestSaved(
                It.IsAny<SearchRequestSavedEvent>()), Times.Once);
        }


        [Test]
        public async Task NotifySearchRequestEventAsync_should_publish_failed_if_invalid_url_setting()
        {
            _searchRequestOptionsMock.Setup(x => x.Value).Returns(
                new SearchRequestAdaptorOptions().AddWebHook("test", "invalidUrl"));

            _sut = new WebHookSearchRequestNotifier(_httpClient, _searchRequestOptionsMock.Object, _loggerMock.Object, _searchRquestEventPublisherMock.Object);
            await _sut.NotifySearchRequestEventAsync(
                    _fakeSearchRequestOrdered.RequestId,
                    _fakeSearchRequestOrdered, CancellationToken.None);

            _httpHandlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(0),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );

            _searchRquestEventPublisherMock.Verify(
                x => x.PublishSearchRequestFailed(
                It.IsAny<SearchRequestEvent>(), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public async Task NotifySearchRequestEventAsync_should_publish_rejected_if_http_return_non_success()
        {
            _searchRequestOptionsMock.Setup(x => x.Value).Returns(
                 new SearchRequestAdaptorOptions().AddWebHook("test", "http://test.org"));
            _httpHandlerMock
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
                    Content = new StringContent("bad request!"),
                })
                .Verifiable();

            _sut = new WebHookSearchRequestNotifier(_httpClient, _searchRequestOptionsMock.Object, _loggerMock.Object, _searchRquestEventPublisherMock.Object);
            await _sut.NotifySearchRequestEventAsync(
                     _fakeSearchRequestOrdered.RequestId,
                     _fakeSearchRequestOrdered, CancellationToken.None);

            _httpHandlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );

            _searchRquestEventPublisherMock.Verify(
                x => x.PublishSearchRequestRejected(
                It.IsAny<SearchRequestEvent>(), It.IsAny<List<ValidationResult>>()), Times.Once);
        }

        [Test]
        public async Task NotifySearchRequestEventAsync_should_publish_failed_if_http_throw_exception()
        {
            _searchRequestOptionsMock.Setup(x => x.Value).Returns(
                   new SearchRequestAdaptorOptions().AddWebHook("test", "http://test.org"));
            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).Throws(new Exception("mock http throw exception"));

            _sut = new WebHookSearchRequestNotifier(_httpClient, _searchRequestOptionsMock.Object, _loggerMock.Object, _searchRquestEventPublisherMock.Object);
            await _sut.NotifySearchRequestEventAsync(
                     _fakeSearchRequestOrdered.RequestId,
                     _fakeSearchRequestOrdered, CancellationToken.None);

            _httpHandlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );

            _searchRquestEventPublisherMock.Verify(
                x => x.PublishSearchRequestFailed(
                It.IsAny<SearchRequestEvent>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void NotifySearchRequestEventAsync_should_throw_exception_when_searchREquestOrdered_is_null()
        {
            _sut = new WebHookSearchRequestNotifier(_httpClient, _searchRequestOptionsMock.Object, _loggerMock.Object, _searchRquestEventPublisherMock.Object);

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.NotifySearchRequestEventAsync(
                _fakeSearchRequestOrdered.RequestId,
                null, CancellationToken.None));

        }
    }
}
