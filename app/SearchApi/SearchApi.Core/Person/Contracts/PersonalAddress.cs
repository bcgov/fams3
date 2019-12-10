using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Person.Contracts
{
    public interface PersonalAddress
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
