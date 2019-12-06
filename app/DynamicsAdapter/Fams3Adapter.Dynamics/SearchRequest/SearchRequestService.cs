﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Simple.OData.Client;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    using Entry = System.Collections.Generic.Dictionary<string, object>;
    public interface ISearchRequestService
    {
        Task<SSG_Identifier> UploadIdentifier(SSG_Identifier identifier, CancellationToken cancellationToken);
        Task<SSG_Address> UploadAddress(SSG_Address address, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchRequestService : ISearchRequestService
    {
        private readonly IODataClient _oDataClient;

        public SearchRequestService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }

        /// <summary>
        /// Gets all the search request in `Ready for Search` status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SSG_Identifier> UploadIdentifier(SSG_Identifier identifier, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Identifier>().Set(identifier).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Address> UploadAddress(SSG_Address address, CancellationToken cancellationToken)
        {
            string countryName = address.Country.Name;
            var country = await _oDataClient.For<SSG_Country>()
                                         .Filter(x => x.Name == countryName)
                                         .FindEntryAsync();

            address.Country = country;
            return await this._oDataClient.For<SSG_Address>().Set(address).InsertEntryAsync(cancellationToken);
        }
    }
}