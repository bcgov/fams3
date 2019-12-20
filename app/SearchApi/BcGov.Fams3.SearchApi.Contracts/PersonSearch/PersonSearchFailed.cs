namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchFailed : PersonSearchAdapterEvent
    {
        string Cause { get; }
    }
}