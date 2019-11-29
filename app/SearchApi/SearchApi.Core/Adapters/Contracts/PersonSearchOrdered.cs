using SearchApi.Core.Person.Contracts;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchOrdered : PersonSearchEvent
    {
        Person.Contracts.Person Person { get; }
    }
}