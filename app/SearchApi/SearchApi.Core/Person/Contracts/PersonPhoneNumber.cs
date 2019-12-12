using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Person.Contracts
{
    public interface PersonPhoneNumber
    {
        string SuppliedBy { get; }

        string EffectiveDate { get; }

        string EffectiveDateType { get; }

        string PhoneNumber { get; }

        string PhoneNumberType { get; }
    }
}
