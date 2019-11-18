using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Simple.OData.Client;

namespace Fams3Adapter.Dynamics.SearchApiRequest
{

    public interface ISearchApiRequestService
    {
        Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchApiRequestService : ISearchApiRequestService
    {
        private const int ReadyForSearchStatus = 1;

        private readonly IODataClient _oDataClient;
        public SearchApiRequestService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }

        /// <summary>
        /// Gets all the search request in `Ready for Search` status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken)
        { 
            return await this._oDataClient.For<SSG_SearchApiRequest>().Filter(x => x.StatusCode == ReadyForSearchStatus).FindEntriesAsync(cancellationToken);
        }

    }
}