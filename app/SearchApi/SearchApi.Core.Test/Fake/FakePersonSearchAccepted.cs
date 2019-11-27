using SearchApi.Core.Adapters.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchAccepted : PersonSearchAccepted
    {
        public Guid SearchRequestId { get; } = Guid.NewGuid();

        public DateTime TimeStamp { get; } = DateTime.Now;

        public ProviderProfile ProviderProfile { get; } = new FakeProviderProfile();
    }
}
