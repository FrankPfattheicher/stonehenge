using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using ServiceStack.Common.Web;
using ServiceStack.Text;

namespace IctBaden.Stonehenge.Services
{
    public class AppViewModelService : AppService
    {
        public object Get(AppViewModel request)
        {
            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            if (appSession == null)
            {
                return new HttpResult("No session for viewmodel request", HttpStatusCode.NotFound);
            }
            appSession.Accessed();

            Debug.WriteLine("ViewModelService:" + request.ViewModel);

            var vm = appSession.SetViewModelType(request.ViewModel);
            appSession.EventsClear(true);

            if (Request.RemoteIp != appSession.ClientAddress)
            {
                appSession.ClientAddressChanged(Request.RemoteIp);
                Debug.WriteLine("AppViewModelService: Client address changed: " + appSession.ClientAddress);
            }

            var ty = vm.GetType();
            Debug.WriteLine("AppViewModelService: ~vm=" + ty.Name);

            var pi = ty.GetProperty(request.ViewModel);
            if (pi != null)
            {
                if (request.Source != null)
                    pi.SetValue(vm, request.Source, null);
            }

            var data = new List<string>();
            var activeVm = vm as ActiveViewModel;
            if (activeVm != null)
            {
                foreach (var model in activeVm.ActiveModels)
                {
                    data.AddRange(SerializeObject(model.Prefix, model.Model));
                }

                if (!string.IsNullOrEmpty(activeVm.NavigateToRoute))
                {
                    data.Add("\"stonehenge_navigate\":" + JsonSerializer.SerializeToString(activeVm.NavigateToRoute));
                }

            }

            data.AddRange(SerializeObject(null, vm));

            var result = "{" + string.Join(",", data) + "}";

            appSession.EventsClear(true);

            HttpResult httpResult;

            if (!string.IsNullOrEmpty(RequestContext.CompressionType))
            {
                var compressed = new CompressedResult(Encoding.UTF8.GetBytes(result), RequestContext.CompressionType)
                  {
                      ContentType = "application/json"
                  };
                httpResult = new HttpResult(compressed.Contents, "application/json");
                httpResult.Headers.Add("CompressionType", RequestContext.CompressionType);
                return httpResult;
            }

            httpResult = new HttpResult(result, "application/json");
            return httpResult;
        }

        public object Post(AppViewModel request)
        {
            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            if (appSession == null)
            {
                return new HttpResult("No session for viewmodel request", HttpStatusCode.NotFound);
            }

            appSession.EventsClear(true);

            var vm = appSession.ViewModel;
            if (vm == null)
                return Get(request);

            if (request.ViewModel == vm.GetType().FullName)
            {
                foreach (var query in Request.FormData.AllKeys)
                {
                    var newval = Request.FormData[query];
                    SetPropertyValue(vm, query, newval);
                }
            }

            var returnData = true;
            var mi = vm.GetType().GetMethod(request.Source);
            if (mi != null)
            {
                var parameters = (mi.GetParameters().Length == 0) ? new object[0] : new object[] { Request.FormData["_stonehenge_CommandParameter_"] };
                if (mi.ReturnType == typeof(bool))
                {
                    returnData = (bool)mi.Invoke(vm, parameters);
                }
                else if (mi.ReturnType == typeof(UserData))
                {
                    var data = (UserData)mi.Invoke(vm, parameters);
                    return new HttpResult(data.Bytes, data.ContentType);
                }
                else
                {
                    mi.Invoke(vm, parameters);
                }
            }

            var host = GetResolver() as AppHost;
            if (host != null)
            {
                if (host.Redirect != null)
                {
                    var redirect = new HttpResult { StatusCode = HttpStatusCode.Redirect };
                    redirect.Headers.Add("Location", Request.Headers["Referer"] + "#/" + host.Redirect);
                    host.Redirect = null;
                    return redirect;
                }
            }

            return returnData ? Get(request) : new HttpResult("{}", "application/json");
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
                else
                {
                    json = "\"" + prefix + prop.Name + "\":" + JsonSerializer.SerializeToString(value);
                }
                data.Add(json);
            }

            return data;
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

                    var val = JsonSerializer.DeserializeFromString(newval, pi.PropertyType);
                    activeVm.TrySetMember(propName, val);
                }
                else
                {
                    var pi = vm.GetType().GetProperty(propName);
                    if ((pi == null) || !pi.CanWrite)
                        return;

                    var val = JsonSerializer.DeserializeFromString(newval, pi.PropertyType);
                    pi.SetValue(vm, val, null);
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }
    }
}