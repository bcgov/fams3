using System;

namespace SearchApi.Core.Contracts
{
    public interface ExecuteSearch
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
    }
}