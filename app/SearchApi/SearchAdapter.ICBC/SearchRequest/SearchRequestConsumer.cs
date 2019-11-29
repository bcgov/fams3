using System;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchAdapter.ICBC.SearchRequest.MatchFound;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using SearchApi.Core.Person.Contracts;
using SearchApi.Core.Person.Enums;

namespace SearchAdapter.ICBC.SearchRequest
{
    /// <summary>
    /// The SearchRequestConsumer consumes ICBC execute search commands, execute the search a publish a response back to the searchApi
    /// </summary>
    public class SearchRequestConsumer : IConsumer<PersonSearchOrdered>
    {

        private readonly ILogger<SearchRequestConsumer> _logger;
        private readonly ProviderProfile _profile;
        private readonly IValidator<Person> _personSearchValidator;


        public SearchRequestConsumer(
            IValidator<Person> personSearchValidator,
            IOptions<ProviderProfileOptions> profile,
            ILogger<SearchRequestConsumer> logger)
        {
            _personSearchValidator = personSearchValidator;
            _profile = profile.Value;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PersonSearchOrdered> context)
        {
            _logger.LogInformation($"Successfully handling new search request [{context.Message.Person}]");

            _logger.LogWarning("Currently under development, ICBC Adapter is generating FAKE results.");

            if (await ValidatePersonSearch(context))
            {
                await context.Publish<PersonSearchCompleted>(BuildFakeResult(context.Message));
            }
        }

        private async Task<bool> ValidatePersonSearch(ConsumeContext<PersonSearchOrdered> context)
        {

            _logger.LogDebug("Attempting to validate the personSearch");
            var validation = _personSearchValidator.Validate(context.Message.Person);

            if (validation.IsValid)
            {
                await context.Publish<PersonSearchAccepted>(new DefaultPersonSearchAccepted(context.Message.SearchRequestId, _profile));
            }
            else
            {
                _logger.LogInformation("PersonSearch does not have sufficient information for the adapter to proceed the search.");

                var rejectionEvent = new PersonSearchRejectedEvent(context.Message.SearchRequestId, _profile);

                validation.Errors.ToList().ForEach(x => rejectionEvent.AddValidationResult(new ValidationResult()
                {
                    PropertyName = x.PropertyName,
                    ErrorMessage = x.ErrorMessage
                }));

                await context.Publish<PersonSearchRejected>(rejectionEvent);
            }

            return validation.IsValid;

        }

        public PersonSearchCompleted BuildFakeResult(PersonSearchOrdered personSearchOrdered)
        {

            var person = new IcbcPersonBuilder()
                .WithFirstName(personSearchOrdered.Person.FirstName)
                .WithLastName(personSearchOrdered.Person.LastName)
                .WithDateOfBirth(personSearchOrdered.Person.DateOfBirth)
                .AddIdentifier(new ICBCIdentifier()
                {
                    Type = PersonalIdentifierType.DriverLicense,
                    EffectiveDate = DateTime.Now.AddDays(-365),
                    ExpirationDate = DateTime.Now.AddDays(365),
                    IssuedBy = "British Columbia",
                    SerialNumber = new Random().Next(0, 50000).ToString()
                })
                .Build();

            return new IcbcMatchFoundBuilder(personSearchOrdered.SearchRequestId).WithPerson(person).Build();

        }

    }
}