using System;
using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface Person
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime? DateOfBirth { get; }
   
        string SecondName { get; }
        string ThirdName { get; }
        string HairColour { get;  }
        string EyeColour { get;  }
        decimal Height { get; }
        decimal Weight { get; }
    }
}