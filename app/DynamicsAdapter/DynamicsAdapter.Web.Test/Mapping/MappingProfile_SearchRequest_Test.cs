using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicsAdapter.Web.Test.Mapping
{
    public class MappingProfile_SearchRequest_Test
    {

        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void normal_SearchRequestOrdered_should_map_to_SearchRequestEntity_correctly()
        {
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "requestId",
                SearchRequestKey = "requestKey",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2001, 1, 1),
                Person = new Person() {
                    Agency= new Agency()
                    {
                        Agent = new Name() { FirstName = "agentFirstName", LastName = "agentLastName"},
                        AgentContact = new List<Phone> { 
                            new Phone(){PhoneNumber="agentPhoneNumber", Extension="agentExt", Type="Phone"},
                            new Phone(){PhoneNumber="agentFaxNumber", Type="Fax"}
                        },
                        Notes = "agency notes",
                        RequestPriority = RequestPriority.Normal,
                        Code = "FMEP",
                        RequestId = "QFP-12422509096920180928083433",
                        ReasonCode = "EnfPayAgr",
                        RequestDate = new DateTimeOffset(2018,9,28,0,0,0, new TimeSpan(1, 0, 0)),
                        Email = "agencyemail@test.com",
                        InformationRequested = new List<string> { "location", "phn","DL"}
                    }
                },
            };
            SearchRequestEntity entity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            Assert.AreEqual("agentFirstName", entity.AgentFirstName);
            Assert.AreEqual("agentLastName", entity.AgentLastName);
            Assert.AreEqual("agentPhoneNumber", entity.AgentPhoneNumber);
            Assert.AreEqual("agentExt", entity.AgentPhoneExtension);
            Assert.AreEqual("agentFaxNumber", entity.AgentFax);
            Assert.AreEqual("agency notes", entity.Notes);
            Assert.AreEqual("QFP-12422509096920180928083433", entity.OriginalRequestorReference);
            Assert.AreEqual(PersonSoughtType.P.Value, entity.PersonSoughtRole);
            Assert.AreEqual("124225", entity.PayerId);
            Assert.AreEqual("090969", entity.CaseTrackingId);
            Assert.AreEqual(new DateTime(2018,9,28), entity.RequestDate);
            Assert.AreEqual("agencyemail@test.com", entity.AgentEmail);
            Assert.AreEqual(true, entity.LocationRequested);
            Assert.AreEqual(true, entity.PHNRequested);
            Assert.AreEqual(true, entity.DriverLicenseRequested);
        }

        [Test]
        public void Agency_null_SearchRequestOrdered_should_return_null()
        {
        }
    }
}
