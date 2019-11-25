<<<<<<< refs/remotes/bcgov/master
using Fams3Adapter.Dynamics.Identifier;
=======
﻿using Fams3Adapter.Dynamics.Identifier;
>>>>>>> Auto stash before rebase of "bcgov/master"
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest
{
    public class PersonSearchRequestBuilder
    {
        private PersonSearchRequest _personSearchRequest;

        public PersonSearchRequestBuilder()
        {
        }

        public PersonSearchRequestBuilder WithSearchApiRequest(SSG_SearchApiRequest request)
        {
            this._personSearchRequest = new PersonSearchRequest()
            {
                FirstName = request.PersonGivenName,
                LastName = request.PersonSurname,
                DateOfBirth = request.PersonBirthDate,
                Identifiers = new List<Identifier>()
            };

            foreach (SSG_Identifier sSG_Identifier in request.Identifiers)
            {
                Identifier id = new Identifier();
                id.Identification = sSG_Identifier.Identification;
                id.IdentificationEffectiveDate = sSG_Identifier.IdentificationEffectiveDate;
                id.IdentificationExpirationDate = sSG_Identifier.IdentificationExpirationDate;
                id.IdentifierType = sSG_Identifier.IdentifierType==null? 0 
                                       : Enumeration.FromValue<IdentificationType>((int)sSG_Identifier.IdentifierType).SearchApiValue;
                id.InformationSourceType = sSG_Identifier.InformationSource==null ? 0 
                                            : Enumeration.FromValue<InformationSourceType>((int)sSG_Identifier.InformationSource).SearchApiValue;

                id.IssuedBy = sSG_Identifier.IssuedBy;
                this._personSearchRequest.Identifiers.Add(id);
            }
            return this;
        }

        public PersonSearchRequest Build()
        {
            return this._personSearchRequest;
        }
    }
}
