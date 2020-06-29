using Fams3Adapter.Dynamics.Person;
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

namespace Fams3Adapter.Dynamics.Config
{
    public interface IDuplicateDetectionService
    {
        Task<string> GetDuplicateDetectHashData(object entity);
    }

    public class DuplicateDetectionService : IDuplicateDetectionService
    {
        private readonly IODataClient _oDataClient;
        public static IEnumerable<SSG_DuplicateDetectionConfig> _configs;
        public static Dictionary<string, string> EntityNameMap = new Dictionary<string, string>
        {
            {"PersonEntity", "ssg_person" }
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
