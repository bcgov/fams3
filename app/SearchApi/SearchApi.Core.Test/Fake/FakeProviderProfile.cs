using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchApi.Core.Test.Fake
{
    public class FakeProviderProfile : ProviderProfile
    {
        public string Name { get; set; } = "SampleProvider";
    }
}
