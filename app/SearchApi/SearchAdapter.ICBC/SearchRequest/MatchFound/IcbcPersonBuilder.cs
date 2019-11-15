using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest.MatchFound
{
    public class IcbcPersonBuilder
    {
        private readonly IcbcPerson _icbcPerson = new IcbcPerson();

        public IcbcPersonBuilder WithFirstName(string firstName)
        {
            _icbcPerson.FirstName = firstName;
            return this;
        }

        public IcbcPersonBuilder WithLastName(string lastName)
        {
            _icbcPerson.LastName = lastName;
            return this;
        }

        public IcbcPersonBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            _icbcPerson.DateOfBirth = dateOfBirth;
            return this;
        }

        public IcbcPerson Build()
        {
            return _icbcPerson;
        }

    }


    public sealed class IcbcPerson : Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
