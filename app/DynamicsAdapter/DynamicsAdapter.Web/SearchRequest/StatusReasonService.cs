using DynamicsAdapter.Web.SearchRequest.Models;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz.Logging;

namespace DynamicsAdapter.Web.SearchRequest
{
    public interface IStatusReasonService
    {
        Task<StatusReason> GetListAsync(CancellationToken cancellationToken);
    }

    public class StatusReasonService : IStatusReasonService
    {
        private readonly HttpClient _httpClient;
        private readonly OAuthOptions _oauthOptions;
        private readonly ILogger<StatusReasonService> _logger;

        public StatusReasonService(HttpClient httpClient, IOptions<OAuthOptions> oauthOptions,ILogger<StatusReasonService> logger)
        { 
            this._httpClient = httpClient;

            _oauthOptions = oauthOptions.Value;
            _logger = logger;
        }

        public async Task<StatusReason> GetListAsync(CancellationToken cancellationToken)
        {


            TryCreateUri(_oauthOptions.ResourceUrl, "EntityDefinitions(LogicalName='ssg_searchrequest')/Attributes(LogicalName='statuscode')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet", out var endpoint);


            _logger.LogDebug(
                $"The status reason service endpoint for ssg_searchrequest {endpoint}");
            using var request = new HttpRequestMessage();
            try
            {
                request.Method = HttpMethod.Get;
                request.Headers.Accept.Add(
                    System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                request.RequestUri = endpoint;
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogDebug(
                        $" {endpoint} failed with status code : {response.StatusCode}");
                    return await Task.FromResult(new StatusReason());
                }
                _logger.LogDebug(
                    $" {endpoint} succeeded with status code : {response.StatusCode}");
                var responseData = await response.Content.ReadAsStringAsync();
                return await Task.FromResult(JsonConvert.DeserializeObject<StatusReason>(responseData));

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message); 
            }
           

        }

        public static bool TryCreateUri(string baseUrl, string path, out Uri uri)
        {
            uri = null;
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            {
                return false;
            }

            return Uri.TryCreate(baseUri, path, out uri);
        }

    }
}
