using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Simple.OData.Client;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    using Entry = System.Collections.Generic.Dictionary<string, object>;
    public interface ISearchRequestService
    {
        Task<SSG_Identifier> UploadIdentifier(Guid searchRequestGuid, SSG_Identifier identifier, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchRequestService : ISearchRequestService
    {
        private readonly IODataClient _oDataClient;
        //private readonly ILogger<SearchRequestService> _logger;

        public SearchRequestService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
            //this._logger = logger;
        }

        /// <summary>
        /// Gets all the search request in `Ready for Search` status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SSG_Identifier> UploadIdentifier(Guid searchRequestGuid, SSG_Identifier identifier, CancellationToken cancellationToken)
        {
            /////////////fake data/////////////
            SSG_Identifier i = new SSG_Identifier();
            i.SSG_Identification = "Test from Fams3Adapter Dynamics";
            i.StateCode = 0;
            i.StatusCode = 1;
            i.ssg_identificationeffectivedate = DateTime.Now;
            searchRequestGuid = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
            ///////////////////////////////////
            Entry e =(System.Collections.Generic.Dictionary<string, object>) i.AsDictionary();
            try
            {
                e.Add("ssg_SearchRequest", new SSG_SearchRequest
                {
                    SearchRequestId = searchRequestGuid
                });

                var identifier_inserted = await this._oDataClient.For("SSG_Identifiers").Set(e).InsertEntryAsync();
                return null;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                return null;
            }
            
        }

    }
}