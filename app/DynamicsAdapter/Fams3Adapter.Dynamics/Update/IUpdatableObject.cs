using Fams3Adapter.Dynamics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fams3Adapter.Dynamics.Update
{
    public interface IUpdatableObject
    {
        bool Updated { get; set; }
    }

    public static class IUpdatableObjectExtensions
    {

        public static IUpdatableObject MergeUpdates<IUpdatableObject>(this IUpdatableObject originObj, object newObj)
        {
            if (newObj == null || originObj == null) return originObj;

            Type newType = newObj.GetType();
            bool updated = false;
            IList<PropertyInfo> props = new List<PropertyInfo>(newType.GetProperties());
            foreach (PropertyInfo propertyInfo in props)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(UpdateIgnoreAttribute))) continue;

                var newValue = propertyInfo.GetValue(newObj, null);
                var oldValue = propertyInfo.GetValue(originObj, null);

                if (newValue != null)
                {
                    bool isDifferent = !string.Equals(newValue.ToString(), oldValue == null ? "null" : oldValue.ToString(), StringComparison.InvariantCultureIgnoreCase);
                    if (isDifferent)
                    {
                        if (propertyInfo.PropertyType.Name == "Boolean")
                        {
                            propertyInfo.SetValue(originObj, newValue);
                            updated = true;
                        }
                        else if (propertyInfo.PropertyType.Name == "String")
                        {
                            if (!string.IsNullOrEmpty((string)newValue))//new value is null, no matter old value has value or not, we do not change the old value
                            {
                                propertyInfo.SetValue(originObj, newValue);
                                updated = true;
                            }
                        }
                        else
                        {
                            propertyInfo.SetValue(originObj, newValue);
                            updated = true;
                        }
                    }
                    Type[] t = propertyInfo.PropertyType.GetInterfaces();
                    if (propertyInfo.PropertyType.GetInterfaces().SingleOrDefault(m => m.Name == "IUpdatableObject") != null)
                    {
                        propertyInfo.SetValue(originObj, MergeUpdates(oldValue, newValue));
                    }
                }
            }
            PropertyInfo pi = originObj.GetType().GetProperties().SingleOrDefault(m => m.Name == "Updated");
            if (pi == null) return originObj;
            else pi.SetValue(originObj, updated);
            return originObj;
        }

        public static IDictionary<string, object> GetUpdateEntries<IUpdatableObject>(this IUpdatableObject originObj, object newObj)
        {
            IDictionary<string, object> entries = new Dictionary<string, object>();
            if (newObj == null || originObj == null) return null;

            Type newType = newObj.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(newType.GetProperties());
            foreach (PropertyInfo propertyInfo in props)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(UpdateIgnoreAttribute))) continue;

                var newValue = propertyInfo.GetValue(newObj, null);
                var oldValue = propertyInfo.GetValue(originObj, null);

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
                    }

                }
            }

            return entries;
        }
    }
}
