using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface PersonFound : Person
    {
        IEnumerable<PersonalIdentifier> Identifiers { get; }
        IEnumerable<Address> Addresses { get; }
        IEnumerable<PhoneNumber> PhoneNumbers { get; }
        IEnumerable<Name> Names { get; }


    }
}
