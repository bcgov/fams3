using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Email;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.TaxIncomeInformation;
using Fams3Adapter.Dynamics.Vehicle;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Duplicate
{
    public interface IDuplicateDetectionService
    {
        Task<string> GetDuplicateDetectHashData(object entity);
        Task<Guid> Exists(object fatherObj, object entity);
        Task<bool> Same(object entity, object ssg);
    }

    public class DuplicateDetectionService : IDuplicateDetectionService
    {
        private readonly IODataClient _oDataClient;
        public static IEnumerable<SSG_DuplicateDetectionConfig> _configs;
        public static Dictionary<string, string> EntityNameMap = new Dictionary<string, string>
        {
            {"PersonEntity", "ssg_person" },
            {"AddressEntity", "ssg_address" },
            {"IdentifierEntity", "ssg_identifier" },
            {"PhoneNumberEntity", "ssg_phonenumber" },
            {"AliasEntity", "ssg_aliase"},
            {"VehicleEntity", "ssg_asset_vehicle"},
            {"AssetOwnerEntity", "ssg_assetowner"},
            {"RelatedPersonEntity", "SSG_identity" },
            {"EmploymentEntity", "ssg_employment" },
            {"EmploymentContactEntity","ssg_employmentcontact" },
            {"AssetOtherEntity","ssg_asset_other" },
            {"BankingInformationEntity","ssg_asset_bankinginformation"},
            {"CompensationClaimEntity","ssg_asset_worksafebcclaim"},
            {"ICBCClaimEntity","ssg_asset_icbcclaim"},
            {"SimplePhoneNumberEntity","ssg_simplephonenumber" },
            {"InvolvedPartyEntity","ssg_involvedparty" },
            {"NotesEntity","ssg_notese" },
            {"SafetyConcernEntity", "ssg_safetyconcerndetail" },
            {"EmailEntity", "ssg_email" },
            {"TaxIncomeInformationEntity", "ssg_taxincomeinformation" }
        };

        public DuplicateDetectionService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }

        /// <summary>
        /// make the entity hash data fields according to configuration.
        /// Example: config is : ssg_person, ssg_firstname|ssg_lastname
        ///     ssg_person person1: firstname="person1", lastname="lastname1"
        ///     it should return SHA512("person1lastname1")
        /// This is mainly used for Person
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>SHA512 </returns>
        public async Task<string> GetDuplicateDetectHashData(object entity)
        {
            if (_configs == null) await GetDuplicateDetectionConfig(CancellationToken.None);

            Type type = entity.GetType();
            string name;
            if (!EntityNameMap.TryGetValue(type.Name, out name))
            {
                return null;
            }

            SSG_DuplicateDetectionConfig config = _configs.FirstOrDefault(m => m.EntityName == name);
            if (config == null) return null;

            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());       

            return hashstring(GetConcateFieldsStr(config.DuplicateFieldList, props, entity));
        }

        /// <summary>
        /// check if existing fatherObj contains the same entity. The standard for "same" is the config from dynamics.
        /// if there is duplicated entity in this fatherObj, then return its guid.
        /// if no duplicate found, return guid.empty.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Guid> Exists(object fatherObj, object entity)
        {
            if (_configs == null) await GetDuplicateDetectionConfig(CancellationToken.None);

            Type type = entity.GetType();
            string name;
            if (!EntityNameMap.TryGetValue(type.Name, out name))
            {
                return Guid.Empty;
            }

            switch (type.Name)
            {
                case "IdentifierEntity":
                    foreach (SSG_Identifier identifier in ((SSG_Person)fatherObj).SSG_Identifiers)
                    {
                        if (await Same(entity, identifier)) return identifier.IdentifierId;
                    };
                    break;
                case "PhoneNumberEntity":
                    foreach (SSG_PhoneNumber phone in ((SSG_Person)fatherObj).SSG_PhoneNumbers)
                    {
                        if (await Same(entity, phone)) return phone.PhoneNumberId;
                    };
                    break;
                case "AddressEntity":
                    foreach (SSG_Address addr in ((SSG_Person)fatherObj).SSG_Addresses)
                    {
                        if (await Same(entity, addr)) return addr.AddressId;
                    };
                    break;
                case "AliasEntity":
                    foreach (SSG_Aliase alias in ((SSG_Person)fatherObj).SSG_Aliases)
                    {
                        if (await Same(entity, alias)) return alias.AliasId;
                    };
                    break;
                case "VehicleEntity":
                    foreach (SSG_Asset_Vehicle v in ((SSG_Person)fatherObj).SSG_Asset_Vehicles)
                    {
                        if (await Same(entity, v)) return v.VehicleId;
                    };
                    break;
                case "AssetOwnerEntity":
                    foreach (SSG_AssetOwner owner in ((SSG_Asset_Vehicle)fatherObj).SSG_AssetOwners)
                    {
                        if (await Same(entity, owner)) return owner.AssetOwnerId;
                    };
                    break;
                case "RelatedPersonEntity":
                    foreach (SSG_Identity relatedPerson in ((SSG_Person)fatherObj).SSG_Identities)
                    {
                        if (await Same(entity, relatedPerson)) return relatedPerson.RelatedPersonId;
                    };
                    break;
                case "EmploymentEntity":
                    foreach (SSG_Employment employment in ((SSG_Person)fatherObj).SSG_Employments)
                    {
                        if (await Same(entity, employment)) return employment.EmploymentId;
                    };
                    break;
                case "EmploymentContactEntity":
                    foreach (SSG_EmploymentContact contact in ((SSG_Employment)fatherObj).SSG_EmploymentContacts)
                    {
                        if (await Same(entity, contact)) return contact.EmploymentContactId;
                    };
                    break;
                case "AssetOtherEntity":
                    foreach (SSG_Asset_Other other in ((SSG_Person)fatherObj).SSG_Asset_Others)
                    {
                        if (await Same(entity, other)) return other.AssetOtherId;
                    };
                    break;
                case "BankingInformationEntity":
                    foreach (SSG_Asset_BankingInformation bankInfo in ((SSG_Person)fatherObj).SSG_Asset_BankingInformations)
                    {
                        if (await Same(entity, bankInfo)) return bankInfo.BankingInformationId;
                    };
                    break;
                case "CompensationClaimEntity":
                    foreach (SSG_Asset_WorkSafeBcClaim claim in ((SSG_Person)fatherObj).SSG_Asset_WorkSafeBcClaims)
                    {
                        if (await Same(entity, claim)) return claim.CompensationClaimId;
                    };
                    break;
                case "ICBCClaimEntity":
                    foreach (SSG_Asset_ICBCClaim insurance in ((SSG_Person)fatherObj).SSG_Asset_ICBCClaims)
                    {
                        if (await Same(entity, insurance)) return insurance.ICBCClaimId;
                    };
                    break;
                case "SimplePhoneNumberEntity":
                    foreach (SSG_SimplePhoneNumber phone in ((SSG_Asset_ICBCClaim)fatherObj).SSG_SimplePhoneNumbers)
                    {
                        if (await Same(entity, phone)) return phone.SimplePhoneNumberId;
                    };
                    break;
                case "InvolvedPartyEntity":
                    foreach (SSG_InvolvedParty party in ((SSG_Asset_ICBCClaim)fatherObj).SSG_InvolvedParties)
                    {
                        if (await Same(entity, party)) return party.InvolvedPartyId;
                    };
                    break;
                case "NotesEntity":
                    foreach (SSG_Notese note in ((SSG_SearchRequest)fatherObj).SSG_Notes)
                    {
                        if (await Same(entity, note)) return note.NotesId;
                    };
                    break;
                case "SafetyConcernEntity":
                    foreach (SSG_SafetyConcernDetail safety in ((SSG_Person)fatherObj).SSG_SafetyConcernDetails)
                    {
                        if (await Same(entity, safety)) return safety.SafetyConcernDetailId;
                    };
                    break;
                case "EmailEntity":
                    foreach (SSG_Email email in ((SSG_Person)fatherObj).SSG_Emails)
                    {
                        if (await Same(entity, email)) return email.EmailId;
                    };
                    break;
                case "TaxIncomeInformationEntity":
                    if (((SSG_Person)fatherObj).SSG_Taxincomeinformations != null) 
                    {
                        foreach (SSG_Taxincomeinformation taxinfo in ((SSG_Person)fatherObj).SSG_Taxincomeinformations)
                        {
                            if (await Same(entity, taxinfo)) return taxinfo.TaxincomeinformationId;
                        };
                    }
                    break;
            }            

            return Guid.Empty;
        }

        /// <summary>
        /// This is to compare if an entity obj is the same as ssg obj. same means the configed properties are same value
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ssg"></param>
        /// <returns></returns>
        public async Task<bool> Same(object entity, object ssg)
        {
            if (entity == null && ssg == null) return true;
            if (entity == null || ssg == null) return false;
            if (_configs == null) await GetDuplicateDetectionConfig(CancellationToken.None);

            Type type = entity.GetType();
            string ssgName;
            if(!EntityNameMap.TryGetValue(type.Name, out ssgName)) return false;
            if( !ssgName.Equals(ssg.GetType().Name,StringComparison.InvariantCultureIgnoreCase)) 
                return false;

            SSG_DuplicateDetectionConfig config = _configs.FirstOrDefault(m => m.EntityName.ToLower() == ssgName.ToLower());
            if (config == null) return false;

            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());
            string entityStr = GetConcateFieldsStr(config.DuplicateFieldList, props, entity);
            string ssgStr = GetConcateFieldsStr(config.DuplicateFieldList, props, ssg);
            return entityStr.Equals(ssgStr, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetConcateFieldsStr(string[] duplicateFieldList, IList<PropertyInfo> props, object entity)
        {
            string concatedString = string.Empty;
            foreach (string field in duplicateFieldList)
            {
                foreach (PropertyInfo p in props)
                {
                    JsonPropertyAttribute attr = p.GetCustomAttributes<JsonPropertyAttribute>().FirstOrDefault(m => m.PropertyName.ToLower() == field.ToLower());
                    if (attr != null)
                    {
                        object value = p.GetValue(entity, null);
                        if (value != null)
                            concatedString += value.ToString();
                        break;
                    }
                }
            }
            return concatedString;
        }

        private async Task<bool> GetDuplicateDetectionConfig(CancellationToken cancellationToken)
        {
            if (_configs != null) return true;
            IEnumerable<SSG_DuplicateDetectionConfig> duplicateConfigs = await _oDataClient.For<SSG_DuplicateDetectionConfig>()
                .FindEntriesAsync(cancellationToken);

            SSG_DuplicateDetectionConfig[] array = duplicateConfigs.ToArray();
            for(int i=0; i<array.Count(); i++)
            {
                array[i].DuplicateFieldList=array[i].DuplicateFields.Split("|");
            }

            _configs = array.AsEnumerable<SSG_DuplicateDetectionConfig>();
            return true;
        }

        private static string hashstring(string input)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }
    }
}
