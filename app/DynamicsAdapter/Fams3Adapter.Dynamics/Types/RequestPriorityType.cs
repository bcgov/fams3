using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class RequestPriorityType : Enumeration
    {
        public static RequestPriorityType Regular = new RequestPriorityType(867670000, "Regular");
        public static RequestPriorityType Rush = new RequestPriorityType(867670001, "Rush");
        public static RequestPriorityType Urgent = new RequestPriorityType(867670002, "Urgent");

        protected RequestPriorityType(int value, string name) : base(value, name)
        {

        }
    }
}
