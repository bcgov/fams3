using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Fams3Adapter.Dynamics.Update
{
    public interface IUpdatableObject
    {
    }

    public static class IUpdatableObjectExtensions
    {


        public static IDictionary<string, object> GetUpdateEntries<IUpdatableObject>(this IUpdatableObject originObj, object newObj)
        {
            IDictionary<string, object> entries = new Dictionary<string, object>();
            if (newObj == null || originObj == null)
            {
                return null;
            }
            Type newType = newObj.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(newType.GetProperties());
            string updatedDetails = null;
            foreach (PropertyInfo propertyInfo in props)
            {
                //only properties having DisplayName need to be updated.
                if (!Attribute.IsDefined(propertyInfo, typeof(DisplayNameAttribute))) continue;

                var newValue = propertyInfo.GetValue(newObj, null);
                var oldValue = propertyInfo.GetValue(originObj, null);

                if (Attribute.IsDefined(propertyInfo, typeof(CompareOnlyNumberAttribute)))
                {
                    newValue = ((string)newValue)?.GetNumbers();
                    oldValue = ((string)oldValue)?.GetNumbers();
                }

                if (newValue != null)
                {
                    bool isDifferent = !string.Equals(newValue.ToString(), oldValue == null ? "null" : oldValue.ToString(), StringComparison.InvariantCultureIgnoreCase);
                    if (isDifferent)
                    {
                        string jsonPropertyName = propertyInfo.GetCustomAttributes<JsonPropertyAttribute>()?.FirstOrDefault()?.PropertyName;

                        if (jsonPropertyName != null)
                        {
                            entries.Add(new KeyValuePair<string, object>(
                                propertyInfo.GetCustomAttributes<JsonPropertyAttribute>().FirstOrDefault().PropertyName,
                                newValue));
                        }
                        updatedDetails += string.IsNullOrEmpty(updatedDetails) ? propertyInfo.GetCustomAttributes<DisplayNameAttribute>()?.FirstOrDefault()?.DisplayName
                            : ", " + propertyInfo.GetCustomAttributes<DisplayNameAttribute>()?.FirstOrDefault()?.DisplayName;
                    }

                }
            }

            if (!string.IsNullOrEmpty(updatedDetails))
            {
                entries.Add(new KeyValuePair<string, object>("ssg_agencyupdatedescription", updatedDetails));
            }
            return entries;
        }

        public static string GetNumbers(this string text)
        {
            text = text ?? string.Empty;
            return new string(text.Where(p => char.IsDigit(p)).ToArray());
        }
    }
}
