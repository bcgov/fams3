namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchFinalized : PersonSearchEvent
    {
        string Message { get; }
    }
}