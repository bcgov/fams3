using System;

namespace SearchApi.Core.Contracts
{
    public interface ExecuteSearch
    {
        Guid Id { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
    }
}