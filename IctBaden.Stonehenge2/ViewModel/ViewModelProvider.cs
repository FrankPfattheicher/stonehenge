namespace IctBaden.Stonehenge2.ViewModel
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    using Newtonsoft.Json;

    public class ViewModelProvider : IResourceProvider
    {
        public Resource Load(AppSession session, string resourceName)
        {
            if (!resourceName.StartsWith("ViewModels/")) return null;
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

        private string GetViewModelJson(object viewModel)
        {
            var ty = viewModel.GetType();
            Debug.WriteLine("ViewModelProvider: viewModel=" + ty.Name);

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