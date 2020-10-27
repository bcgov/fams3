using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Investment;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Pension;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RealEstate;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.SearchResponse;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicsAdapter.Web.Test.Mapping
{
    [System.Runtime.InteropServices.Guid("2EF2EBF7-5CD8-4C03-A254-D6281F355654")]
    public class MappingProfile_SearchResponse_Test
    {

        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void normal_SearchResponse_should_map_to_Person_correctly()
        {
            SSG_SearchRequestResponse response = new SSG_SearchRequestResponse()
            {
                SSG_BankInfos = new List<SSG_Asset_BankingInformation>
                {
                    new SSG_Asset_BankingInformation { AccountNumber = "accountNumber" }
                }.ToArray(),
                SSG_Asset_Others = new List<SSG_Asset_Other>
                {
                    new SSG_Asset_Other{Description="other Asset"}
                }.ToArray(),
                SSG_Addresses = new List<SSG_Address>
                {
                    new SSG_Address{AddressLine1="line1"}
                }.ToArray(),
                SSG_Aliases = new List<SSG_Aliase>
                {
                    new SSG_Aliase{ FirstName="aliasFirstName"}
                }.ToArray(),
                SSG_Asset_ICBCClaims = new List<SSG_Asset_ICBCClaim>
                {
                    new SSG_Asset_ICBCClaim{ClaimNumber="claimNumber"}
                }.ToArray(),
                SSG_Asset_Vehicles = new List<SSG_Asset_Vehicle>
                {
                    new SSG_Asset_Vehicle{PlateNumber="vehiclePlatNumber"}
                }.ToArray(),
                SSG_Employments = new List<SSG_Employment>
                {
                    new SSG_Employment{ BusinessName="employment"}
                }.ToArray(),
                SSG_Identifiers = new List<SSG_Identifier>
                {
                    new SSG_Identifier{ Identification="identification"}
                }.ToArray(),
                SSG_Identities = new List<SSG_Identity>
                {
                    new SSG_Identity{ FirstName="relatedPerson"}
                }.ToArray(),
                SSG_Noteses = new List<SSG_Notese>
                {
                    new SSG_Notese{ Description="notes" }
                }.ToArray(),
                SSG_Persons = new List<SSG_Person>
                {
                    new SSG_Person{ FirstName="personFirstName" }
                }.ToArray(),
                SSG_PhoneNumbers = new List<SSG_PhoneNumber>
                {
                    new SSG_PhoneNumber{ TelePhoneNumber="phoneNumber" }
                }.ToArray(),
                SSG_SearchRequests = new List<SSG_SearchRequest>
                {
                    new SSG_SearchRequest{Agency = new SSG_Agency{AgencyCode="FMEP" } }
                }.ToArray(),
                SSG_Asset_Investments = new List<SSG_Asset_Investment>
                {
                    new SSG_Asset_Investment{ AccountNumber="accountNumber" }
                }.ToArray(),
                SSG_Asset_WorkSafeBcClaims = new List<SSG_Asset_WorkSafeBcClaim>
                {
                     new SSG_Asset_WorkSafeBcClaim{ ClaimNumber="claim" }
                }.ToArray(),
                SSG_SafetyConcernDetails = new List<SSG_SafetyConcernDetail>
                {
                     new SSG_SafetyConcernDetail{ Detail="detail" }
                }.ToArray(),
                SSG_Asset_PensionDisablilitys = new List<SSG_Asset_PensionDisablility>
                {
                     new SSG_Asset_PensionDisablility{ AddressLine1="lin1" }
                }.ToArray(),
                SSG_Asset_RealEstatePropertys = new List<SSG_Asset_RealEstateProperty>
                {
                     new SSG_Asset_RealEstateProperty{ AddressLine1="lin1" }
                }.ToArray(),
            };
            Person person = _mapper.Map<Person>(response);
            Assert.AreEqual(1, person.Names.Count);
            Assert.AreEqual(1, person.Identifiers.Count);
            Assert.AreEqual(1, person.Addresses.Count);
            Assert.AreEqual(1, person.Phones.Count);
            Assert.AreEqual(1, person.Investments.Count);
            Assert.AreEqual(1, person.RelatedPersons.Count);
            Assert.AreEqual(1, person.BankInfos.Count);
            Assert.AreEqual(1, person.Employments.Count);
            Assert.AreEqual(1, person.InsuranceClaims.Count);
            Assert.AreEqual(1, person.CompensationClaims.Count);
            Assert.AreEqual(1, person.Vehicles.Count);
            Assert.AreEqual(1, person.OtherAssets.Count);
            Assert.AreEqual(1, person.SafetyConcerns.Count);
            Assert.AreEqual(1, person.Pensions.Count);
            Assert.AreEqual(1, person.RealEstateProperties.Count);
            Assert.AreEqual(1, person.ResponseNotes.Count);
            Assert.AreEqual(1, person.ResponsePersons.Count);
        }

        [Test]
        public void SSG_SearchRequest_should_map_to_Person_Agency_correctly()
        {
            SSG_SearchRequestResponse response = new SSG_SearchRequestResponse()
            {
                SSG_SearchRequests = new List<SSG_SearchRequest>
                {
                    new SSG_SearchRequest{
                        Agency = new SSG_Agency{AgencyCode="FMEP" },
                        RequestDate=new DateTime(2001,1,1),
                        PersonSoughtFirstName="firstName",
                        PersonSoughtMiddleName="middleName",
                        PersonSoughtLastName="surname",
                        PersonSoughtThirdGiveName="thirdGivenName",
                        PersonSoughtDateOfBirth=new DateTime(2001,1,1),
                        PersonSoughtGender = GenderType.Male.Value,
                        AgentFirstName="agentFirstName",
                        AgentLastName="agentLastName",
                        SearchReason = new SSG_SearchRequestReason{ ReasonCode="Other"},
                        OriginalRequestorReference="originalRef",
                        RequestPriority = RequestPriorityType.Rush.Value,
                        PersonSoughtRole = PersonSoughtType.P.Value,
                        MinsOpen = 100
                    }
                }.ToArray(),
                SSG_Persons = new List<SSG_Person>
                {
                    new SSG_Person{ FirstName="personFirstName" }
                }.ToArray(),
            };
            Person person = _mapper.Map<Person>(response);
            Assert.AreEqual("agentFirstName", person.Agency.Agent.FirstName);
            Assert.AreEqual("agentLastName", person.Agency.Agent.LastName);
            Assert.AreEqual(SearchReasonCode.Other, person.Agency.ReasonCode);
            Assert.AreEqual("originalRef", person.Agency.RequestId);
            Assert.AreEqual(RequestPriority.Rush, person.Agency.RequestPriority);
            Assert.AreEqual("firstName", person.FirstName);
            Assert.AreEqual("middleName", person.MiddleName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2001, 1, 1)), person.DateOfBirth);
            Assert.AreEqual(SoughtPersonType.PAYOR, person.Type);
            Assert.AreEqual("FMEP", person.Agency.Code);
            Assert.AreEqual(1, person.Agency.DaysOpen);
            Assert.AreEqual(1, person.ResponsePersons.Count);
        }


        [Test]
        public void SSG_Aliase_should_map_to_Person_Name_correctly()
        {
            var alias = new SSG_Aliase
            {
                FirstName = "aliasFirstName",
                Date1 = new DateTime(2000, 1, 1),
                Date1Label = "label",
                ResponseComments = "aliasComments",
                DateOfBirth = new DateTime(2004, 1, 1)
            };

            Name name = _mapper.Map<Name>(alias);
            Assert.AreEqual("aliasFirstName", name.FirstName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2004, 1, 1)), name.DateOfBirth);
        }

        [Test]
        public void SSG_Identifier_should_map_to_Person_Identifier_correctly()
        {
            var id = new SSG_Identifier
            {
                Identification = "1234455",
                IssuedBy = "ICBC",
                Date1Label = "label",
                Date1 = new DateTime(2000, 1, 1),
                ResponseComments = "identifierComments",
                IdentifierType = IdentificationType.BCDriverLicense.Value,
            };

            PersonalIdentifier personalIdentifier = _mapper.Map<PersonalIdentifier>(id);
            Assert.AreEqual("ICBC", personalIdentifier.IssuedBy);
            Assert.AreEqual("1234455", personalIdentifier.Value);
            Assert.AreEqual("identifierComments", personalIdentifier.ResponseComments);
            Assert.AreEqual(PersonalIdentifierType.BCDriverLicense, personalIdentifier.Type);
            Assert.AreEqual(1, personalIdentifier.ReferenceDates.Count);
        }

        [Test]
        public void SSG_Address_should_map_to_Person_Address_correctly()
        {
            var address = new SSG_Address
            {
                AddressLine1 = "addressline1",
                AddressLine2 = "addressline2",
                PostalCode = "postalCode",
                City = "city",
                Date1Label = "label",
                Date1 = new DateTime(2000, 1, 1),
                ResponseComments = "addressComments",
                CountryText = "canada",
                CountrySubdivisionText = "bc",
                Category = LocationType.Business.Value,
                IncarcerationStatus = "incarceration"
            };

            Address addr = _mapper.Map<Address>(address);
            Assert.AreEqual("addressline1", addr.AddressLine1);
            Assert.AreEqual("addressComments", addr.ResponseComments);
            Assert.AreEqual("city", addr.City);
            Assert.AreEqual("canada", addr.CountryRegion);
            Assert.AreEqual("bc", addr.StateProvince);
            Assert.AreEqual("business", addr.Type);
            Assert.AreEqual("incarceration", addr.IncarcerationStatus);
            Assert.AreEqual(1, addr.ReferenceDates.Count);
        }

        [Test]
        public void SSG_Address_couldnotlocate_map_to_Person_Address_correctly()
        {
            var address = new SSG_Address
            {
                CouldNotLocate = true,
                Category = LocationType.Business.Value,
                IncarcerationStatus = "incarceration"
            };

            Address addr = _mapper.Map<Address>(address);
            Assert.AreEqual("Could Not Locate", addr.AddressLine1);
        }

        [Test]
        public void SSG_PhoneNumber_should_map_to_Person_Phone_correctly()
        {
            SSG_PhoneNumber phoneNumber = new SSG_PhoneNumber
            {
                TelePhoneNumber = "123456",
                PhoneExtension = "123",
                TelephoneNumberType = TelephoneNumberType.Home.Value,
                ResponseComments = "responseComments",
                PhoneNumber="123456"
            };

            Phone phone = _mapper.Map<Phone>(phoneNumber);

            Assert.AreEqual("123456", phone.PhoneNumber);
            Assert.AreEqual("123", phone.Extension);
            Assert.AreEqual("home", phone.Type);
            Assert.AreEqual("responseComments", phone.ResponseComments);
        }

        [Test]
        public void SSG_Employment_should_map_to_Employment_correctly()
        {
            SSG_Employment employment = new SSG_Employment
            {
                EmploymentType = EmploymentRecordType.IncomeAssistance.Value,
                BusinessName = "legalBizName",
                DBAName = "dbaName",
                PrimaryPhoneNumber = "primaryPhone",
                PrimaryPhoneExtension = "primaryExt",
                PrimaryFax = "primaryFax",
                Website = "www.website.com",
                AddressLine1 = "employmentAddress1",
                AddressLine2 = "employmentAddress2",
                City = "employmentCity",
                PostalCode = "postalCode",
                CountryText = "country",
                CountrySubdivisionText = "province",
                EmploymentStatus = EmploymentStatusType.NotEmployed.Value,
                SelfEmployComRegistrationNo = "selfComNo",
                SelfEmployComType = SelfEmploymentCompanyType.ExtraprovincialNonShareCorp.Value,
                Occupation = "occupation",
                SelfEmployComRole = SelfEmploymentCompanyRoleType.Officer.Value,
                SelfEmployPercentOfShare = 50,
                IncomeAssistanceStatusOption = IncomeAssistanceStatusType.Closed.Value,
                IncomeAssistanceDesc = "income assistance description",
                ContactPerson = "contact person",
                PrimaryContactPhone = "primaryContactPhoneNumber",
                PrimaryContactPhoneExt = "primaryContactExt",
                SSG_EmploymentContacts = new List<SSG_EmploymentContact>
                {
                    new SSG_EmploymentContact
                    {
                        ContactName="contactName",
                        Description="description",
                        PhoneNumber="phone",
                        PhoneExtension="ext",
                        FaxNumber="faxNumber",
                        PhoneType=TelephoneNumberType.Cell.Value
                    }
                }.ToArray()
            };

            Employment e = _mapper.Map<Employment>(employment);

            Assert.AreEqual(true, e.IncomeAssistance);
            Assert.AreEqual("legalBizName", e.Employer.Name);
            Assert.AreEqual("dbaName", e.Employer.DbaName);
            Assert.AreEqual("employmentAddress1", e.Employer.Address.AddressLine1);
            Assert.AreEqual("Not Employed", e.EmploymentStatus);
            Assert.AreEqual("Extraprovincial Non-Share Corporation", e.SelfEmployComType);
            Assert.AreEqual(5, e.Employer.Phones.Count);
        }

        [Test]
        public void SSG_Employment_couldnotlocate_should_map_to_Employment_correctly()
        {
            SSG_Employment employment = new SSG_Employment
            {
                CouldNotLocate = true
            };

            Employment e = _mapper.Map<Employment>(employment);

            Assert.AreEqual("Could Not Locate", e.Employer.Name);

        }

        [Test]
        public void SSG_Vehicle_should_map_to_Vehicle_correctly()
        {
            SSG_Asset_Vehicle v = new SSG_Asset_Vehicle
            {
                Year = "2016",
                Make = "make",
                Model = "model",
                Color = "color",
                PlateNumber = "plateNumber",
                Vin = "vin",
                Type = "v type",
                LeasingCom = "leasingCom",
                Lessee = "lessee",
                LeasingComAddr = "leasingComAddr"
            };

            Vehicle vehicle = _mapper.Map<Vehicle>(v);

            Assert.AreEqual("make", vehicle.Make);
            Assert.AreEqual(1, vehicle.Owners.Count);
        }

        [Test]
        public void SSG_ICBCClaim_should_map_to_InsuranceClaim_correctly()
        {
            SSG_Asset_ICBCClaim icbcClaim = new SSG_Asset_ICBCClaim
            {
                ClaimNumber = "claimNumber",
                ClaimType = "ClaimType",
                ClaimAmount = "ClaimAmount",
                ClaimCenterLocationCode = "claimCentre",
                AdjusterFirstName = "adjustfirstName",
                AdjusterLastName = "adjustlastName",
                AdjusterPhoneNumber = "phonenumber"
            };

            InsuranceClaim claim = _mapper.Map<InsuranceClaim>(icbcClaim);

            Assert.AreEqual("adjustfirstName", claim.Adjustor.FirstName);
            Assert.AreEqual("ClaimAmount", claim.ClaimAmount);
            Assert.AreEqual("claimCentre", claim.ClaimCentre.Location);
            Assert.AreEqual("phonenumber", claim.AdjustorPhone.PhoneNumber);
        }

        [Test]
        public void SSG_WorkSafeBCClaim_should_map_to_CompensationClaim_correctly()
        {
            SSG_Asset_WorkSafeBcClaim claim = new SSG_Asset_WorkSafeBcClaim
            {
                ClaimNumber = "claimNumber",
                ClaimAmount = "ClaimAmount",
                ResponseComments = "response"
            };

            CompensationClaim comClaim = _mapper.Map<CompensationClaim>(claim);

            Assert.AreEqual("claimNumber", comClaim.ClaimNumber);
            Assert.AreEqual("ClaimAmount", comClaim.ClaimAmount);
            Assert.AreEqual("response", comClaim.ResponseComments);
        }

        [Test]
        public void SSG_BankInfo_should_map_to_BankInfo_correctly()
        {
            SSG_Asset_BankingInformation bank = new SSG_Asset_BankingInformation
            {
                AccountNumber = "AccountNumber",
                AccountType = BankAccountType.Joint.Value,
                Branch = "branch",
                BankName = "institutionName",
                BranchNumber = "branchNumer",
                TransitNumber = "TransitNumber"
            };

            BankInfo bankInfo = _mapper.Map<BankInfo>(bank);

            Assert.AreEqual("Joint", bankInfo.AccountType);
            Assert.AreEqual("branchNumer", bankInfo.BranchNumber);
        }

        [Test]
        public void SSG_Identity_should_map_to_RelatedPerson_correctly()
        {
            SSG_Identity relatedPerson = new SSG_Identity
            {
                PersonType = RelatedPersonPersonType.Relation.Value,
                Type = PersonRelationType.Friend.Value,
                FirstName = "firstName",
                MiddleName = "middleNm",
                LastName = "lastNm",
                ThirdGivenName = "middleNm2",
                Gender = GenderType.Female.Value,
                DateOfBirth = new DateTime(2015, 1, 1),
                DateOfDeath = new DateTime(2020, 10, 1)
            };

            RelatedPerson person = _mapper.Map<RelatedPerson>(relatedPerson);

            Assert.AreEqual("firstName", person.FirstName);
            Assert.AreEqual("f", person.Gender);
            Assert.AreEqual("Relation", person.PersonType);
            Assert.AreEqual("Friend", person.Type);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2015, 1, 1)), person.DateOfBirth);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2020, 10, 1)), person.DateOfDeath);
        }

        [Test]
        public void SSG_Notese_should_map_to_RepsonseNote_correctly()
        {
            SSG_Notese notese = new SSG_Notese
            {
                Description = "note"
            };

            ResponseNote n = _mapper.Map<ResponseNote>(notese);

            Assert.AreEqual("note", n.Description);
        }

        [Test]
        public void SSG_Asset_Investment_should_map_to_Investment_correctly()
        {
            SSG_Asset_Investment invest = new SSG_Asset_Investment
            {
                AccountNumber = "accountNumber",
                Company = "company",
                BalanceAmount = "1234",
                Location = "location",
                MaturityDate = new DateTime(2019, 1, 1),
                Type = "type"
            };

            Investment i = _mapper.Map<Investment>(invest);

            Assert.AreEqual(new DateTimeOffset(new DateTime(2019, 1, 1)), i.MaturityDate);
        }

        [Test]
        public void SSG_Other_asset_should_map_to_OtherAsset_correctly()
        {
            SSG_Asset_Other other = new SSG_Asset_Other
            {
                AssetDescription = "assetDesc",
                TypeDescription = "typeDesc"
            };

            OtherAsset asset = _mapper.Map<OtherAsset>(other);

            Assert.AreEqual("assetDesc", asset.Description);
            Assert.AreEqual("typeDesc", asset.TypeDescription);
        }

        [Test]
        public void SSG_SafetyConcernDetail_should_map_to_SafetyConcern_correctly()
        {
            SSG_SafetyConcernDetail safe = new SSG_SafetyConcernDetail
            {
                Detail = "detail",
                Type = SafetyConcernType.Suicidal.Value
            };

            SafetyConcern concern = _mapper.Map<SafetyConcern>(safe);

            Assert.AreEqual("detail", concern.Description);
            Assert.AreEqual("Suicidal", concern.Type);
        }

        [Test]
        public void SSG_PensionDisability_should_map_to_Persion_correctly()
        {
            SSG_Asset_PensionDisablility pension = new SSG_Asset_PensionDisablility
            {
                Provider = "provider",
                BalanceAmount_base = 2234,
                BalanceAmount = -123.09,
                //Currency =
                ExchangeRate = 1.3,
                ProviderPhone = "12233",
                AddressLine1 = "line1",
                City = "city"
            };

            Pension p = _mapper.Map<Pension>(pension);
            Assert.AreEqual("provider", p.Provider);
            Assert.AreEqual("2234", p.BalanceAmount_base);
            Assert.AreEqual("-123.09", p.BalanceAmount);
            Assert.AreEqual("1.3", p.ExchangeRate);
            Assert.AreEqual("12233", p.ProviderPhone);
            Assert.AreEqual("line1", p.ProviderAddress.AddressLine1);
            Assert.AreEqual("city", p.ProviderAddress.City);
        }

        [Test]
        public void SSG_RealEstateProperty_should_map_to_RealEstateProperty_correctly()
        {
            SSG_Asset_RealEstateProperty property = new SSG_Asset_RealEstateProperty
            {
                PID = "pdi",
                TitleNumber = "title123",
                AddressLine1 = "line1",
                City = "city"
            };

            RealEstateProperty p = _mapper.Map<RealEstateProperty>(property);
            Assert.AreEqual("pdi", p.Pid);
            Assert.AreEqual("title123", p.TitleNumber);
            Assert.AreEqual("line1", p.PropertyAddress.AddressLine1);
            Assert.AreEqual("city", p.PropertyAddress.City);
        }


        [Test]
        public void SSG_Person_should_map_to_ResponsePerson_correctly()
        {
            SSG_Person person = new SSG_Person
            {
                DateOfDeath = new DateTime(2015, 1, 1),
                GenderOptionSet = GenderType.Other.Value,
                FirstName = "firstName"
            };

            ResponsePerson rp = _mapper.Map<ResponsePerson>(person);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2015, 1, 1)), rp.DateOfDeath);
            Assert.AreEqual("u", rp.Gender);
            Assert.AreEqual("firstName", rp.FirstName);
        }
    }
}
