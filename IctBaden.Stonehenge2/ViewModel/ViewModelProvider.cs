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

        public Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData)
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
                            postParams,
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

        public Resource Get(AppSession session, string resourceName)
        {
            if (resourceName.StartsWith("ViewModel/"))
            {
                return GetViewModel(session, resourceName);
            }
            if (resourceName.StartsWith("Events/"))
            {
                return GetEvents(session, resourceName);
            }
            if (resourceName.StartsWith("Data/"))
            {
                return GetDataResource(session, resourceName.Substring(5));
            }

            return null;
        }

        private Resource GetViewModel(AppSession session, string resourceName)
        {
            var vmTypeName = Path.GetFileNameWithoutExtension(resourceName);
            if ((session.ViewModel == null) || (session.ViewModel.GetType().Name != vmTypeName))
            {
                if (session.SetViewModelType(vmTypeName) == null)
                {
                    Trace.TraceError("Could not set ViewModel type to " + vmTypeName);
                    return null;
                }
            }
            session.EventsClear(true);

            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, GetViewModelJson(session.ViewModel),
                Resource.Cache.None);
        }

        private static Resource GetEvents(AppSession session, string resourceName)
        {
            var json = new JObject { ["StonehengeContinuePolling"] = true };
            var events = session.CollectEvents();
            if (events.Count > 0)
            {
                var vm = session.ViewModel as ActiveViewModel;
                if (vm != null)
                {
                    foreach (var property in events)
                    {
                        json[property] = JToken.FromObject(vm.TryGetMember(property));
                    }
                }
            }

            var text = JsonConvert.SerializeObject(json);
            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, text, Resource.Cache.None);
        }

        private static Resource GetDataResource(AppSession session, string resourceName)
        {
            var vm = session.ViewModel as ActiveViewModel;
            var method = vm?.GetType()
                .GetMethods()
                .FirstOrDefault(m => string.Compare(m.Name, "GetDataResource", StringComparison.InvariantCultureIgnoreCase) == 0);
            if (method == null || method.ReturnType != typeof(Resource)) return null;

            Resource data;
            if (method.GetParameters().Count() == 2)
            {
                var parameters = new Dictionary<string, string>();
                    //Request.QueryString.AllKeys.ToDictionary(n => n, n => Request.QueryString[n]);
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

            var result = "{" + string.Join(",", data) + "}";
            return result;
        }

        private static IEnumerable<string> SerializeObject(string prefix, object obj)
        {
            var data = new List<string>();
            if (prefix == null)
                prefix = string.Empty;

            foreach (var prop in obj.GetType().GetProperties())
            {
                var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
                if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
                    continue;

                var value = prop.GetValue(obj, null);
                if (value == null)
                    continue;

                string json;
                if (prop.PropertyType.Name == "GraphOptions")
                {
                    json = "\"" + prefix + prop.Name + "\":" + value;
                }
                else if (prop.PropertyType.IsValueType && !prop.PropertyType.IsPrimitive && (prop.PropertyType.Namespace != "System")) // struct
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
                data.Add(json);
            }

            return data;
        }



    }
}