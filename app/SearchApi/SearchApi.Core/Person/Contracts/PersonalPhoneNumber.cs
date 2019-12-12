using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Person.Contracts
{
    public interface PersonalPhoneNumber
    {
        string SuppliedBy { get; }

        DateTime? Date { get; }

        string DateType { get; }

        string PhoneNumber { get; }

        string PhoneNumberType { get; }
    }
}
