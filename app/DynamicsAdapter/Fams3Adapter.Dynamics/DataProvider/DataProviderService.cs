using Microsoft.Extensions.Logging;
using Simple.OData.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.DataProvider
{

    public interface IDataProviderService
    {
        Task<IEnumerable<SSG_DataProvider>> GetAllDataProviders(CancellationToken cancellationToken);

        Task<SSG_SearchapiRequestDataProvider> UpdateSearchRequestApiProvider(SSG_SearchapiRequestDataProvider SSG_SearchapiRequestDataProvider, CancellationToken cancellationToken);

    }
    public class DataProviderService : IDataProviderService
    {
        private readonly IODataClient _oDataClient;
        private readonly ILogger<DataProviderService> _logger;
        public DataProviderService(ILogger<DataProviderService> logger, IODataClient oDataClient)
        {
            _oDataClient = oDataClient;
            _logger = logger;

        }
        public async Task<IEnumerable<SSG_DataProvider>> GetAllDataProviders(CancellationToken cancellationToken)
        {
            return await _oDataClient.For<SSG_DataProvider>()
                        .FindEntriesAsync(cancellationToken);
        }

        public async Task<SSG_SearchapiRequestDataProvider> UpdateSearchRequestApiProvider(SSG_SearchapiRequestDataProvider SSG_SearchapiRequestDataProvider, CancellationToken cancellationToken)
        {
            return await _oDataClient.For<SSG_SearchapiRequestDataProvider>().Key(SSG_SearchapiRequestDataProvider.SearchApiRequestDataProvider).Set(SSG_SearchapiRequestDataProvider).UpdateEntryAsync(cancellationToken);

        }
    }
}
