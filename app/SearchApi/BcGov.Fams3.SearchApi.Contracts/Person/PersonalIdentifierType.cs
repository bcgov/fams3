namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public enum PersonalIdentifierType
    {
        BCDriverLicense,
        SocialInsuranceNumber,
        PersonalHealthNumber,
        BirthCertificate,
        CorrectionsId,
        NativeStatusCard,
        Passport,
        WorkSafeBCCCN,
        Other,
        BCID,
        BCHydroBP,
        OtherDriverLicense
    }

    public enum OwnerType
    {
        Applicant,
        PersonSought,
        InvolvedPerson,
        NotApplicable

    }
}