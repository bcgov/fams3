using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Context;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.Test.SearchRequest
{
    public class SearchRequestHandlerTest
    {

        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<SearchRequestConsumer> _sut;

        private readonly Mock<ILogger<SearchRequestConsumer>> _loggerMock = new Mock<ILogger<SearchRequestConsumer>>();

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new SearchRequestConsumer(_loggerMock.Object));

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send<ExecuteSearch>(new
            {
                Id = Guid.NewGuid(),
                FirstName = "firstName",
                LastName = "lastName",
                DateOfBirth = new DateTime(2001, 1, 1)
            });

        }


        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Sent.Select<ExecuteSearch>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<ExecuteSearch>().Any());
        }


    }
}