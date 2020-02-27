using BcGov.Fams3.SearchApi.Contracts.Person;
using System;
using System.Threading.Tasks;

namespace BcGov.Fams3.Redis
{
    public interface ICacheService
    {
        Task SaveRequest(Guid searchRequestId, Person searchRequest);
        Task<Person> GetRequest(Guid searchRequestId);
    }
}
