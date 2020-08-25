using Microsoft.Extensions.Logging;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entry = System.Collections.Generic.Dictionary<string, object>;
namespace Fams3Adapter.Dynamics.DataProvider
{

    public interface IDataPartnerService
    {

        Task<IEnumerable<SSG_DataProvider>> GetAllDataProviders(CancellationToken cancellationToken);

        Task<SSG_SearchapiRequestDataProvider> UpdateSearchRequestApiProvider(SSG_SearchapiRequestDataProvider SSG_SearchapiRequestDataProvider, CancellationToken cancellationToken);

        Task<SSG_SearchapiRequestDataProvider> GetSearchApiRequestDataProvider(Guid SearchApiRequestId, string ProviderProfileName, CancellationToken cancellationToken);
    }
    public class DataPartnerService : IDataPartnerService
    {
        private readonly IODataClient _oDataClient;
        private readonly ILogger<DataPartnerService> _logger;
        public DataPartnerService(ILogger<DataPartnerService> logger, IODataClient oDataClient)
        {
            _oDataClient = oDataClient;
            _logger = logger;

        }
        public async Task<IEnumerable<SSG_DataProvider>> GetAllDataProviders(CancellationToken cancellationToken)
        {
            return await _oDataClient.For<SSG_DataProvider>()
                        .FindEntriesAsync(cancellationToken);
        }

    
        public async Task<SSG_SearchapiRequestDataProvider> GetSearchApiRequestDataProvider(Guid SearchApiRequestId, string ProviderProfileName, CancellationToken cancellationToken)
        {
           return await _oDataClient.For<SSG_SearchapiRequestDataProvider>()
                .Filter(x => x.SearchAPIRequestId == SearchApiRequestId)
                .Filter( x => x.AdaptorName == ProviderProfileName)
                .FindEntryAsync(cancellationToken);
        }

        public async Task<SSG_SearchapiRequestDataProvider> UpdateSearchRequestApiProvider(SSG_SearchapiRequestDataProvider searchapiRequestDataProvider, CancellationToken cancellationToken)
        {
            return await _oDataClient
                .For<SSG_SearchapiRequestDataProvider>()
                .Key(searchapiRequestDataProvider.SearchApiRequestDataProvider)
                .Set(new Entry { { "ssg_numberoffailures", searchapiRequestDataProvider.NumberOfFailures }, { "ssg_allretriesflag", searchapiRequestDataProvider.AllRetriesDone } })
                .UpdateEntryAsync(cancellationToken);
        }
    }
}
