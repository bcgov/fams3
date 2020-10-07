using BcGov.Fams3.SearchApi.Contracts.Person;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    /// <summary>
    /// Represents a data provider profile
    /// </summary>
    public interface ProviderProfile
    {
        string Name { get; }
        SearchSpeedType SearchSpeedType { get; }
    }
}