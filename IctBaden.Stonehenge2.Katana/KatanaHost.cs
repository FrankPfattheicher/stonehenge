using System;
using System.Net;

namespace IctBaden.Stonehenge2.Katana
{
    using System.Diagnostics;

    using Hosting;
    using Resources;

    using Microsoft.Owin.Hosting;

    public class KatanaHost : IStonehengeHost
    {
        private IDisposable _webApp;

        private readonly IStonehengeResourceProvider _resourceLoader;

        public KatanaHost(IStonehengeResourceProvider loader)
        {
            _resourceLoader = loader;
        }

        public string AppTitle { get; private set; }

        public string BaseUrl { get; private set; }

        public bool Start(string title, bool useSsl, string hostAddress, int hostPort)
        {
            AppTitle = title;

            try
            {
                BaseUrl = (useSsl ? "https://" : "http://") 
                    + (hostAddress ?? "*" )
                    + ":" 
                    + (hostPort != 0 ? hostPort : (useSsl ? 443 : 80));

                var startup = new Startup(title, _resourceLoader);
                _webApp = WebApp.Start(BaseUrl, startup.Configuration);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException as HttpListenerException;
                if ((inner != null) && (inner.ErrorCode == 5))
                {
                    Trace.TraceError($"Access denied: Try netsh http delete urlacl {BaseUrl}");
                }
                else if(ex is MissingMemberException && ex.Message.Contains("Microsoft.Owin.Host.HttpListener"))
                {
                    Trace.TraceError("Missing reference to nuget package 'Microsoft.Owin.Host.HttpListener'");
                }

                var message = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += Environment.NewLine + "    " + ex.Message;
                }
                Trace.TraceError("KatanaHost.Start: " + message);
                _webApp = null;
            }
            return _webApp != null;
        }

        public void Terminate()
        {
            _webApp?.Dispose();
            _webApp = null;
        }
    }
}
