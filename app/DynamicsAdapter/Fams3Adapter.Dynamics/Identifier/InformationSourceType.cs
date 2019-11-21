using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class InformationSourceType : Enumeration
    {

        public static InformationSourceType DriverLicense = new InformationSourceType(867670000, "Request");
        public static InformationSourceType SocialInsuranceNumber = new InformationSourceType(867670001, "ICBC");
        public static InformationSourceType PersonalHealthNumber = new InformationSourceType(867670002, "Employer");


        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
}
