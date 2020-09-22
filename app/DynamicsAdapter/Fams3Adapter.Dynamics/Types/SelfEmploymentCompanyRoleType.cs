using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class SelfEmploymentCompanyRoleType : Enumeration
    {

        public static SelfEmploymentCompanyRoleType Director = new SelfEmploymentCompanyRoleType(867670000, "Director");
        public static SelfEmploymentCompanyRoleType Officer = new SelfEmploymentCompanyRoleType(867670001, "Officer");
        public static SelfEmploymentCompanyRoleType Treasurer = new SelfEmploymentCompanyRoleType(867670002, "Treasurer");
        public static SelfEmploymentCompanyRoleType Shareholder = new SelfEmploymentCompanyRoleType(867670003, "Shareholder");
        public static SelfEmploymentCompanyRoleType Owner = new SelfEmploymentCompanyRoleType(867670004, "Owner");

        protected SelfEmploymentCompanyRoleType(int value, string name) : base(value, name)
        {
        }
    }
}
