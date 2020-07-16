using BcGov.Fams3.SearchApi.Contracts.Person;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace AgencyAdapter.Sample.SearchRequest
{

    public interface ISearchRequestPublisher
    {
        Task ProcessRequest(Models.SearchRequest request);
    }
    public class SearchRequestPublisher : ISearchRequestPublisher
    {

        private readonly ILogger<SearchRequestPublisher> _logger;

        private readonly IValidator<Models.SearchRequest> _searchRequestValidator;

        public SearchRequestPublisher(
          IValidator<Models.SearchRequest> searchRequestValidator,
          ILogger<SearchRequestPublisher> logger)
        {
            _searchRequestValidator = searchRequestValidator;

            _logger = logger;
        }

        public async Task ProcessRequest(Models.SearchRequest request)
        {
            _logger.LogDebug($"Start the process");

            _logger.LogWarning("Sample Agency Adapter, do not use in PRODUCTION.");

            if (await ValidatePersonSearch(request))
            {
                _logger.LogDebug($"Process and send to queue");
            }

            await Task.FromResult(0);
        }

        private async Task<bool> ValidatePersonSearch(Models.SearchRequest request)
        {

            _logger.LogDebug("Attempting to validate the Search Request");
            var validation = _searchRequestValidator.Validate(request);

            if (validation.IsValid)
            {
                _logger.LogDebug("Request is valid.");
                await Task.FromResult(0);
            }
            else
            {
                _logger.LogDebug("Request failed.");

                await Task.FromResult(0);
            }

            return validation.IsValid;

        }
    }
}
