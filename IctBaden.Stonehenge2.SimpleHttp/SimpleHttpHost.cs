using System;
using System.Diagnostics;

namespace IctBaden.Stonehenge2.SimpleHttp
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Core;
    using Hosting;
    using Resources;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SimpleHttpHost : IStonehengeHost
    {
        private SimpleHttpServer server;

        public string BaseUrl { get; private set; }

        private readonly IResourceProvider resourceLoader;

        public SimpleHttpHost(IResourceProvider loader)
        {
            resourceLoader = loader;
        }

        public bool Start(bool useSsl = false, string hostAddress = null, int hostPort = 0)
        {
            if(useSsl)
                throw new NotSupportedException("SSL not supported.");
            if (hostPort == 0) hostPort = 80;

            BaseUrl = "http://"
                + (hostAddress ?? "127.0.0.1")
                + ":" + hostPort;

            server = new SimpleHttpServer(hostPort);
            server.HandleGet += ServerOnHandleGet;
            server.HandlePost += ServerOnHandlePost;

            server.Start();
            return true;
        }

        public void Terminate()
        {
            server?.Terminate();
        }

        private void ServerOnHandleGet(SimpleHttpProcessor httpProcessor)
        {
            // get session
Debug.Assert(false, "implement session cache");

            if (httpProcessor.HttpUrl == "/")
            {
                httpProcessor.WriteRedirect("/Index.html");
                return;
            }

            var resourceName = httpProcessor.HttpUrl.Substring(1);
            var content = resourceLoader.Get(new AppSession(), resourceName);
            if (content == null)
            {
                httpProcessor.WriteNotFound();
                return;
            }
            httpProcessor.WriteSuccess(content.ContentType);
            if (content.IsBinary)
            {
                httpProcessor.OutputStream.BaseStream.Write(content.Data, 0, content.Data.Length);
            }
            else
            {
                httpProcessor.OutputStream.Write(content.Text);
            }
        }

        private void ServerOnHandlePost(SimpleHttpProcessor httpProcessor, StreamReader streamReader)
        {
            var resourceName = httpProcessor.HttpUrl.Substring(1);
            var queryPart = "";
            var queryIndex = resourceName.IndexOf("?", 0, StringComparison.InvariantCulture);
            if (queryIndex != -1)
            {
                queryPart = resourceName.Substring(queryIndex + 1);
                resourceName = resourceName.Substring(0, queryIndex);
            }
            var body = streamReader.ReadToEnd();
            var formData = JsonConvert.DeserializeObject<JObject>(body).AsJEnumerable().Cast<JProperty>()
                .ToDictionary(data => data.Name, data => Convert.ToString(data.Value, CultureInfo.InvariantCulture));

            var queryString = HttpUtility.ParseQueryString(queryPart);
            var paramObjects = queryString.AllKeys.Select(key => queryString[key]).Cast<object>().ToArray();
            var content = resourceLoader.Post(new AppSession(), resourceName, paramObjects, formData);
            if (content == null)
            {
                httpProcessor.WriteNotFound();
                return;
            }
            httpProcessor.WriteSuccess(content.ContentType);
            if (content.IsBinary)
            {
                httpProcessor.OutputStream.BaseStream.Write(content.Data, 0, content.Data.Length);
            }
            else
            {
                httpProcessor.OutputStream.Write(content.Text);
            }
        }

    }
}
