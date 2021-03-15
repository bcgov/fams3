using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using BcGov.Fams3.SearchApi.Core.Configuration;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAdapter.Sample.SearchRequest
{
    /// <summary>
    /// The SearchRequestConsumer consumes search ordered events, execute the search a publish a response back to the searchApi
    /// </summary>
    public class SearchRequestConsumer : IConsumer<PersonSearchOrdered>
    {

        private readonly ILogger<SearchRequestConsumer> _logger;
        private readonly ProviderProfile _profile;
        private readonly RetryConfiguration _retryConfigration;
        private readonly IValidator<Person> _personSearchValidator;
        private int count = 0;

        public SearchRequestConsumer(
            IValidator<Person> personSearchValidator,
            IOptions<ProviderProfileOptions> profile,
            IOptions<RetryConfiguration> retryConfiguration,
            ILogger<SearchRequestConsumer> logger)
        {
            _personSearchValidator = personSearchValidator;
            _profile = profile.Value;
            _retryConfigration = retryConfiguration.Value;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PersonSearchOrdered> context)
        {
            try
            {
                _logger.LogInformation($"Successfully handling new search request [{context.Message.Person}]");

                _logger.LogWarning("Sample Adapter, do not use in PRODUCTION.");

                if (await ValidatePersonSearch(context))
                {

                    count++;
                    if (string.Equals(context.Message.Person.FirstName, "exception", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception("Exception from Sample Adapter, Your name is no exception");

                    }
                    _logger.LogDebug(count.ToString());
                    await context.Publish(FakePersonBuilder.BuildFakePersonSearchCompleted(context.Message.SearchRequestId, context.Message.SearchRequestKey, context.Message.Person.FirstName, context.Message.Person.LastName, (DateTime)context.Message.Person.DateOfBirth, _profile));


                }
            }
            catch (Exception ex)
            {
                if (context.GetRetryAttempt() == _retryConfigration.RetryTimes)
                    await context.Publish(FakePersonBuilder.BuildFakePersonFailed(_profile, context.Message.SearchRequestId, context.Message.SearchRequestKey, ex.Message));
                else
                    throw new Exception(ex.Message);

            }

        }

        private async Task<bool> ValidatePersonSearch(ConsumeContext<PersonSearchOrdered> context)
        {

            _logger.LogDebug("Attempting to validate the personSearch");
            var validation = _personSearchValidator.Validate(context.Message.Person);

            if (validation.IsValid)
            {
                await context.Publish<PersonSearchAccepted>(new DefaultPersonSearchAccepted(context.Message.SearchRequestId, _profile, context.Message.SearchRequestKey));
            }
            else
            {
                _logger.LogInformation("PersonSearch does not have sufficient information for the adapter to proceed the search.");

                var rejectionEvent = new PersonSearchRejectedEvent(context.Message.SearchRequestId, context.Message.SearchRequestKey, _profile);

                validation.Errors.ToList().ForEach(x => rejectionEvent.AddValidationResult(new DefaultValidationResult()
                {
                    PropertyName = x.PropertyName,
                    ErrorMessage = x.ErrorMessage
                }));

                await context.Publish<PersonSearchRejected>(rejectionEvent);
            }

            return validation.IsValid;

        }

    }
}