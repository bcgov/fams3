﻿using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchApi.Core.Test.Fake
{
    public class FakeProviderProfile : ProviderProfile
    {
        public string Name { get; } = "SampleProvider";
        public DataProviderEnums DataProviderID { get; } = DataProviderEnums.ICBC;
    }
}
