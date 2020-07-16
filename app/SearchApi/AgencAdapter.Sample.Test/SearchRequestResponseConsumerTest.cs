using AgencyAdapter.Sample.SearchRequest;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgencAdapter.Sample.Test
{
    public class SearchRequestResponseConsumerTest
    {

        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<SearchRequestResponseConsumer> _sut;

        private Mock<ILogger<SearchRequestResponseConsumer>> _loggerMock;
       
      

        private Guid validGuid = Guid.NewGuid();
 

    

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {

            _loggerMock = new Mock<ILogger<SearchRequestResponseConsumer>>();
           
            


            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new SearchRequestResponseConsumer( _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<SearchRequestNotification>(new SearchRequestNotificationSample()
            {
                ProviderProfile = new AgencySample { Name = "FEMP" },
                SearchRequestId = validGuid,
                SearchRequestKey = "SearchRequestKey",
                RequestId = "RequestId",
                TimeStamp = DateTime.Now,

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
            Assert.IsTrue(_harness.Published.Select<SearchRequestNotification>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_publish_recieved()
        {
            Assert.IsTrue(_harness.Consumed.Select<SearchRequestNotification>().Any());
        }

        
    }
}
