using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class JCAPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MotherMaidName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Notes { get; set; }
        public string Gender { get; set; }
    }
}
