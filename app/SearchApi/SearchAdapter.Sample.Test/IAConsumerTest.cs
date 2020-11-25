using BcGov.Fams3.SearchApi.Contracts.IA;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchAdapter.Sample.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAdapter.Sample.Test
{
    public class IAConsumerTest
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<IASearchConsumer> _sut;

        private Mock<ILogger<IASearchConsumer>> _loggerMock;


        public class IAOrderedTest : IASearchOrdered
        {
            public Guid SearchRequestId { get; set; }
            public DateTime TimeStamp { get; set; }
            public Person Person { get; set; }
            public string SearchRequestKey { get; set; }

            public string BatchNo { get; set; }

        }

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {

            _loggerMock = new Mock<ILogger<IASearchConsumer>>();
      

        

            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new IASearchConsumer(_loggerMock.Object));

            await _harness.Start();

            for(int x=0; x<=25;x++)
            {
                var y = ((x > 16) && (x % 2 > 0)) ? "" : "1";
                await _harness.BusControl.Publish<IASearchOrdered>(new IAOrderedTest()
                {
                    SearchRequestId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now,
                    BatchNo = "091212",
                    Person = new Person()
                    {
                        FirstName = "firstName",
                        LastName = "lastName",
                        MiddleName = "middleName",
                        OtherName = "otherName",
                        Identifiers = ((x > 16) && (x % 2 > 0)) ? null :  new List<PersonalIdentifier>()
                        {
                        new PersonalIdentifier {Type = PersonalIdentifierType.SocialInsuranceNumber, Value = "123123123123",    ReferenceDates  = new List<ReferenceDate>(){
                                    
                                }}
                        }
                    },
                    SearchRequestKey = "FileId",

                });
            }

           

       
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Published.Select<IASearchOrdered>().Any());
            Assert.IsTrue(_harness.Published.Select<IASearchFailed>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<IASearchOrdered>().Any());
        }

    }
}
