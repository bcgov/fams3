using DynamicsAdapter.Web.SearchRequest.Models;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest
{
    public interface IStatusReasonService
    {
        Task<StatusReason> GetList(CancellationToken cancellationToken);
    }

    public class StatusReasonService : IStatusReasonService
    {
        private readonly HttpClient _httpClient;
        public StatusReasonService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<StatusReason> GetList(CancellationToken cancellationToken)
        {
            string requestUrl = "EntityDefinitions(LogicalName='ssg_searchrequest')/Attributes(LogicalName='statuscode')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet";

            var res = await _httpClient.GetAsync(requestUrl, cancellationToken);

            return await Task.FromResult(new StatusReason());
        }
    }
}
