using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    
    public class IdentificationType : Enumeration
    { 
        public static IdentificationType BCDriverLicense = new IdentificationType(867670000, "Driver's License - BC");
        public static IdentificationType SocialInsuranceNumber = new IdentificationType(867670006, "Social Insurance Number");
        public static IdentificationType PersonalHealthNumber = new IdentificationType(867670001, "Personal Health Number");
        public static IdentificationType BirthCertificate = new IdentificationType(867670008,  "Birth Certificate");
        public static IdentificationType CorrectionsId = new IdentificationType(867670009, "Corrections CS");
        public static IdentificationType NativeStatusCard = new IdentificationType(867670011,  "Native Status Card");
        public static IdentificationType Passport = new IdentificationType(867670002,  "Passport");
        public static IdentificationType WorkSafeBCCCN = new IdentificationType(867670015, "WorkSafeBC CCN");
        public static IdentificationType BCID = new IdentificationType(867670003, "BCID");
        public static IdentificationType BCHydroBP = new IdentificationType(867670004, "BC Hydro BP");      
        public static IdentificationType Other = new IdentificationType(867670012,  "Other");
        public static IdentificationType OutOfBCDriverLicense = new IdentificationType(867670005, "Out of Province DL");


        protected IdentificationType(int value, string name) : base(value, name)
        {
        }


    }

}