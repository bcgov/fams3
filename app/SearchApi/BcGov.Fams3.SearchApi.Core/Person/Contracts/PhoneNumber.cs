using System;

namespace BcGov.Fams3.SearchApi.Core.Person.Contracts
{
    public interface PhoneNumber
    {
        string SuppliedBy { get; }

        DateTime? Date { get; }

        string DateType { get; }

        string PhoneNumber { get; }

        string PhoneNumberType { get; }
    }
}
