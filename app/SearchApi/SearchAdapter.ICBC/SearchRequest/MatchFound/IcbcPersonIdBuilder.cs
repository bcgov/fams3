using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest.MatchFound
{
    public class IcbcPersonIdBuilder
    {

        private readonly IcbcPersonId _personId;

        public IcbcPersonIdBuilder(PersonIDKind personIdKind)
        {
            _personId = new IcbcPersonId(personIdKind);
        }

        public IcbcPersonIdBuilder WithIssuer(string issuer)
        {
            _personId.Issuer = issuer;
            return this;
        }

        public IcbcPersonIdBuilder WithNumber(string number)
        {
            _personId.Number = number;
            return this;
        }

        public IcbcPersonId Build()
        {
            return _personId;
        }


    } 
    
    public sealed class IcbcPersonId : PersonId
    {
        public IcbcPersonId(PersonIDKind kind)
        {
            Kind = kind;
        }

        public PersonIDKind Kind { get; }
        public string Issuer { get; set; }
        public string Number { get; set; }
    }
}