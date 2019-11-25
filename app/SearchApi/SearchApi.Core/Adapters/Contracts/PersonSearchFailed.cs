using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchFailed : PersonSearchEvent
    {
        Exception Cause { get; }
    }
}