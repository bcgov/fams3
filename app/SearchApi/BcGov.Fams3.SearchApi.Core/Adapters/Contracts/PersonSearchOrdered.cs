namespace BcGov.Fams3.SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchOrdered : PersonSearchEvent
    {
        Person.Contracts.Person Person { get; }
    }
}