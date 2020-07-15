

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public interface SearchRequestOrdered : SearchRequestEvent
    {
        Person.Person Person { get; }

    }
}
