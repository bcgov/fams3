namespace BcGov.Fams3.SearchApi.Core.Adapters.Contracts
{
   public  interface PersonSearchCompleted  : PersonSearchAdapterEvent
    {
        Person.Contracts.Person MatchedPerson { get; }
    }
}
