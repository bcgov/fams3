using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.OptionSets.Models;
using System.Text;

namespace DynamicsAdapter.Web.Mapping
{

    public static class IDType
    {
        internal static readonly IDictionary<int, PersonalIdentifierType> IDTypeDictionary = new Dictionary<int, PersonalIdentifierType>
        {
            { IdentificationType.BCDriverLicense.Value, PersonalIdentifierType.BCDriverLicense },
            { IdentificationType.SocialInsuranceNumber.Value, PersonalIdentifierType.SocialInsuranceNumber },
            { IdentificationType.PersonalHealthNumber.Value, PersonalIdentifierType.PersonalHealthNumber },
            { IdentificationType.BirthCertificate.Value, PersonalIdentifierType.BirthCertificate },
            { IdentificationType.CorrectionsId.Value, PersonalIdentifierType.CorrectionsId },
            { IdentificationType.NativeStatusCard.Value, PersonalIdentifierType.NativeStatusCard },
            { IdentificationType.Passport.Value, PersonalIdentifierType.Passport },
            { IdentificationType.WorkSafeBCCCN.Value, PersonalIdentifierType.WorkSafeBCCCN },
            { IdentificationType.BCHydroBP.Value, PersonalIdentifierType.BCHydroBP },
            { IdentificationType.BCID.Value, PersonalIdentifierType.BCID },
            { IdentificationType.Other.Value, PersonalIdentifierType.Other },
            { IdentificationType.OutOfBCDriverLicense.Value, PersonalIdentifierType.OtherDriverLicense }
        };
    }

    public class IdentifierTypeConverter : IValueConverter<int?, PersonalIdentifierType>
    {
        public PersonalIdentifierType Convert(int? sourceMember, ResolutionContext context)
        {
            return sourceMember == null ? PersonalIdentifierType.Other : IDType.IDTypeDictionary[(int)sourceMember];
        }
    }

    public class PersonalIdentifierTypeConverter : IValueConverter<PersonalIdentifierType, int?>
    {
        public int? Convert(PersonalIdentifierType sourceMember, ResolutionContext context)
        {
            return IDType.IDTypeDictionary.FirstOrDefault(m => m.Value == sourceMember).Key;
        }
    }

