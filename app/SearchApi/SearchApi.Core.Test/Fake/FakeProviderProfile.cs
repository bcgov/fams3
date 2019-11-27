using SearchApi.Core.Adapters.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Test.Fake
{
    public class FakeProviderProfile : ProviderProfile
    {
        public string Name { get; } = "ICBC";
    }
}
