using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fams3Adapter.Dynamics
{
    public abstract class DynamicsEntity : IUpdatableObject
    {
        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_datadate")]
        public System.DateTime? Date1 { get; set; }

        [JsonProperty("ssg_datadatelabel")]
        public string Date1Label { get; set; }

        [JsonProperty("ssg_datadate2")]
        public System.DateTime? Date2 { get; set; }

        [JsonProperty("ssg_datadatelabel2")]
        public string Date2Label { get; set; }

        public bool Updated { get; set; }
    }

    public interface IUpdatableObject
    {
        public bool Updated { get; set; }
    }

    public static class IUpdatableObjectExtensions
    {
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static IUpdatableObject MergeUpdates<IUpdatableObject>(this IUpdatableObject originObj, object newObj)
        {
            if (newObj == null || originObj == null) return originObj;

            Type newType = newObj.GetType();
            bool updated = false;
            IList<PropertyInfo> props = new List<PropertyInfo>(newType.GetProperties());
            foreach (PropertyInfo propertyInfo in props)
            {
                var newValue = propertyInfo.GetValue(newObj, null);
                var oldValue = propertyInfo.GetValue(originObj, null);

                if (newValue != null && propertyInfo.Name != "Updated")
                {
                    bool isDifferent = !String.Equals(newValue.ToString(), oldValue == null ? "null" : oldValue.ToString(), StringComparison.InvariantCultureIgnoreCase);
                    if (isDifferent)
                    {
                        if (propertyInfo.PropertyType.Name == "Boolean")
                        {
                            if ((bool)newValue != false && newValue != oldValue) //new value is null or false, no matter old value has value or not, we do not change the old value
                            {
                                propertyInfo.SetValue(originObj, newValue);
                                updated = true;
                            }
                        }
                        else if (propertyInfo.PropertyType.Name == "String")
                        {
                            if (!String.IsNullOrEmpty((String)newValue))//new value is null, no matter old value has value or not, we do not change the old value
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
            PropertyInfo pi = props.SingleOrDefault(m => m.Name == "Updated");
            if (pi == null) return originObj;
            else pi.SetValue(originObj, updated);
            return originObj;
        }
    }
}