    public class AddressTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            return sourceMember == null ? null : (int?)(AddressType.AddressTypeDictionary[sourceMember.ToLower()].Value);
        }
    }

    public static class AddressType
    {
        internal static readonly IDictionary<string, LocationType> AddressTypeDictionary = new Dictionary<string, LocationType>
        {
            { "mailing", LocationType.Mailing },
            { "residence", LocationType.Residence },
            { "business", LocationType.Business },
            { "other", LocationType.Other },
            { "blank", LocationType.Other },
            { "home", LocationType.Residence },
            { "unknown", LocationType.Other }
        };
    }

    public class NameCategoryConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            return
                sourceMember?.ToLower() switch
                {
                    "legal" => PersonNameCategory.LegalName.Value,
                    "alias" => PersonNameCategory.Alias.Value,
                    "blank" => (int?)null,
                    _ => PersonNameCategory.Other.Value
                };
        }
    }

    public class PhoneTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            return
                sourceMember?.ToLower() switch
                {
                    "cell" => TelephoneNumberType.Cell.Value,
                    "home" => TelephoneNumberType.Home.Value,
                    "work" => TelephoneNumberType.Work.Value,
                    "homecell" => TelephoneNumberType.Home.Value,
                    "workcell" => TelephoneNumberType.Work.Value,
                    "business" => TelephoneNumberType.Work.Value,
                    "unknown" => TelephoneNumberType.Other.Value,
                    "other" => TelephoneNumberType.Other.Value,
                    "fax" => TelephoneNumberType.Fax.Value,
                    "blank" => (int?)null,
                    _ => TelephoneNumberType.Other.Value
                };
        }
    }
    public class IncaceratedConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember))
            {
                return
                    sourceMember.ToLower() switch
                    {
                        "yes" => NullableBooleanType.Yes.Value,
                        "no" => NullableBooleanType.No.Value,
                        _ => (int?)null
                    };
            }
            return (int?)null;

        }
    }

    public class RelatedPersonCategoryConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;

            if ("aunt/uncle".Contains(sourceMember.ToLower())) 
                return PersonRelationType.AuntUncle.Value;
            else
                return
                    sourceMember.ToLower() switch
                    {
                        "spouse" => PersonRelationType.Spouse.Value,
                        "parent" => PersonRelationType.Parent.Value,
                        "child" => PersonRelationType.Child.Value,
                        "sibling" => PersonRelationType.Sibling.Value,
                        "cousin" => PersonRelationType.Cousin.Value,
                        "friend" => PersonRelationType.Friend.Value,
                        "lalw" => PersonRelationType.MayLiveWith.Value,
                        _ => (int?)null
                    };
        }
    }

    public class PersonGenderConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                sourceMember.ToLower() switch
                {
                    "m" => GenderType.Male.Value,
                    "f" => GenderType.Female.Value,
                    "u" => GenderType.Other.Value,
                    _ => (int?)null
                };
        }
    }

    public class PersonSearchCompletedMessageConvertor : IValueConverter<IEnumerable<Person>, string>
    {
        public string Convert(IEnumerable<Person> sourceMember, ResolutionContext context)
        {
            var strbuilder = new StringBuilder();
            if (sourceMember==null)
                return $"Auto search processing completed successfully. 0 Matched Persons found.";

            strbuilder.Append($"Auto search processing completed successfully. {sourceMember.Count()} Matched Persons found.\n");
            int i = 1;
            foreach(Person p in sourceMember)
            {
                strbuilder.Append($"For Matched Person {i} : ");
                strbuilder.Append($"{(p.Identifiers == null ? 0 : p.Identifiers.Count)} identifier(s) found.  ");
                strbuilder.Append($"{(p.Addresses == null ? 0 : p.Addresses.Count)} addresses found. ");
                strbuilder.Append($"{(p.Phones == null ? 0 : p.Phones.Count)} phone number(s) found. ");
                strbuilder.Append($"{(p.Names == null ? 0 : p.Names.Count)} name(s) found. ");
                strbuilder.Append($"{(p.Employments == null ? 0 : p.Employments.Count)} employment(s) found. ");
                strbuilder.Append($"{(p.RelatedPersons == null ? 0 : p.RelatedPersons.Count)} related person(s) found. ");
                strbuilder.Append($"{(p.BankInfos == null ? 0 : p.BankInfos.Count)} bank info(s) found. ");
                strbuilder.Append($"{(p.Vehicles == null ? 0 : p.Vehicles.Count)} vehicle(s) found. ");
                strbuilder.Append($"{(p.OtherAssets == null ? 0 : p.OtherAssets.Count)} other asset(s) found. ");
                strbuilder.Append($"{(p.CompensationClaims == null ? 0 : p.CompensationClaims.Count)} compensation claim(s) found. ");
                strbuilder.Append($"{(p.InsuranceClaims == null ? 0 : p.InsuranceClaims.Count)} insurance claim(s) found.\n");
                i++;
            }
            return strbuilder.ToString();
        }
    }

    public class IncomeAssistanceConvertor : IValueConverter<bool?, int>
    {
        public int Convert(bool? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null || sourceMember == false)
                return EmploymentRecordType.Employment.Value;
            else
                return EmploymentRecordType.IncomeAssistance.Value;
        }
    }

    public class IncomeAssistanceStatusConvertor : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember))
            {
                 int? source = Enumeration.GetAll<IncomeAssistanceStatusType>().FirstOrDefault(m => m.Name.Equals(sourceMember, StringComparison.OrdinalIgnoreCase))?.Value;

              return  (source == null) ? IncomeAssistanceStatusType.Unknown.Value : source;

            }
            else
                return IncomeAssistanceStatusType.Unknown.Value;
        }
    }

    public class RequestPriorityConverter : IValueConverter<RequestPriority, int?>
    {
        public int? Convert(RequestPriority sourceMember, ResolutionContext context)
        {
            return
                sourceMember.ToString().ToLower() switch
                {
                    "urgent" => RequestPriorityType.Urgent.Value,
                    "rush" => RequestPriorityType.Rush.Value,
                    "normal" => RequestPriorityType.Regular.Value,
                    _ => RequestPriorityType.Regular.Value
                };
        }
    }
}
