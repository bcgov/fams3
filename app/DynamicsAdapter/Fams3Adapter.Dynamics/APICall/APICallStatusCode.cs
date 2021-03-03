using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.APICall
{
    public class APICallStatusCode : Enumeration
    {
        public static APICallStatusCode Completed =
            new APICallStatusCode(2, "Completed");
        public static APICallStatusCode Failed =
            new APICallStatusCode(867670001, "Failed");

        protected APICallStatusCode(int value, string name) : base(value, name)
        {
        }

    }
}
