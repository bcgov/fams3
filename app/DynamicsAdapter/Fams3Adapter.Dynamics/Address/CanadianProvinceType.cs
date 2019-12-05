using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Address
{
    public class CanadianProvinceType : Enumeration
    {
        public static CanadianProvinceType Alberta = new CanadianProvinceType(867670000, "Alberta");
        public static CanadianProvinceType BritishColumbia = new CanadianProvinceType(867670001, "British Columbia");
        public static CanadianProvinceType Manitoba = new CanadianProvinceType(867670002, "Manitoba");
        public static CanadianProvinceType NewBrunswick = new CanadianProvinceType(867670003, "New Brunswick");
        public static CanadianProvinceType NewfoundlandLabrador = new CanadianProvinceType(867670004, "Newfoundland and Labrador");
        public static CanadianProvinceType NovaScotia = new CanadianProvinceType(867670005, "Nova Scotia");
        public static CanadianProvinceType NorthwestTerritories = new CanadianProvinceType(867670006, "Northwest Territories");
        public static CanadianProvinceType Nunavut = new CanadianProvinceType(867670007, "Nunavut");
        public static CanadianProvinceType Ontario = new CanadianProvinceType(867670008, "Ontario");
        public static CanadianProvinceType PrinceEdward  = new CanadianProvinceType(867670009, "Prince Edward Island");
        public static CanadianProvinceType Quebec = new CanadianProvinceType(867670010, "Quebec");
        public static CanadianProvinceType Saskatchewan = new CanadianProvinceType(867670011, "Saskatchewan");
        public static CanadianProvinceType Yukon = new CanadianProvinceType(867670012, "Yukon");
        protected CanadianProvinceType(int value, string name) : base(value, name)
        {
        }


    }
}
