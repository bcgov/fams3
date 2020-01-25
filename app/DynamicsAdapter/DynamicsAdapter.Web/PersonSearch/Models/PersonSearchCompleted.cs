using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;


namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchCompleted : PersonSearchStatus, BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchCompleted
    {
        public BcGov.Fams3.SearchApi.Contracts.Person.Person MatchedPerson { get;  set; }
        BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile AdapterEvent.ProviderProfile => ProviderProfile;
    }
}
