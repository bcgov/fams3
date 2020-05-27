using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{

    public class LocationType : Enumeration
    {
        public static LocationType Business = new LocationType(867670002, "Business");
        public static LocationType Residence = new LocationType(867670001, "Residence");
        public static LocationType Mailing = new LocationType(867670000, "Mailing");
        public static LocationType Other = new LocationType(867670003, "Other");

        protected LocationType(int value, string name) : base(value, name)
        {
        }


    }
}
