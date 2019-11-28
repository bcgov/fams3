using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchFailed : PersonSearchAdapterEvent
    {
        Exception Cause { get; }
    }
}