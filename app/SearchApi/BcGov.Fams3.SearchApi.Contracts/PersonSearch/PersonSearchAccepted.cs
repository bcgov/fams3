namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchAccepted : PersonSearchAdapterEvent
    {
        string Message { get; }
    }
}