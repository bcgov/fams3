using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.People
{
    public enum IdentifierTypeEnum
    {
        None = 0,
        DriverLicense = 1,
        SocialInsuranceNumber = 2,
        PersonalHealthNumber = 3,
        BirthCertificate = 4,
        CorrectionsId = 5,
        NativeStatusCard = 6,
        Passport = 7,
        WcbClaim = 8,
        Other = 9,
        SecurityKeyword = 10
    }
}
