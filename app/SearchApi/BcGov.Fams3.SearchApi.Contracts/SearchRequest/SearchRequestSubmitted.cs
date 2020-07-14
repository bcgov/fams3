

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public  interface SearchRequestSubmitted : SearchRequestEvent
    {
        string Message { get; }
    }
}
