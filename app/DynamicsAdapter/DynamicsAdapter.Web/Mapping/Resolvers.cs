using AutoMapper;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.Pension;
using Fams3Adapter.Dynamics.RealEstate;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchResponse;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicsAdapter.Web.Mapping
{

    public class Date1Resolver : IValueResolver<PersonalInfo, DynamicsEntity, DateTime?>
    {
        public DateTime? Resolve(PersonalInfo source, DynamicsEntity destination, DateTime? destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Value.DateTime;
        }

    }


    public class Date1LabelResolver : IValueResolver<PersonalInfo, DynamicsEntity, string>
    {
        public string Resolve(PersonalInfo source, DynamicsEntity destination, string destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Key;
        }
    }

    public class Date2Resolver : IValueResolver<PersonalInfo, DynamicsEntity, DateTime?>
    {
        public DateTime? Resolve(PersonalInfo source, DynamicsEntity destination, DateTime? destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Value.DateTime;
        }
    }

    public class Date2LabelResolver : IValueResolver<PersonalInfo, DynamicsEntity, string>
    {
        public string Resolve(PersonalInfo source, DynamicsEntity destination, string destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Key;
        }
    }

    public class ReferenceDatesResolver : IValueResolver<DynamicsEntity, PersonalInfo, ICollection<ReferenceDate>>
    {
        public ICollection<ReferenceDate> Resolve(DynamicsEntity source, PersonalInfo destination, ICollection<ReferenceDate> referenceDates, ResolutionContext context)
        {
            List<ReferenceDate> dates = new List<ReferenceDate>();
            if (source.Date1 != null)
            {
                ReferenceDate referenceDate = new ReferenceDate
                {
                    Index = 0,
                    Key = source.Date1Label,
                    Value = new DateTimeOffset((DateTime)source.Date1)
                };
                dates.Add(referenceDate);
            }
            if (source.Date2 != null)
            {
                ReferenceDate referenceDate = new ReferenceDate
                {
                    Index = 1,
                    Key = source.Date2Label,
                    Value = new DateTimeOffset((DateTime)source.Date2)
                };
                dates.Add(referenceDate);
            }
            return dates;
        }
    }

    public class PersonReferenceDatesResolver : IValueResolver<SSG_SearchRequestResponse, Person, ICollection<ReferenceDate>>
    {
        public ICollection<ReferenceDate> Resolve(SSG_SearchRequestResponse source, Person destination, ICollection<ReferenceDate> referenceDates, ResolutionContext context)
        {
            List<ReferenceDate> dates = new List<ReferenceDate>();
            if (source.SSG_Persons[0].Date1 != null)
            {
                ReferenceDate referenceDate = new ReferenceDate
                {
                    Index = 0,
                    Key = source.SSG_Persons[0].Date1Label,
                    Value = new DateTimeOffset((DateTime)source.SSG_Persons[0].Date1)
                };
                dates.Add(referenceDate);
            }
            if (source.SSG_Persons[0].Date2 != null)
            {
                ReferenceDate referenceDate = new ReferenceDate
                {
                    Index = 1,
                    Key = source.SSG_Persons[0].Date2Label,
                    Value = new DateTimeOffset((DateTime)source.SSG_Persons[0].Date2)
                };
                dates.Add(referenceDate);
            }
            return dates;
        }
    }

    public class PersonalIdentifier_ReferenceDateResolver : IValueResolver<IdentifierEntity, PersonalIdentifier, ICollection<ReferenceDate>>
    {
        public ICollection<ReferenceDate> Resolve(IdentifierEntity source, PersonalIdentifier destination, ICollection<ReferenceDate> destMember, ResolutionContext context)
        {
            List<ReferenceDate> referDates = new List<ReferenceDate>();
            if (source.Date1 != null)
            {
                referDates.Add(new ReferenceDate() { Index = 0, Key = source.Date1Label, Value = new DateTimeOffset((DateTime)source.Date1) });
            }
            if (source.Date2 != null)
            {
                referDates.Add(new ReferenceDate() { Index = 1, Key = source.Date2Label, Value = new DateTimeOffset((DateTime)source.Date2) });
            }
            return referDates;
        }
    }

    public class NamesResolver : IValueResolver<SSG_SearchApiRequest, PersonSearchRequest, ICollection<Name>>
    {
        public ICollection<Name> Resolve(SSG_SearchApiRequest source, PersonSearchRequest destination, ICollection<Name> destMember, ResolutionContext context)
        {
            if (source?.SearchRequest?.ApplicantFirstName != null || source?.SearchRequest?.ApplicantLastName != null)
                return new List<Name>() { new Name { FirstName = source?.SearchRequest?.ApplicantFirstName, LastName = source?.SearchRequest?.ApplicantLastName, Owner = OwnerType.Applicant } };
            else
                return null;
        }
    }

    public class AgencyResolver : IValueResolver<SSG_SearchApiRequest, PersonSearchRequest, Agency>
    {
        public Agency Resolve(SSG_SearchApiRequest source, PersonSearchRequest destination, Agency destMember, ResolutionContext context)
        {
            return new Agency
            {

                ReasonCode = source.SearchRequest?.SearchReason?.ReasonCode switch
                {
                    "EnfPayAgr" => SearchReasonCode.EnfPayAgr,
                    "ChngAccAgr" => SearchReasonCode.ChngAccAgr,
                    "ChngCustAg" => SearchReasonCode.ChngCustAg,
                    "AstRecpAgy" => SearchReasonCode.AstRecpAgy,
                    "Other" => SearchReasonCode.Other,
                    _ => SearchReasonCode.Other
                }
            };

        }
    }

    public class EmployerPhoneResponseResolver : IValueResolver<SSG_Employment, Employer, ICollection<Phone>>
    {
        public ICollection<Phone> Resolve(SSG_Employment source, Employer destination, ICollection<Phone> destMember, ResolutionContext context)
        {
            List<Phone> phones = new List<Phone>();
            List<EmployerContact> contacts = new List<EmployerContact>();

            if (!string.IsNullOrEmpty(source?.PrimaryPhoneNumber))
            {
                Phone phone = new Phone { PhoneNumber = source.PrimaryPhoneNumber, Extension = source.PrimaryPhoneExtension, Type = "primaryPhone" };
                phones.Add(phone);
            }

            if (!string.IsNullOrEmpty(source?.PrimaryFax))
                phones.Add(new Phone { PhoneNumber = source.PrimaryFax, Type = "primaryFax" });

            Phone primaryContactPhone = null;
            if (!string.IsNullOrEmpty(source?.PrimaryContactPhone)) {
                primaryContactPhone = new Phone { PhoneNumber = source.PrimaryContactPhone, Extension = source.PrimaryContactPhoneExt, Type = "primaryContactPhone" };
                phones.Add(primaryContactPhone); 
            }
            EmployerContact primaryContact = null;
            if (!string.IsNullOrEmpty(source?.PrimaryContactEmail)|| primaryContactPhone!=null) {
                primaryContact = new EmployerContact();
                primaryContact.Email = source.PrimaryContactEmail;
                primaryContact.Phone = primaryContactPhone;
                primaryContact.Name = source.ContactPerson;
                primaryContact.Type = "Primary";
                contacts.Add(primaryContact);
            }

            if (source.SSG_EmploymentContacts != null)
            {
                foreach (SSG_EmploymentContact contact in source.SSG_EmploymentContacts)
                {
                    if (!string.IsNullOrEmpty(contact.PhoneNumber))
                    {
                        phones.Add(new Phone
                        {
                            ContactName = contact.ContactName,
                            PhoneNumber = contact.PhoneNumber,
                            Extension = contact.PhoneExtension,
                            Description = contact.Description,
                            Type = contact.PhoneType == null ? null : Enumeration.GetAll<TelephoneNumberType>().SingleOrDefault(m => m.Value == contact.PhoneType.Value)?.Name
                        });
                    }
                    if (!string.IsNullOrEmpty(contact.FaxNumber))
                    {
                        phones.Add(new Phone
                        {
                            ContactName = contact.ContactName,
                            PhoneNumber = contact.FaxNumber,
                            Description = contact.Description,
                            Type = contact.PhoneType == null ? "Fax" : Enumeration.GetAll<TelephoneNumberType>().FirstOrDefault(m => m.Value == contact.PhoneType.Value)?.Name
                        });
                    }
                    if (!string.IsNullOrEmpty(contact.Email)
                        || !string.IsNullOrEmpty(contact.PhoneNumber)
                        || !string.IsNullOrEmpty(contact.ContactName)
                        || !string.IsNullOrEmpty(contact.FaxNumber))
                    {
                        contacts.Add(new EmployerContact
                        {
                            Email = contact.Email,
                            Phone = new Phone
                            {
                                ContactName = contact.ContactName,
                                PhoneNumber = contact.PhoneNumber,
                                Extension = contact.PhoneExtension,
                                Description = contact.Description,
                                Type = contact.PhoneType == null ? null : Enumeration.GetAll<TelephoneNumberType>().SingleOrDefault(m => m.Value == contact.PhoneType.Value)?.Name
                            },
                            Name = contact.ContactName,
                            Fax = contact.FaxNumber,
                            Type = "additional"
                        });
                    }
                }
            }
            if (contacts.Count > 0)
                destination.EmployerContacts = contacts;

            return phones;
        }
    }


    public class EmployerAddressResponseResolver : IValueResolver<SSG_Employment, Employer, Address>
    {
        public Address Resolve(SSG_Employment source, Employer destination, Address destMember, ResolutionContext context)
        {
            Address addr = new Address
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                City = source.City,
                StateProvince = source.CountrySubdivisionText,
                CountryRegion = source.CountryText,
                ZipPostalCode = source.PostalCode
            };
            return addr;
        }
    }

    public class EmploymentEntityNotesResolver : IValueResolver<Employment, EmploymentEntity, string>
    {
        public string Resolve(Employment source, EmploymentEntity destination, string destMember, ResolutionContext context)
        {
            if (source.Employer == null || source.Employer.Phones==null || source.Employer.Phones.Count <= 2) return source.Notes;
            int index = 0;
            string notes=string.Empty;
            foreach(Phone p in source.Employer.Phones)
            {
                if( index > 2)
                {
                    notes += $"{p.ContactName} {p.PhoneNumber} {p.Extension}\r\n";
                }
                index++;
            }
            return $"{notes}{source.Notes}";
        }
    }

    public class ProviderAddressResponseResolver : IValueResolver<SSG_Asset_PensionDisablility, Pension, Address>
    {
        public Address Resolve(SSG_Asset_PensionDisablility source, Pension destination, Address destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.AddressLine1) && string.IsNullOrEmpty(source.AddressLine2) &&
               string.IsNullOrEmpty(source.AddressLine3) && string.IsNullOrEmpty(source.City) &&
               string.IsNullOrEmpty(source.Country) && string.IsNullOrEmpty(source.ProvinceState))
                return null;

            Address addr = new Address
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                City = source.City,
                StateProvince = source.ProvinceState,
                CountryRegion = source.Country,
                ZipPostalCode = source.PostalCode
            };
            return addr;
        }
    }

    public class PropertyAddressResponseResolver : IValueResolver<SSG_Asset_RealEstateProperty, RealEstateProperty, Address>
    {
        public Address Resolve(SSG_Asset_RealEstateProperty source, RealEstateProperty destination, Address destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.AddressLine1) && string.IsNullOrEmpty(source.AddressLine2) &&
               string.IsNullOrEmpty(source.AddressLine3) && string.IsNullOrEmpty(source.City) &&
               string.IsNullOrEmpty(source.Country) && string.IsNullOrEmpty(source.ProvinceState))
                return null;

            Address addr = new Address
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                City = source.City,
                StateProvince = source.ProvinceState,
                CountryRegion = source.Country,
                ZipPostalCode = source.PostalCode
            };
            return addr;
        }
    }

    public class VehicleOwnersRespnseResolver : IValueResolver<VehicleEntity, Vehicle, ICollection<InvolvedParty>>
    {
        public ICollection<InvolvedParty> Resolve(VehicleEntity source, Vehicle destination, ICollection<InvolvedParty> destMember, ResolutionContext context)
        {
            List<InvolvedParty> owners = new List<InvolvedParty>();
            if (!string.IsNullOrEmpty(source.Lessee) || !string.IsNullOrEmpty(source.LeasingCom))
            {
                owners.Add(new InvolvedParty
                {
                    Address = source.LeasingComAddr,
                    Name = new Name { FirstName = source.Lessee, Type = "Lessee full name" },
                    Organization = source.LeasingCom,
                    Type = "Leased"
                });
                return owners;
            }
            return null;
        }
    }

    public class SafetyConcernTypeResolver : IValueResolver<Person, SafetyConcernEntity, int?>
    {
        public int? Resolve(Person source, SafetyConcernEntity destination, int? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.CautionFlag) && string.IsNullOrEmpty(source.CautionReason)) return null;
            SafetyConcernType type = null;
            if (!string.IsNullOrEmpty(source.CautionFlag))
            {
                type = SafetyConcernType.GetAll<SafetyConcernType>().SingleOrDefault(m => string.Equals(m.Name, source.CautionFlag, StringComparison.InvariantCultureIgnoreCase));
                destination.SupplierTypeCode = source.CautionFlag;
                if (type == null) return SafetyConcernType.Other.Value;
                return type.Value;
            }
            if (!string.IsNullOrEmpty(source.CautionReason))
            {
                type = SafetyConcernType.GetAll<SafetyConcernType>().SingleOrDefault(m => string.Equals(m.Name, source.CautionReason, StringComparison.InvariantCultureIgnoreCase));
                destination.SupplierTypeCode = source.CautionReason;
                if (type == null) return SafetyConcernType.Other.Value;
                return type.Value;
            }

            return null;
        }
    }
}
