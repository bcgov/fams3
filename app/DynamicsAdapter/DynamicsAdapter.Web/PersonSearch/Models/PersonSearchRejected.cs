using System.Collections.Generic;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchRejected : PersonSearchStatus
    {
        public IEnumerable<ValidationResult> Reasons { get; set; }
    }

    public class ValidationResult
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
