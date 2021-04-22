using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class User
    {
        [Description("User Email")]
        public string Email { get; set; }
        [Description("User FirstName ")]
        public string FirstName { get; set; }
        [Description("User lastname")]
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool SystemTriggered { get; set; }
    }
}
