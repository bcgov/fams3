using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public abstract class PersonalInfo
    {
        [Description("First Name")]
        public string FirstName { get; set; }

        [Description("Last Name")]
        public string LastName { get; set; }

        [Description("Birth Date")]
        public DateTime BirthDate { get; set; }

        [Description("The related dates information of the personal info")]
        public IEnumerable<ReferenceDate> ReferenceDates { get; set; }

        [Description("The description of the information")]
        public string Description { get; set; }

        [Description("The notes of the information")]
        public string Notes { get; set; }

        [Description("The comments for response")]
        public string ResponseComments { get; set; }
    }
}
