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

            try
            {

                //IEnumerable<SSG_SearchApiRequest> results = await _oDataClient.For<SSG_SearchApiRequest>().Filter(x => x.StatusCode == readyForSeachCode).Expand(x => x.Identifiers)
                //    .FindEntriesAsync(cancellationToken);

                //todo: we need to change to use above code, but ODataClient 4 has problems with expand, following code is a workaround
                //ref: https://powerusers.microsoft.com/t5/Power-Apps-Ideas/Web-API-Implement-expand-on-collections/idi-p/221291

                IEnumerable<SSG_SearchApiRequest> searchApiRequests = await _oDataClient.For<SSG_SearchApiRequest>()
                    .Select(x => x.SearchApiRequestId)
                    .Filter(x => x.StatusCode == readyForSearchCode)
                    .FindEntriesAsync(cancellationToken);

                List<SSG_SearchApiRequest> results = new List<SSG_SearchApiRequest>();
                foreach (SSG_SearchApiRequest request in searchApiRequests)
                {
                    SSG_SearchApiRequest r = await _oDataClient.For<SSG_SearchApiRequest>()
                        .Key(request.SearchApiRequestId)
                        .Expand(x => x.Identifiers).FindEntryAsync(cancellationToken);
                    results.Add(r);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw;
            }         
        }

        /// <summary>(
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

    }
}