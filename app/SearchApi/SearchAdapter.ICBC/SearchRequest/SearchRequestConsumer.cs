using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchAdapter.ICBC.SearchRequest.MatchFound;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{
    /// <summary>
    /// The SearchRequestConsumer consumes ICBC execute search commands, execute the search a publish a response back to the searchApi
    /// </summary>
    public class SearchRequestConsumer : IConsumer<ExecuteSearch>
    {

        private readonly ILogger<SearchRequestConsumer> _logger;
        private readonly ProviderProfile _profile;
        private readonly IValidator<ExecuteSearch> _personSearchValidator;
        

        public SearchRequestConsumer(
            IValidator<ExecuteSearch> personSearchValidator,
            IOptions<ProviderProfileOptions> profile,
            ILogger<SearchRequestConsumer> logger)
        {
            _personSearchValidator = personSearchValidator;
            _profile = profile.Value;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ExecuteSearch> context)
        {
            _logger.LogInformation($"Successfully handling new search request [{context.Message.Id}]");

            _logger.LogWarning("Currently under development, ICBC Adapter is generating FAKE results.");

            _logger.LogDebug("Attempting to validate the personSearch");
            var validation = _personSearchValidator.Validate(context.Message);

            if (!validation.IsValid)
            {
                _logger.LogInformation("PersonSearch does not have sufficient information for the adapter to proceed the search.");
                await context.Publish<PersonSearchRejected>(new PersonSearchRejectedEvent()
                {
                    RequestId = context.Message.Id,
                    ProviderProfile = _profile,
                    Reasons = validation.Errors.Select(x => new ValidationResult()
                    {
                        PropertyName = x.PropertyName,
                        ErrorMessage = x.ErrorMessage
                    })
                });
                return;
            }

            await context.Publish<SearchApi.Core.Adapters.Contracts.MatchFound>(BuildFakeResult(context.Message));
        }


        public SearchApi.Core.Adapters.Contracts.MatchFound BuildFakeResult(ExecuteSearch executeSearch)
        {


            var fakeIdentifier = new IcbcPersonIdBuilder(PersonIDKind.DriverLicense).WithIssuer("British Columbia")
                .WithNumber("1234568").Build();

            var person = new IcbcPersonBuilder().WithFirstName(executeSearch.FirstName)
                .WithFirstName(executeSearch.FirstName).WithDateOfBirth(executeSearch.DateOfBirth).Build();


            return new IcbcMatchFoundBuilder(executeSearch.Id).WithPerson(person).AddPersonId(fakeIdentifier).Build();

        }

    }
}