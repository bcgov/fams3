using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchRequest;
using Simple.OData.Client;

using Entry = System.Collections.Generic.Dictionary<string, object>;


namespace Fams3Adapter.Dynamics.SearchApiRequest
{

    public interface ISearchApiRequestService
    {
        Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken);

        Task<Guid> GetLinkedSearchRequestIdAsync(Guid searchApiRequestId,
            CancellationToken cancellationToken);

        Task<SSG_SearchApiRequest> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken);

        Task<SSG_SearchApiEvent> AddEventAsync(Guid searchApiRequestId, SSG_SearchApiEvent searchApiEvent,
            CancellationToken cancellationToken);

        
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchApiRequestService : ISearchApiRequestService
    {

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
        public async Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(
            CancellationToken cancellationToken)
        {
            int readyForSearchCode = SearchApiRequestStatusReason.ReadyForSearch.Value;
            List<SSG_SearchApiRequest> results = new List<SSG_SearchApiRequest>();

            //todo: we need to change to use following code, but ODataClient 4 has problems with expand, curent implemented code is a workaround
            //ref: https://powerusers.microsoft.com/t5/Power-Apps-Ideas/Web-API-Implement-expand-on-collections/idi-p/221291

            IEnumerable<SSG_SearchApiRequest> searchApiRequests = await _oDataClient.For<SSG_SearchApiRequest>()
                .Select(x => x.SearchApiRequestId)
                .Filter(x => x.StatusCode == readyForSearchCode)
                .FindEntriesAsync(cancellationToken);

            foreach (SSG_SearchApiRequest request in searchApiRequests)
            {
                SSG_SearchApiRequest searchApiRequest = await _oDataClient.For<SSG_SearchApiRequest>()
                    .Key(request.SearchApiRequestId)
                    .Expand(x => x.Identifiers)
                    .FindEntryAsync(cancellationToken);

                Guid id = searchApiRequest.SearchApiRequestId;
                IEnumerable<SSG_SearchapiRequestDataProvider> dataProviders=await _oDataClient.For<SSG_SearchapiRequestDataProvider>()
                    .Filter(x=>x.SearchApiRequest.SearchApiRequestId==id)
                    .FindEntriesAsync(cancellationToken);

                searchApiRequest.DataProviders = dataProviders.ToArray();

                results.Add( searchApiRequest );
            }
            return results;       
        }

        /// <summary>
        /// Get the corresponding searchRequestId give a searchApiRequestId
        /// </summary>
        /// <param name="searchApiRequestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Guid> GetLinkedSearchRequestIdAsync(Guid searchApiRequestId,
            CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty) throw new ArgumentNullException(nameof(searchApiRequestId));

            var result = await _oDataClient
                .For<SSG_SearchApiRequest>()
                .Key(searchApiRequestId)
                .Select(x => x.SearchRequestId)
                .FindEntryAsync(cancellationToken);

            return result.SearchRequestId;
        }

        /// <summary>
        /// Marks a search request in Progress
        /// </summary>
        /// <param name="searchApiRequestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SSG_SearchApiRequest> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty) throw new ArgumentNullException(nameof(searchApiRequestId));

            return await _oDataClient
                .For<SSG_SearchApiRequest>()
                .Key(searchApiRequestId)
                .Set(new Entry { { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchApiRequestStatusReason.InProgress.Value } })
                .UpdateEntryAsync(cancellationToken);
        }


        public async Task<SSG_SearchApiEvent> AddEventAsync(Guid searchApiRequestId, SSG_SearchApiEvent searchApiEvent,
            CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty) throw new ArgumentNullException(nameof(searchApiRequestId));

            searchApiEvent.SearchApiRequest = new SSG_SearchApiRequest() { SearchApiRequestId = searchApiRequestId};

            return await this._oDataClient.For<SSG_SearchApiEvent>().Set(searchApiEvent).InsertEntryAsync(cancellationToken);
        }

    }
}