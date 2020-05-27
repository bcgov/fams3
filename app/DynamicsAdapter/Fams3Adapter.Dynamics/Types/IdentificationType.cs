using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    
    public class IdentificationType : Enumeration
    { 
        public readonly IdentificationType BCDriverLicense = new IdentificationType(867670000, "BC Driver's License");
        public readonly IdentificationType SocialInsuranceNumber = new IdentificationType(867670006, "Social Insurance Number");
        public readonly IdentificationType PersonalHealthNumber = new IdentificationType(867670001, "Personal Health Number");
        public readonly IdentificationType BirthCertificate = new IdentificationType(867670008,  "Birth Certificate");
        public readonly IdentificationType CorrectionsId = new IdentificationType(867670009, "Corrections ID");
        public readonly IdentificationType NativeStatusCard = new IdentificationType(867670011,  "Native Status Card");
        public readonly IdentificationType Passport = new IdentificationType(867670002,  "Passport");
        public readonly IdentificationType WorkSafeBCCCN = new IdentificationType(867670015, "WorkSafeBC CCN");
        public readonly IdentificationType BCID = new IdentificationType(867670003, "BCID");
        public readonly IdentificationType BCHydroBP = new IdentificationType(867670004, "BC Hydro BP");      
        public readonly IdentificationType Other = new IdentificationType(867670012,  "Other");
        public readonly IdentificationType OutOfBCDriverLicense = new IdentificationType(867670005, "OOP Driver's License");


        protected IdentificationType(int value, string name) : base(value, name)
        {
        }


    }

}