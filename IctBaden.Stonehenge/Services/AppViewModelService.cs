
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using ServiceStack.Common.Web;
using ServiceStack.Text;
using JsonSerializer = ServiceStack.Text.JsonSerializer;

namespace IctBaden.Stonehenge.Services
{
    public class AppViewModelService : AppService
    {
        private const string ViewModelContentType = "application/json; charset=utf-8";

        public object Get(AppViewModel request)
        {
#if DEBUG
            var duration = new Stopwatch();
            duration.Start();
            Trace.TraceInformation("GET(vm) " + Request.AbsoluteUri);

            var result = GetInternal(request);

            duration.Stop();
            Trace.TraceInformation($"GET(vm) {duration.ElapsedMilliseconds}ms");

            return result;
        }

        private object GetInternal(AppViewModel request)
        {
#endif
            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            var context = Request.QueryString.Get("stonehenge_ctx");
            if (appSession == null)
            {
                return new HttpResult("No session for viewmodel request", HttpStatusCode.NotFound);
            }
            var currentParameters = Request.QueryString.AllKeys
                .ToDictionary(key => key, key => Request.QueryString.Get(key));
            appSession.Accessed(Request.Cookies, currentParameters, false);
            //appSession.EventsClear(true);
            appSession.SetContext(context);
            Debug.WriteLine("ViewModelService:" + request.ViewModel + " " + context);

            var vm = appSession.SetViewModelType(request.ViewModel);

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

            var builder = new ViewModelResult(RequestContext.CompressionType, appSession, vm);
            builder.Build();
            var result = builder.Result;
            return result;
        }

        public object Post(AppViewModel request)
        {
#if DEBUG
            var duration = new Stopwatch();
            duration.Start();
            Trace.TraceInformation("POST(vm) " + Request.AbsoluteUri);

            var result = PostInternal(request);

            duration.Stop();
            Trace.TraceInformation($"POST(vm) {duration.ElapsedMilliseconds}ms");

            return result;
        }

        private object PostInternal(AppViewModel request)
        {
#endif
            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            if (appSession == null)
            {
                return new HttpResult("No session for viewmodel request", HttpStatusCode.NotFound);
            }

            var currentParameters = Request.QueryString.AllKeys
                .ToDictionary(key => key, key => Request.QueryString.Get(key));
            appSession.Accessed(Request.Cookies, currentParameters, true);
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
                    var httpResult = new HttpResult(data.Bytes, data.ContentType);
                    if (!appSession.CookieSet)
                    {
                        httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
                    }
                    return httpResult;
                }
                else
                {
                    try
                    {
                        mi.Invoke(vm, parameters);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null) ex = ex.InnerException;
                        Trace.TraceError(ex.Message);
                        Trace.TraceError(ex.StackTrace);
#if DEBUG
                        MessageBox.Show(ex.Message, ex.StackTrace);
#endif
                        return new HttpResult(HttpStatusCode.InternalServerError, 
                            ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
            }

            var host = GetResolver() as AppHost;
            if (host?.Redirect == null)
            {
                var builder = new ViewModelResult(RequestContext.CompressionType, appSession, vm);
                builder.Build();
                var result = builder.Result;

                return returnData ? result : new HttpResult("{}", ViewModelContentType);
            }

            var redirect = new HttpResult { StatusCode = HttpStatusCode.Redirect };
            redirect.Headers.Add("Location", Request.Headers["Referer"] + "#/" + host.Redirect);
            host.Redirect = null;
            return redirect;
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
                            var members = JsonSerializer.DeserializeFromString(newval, typeof(JsonObject));
                            if (members != null)
                            {
                                foreach (var member in members.ToStringDictionary())
                                {
                                    var mProp = pi.PropertyType.GetProperty(member.Key);
                                    if (mProp != null)
                                    {
                                        var val = JsonSerializer.DeserializeFromString(member.Value, mProp.PropertyType);
                                        mProp.SetValue(structObj, val, null);
                                    }
                                }
                            }
                            activeVm.TrySetMember(propName, structObj);
                        }
                    }
                    else
                    {
                        var val = JsonSerializer.DeserializeFromString(newval, pi.PropertyType);
                        activeVm.TrySetMember(propName, val);
                    }
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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        public object Any(AppViewModel request)
        {
            var message = "AppViewModelService: No handler for HTTP method: " + Request.HttpMethod;
            Trace.TraceWarning(message);
            return new HttpResult(HttpStatusCode.NotFound, message);
        }

    }
}