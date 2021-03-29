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
    }
}
