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
        readonly Func<IDictionary<string, object>, Task> next;

        public StonehengeContent(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var path = context.Request.Path.Value;
            if (path == "/")
            {
                await next.Invoke(environment);
                return;
            }

            var response = context.Get<Stream>("owin.ResponseBody");
            var resourceLoader = context.Get<IStonehengeResourceProvider>("stonehenge.ResourceLoader");
            var resourceName = path.Substring(1);
            var appSession = context.Get<AppSession>("stonehenge.AppSession");
            var requestVerb = context.Get<string>("owin.RequestMethod");

            Resource content = null;
            switch (requestVerb)
            {
                case "GET":
                    content = resourceLoader.Get(appSession, resourceName);
                    break;
                case "POST":
                    var body = new StreamReader(context.Request.Body).ReadToEndAsync().Result;
                    var formData = JsonConvert.DeserializeObject<JObject>(body).AsJEnumerable().Cast<JProperty>()
                        .ToDictionary(data => data.Name, data => Convert.ToString(data.Value, CultureInfo.InvariantCulture));

                    //var formData = context.Request.ReadFormAsync().Result
                    //    .ToDictionary(data => data.Key, data => data.Value.FirstOrDefault());

                    var queryString = HttpUtility.ParseQueryString(context.Get<string>("owin.RequestQueryString"));
                    var paramObjects = queryString.AllKeys.Select(key => queryString[key]).Cast<object>().ToArray();
                    content = resourceLoader.Post(appSession, resourceName, paramObjects, formData);
                    break;
            }

            if (content == null)
            {
                await next.Invoke(environment);
                return;
            }
            context.Response.ContentType = content.ContentType;
            try
            {
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
    }
}