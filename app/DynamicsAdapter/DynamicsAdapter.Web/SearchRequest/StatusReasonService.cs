using DynamicsAdapter.Web.SearchRequest.Models;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IODataClient _oDataClient;
        public StatusReasonService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }

        public async Task<StatusReason> GetList(CancellationToken cancellationToken)
        {
            string commandText = "EntityDefinitions(LogicalName='ssg_searchrequest')/Attributes(LogicalName='statuscode')/Microsoft.Dynamics.CRM.StatusAttributeMetadata?$select=LogicalName&$expand=OptionSet";
   
            var res = await _oDataClient.FindEntriesAsync(commandText, cancellationToken);

            return await Task.FromResult(new StatusReason());
        }
    }
}
