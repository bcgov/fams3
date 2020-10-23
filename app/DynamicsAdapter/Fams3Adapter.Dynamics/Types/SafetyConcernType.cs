
using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class SafetyConcernType : Enumeration
    {
        public static SafetyConcernType POR = new SafetyConcernType(867670000, "POR");
        public static SafetyConcernType Violence = new SafetyConcernType(867670001, "Violence");
        public static SafetyConcernType Threat = new SafetyConcernType(867670002, "Threat");
        public static SafetyConcernType Suicidal = new SafetyConcernType(867670003, "Suicidal");
        public static SafetyConcernType Caution = new SafetyConcernType(867670004, "Caution");
        public static SafetyConcernType Other = new SafetyConcernType(867670005, "Other");

        protected SafetyConcernType(int value, string name) : base(value, name)
        {

        }
    }
}
