using AutoMapper;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchApi.Core.Adapters.Contracts;
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

        private Mock<ILogger<PersonSearchAcceptedConsumer>> _loggerMock;
        private Mock<ISearchApiNotifier<ProviderSearchEventStatus>> _searchApiNotifierMock;
        private Mock<IMapper> _mapper;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _loggerMock = LoggerUtils.LoggerMock<PersonSearchAcceptedConsumer>();
            _searchApiNotifierMock = new Mock<ISearchApiNotifier<ProviderSearchEventStatus>>();
            _harness = new InMemoryTestHarness();
            _mapper = new Mock<IMapper>();

            var fakePersonSearchStatus = new ProviderSearchEventStatus

            {
                EventType = "PersonSearchAccepted",
                Message = "Ok",
                ProviderName = "ICBC",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now
            };

            _mapper.Setup(m => m.Map<ProviderSearchEventStatus>(It.IsAny<PersonSearchAccepted>()))
                                .Returns(fakePersonSearchStatus);

            _sut = _harness.Consumer(() => new PersonSearchAcceptedConsumer(_searchApiNotifierMock.Object, _loggerMock.Object, _mapper.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<ProviderSearchEventStatus>(new
            {


                FirstName = "firstName",
                LastName = "lastName",
                DateOfBirth = new DateTime(2001, 1, 1)
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

    }
}
