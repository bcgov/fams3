using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchAccepted : PersonSearchStatus, BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchAccepted
    {
        BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile AdapterEvent.ProviderProfile => ProviderProfile;
    }
}
