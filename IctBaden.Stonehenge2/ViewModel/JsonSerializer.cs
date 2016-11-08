using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace IctBaden.Stonehenge2.ViewModel
{
    public static class JsonSerializer
    {
        public static string SerializeObjectString(string prefix, object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            var serialized = SerializeObject(prefix, obj).ToArray();
            if (serialized.Length == 1)
            {
                return serialized[0];
            }
            return "{" + string.Join(",", serialized) + "}";
        }

        public static IEnumerable<string> SerializeObject(string prefix, object obj)
        {
            if (prefix == null)
                prefix = string.Empty;

            var objType = obj.GetType();
            var data = new List<string>();

            if (objType == typeof(string))
            {
                data.Add($"\"{obj}\"");
                return data;
            }
            if (objType == typeof(bool))
            {
                data.Add(obj.ToString().ToLower());
                return data;
            }

            var converter = objType.GetCustomAttributes(typeof(JsonConverterAttribute), true);
            if (converter.Length > 0)
            {
                data.Add(JsonConvert.SerializeObject(obj));
                return data;
            }

            var enumerable = obj as IEnumerable;
            if (enumerable != null)
            {
                var elements = new List<string>();
                foreach (var element in enumerable)
                {
                    elements.Add(SerializeObjectString(null, element));
                }
                data.Add("[" + string.Join(",", elements) + "]");
                return data;
            }

            foreach (var prop in objType.GetProperties())
            {
                var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
                if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
                    continue;
                var ignore = prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
                if (ignore.Length > 0)
                    continue;

                string json;
                var value = prop.GetValue(obj, null);
                if (value == null)
                    continue;

                if (prop.PropertyType.IsValueType && !prop.PropertyType.IsPrimitive)
                {
                    if (!prop.PropertyType.IsEnum && prop.PropertyType.Namespace != "System") // struct
                    {
                        var structJson = new List<string>();

                        foreach (var member in prop.PropertyType.GetProperties())
                        {
                            var memberValue = member.GetValue(value, null);
                            if (memberValue != null)
                            {
                                json = "\"" + prefix + member.Name + "\":" + JsonConvert.SerializeObject(memberValue);
                                structJson.Add(json);
                            }
                        }

                        json = "\"" + prefix + prop.Name + "\": { " + string.Join(",", structJson) + " }";
                    }
                    else
                    {
                        json = "\"" + prefix + prop.Name + "\":" + JsonConvert.SerializeObject(value);
                    }
                }
                else
                {
                    //json = "\"" + prefix + prop.Name + "\":" + JsonConvert.SerializeObject(value);
                    json = "\"" + prefix + prop.Name + "\":" + SerializeObjectString(null, value);
                }
                data.Add(json);
            }

            return data;
        }

    }
}