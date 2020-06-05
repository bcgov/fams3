using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SearchAdapter.Sample.SearchResult;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAdapter.Sample.Test
{
    public class SearchResultConsumerTest
    {

        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<SearchResultConsumer> _sut;

        private Mock<ILogger<SearchResultConsumer>> _loggerMock;
        private Mock<IOptions<ProviderProfileOptions>> _providerProfileMock;
      

        private Guid validGuid = Guid.NewGuid();
 

        public class PersonSearchReceivedTest : PersonSearchReceived
        {
            public Guid SearchRequestId { get; set; }
            public DateTime TimeStamp { get; set; }
            public PersonFound Person { get; set; }
            public string FileId { get; set; }
        }

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {

            _loggerMock = new Mock<ILogger<SearchResultConsumer>>();
            _providerProfileMock = new Mock<IOptions<ProviderProfileOptions>>();
            


            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new SearchResultConsumer( _providerProfileMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<PersonSearchReceived>(new PersonSearchReceivedTest()
            {
                SearchRequestId = validGuid,
                TimeStamp = DateTime.Now,
                Person = new PersonFound()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    DateOfBirth = new DateTime(2001, 1, 1)
                },
                FileId = "FileId"
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
            Assert.IsTrue(_harness.Published.Select<PersonSearchReceived>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_publish_recieved()
        {
            Assert.IsTrue(_harness.Consumed.Select<PersonSearchReceived>().Any());
        }


        [Test]
        public void Should_send_a_match_found_event()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchCompleted>().Any(x => x.Context.Message.SearchRequestId == validGuid));
        }

        
    }
}
