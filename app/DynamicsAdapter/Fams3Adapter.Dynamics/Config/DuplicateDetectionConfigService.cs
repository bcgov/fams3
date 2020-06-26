using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Config
{
    public interface IDuplicateDetectionConfigService
    {
        Task<IEnumerable<SSG_DuplicateDetectionConfig>> GetDuplicateDetectionConfig(CancellationToken cancellationToken);
    }

    public class DuplicateDetectionConfigService : IDuplicateDetectionConfigService
    {
        private readonly IODataClient _oDataClient;
        public static IEnumerable<SSG_DuplicateDetectionConfig> configs;

        public DuplicateDetectionConfigService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }

        /// <summary>
        /// Gets all the search request in `Ready for Search` status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SSG_DuplicateDetectionConfig>> GetDuplicateDetectionConfig(CancellationToken cancellationToken)
        {
            if (configs != null) return configs;
            IEnumerable<SSG_DuplicateDetectionConfig> duplicateConfigs = await _oDataClient.For<SSG_DuplicateDetectionConfig>()
                .FindEntriesAsync(cancellationToken);
            configs = duplicateConfigs;
            return duplicateConfigs;
        }
    }
}
