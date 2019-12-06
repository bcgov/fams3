using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
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

    public class IdentifierTypeConverter : IValueConverter<int?, int?>
    {
        public int? Convert(int? source, ResolutionContext context)
        {
            if (source == IdentificationType.DriverLicense.Value) return (int)PersonalIdentifierType.DriverLicense;
            else if (source == IdentificationType.SocialInsuranceNumber.Value) return (int)PersonalIdentifierType.SocialInsuranceNumber;
            else if (source == IdentificationType.PersonalHealthNumber.Value) return (int)PersonalIdentifierType.PersonalHealthNumber;
            else if (source == IdentificationType.BirthCertificate.Value) return (int)PersonalIdentifierType.BirthCertificate;
            else if (source == IdentificationType.CorrectionsId.Value) return (int)PersonalIdentifierType.CorrectionsId;
            else if (source == IdentificationType.NativeStatusCard.Value) return (int)PersonalIdentifierType.NativeStatusCard;
            else if (source == IdentificationType.Passport.Value) return (int)PersonalIdentifierType.Passport;
            else if (source == IdentificationType.WcbClaim.Value) return (int)PersonalIdentifierType.WcbClaim;
            else if (source == IdentificationType.Other.Value) return (int)PersonalIdentifierType.Other;
            else if (source == IdentificationType.SecurityKeyword.Value) return (int)PersonalIdentifierType.SecurityKeyword;
            else return null;
        }
    }

    public class PersonalIdentifierTypeConverter : IValueConverter<PersonalIdentifierType, int?>
    {
        public int? Convert(PersonalIdentifierType source, ResolutionContext context)
        {
            return source.ToDynamicIdentifierType();
        }
    }

    public class ProvinceConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<CanadianProvinceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    public class AddressTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<LocationType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    public class CountryConverter : IValueConverter<string, SSG_Country?>
    {
        public SSG_Country Convert(string source, ResolutionContext context)
        {
            return new SSG_Country()
            {
                Name = source
            };
        }
    }

    public static class SearchApiIdentifierType
    {
        public static int? ToDynamicIdentifierType(this PersonalIdentifierType value)
        {
            switch (value)
            {
                case PersonalIdentifierType.DriverLicense:
                    return IdentificationType.DriverLicense.Value;
                case PersonalIdentifierType.SocialInsuranceNumber:
                    return IdentificationType.SocialInsuranceNumber.Value;
                case PersonalIdentifierType.PersonalHealthNumber:
                    return IdentificationType.PersonalHealthNumber.Value;
                case PersonalIdentifierType.BirthCertificate:
                    return IdentificationType.BirthCertificate.Value;
                case PersonalIdentifierType.CorrectionsId:
                    return IdentificationType.CorrectionsId.Value;
                case PersonalIdentifierType.NativeStatusCard:
                    return IdentificationType.NativeStatusCard.Value;
                case PersonalIdentifierType.Passport:
                    return IdentificationType.Passport.Value;
                case PersonalIdentifierType.WcbClaim:
                    return IdentificationType.WcbClaim.Value;
                case PersonalIdentifierType.Other:
                    return IdentificationType.Other.Value;
                case PersonalIdentifierType.SecurityKeyword:
                    return IdentificationType.SecurityKeyword.Value;
                default:
                    return null;
            }
        }
    }
}
