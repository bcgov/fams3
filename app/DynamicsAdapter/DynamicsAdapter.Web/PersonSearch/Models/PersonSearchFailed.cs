using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchFailed : PersonSearchStatus
    {
        public string Cause { get; set; }
    }
}
