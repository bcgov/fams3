using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

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

                validation.Errors.ToList().ForEach(x => rejectionEvent.AddValidationResult(new DefaultValidationResult()
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
                    SecondName = personSearchOrdered.Person.SecondName,
                    ThirdName  = personSearchOrdered.Person.ThirdName,
                    Weight = personSearchOrdered.Person.Weight,
                    Height = personSearchOrdered.Person.Height,
                    HairColour = personSearchOrdered.Person.HairColour,
                    EyeColour =  personSearchOrdered.Person.EyeColour,
                    DateOfBirth = personSearchOrdered.Person.DateOfBirth,
                    Identifiers = new List<PersonalIdentifierSample>()
                    {

                        new PersonalIdentifierSample()
                        {
                            Type = PersonalIdentifierType.DriverLicense,
                            TypeCode = "BCDL",
                            Description = "Sample Identifier Description",
                            Notes = "Sample Identifier Notes",
                            IssuedBy = "BC",
                            Value = new Random().Next(0, 50000).ToString(),
                            ReferenceDates = new List<ReferenceDate>(){ 
                                new ReferenceDateSample(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDateSample(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                            }
                        }
                    },
                    Addresses = new List<AddressSample>()
                    {
                        new AddressSample()
                        {
                            Type = AddressType.Mailing,
                            AddressLine1 = "address in line 1",
                            AddressLine2 = "address in line 2",
                            AddressLine3 = "address in line 3",
                          ReferenceDates = new List<ReferenceDate>(){
                                new ReferenceDateSample(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDateSample(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                            },
                            Province = "British Columbia",
                            City = "victoria" ,
                            CountryRegion= "canada",
                            ZipPostalCode = "t4t4t4",
                            SuppliedBy = "ICBC",
                            TypeCode = "mailing"
                            
                        },
                         new AddressSample()
                        {
                            Type = AddressType.Residence,
                            AddressLine1 = "residence address in line 1",
                            AddressLine2 = "residence address in line 2",
                            Province = "British Columbia",
                            City = "vancouver" ,
                            CountryRegion="canada",
                            ZipPostalCode = "5r5r5r",
                            SuppliedBy = "employer"
                        }
                    },
                    Names = new List<NameSample>()
                    {
                        new NameSample()
                        {
                            TypeCode = "legal",
                            Type = NameType.Legal,
                            FirstName = "firstName",
                            LastName = "LastName",
                            SecondName = "MiddleName",
                           ThirdName = "ThirdName",
                            ReferenceDates = new List<ReferenceDate>(){
                                new ReferenceDateSample(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDateSample(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                            },
                            Description = "Sample Name"
                        }
                    }
                }
            };

        }

    }
}