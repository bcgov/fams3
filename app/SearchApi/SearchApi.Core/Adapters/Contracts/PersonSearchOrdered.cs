using SearchApi.Core.Contracts;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchOrdered : PersonSearchEvent
    {
        ExecuteSearch ExecuteSearch { get; }
    }
}