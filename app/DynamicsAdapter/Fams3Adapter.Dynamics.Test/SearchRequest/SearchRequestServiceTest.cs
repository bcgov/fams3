using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock;

        private readonly Guid testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");

        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            odataClientMock = new Mock<IODataClient>();

            odataClientMock.Setup(x => x.For<SSG_Identifier>(null).Set(It.Is<SSG_Identifier>(x => x.Identification == "identificationtest"))
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_Identifier()
            {
                Identification = "test"
            })
            );

            odataClientMock.Setup(x => x.For<SSG_Country>(null)
                            .Filter(It.IsAny<Expression<Func<SSG_Country, bool>>>())
                            .FindEntryAsync(It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult<SSG_Country>(new SSG_Country()
                            {
                                CountryId = Guid.NewGuid(),
                                Name = "Canada"
                            }));

            odataClientMock.Setup(x => x.For<SSG_Address>(null).Set(It.Is<SSG_Address>(x => x.AddressLine1 == "address full text"))
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_Address()
            {
                AddressLine1 = "test"
            })
            );

            odataClientMock.Setup(x => x.For<SSG_PhoneNumber>(null).Set(It.Is<SSG_PhoneNumber>(x => x.TelePhoneNumber == "4007678231"))
           .InsertEntryAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new SSG_PhoneNumber()
           {
               TelePhoneNumber = "4007678231"
           })
           );

            odataClientMock.Setup(x => x.For<SSG_Aliase>(null).Set(It.Is<SSG_Aliase>(x => x.FirstName == "firstName"))
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_Aliase()
            {
               FullName = "firstName middleName lastName"
            })
            );

            _sut = new SearchRequestService(odataClientMock.Object);
        }


        [Test]
        public async Task with_correct_searchRequestid_upload_identifier_should_success()
        {
            var identifier = new SSG_Identifier()
            {
                Identification = "identificationtest",
                //IdentificationEffectiveDate = DateTime.Now,
                StateCode = 0,
                StatusCode = 1,
                SSG_SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId }
            };

            var result = await _sut.CreateIdentifier(identifier, CancellationToken.None);

            Assert.AreEqual("test", result.Identification);
        }

        [Test]
        public async Task with_correct_searchRequestid_upload_phone_number_should_success()
        {
            var phone = new SSG_PhoneNumber()
            {
              
                DateData = DateTime.Now,
                DateType = "Effective Date",
                TelePhoneNumber = "4007678231",
                StateCode = 0,
                StatusCode = 1,
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId }
            };

            var result = await _sut.CreatePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual("4007678231", result.TelePhoneNumber);
        }

        [Test]
        public async Task with_correct_searchRequestid_upload_address_should_success()
        {
            var address = new SSG_Address()
            {
                AddressLine1 = "address full text",
                Country = new SSG_Country() { Name = "canada" },
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId }
            };

            var result = await _sut.CreateAddress(address, CancellationToken.None);

            Assert.AreEqual("test", result.AddressLine1);
        }

        [Test]
        public async Task with_correct_searchRequestid_upload_name_should_success()
        {
            var name = new SSG_Aliase()
            {
                FirstName = "firstName",
                LastName = "lastName",
                MiddleName = "middleName",
                FullName = "firstName middleName lastName",
                Comments = "testComments",
                Type = PersonNameCategory.MaidenName.Value,
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId }
            };

            var result = await _sut.CreateName(name, CancellationToken.None);

            Assert.AreEqual("firstName middleName lastName", result.FullName);
        }
    }
}
