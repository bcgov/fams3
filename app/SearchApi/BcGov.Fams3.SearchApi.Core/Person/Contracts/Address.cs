namespace BcGov.Fams3.SearchApi.Core.Person.Contracts
{
    public interface Address
    {
        string Type { get; }
        string AddressLine1 { get; }
        string AddressLine2 { get;  }
        string Province { get;  }
        string City { get;  }
        string Country { get;  }
        string PostalCode { get;  }
        string NonCanadianState { get;  }
        string SuppliedBy { get;  }
    }
}
