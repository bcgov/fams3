
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.Error;
using Fams3Adapter.Dynamics.OptionSets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicsAdapter.Web.SearchRequest
{
    public interface IStatusReasonService
    {
        Task<StatusReason> GetListAsync(CancellationToken cancellationToken);
    }

    public class StatusReasonService : IStatusReasonService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StatusReasonService> _logger;

        private const string _path =
            "EntityDefinitions(LogicalName='ssg_searchapirequest')/Attributes(LogicalName='statuscode')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet";

        public StatusReasonService(HttpClient httpClient, ILogger<StatusReasonService> logger)
        {
            this._httpClient = httpClient;
            _logger = logger;
        }

        public async Task<StatusReason> GetListAsync(CancellationToken cancellationToken)
        {

            _logger.LogDebug($"The status reason service endpoint for ssg_searchapirequest {_path}");

            using var request = new HttpRequestMessage {Method = HttpMethod.Get};
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
            request.RequestUri = new Uri(_path, UriKind.Relative);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug(
                    $" {_path} failed with status code : {response.StatusCode}");
                var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new DynamicsApiException(
                    "The HTTP status code of the response was not expected (" + (int) response.StatusCode + ").",
                    (int) response.StatusCode, responseData, response.Headers.ToDictionary(h => h.Key, h => h.Value),
                    null);
            }

            _logger.LogDebug(
                $" {_path} succeeded with status code : {response.StatusCode}");
            return JsonConvert.DeserializeObject<StatusReason>(await response.Content.ReadAsStringAsync());

        }

    }
}
