using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.OptionSets.Models;
using System.Text;
using Fams3Adapter.Dynamics.SearchRequest;
using DynamicsAdapter.Web.PersonSearch.Models;

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
            { "correctionalcenter", LocationType.CorrectionalCentre},
            { "unknown", LocationType.Other }
        };
    }

    public class AddressTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            return sourceMember == null ? null : (int?)(AddressType.AddressTypeDictionary[sourceMember.ToLower()].Value);
        }
    }

    public class AddressTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return LocationType.GetAll<LocationType>().FirstOrDefault(m => m.Value == sourceMember)?.Name;
        }
    }

    public class SafetyConcernTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return SafetyConcernType.GetAll<SafetyConcernType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;
        }
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

    public static class PhoneType
    {
        internal static readonly IDictionary<string, int?> PhoneTypeDictionary = new Dictionary<string, int?>
        {
            { "cell", TelephoneNumberType.Cell.Value },
            { "home", TelephoneNumberType.Home.Value },
            { "work", TelephoneNumberType.Work.Value },
            { "homecell", TelephoneNumberType.Home.Value },
            { "workcell", TelephoneNumberType.Work.Value },
            { "business", TelephoneNumberType.Work.Value },
            { "unknown", TelephoneNumberType.Other.Value },
            { "other", TelephoneNumberType.Other.Value},
            { "fax", TelephoneNumberType.Fax.Value},
            { "blank", null}
        };
    }

    public class PhoneTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            if (PhoneType.PhoneTypeDictionary.ContainsKey(sourceMember.ToLower()))
                return (int?)(PhoneType.PhoneTypeDictionary[sourceMember.ToLower()].Value);
            else
                return TelephoneNumberType.Other.Value;
        }
    }

    public class EmailTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            EmailType type = Enumeration.GetAll<EmailType>().SingleOrDefault(m => m.Name.Equals(sourceMember, StringComparison.InvariantCultureIgnoreCase));
            return type == null ? EmailType.Personal.Value : type.Value;
        }
    }

    public class PhoneTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            return sourceMember == null ? null :
                (string)(PhoneType.PhoneTypeDictionary.FirstOrDefault(m => m.Value == (int)sourceMember).Key);
        }
    }

    public class PhoneNumberResponseConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember)) return null;
            string[] number = sourceMember.Split("|");
            if(number != null && number.Length>1 && number[0].Trim().Equals(Constants.OutOfProvinceRJ, StringComparison.InvariantCultureIgnoreCase) )
            {
                return number[1].Trim();
            }
            else
            {
                return sourceMember;
            }
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

    public class IncaceratedReponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == NullableBooleanType.Yes.Value) return "yes";
            if (sourceMember == NullableBooleanType.No.Value) return "no";
            return null;
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
                        "lalw" => PersonRelationType.LastLiveWith.Value,
                        "mlw" => PersonRelationType.MayLiveWith.Value,
                        "dependent" => PersonRelationType.Child.Value,
                        "subscriber" => PersonRelationType.AccountHolder.Value,
                        _ => PersonRelationType.Other.Value
                    };
        }
    }

    public static class GenderDictionary
    {
        internal static readonly IDictionary<string, int> GenderTypeDictionary = new Dictionary<string, int>
        {
            { "m", GenderType.Male.Value },
            { "f", GenderType.Female.Value },
            { "u", GenderType.Other.Value }
        };
    }

    public class PersonGenderConverter : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                GenderDictionary.GenderTypeDictionary.ContainsKey(sourceMember.ToLower()) ?
                    GenderDictionary.GenderTypeDictionary[sourceMember.ToLower()] : (int?)null;
        }
    }

    public class PersonGenderTypeConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                GenderDictionary.GenderTypeDictionary.FirstOrDefault(m => m.Value == (int)sourceMember).Key;

        }
    }


    public class EmploymentStatusResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<EmploymentStatusType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class SelfEmployComTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<SelfEmploymentCompanyType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class SelfEmployComRoleResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<SelfEmploymentCompanyRoleType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class IncomeAssistanceStatusResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<IncomeAssistanceStatusType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class AccountTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<BankAccountType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class RelatedPersonCategoryResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            if (sourceMember == PersonRelationType.LastLiveWith.Value)
            {
                return "LALW";
            }
            return
                Enumeration.GetAll<PersonRelationType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class RelatedPersonTypeConverter : IValueConverter<RelatedPerson, int?>
    {
        public int? Convert(RelatedPerson sourceMember, ResolutionContext context)
        {
            return Fams3Adapter.Dynamics.Types.RelatedPersonPersonType.Relation.Value;

        }
    }

    public class RelatedPersonTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<RelatedPersonPersonType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class EmailTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<EmailType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class EmploymentIncomeClsResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<IncomeAssistanceClassType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }
    

    public class SocailMediaTypeResponseConverter : IValueConverter<int?, string>
    {
        public string Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return
                Enumeration.GetAll<SocialMediaType>().SingleOrDefault(m => m.Value == sourceMember)?.Name;

        }
    }

    public class PersonSearchCompletedMessageConvertor : IValueConverter<PersonSearchCompleted, string>
    {
        public string Convert(PersonSearchCompleted sourceMember, ResolutionContext context)
        {
            var strbuilder = new StringBuilder();
            if (sourceMember.MatchedPersons == null)
                return $"Auto search processing completed successfully. 0 Matched Persons found.";
            var matchedPersons = sourceMember.MatchedPersons;

            
            if (!string.IsNullOrEmpty(sourceMember.Message))
                strbuilder.Append($"Auto search processing completed successfully. {sourceMember.Message}. {matchedPersons.Count()} Matched Persons found.\n");
            else
                strbuilder.Append($"Auto search processing completed successfully. {matchedPersons.Count()} Matched Persons found.\n");


            int i = 1;
            foreach (PersonFound p in matchedPersons)
            {
           
               
                strbuilder.Append($"For Matched Person {i} : ");
                if (p.SourcePersonalIdentifier != null)
                    strbuilder.Append($" Source ID:- {p.SourcePersonalIdentifier.Type}/{p.SourcePersonalIdentifier.Value} - ");
                strbuilder.Append($"{(p.Identifiers == null ? 0 : p.Identifiers.Count)} identifier(s) found.  ");
                strbuilder.Append($"{(p.Addresses == null ? 0 : p.Addresses.Count)} addresses found. ");
                strbuilder.Append($"{(p.Phones == null ? 0 : p.Phones.Count)} phone number(s) found. ");
                strbuilder.Append($"{(p.Emails == null ? 0 : p.Emails.Count)} email(s) found. ");
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

    public class PrimaryPhoneNumberConvertor : IValueConverter<ICollection<Phone>, string>
    {
        public string Convert(ICollection<Phone> phones, ResolutionContext context)
        {
            return phones?.FirstOrDefault(m => string.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase))?.PhoneNumber;
        }
    }

    public class PrimaryPhoneExtConvertor : IValueConverter<ICollection<Phone>, string>
    {
        public string Convert(ICollection<Phone> phones, ResolutionContext context)
        {
            return phones?.FirstOrDefault(m => string.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase))?.Extension;
        }
    }

    public class PrimaryContactPhoneNumberConvertor : IValueConverter<ICollection<Phone>, string>
    {
        public string Convert(ICollection<Phone> phones, ResolutionContext context)
        {
            if (phones == null) return null;
            if (phones.Where(m => string.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase)).Count() > 1)
            {
                return phones.Where(m => string.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase)).ElementAt(1).PhoneNumber;
            };
            return null;
        }
    }

    public class PrimaryContactPhoneExtConvertor : IValueConverter<ICollection<Phone>, string>
    {
        public string Convert(ICollection<Phone> phones, ResolutionContext context)
        {
            if (phones == null) return null;
            if (phones.Where(m => string.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase)).Count() > 1)
            {
                return phones.Where(m => string.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase)).ElementAt(1).Extension;
            };
            return null;
        }
    }

    public class IncomeAssistanceConvertor : IValueConverter<string, int>
    {
        public int Convert(string sourceMember, ResolutionContext context)
        {
            int ? source = Enumeration.GetAll<IncomeAssistanceStatusType>().FirstOrDefault(m => m.Name.Equals(sourceMember, StringComparison.OrdinalIgnoreCase))?.Value;
            if( source==null || source== IncomeAssistanceStatusType.Unknown.Value)
                return EmploymentRecordType.Employment.Value;
            else
                return EmploymentRecordType.IncomeAssistance.Value;
        }
    }
    public class IncomeAssistanceResponseConvertor : IValueConverter<int?, bool?>
    {
        public bool? Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null)
                return false;
            else if (sourceMember == EmploymentRecordType.IncomeAssistance.Value)
                return true;

            return false;
        }
    }

    public class IncomeAssistanceStatusConvertor : IValueConverter<string, int?>
    {
        public int? Convert(string sourceMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember))
            {
                int? source = Enumeration.GetAll<IncomeAssistanceStatusType>().FirstOrDefault(m => m.Name.Equals(sourceMember, StringComparison.OrdinalIgnoreCase))?.Value;

                return (source == null) ? IncomeAssistanceStatusType.Unknown.Value : source;

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

    public class RequestPriorityTypeConverter : IValueConverter<int?, RequestPriority>
    {
        public RequestPriority Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == RequestPriorityType.Urgent.Value) return RequestPriority.Urgent;
            if (sourceMember == RequestPriorityType.Rush.Value) return RequestPriority.Rush;
            if (sourceMember == RequestPriorityType.Regular.Value) return RequestPriority.Normal;
            return RequestPriority.Normal;
        }
    }

    public class SearchReasonCodeConverter : IValueConverter<SSG_SearchRequestReason, SearchReasonCode>
    {
        public SearchReasonCode Convert(SSG_SearchRequestReason reason, ResolutionContext context)
        {
            if (reason == null) return SearchReasonCode.Other;
            try
            {
                return (SearchReasonCode)Enum.Parse(typeof(SearchReasonCode), reason.ReasonCode, true);
            }
            catch (Exception)
            {
                return SearchReasonCode.Other;
            }
        }
    }

    public class PersonSoughtRoleConverter : IValueConverter<int?, SoughtPersonType>
    {
        public SoughtPersonType Convert(int? sourceMember, ResolutionContext context)
        {
            if (sourceMember == PersonSoughtType.P.Value) return SoughtPersonType.PAYOR;
            if (sourceMember == PersonSoughtType.R.Value) return SoughtPersonType.RECIPIENT;
            return SoughtPersonType.PAYOR;
        }
    }

    public class MinsToDaysConverter : IValueConverter<int, int?>
    {
        public int? Convert(int mins, ResolutionContext context)
        {
            if(mins<=0) return null;
            TimeSpan timeSpan = new TimeSpan(0,mins,0);
            return timeSpan.Days+1;
        }
    }
}
