using System;
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
            var objType = obj.GetType();
            var serialized = SerializeObject(prefix, obj).ToArray();
            if((objType == typeof(string)) || obj is IEnumerable)
            {
                return serialized[0];
            }
            var converter = objType.GetCustomAttributes(typeof(JsonConverterAttribute), true);
            if ((serialized.Length == 1) && (!objType.IsClass || (converter.Length > 0)))
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
                data.Add(JsonConvert.SerializeObject(obj));
                return data;
            }
            if (objType == typeof(bool))
            {
                data.Add(obj.ToString().ToLower());
                return data;
            }
            if ((objType == typeof(DateTime)) || (objType == typeof(DateTimeOffset)))
            {
                data.Add(JsonConvert.SerializeObject(obj));
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
            if (objType.IsValueType)
            {
                if (!objType.IsPrimitive && !objType.IsEnum && objType.Namespace != "System") // struct
                {
                    var structJson = new List<string>();
                    string json;
                    foreach (var member in objType.GetProperties())
                    {
                        var memberValue = member.GetValue(obj, null);
                        if (memberValue != null)
                        {
                            json = "\"" + prefix + member.Name + "\":" + JsonConvert.SerializeObject(memberValue);
                            structJson.Add(json);
                        }
                    }

                    json = "{ " + string.Join(",", structJson) + " }";
                    data.Add(json);
                    return data;
                }
                else
                {
                    data.Add(JsonConvert.SerializeObject(obj));
                    return data;
                }
            }

            foreach (var prop in objType.GetProperties())
            {
                var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
                if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
                    continue;
                var ignore = prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
                if (ignore.Length > 0)
                    continue;

                var value = prop.GetValue(obj, null);
                if (value == null)
                    continue;

                var json = "\"" + prefix + prop.Name + "\":" + SerializeObjectString(null, value);
                data.Add(json);
            }

            return data;
        }

    }
}