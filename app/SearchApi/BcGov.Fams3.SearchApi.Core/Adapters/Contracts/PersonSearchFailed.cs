namespace BcGov.Fams3.SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchFailed : PersonSearchAdapterEvent
    {
        string Cause { get; }
    }
}