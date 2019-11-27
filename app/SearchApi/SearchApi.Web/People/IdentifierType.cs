using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.People
{
    public enum IdentifierType
    {
        DriverLicense,
        SocialInsuranceNumber,
        PersonalHealthNumber,
        BirthCertificate,
        CorrectionsId,
        NativeStatusCard,
        Passport,
        WcbClaim,
        Other,
        SecurityKeyword
    }
}
