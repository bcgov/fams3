using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{

    public class CanadianProvinceType : Enumeration
    {
        public readonly CanadianProvinceType Alberta = new CanadianProvinceType(867670000, "Alberta");
        public readonly CanadianProvinceType BritishColumbia = new CanadianProvinceType(867670001, "British Columbia");
        public readonly CanadianProvinceType Manitoba = new CanadianProvinceType(867670002, "Manitoba");
        public readonly CanadianProvinceType NewBrunswick = new CanadianProvinceType(867670003, "New Brunswick");
        public readonly CanadianProvinceType NewfoundlandLabrador = new CanadianProvinceType(867670004, "Newfoundland and Labrador");
        public readonly CanadianProvinceType NovaScotia = new CanadianProvinceType(867670005, "Nova Scotia");
        public readonly CanadianProvinceType NorthwestTerritories = new CanadianProvinceType(867670006, "Northwest Territories");
        public readonly CanadianProvinceType Nunavut = new CanadianProvinceType(867670007, "Nunavut");
        public readonly CanadianProvinceType Ontario = new CanadianProvinceType(867670008, "Ontario");
        public readonly CanadianProvinceType PrinceEdward  = new CanadianProvinceType(867670009, "Prince Edward Island");
        public readonly CanadianProvinceType Quebec = new CanadianProvinceType(867670010, "Quebec");
        public readonly CanadianProvinceType Saskatchewan = new CanadianProvinceType(867670011, "Saskatchewan");
        public readonly CanadianProvinceType Yukon = new CanadianProvinceType(867670012, "Yukon");
        protected CanadianProvinceType(int value, string name) : base(value, name)
        {
        }


    }
}
