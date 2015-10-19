using System;

namespace IctBaden.Stonehenge2.Katana
{
    using System.Diagnostics;

    using Hosting;
    using Resources;

    using Microsoft.Owin.Hosting;

    public class KatanaHost : IStonehengeHost
    {
        private IDisposable webApp;

        private readonly IResourceProvider resourceLoader;

        public KatanaHost(IResourceProvider loader)
        {
            resourceLoader = loader;
        }

        public string BaseUrl { get; private set; }

        public bool Start(bool useSsl, string hostAddress, int hostPort)
        {
            try
            {
                BaseUrl = (useSsl ? "https://" : "http://") 
                    + (hostAddress ?? "127.0.0.1" )
                    + ":" 
                    + ((hostPort != 0) ? hostPort : (useSsl ? 443 : 80));

                var startup = new Startup(resourceLoader);
                webApp = WebApp.Start(BaseUrl, startup.Configuration);
            }
            catch (Exception ex)
            {
                Trace.TraceError("KatanaHost.Start: " + ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Trace.TraceError(" + " + ex.Message);
                }
            }
            return webApp != null;
        }

        public void Terminate()
        {
            if (webApp != null)
            {
                webApp.Dispose();
                webApp = null;
            }
        }
    }
}
