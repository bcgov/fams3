using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using FluentValidation;
using FluentValidation.Results;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SearchAdapter.Sample.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SearchAdapter.Sample.Test
{
    public class SearchRequestHandlerTest
    {

        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<SearchRequestConsumer> _sut;

        private Mock<ILogger<SearchRequestConsumer>> _loggerMock;
        private Mock<IOptions<ProviderProfileOptions>> _providerProfileMock;
        private Mock<IValidator<Person>> _personSearchValidatorMock;

        private Guid validGuid = Guid.NewGuid();
        private Guid inValidGuid = Guid.NewGuid();

        public class PersonSearchOrderedTest : PersonSearchOrdered
        {
            public Guid SearchRequestId { get; set; }
            public DateTime TimeStamp { get; set; }
            public Person Person { get; set; }
            public string SearchRequestKey { get; set; }
        }
        
        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {

            _loggerMock = new Mock<ILogger<SearchRequestConsumer>>();
            _providerProfileMock = new Mock<IOptions<ProviderProfileOptions>>();
            _personSearchValidatorMock = new Mock<IValidator<Person>>();

            _personSearchValidatorMock.Setup(x => x.Validate(It.Is<Person>(person => !string.IsNullOrEmpty(person.FirstName))))
                .Returns(new ValidationResult(Enumerable.Empty<ValidationFailure>()));

            _personSearchValidatorMock.Setup(x => x.Validate(It.Is<Person>(person => string.IsNullOrEmpty(person.FirstName))))
                .Returns(new ValidationResult(new List<ValidationFailure>()
                {
                    new ValidationFailure("firstName", "firstName is required.")
                }));

            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new SearchRequestConsumer(_personSearchValidatorMock.Object, _providerProfileMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<PersonSearchOrdered>(new PersonSearchOrderedTest()
            {
                SearchRequestId = validGuid,
                TimeStamp = DateTime.Now,
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    DateOfBirth = new DateTime(2001, 1, 1)
                },
                SearchRequestKey="SearchRequestKey"
            });

            await _harness.BusControl.Publish<PersonSearchOrdered>(new PersonSearchOrderedTest()
            {
                SearchRequestId = inValidGuid,
                TimeStamp = DateTime.Now,
                Person = new Person()
                {
                    FirstName = "",
                    LastName = "lastName",
                    DateOfBirth = new DateTime(2001, 1, 1)
                },
                SearchRequestKey = "SearchRequestKey"
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
            Assert.IsTrue(_harness.Published.Select<PersonSearchOrdered>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<PersonSearchOrdered>().Any());
        }

        [Test]
        public void Should_send_a_search_rejected_event()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchRejected>().Any(x => x.Context.Message.SearchRequestId == inValidGuid));
        }

        [Test]
        public void Should_send_a_match_found_event()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchAccepted>().Any(x => x.Context.Message.SearchRequestId == validGuid));
        }

        [Test]
        public void Should_send_a_search_accepted_event()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchAccepted>().Any(x => x.Context.Message.SearchRequestId == validGuid));
        }
    }
}