using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchCompleted : PersonSearchStatus
    {
        public Person MatchedPerson { get; set; }
    }
}
