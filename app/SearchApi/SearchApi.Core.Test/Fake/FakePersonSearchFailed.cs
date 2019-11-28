using SearchApi.Core.Adapters.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchFailed : PersonSearchFailed
    {
        public Exception Cause { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}
