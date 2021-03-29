using Newtonsoft.Json;
using System;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonFound : Person
    {
        public PersonalIdentifier SourcePersonalIdentifier { get; set; }
    }

    public static class PersonFoundExtensions
    {
        public static bool SamePersonFound(this PersonFound thisPerson, PersonFound comparePerson)
        {
            return
                 string.Equals(thisPerson.FirstName, comparePerson.FirstName, StringComparison.InvariantCultureIgnoreCase)
              && string.Equals(thisPerson.LastName, comparePerson.LastName, StringComparison.InvariantCultureIgnoreCase)
              && string.Equals(thisPerson.MiddleName, comparePerson.MiddleName, StringComparison.InvariantCultureIgnoreCase)
              && string.Equals(thisPerson.OtherName, comparePerson.OtherName, StringComparison.InvariantCultureIgnoreCase)
              && thisPerson.DateOfBirth == comparePerson.DateOfBirth
              && thisPerson.DateOfDeath == comparePerson.DateOfDeath
              && thisPerson.AgeInYears == comparePerson.AgeInYears
              && thisPerson.Gender == comparePerson.Gender
              && thisPerson.Incacerated == comparePerson.Incacerated
              && thisPerson.Height == comparePerson.Height
              && thisPerson.HeightUnits == comparePerson.HeightUnits
              && thisPerson.Weight == comparePerson.Weight
              && thisPerson.WeightUnits == comparePerson.WeightUnits
              && thisPerson.HairColour == comparePerson.HairColour
              && thisPerson.EyeColour == comparePerson.EyeColour
              && thisPerson.Complexion == comparePerson.Complexion
              && thisPerson.DistinguishingFeatures == comparePerson.DistinguishingFeatures
              && thisPerson.WearGlasses == comparePerson.WearGlasses
              && thisPerson.SecurityKeyword == comparePerson.SecurityKeyword
              && thisPerson.CautionFlag == comparePerson.CautionFlag
              && thisPerson.CautionReason == comparePerson.CautionReason
              && thisPerson.CautionNotes == comparePerson.CautionNotes;
        }

        public static void RemoveDuplicateProperties(this PersonFound thisPerson, PersonFound comparePerson)
        {
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Addresses), JsonConvert.SerializeObject(comparePerson.Addresses), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Addresses = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Emails), JsonConvert.SerializeObject(comparePerson.Emails), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Emails = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Employments), JsonConvert.SerializeObject(comparePerson.Employments), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Employments = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Identifiers), JsonConvert.SerializeObject(comparePerson.Identifiers), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Identifiers = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.InsuranceClaims), JsonConvert.SerializeObject(comparePerson.InsuranceClaims), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.InsuranceClaims = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Investments), JsonConvert.SerializeObject(comparePerson.Investments), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Investments = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.OtherAssets), JsonConvert.SerializeObject(comparePerson.OtherAssets), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.OtherAssets = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Pensions), JsonConvert.SerializeObject(comparePerson.Pensions), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Pensions = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Phones), JsonConvert.SerializeObject(comparePerson.Phones), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Phones = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.RealEstateProperties), JsonConvert.SerializeObject(comparePerson.RealEstateProperties), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.RealEstateProperties = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.RelatedPersons), JsonConvert.SerializeObject(comparePerson.RelatedPersons), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.RelatedPersons = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.SafetyConcerns), JsonConvert.SerializeObject(comparePerson.SafetyConcerns), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.SafetyConcerns = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.SocialMedias), JsonConvert.SerializeObject(comparePerson.SocialMedias), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.SocialMedias = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Vehicles), JsonConvert.SerializeObject(comparePerson.Vehicles), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Vehicles = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.Names), JsonConvert.SerializeObject(comparePerson.Names), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.Names = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.BankInfos), JsonConvert.SerializeObject(comparePerson.BankInfos), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.BankInfos = null;
            if (string.Equals(JsonConvert.SerializeObject(thisPerson.CompensationClaims), JsonConvert.SerializeObject(comparePerson.CompensationClaims), StringComparison.InvariantCultureIgnoreCase))
                thisPerson.CompensationClaims = null;
        }
    }
}
