using BcGov.Fams3.SearchApi.Contracts.IA;
using BcGov.Fams3.SearchApi.Contracts.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAdapter.Sample.IA
{
    public class IASearchResultSample : IASearchResult
    {
        public Guid SearchRequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public Person Person { get; set; }

        public string BatchNo { get; set; }
    }

  

    public class IASearchFailedSample : IASearchFailed
    {
        public Guid SearchRequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public Person Person { get; set; }

        public string BatchNo { get; set; }

        public bool Retry { get; set; }

        public string Cause { get; set; }
    }

    public static class FakeIABuilder
    {
        public static IASearchResult BuildFakeIASeachResult(Guid searchrequestId, string SearchRequestKey, string batchno)
        {

            return new IASearchResultSample
            {
                BatchNo = batchno,
                SearchRequestKey =SearchRequestKey,
                SearchRequestId = searchrequestId,
                TimeStamp = DateTime.Now,
                Person = new Person
                {
                 
                    FirstName = "firstname",
                    LastName = "secondname",
                    MiddleName = "middlename",
                    OtherName = "othername",
                    Identifiers = new List<PersonalIdentifier>()
                    {
                        new PersonalIdentifier {Type = PersonalIdentifierType.SocialInsuranceNumber, Value = "123123123123",    ReferenceDates  = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                                }}
                    }
                }
            };

        }

        public static IASearchFailed BuildFakeIAFailed( Guid searchrequestId, string searchRequestKey,  string batchno, string message, bool retry)
        {
            return new IASearchFailedSample()
            {
               Cause = message,
               Retry = retry,
                BatchNo = batchno,
                SearchRequestKey = searchRequestKey,
                SearchRequestId = searchrequestId,
                TimeStamp = DateTime.Now,
                Person = new Person
                {

                    FirstName = "firstname",
                    LastName = "secondname",
                    MiddleName = "middlename",
                    OtherName = "othername",
                    Identifiers = new List<PersonalIdentifier>()
                    {
                        new PersonalIdentifier {Type = PersonalIdentifierType.SocialInsuranceNumber, Value = "123123123123",    ReferenceDates  = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="Effective Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="Expiration Date", Value=new DateTime(2020,9,1) }
                                }}
                    }
                }
            };
        }

    }
}