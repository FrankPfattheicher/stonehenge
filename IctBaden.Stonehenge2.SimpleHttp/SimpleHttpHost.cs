using System;

namespace IctBaden.Stonehenge2.SimpleHttp
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using Core;
    using Hosting;

    using IctBaden.Stonehenge2.Caching;

    using Resources;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SimpleHttpHost : IStonehengeHost
    {
        private SimpleHttpServer server;

        public string AppTitle { get; private set; }
        public string BaseUrl { get; private set; }

        private readonly IStonehengeResourceProvider resourceLoader;
        private readonly IStonehengeSessionCache sessionCache;

        public SimpleHttpHost(IStonehengeResourceProvider loader, IStonehengeSessionCache cache)
        {
            resourceLoader = loader;
            sessionCache = cache;
        }

        public bool Start(string title, bool useSsl = false, string hostAddress = null, int hostPort = 0)
        {
            AppTitle = title;

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
            var sessionId = string.Empty;
            AppSession session = null;
            var cookie = httpProcessor.Headers.FirstOrDefault(h => h.Key == "Cookie" && h.Value.Contains("StonehengeSession="));
            if(!string.IsNullOrEmpty(cookie.Value))
            {
                var extract = new Regex("StonehengeSession=([0-9a-fA-F]+)");
                var match = extract.Match(cookie.Value);
                sessionId = match.Groups[1].Value;
                if (sessionCache.ContainsKey(sessionId))
                    session = sessionCache[sessionId] as AppSession;
            }

            if (session == null)
            {
                session = new AppSession();
                sessionId = session.Id;
                sessionCache.Add(sessionId, session);
            }

            var header = new Dictionary<string, string> { { "Cookie", "StonehengeSession=" + sessionId } };

            if (httpProcessor.Url == "/")
            {
                httpProcessor.WriteRedirect("/Index.html", header);
                return;
            }

            var resourceName = httpProcessor.Url.Substring(1);
            var parameters = new Dictionary<string, string>();  //TODO: extract parameters from URL
            var content = resourceLoader.Get(session, resourceName, parameters);
            if (content == null)
            {
                httpProcessor.WriteNotFound();
                return;
            }
            httpProcessor.WriteSuccess(content.ContentType, header);
            if (content.IsBinary)
            {
                httpProcessor.WriteContent(content.Data);
            }
            else
            {
                httpProcessor.WriteContent(content.Text);
            }
        }

        private void ServerOnHandlePost(SimpleHttpProcessor httpProcessor, StreamReader streamReader)
        {
            var resourceName = httpProcessor.Url.Substring(1);
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
            var paramObjects = queryString.AllKeys
                .ToDictionary(key => key, key => queryString[key]);
            var content = resourceLoader.Post(new AppSession(), resourceName, paramObjects, formData);
            if (content == null)
            {
                httpProcessor.WriteNotFound();
                return;
            }
            httpProcessor.WriteSuccess(content.ContentType);
            if (content.IsBinary)
            {
                httpProcessor.WriteContent(content.Data);
            }
            else
            {
                httpProcessor.WriteContent(content.Text);
            }
        }

    }
}
