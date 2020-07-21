using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

namespace SearchAdapter.Sample.SearchRequest
{
    public class PersonSearchCompletedSample : PersonSearchCompleted
    {

       
        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public IEnumerable<PersonFound> MatchedPersons { get; set; }
    }
    public static class FakePersonBuilder
    {
        public static PersonSearchCompleted BuildFakePersonSearchCompleted(Guid searchrequestId, string SearchRequestKey, string firstname, string lastname, DateTime dob, ProviderProfile _profile)
        {

            return new PersonSearchCompletedSample()
            {
                ProviderProfile = _profile,
                SearchRequestId = searchrequestId,
                SearchRequestKey = SearchRequestKey,
                TimeStamp = DateTime.Now,
                MatchedPersons = new List<PersonFound>()
                {
                    new PersonFound(){
                        FirstName = firstname,
                        LastName = lastname,
                        DateOfBirth = dob,
                        Incacerated = "N",
                        DateDeathConfirmed = false,
                        DateOfDeath = null,
                        Gender = "F",
                        MiddleName = "FoundMiddleName",
                        OtherName = "FoundOtherName",
                        Notes = "some notes",
                        Height = "178",
                        Complexion = "Dark",
                        DistinguishingFeatures = "None",
                        EyeColour = "Hazel",
                        HairColour = "Blonde",
                        WearGlasses = "No",
                        Weight= "200",
                        SourcePersonalIdentifier = new PersonalIdentifier()
                        {
                            Type = PersonalIdentifierType.BCDriverLicense,
                            TypeCode = "BCDL",
                            IssuedBy = "BC",
                            Value = new Random().Next(0, 50000).ToString()
                        },
                        Identifiers = new List<PersonalIdentifier>()
                        {

                            new PersonalIdentifier()
                            {
                                Type = PersonalIdentifierType.BCDriverLicense,
                                TypeCode = "BCDL",
                                Description = "Sample Identifier Description",
                                Notes = "Sample Identifier Notes",
                                IssuedBy = "BC",
                                Owner = OwnerType.NotApplicable,
                                Value = new Random().Next(0, 50000).ToString(),
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                                }


                            },
                             new PersonalIdentifier()
                            {
                                Type = PersonalIdentifierType.BCID,
                                TypeCode = "BCID",
                                Description = "Sample Identifier Description",
                                Notes = "Sample Identifier Notes",
                                IssuedBy = "BC",
                                Owner = OwnerType.NotApplicable,
                                Value = new Random().Next(0, 50000).ToString(),
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                                }


                            }

                        },
                        InsuranceClaims = new List<InsuranceClaim>()
                        {
                            new InsuranceClaim ()
                            {
                                Adjustor = new Name {FirstName = "FirstName", LastName="LastName",OtherName = "OtherName",MiddleName="MiddleName"},
                                AdjustorPhone = new Phone {Extension="1", PhoneNumber = "222222222",  Type="Phone"},
                                ClaimNumber = "123123123",
                                ClaimStatus = "Status",
                                ClaimType = "Type",
                                 ClaimCentre = new ClaimCentre
                                 {
                                     ContactAddress = new Address
                                     {
                                        AddressLine1 = "address in line 1",
                                        AddressLine2 = "address in line 2",
                                        AddressLine3 = "address in line 3",
                                        StateProvince = "British Columbia",
                                        City = "victoria" ,
                                        CountryRegion= "canada",
                                        ZipPostalCode = "t4t4t4",

                                     },
                                     Location = "123",
                                     ContactNumber = new List<Phone> ()
                                     {
                                         new Phone {Extension="1", PhoneNumber = "222222222",  Type="Phone"},
                                         new Phone {Extension="1", PhoneNumber = "333333333",  Type="Fax"},
                                         new Phone {Extension="1", PhoneNumber = "444444444",  Type="Phone"}
                                     }
                                 },
                                 Description = "Description",
                                 Identifiers = new List<PersonalIdentifier>
                                 {
                                    new PersonalIdentifier
                                    {
                                        Type = PersonalIdentifierType.BCDriverLicense,
                                        TypeCode = "BCDL",
                                        Description = "Sample Identifier Description",
                                        Notes = "Sample Identifier Notes",
                                        IssuedBy = "BC",
                                        Owner = OwnerType.NotApplicable,
                                        Value = new Random().Next(0, 50000).ToString()
                                    },
                                    new PersonalIdentifier
                                    {
                                        Type = PersonalIdentifierType.PersonalHealthNumber,
                                        TypeCode = "PHN",
                                        Description = "Sample Identifier Description",
                                        Notes = "Sample Identifier Notes",
                                        IssuedBy = "BC",
                                        Owner = OwnerType.NotApplicable,
                                        Value = new Random().Next(0, 50000).ToString()
                                    },

                                 },
                                 ReferenceDates =  new List<ReferenceDate>()
                                 {
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                },
                                 Notes = "notes",
                                 InsuredParties = new List<InvolvedParty>()
                                 {
                                    new InvolvedParty()
                                    {
                                      Name =  new Name {FirstName = "FirstName", LastName="LastName",OtherName = "OtherName",MiddleName="MiddleName"},
                                      Organization= "Organization",
                                      Type = "Type",
                                      TypeDescription = "Type Description"
                                    }

                                 }
                            }

                        },
                        Addresses = new List<Address>()
                        {
                            new Address()
                            {
                                Type = "mailing",
                                AddressLine1 = "address in line 1",
                                AddressLine2 = "address in line 2",
                                AddressLine3 = "address in line 3",
                                StateProvince = "British Columbia",
                                City = "victoria" ,
                                CountryRegion= "canada",
                                ZipPostalCode = "t4t4t4",
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                },
                                Description = "description",
                                Notes = "notes"
                            },
                             new Address()
                            {
                                Type = "unknown",
                                AddressLine1 = "residence address in line 1",
                                AddressLine2 = "residence address in line 2",
                                StateProvince = "British Columbia",
                                City = "vancouver" ,
                                CountryRegion="canada",
                                ZipPostalCode = "5r5r5r",
                                ReferenceDates = null,
                                Description = "description2",
                                Notes = "notes2"
                            }
                        },
                        Names = new List<Name>()
                        {
                            new Name()
                            {
                                Type = "legal name",
                                FirstName = "firstName",
                                LastName = "LastName",
                                MiddleName = "MiddleName",
                                  ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                },
                                Description = "Sample Name"
                            }
                        },
                        Phones = new List<Phone>()
                        {
                            new Phone
                            {
                                Description = "Sample Phone",
                                Extension = "1233",
                                Notes = "Notes",
                                PhoneNumber ="768990123",
                                Type = "home",
                                 ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                }

                            },
                            new Phone
                            {
                                Description = "Sample Phone",
                                Extension = "1233",
                                PhoneNumber ="768990123",
                                Type = "work",
                                 ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                }
                            }

                        },
                        Employments = new List<Employment>()
                        {
                            new Employment
                            {
                                EmploymentConfirmed = true,
                                IncomeAssistance = true,
                                IncomeAssistanceStatus = "Real Status",
                                 Notes = "Sample Notes",
                                 Occupation = "Occupation",
                                 ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                },
                                 Website = "www.websitejob.com",
                                 Employer = new Employer
                                 {
                                     Address = new Address
                                     {
                                         AddressLine1 = "Address 1",
                                         AddressLine2 = "Address 2",
                                         AddressLine3 = "Address 3",
                                         City = "City",
                                         StateProvince = "AB",
                                         CountryRegion = "Canada",
                                         ZipPostalCode = "VR4 123"
                                     },
                                     ContactPerson = "Surname FirstName",
                                     Name = "Sample Company",
                                     OwnerName = "Sample Company Owner",
                                     Phones = new List<Phone>()
                                     {
                                         new Phone {PhoneNumber = "12345678", Extension ="123", Type ="Phone"},
                                         new Phone {PhoneNumber = "901237123", Extension ="123", Type ="Fax"},
                                         new Phone {PhoneNumber = "762349303", Extension ="123", Type ="Phone"}
                                     }
                                 }
                            },
                              new Employment
                            {
                                EmploymentConfirmed = false,
                                IncomeAssistance = false,
                                IncomeAssistanceStatus = "Real Status",
                                 Notes = "Sample Notes",
                                 Occupation = "Occupation",
                                 ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                                },
                                 Website = "www.websitejob.com",
                                 Employer = new Employer
                                 {
                                     Address = new Address
                                     {
                                         AddressLine1 = "Address 1",
                                         AddressLine2 = "Address 2",
                                         AddressLine3 = "Address 3",
                                         City = "City",
                                         StateProvince = "AB",
                                         CountryRegion = "Canada",
                                         ZipPostalCode = "VR4 123"
                                     },
                                     ContactPerson = "Surname FirstName",
                                     Name = "Sample Company",
                                     OwnerName = "Sample Company Owner",
                                     Phones = new List<Phone>()
                                     {
                                         new Phone {PhoneNumber = "12345678", Extension ="123", Type ="Phone"},
                                         new Phone {PhoneNumber = "901237123", Extension ="123", Type ="Fax"},
                                         new Phone {PhoneNumber = "762349303", Extension ="123", Type ="Phone"}
                                     }
                                 }
                            }
                        },
                        RelatedPersons = new List<RelatedPerson>()
                        {
                            new RelatedPerson
                            {
                                Description = "RelatedPersonDescription",
                                DateOfBirth = new DateTime(1987, 1,1),
                                Notes = "RelatedPerson Notes",
                                FirstName ="SampleRelateFirst",
                                LastName="SampleRelateLast",
                                MiddleName="SampleRelateMiddle",
                                OtherName="SampleRelateOther",
                                Type = "Wife",
                                Gender="F",
                                 ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Relation Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Relation End Date", Value=new DateTime(2020,9,1) }
                                }
                            }
                        },
                        BankInfos = new List<BankInfo>()
                        {
                            new BankInfo()
                            {
                                BankName="BankName",
                                AccountNumber="AccountNumber123",
                                Branch="Branch",
                                TransitNumber="TransitNumber123",
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Account Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Account Expired Date", Value=new DateTime(2020,9,1) }
                                },
                                Description="Description",
                                Notes="Notes"
                            }
                        },
                        Vehicles = new List<Vehicle>()
                        {
                            new Vehicle()
                            {
                                OwnershipType="SINGLE OWNER",
                                Vin="ABCDEFGHIJK",
                                PlateNumber="AAA.BBB",
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Account Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Account Expired Date", Value=new DateTime(2020,9,1) }
                                },
                                Description="2020 BRAND TRUCK $DR / CAR AWD",
                                Notes="vehicle Notes",
                                Owners=new List<InvolvedParty>()
                                {
                                    new  InvolvedParty()
                                    {
                                        Name = new Name {FirstName = "FirstName", LastName="LastName",OtherName = "OtherName",MiddleName="MiddleName"},
                                        Description="description",
                                        Type="lease",
                                        Notes="notes",
                                        Organization ="TRUCK COMPANY INC."
                                    }
                                }
                            }
                        },
                        OtherAssets= new List<OtherAsset>()
                        {
                            new OtherAsset()
                            {
                                TypeDescription="type description",
                                ReferenceDescription="reference description",
                                ReferenceValue="reference value",
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Expired Date", Value=new DateTime(2020,9,1) }
                                },
                                Description="other asset description",
                                Notes="other asset Notes",
                                Owners=new List<InvolvedParty>()
                                {
                                    new InvolvedParty()
                                    {
                                        Name = new Name {FirstName = "FirstName", LastName="LastName",OtherName = "OtherName",MiddleName="MiddleName"},
                                        Description="other description",
                                        Type="other type",
                                        Notes="notes",
                                        Organization ="owner other org name"
                                    }
                                }
                            }
                        },
                        CompensationClaims = new List<CompensationClaim>(){
                            new CompensationClaim()
                            {
                                ClaimNumber="claimNumber",
                                ClaimantNumber="claimant121",
                                ClaimStatus="Processing",
                                ClaimType="Disable compensation",
                                Description="dis",
                                Notes="compensation notes",
                                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="compensation Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="compensation Expired Date", Value=new DateTime(2020,9,1) }
                                },
                                BankInfo=new BankInfo()
                                {
                                    BankName="compensation BankName",
                                    AccountNumber="compensation AccountNumber123",
                                    Branch="Branch-compensation",
                                    TransitNumber="TransitNumber123-compensation",
                                    ReferenceDates = new List<ReferenceDate>(){
                                        new ReferenceDate(){ Index=0, Key="Account Start Date", Value=new DateTime(2019,9,1) },
                                        new ReferenceDate(){ Index=1, Key="Account Expired Date", Value=new DateTime(2020,9,1) }
                                    },
                                    Description="compensation bank Description",
                                    Notes="compensation bank Notes"
                                },

                                Employer=new Employer()
                                {
                                     Address = new Address
                                     {
                                         AddressLine1 = "compensation Employer Address 1",
                                         AddressLine2 = "compensation Employer Address 2",
                                         AddressLine3 = "compensation Employer Address 3",
                                         City = "compensation Employer City",
                                         StateProvince = "AB",
                                         CountryRegion = "Canada",
                                         ZipPostalCode = "VR4 123"
                                     },
                                     ContactPerson = "compensation Employer Surname FirstName",
                                     Name = "compensation Employer Sample Company",
                                     OwnerName = "compensation Employer Sample Company Owner",
                                     Phones = new List<Phone>()
                                     {
                                         new Phone {PhoneNumber = "11111111", Extension ="123", Type ="Phone"},
                                         new Phone {PhoneNumber = "22222222", Extension ="123", Type ="Fax"},
                                         new Phone {PhoneNumber = "33333333", Extension ="123", Type ="Phone"}
                                     }
                                }

                            }
                        }
                    }
                }
            };

        }

    }

    public class PersonSearchRejectedEvent : PersonSearchRejected
    {

        private readonly List<DefaultValidationResult> _validationResults = new List<DefaultValidationResult>();

        public PersonSearchRejectedEvent(Guid searchRequestId, string SearchRequestKey, ProviderProfile providerProfile)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            this.SearchRequestKey = SearchRequestKey;
        }

        public void AddValidationResult(DefaultValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }

        public Guid SearchRequestId { get; }
        public string SearchRequestKey { get; }

        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public IEnumerable<ValidationResult> Reasons => _validationResults;
    }

}