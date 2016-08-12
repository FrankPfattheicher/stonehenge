
#define NotUseTaskParallelLibrary

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ServiceStack.Common.Web;
using ServiceStack.Text;

#if UseTaskParallelLibrary
using System.Threading.Tasks;
#endif

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
            appSession.Accessed(Request.Cookies, false);
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

            return GetViewModelResult(request, appSession, vm);
        }

        public object GetViewModelResult(AppViewModel request, AppSession appSession, object viewModel)
        {
            var data = new List<string>();
            var activeVm = viewModel as ActiveViewModel;
            if (activeVm != null)
            {
#if UseTaskParallelLibrary
                Parallel.ForEach(activeVm.ActiveModels,
                    model => data.AddRange(SerializeObject(model.Prefix, model.Model)));
#else
                foreach (var model in activeVm.ActiveModels)
                {
                    data.AddRange(SerializeObject(model.Prefix, model.Model));
                }
#endif

                if (!string.IsNullOrEmpty(activeVm.NavigateToRoute))
                {
                    data.Add("\"stonehenge_navigate\":" + JsonSerializer.SerializeToString(activeVm.NavigateToRoute));
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var name in activeVm.GetDictionaryNames())
                {
                    data.Add($"\"{name}\":{JsonSerializer.SerializeToString(activeVm.TryGetMember(name))}");
                }
            }

            data.AddRange(SerializeObject(null, viewModel));

            var result = "{" + string.Join(",", data) + "}";

            HttpResult httpResult;
            var contentBytes = Encoding.UTF8.GetBytes(result);

            if (!string.IsNullOrEmpty(RequestContext.CompressionType))
            {
                var compressed = new CompressedResult(contentBytes, RequestContext.CompressionType, ViewModelContentType);
                httpResult = new HttpResult(compressed.Contents, ViewModelContentType);
                httpResult.Headers.Add("CompressionType", RequestContext.CompressionType);
            }
            else
            {
                httpResult = new HttpResult(contentBytes, ViewModelContentType);
            }
            if (!appSession.CookieSet)
            {
                httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
            }

            return httpResult;
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

            appSession.Accessed(Request.Cookies, true);
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
                return returnData 
                    ? GetViewModelResult(request, appSession, vm) 
                    : new HttpResult("{}", ViewModelContentType);
            }

            var redirect = new HttpResult { StatusCode = HttpStatusCode.Redirect };
            redirect.Headers.Add("Location", Request.Headers["Referer"] + "#/" + host.Redirect);
            host.Redirect = null;
            return redirect;
        }

        public static Func<object, object> BuildUntypedGetter(Type targetType, PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetGetMethod();

            var exTarget = System.Linq.Expressions.Expression.Parameter(typeof(object), "t");
            var typedTarget = System.Linq.Expressions.Expression.Convert(exTarget, targetType);

            var exBody = System.Linq.Expressions.Expression.Call(typedTarget, methodInfo);
            var exBody2 = System.Linq.Expressions.Expression.Convert(exBody, typeof(object));

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(exBody2, exTarget);
            // t => Convert(t.get_Foo())

            var action = lambda.Compile();
            return action;
        }

        private static IEnumerable<string> SerializeObject(string prefix, object obj)
        {
            var data = new List<string>();
            if (prefix == null)
                prefix = string.Empty;
#if DEBUG
            var time = new Stopwatch();
            time.Start();
#endif
#if UseTaskParallelLibrary
            Parallel.ForEach(obj.GetType().GetProperties(), prop =>
#else
            foreach (var prop in obj.GetType().GetProperties())
#endif

            {

                var bindable = prop.GetCustomAttributes(typeof (BindableAttribute), true);
                if ((bindable.Length > 0) && !((BindableAttribute) bindable[0]).Bindable)
                {
#if UseTaskParallelLibrary
                    return;
#else
                    continue;
#endif
                }

#if DEBUG
                time.Restart();
#endif
                //var value = prop.GetValue(obj, null);

                var get = BuildUntypedGetter(obj.GetType(), prop);
                var value = get(obj);

                //TODO: evaluate fastest method
                //var arg = System.Linq.Expressions.Expression.Parameter(typeof(object), "x");
                //var targ = System.Linq.Expressions.Expression.Convert(arg, obj.GetType());
                //var expr = System.Linq.Expressions.Expression.Property(targ, prop.Name);
                //var resu = System.Linq.Expressions.Expression.Convert(expr, typeof(object));
                //var propertyResolver = (Func<object, object>)System.Linq.Expressions.Expression.Lambda(resu, arg).Compile();
                //var value = propertyResolver(obj);

                if (value == null)
                {
#if UseTaskParallelLibrary
                    return;
#else
                    continue;
#endif
                }

#if DEBUG
                Trace.TraceInformation($"GetValue({prop.Name}) {time.ElapsedMilliseconds}ms");
#endif

                string json;
                if (prop.PropertyType.Name == "GraphOptions")
                {
                    json = "\"" + prefix + prop.Name + "\":" + value;
                }
                else if (prop.PropertyType.IsValueType && !prop.PropertyType.IsPrimitive &&
                         (prop.PropertyType.Namespace != "System")) // struct
                {
                    var structJson = new List<string>();

                    foreach (var member in prop.PropertyType.GetProperties())
                    {
                        var memberValue = member.GetValue(value, null);
                        if (memberValue != null)
                        {
                            json = "\"" + prefix + member.Name + "\":" +
                                   JsonSerializer.SerializeToString(memberValue);
                            structJson.Add(json);
                        }
                    }


                    json = "\"" + prefix + prop.Name + "\": { " + string.Join(",", structJson) + " }";
                }
                else
                {
                    json = "\"" + prefix + prop.Name + "\":" + JsonSerializer.SerializeToString(value);
                }
                data.Add(json);
            }
#if UseTaskParallelLibrary
            );
#endif

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
    }
}