using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class SelfEmploymentCompanyType : Enumeration
    {

        public static SelfEmploymentCompanyType SoleProp = new SelfEmploymentCompanyType(867670000, "Sole Proprietorship");
        public static SelfEmploymentCompanyType GeneralPartner = new SelfEmploymentCompanyType(867670001, "General Partnership");
        public static SelfEmploymentCompanyType Incorp = new SelfEmploymentCompanyType(867670002, "Incorporation");
        public static SelfEmploymentCompanyType ExtraProvincialRegistered = new SelfEmploymentCompanyType(867670003, "Extra Provincially Registered Company");
        public static SelfEmploymentCompanyType CoOp = new SelfEmploymentCompanyType(867670004, "Co-Op");
        public static SelfEmploymentCompanyType IncorporatedSociety = new SelfEmploymentCompanyType(867670005, "Incorporated Society");
        public static SelfEmploymentCompanyType ExtraprovincialNonShareCorp = new SelfEmploymentCompanyType(867670006, "Extraprovincial Non-Share Corporation");
        public static SelfEmploymentCompanyType Other = new SelfEmploymentCompanyType(867670007, "Other");

        protected SelfEmploymentCompanyType(int value, string name) : base(value, name)
        {
        }
    }
}
