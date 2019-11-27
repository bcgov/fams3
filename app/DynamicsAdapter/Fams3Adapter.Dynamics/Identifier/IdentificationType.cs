using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class IdentificationType : Enumeration
    {

        public static IdentificationType DriverLicense = new IdentificationType(867670000, 0, "Driver's License");
        public static IdentificationType SocialInsuranceNumber = new IdentificationType(867670006, 1, "Social Insurance Number");
        public static IdentificationType PersonalHealthNumber = new IdentificationType(867670001, 2,  "Personal Health Number");
        public static IdentificationType BirthCertificate = new IdentificationType(867670008, 3, "Birth Certificate");
        public static IdentificationType CorrectionsId = new IdentificationType(867670009, 4, "Corrections ID");
        public static IdentificationType NativeStatusCard = new IdentificationType(867670011, 5, "Native Status Card");
        public static IdentificationType Passport = new IdentificationType(867670002, 6, "Passport");
        public static IdentificationType WcbClaim = new IdentificationType(867670015, 7, "WCB Claim");
        public static IdentificationType Other = new IdentificationType(867670012, 8, "Other");
        public static IdentificationType SecurityKeyword = new IdentificationType(867670013, 9, "Security Keyword");

        public int DynamicValue { get; private set; }
        public int SearchApiValue { get; private set; }

        protected IdentificationType(int dynamicValue, int searchApiValue, string name) : base(dynamicValue, name)
        {
            this.DynamicValue = dynamicValue;
            this.SearchApiValue = searchApiValue;
        }


    }
}