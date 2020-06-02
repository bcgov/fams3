using System.Collections.Generic;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchCompleted : PersonSearchStatus
    {
        public IEnumerable<PersonFoundResult> MatchedPersons { get; set; }

    }
}
