using System;

namespace SearchApi.Web.Controllers
{
    public interface ExecuteSearch
    {
        Guid Id { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
    }
}