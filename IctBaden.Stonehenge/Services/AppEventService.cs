using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Text;

namespace IctBaden.Stonehenge.Services
{
    public class AppEventService : AppService
    {
        private TimeSpan EventTimeout
        {
            get
            {
                var host = GetResolver() as AppHost;
                return host?.EventTimeout ?? TimeSpan.FromSeconds(10);
            }
        }

        public object Get(AppEvent request)
        {
#if DEBUG
            var duration = new Stopwatch();
            duration.Start();
            Trace.TraceInformation("GET(ev) " + Request.AbsoluteUri);

            var result = GetInternal(request);

            duration.Stop();
            Trace.TraceInformation($"GET(ev) {duration.ElapsedMilliseconds}ms");

            return result;
        }

        private object GetInternal(AppEvent request)
        {
#endif
            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            if (appSession == null)
            {
                return new HttpResult()
                {
                    StatusCode = HttpStatusCode.SeeOther,
                    Headers = { { HttpHeaders.Location, Request.UrlReferrer.AbsoluteUri } }
                };
                //return new HttpResult("No view for event request", HttpStatusCode.NotFound);
            }
            var currentParameters = Request.QueryString.AllKeys
                .ToDictionary(key => key, key => Request.QueryString.Get(key));
            appSession.Accessed(Request.Cookies, currentParameters, false);
            appSession.EventPollingActive.Start((long)EventTimeout.TotalMilliseconds * 2);

            Debug.WriteLine("EventService:" + request.ViewModel);

            appSession.EventRelease.WaitOne(EventTimeout);
            // wait for maximum 500ms for more events - if there is none within 100ms - continue
            var max = 5;
            while (appSession.EventRelease.WaitOne(100) && (max > 0))
            {
                max--;
            }

            var values = new Dictionary<string, object>();
            var vm = appSession.ViewModel as ActiveViewModel;
            if ((vm == null) || (vm.GetType().FullName != request.ViewModel) || !vm.SupportsEvents)
            {
                return new HttpResult("{}", "application/json");
            }
            lock (appSession.Events)
            {
                values.Add("stonehenge_poll", 1);
                foreach (var name in appSession.Events.Where(name => !string.IsNullOrEmpty(name) && !values.ContainsKey(name)))
                {
                    if (name == PropertyNameMessageBox)
                    {
                        var title = vm.MessageBoxTitle;
                        var text = vm.MessageBoxText;
                        // ReSharper disable once UseStringInterpolation
                        values.Add("stonehenge_eval", string.Format("app.showMessage('{0}','{1}');", HttpUtility.JavaScriptStringEncode(text), HttpUtility.JavaScriptStringEncode(title)));
                    }
                    else if (name == PropertyNameNavigate)
                    {
                        var route = vm.NavigateToRoute;
                        values.Add("stonehenge_navigate", route);
                    }
                    else if (name == PropertyNameClientScript)
                    {
                        var script = vm.ClientScript;
                        values.Add("stonehenge_eval", script);
                    }
                    else
                    {
                        var pi = vm.GetPropertyInfo(name);
                        if (pi == null)
                        {
                            var value = vm.TryGetMember(name);
                            if (value != null)
                            {
                                values.Add(name, value);
                            }
                            continue;
                        }
                        var bindable = pi.GetCustomAttributes(false).OfType<BindableAttribute>().ToArray();
                        if ((bindable.Length > 0) && !bindable[0].Bindable)
                            continue;
                        values.Add(name, vm.TryGetMember(name));
                    }
                }
                appSession.Events.Clear();
                appSession.EventRelease.Reset();
            }

            HttpResult httpResult;
            if (!string.IsNullOrEmpty(RequestContext.CompressionType))
            {
                var properties = new List<string>();
                foreach (var value in values)
                {
                    if (value.Value == null)
                        continue;
                    var txt = JsonSerializer.SerializeToString(value.Value);
                    if (txt.StartsWith("\"{") && txt.EndsWith("}\""))
                        txt = txt.Substring(1, txt.Length - 2).Replace("\\\"", "\"");
                    properties.Add('"' + value.Key + '"' + ": " + txt);
                }
                var data = "{" + string.Join(",", properties) + "}";

                var compressed = new CompressedResult(data.Compress(RequestContext.CompressionType), RequestContext.CompressionType) { ContentType = "application/json" };
                httpResult = new HttpResult(compressed.Contents, "application/json");
                foreach (var header in compressed.Headers)
                {
                    httpResult.Headers.Add(header.Key, header.Value);
                }
                httpResult.Headers.Add("Cache-Control", "no-cache, max-age=0");
            }
            else
            {
                httpResult = new HttpResult(values, "application/json");
            }
            if (!appSession.CookieSet)
            {
                httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
            }
            return httpResult;
        }

        public object Any()
        {
            var message = "AppEventService: No handler for HTTP method: " + Request.HttpMethod;
            Trace.TraceWarning(message);
            return new HttpResult(message, HttpStatusCode.NotFound);
        }

    }
}