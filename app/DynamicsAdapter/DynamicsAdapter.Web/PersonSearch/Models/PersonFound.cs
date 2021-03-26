using Newtonsoft.Json;
using System;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonFound : Person
    {
        public PersonalIdentifier SourcePersonalIdentifier { get; set; }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                PersonFound p = (PersonFound)obj;
                return
                     string.Equals(FirstName, p.FirstName, StringComparison.InvariantCultureIgnoreCase)
                  && string.Equals(LastName, p.LastName, StringComparison.InvariantCultureIgnoreCase)
                  && string.Equals(MiddleName, p.MiddleName, StringComparison.InvariantCultureIgnoreCase)
                  && string.Equals(OtherName, p.OtherName, StringComparison.InvariantCultureIgnoreCase)
                  && DateOfBirth == p.DateOfBirth
                  && DateOfDeath == p.DateOfDeath
                  && AgeInYears == p.AgeInYears
                  && Gender == p.Gender
                  && Incacerated == p.Incacerated
                  && Height == p.Height
                  && HeightUnits == p.HeightUnits
                  && Weight == p.Weight
                  && WeightUnits == p.WeightUnits
                  && HairColour == p.HairColour
                  && EyeColour == p.EyeColour
                  && Complexion == p.Complexion
                  && DistinguishingFeatures == p.DistinguishingFeatures
                  && WearGlasses == p.WearGlasses
                  && SecurityKeyword == p.SecurityKeyword
                  && CautionFlag == p.CautionFlag
                  && CautionReason == p.CautionReason
                  && CautionNotes == p.CautionNotes;
            }
        }
    }
}
