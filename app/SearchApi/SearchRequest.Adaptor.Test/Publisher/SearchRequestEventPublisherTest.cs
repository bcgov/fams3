using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SearchRequestAdaptor.Publisher;
using SearchRequestAdaptor.Publisher.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Test.Publisher
{
    public class SearchRequestEventPublisherTest
    {
        private SearchRequestEventPublisher _sut;

        private Mock<ISendEndpointProvider> _sendEndpointProviderMock;

        private Mock<IOptions<RabbitMqConfiguration>> _rabbitMqConfigurationMock;

        private Mock<ILogger<SearchRequestEventPublisher>> _loggerMock;

        private Mock<ISendEndpoint> _sendEndpointMock;
        private SearchRequestEvent _baseEvent;

        [SetUp]
        public void SetUp()
        {
            _sendEndpointProviderMock = new Mock<ISendEndpointProvider>();

            _rabbitMqConfigurationMock = new Mock<IOptions<RabbitMqConfiguration>>();

            _sendEndpointMock = new Mock<ISendEndpoint>();

            _loggerMock = new Mock<ILogger<SearchRequestEventPublisher>>();

            RabbitMqConfiguration rabbitMqConfiguration = new RabbitMqConfiguration();
            rabbitMqConfiguration.Host = "localhost";
            rabbitMqConfiguration.Port = 15672;
            rabbitMqConfiguration.Username = "username";
            rabbitMqConfiguration.Password = "password";

            _sendEndpointProviderMock
                .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                .Returns(Task.FromResult(_sendEndpointMock.Object));

            _rabbitMqConfigurationMock.Setup(x => x.Value).Returns(rabbitMqConfiguration);

            _sendEndpointMock.Setup(x => x.Send<SearchRequestFailedEvent>(It.IsAny<SearchRequestFailed>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_sendEndpointMock));

            _baseEvent = new FakeSearchRequestEvent() { SearchRequestKey = "key" };

            _sut = new SearchRequestEventPublisher(_sendEndpointProviderMock.Object, _rabbitMqConfigurationMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task PublishSearchRequestFailed_should_pubish_failedEvent()
        {
            await _sut.PublishSearchRequestFailed(_baseEvent, "failed");
            _sendEndpointMock.Verify(x => x.Send<SearchRequestFailed>(It.IsAny<SearchRequestFailedEvent>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });
        }

        [Test]
        public async Task PublishSearchRequestRejected_should_pubish_rejectedEvent()
        {
            List<ValidationResultData> validationResults = new List<ValidationResultData>() {
                new ValidationResultData(){ }
            };
            await _sut.PublishSearchRequestRejected(_baseEvent, validationResults);
            _sendEndpointMock.Verify(x => x.Send<SearchRequestRejected>(It.IsAny<SearchRequestRejectedEvent>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });

        }

        [Test]
        public async Task PublishSearchRequestRejected_should_pubish_rejectedEvent_when_valiationResults_is_null()
        {
            await _sut.PublishSearchRequestRejected(_baseEvent, null);
            _sendEndpointMock.Verify(x => x.Send<SearchRequestRejected>(It.IsAny<SearchRequestRejectedEvent>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });

        }

        [Test]
        public async Task PublishSearchRequestSubmitted_should_pubish_submittedEvent()
        {
            SearchRequestSavedEvent submittedEvent = new SearchRequestSavedEvent();
            await _sut.PublishSearchRequestSaved(submittedEvent);
            _sendEndpointMock.Verify(x => x.Send<SearchRequestSaved>(It.IsAny<SearchRequestSavedEvent>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });
        }

        [Test]
        public void with_null_submittedEvent_PublishSearchRequestSubmitted_should_throw_exception()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.PublishSearchRequestSaved(null));
        }

        [Test]
        public void with_null_baseEvent_PublishSearchRequestFailed_should_throw_exception()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.PublishSearchRequestFailed(null, "failed"));
        }

        [Test]
        public void with_null_baseEvent_PublishSearchRequestRejected_should_throw_exception()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.PublishSearchRequestRejected(null, null));
        }
    }


}
