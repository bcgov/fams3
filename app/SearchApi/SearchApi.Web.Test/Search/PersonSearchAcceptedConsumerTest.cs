using AutoMapper;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchApi.Core.Adapters.Models;
using SearchApi.Web.Notifications;
using SearchApi.Web.Search;
using SearchApi.Web.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApi.Web.Test.Search
{
    public class PersonSearchAcceptedConsumerTest
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<PersonSearchAcceptedConsumer> _sut;

        private Mock<ILogger<ProviderSearchEventStatus>> _loggerMock;
        private Mock<ISearchApiNotifier<ProviderSearchEventStatus>> _searchApiNotifierMock;
        private Mock<IMapper> _mapper;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _loggerMock = LoggerUtils.LoggerMock<ProviderSearchEventStatus>();
            _searchApiNotifierMock = new Mock<ISearchApiNotifier<ProviderSearchEventStatus>>();
            _harness = new InMemoryTestHarness();
            _mapper = new Mock<IMapper>();
            _sut = _harness.Consumer(() => new PersonSearchAcceptedConsumer(_searchApiNotifierMock.Object, _loggerMock.Object, _mapper.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<ProviderSearchEventStatus>(new
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ProviderName = "WorksafeBC",
                Message = "Successful",
                EventType = "Accepted"
            }) ;

        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Published.Select<ProviderSearchEventStatus>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<ProviderSearchEventStatus>().Any());
        }
    }
}
