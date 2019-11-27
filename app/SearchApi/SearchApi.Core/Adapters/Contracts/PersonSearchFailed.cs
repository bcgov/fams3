using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchFailed : PersonSearchEvent, AdapterEvent
    {
        Exception Cause { get; }
    }
}