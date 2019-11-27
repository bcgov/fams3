using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using MassTransit.Context;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.Test.SearchRequest
{
    public class SearchRequestHandlerTest
    {

        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<SearchRequestConsumer> _sut;

        private Mock<ILogger<SearchRequestConsumer>> _loggerMock;
        private Mock<IOptions<ProviderProfileOptions>> _providerProfileMock;
        private Mock<IValidator<ExecuteSearch>> _personSearchValidatorMock;

        private Guid validGuid = Guid.NewGuid();
        private Guid inValidGuid = Guid.NewGuid();


            [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {

            _loggerMock = new Mock<ILogger<SearchRequestConsumer>>();
            _providerProfileMock = new Mock<IOptions<ProviderProfileOptions>>();
            _personSearchValidatorMock = new Mock<IValidator<ExecuteSearch>>();

            _personSearchValidatorMock.Setup(x => x.Validate(It.Is<ExecuteSearch>(x => x.Id == validGuid)))
                .Returns(new ValidationResult(Enumerable.Empty<ValidationFailure>()));

            _personSearchValidatorMock.Setup(x => x.Validate(It.Is<ExecuteSearch>(x => x.Id == inValidGuid)))
                .Returns(new ValidationResult(new List<ValidationFailure>()
                {
                    new ValidationFailure("firstName", "firstName is required.")
                }));

            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new SearchRequestConsumer(_personSearchValidatorMock.Object, _providerProfileMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send<ExecuteSearch>(new
            {
                Id = validGuid,
                FirstName = "firstName",
                LastName = "lastName",
                DateOfBirth = new DateTime(2001, 1, 1)
            });

            await _harness.InputQueueSendEndpoint.Send<ExecuteSearch>(new
            {
                Id = inValidGuid,
                FirstName = "",
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

        [Test]
        public void Should_send_a_search_rejected_event()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchRejected>().Any(x => x.Context.Message.SearchRequestId == inValidGuid));
        }

        [Test]
        public void Should_send_a_match_found_event()
        {
            Assert.IsTrue(_harness.Published.Select<SearchApi.Core.Adapters.Contracts.PersonFound>().Any(x => x.Context.Message.SearchRequestId == validGuid));
        }

        [Test]
        public void Should_send_a_search_accepted_event()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchAccepted>().Any(x => x.Context.Message.SearchRequestId == validGuid));
        }
    }
}