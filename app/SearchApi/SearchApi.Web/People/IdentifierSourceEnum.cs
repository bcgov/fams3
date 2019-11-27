using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.People
{
    public enum IdentifierSourceEnum
    {
        None = 0,
        Request = 1,
        ICBC = 2,
        Employer = 3
    }
}
