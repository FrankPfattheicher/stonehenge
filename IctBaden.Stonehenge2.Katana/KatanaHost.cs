using System;

namespace IctBaden.Stonehenge2.Katana
{
    using System.Diagnostics;

    using IctBaden.Stonehenge2.Hosting;
    using IctBaden.Stonehenge2.Resources;

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

        public bool Start(string hostAddress, int hostPort, bool useSsl)
        {
            try
            {
                BaseUrl = (useSsl ? "https://" : "http://") + hostAddress + ":" + hostPort;
                var startup = new Startup(resourceLoader);
                webApp = WebApp.Start(BaseUrl, startup.Configuration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
