using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class InformationSourceType : Enumeration
    {

        public static InformationSourceType Request = new InformationSourceType(867670000, 1, "Request");
        public static InformationSourceType ICBC = new InformationSourceType(867670001, 2, "ICBC");
        public static InformationSourceType Employer = new InformationSourceType(867670002, 3, "Employer");

        public int DynamicValue { get; private set; }
        public int SearchApiValue { get; private set; }

        protected InformationSourceType(int dynamicValue, int searchApiValue, string name) : base(dynamicValue, name)
        {
            this.DynamicValue = dynamicValue;
            this.SearchApiValue = searchApiValue;
        }
    }
}
