using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.OptionSets.Models;

namespace DynamicsAdapter.Web.Mapping
{
    public class InformationSourceConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            return Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Value == source)?.Name;
        }
    }


    public class IssuedByTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            InformationSourceType sourceType = Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase));
            return (sourceType == null) ? InformationSourceType.Other.Value : sourceType.Value;
        }
    }

    public class SuppliedByIDConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            InformationSourceType sourceType = Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Value == source);
            return (sourceType == null) ? InformationSourceType.Other.Name : sourceType.Name;
        }
    }

    public class SuppliedByValueConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            InformationSourceType sourceType = Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase));
            return (sourceType == null) ? InformationSourceType.Other.Value : sourceType.Value;
        }
    }

    public class IDType
    {
        public static readonly IDictionary<int, PersonalIdentifierType> IDTypeDictionary = new Dictionary<int, PersonalIdentifierType>
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
        public PersonalIdentifierType Convert(int? source, ResolutionContext context)
        {
            return source == null ? PersonalIdentifierType.Other : IDType.IDTypeDictionary[(int)source];
        }
    }

    public class PersonalIdentifierTypeConverter : IValueConverter<PersonalIdentifierType, int?>
    {
        public int? Convert(PersonalIdentifierType source, ResolutionContext context)
        {
            return IDType.IDTypeDictionary.FirstOrDefault(m => m.Value == source).Key;
        }
    }

    public class IssuedByConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return null;
        }
    }

    public class TelephoneNumberIdConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<TelephoneNumberType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    public class TelephoneNumberValueConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            return Enumeration.GetAll<TelephoneNumberType>().FirstOrDefault(m => m.Value == source)?.Name;
        }
    }

    public class ProvinceConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<CanadianProvinceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    public class ProvinceValueConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            return Enumeration.GetAll<CanadianProvinceType>().FirstOrDefault(m => m.Value == source)?.Name;
        }
    }

    public class AddressTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return source == null ? null : (int?)(AddressType.AddressTypeDictionary[source.ToLower()].Value);
        }
    }

    public class AddressType
    {
        public static readonly IDictionary<string, LocationType> AddressTypeDictionary = new Dictionary<string, LocationType>
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

    public class AddressTypeValueConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            return Enumeration.GetAll<LocationType>().FirstOrDefault(m => m.Value == source)?.Name;
        }
    }

    public class DateTimeOffsetConverter : IValueConverter<DateTimeOffset?, DateTime?>
    {
        public DateTime? Convert(DateTimeOffset? source, ResolutionContext context)
        {
            return source == null ? null : (DateTime?)(((DateTimeOffset)source).DateTime);
        }
    }

    public class NameCategoryConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return
                source.ToLower() switch
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
        public int? Convert(string source, ResolutionContext context)
        {
            return
                source.ToLower() switch
                {
                    "cell" => TelephoneNumberType.Cell.Value,
                    "home" => TelephoneNumberType.Home.Value,
                    "work" => TelephoneNumberType.Work.Value,
                    "homecell" => TelephoneNumberType.Home.Value,
                    "workcell" => TelephoneNumberType.Work.Value,
                    "business" => TelephoneNumberType.Work.Value,
                    "unknown" => TelephoneNumberType.Other.Value,
                    "other" => TelephoneNumberType.Other.Value,
                    "blank" => (int?)null,
                    _ => TelephoneNumberType.Other.Value
                };
        }
    }
    public class IncaceratedConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source))
            {
                return 
                    source.ToLower() switch
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
        public int? Convert(string source, ResolutionContext context)
        {
            if (source == null) return null;

            if ("aunt/uncle".Contains(source.ToLower())) 
                return PersonRelationType.AuntUncle.Value;
            else
                return
                    source.ToLower() switch
                    {
                        "spouse" => PersonRelationType.Spouse.Value,
                        "parent" => PersonRelationType.Parent.Value,
                        "child" => PersonRelationType.Child.Value,
                        "sibling" => PersonRelationType.Sibling.Value,
                        "cousin" => PersonRelationType.Cousin.Value,
                        "friend" => PersonRelationType.Friend.Value,
                        _ => (int?)null
                    };
        }
    }

    public class PersonGenderConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            if (source == null) return null;
            return
                source.ToLower() switch
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
        public string Convert(IEnumerable<Person> source, ResolutionContext context)
        {
            if(source==null)
                return $"Auto search processing completed successfully. 0 Matched Persons found.";

            string msg = $"Auto search processing completed successfully. {source.Count()} Matched Persons found.\n";
            int i = 1;
            foreach(Person p in source)
            {
                msg = msg 
                    + $"For Matched Person {i} : "
                    + $"{(p.Identifiers == null ? 0 : p.Identifiers.Count())} identifier(s) found.  "
                    + $"{(p.Addresses == null ? 0 : p.Addresses.Count())} addresses found. "
                    + $"{(p.Phones == null ? 0 : p.Phones.Count())} phone number(s) found. "
                    + $"{(p.Names == null ? 0 : p.Names.Count())} name(s) found. " 
                    + $"{(p.Employments == null ? 0 : p.Employments.Count())} employment(s) found. " 
                    + $"{(p.RelatedPersons == null ? 0 : p.RelatedPersons.Count())} related person(s) found. "
                    + $"{(p.BankInfos == null ? 0 : p.BankInfos.Count())} bank info(s) found. "
                    + $"{(p.Vehicles == null ? 0 : p.Vehicles.Count())} vehicle(s) found. "
                    + $"{(p.OtherAssets == null ? 0 : p.OtherAssets.Count())} other asset(s) found. "
                    + $"{(p.CompensationClaims == null ? 0 : p.CompensationClaims.Count())} compensation claim(s) found.\n";
                i++;
            };
            return msg;
        }
    }
}
