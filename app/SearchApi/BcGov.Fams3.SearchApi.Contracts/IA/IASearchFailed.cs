using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Contracts.IA
{
    public  interface IASearchFailed : PersonSearchEvent, IASearchEvent
    {
       
        bool Retry { get; }

        string Cause { get; }
    }
}
