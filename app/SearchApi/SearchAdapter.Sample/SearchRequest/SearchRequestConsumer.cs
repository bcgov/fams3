using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using SearchApi.Core.Person.Contracts;
using SearchApi.Core.Person.Enums;

namespace SearchAdapter.Sample.SearchRequest
{
    /// <summary>
    /// The SearchRequestConsumer consumes search ordered events, execute the search a publish a response back to the searchApi
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

            _logger.LogWarning("Sample Adapter, do not use in PRODUCTION.");

            if (await ValidatePersonSearch(context))
            {

                if(string.Equals(context.Message.Person.FirstName, "exception", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Exception from Sample Adapter, Your name is no exception");

                await context.Publish<PersonSearchCompleted>(BuildFakePersonSearchCompleted(context.Message));
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

        public PersonSearchCompleted BuildFakePersonSearchCompleted(PersonSearchOrdered personSearchOrdered)
        {

            return new PersonSearchCompletedSample()
            {
                ProviderProfile = _profile,
                SearchRequestId = personSearchOrdered.SearchRequestId,
                TimeStamp = DateTime.Now,
                MatchedPerson = new PersonSample()
                {
                    FirstName = personSearchOrdered.Person.FirstName,
                    LastName = personSearchOrdered.Person.LastName,
                    DateOfBirth = personSearchOrdered.Person.DateOfBirth,
                    Identifiers = new List<PersonalIdentifierSample>()
                    {
                        new PersonalIdentifierSample()
                        {
                            Type = PersonalIdentifierType.DriverLicense,
                            EffectiveDate = DateTime.Now.AddDays(-365),
                            ExpirationDate = DateTime.Now.AddDays(365),
                            IssuedBy = "ICBC",
                            SerialNumber = new Random().Next(0, 50000).ToString()
                        }
                    },
                    Addresses = new List<PersonalAddressSample>() 
                    {
                        new PersonalAddressSample()
                        {
                            Type = "mailing",
                            AddressLine1 = "address in line 1",
                            AddressLine2 = "address in line 2",
                            Province = "British Columbia",
                            City = "victoria" ,
                            Country="canada",
                            PostalCode = "t4t4t4",
                            NonCanadianState="",
                            SuppliedBy = "ICBC"
                        },
                         new PersonalAddressSample()
                        {
                            Type = "residence",
                            AddressLine1 = "residence address in line 1",
                            AddressLine2 = "residence address in line 2",
                            Province = "British Columbia",
                            City = "vancouver" ,
                            Country="canada",
                            PostalCode = "5r5r5r",
                            NonCanadianState="",
                            SuppliedBy = "employer"
                        }
                    }
                }
            };

        }

    }
}