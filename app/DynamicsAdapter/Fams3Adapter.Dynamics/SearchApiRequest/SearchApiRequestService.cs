using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Simple.OData.Client;

using Entry = System.Collections.Generic.Dictionary<string, object>;


namespace Fams3Adapter.Dynamics.SearchApiRequest
{

    public interface ISearchApiRequestService
    {
        Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken);

        Task<SSG_SearchApiRequest> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken);
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
            return await _oDataClient.For<SSG_SearchApiRequest>().Filter(x => x.StatusCode == ReadyForSearchStatus).FindEntriesAsync(cancellationToken);
        }


        public async Task<SSG_SearchApiRequest> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken)
        {
            if(searchApiRequestId == default || searchApiRequestId == Guid.Empty) throw new ArgumentNullException(nameof(searchApiRequestId));

            return await _oDataClient
                .For<SSG_SearchApiRequest>()
                .Key(searchApiRequestId)
                .Set(new Entry {{ Keys.DYNAMICS_STATUS_CODE_FIELD, SearchApiRequestStatusReason.InProgress.GetHashCode() }})
                .UpdateEntryAsync(cancellationToken);
        }

    }
}