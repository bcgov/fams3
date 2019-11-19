using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.Error;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.OptionSets
{
    public interface IOptionSetService
    {
        Task<IEnumerable<GenericOption>> GetAllStatusCode(string entityName, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllOptions<T>(CancellationToken cancellationToken) where T : Enumeration;
    }

    public class OptionSetService : IOptionSetService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OptionSetService> _logger;
     
        public OptionSetService(HttpClient httpClient, ILogger<OptionSetService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<GenericOption>> GetAllStatusCode(string entityName, CancellationToken cancellationToken)
        {
            var uri = new Uri(string.Format(Keys.GLOBAL_STATUS_CODE_URL_TEMPLATE, entityName), UriKind.RelativeOrAbsolute);
            var result = JsonConvert.DeserializeObject<StatusReason>(await GetAllOptions(uri, cancellationToken));
            return result?.OptionSet?.Options == null ? new List<GenericOption>() : result.OptionSet.Options.Where(x => x.Label?.UserLocalizedLabel?.Label != null).Select(x => new GenericOption(x.Value, x.Label.UserLocalizedLabel.Label));
        }

        public async Task<IEnumerable<T>> GetAllOptions<T>(CancellationToken cancellationToken) where T : Enumeration
        {
            var uri = new Uri(string.Format(Keys.GLOBAL_OPTIONS_SET_DEFINTION_URL_TEMPLATE, typeof(T).Name.ToLower()), UriKind.RelativeOrAbsolute);
            var result = JsonConvert.DeserializeObject<OptionSet>(await GetAllOptions(uri, cancellationToken));
            return result?.Options == null ? new List<T>() : result.Options.Where(x => x.Label?.UserLocalizedLabel?.Label != null).Select(x => (T)Activator.CreateInstance(typeof(T), x.Value, x.Label.UserLocalizedLabel.Label));
        }

        private async Task<string> GetAllOptions(Uri relativeUri, CancellationToken cancellationToken)
        {

            using var request = new HttpRequestMessage { Method = HttpMethod.Get };
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
            request.RequestUri = relativeUri;

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{request.RequestUri} failed with status code : {response.StatusCode}");
                var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new DynamicsApiException(
                    "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
                    (int)response.StatusCode, responseData, response.Headers.ToDictionary(h => h.Key, h => h.Value),
                    null);
            }

            _logger.LogInformation($"{request.RequestUri} succeeded with status code : {response.StatusCode}");

            return await response.Content.ReadAsStringAsync();
            
        }

    }
}