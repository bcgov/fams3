using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Config;
using Fams3Adapter.Dynamics.Person;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.Config
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
        public async Task same_address_GetDuplicateDetectHashData_should_return_same_string()
        {
            DuplicateDetectionService._configs = null;
            AddressEntity address1 = new AddressEntity()
            {
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                City = "city"
            };
            string str1 = await _sut.GetDuplicateDetectHashData(address1);
            AddressEntity address2 = new AddressEntity()
            {
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                City = "city"
            };

            string str2 = await _sut.GetDuplicateDetectHashData(address2);
            Assert.AreEqual(true, str1 == str2);
        }

        [Test]
        public async Task different_address_GetDuplicateDetectHashData_should_return_different_string()
        {
            DuplicateDetectionService._configs = null;
            AddressEntity address1 = new AddressEntity()
            {
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                City = "city"
            };
            string str1 = await _sut.GetDuplicateDetectHashData(address1);
            AddressEntity address2 = new AddressEntity()
            {
                AddressLine1 = "line11",
                AddressLine2 = "line22",
                City = "city"
            };

            string str2 = await _sut.GetDuplicateDetectHashData(address2);
            Assert.AreEqual(false, str1 == str2);
        }

        [Test]
        public async Task different_address_with_same_duplicate_detect_fields_GetDuplicateDetectHashData_return_same()
        {
            DuplicateDetectionService._configs = null;
            AddressEntity address1 = new AddressEntity()
            {
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                City = "city",
                CountryText="canada"
            };
            string str1 = await _sut.GetDuplicateDetectHashData(address1);
            AddressEntity address2 = new AddressEntity()
            {
                AddressLine1 = "line1",
                AddressLine2 = "line2",
                City = "city",
                CountryText = "usa"
            };
            string str2 = await _sut.GetDuplicateDetectHashData(address2);
            Assert.AreEqual(str1, str2);
        }
    }

    public class TestEntity : DynamicsEntity
    { 
    }
}
