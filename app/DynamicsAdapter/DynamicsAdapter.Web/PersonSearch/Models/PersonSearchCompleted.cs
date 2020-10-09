using System.Collections.Generic;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchCompleted : PersonSearchStatus
    {
        public IEnumerable<PersonFound> MatchedPersons { get; set; }

        public string Message { get; set; }

    }
}
