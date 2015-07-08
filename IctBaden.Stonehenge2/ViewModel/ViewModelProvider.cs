﻿namespace IctBaden.Stonehenge2.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    using Newtonsoft.Json;

    public class ViewModelProvider : IResourceProvider
    {
        public Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData)
        {
            if (!resourceName.StartsWith("ViewModel/")) return null;

            foreach (var data in formData)
            {
                SetPropertyValue(session.ViewModel, data.Key, data.Value);
            }


            var methodName = Path.GetFileNameWithoutExtension(resourceName);
            var vmTypeName = Path.GetFileNameWithoutExtension(resourceName.Replace("/" + methodName, string.Empty));


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
                Trace.TraceError(ex.Message);
                throw;
            }
            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, GetViewModelJson(session.ViewModel));
        }

        public Resource Get(AppSession session, string resourceName)
        {
            if (resourceName.StartsWith("ViewModel/"))
            {
                if (session.ViewModel == null)
                {
                    var vmTypeName = Path.GetFileNameWithoutExtension(resourceName);
                    if (session.SetViewModelType(vmTypeName) == null)
                    {
                        Trace.TraceError("Could not set ViewModel type to " + vmTypeName);
                        return null;
                    }
                }
                session.EventsClear(true);

                return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, GetViewModelJson(session.ViewModel));
            }
            else if (resourceName.StartsWith("Events/"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                var json = string.Format("{{ \"StonehengeContinuePolling\": true, \"Test\":{0} }}", DateTime.Now.Second);
                return new Resource(resourceName, "ViewModelProvider", ResourceType.Json, json);
            }

            return null;
        }

        private static void SetPropertyValue(object vm, string propName, string newval)
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
                            var members = JsonConvert.DeserializeObject(newval, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
                            if (members != null)
                            {
                                foreach (var member in members)
                                {
                                    var mProp = pi.PropertyType.GetProperty(member.Key);
                                    if (mProp != null)
                                    {
                                        var val = JsonConvert.DeserializeObject(member.Value, mProp.PropertyType);
                                        mProp.SetValue(structObj, val, null);
                                    }
                                }
                            }
                            activeVm.TrySetMember(propName, structObj);
                        }
                    }
                    else
                    {
                        var val = JsonConvert.DeserializeObject(newval, pi.PropertyType);
                        activeVm.TrySetMember(propName, val);
                    }
                }
                else
                {
                    var pi = vm.GetType().GetProperty(propName);
                    if ((pi == null) || !pi.CanWrite)
                        return;

                    var val = JsonConvert.DeserializeObject(newval, pi.PropertyType);
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