namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
   public  interface PersonSearchCompleted  : PersonSearchAdapterEvent
    {
        Person.Person MatchedPerson { get; }
    }
}
