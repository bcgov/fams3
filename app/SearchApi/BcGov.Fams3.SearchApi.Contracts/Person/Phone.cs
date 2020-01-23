using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Phone
    {
        [Description("Supply ")]
        public string SuppliedBy { get; set; }
        [Description("A Date")]
        public DateTime? Date { get; set; }
        [Description("The Date type of the supplied Date")]
        public string DateType { get; set; }
        [Description("The Phone number")]
        public string PhoneNumber { get; set; }
        [Description("The phone number type")]
        public string PhoneNumberType { get; set; }
    }
}
