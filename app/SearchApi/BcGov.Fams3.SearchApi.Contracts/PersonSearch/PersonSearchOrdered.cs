namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchOrdered : PersonSearchEvent
    {
        Person.Person Person { get; }

    }
}