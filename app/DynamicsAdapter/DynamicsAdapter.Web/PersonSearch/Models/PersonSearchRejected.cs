using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchRejected : PersonSearchStatus
    {
        public IEnumerable<ValidationResult> Reasons { get; }
    }

    public class ValidationResult
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
