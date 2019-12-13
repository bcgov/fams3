using System;
using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Core.Person.Contracts
{
    public interface Person
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime? DateOfBirth { get; }
        IEnumerable<PersonalIdentifier> Identifiers { get; }
        IEnumerable<PersonalAddress> Addresses { get; }

        IEnumerable<PersonalPhoneNumber> PhoneNumbers { get; }
    }
}