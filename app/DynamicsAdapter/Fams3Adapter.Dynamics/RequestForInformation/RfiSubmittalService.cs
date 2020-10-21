using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.DataProvider;
using Microsoft.Extensions.Logging;
using Simple.OData.Client;
using Entry = System.Collections.Generic.Dictionary<string, object>;

namespace Fams3Adapter.Dynamics.RfiService
{
	public interface IRfiSubmittalService
    {
        Task<IEnumerable<SSG_RfiMessage>> GetAllReadyForSendAsync(CancellationToken cancellationToken, SSG_DataProvider[] dataProviders);
        Task<IEnumerable<SSG_RfiMessage>> GetAllValidFailedSendRequest(CancellationToken cancellationToken, SSG_DataProvider[] dataProviders);
        Task<IEnumerable<SSG_DataProvider>> GetDataProvidersList(CancellationToken cancellationToken);
        Task<SSG_RfiMessage> MarkInProgress(Guid searchApiRequestId, CancellationToken cancellationToken);
        Task<SSG_RfiMessage> MarkComplete(Guid searchApiRequestId, CancellationToken cancellationToken);
	}

	public class RfiSubmittalService : IRfiSubmittalService
	{
        private readonly IODataClient _oDataClient;
        private readonly ILogger<RfiSubmittalService> _logger;
        
        public RfiSubmittalService(IODataClient oDataClient,ILogger<RfiSubmittalService> logger)
        {
            this._oDataClient = oDataClient;
            _logger = logger;
        }

		public async Task<IEnumerable<SSG_RfiMessage>> GetAllReadyForSendAsync(CancellationToken cancellationToken, SSG_DataProvider[] dataProviders)
		{           
            List<SSG_DataProvider> providers = dataProviders.ToList();
            int readyForSearchCode = RfiStatusCodes.New.Value;
            int faxChannel = RfiChannels.Fax.Value;
            List<SSG_RfiMessage> results = new List<SSG_RfiMessage>();

            IEnumerable<SSG_RfiMessage> rfiMessages = await _oDataClient.For<SSG_RfiMessage>()
                .Filter(r => r.StatusCode == readyForSearchCode)
                .Filter(r => r.Channel == faxChannel)
                .FindEntriesAsync(cancellationToken);
            _logger.LogInformation($"Found {rfiMessages.Count()} messages ready to send.");

            foreach (SSG_RfiMessage msg in rfiMessages)
            {
                if(msg.NoteId.Equals(Guid.Empty))
                    continue;
                var doc = await _oDataClient.For<Annotation>()
                .Key(msg.NoteId)
                .FindEntryAsync(cancellationToken);

                msg.DocumentBody = doc.DocumentBody;

                results.Add(msg);
            }
            return results;     
		}

		public Task<IEnumerable<SSG_RfiMessage>> GetAllValidFailedSendRequest(CancellationToken cancellationToken, SSG_DataProvider[] dataProviders)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<SSG_DataProvider>> GetDataProvidersList(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<SSG_RfiMessage> MarkComplete(Guid searchApiRequestId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<SSG_RfiMessage> MarkInProgress(Guid msgId, CancellationToken cancellationToken)
		{
            if (msgId == default || msgId == Guid.Empty) throw new ArgumentNullException(nameof(msgId));

            return await _oDataClient
                .For<SSG_RfiMessage>()
                .Key(msgId)
                .Set(new Entry { { "statuscode", RfiStatusCodes.InProgress.Value } })
                .UpdateEntryAsync(cancellationToken);
		}
	}
}