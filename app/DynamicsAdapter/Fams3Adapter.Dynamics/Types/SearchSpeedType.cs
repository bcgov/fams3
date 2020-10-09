using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class AutoSearchSpeedType : Enumeration
    {
     

        public static AutoSearchSpeedType Fast = new AutoSearchSpeedType(867670000, "Fast");
        public static AutoSearchSpeedType Slow = new AutoSearchSpeedType(867670001, "Slow");
     

        protected AutoSearchSpeedType(int value, string name) : base(value, name)
        {

        }
    }
}
