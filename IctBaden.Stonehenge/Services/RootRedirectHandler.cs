using System;
using System.Net;
using System.Web;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints.Support;

namespace IctBaden.Stonehenge.Services
{
    public class RootRedirectHandler : IServiceStackHttpHandler, IHttpHandler
    {
        public string RelativeUrl { get; set; }
        public bool IsReusable => false;

        private readonly RedirectHttpHandler handler;

        public RootRedirectHandler()
        {
            handler = new RedirectHttpHandler();
        }

        public void ProcessRequest(HttpContext context)
        {
            handler.RelativeUrl = RelativeUrl;
            handler.ProcessRequest(context);
        }

        public void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            var hostdomain = httpReq.QueryString.Get("hostdomain");
            if (!string.IsNullOrEmpty(hostdomain))
            {
                RelativeUrl = RelativeUrl.Replace("#/", "?hostdomain=" + hostdomain + "#/");
            }
            var originalRequest = httpReq.OriginalRequest as HttpListenerRequest;
            if (originalRequest?.UserHostName != null)
            {
                var userHostUri = new Uri($"{originalRequest.Url.Scheme}://{originalRequest.UserHostName}");
                handler.AbsoluteUrl = new Uri(userHostUri, RelativeUrl).ToString();
            }
            else
            {
                handler.RelativeUrl = RelativeUrl;
            }
            handler.ProcessRequest(httpReq, httpRes, operationName);
        }
    }
}