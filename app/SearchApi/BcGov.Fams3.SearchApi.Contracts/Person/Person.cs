using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Person : PersonalInfo
    {
        public Agency Agency { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string OtherName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public int? AgeInYears { get; set; }

        public string Gender { get; set; }
        public bool? DateDeathConfirmed { get; set; }
        public string Incacerated { get; set; }

        public string Height { get; set; }
        public string HeightUnits { get; set; }

        public string Weight { get; set; }
        public string WeightUnits { get; set; }
        public string HairColour { get; set; }
        public string EyeColour { get; set; }
        public string Complexion { get; set; }
        public string DistinguishingFeatures { get; set; }
        public string WearGlasses { get; set; }
        public string SecurityKeyword { get; set; }
        public string CautionFlag { get; set; }
        public string CautionReason { get; set; }
        public string CautionNotes { get; set; }
        public SoughtPersonType Type { get; set; }
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Phone> Phones { get; set; }
        public IEnumerable<Name> Names { get; set; }
        public IEnumerable<RelatedPerson> RelatedPersons { get; set; }
        public IEnumerable<Employment> Employments { get; set; }
        public IEnumerable<BankInfo> BankInfos { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public IEnumerable<OtherAsset> OtherAssets { get; set; }
        public IEnumerable<CompensationClaim> CompensationClaims { get; set; }
        public IEnumerable<InsuranceClaim> InsuranceClaims { get; set; }
        public IEnumerable<ResponseNote> ResponseNotes { get; set; }
        public IEnumerable<Investment> Investments { get; set; }
        public IEnumerable<SafetyConcern> SafetyConcerns { get; set; }
        public IEnumerable<Pension> Pensions { get; set; }
        public IEnumerable<RealEstateProperty> RealEstateProperties { get; set; }
        public IEnumerable<ResponsePerson> ResponsePersons { get; set; }

        public JCAPerson JCAPerson { get; set; }
    }

    public enum SoughtPersonType
    {
        PAYOR,
        RECIPIENT,
    }
}