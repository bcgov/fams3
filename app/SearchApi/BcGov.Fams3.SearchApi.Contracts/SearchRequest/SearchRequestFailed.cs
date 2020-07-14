

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public interface SearchRequestFailed : SearchRequestEvent
    {
        string Cause { get; }
    }
}
