using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class InvolvedParty : PersonalInfo
    {
        [Description("Name of person involved.  If this is returned, organization is not returned")]
        public Name Name { get; set; }
        [Description("Type code for the party involved")]
        public string Type { get; set; }
        [Description("Type description for the party involved")]
        public string TypeDescription { get; set; }
        [Description("Organization involved. If this is returned, name is not returned")]
        public  string Organization { get; set; }

       
    }
}
