using AutoMapper;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.PersonSearch
{
    public class SearchResultServiceTest
    {
        private SearchResultService _sut;
        private Mock<ILogger<SearchResultService>> _loggerMock;
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ISearchRequestService> _searchRequestServiceMock;
        private Person _fakePerson;

        private IdentifierEntity _fakePersoneIdentifier;
        private SSG_Identifier _fakeSourceIdentifier;
        private AddressEntity _fakePersonAddress;
        private PhoneNumberEntity _fakePersonPhoneNumber;
        private AliasEntity _fakeName;
        private RelatedPersonEntity _fakeRelatedPerson;
        private EmploymentEntity _fakeEmployment;
        private EmploymentEntity _fakeCompensationEmployment;
        private EmploymentContactEntity _fakeEmploymentContact;
        private BankingInformationEntity _fakeBankInfo;
        private PersonEntity _ssg_fakePerson;
        private AssetOwnerEntity _fakeAssetOwner;
        private VehicleEntity _fakeVehicleEntity;
        private AssetOtherEntity _fakeOtherAsset;
        private CompensationClaimEntity _fakeWorkSafeBcClaim;
        private ICBCClaimEntity _fakeIcbcClaim;
        private ProviderProfile _providerProfile;
        private SSG_SearchRequest _searchRequest;
        private SSG_InvolvedParty _fakeInvolvedParty;
        private SSG_SimplePhoneNumber _fakeSimplePhone;
        private CancellationToken _fakeToken;
        private string COMPENSATION_BUISNESS_NAME = "businessName";

        private Mock<IMapper> _mapper;

        [SetUp]
        public void Init()
        {

            _loggerMock = new Mock<ILogger<SearchResultService>>();
            _searchRequestServiceMock = new Mock<ISearchRequestService>();
            _mapper = new Mock<IMapper>();
            var validRequestId = Guid.NewGuid();

            var validVehicleId = Guid.NewGuid();
            var validOtherAssetId = Guid.NewGuid();
            var validBankInformationId = Guid.NewGuid();
            var validEmploymentId = Guid.NewGuid();

            _fakePersoneIdentifier = new IdentifierEntity
            {
                Identification="1234567",
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakeSourceIdentifier = new SSG_Identifier
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakePersonAddress = new AddressEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                },
                AddressLine1="addressLine1"
            };
            _fakeEmployment = new EmploymentEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakeEmploymentContact = new EmploymentContactEntity
            {
                PhoneNumber = "11111111"
            };

            _fakePersonPhoneNumber = new PhoneNumberEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakeRelatedPerson = new RelatedPersonEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakeName = new AliasEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _ssg_fakePerson = new PersonEntity
            {
            };

            _searchRequest = new SSG_SearchRequest
            {
                SearchRequestId = validRequestId
            };

            _fakeBankInfo = new BankingInformationEntity
            {
                BankName = "bank"
            };

            _fakeVehicleEntity = new VehicleEntity
            {
                SearchRequest = _searchRequest,
                PlateNumber = "AAA.BBB"
            };

            _fakeAssetOwner = new SSG_AssetOwner()
            {
                OrganizationName = "Ford Inc."
            };

            _fakeOtherAsset = new AssetOtherEntity()
            {
                SearchRequest = _searchRequest,
                TypeDescription = "type description"
            };

            _fakeWorkSafeBcClaim = new CompensationClaimEntity()
            {
                SearchRequest = _searchRequest,
                ClaimNumber = "claimNumber",
                BankingInformation = new SSG_Asset_BankingInformation()
                {
                    BankingInformationId = validBankInformationId
                },
                Employment = new SSG_Employment()
                {
                    EmploymentId = validEmploymentId
                }

            };

            _fakeCompensationEmployment = new EmploymentEntity()
            {
                BusinessName = COMPENSATION_BUISNESS_NAME
            };

            _fakeIcbcClaim = new ICBCClaimEntity()
            {
                ClaimNumber = "icbcClaimNumber"
            };

            _fakeInvolvedParty = new SSG_InvolvedParty()
            {
                OrganizationName = "name"
            };

            _fakeSimplePhone = new SSG_SimplePhoneNumber()
            {
                PhoneNumber = "0"
            };

            _fakePerson = new Person()
            {
                DateOfBirth = DateTime.Now,
                FirstName = "TEST1",
                LastName = "TEST2",
                Identifiers = new List<PersonalIdentifier>()
                {
                    new PersonalIdentifier()
                    {
                        Value = "test",
                        IssuedBy = "test",
                        Type = PersonalIdentifierType.BCDriverLicense
                    }
                },
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AddressLine1 = "AddressLine1",
                        AddressLine2 = "AddressLine2",
                        AddressLine3 = "AddressLine3",
                        StateProvince = "Manitoba",
                        City = "testCity",
                        Type = "residence",
                        CountryRegion = "canada",
                        ZipPostalCode = "p3p3p3",
                        ReferenceDates = new List<ReferenceDate>() {
                            new ReferenceDate() { Index = 0, Key = "Start Date", Value = new DateTime(2019, 9, 1) },
                            new ReferenceDate() { Index = 1, Key = "End Date", Value = new DateTime(2020, 9, 1) }
                        },
                        Description = "description"
                    }
                },
                Phones = new List<Phone>()
                {
                    new Phone()
                    {
                        PhoneNumber = "4005678900"
                    }
                },
                Names = new List<Name>()
                {
                    new Name()
                    {
                        FirstName = "firstName"
                    }
                },
                RelatedPersons = new List<RelatedPerson>()
                {
                    new RelatedPerson() { FirstName = "firstName" }
                },

                Employments = new List<Employment>()
                {
                    new Employment()
                    {
                        Occupation = "Occupation",
                        Employer = new Employer()
                        {
                            Phones = new List<Phone>() {
                                new Phone() { PhoneNumber = "1111111", Type = "Phone" }
                            }
                        }
                    }
                },

                BankInfos = new List<BankInfo>()
                {
                    new BankInfo()
                    {
                        BankName = "BankName",
                    }
                },
                Vehicles = new List<Vehicle>() {
                    new Vehicle()
                    {

                    },
                    new Vehicle()
                    {
                        Owners=new List<InvolvedParty>()
                        {
                            new InvolvedParty(){}
                        }
                    }
                },
                OtherAssets = new List<OtherAsset>()
                {
                    new OtherAsset()
                    {
                        Owners=new List<InvolvedParty>()
                        {
                            new InvolvedParty(){}
                        }
                    }
                },
                CompensationClaims = new List<CompensationClaim>()
                {
                    new CompensationClaim()
                    {
                        ClaimNumber="claimNumber",
                        BankInfo = new BankInfo()
                        {
                            BankName="compensationBankName"
                        },
                        Employer = new Employer()
                        {
                            Name=COMPENSATION_BUISNESS_NAME
                        },
                        ReferenceDates=new ReferenceDate[]
                        {
                            new ReferenceDate() { Index = 0, Key = "Start Date", Value = new DateTime(2019, 9, 1) }
                        }
                    }
                },
                InsuranceClaims = new List<InsuranceClaim>()
                {
                    new InsuranceClaim(){
                        ClaimNumber = "icbcClaimNumber",
                        InsuredParties=new List<InvolvedParty>()
                        {
                            new InvolvedParty(){Organization="insuranceClaimOrg"}
                        },
                        ClaimCentre=new ClaimCentre()
                        {
                            ContactNumber=new List<Phone>()
                            {
                                new Phone(){PhoneNumber="9999"}
                            }
                        }
                    }
                }

            };

            _providerProfile = new ProviderProfile()
            {
                Name = "Other"
            };


            _fakeToken = new CancellationToken();

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                               .Returns(_fakePersoneIdentifier);

            _mapper.Setup(m => m.Map<PhoneNumberEntity>(It.IsAny<Phone>()))
                             .Returns(_fakePersonPhoneNumber);

            _mapper.Setup(m => m.Map<AddressEntity>(It.IsAny<Address>()))
                              .Returns(_fakePersonAddress);

            _mapper.Setup(m => m.Map<AliasEntity>(It.IsAny<Name>()))
                  .Returns(_fakeName);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(_ssg_fakePerson);

            _mapper.Setup(m => m.Map<EmploymentEntity>(It.Is<Employer>(m => m.Name == COMPENSATION_BUISNESS_NAME)))
                .Returns(_fakeCompensationEmployment);

            _mapper.Setup(m => m.Map<EmploymentEntity>(It.IsAny<Employment>()))
              .Returns(_fakeEmployment);

            _mapper.Setup(m => m.Map<EmploymentContactEntity>(It.IsAny<Phone>()))
                .Returns(_fakeEmploymentContact);

            _mapper.Setup(m => m.Map<RelatedPersonEntity>(It.IsAny<RelatedPerson>()))
                   .Returns(_fakeRelatedPerson);

            _mapper.Setup(m => m.Map<BankingInformationEntity>(It.IsAny<BankInfo>()))
                   .Returns(_fakeBankInfo);

            _mapper.Setup(m => m.Map<VehicleEntity>(It.IsAny<Vehicle>()))
                    .Returns(_fakeVehicleEntity);

            _mapper.Setup(m => m.Map<AssetOwnerEntity>(It.IsAny<InvolvedParty>()))
                    .Returns(_fakeAssetOwner);

            _mapper.Setup(m => m.Map<AssetOtherEntity>(It.IsAny<OtherAsset>()))
                    .Returns(_fakeOtherAsset);

            _mapper.Setup(m => m.Map<CompensationClaimEntity>(It.IsAny<CompensationClaim>()))
                    .Returns(_fakeWorkSafeBcClaim);

            _mapper.Setup(m => m.Map<ICBCClaimEntity>(It.IsAny<InsuranceClaim>()))
                    .Returns(_fakeIcbcClaim);

            _mapper.Setup(m => m.Map<InvolvedPartyEntity>(It.Is<InvolvedParty>(m => m.Organization == "insuranceClaimOrg")))
                    .Returns(_fakeInvolvedParty);

            _mapper.Setup(m => m.Map<SimplePhoneNumberEntity>(It.Is<Phone>(m => m.PhoneNumber == "9999")))
                   .Returns(_fakeSimplePhone);

            _searchRequestServiceMock.Setup(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Identifier>(new SSG_Identifier()
                {                    
                }));

            _searchRequestServiceMock.Setup(x => x.CreateAddress(It.Is<AddressEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Address>(new SSG_Address()
                {
                    AddressLine1 = "test full line"
                }));

            _searchRequestServiceMock.Setup(x => x.CreatePhoneNumber(It.Is<PhoneNumberEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_PhoneNumber>(new SSG_PhoneNumber()
              {
                  TelePhoneNumber = "4007678231"
              }));

            _searchRequestServiceMock.Setup(x => x.CreateName(It.Is<AliasEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Aliase>(new SSG_Aliase()
                {
                    FirstName = "firstName"
                }));

            _searchRequestServiceMock.Setup(x => x.SavePerson(It.Is<PersonEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
            {
                FirstName = "First"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateEmployment(It.Is<EmploymentEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
            {
                EmploymentId = Guid.NewGuid(),
                Occupation = "Occupation"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_EmploymentContact>(new SSG_EmploymentContact()
            {
                PhoneNumber = "4007678231"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateRelatedPerson(It.Is<RelatedPersonEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_Identity>(new SSG_Identity()
              {
                  FirstName = "firstName"
              }));

            _searchRequestServiceMock.Setup(x => x.CreateBankInfo(It.Is<BankingInformationEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Asset_BankingInformation>(new SSG_Asset_BankingInformation()
                {
                    BankingInformationId = validBankInformationId,
                    BankName = "bankName"
                }));

            _searchRequestServiceMock.Setup(x => x.CreateVehicle(It.Is<VehicleEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Asset_Vehicle>(new SSG_Asset_Vehicle()
                {
                    VehicleId = validVehicleId
                }));

            _searchRequestServiceMock.Setup(x => x.CreateAssetOwner(It.Is<SSG_AssetOwner>(x => x.Vehicle.VehicleId == validVehicleId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_AssetOwner>(new SSG_AssetOwner()
                {
                    Type = "Owner"
                }));

            _searchRequestServiceMock.Setup(x => x.CreateOtherAsset(It.Is<AssetOtherEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Asset_Other>(new SSG_Asset_Other()
                {
                    AssetOtherId = validOtherAssetId
                }));

            _searchRequestServiceMock.Setup(x => x.CreateCompensationClaim(It.Is<CompensationClaimEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Asset_WorkSafeBcClaim>(new SSG_Asset_WorkSafeBcClaim()
                {
                    ClaimNumber = "claimNumber"
                }));

            _searchRequestServiceMock.Setup(x => x.CreateEmployment(It.Is<EmploymentEntity>(x => x.BusinessName == COMPENSATION_BUISNESS_NAME && x.Date1 == new DateTime(2019, 9, 1)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
                {
                    EmploymentId = Guid.NewGuid(),

                }));

            _searchRequestServiceMock.Setup(x => x.CreateInsuranceClaim(It.Is<ICBCClaimEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Asset_ICBCClaim>(new SSG_Asset_ICBCClaim()
                {
                    ICBCClaimId = Guid.NewGuid()
                }));

            _searchRequestServiceMock.Setup(x => x.CreateInvolvedParty(It.IsAny<SSG_InvolvedParty>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_InvolvedParty>(new SSG_InvolvedParty()
                { }));

            _searchRequestServiceMock.Setup(x => x.CreateSimplePhoneNumber(It.IsAny<SSG_SimplePhoneNumber>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SimplePhoneNumber>(new SSG_SimplePhoneNumber()
                { }));

            _searchRequestServiceMock.Setup(x => x.CreateTransaction(It.IsAny<SSG_SearchRequestResultTransaction>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_SearchRequestResultTransaction>(new SSG_SearchRequestResultTransaction()
            {                
            }));

            _sut = new SearchResultService(_searchRequestServiceMock.Object, _loggerFactoryMock.Object, _mapper.Object);

        }

        [Test]
        public async Task valid_Person_with_sourceIdentifier_should_be_processed_correctly()
        {

            var result = await _sut.ProcessPersonFound(_fakePerson, _providerProfile, _searchRequest,Guid.NewGuid(), _fakeToken, _fakeSourceIdentifier);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                 .Verify(x => x.CreateAddress(It.IsAny<AddressEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateName(It.IsAny<AliasEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
               .Verify(x => x.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
              .Verify(x => x.CreateEmployment(It.IsAny<EmploymentEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
               .Verify(x => x.CreateBankInfo(It.IsAny<BankingInformationEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
              .Verify(x => x.CreateVehicle(It.IsAny<VehicleEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            _searchRequestServiceMock
              .Verify(x => x.CreateOtherAsset(It.IsAny<AssetOtherEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateAssetOwner(It.IsAny<SSG_AssetOwner>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            _searchRequestServiceMock
               .Verify(x => x.CreateCompensationClaim(It.IsAny<CompensationClaimEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateInsuranceClaim(It.IsAny<ICBCClaimEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateInvolvedParty(It.IsAny<SSG_InvolvedParty>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateSimplePhoneNumber(It.IsAny<SSG_SimplePhoneNumber>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateTransaction(It.IsAny<SSG_SearchRequestResultTransaction>(), It.IsAny<CancellationToken>()), Times.Exactly(13));

            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task valid_Person_with_null_sourceIdentifier_should_be_processed_correctly()
        {

            var result = await _sut.ProcessPersonFound(_fakePerson, _providerProfile, _searchRequest, Guid.NewGuid(), _fakeToken, null);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                 .Verify(x => x.CreateAddress(It.IsAny<AddressEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateName(It.IsAny<AliasEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
               .Verify(x => x.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
              .Verify(x => x.CreateEmployment(It.IsAny<EmploymentEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
               .Verify(x => x.CreateBankInfo(It.IsAny<BankingInformationEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
              .Verify(x => x.CreateVehicle(It.IsAny<VehicleEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            _searchRequestServiceMock
              .Verify(x => x.CreateOtherAsset(It.IsAny<AssetOtherEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateAssetOwner(It.IsAny<SSG_AssetOwner>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

            _searchRequestServiceMock
               .Verify(x => x.CreateCompensationClaim(It.IsAny<CompensationClaimEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateInsuranceClaim(It.IsAny<ICBCClaimEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateInvolvedParty(It.IsAny<SSG_InvolvedParty>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateSimplePhoneNumber(It.IsAny<SSG_SimplePhoneNumber>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            _searchRequestServiceMock
                .Verify(x => x.CreateTransaction(It.IsAny<SSG_SearchRequestResultTransaction>(), It.IsAny<CancellationToken>()), Times.Exactly(0));

            Assert.AreEqual(true, result);
        }
        [Test]
        public async Task null_Person_should_return_true_correctly()
        {

            var result = await _sut.ProcessPersonFound(null, _providerProfile, _searchRequest, Guid.NewGuid(),_fakeToken);
  
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task valid_Person_with_null_properties_should_be_processed_correctly()
        {
            Person fakePersonNull = new Person()
            {
                DateOfBirth = DateTime.Now,
                FirstName = "TEST1",
                LastName = "TEST2",
                Identifiers = null,
                Addresses = null,
                Phones = null,
                Names = null,
                RelatedPersons = null,
                Employments = null,
                BankInfos = null,
                Vehicles = null,
                OtherAssets = null,
                CompensationClaims = null,
                InsuranceClaims = null
            };
            var result = await _sut.ProcessPersonFound(fakePersonNull, _providerProfile, _searchRequest,Guid.NewGuid(), _fakeToken, _fakeSourceIdentifier);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                 .Verify(x => x.CreateAddress(It.IsAny<AddressEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateName(It.IsAny<AliasEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
               .Verify(x => x.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
              .Verify(x => x.CreateEmployment(It.IsAny<EmploymentEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
               .Verify(x => x.CreateBankInfo(It.IsAny<SSG_Asset_BankingInformation>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
              .Verify(x => x.CreateVehicle(It.IsAny<VehicleEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateAssetOwner(It.IsAny<SSG_AssetOwner>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateOtherAsset(It.IsAny<AssetOtherEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateCompensationClaim(It.IsAny<CompensationClaimEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateInsuranceClaim(It.IsAny<ICBCClaimEntity>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateInvolvedParty(It.IsAny<SSG_InvolvedParty>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                .Verify(x => x.CreateSimplePhoneNumber(It.IsAny<SSG_SimplePhoneNumber>(), It.IsAny<CancellationToken>()), Times.Never);

            _searchRequestServiceMock
                 .Verify(x => x.CreateTransaction(It.IsAny<SSG_SearchRequestResultTransaction>(), It.IsAny<CancellationToken>()), Times.Exactly(1));


            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task exception_should_be_caught_correctly()
        {
            Guid invalidRequestId = Guid.NewGuid();
      
            SSG_SearchRequest invalidSearchRequest = new SSG_SearchRequest
            {
                SearchRequestId = invalidRequestId
            };
            Person exceptionPerson = new Person()
            {
                Identifiers = new List<PersonalIdentifier>()
                {
                    new PersonalIdentifier()
                    {
                        Value = "exceptionID"
                    }
                },
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AddressLine1 = "AddressLine1",                        
                    }
                },
                Phones = new List<Phone>()
                {
                    new Phone()
                    {
                        PhoneNumber = "4005678900"
                    }
                },
                Names = new List<Name>()
                {
                    new Name()
                    {
                        FirstName = "firstName"
                    }
                },
                RelatedPersons = new List<RelatedPerson>()
                {
                    new RelatedPerson() { FirstName = "firstName" }
                },
            };

            _searchRequestServiceMock.Setup(x => x.CreateIdentifier(It.Is<IdentifierEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                 .Throws(new Exception("identifier random exception"));
            _searchRequestServiceMock.Setup(x => x.CreateAddress(It.Is<AddressEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                .Throws(new Exception("address random exception"));
            _searchRequestServiceMock.Setup(x => x.CreatePhoneNumber(It.Is<PhoneNumberEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                .Throws(new Exception("phone random exception"));
            _searchRequestServiceMock.Setup(x => x.CreateName(It.Is<AliasEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                .Throws(new Exception("name random exception"));
            _searchRequestServiceMock.Setup(x => x.CreateRelatedPerson(It.Is<RelatedPersonEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                .Throws(new Exception("relatedPerson random exception"));

            var result = await _sut.ProcessPersonFound(exceptionPerson, _providerProfile, invalidSearchRequest, Guid.NewGuid(),_fakeToken);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, "identifier random exception", Times.Once());
            _loggerMock.VerifyLog(LogLevel.Error, "address random exception", Times.Once());
            _loggerMock.VerifyLog(LogLevel.Error, "phone random exception", Times.Once());
            _loggerMock.VerifyLog(LogLevel.Error, "name random exception", Times.Once());
            _loggerMock.VerifyLog(LogLevel.Error, "relatedPerson random exception", Times.Once());
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task exception_compensation_should_be_caught_correctly()
        {
            Guid invalidRequestId = Guid.NewGuid();

            SSG_SearchRequest invalidSearchRequest = new SSG_SearchRequest
            {
                SearchRequestId = invalidRequestId
            };
            Person exceptionPerson = new Person()
            {
                CompensationClaims = new List<CompensationClaim>()
                {
                    new CompensationClaim()
                    {
                        ClaimNumber = "claimNumber"
                    }
                }
            };

            _searchRequestServiceMock.Setup(x => x.CreateCompensationClaim(It.Is<CompensationClaimEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                 .Throws(new Exception("compensation random exception"));
    
            var result = await _sut.ProcessPersonFound(exceptionPerson, _providerProfile, invalidSearchRequest, Guid.NewGuid(), _fakeToken);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateCompensationClaim(It.IsAny<CompensationClaimEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, "compensation random exception", Times.Once());
 
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task exception_insurance_should_be_caught_correctly()
        {
            Guid invalidRequestId = Guid.NewGuid();

            SSG_SearchRequest invalidSearchRequest = new SSG_SearchRequest
            {
                SearchRequestId = invalidRequestId
            };
            Person exceptionPerson = new Person()
            {
                InsuranceClaims = new List<InsuranceClaim>()
                {
                    new InsuranceClaim()
                    {
                        ClaimNumber = "claimNumber"
                    }
                }
            };

            _searchRequestServiceMock.Setup(x => x.CreateInsuranceClaim(It.Is<ICBCClaimEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                 .Throws(new Exception("insurance random exception"));

            var result = await _sut.ProcessPersonFound(exceptionPerson, _providerProfile, invalidSearchRequest, Guid.NewGuid(), _fakeToken);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateInsuranceClaim(It.IsAny<ICBCClaimEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, "insurance random exception", Times.Once());

            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task exception_simplePhoneNumber_should_be_caught_correctly()
        {
            Guid invalidRequestId = Guid.NewGuid();
            Guid invalidClaimId = Guid.NewGuid();

            SSG_SearchRequest invalidSearchRequest = new SSG_SearchRequest
            {
                SearchRequestId = invalidRequestId
            };
            Person exceptionPerson = new Person()
            {
                InsuranceClaims = new List<InsuranceClaim>()
                {
                    new InsuranceClaim()
                    {
                        ClaimNumber = "claimNumber",
                        ClaimCentre=new ClaimCentre()
                        {
                            ContactNumber=new List<Phone>()
                            {
                                new Phone(){PhoneNumber="9999"}
                            }
                        }
                    }
                }
            };

            _searchRequestServiceMock.Setup(x => x.CreateInsuranceClaim(It.Is<ICBCClaimEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_Asset_ICBCClaim>(new SSG_Asset_ICBCClaim()
                 {
                     ICBCClaimId = invalidClaimId
                 }));

            _searchRequestServiceMock.Setup(x => x.CreateSimplePhoneNumber(It.Is<SimplePhoneNumberEntity>(x => x.SSG_Asset_ICBCClaim.ICBCClaimId == invalidClaimId), It.IsAny<CancellationToken>()))
                 .Throws(new Exception("simplePhoneNumber random exception"));

            var result = await _sut.ProcessPersonFound(exceptionPerson, _providerProfile, invalidSearchRequest, Guid.NewGuid(), _fakeToken);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateInsuranceClaim(It.IsAny<ICBCClaimEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateSimplePhoneNumber(It.IsAny<SimplePhoneNumberEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, "simplePhoneNumber random exception", Times.Once());

            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task exception_involvedParty_should_be_caught_correctly()
        {
            Guid invalidRequestId = Guid.NewGuid();
            Guid invalidClaimId = Guid.NewGuid();

            SSG_SearchRequest invalidSearchRequest = new SSG_SearchRequest
            {
                SearchRequestId = invalidRequestId
            };
            Person exceptionPerson = new Person()
            {
                InsuranceClaims = new List<InsuranceClaim>()
                {
                    new InsuranceClaim()
                    {
                        InsuredParties=new List<InvolvedParty>()
                        {
                            new InvolvedParty(){Organization="insuranceClaimOrg"}
                        }
                    }
                }
            };

            _searchRequestServiceMock.Setup(x => x.CreateInsuranceClaim(It.Is<ICBCClaimEntity>(x => x.SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_Asset_ICBCClaim>(new SSG_Asset_ICBCClaim()
                 {
                     ICBCClaimId = invalidClaimId
                 }));

            _searchRequestServiceMock.Setup(x => x.CreateInvolvedParty(It.Is<InvolvedPartyEntity>(x => x.SSG_Asset_ICBCClaim.ICBCClaimId == invalidClaimId), It.IsAny<CancellationToken>()))
                 .Throws(new Exception("involved party random exception"));

            var result = await _sut.ProcessPersonFound(exceptionPerson, _providerProfile, invalidSearchRequest,Guid.NewGuid(), _fakeToken);

            _searchRequestServiceMock
              .Verify(x => x.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateInsuranceClaim(It.IsAny<ICBCClaimEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _searchRequestServiceMock
                .Verify(x => x.CreateInvolvedParty(It.IsAny<InvolvedPartyEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, "involved party random exception", Times.Once());

            Assert.AreEqual(true, result);
        }
    }
  
}
