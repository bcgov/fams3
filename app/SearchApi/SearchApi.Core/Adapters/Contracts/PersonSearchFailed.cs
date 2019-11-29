using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchFailed : PersonSearchAdapterEvent
    {
        string Cause { get; }
    }
}