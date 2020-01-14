﻿using System;
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
            { IdentificationType.DriverLicense.Value, PersonalIdentifierType.DriverLicense },
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
           
        };
    }

    public class IdentifierTypeConverter : IValueConverter<int?, PersonalIdentifierType>
    {      
        public PersonalIdentifierType Convert(int? source, ResolutionContext context)
        {
            return source==null? PersonalIdentifierType.Other : IDType.IDTypeDictionary[(int)source];
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
            return Enumeration.GetAll<LocationType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
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
            return source ==null? null : (DateTime?)(((DateTimeOffset)source).DateTime);
        }
    }

    public class CountryConverter : IValueConverter<string, SSG_Country>
    {
        public SSG_Country Convert(string source, ResolutionContext context)
        {
            return new SSG_Country()
            {
                Name = source
            };
        }
    }

    public class CountryValueConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            return Enumeration.GetAll<LocationType>().FirstOrDefault(m => m.Value == source)?.Name;
        }
    }

    public class NameCategoryConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<PersonNameCategory>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}
