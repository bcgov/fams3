using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.Duplicate
{
    public class DuplicateDetectionServiceTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

        private DuplicateDetectionService _sut;

        [SetUp]
        public void SetUp()
        {

            odataClientMock.Setup(x => x.For<SSG_DuplicateDetectionConfig>(null)                
                .FindEntriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_DuplicateDetectionConfig>>(new List<SSG_DuplicateDetectionConfig>()
                {
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_person",
                         DuplicateFields = "ssg_firstname|ssg_lastname|ssg_dateofbirth"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_address",
                         DuplicateFields = "ssg_address|ssg_addresssecondaryunittext|ssg_locationcityname"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_identifier",
                         DuplicateFields = "ssg_identification|ssg_identificationcategorytext"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_phonenumber",
                         DuplicateFields = "ssg_telephonenumber|ssg_phoneextension"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_alias",
                         DuplicateFields = "ssg_PersonSurName|ssg_persongivenname"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_asset_vehicle",
                         DuplicateFields = "ssg_vin|ssg_licenseplate"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_assetowner",
                         DuplicateFields = "ssg_lastname"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_identity",
                         DuplicateFields = "ssg_persongivenname"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_employment",
                         DuplicateFields = "ssg_websiteurl|ssg_address"
                     },
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "ssg_employmentcontact",
                         DuplicateFields = "ssg_phonenumber"
                     }
                }));

            _sut = new DuplicateDetectionService(odataClientMock.Object);

        }


        [Test]
        public async Task same_person_GetDuplicateDetectHashData_should_return_same_string()
        {
            DuplicateDetectionService._configs = null;
            PersonEntity person1 = new PersonEntity()
            {
                FirstName = "test",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1)
            };
            string str1 = await _sut.GetDuplicateDetectHashData(person1);
            PersonEntity person2 = new PersonEntity()
            {
                FirstName = "test",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1)
            };
            
            string str2 = await _sut.GetDuplicateDetectHashData(person2);
            Assert.AreEqual(true,str1==str2);
        }

        [Test]
        public async Task different_person_GetDuplicateDetectHashData_should_return_different_string()
        {
            DuplicateDetectionService._configs = null;
            PersonEntity person1 = new PersonEntity()
            {
                FirstName = "test1",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1)
            };
            string str1 = await _sut.GetDuplicateDetectHashData(person1);
            PersonEntity person2 = new PersonEntity()
            {
                FirstName = "test2",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1)
            };
            string str2 = await _sut.GetDuplicateDetectHashData(person2);
            Assert.AreEqual(false, str1 == str2);
        }

        [Test]
        public async Task not_mapped_entity_GetDuplicateDetectHashData_return_null()
        {
            DuplicateDetectionService._configs = null;
            TestEntity testEntity = new TestEntity()
            {               
            };
            string str = await _sut.GetDuplicateDetectHashData(testEntity);
            Assert.AreEqual(null, str);
        }

        [Test]
        public async Task config_not_contain_entity_GetDuplicateDetectHashData_return_null()
        {
            DuplicateDetectionService._configs = null;
            odataClientMock.Setup(x => x.For<SSG_DuplicateDetectionConfig>(null)
                .FindEntriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_DuplicateDetectionConfig>>(new List<SSG_DuplicateDetectionConfig>()
                {
                                 new SSG_DuplicateDetectionConfig()
                                 {
                                     EntityName = "ssg_test",
                                     DuplicateFields = "ssg_firstname|ssg_lastname|ssg_dateofbirth"
                                 }
                }));

            PersonEntity person1 = new PersonEntity()
            {
                FirstName = "test1",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1)
            };
            string str = await _sut.GetDuplicateDetectHashData(person1);
            Assert.AreEqual(null, str);
        }

        [Test]
        public async Task different_person_with_same_duplicate_detect_fields_GetDuplicateDetectHashData_return_same()
        {
            DuplicateDetectionService._configs = null;
            PersonEntity person1 = new PersonEntity()
            {
                FirstName = "test1",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1),
                DateOfDeath = new DateTime(2025,1,1)
            };
            string str1 = await _sut.GetDuplicateDetectHashData(person1);
            PersonEntity person2 = new PersonEntity()
            {
                FirstName = "test1",
                LastName = "lastname",
                DateOfBirth = new DateTime(1999, 1, 1),
                DateOfDeath = null
            };
            string str2 = await _sut.GetDuplicateDetectHashData(person2);
            Assert.AreEqual(str1, str2);
        }


        [Test]
        public async Task person_has_same_identifier_Exists_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedIdentifierID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Identifiers = new List<SSG_Identifier>() {
                    new SSG_Identifier(){Identification="11111",IdentifierType=IdentificationType.BCDriverLicense.Value, IdentifierId=existedIdentifierID}
                }.ToArray()
            };
            IdentifierEntity entity = new IdentifierEntity() { Identification = "11111", IdentifierType = IdentificationType.BCDriverLicense.Value };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedIdentifierID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_identifier_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedIdentifierID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Identifiers = new List<SSG_Identifier>() {
                    new SSG_Identifier(){Identification="22111",IdentifierType=IdentificationType.BCDriverLicense.Value, IdentifierId=existedIdentifierID}
                }.ToArray()
            };
            IdentifierEntity entity = new IdentifierEntity() { Identification = "11111", IdentifierType = IdentificationType.BCDriverLicense.Value };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task person_has_same_phonenumber_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedPhoneID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_PhoneNumbers = new List<SSG_PhoneNumber>() {
                    new SSG_PhoneNumber(){TelePhoneNumber="11111",PhoneExtension="111", PhoneNumberId=existedPhoneID}
                }.ToArray()
            };
            PhoneNumberEntity entity = new PhoneNumberEntity() { TelePhoneNumber = "11111", PhoneExtension = "111" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedPhoneID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_phonenumber_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedPhoneID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_PhoneNumbers = new List<SSG_PhoneNumber>() {
                    new SSG_PhoneNumber(){TelePhoneNumber="111112",PhoneExtension="111", PhoneNumberId=existedPhoneID}
                }.ToArray()
            };
            PhoneNumberEntity entity = new PhoneNumberEntity() { TelePhoneNumber = "11111", PhoneExtension = "111" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task person_has_same_address_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedAddressID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Addresses = new List<SSG_Address>() {
                    new SSG_Address(){AddressLine1="11111",AddressLine2="111",City="city",CountryText="usa",AddressId=existedAddressID}
                }.ToArray()
            };
            AddressEntity entity = new AddressEntity() { AddressLine1 = "11111", AddressLine2 = "111", City = "city" , CountryText = "canada"};
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedAddressID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_address_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedAddressID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Addresses = new List<SSG_Address>() {
                    new SSG_Address(){AddressLine1="11111",AddressLine2="111",City="city",CountryText="usa",AddressId=existedAddressID}
                }.ToArray()
            };
            AddressEntity entity = new AddressEntity() { AddressLine1 = "11111", AddressLine2 = "111", CountryText = "canada" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task person_has_same_name_Exists_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedAliasID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Aliases = new List<SSG_Aliase>() {
                    new SSG_Aliase(){FirstName="firstname",MiddleName="middlename",LastName="lastName",AliasId=existedAliasID}
                }.ToArray()
            };
            AliasEntity entity = new AliasEntity() { FirstName = "firstname", LastName = "lastName" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedAliasID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_name_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedAliasID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Aliases = new List<SSG_Aliase>() {
                    new SSG_Aliase(){FirstName="firstname",MiddleName="middlename",LastName="lastName",AliasId=existedAliasID}
                }.ToArray()
            };
            AliasEntity entity = new AliasEntity() { FirstName = "firstnameNew", LastName = "lastName" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task person_has_same_vehicle_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedVehicleID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Asset_Vehicles = new List<SSG_Asset_Vehicle>() {
                    new SSG_Asset_Vehicle(){Vin="vin",PlateNumber="platenumber",VehicleId=existedVehicleID}
                }.ToArray()
            };
            VehicleEntity entity = new VehicleEntity() { Vin = "vin", PlateNumber = "platenumber" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedVehicleID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_vehicle_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedVehicleID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Asset_Vehicles = new List<SSG_Asset_Vehicle>() {
                    new SSG_Asset_Vehicle(){Vin="vin",PlateNumber="platenumber",VehicleId=existedVehicleID}
                }.ToArray()
            };
            VehicleEntity entity = new VehicleEntity() { Vin = "vin", PlateNumber = "platenumber2" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task vehicle_has_same_owner_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedOwnerID = Guid.NewGuid();
            SSG_Asset_Vehicle vehicle = new SSG_Asset_Vehicle()
            {
                SSG_AssetOwners = new List<SSG_AssetOwner>() {
                    new SSG_AssetOwner(){LastName="ownerlastname",AssetOwnerId=existedOwnerID}
                }.ToArray()
            };
            AssetOwnerEntity entity = new AssetOwnerEntity() { LastName = "ownerlastname",FirstName="test" };
            Guid guid = await _sut.Exists(vehicle, entity);
            Assert.AreEqual(existedOwnerID, guid);
        }

        [Test]
        public async Task vehicle_does_not_contain_same_owner_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedOwnerID = Guid.NewGuid();
            SSG_Asset_Vehicle vehicle = new SSG_Asset_Vehicle()
            {
                SSG_AssetOwners = new List<SSG_AssetOwner>() {
                    new SSG_AssetOwner(){LastName="ownerlastname",AssetOwnerId=existedOwnerID}
                }.ToArray()
            };
            AssetOwnerEntity entity = new AssetOwnerEntity() { LastName = "ownerlastname1", FirstName = "test" };
            Guid guid = await _sut.Exists(vehicle, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task person_has_same_relatedperson_Exists_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedRelatedPersonID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Identities = new List<SSG_Identity>() {
                    new SSG_Identity(){FirstName="11111", RelatedPersonId=existedRelatedPersonID}
                }.ToArray()
            };
            RelatedPersonEntity entity = new RelatedPersonEntity() { FirstName = "11111" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedRelatedPersonID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_relatedperson_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedRelatedPersonID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Identities = new List<SSG_Identity>() {
                    new SSG_Identity(){FirstName="11111", RelatedPersonId=existedRelatedPersonID}
                }.ToArray()
            };
            RelatedPersonEntity entity = new RelatedPersonEntity() { FirstName = "11121" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task person_has_same_employment_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedEmploymentID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Employments = new List<SSG_Employment>() {
                    new SSG_Employment(){Website="web",AddressLine1="line1",AddressLine2="line2", EmploymentId=existedEmploymentID}
                }.ToArray()
            };
            EmploymentEntity entity = new EmploymentEntity() { Website = "web", AddressLine1 = "line1" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(existedEmploymentID, guid);
        }

        [Test]
        public async Task person_does_not_contain_same_employment_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedEmploymentID = Guid.NewGuid();
            SSG_Person person = new SSG_Person()
            {
                SSG_Employments = new List<SSG_Employment>() {
                    new SSG_Employment(){Website="web",AddressLine1="line1",AddressLine2="line2", EmploymentId=existedEmploymentID}
                }.ToArray()
            };
            EmploymentEntity entity = new EmploymentEntity() { Website = "webdiffer", AddressLine1 = "line1" };
            Guid guid = await _sut.Exists(person, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task employment_has_same_employmentcontact_should_return_existed_entity_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedEmploymentContactID = Guid.NewGuid();
            SSG_Employment employment = new SSG_Employment()
            {
                SSG_EmploymentContacts = new List<SSG_EmploymentContact>() {
                    new SSG_EmploymentContact(){PhoneNumber="123456", EmploymentContactId=existedEmploymentContactID}
                }.ToArray()
            };
            EmploymentContactEntity entity = new EmploymentContactEntity() { PhoneNumber = "123456" };
            Guid guid = await _sut.Exists(employment, entity);
            Assert.AreEqual(existedEmploymentContactID, guid);
        }

        [Test]
        public async Task employment_does_not_contain_same_employmentcontact_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            Guid existedEmploymentContactID = Guid.NewGuid();
            SSG_Employment employment = new SSG_Employment()
            {
                SSG_EmploymentContacts = new List<SSG_EmploymentContact>() {
                    new SSG_EmploymentContact(){PhoneNumber="123456", EmploymentContactId=existedEmploymentContactID}
                }.ToArray()
            };
            EmploymentContactEntity entity = new EmploymentContactEntity() { PhoneNumber = "12345678" };
            Guid guid = await _sut.Exists(employment, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public async Task entity_not_having_config_Exists_should_return_empty_guid()
        {
            DuplicateDetectionService._configs = null;
            SSG_Asset_Vehicle vehicle = new SSG_Asset_Vehicle(){};
            TestEntity entity = new TestEntity() {  };
            Guid guid = await _sut.Exists(vehicle, entity);
            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public void fatherObj_not_contain_entity_type_should_throw_exception()
        {
            DuplicateDetectionService._configs = null;
            Guid existedOwnerID = Guid.NewGuid();
            SSG_Person person = new SSG_Person() {};
            AssetOwnerEntity entity = new AssetOwnerEntity() { LastName = "ownerlastname1", FirstName = "test" };
            Assert.ThrowsAsync<System.InvalidCastException>(async()=>await _sut.Exists(person, entity));
        }
    }

    public class TestEntity : DynamicsEntity
    { 
    }
}
