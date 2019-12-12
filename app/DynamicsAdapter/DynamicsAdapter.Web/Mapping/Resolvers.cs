using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Mapping
{
    public class FullTextResolver : IValueResolver<PersonalAddress, SSG_Address, string>
    {
        public string Resolve(PersonalAddress source, SSG_Address dest, string fullText, ResolutionContext context)
        {
            return $"{source.AddressLine1} {source.AddressLine2} {source.City} {source.Province} {source.Country} {source.PostalCode}";
        }
    }
}
