using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Error;
using Simple.OData.Client;
using Microsoft.Extensions.Logging;

using Entry = System.Collections.Generic.Dictionary<string, object>;


namespace Fams3Adapter.Dynamics.SearchApiRequest
{

    public interface ISearchApiRequestService
    {
        Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken, SSG_DataProvider[] dataProviders, string availableDataPartners);

        Task<IEnumerable<SSG_SearchApiRequest>> GetAllValidFailedSearchRequest(CancellationToken cancellationToken, SSG_DataProvider[] dataProviders);

        Task<IEnumerable<SSG_DataProvider>> GetDataProvidersList(CancellationToken cancellationToken);

        Task<IEnumerable<SSG_SearchApiEvent>> GetEventsAsync(Guid searchApiRequestId, CancellationToken cancellationToken);

        Task<Guid> GetLinkedSearchRequestIdAsync(Guid searchApiRequestId,
            CancellationToken cancellationToken);

        Task<SSG_SearchApiRequest> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken);

        Task<SSG_SearchApiEvent> AddEventAsync(Guid searchApiRequestId, SSG_SearchApiEvent searchApiEvent,
            CancellationToken cancellationToken);

        Task<SSG_SearchApiRequest> MarkComplete(Guid searchApiRequestId, CancellationToken cancellationToken);

        Task<SSG_SearchApiRequest> GetSearchApiRequest(string searchRequestKey, CancellationToken cancellationToken);

    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchApiRequestService : ISearchApiRequestService
    {

        private readonly IODataClient _oDataClient;
        private readonly ILogger<SearchApiRequestService> _logger;
      //  private readonly SearchApiConfiguration _searchApiConfiguration;
      
        public SearchApiRequestService(IODataClient oDataClient, ILogger<SearchApiRequestService> logger)
        {
            this._oDataClient = oDataClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets all the search request in `Ready for Search` status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(
            CancellationToken cancellationToken, 
            SSG_DataProvider[] dataProviders,
            string availableDataPartners)
        {
            try
            {
                List<SSG_DataProvider> providers = dataProviders.ToList();
                int readyForSearchCode = SearchApiRequestStatusReason.ReadyForSearch.Value;
                List<SSG_SearchApiRequest> results = new List<SSG_SearchApiRequest>();

                _logger.LogDebug("➡️ Start GetAllReadyForSearchAsync...");

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
                        .Expand(x => x.DataProviders)
                        .Expand(x => x.SearchRequest)
                        .FindEntryAsync(cancellationToken);
                    if (searchApiRequest.SearchRequest != null)
                    {
                        searchApiRequest.SearchRequest = await _oDataClient.For<SSG_SearchRequest>()
                        .Key(searchApiRequest.SearchRequest.SearchRequestId)
                        .Expand(x => x.SearchReason).FindEntryAsync(cancellationToken);
                    }
                    searchApiRequest.IsFailed = false;
                    UpdateProviderInfo(providers, searchApiRequest, availableDataPartners);
                    results.Add(searchApiRequest);
                }
                _logger.LogDebug("🏁 End GetAllReadyForSearchAsync with {Count} results.", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to retrieve SearchApiRequests ready for search. AvailableDataPartners: {Partners}",
                    availableDataPartners);
                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                // rethrow to preserve existing behavior
                throw;
            }
        }

        private static void UpdateProviderInfo(List<SSG_DataProvider> providers, SSG_SearchApiRequest searchApiRequest, string availableDataPartners)
        {
            if (searchApiRequest.DataProviders != null)
            {
                List<string> dataPartner;
                if (!string.IsNullOrEmpty(availableDataPartners))
                    dataPartner = availableDataPartners.Split(new char[] { ':' }).ToList();
                else
                    dataPartner = providers.Select(x => x.AdaptorName).ToList();

                List <SSG_SearchapiRequestDataProvider> apiProviders = new List<SSG_SearchapiRequestDataProvider>();
                foreach (SSG_SearchapiRequestDataProvider prov in searchApiRequest.DataProviders)
                {

                    /// get a list of data partner in prod - appsettings
                    /// if provider not in list, do not add to apiProviders
                    if (dataPartner.Any(dataPartner => dataPartner == prov.AdaptorName))
                    {
                        var provider = providers.FirstOrDefault(x => x.AdaptorName == prov.AdaptorName);
                        if (provider != null)
                        {
                            prov.NumberOfRetries = provider.NumberOfRetries;
                            prov.TimeBetweenRetries = provider.TimeBetweenRetries;
                            prov.SearchSpeed = provider.SearchSpeed;
                        }
                        apiProviders.Add(prov);
                    }
                }
                searchApiRequest.DataProviders = apiProviders.ToArray();
            }
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
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(searchApiRequestId));
            }

            _logger.LogDebug("➡️ Start GetLinkedSearchRequestIdAsync for SearchApiRequestId {Id}", searchApiRequestId);

            try
            {
                SSG_SearchApiRequest result = await _oDataClient
                    .For<SSG_SearchApiRequest>()
                    .Key(searchApiRequestId)
                    .Select(x => x.SearchRequestId)
                    .FindEntryAsync(cancellationToken);

                _logger.LogDebug(
                    "🏁 End GetLinkedSearchRequestIdAsync for SearchApiRequestId {Id} → SearchRequestId {SearchRequestId}",
                    searchApiRequestId,
                    result?.SearchRequestId);

                return result.SearchRequestId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to retrieve linked SearchRequestId for SearchApiRequestId {Id}",
                    searchApiRequestId);

                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw; // Re-throw so calling code can handle it
            }
        }

        /// <summary>
        /// Marks a search request in Progress
        /// </summary>
        /// <param name="searchApiRequestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SSG_SearchApiRequest> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(searchApiRequestId));
            }
            try
            {
                _logger.LogDebug("➡️ Start MarkInProgress for SearchApiRequestId {Id}", searchApiRequestId);

                SSG_SearchApiRequest updatedRequest = await _oDataClient
                    .For<SSG_SearchApiRequest>()
                    .Key(searchApiRequestId)
                    .Set(new Entry
                    {
                        { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchApiRequestStatusReason.InProgress.Value }
                    })
                    .UpdateEntryAsync(cancellationToken);

                _logger.LogDebug(
                    "🏁 End MarkInProgress for SearchApiRequestId {Id} → StatusCode {StatusCode}",
                    searchApiRequestId,
                    updatedRequest?.StatusCode);

                return updatedRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to mark SearchApiRequestId {Id} as InProgress",
                    searchApiRequestId);

                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);

                throw; // rethrow for upstream handling
            }
        }

        public async Task<SSG_SearchApiEvent> AddEventAsync(
            Guid searchApiRequestId,
            SSG_SearchApiEvent searchApiEvent,
            CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(searchApiRequestId));
            }

            try
            {
                _logger.LogDebug(
                    "➡️ Start AddEventAsync for SearchApiRequestId {Id} with EventType {EventType}",
                    searchApiRequestId,
                    searchApiEvent?.EventType);

                searchApiEvent.SearchApiRequest = new SSG_SearchApiRequest
                {
                    SearchApiRequestId = searchApiRequestId
                };

                SSG_SearchApiEvent insertedEvent = await _oDataClient
                    .For<SSG_SearchApiEvent>()
                    .Set(searchApiEvent)
                    .InsertEntryAsync(cancellationToken);

                _logger.LogDebug(
                    "🏁 End AddEventAsync for SearchApiRequestId {Id} → EventId {EventId}",
                    searchApiRequestId,
                    insertedEvent?.Id);

                return insertedEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to add event for SearchApiRequestId {Id} with EventType {EventType}",
                    searchApiRequestId,
                    searchApiEvent?.EventType);

                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw; // rethrow to allow upstream handling
            }
        }

        public async Task<IEnumerable<SSG_SearchApiEvent>> GetEventsAsync(
            Guid searchApiRequestId,
            CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(searchApiRequestId));
            }

            try
            {
                _logger.LogDebug("➡️ Start GetEventsAsync for SearchApiRequestId {Id}", searchApiRequestId);

                IEnumerable<SSG_SearchApiEvent> events = await _oDataClient
                    .For<SSG_SearchApiEvent>()
                    .Filter(m => m.SearchApiRequest.SearchApiRequestId == searchApiRequestId)
                    .FindEntriesAsync(cancellationToken);

                _logger.LogDebug(
                    "🏁 End GetEventsAsync for SearchApiRequestId {Id} → {Count} events retrieved",
                    searchApiRequestId,
                    events?.Count() ?? 0);

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to retrieve events for SearchApiRequestId {Id}",
                    searchApiRequestId);

                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw; // rethrow so upstream code can handle
            }
        }

        public async Task<SSG_SearchApiRequest> MarkComplete(
            Guid searchApiRequestId,
            CancellationToken cancellationToken)
        {
            if (searchApiRequestId == default || searchApiRequestId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(searchApiRequestId));
            }

            try
            {
                _logger.LogDebug("➡️ Start MarkComplete for SearchApiRequestId {Id}", searchApiRequestId);

                SSG_SearchApiRequest updatedRequest = await _oDataClient
                    .For<SSG_SearchApiRequest>()
                    .Key(searchApiRequestId)
                    .Set(new Entry
                    {
                        { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchApiRequestStatusReason.Complete.Value }
                    })
                    .UpdateEntryAsync(cancellationToken);

                _logger.LogDebug(
                    "🏁 End MarkComplete for SearchApiRequestId {Id} → StatusCode {StatusCode}",
                    searchApiRequestId,
                    updatedRequest?.StatusCode);

                return updatedRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Failed to mark SearchApiRequestId {Id} as Complete",
                    searchApiRequestId);

                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw; // rethrow for upstream handling
            }
        }

        /// <summary>
        /// Get all failed search request that has not exceeded the number of days to retry
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SSG_SearchApiRequest>> GetAllValidFailedSearchRequest(
            CancellationToken cancellationToken,
            SSG_DataProvider[] dataProviders)
        {
            List<SSG_SearchApiRequest> results = new List<SSG_SearchApiRequest>();

            //todo: we need to change to use following code, but ODataClient 4 has problems with expand, curent implemented code is a workaround
            //ref: https://powerusers.microsoft.com/t5/Power-Apps-Ideas/Web-API-Implement-expand-on-collections/idi-p/221291

            try
            {
                _logger.LogDebug("➡️ Start GetAllValidFailedSearchRequest for {ProviderCount} providers", dataProviders?.Length ?? 0);

                foreach (SSG_DataProvider provider in dataProviders)
                {
                    string adaptorName = provider.AdaptorName;
                    int noOfDaysToRetry = provider.NumberOfDaysToRetry;
                    int allRetryDone = NullableBooleanType.No.Value;
                    IEnumerable<SSG_SearchapiRequestDataProvider> searchApiRequests = await _oDataClient.For<SSG_SearchapiRequestDataProvider>()
                        .Select(x => x.SearchAPIRequestId)
                        .Filter(x => x.NumberOfFailures > 0)
                        .Filter(x => x.AdaptorName == adaptorName)
                        .Filter(x => x.NumberOfFailures < noOfDaysToRetry)
                        .Filter(x => x.AllRetriesDone == allRetryDone)
                        .FindEntriesAsync(cancellationToken);

                    foreach (SSG_SearchapiRequestDataProvider request in searchApiRequests)
                    {

                        SSG_SearchApiRequest searchApiRequest = await _oDataClient.For<SSG_SearchApiRequest>()
                            .Key(request.SearchAPIRequestId)
                            .Expand(x => x.Identifiers)
                            .Expand(x => x.DataProviders)
                            .Expand(x => x.SearchRequest)
                            .FindEntryAsync(cancellationToken);
                        FilterAffectedDataProvider( provider, searchApiRequest);
                        searchApiRequest.IsFailed = true;
                        results.Add(searchApiRequest);
                    }
                }
                _logger.LogDebug(
                    "🏁 End GetAllValidFailedSearchRequest → {Count} failed requests retrieved",
                    results.Count);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to retrieve valid failed search requests");
                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw;
            }
        }

        public async Task<IEnumerable<SSG_DataProvider>> GetDataProvidersList(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("➡️ Start GetDataProvidersList");

                IEnumerable<SSG_DataProvider> dataProviders = await _oDataClient
                    .For<SSG_DataProvider>()
                    .FindEntriesAsync(cancellationToken);

                _logger.LogDebug(
                    "🏁 End GetDataProvidersList → {Count} data providers retrieved",
                    dataProviders?.Count() ?? 0);

                return dataProviders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to retrieve data providers list");
                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw;
            }
        }

        private static void FilterAffectedDataProvider(SSG_DataProvider provider, SSG_SearchApiRequest searchApiRequest)
        {
        
            List<SSG_SearchapiRequestDataProvider> ssgApiDataProviders = new List<SSG_SearchapiRequestDataProvider>();
            ssgApiDataProviders.AddRange(searchApiRequest.DataProviders);
            var list = ssgApiDataProviders.FindAll(x => x.AdaptorName == provider.AdaptorName);
            list.ForEach(x => x.TimeBetweenRetries = provider.TimeBetweenRetries);
            list.ForEach(x => x.NumberOfRetries = provider.NumberOfRetries);
            searchApiRequest.DataProviders = list.ToArray();
        }

        public async Task<SSG_SearchApiRequest> GetSearchApiRequest(string searchRequestKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(searchRequestKey))
            {
                throw new ArgumentNullException(nameof(searchRequestKey));
            }

            try
            {
                _logger.LogDebug("➡️ Start GetSearchApiRequest for SearchRequestKey: {Key}", searchRequestKey);

                string[] parts = searchRequestKey.Split("_");
                SSG_SearchApiRequest result = null;

                if (parts.Length > 1)
                {
                    string sequence = parts[1];

                    IEnumerable<SSG_SearchApiRequest> searchApiRequests = await _oDataClient
                        .For<SSG_SearchApiRequest>()
                        .Select(x => x.SearchApiRequestId)
                        .Filter(x => x.SequenceNumber == sequence)
                        .FindEntriesAsync(cancellationToken);

                    result = searchApiRequests?.FirstOrDefault();
                }

                _logger.LogDebug(
                    "🏁 End GetSearchApiRequest → Found: {Found}, SearchRequestKey: {Key}",
                    result != null,
                    searchRequestKey);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to retrieve SearchApiRequest for key {Key}", searchRequestKey);
                DynamicsApiErrorLogger.LogDynamicsError(ex, _logger);
                throw;
            }
        }
    }
}