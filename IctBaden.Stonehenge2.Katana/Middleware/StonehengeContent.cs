using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using Core;
    using Resources;

    using Microsoft.Owin;

    public class StonehengeContent
    {
      private readonly Func<IDictionary<string, object>, Task> _next;

        public StonehengeContent(Func<IDictionary<string, object>, Task> next)
        {
            this._next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var path = context.Request.Path.Value;
            if (path == "/")
            {
                await _next.Invoke(environment);
                return;
            }
            try
            {
                var response = context.Get<Stream>("owin.ResponseBody");
                var resourceLoader = context.Get<IStonehengeResourceProvider>("stonehenge.ResourceLoader");
                var resourceName = path.Substring(1);
                var appSession = context.Get<AppSession>("stonehenge.AppSession");
                var requestVerb = context.Get<string>("owin.RequestMethod");
                var requestCookies = context.Request.Headers
                    .FirstOrDefault(h => h.Key == "Cookie").Value?
                    .SelectMany(c => c.Split(';').Select(s => s.Trim()))
                    .Select(s => s.Split('='));
                var cookies = new Dictionary<string, string>();
                if(requestCookies != null)
                {
                    foreach (var cookie in requestCookies)
                    {
                        if (!cookies.ContainsKey(cookie[0]))
                        {
                            cookies.Add(cookie[0], cookie[1]);
                        }
                    }
                }
                var queryString = HttpUtility.ParseQueryString(context.Get<string>("owin.RequestQueryString"));
                var parameters = queryString.AllKeys
                    .ToDictionary(key => key, key => queryString[key]);

                Resource content = null;
                switch (requestVerb)
                {
                    case "GET":
                        appSession.Accessed(cookies, false);
                        content = resourceLoader.Get(appSession, resourceName, parameters);
                        if (string.Compare(resourceName, "index.html", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            HandleIndexContent(context, content);
                        }
                        break;

                    case "POST":
                        appSession.Accessed(cookies, true);
                        var body = new StreamReader(context.Request.Body).ReadToEndAsync().Result;

                        var formData = new Dictionary<string, string>();
                        if (!string.IsNullOrEmpty(body))
                        {
                            try
                            {
                                formData = JsonConvert.DeserializeObject<JObject>(body).AsJEnumerable().Cast<JProperty>()
                                .ToDictionary(data => data.Name, data => Convert.ToString(data.Value, CultureInfo.InvariantCulture));
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null) ex = ex.InnerException;
                                Trace.TraceError(ex.Message);
                                Trace.TraceError(ex.StackTrace);
                                Debug.Assert(false);
                            }
                        }

                        content = resourceLoader.Post(appSession, resourceName, parameters, formData);
                        break;
                }

                if (content == null)
                {
                    await _next.Invoke(environment);
                    return;
                }
                context.Response.ContentType = content.ContentType;
                switch (content.CacheMode)
                {
                    case Resource.Cache.None:
                        context.Response.Headers.Add("Cache-Control", new[] { "no-cache" });
                        break;
                    case Resource.Cache.Revalidate:
                        context.Response.Headers.Add("Cache-Control", new[] { "max-age=3600", "must-revalidate", "proxy-revalidate" });
                        context.Response.ETag = appSession.AppVersionId;
                        break;
                    case Resource.Cache.OneDay:
                        context.Response.Headers.Add("Cache-Control", new[] { "max-age=86400" });
                        break;
                }
                if (!appSession.StonehengeCookieSet)
                {
                    context.Response.Headers.Add("Set-Cookie", new[] { "stonehenge-id=" + appSession.Id });
                }

                if (content.IsBinary)
                {
                    using (var writer = new BinaryWriter(response))
                    {
                        writer.Write(content.Data);
                    }
                }
                else
                {
                    using (var writer = new StreamWriter(response))
                    {
                        writer.Write(content.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("StonehengeContent write response: " + ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Trace.TraceError(" + " + ex.Message);
                }
            }

        }

        private void HandleIndexContent(IOwinContext context, Resource content)
        {
            const string placeholderAppTitle = "stonehengeAppTitle";
            var appTitle = context.Get<string>("stonehenge.AppTitle");
            content.Text = content.Text.Replace(placeholderAppTitle, appTitle);
        }
    }
}