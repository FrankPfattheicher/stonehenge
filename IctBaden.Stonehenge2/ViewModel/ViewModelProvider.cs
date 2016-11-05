using System.CodeDom;
using System.Collections;

namespace IctBaden.Stonehenge2.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Core;
    using Resources;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ViewModelProvider : IStonehengeResourceProvider
    {
        public void Dispose()
        {
            
        }

        public Resource Post(AppSession session, string resourceName, Dictionary<string, string> parameters, Dictionary<string, string> formData)
        {
            if (!resourceName.StartsWith("ViewModel/")) return null;

            var parts = resourceName.Split('/');
            if (parts.Length != 3) return null;
                
            var vmTypeName = parts[1];
            var methodName = parts[2];

            if (session.ViewModel == null)
            {
                session.SetViewModelType(vmTypeName);
            }

            foreach (var data in formData)
            {
                SetPropertyValue(session.ViewModel, data.Key, data.Value);
            }

            var vmType = session.ViewModel.GetType();
            if (vmType.Name != vmTypeName) return null;

            var method = vmType.GetMethod(methodName);
            if (method == null) return null;

            try
            {
                var methodParams =
                    method.GetParameters()
                        .Zip(
                            parameters.Values,
                            (parameterInfo, postParam) =>
                            new KeyValuePair<Type, object>(parameterInfo.ParameterType, postParam))
                        .Select(parameterPair => Convert.ChangeType(parameterPair.Value, parameterPair.Key))
                        .ToArray();
                method.Invoke(session.ViewModel, methodParams);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) ex = ex.InnerException;
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
                Debug.Assert(false);
                // ReSharper disable once HeuristicUnreachableCode
                return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, GetViewModelJson(ex), Resource.Cache.None);
            }
            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, GetViewModelJson(session.ViewModel), Resource.Cache.None);
        }

        public Resource Get(AppSession session, string resourceName, Dictionary<string, string> parameters)
        {
            if (resourceName.StartsWith("ViewModel/"))
            {
                if(SetViewModel(session, resourceName))
                    return GetViewModel(session, resourceName);
            }
            else if (resourceName.StartsWith("Events/"))
            {
                if (SetViewModel(session, resourceName))
                    return GetEvents(session, resourceName);
            }
            else if (resourceName.StartsWith("Data/"))
            {
                return GetDataResource(session, resourceName.Substring(5), parameters);
            }

            return null;
        }

        private bool SetViewModel(AppSession session, string resourceName)
        {
            var vmTypeName = Path.GetFileNameWithoutExtension(resourceName);
            if ((session.ViewModel != null) && (session.ViewModel.GetType().Name == vmTypeName)) return true;
            if (session.SetViewModelType(vmTypeName) != null) return true;

            Trace.TraceError("Could not set ViewModel type to " + vmTypeName);
            return false;
        }

        private Resource GetViewModel(AppSession session, string resourceName)
        {
            session.EventsClear(true);

            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, GetViewModelJson(session.ViewModel),
                Resource.Cache.None);
        }

        private static Resource GetEvents(AppSession session, string resourceName)
        {
            var data = new List<string> { "\"StonehengeContinuePolling\":true" };
            var events = session.CollectEvents();
            if (events.Count > 0)
            {
                var vm = session.ViewModel as ActiveViewModel;
                if (vm != null)
                {
                    foreach (var property in events)
                    {
                        var value = vm.TryGetMember(property);
                        data.Add($"\"{property}\":{SerializeObjectString(null, value)}");
                    }
                }
            }

            var json = "{" + string.Join(",", data) + "}";
            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, json, Resource.Cache.None);
        }

        private static Resource GetDataResource(AppSession session, string resourceName, Dictionary<string, string> parameters)
        {
            var vm = session.ViewModel as ActiveViewModel;
            var method = vm?.GetType()
                .GetMethods()
                .FirstOrDefault(m => string.Compare(m.Name, "GetDataResource", StringComparison.InvariantCultureIgnoreCase) == 0);
            if (method == null || method.ReturnType != typeof(Resource)) return null;

            Resource data;
            if (method.GetParameters().Length == 2)
            {
                data = (Resource)method.Invoke(vm, new object[] { resourceName, parameters });
            }
            else
            {
                data = (Resource)method.Invoke(vm, new object[] { resourceName });
            }
            return data;
        }

        private static object DeserializePropertyValue(string propValue, Type propType)
        {
            if (propType == typeof(string))
                return propValue;

            try
            {
                return JsonConvert.DeserializeObject(propValue, propType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        private static void SetPropertyValue(object vm, string propName, string newValue)
        {
            try
            {
                var activeVm = vm as ActiveViewModel;
                if (activeVm != null)
                {
                    var pi = activeVm.GetPropertyInfo(propName);
                    if ((pi == null) || !pi.CanWrite)
                        return;

                    if (pi.PropertyType.IsValueType && !pi.PropertyType.IsPrimitive && (pi.PropertyType.Namespace != "System")) // struct
                    {
                        object structObj = activeVm.TryGetMember(propName);
                        if (structObj != null)
                        {
                            var members = JsonConvert.DeserializeObject(newValue, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
                            if (members != null)
                            {
                                foreach (var member in members)
                                {
                                    var mProp = pi.PropertyType.GetProperty(member.Key);
                                    if (mProp != null)
                                    {
                                        var val = DeserializePropertyValue(member.Value, mProp.PropertyType);
                                        mProp.SetValue(structObj, val, null);
                                    }
                                }
                            }
                            activeVm.TrySetMember(propName, structObj);
                        }
                    }
                    else
                    {
                        var val = DeserializePropertyValue(newValue, pi.PropertyType);
                        activeVm.TrySetMember(propName, val);
                    }
                }
                else
                {
                    var pi = vm.GetType().GetProperty(propName);
                    if ((pi == null) || !pi.CanWrite)
                        return;

                    var val = DeserializePropertyValue(newValue, pi.PropertyType);
                    pi.SetValue(vm, val, null);
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        private string GetViewModelJson(object viewModel)
        {
            var ty = viewModel.GetType();
            Trace.TraceInformation("Stonehenge2.ViewModelProvider: viewModel=" + ty.Name);

            var data = new List<string>();
            var activeVm = viewModel as ActiveViewModel;
            if (activeVm != null)
            {
                foreach (var model in activeVm.ActiveModels)
                {
                    data.AddRange(SerializeObject(model.Prefix, model.Model));
                }

                if (!string.IsNullOrEmpty(activeVm.NavigateToRoute))
                {
                    data.Add("\"stonehenge_navigate\":" + JsonConvert.SerializeObject(activeVm.NavigateToRoute));
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var name in activeVm.GetDictionaryNames())
                {
                    // ReSharper disable once UseStringInterpolation
                    data.Add(string.Format("\"{0}\":{1}", name, JsonConvert.SerializeObject(activeVm.TryGetMember(name))));
                }
            }

            data.AddRange(SerializeObject(null, viewModel));

            var json = "{" + string.Join(",", data) + "}";
            return json;
        }

        private static string SerializeObjectString(string prefix, object obj)
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

        private static IEnumerable<string> SerializeObject(string prefix, object obj)
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