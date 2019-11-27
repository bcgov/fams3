using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest
{
    public static class Extensions
    {
        public static PersonSearchRequest ConvertToPersonSearchRequest(this SSG_SearchApiRequest request)
        {
            PersonSearchRequest personSearchRequest = new PersonSearchRequest()
            {
                FirstName = request.PersonGivenName,
                LastName = request.PersonSurname,
                DateOfBirth = request.PersonBirthDate,
                Identifiers = new List<Identifier>()
            };

            foreach (SSG_Identifier sSG_Identifier in request.Identifiers)
            {
                Identifier id = new Identifier();
                id.SerialNumber = sSG_Identifier.Identification;
                id.EffectiveDate = sSG_Identifier.IdentificationEffectiveDate;
                id.ExpirationDate = sSG_Identifier.IdentificationExpirationDate;
                id.Type = sSG_Identifier.IdentifierType == null ? 0
                                       : Enumeration.FromValue<IdentificationType>((int)sSG_Identifier.IdentifierType).SearchApiValue;
                id.IssuedBy = sSG_Identifier.IssuedBy;
                personSearchRequest.Identifiers.Add(id);
            }
            return personSearchRequest;
        }
    }
}
