using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class EmailType : Enumeration
    {
        public static EmailType Personal = new EmailType(867670000, "Personal");
        public static EmailType Work = new EmailType(867670001, "Work");

        protected EmailType(int value, string name) : base(value, name)
        {

        }
    }
}
