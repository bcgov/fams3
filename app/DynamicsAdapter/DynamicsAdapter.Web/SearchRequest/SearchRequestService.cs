using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Services.Dynamics.Model;
using Simple.OData.Client;

namespace DynamicsAdapter.Web.SearchRequest
{
    public interface ISearchRequestService
    {
        Task<IEnumerable<SSG_SearchRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken);
    }

    public class SearchRequestService : ISearchRequestService
    {
        private readonly ODataClient _oDataClient;
        public SearchRequestService(ODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }

        public async Task<IEnumerable<SSG_SearchRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken)
        { 
            return await this._oDataClient.For<SSG_SearchRequest>().FindEntriesAsync(cancellationToken);
        }

    }
}