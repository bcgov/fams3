using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class IdentificationType : Enumeration
    {

        public static IdentificationType DriverLicense = new IdentificationType(867670000, 1, "Driver's License");
        public static IdentificationType SocialInsuranceNumber = new IdentificationType(867670006, 2, "Social Insurance Number");
        public static IdentificationType PersonalHealthNumber = new IdentificationType(867670001, 3,  "Personal Health Number");
        public static IdentificationType BirthCertificate = new IdentificationType(867670008, 4, "Birth Certificate");
        public static IdentificationType CorrectionsId = new IdentificationType(867670009, 5, "Corrections ID");
        public static IdentificationType NativeStatusCard = new IdentificationType(867670011, 6, "Native Status Card");
        public static IdentificationType Passport = new IdentificationType(867670002, 7, "Passport");
        public static IdentificationType WcbClaim = new IdentificationType(867670015, 8, "WCB Claim");
        public static IdentificationType Other = new IdentificationType(867670012, 9, "Other");
        public static IdentificationType SecurityKeyword = new IdentificationType(867670013, 10, "Security Keyword");

        public int DynamicValue { get; private set; }
        public int SearchApiValue { get; private set; }

        protected IdentificationType(int dynamicValue, int searchApiValue, string name) : base(dynamicValue, name)
        {
        }


    }
}