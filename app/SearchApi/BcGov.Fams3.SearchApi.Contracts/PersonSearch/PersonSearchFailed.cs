
namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchFailed : PersonSearchAdapterEvent
    {
        string Cause { get; }
        BcGov.Fams3.SearchApi.Contracts.Person.PersonalIdentifier personalIdentifier { get; }
    }
}
