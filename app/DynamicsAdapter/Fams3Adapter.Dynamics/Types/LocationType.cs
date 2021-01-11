using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{

    public class LocationType : Enumeration
    {
        public static LocationType Business = new LocationType(867670002, "Business");
        public static LocationType Residence = new LocationType(867670001, "Residence");
        public static LocationType Mailing = new LocationType(867670000, "Mailing");
        public static LocationType Other = new LocationType(867670003, "Other");
        public static LocationType CorrectionalCentre = new LocationType(867670004, "Correctional Centre");
        public static LocationType ServiceAddress = new LocationType(867670005, "Service Address");

        protected LocationType(int value, string name) : base(value, name)
        {
        }


    }
}
