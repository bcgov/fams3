using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchAdapter.ICBC.SearchRequest.MatchFound;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{
    /// <summary>
    /// The SearchRequestConsumer consumes ICBC execute search commands, execute the search a publish a response back to the searchApi
    /// </summary>
    public class SearchRequestConsumer : IConsumer<ExecuteSearch>
    {

        private readonly ILogger<SearchRequestConsumer> _logger;

        public SearchRequestConsumer(ILogger<SearchRequestConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ExecuteSearch> context)
        {
            _logger.LogInformation($"Successfully handling new search request [{context.Message.Id}]");

            await context.Publish<SearchApi.Core.Adapters.Contracts.MatchFound>(BuildFakeResult(context.Message));
        }


        public SearchApi.Core.Adapters.Contracts.MatchFound BuildFakeResult(ExecuteSearch executeSearch)
        {

            _logger.LogWarning("Currently under development, ICBC Adapter is generating FAKE results.");

            var fakeIdentifier = new IcbcPersonIdBuilder(PersonIDKind.DriverLicense).WithIssuer("British Columbia")
                .WithNumber("1234568").Build();

            var person = new IcbcPersonBuilder().WithFirstName(executeSearch.FirstName)
                .WithFirstName(executeSearch.FirstName).WithDateOfBirth(executeSearch.DateOfBirth).Build();


            return new IcbcMatchFoundBuilder(executeSearch.Id).WithPerson(person).AddPersonId(fakeIdentifier).Build();

        }

    }
}