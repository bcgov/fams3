namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchAdapterEvent: PersonSearchEvent, AdapterEvent
    {
        SearchStatus Status { get; }
    }
}